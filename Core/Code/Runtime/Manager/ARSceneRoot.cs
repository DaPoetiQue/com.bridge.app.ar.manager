using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Bridge.Core.Debug;
using Bridge.Core.App.Events;
using Bridge.Core.App.Content.Manager;

namespace Bridge.Core.App.AR.Manager
{
    public class ARSceneRoot : MonoDebug
    {
        #region Instance

        private static ARSceneRoot instance;

        public static ARSceneRoot Instance
        {
            get
            {
                if (instance == false)
                {
                    instance = FindObjectOfType<ARSceneRoot>();
                }

                return instance;
            }
        }

        #endregion

        #region Components

        [HideInInspector]
        public ARSceneFocusData sceneFocusData;

        private Camera sceneEventCamera = null;

        private ARRaycastManager rayCastManager;

        private ARSceneFocusHandler focusHandler = null;

        public ARSceneFocusData SceneFocusData
        {
            get { return sceneFocusData; }
            set { sceneFocusData = value; }
        }

        private Vector3 screenCenter = Vector3.zero;
        private List<ARRaycastHit> results = new List<ARRaycastHit>();

        #endregion

        #region Unity

        private void Start() => Init();

        private void Update() => Tracking();

        #endregion

        #region Initializations

        private void Init()
        {
            if (sceneFocusData == null)
            {
                Log(LogLevel.Warning, this, "Scene focus data not assigned. Focus icon won't appear in the scene tracking state.");
            }

            if (sceneFocusData != null)
            {
                screenCenter = GetScreenCenter(sceneFocusData.focusDistance);

                Log(LogLevel.Debug, this, $"Screen Center : {screenCenter}");

                focusHandler = this.GetComponentInChildren<ARSceneFocusHandler>();
                focusHandler.Init(sceneFocusData);
            }

            sceneEventCamera = this.GetComponentInChildren<Camera>();
            rayCastManager = this.GetComponentInChildren<ARRaycastManager>();
        }

        private void Tracking()
        {
            SceneTracked(screenCenter, (tracked, pose) =>
            { 
                if(tracked.success == true)
                {
                    focusHandler.Log(LogLevel.Debug, this, "Updating focus icons.");
                    focusHandler.SetFocusIconPose(FocusType.Found, pose);

                    Log(LogLevel.Debug, this, tracked.successValue);
                }
                else
                {
                    focusHandler.Log(LogLevel.Debug, this, "Seaching focus icons.");
                    focusHandler.SetFocusIconPose(FocusType.None, pose);
                }
            });
        }

        #endregion

        #region Main

        private void SceneTracked(Vector3 point, Action<AppEventsData.CallBackResults, Content.Manager.Pose> callback = null)
        {
            Ray ray = sceneEventCamera.ScreenPointToRay(point);

            AppEventsData.CallBackResults callbackResults = new AppEventsData.CallBackResults();

            if(rayCastManager.Raycast(ray, results, sceneFocusData.trackableType))
            {
                var pose = new Content.Manager.Pose { position = results[0].pose.position, rotation = results[0].pose.rotation };

                callbackResults.success = true;
                callbackResults.successValue = "Scene tracking.";

                callback.Invoke(callbackResults, pose);
            }
            else
            {
                callbackResults.error = true;
                callbackResults.success = false;
            }
        }

        public Vector3 GetScreenCenter(float trackingDistance)
        {
            return new Vector3(Screen.width / 2, Screen.height / 2, trackingDistance);
        }

        #endregion
    }
}
