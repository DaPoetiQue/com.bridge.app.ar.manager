using System;
using UnityEngine;
using UnityEditor;
using Bridge.Core.App.AR.Manager;
using UnityEngine.XR.ARFoundation;
using Bridge.App.Serializations.Manager;

namespace Bridge.Core.UnityEditor.AR.Manager
{
    public class ARSceneRootEditor : Editor
    {
        #region

        public static Camera arSceneEventCamera = null;
        public static Light arSceneLight = null;

        public static bool usedExistingSceneEventCamera;
        public static bool usedExistingSceneLight;

        private static SceneEventCameraSettings previousEventCameraSettings;

        #endregion

        #region Initialize AR Content Manager

        public static void CreateNewARSceneRoot(string nameTag, ARSceneRootSettings settings, ARSceneRootContent arSceneContent, Action<SceneEventCameraSettings, StorageData.CallBackResults> callback = null)
        {
            var callBackResults = new StorageData.CallBackResults();

            GameObject arSceneManager = new GameObject(nameTag);
            arSceneManager.AddComponent<ARSceneRoot>();

            if (Selection.gameObjects.Length > 0 && !Selection.gameObjects[0].GetComponent<Camera>())
            {
                arSceneManager.transform.SetParent(Selection.gameObjects[0].transform);
            }

            GameObject arRoot = new GameObject("_AR Scene Root");
            arRoot.transform.SetParent(arSceneManager.transform);

            GameObject arSession = new GameObject("AR Scene Session");
            arSession.AddComponent<ARSession>();
            arSession.AddComponent<ARInputManager>();
            arSession.transform.SetParent(arRoot.transform);

            GameObject arSessionOrigin = new GameObject("_AR Scene Session Origin");
            ARSessionOrigin sessionOrigin = arSessionOrigin.AddComponent<ARSessionOrigin>();
            ARPlaneManager planeManager = arSessionOrigin.AddComponent<ARPlaneManager>();
            arSessionOrigin.AddComponent<ARRaycastManager>();
            planeManager.planePrefab = Resources.Load<GameObject>("AR Data/3ridge AR Scene Plane");
            arSessionOrigin.transform.SetParent(arRoot.transform);

            #region Scene Camera

            Camera sceneEventCam = null;

            if (arSceneContent.createEventCamera == false)
            {
                sceneEventCam = FindObjectOfType<Camera>();
            }

            if (arSceneContent != null && arSceneContent.createEventCamera == false)
            {
                if (sceneEventCam == null)
                {
                    callBackResults.error = true;
                    callBackResults.errorValue = "-->> <color=white>Scene Event Camera Critical Warning :</color> <color=orange>There is no camera found in the current scene.</color> <color=white>Create a new camera or enable</color> <color=cyan>'Create Event Camera'</color> <color=white>variable when creating an AR Scene  Root template.</color>";

                    callBackResults.success = false;
                    callBackResults.successValue = "Null";
                }

                arSceneEventCamera = sceneEventCam;
                usedExistingSceneEventCamera = true;
            }

            if(sceneEventCam == null && arSceneContent != null && arSceneContent.createEventCamera == true)
            {
                GameObject arCamera = new GameObject(nameTag);
                arSceneEventCamera = arCamera.AddComponent<Camera>();
                usedExistingSceneEventCamera = false;
            }

            if(arSceneEventCamera != null)
            {
                #region Store Default Cam Settings

                previousEventCameraSettings.nameTag = arSceneEventCamera.name;

                previousEventCameraSettings.clearFlags = arSceneEventCamera.clearFlags;
                previousEventCameraSettings.cullingMask = arSceneEventCamera.cullingMask;
                previousEventCameraSettings.cameraType = arSceneEventCamera.cameraType;
                previousEventCameraSettings.backgroundColor = arSceneEventCamera.backgroundColor;

                previousEventCameraSettings.usePhysicalProperties = arSceneEventCamera.usePhysicalProperties;
                previousEventCameraSettings.depth = arSceneEventCamera.depth;

                previousEventCameraSettings.fieldOfView = arSceneEventCamera.fieldOfView;
                previousEventCameraSettings.nearClipPlane = arSceneEventCamera.nearClipPlane;
                previousEventCameraSettings.farClipPlane = arSceneEventCamera.farClipPlane;

                // Setting Serializable Position
                previousEventCameraSettings.position.x = arSceneEventCamera.transform.localPosition.x;
                previousEventCameraSettings.position.y = arSceneEventCamera.transform.localPosition.y;
                previousEventCameraSettings.position.z = arSceneEventCamera.transform.localPosition.z;

                // Setting Serializable Rotation
                previousEventCameraSettings.rotation.x = arSceneEventCamera.transform.localRotation.x;
                previousEventCameraSettings.rotation.y = arSceneEventCamera.transform.localRotation.y;
                previousEventCameraSettings.rotation.z = arSceneEventCamera.transform.localRotation.z;
                previousEventCameraSettings.rotation.w = arSceneEventCamera.transform.localRotation.w;

                // previousEventCameraSettings.parent = arSceneEventCamera.transform.parent;

                #endregion

                #region Set Current Camera Settings

                arSceneEventCamera.name = "AR Scene Event Cam";

                arSceneEventCamera.clearFlags = CameraClearFlags.SolidColor;
                arSceneEventCamera.cameraType = CameraType.Game;
                arSceneEventCamera.backgroundColor = Color.black;

                arSceneEventCamera.usePhysicalProperties = false;
                arSceneEventCamera.depth = -1; // TODO : Test this in AR.

                arSceneEventCamera.fieldOfView = 60.0f;
                arSceneEventCamera.nearClipPlane = 0.01f;
                arSceneEventCamera.farClipPlane = 150.0f;

                arSceneEventCamera.transform.SetParent(sessionOrigin.transform);
                arSceneEventCamera.transform.localPosition = Vector3.zero;
                arSceneEventCamera.transform.localRotation = Quaternion.identity;

                ARCameraManager cameManager = arSceneEventCamera.gameObject.AddComponent<ARCameraManager>();
                arSceneEventCamera.gameObject.AddComponent<ARPoseDriver>();
                arSceneEventCamera.gameObject.AddComponent<ARCameraBackground>();
                cameManager.requestedLightEstimation = settings.estimatedLighting;

                sessionOrigin.camera = arSceneEventCamera;

                if(sessionOrigin.camera == null)
                {
                    callBackResults.error = true;
                    callBackResults.errorValue = "-->> <color=white>Scene Event Camera Critical Warning :</color> <color=orange>There is no camera assigned to [Session Origin] in the current scene.</color> <color=white>Create a new camera or enable</color> <color=cyan>'Create Event Camera'</color> <color=white>variable when creating an AR Scene  Root template.</color>";

                    callBackResults.success = false;
                    callBackResults.successValue = string.Empty;
                }

                if (sessionOrigin.camera != null)
                {
                    callBackResults.success = true;
                    callBackResults.successValue = "-->> <color=green>Success</color> - <color=white> AR scene root camera has been successfully assigned to the session origin</color>";

                    callBackResults.error = false;
                    callBackResults.errorValue = string.Empty;
                }

                #endregion
            }

            #endregion

            #region Scene Lighting

            GameObject arSceneLight = new GameObject("AR Scene Light");
            arSceneLight.AddComponent<ARSceneLight>();
            arSceneLight.transform.SetParent(sessionOrigin.transform);
            Light sceneLight = arSceneLight.AddComponent<Light>();
            sceneLight.type = settings.sceneLightType;
            sceneLight.shadowNearPlane = arSceneEventCamera.nearClipPlane;
            sceneLight.shadows = settings.lightShadowType;

            #endregion

            callback.Invoke(previousEventCameraSettings, callBackResults);
        }

        #endregion

        public static void UpdateARSceneRoot(string nameTag, ARSceneRootSettings settings, ARSceneRootContent arSceneContent = null, Action<bool> callback = null)
        {
            callback.Invoke(true);
        }

        #region Previous Settings

        public static void SetPreviousEventCamSettings(SceneEventCameraSettings sceneEventCameraSettings)
        {
            previousEventCameraSettings = sceneEventCameraSettings;
        }

        public static SceneEventCameraSettings GetPreviousEventCamSettings()
        {
            return previousEventCameraSettings;
        }

        #endregion
    }
}
