using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.XR.ARFoundation;
using Bridge.Core.Debug;
using Bridge.Core.UnityEditor.Debug;
using Bridge.Core.App.AR.Manager;
using Bridge.Core.App.Events;

namespace Bridge.Core.UnityEditor.AR.Manager
{
    public class ARSceneRootEditor : Editor
    {
        #region

        public static Camera sceneRootCamera = null;
        public static Light arSceneLight = null;

        private static SceneCameraData.SerializableCameraData serializableCameraData;
        private static SceneCameraData.NoneSerializableCameraData sceneCameraData;

        #endregion

        #region Initialize AR Content Manager

        /// <summary>
        /// This function creates a new ar scene root.
        /// </summary>
        /// <param name="nameTag"></param>
        /// <param name="settings"></param>
        /// <param name="sceneRootObject"></param>
        /// <param name="callback"></param>
        public static void CreateARSceneRoot(SceneRootObject sceneRootObject, Action<SceneCameraData.SerializableCameraData, AppEventsData.CallBackResults> callback = null)
        {
            var callBackResults = new AppEventsData.CallBackResults();

            #region Scene Objects

            GameObject arSceneRoot = new GameObject(sceneRootObject.content.nameTag);
            var focusData = arSceneRoot.AddComponent<ARSceneRoot>();
            focusData.SceneFocusData = sceneRootObject.settings.sceneFocusData;

            if (Selection.gameObjects.Length > 0 && !Selection.gameObjects[0].GetComponent<Camera>())
            {
                arSceneRoot.transform.SetParent(Selection.gameObjects[0].transform);
            }

            GameObject sceneRoot = new GameObject("_AR Scene Root");
            sceneRoot.transform.SetParent(arSceneRoot.transform);

            GameObject arSession = new GameObject("AR Scene Session");
            arSession.AddComponent<ARSession>();
            arSession.AddComponent<ARInputManager>();
            arSession.transform.SetParent(sceneRoot.transform);

            GameObject arSessionOrigin = new GameObject("_AR Scene Session Origin");
            ARSessionOrigin sessionOrigin = arSessionOrigin.AddComponent<ARSessionOrigin>();
            ARPlaneManager planeManager = arSessionOrigin.AddComponent<ARPlaneManager>();
            arSessionOrigin.AddComponent<ARRaycastManager>();
            planeManager.planePrefab = Resources.Load<GameObject>("AR Data/3ridge AR Scene Plane");
            arSessionOrigin.transform.SetParent(sceneRoot.transform);

            ARCameraManager cameraManager = null;

            CreateRootCamera(sessionOrigin.transform, sceneRootObject.content.createRootCamera, ref cameraManager, (camera, results) =>
            { 
                if(results.error)
                {
                    callBackResults.error = true;
                    callBackResults.errorValue = results.errorValue;

                    DebugConsole.Log(LogLevel.Error, results.errorValue);
                }

                if(results.success)
                {
                    camera.transform.SetParent(sessionOrigin.transform, false);
                    sessionOrigin.camera = camera;

                    callBackResults.success = true;
                    callBackResults.successValue = results.successValue;

                    cameraManager = camera.gameObject.GetComponent<ARCameraManager>();

                    DebugConsole.Log(LogLevel.Success, results.successValue);
                }  
            });

            #endregion

            #region Scene Lighting

            GameObject arSceneLight = new GameObject("AR Scene Light");
            arSceneLight.transform.SetParent(sessionOrigin.transform);
            arSceneLight.transform.localRotation = Quaternion.Euler(75.0f, 45.0f, 0.0f);
            var sceneLight = arSceneLight.AddComponent<Light>();
            arSceneLight.AddComponent<ARSceneLightEstimation>().cameraManager = cameraManager;
            sceneLight.type = LightType.Directional;
            sceneLight.shadows = sceneRootObject.settings.lightShadowType;
            //sceneLight.shadowNearPlane = sceneEventCam.nearClipPlane;
            sceneLight.shadows = sceneRootObject.settings.lightShadowType;

            #endregion

            #region Focus Data

            if(sceneRootObject.settings.sceneFocusData != null)
            {
                GameObject scenefocusHandler = new GameObject("AR Scene Focus Handler");
                scenefocusHandler.AddComponent<ARSceneFocusHandler>();
                scenefocusHandler.transform.SetParent(sessionOrigin.transform);
            }

            #endregion

            callback.Invoke(serializableCameraData, callBackResults);
        }

        /// <summary>
        /// This functions creates a new ar scene root camera with a call back.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="createCamera"></param>
        /// <param name="callBack"></param>
        private static void CreateRootCamera(Transform parentObject, bool createCamera, ref ARCameraManager cameraManager, Action<Camera, AppEventsData.CallBackResults> callBack = null)
        {
            try
            {
                var callBackResults = new AppEventsData.CallBackResults();

                if (createCamera)
                {
                    CreateNewCamera(parentObject, (createdCamera, createCameraResults) =>
                    {
                        if (createCameraResults.error == true)
                        {
                            callBackResults.error = createCameraResults.error;
                            callBackResults.errorValue = createCameraResults.errorValue;

                            DebugConsole.Log(LogLevel.Error, createCameraResults.errorValue);
                        }

                        if(createCameraResults.success == true)
                        {
                            createdCamera = ARCamera.GetSettings(createdCamera);

                            callBackResults.success = createCameraResults.success;
                            callBackResults.successValue = createCameraResults.successValue;

                            DebugConsole.Log(LogLevel.Success, createCameraResults.successValue);
                        }

                        callBack.Invoke(createdCamera, callBackResults);
                    });
                }
                else
                {
                    Camera createdCamera = (Camera.main == null) ? FindObjectOfType<Camera>() : Camera.main;

                    if (createdCamera == null)
                    {
                        callBackResults.error = true;
                        callBackResults.errorValue = "There is no camera found in the scene.";
                    }

                    if(createdCamera != null)
                    {
                        callBackResults.success = true;
                        callBackResults.successValue = $"Found and updated a camera named : {createdCamera.name} in the scene.";

                        createdCamera.name = "AR Scene Camera";
                        createdCamera = ARCamera.GetSettings(createdCamera);
                        createdCamera.transform.SetParent(parentObject, false);
                    }

                    callBack.Invoke(createdCamera, callBackResults);
                }
            }
            catch(Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Thsi function creates and returns a new ar scene root camera.
        /// </summary>
        /// <param name="callBack"></param>
        /// <returns></returns>
        private static void CreateNewCamera(Transform parentObject, Action<Camera ,AppEventsData.CallBackResults> callBack)
        {
            var callBackResults = new AppEventsData.CallBackResults();

            GameObject rootCameraObject = new GameObject("AR Scene Camera");
            rootCameraObject.transform.SetParent(parentObject, false);
            Camera createdCamera = rootCameraObject.AddComponent<Camera>();

            if(FindObjectOfType<Camera>().name == createdCamera.name)
            {
                callBackResults.success = true;
                callBackResults.successValue = "A new camera has been created in the ar session origin.";
            }
            else
            {
                callBackResults.error = true;
                callBackResults.errorValue = "Failed to create a new ar scene root camera.";
            }

            callBack.Invoke(createdCamera, callBackResults);
        }

        /// <summary>
        /// This function returns the currently set scene root camera.
        /// </summary>
        /// <returns></returns>
        public static Camera GetSceneRootCamera()
        {
            if(sceneRootCamera == null)
            {
                UnityEngine.Debug.LogWarning("-->> There is no scene root camera found. Returning Null.");
            }

            return sceneRootCamera;
        }

        /// <summary>
        /// Resets the camera to its default state.
        /// </summary>
        /// <param name="rootCamera"></param>
        public static void ResetSceneRootCamera(Camera rootCamera)
        {

        }

        private static void SerializeCameraData(SceneCameraData.NoneSerializableCameraData sceneCameraData, Action<SceneCameraData.SerializableCameraData, AppEventsData.CallBackResults> callBack = null)
        {
            var serializedCamera = new SceneCameraData.SerializableCameraData();
            var results = new AppEventsData.CallBackResults();


            callBack.Invoke(serializedCamera, results);
        }

        #endregion

        public static void UpdateARSceneRoot(string nameTag, SceneRootBuilderData settings, SceneRootBuilderData arSceneContent, Action<bool> callback = null)
        {
            callback.Invoke(true);
        }

        #region Previous Settings

        public static void SetPreviousEventCamSettings(SceneCameraData.SerializableCameraData sceneEventCameraSettings)
        {
            serializableCameraData = sceneEventCameraSettings;
        }

        public static SceneCameraData.SerializableCameraData GetPreviousEventCamSettings()
        {
            return serializableCameraData;
        }

        #endregion
    }
}
