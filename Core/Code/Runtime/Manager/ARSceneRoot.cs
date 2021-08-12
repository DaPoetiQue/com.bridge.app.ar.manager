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

        #endregion

        #region Main

        private void Tracking()
        {
            if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                var pose = new Content.Manager.Pose();

                if (SceneTracked(screenCenter, out pose))
                {
                    focusHandler.SetFocusIconPose(FocusType.Found, pose);
                    Log(LogLevel.Debug, this, $"Pos : {pose.position} - Rot : {pose.rotation} - Scene Tracked");
                }
                else
                {
                    focusHandler.SetFocusIconPose(FocusType.Finding, pose);
                    Log(LogLevel.Warning, this, $"Pos : {pose.position} - Rot : {pose.rotation} - Scene Tracking...");
                }
            }
        }

        private bool SceneTracked(Vector3 screenCenter, out Content.Manager.Pose pose)
        {
            if (rayCastManager.Raycast(screenCenter, results, sceneFocusData.trackableType))
            {
                pose = new Content.Manager.Pose { position = results[0].pose.position, rotation = results[0].pose.rotation };
                return true;
            }

            pose = new Content.Manager.Pose();
            return false;
        }

        public Vector3 GetScreenCenter(float trackingDistance)
        {
            return new Vector3(Screen.width / 2, Screen.height / 2, trackingDistance);
        }

        #endregion
    }
}
