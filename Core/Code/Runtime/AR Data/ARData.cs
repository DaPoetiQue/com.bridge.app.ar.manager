using System;
using UnityEngine;

namespace Bridge.Core.App.AR.Manager
{
    [Serializable]
    public class ARData
    {
        #region Enums

        public enum FocusType
        {
            None, Finding, Found
        }

        #endregion

        #region Structs

        public struct FocusData
        {
            public GameObject icon;

            [Space(3)]
            public FocusType focusType;
        }

        #endregion

        #region Classes

        #endregion
    }
}
