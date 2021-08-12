using UnityEngine;
using Bridge.Core.Debug;
using System.Collections.Generic;
using Bridge.Core.App.Content.Manager;

namespace Bridge.Core.App.AR.Manager
{
    public class ARSceneFocusHandler : MonoDebug
    {
        #region Components

        [SerializeField]
        private List<FocusIcon> focusIcons = new List<FocusIcon>();
        private int focusDataCount = 0;

        #endregion

        #region Initialization

        public void Init(ARSceneFocusData sceneFocusData)
        {
            if (sceneFocusData.focusIcon.Count <= 0)
            {
                Log(LogLevel.Warning, this, "There are no AR focus data found.");
                return;
            }

            foreach (var data in sceneFocusData.focusIcon)
            {
                if(data.prefab == null)
                {
                    Log(LogLevel.Warning, this, $"There is no focus icon prefab assigned for focus data at index : <color=cyan>{sceneFocusData.focusIcon.IndexOf(data)}</color>");
                    return;
                }

                FocusIcon icon = new FocusIcon { nameTag = data.type.ToString(), type = data.type};
      
                if (focusIcons.Contains(icon) == false)
                {
                    GameObject focusIcon = Instantiate(data.prefab, this.transform);
                    focusIcon.name = data.type + "-Focus Icon";
                    icon.prefab = focusIcon;
                    focusIcon.SetActive(false);
                    focusIcons.Add(icon);
                }
            }
        }

        #endregion

        #region Main

        public void SetFocusIconPose(FocusType focusType, Content.Manager.Pose pose)
        {
            for (int i = 0; i < focusIcons.Count; i++)
            {
                if(focusIcons[i].type == focusType)
                {
                    focusIcons[i].prefab.transform.localPosition = pose.position;
                    focusIcons[i].prefab.transform.localRotation = pose.rotation;
                    focusIcons[i].prefab.SetActive(true);
                }
                else
                {
                    focusIcons[i].prefab.SetActive(false);
                }
            }
        }

        #endregion
    }
}
