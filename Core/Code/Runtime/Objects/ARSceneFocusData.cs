using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;

namespace Bridge.Core.App.AR.Manager
{
    [CreateAssetMenu(fileName = "AR Focus Data", menuName = "3ridge/AR/Focus Data")]
    public class ARSceneFocusData : ScriptableObject
    {
        [Space(5)]
        public List<FocusIcon> focusIcon;

        [Space(5)]
        public TrackableType trackableType;

        [Space(5)]
        public float focusDistance;
    }

    [Serializable]
    public struct FocusIcon
    {
        [Space(5)]
        public string nameTag;

        [Space(5)]
        public GameObject prefab;

        [Space(5)]
        public FocusType type;
    }

    public enum FocusType
    {
        None, 
        Finding, 
        Found
    }

    #region Tracking Data

    public enum FocusIconTransitionalType
    {
        Default,
        Smooth
    }

    public enum FocusIconPoseType
    {
        PositionOnly, 
        PositionAndRotation
    }

    #endregion
}
