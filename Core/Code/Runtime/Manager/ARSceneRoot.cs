using UnityEngine;
using Bridge.Core.Debug;

namespace Bridge.Core.App.AR.Manager
{
    public class ARSceneRoot : MonoDebug
    {
        #region Components

        [SerializeField]
        private ARSceneFocusHandler focusHandler = null;

        #endregion
    }
}
