using System;
using UnityEngine;
using UnityEditor;
using Bridge.Core.App.AR.Manager;
using UnityEngine.XR.ARFoundation;
using Bridge.Core.App.Events;
using Bridge.Core.Debug;

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
            arSceneRoot.AddComponent<ARSceneRoot>();

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
            //sessionOrigin.camera = CreateRootCamera(sceneRootObject.content.createRootCamera);
            ARPlaneManager planeManager = arSessionOrigin.AddComponent<ARPlaneManager>();
            arSessionOrigin.AddComponent<ARRaycastManager>();
            planeManager.planePrefab = Resources.Load<GameObject>("AR Data/3ridge AR Scene Plane");
            arSessionOrigin.transform.SetParent(sceneRoot.transform);

            #endregion

            #region Scene Lighting

            GameObject arSceneLight = new GameObject("AR Scene Light");
            arSceneLight.AddComponent<ARSceneLight>();
            arSceneLight.transform.SetParent(sessionOrigin.transform);
            Light sceneLight = arSceneLight.AddComponent<Light>();
            sceneLight.type = sceneRootObject.settings.sceneLightType;
            //sceneLight.shadowNearPlane = sceneEventCam.nearClipPlane;
            sceneLight.shadows = sceneRootObject.settings.lightShadowType;

            #endregion

            callback.Invoke(serializableCameraData, callBackResults);
        }

        /// <summary>
        /// This functions create a new ar scene root camera
        /// </summary>
        /// <returns></returns>
        public static Camera SetupRootCamera(Transform parent, bool createCamera)
        {
            if(createCamera)
            {
                CreateRootCamera((createdCamera, createCameraResults) => 
                {
                    if(createCameraResults.success == true)
                    {
                        // DebugLogger.Log(LogData.LogLevel.)
                    }
                });
            }
            else
            {
                sceneRootCamera = Camera.main;


            }

            return sceneRootCamera;
        }

        /// <summary>
        /// Thsi function creates and returns a new ar scene root camera.
        /// </summary>
        /// <param name="callBack"></param>
        /// <returns></returns>
        private static void CreateRootCamera(Action<Camera ,AppEventsData.CallBackResults> callBack)
        {
            var callBackResults = new AppEventsData.CallBackResults();

            Camera rootCamera = new Camera();
            rootCamera.name = "AR Root Camera";
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
