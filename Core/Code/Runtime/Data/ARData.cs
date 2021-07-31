using JetBrains.Annotations;
using System;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace Bridge.Core.App.AR.Manager
{
    #region Enums

    public enum ARSceneTempType
    {
    }

    #endregion

    #region Settings

    public class ARSceneRootObject : ScriptableObject
    {
        public ARSceneRootContent content;
        public ARSceneRootSettings settings;
    }

    [CanBeNull]
    [Serializable]
    public class ARSceneRootContent
    {
        public string nameTag;

        #region Scene Display

        [Header("Scene Display")]
        [Space(15)]
        public bool createEventCamera;

        #endregion

        #region Scene Components

        [Space(15)]
        public GameObject SceneTrackingPlanePrefab;

        #endregion
    }

    [Serializable]
    public struct ARSceneRootSettings
    {
        #region Lighting

        [Header("Scene Lighting")]
        [Space(15)]
        public LightEstimation estimatedLighting;

        [Space(5)]
        public LightType sceneLightType;

        [Space(5)]
        public LightShadows lightShadowType;

        #endregion

        #region Focus Handler

        [Space(5)]
        public ARSceneFocusHandler focusHandler;

        #endregion
    }

    [Serializable]
    public struct SceneEventCameraSettings
    {
        public string nameTag;
        public CameraClearFlags clearFlags;
        public CameraType cameraType;
        public Color backgroundColor;
        public int cullingMask;

        public bool usePhysicalProperties;

        public float depth;
        public float fieldOfView;
        public float nearClipPlane;
        public float farClipPlane;

        public Vector3 position;
        public Vector3 scale;
        public Quaternion rotation;
    }

    #endregion

    #region Objects

    public class ARSceneContent : ScriptableObject
    {
        public string nameTag;
    }

    #endregion
}
