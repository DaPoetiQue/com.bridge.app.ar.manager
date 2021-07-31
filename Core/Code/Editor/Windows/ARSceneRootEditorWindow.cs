using System;
using UnityEngine;
using UnityEditor;
using Bridge.Core.App.AR.Manager;
using UnityEngine.XR.ARFoundation;
using Bridge.App.Serializations.Manager;

namespace Bridge.Core.UnityEditor.AR.Manager
{
    public class ARSceneRootEditorWindow : EditorWindow
    {
        #region Components

        private static ARSceneRootEditorWindow window;

        #endregion

        #region Open Editor Window

        [MenuItem("Window/3ridge/AR Content Manager %&a")]
        private static void OpenARManagerEditor()
        {
            window = GetWindow<ARSceneRootEditorWindow>("AR Content Editor");
            window.minSize = new Vector2(400, 350);

            window.Show();
        }

        #endregion

        #region Window Layouts

        #region Textures

        private Texture2D iconTexture;
        private Texture2D headerSectionTexture;
        private Texture2D settingsSectionTexture;
        private Texture2D settingsSectionContentTexture;

        #endregion

        #region Colors

        private Color headerSectionColor = new Color(0.0f, 135.0f / 255.0f, 0.0f, 1.0f);
        private Color settingsSectionColor = new Color(25.0f / 255.0f, 25.0f / 255.0f, 25.0f / 255.0f, 1.0f);
        private Color settingsSectionContentColor = new Color(25.0f / 255.0f, 25.0f / 255.0f, 25.0f / 255.0f, 1.0f);

        #endregion

        #region Rects

        private Rect iconRect;
        private Rect headerSectionRect;
        private Rect settingsSectionRect;
        private Rect settingsSectionContentRect;

        #endregion

        #region Window Styles

        private GUIStyle settingsHeaderStyle = new GUIStyle();
        private GUIStyle settingContentStyle = new GUIStyle();

        #endregion

        #endregion

        #region Window Content

        private GUIContent settingsHeaderContent = new GUIContent();

        #endregion

        #region Window Settings

        private static bool addCustomSceneRootContent;
        private static ARSceneRootObject arSceneRootSettings;

        #region Storage Data

        private static StorageData.Info storageDataInfo = new StorageData.Info();

        private static string fileName = "ARSceenRootData";
        private static string folderName = "3ridge App Data";

        #endregion

        #endregion

        #region Unity

        private void OnEnable() => Init();
        private void OnGUI() => OnWindowUpdates();

        #endregion

        #region Initializations

        private void Init()
        {
            InitializeTextures();
            InitializeLayoutStyles();
            InitializeContentData();

            //SerializationManager.LoadData<>((loadedData, success) => 
            //{
            //    if(success == false)
            //    {
            //        UnityEngine.Debug.LogWarning("-->> Config data failed to load data from given path.");
            //    }

            //    ARSceneRootEditor.SetPreviousEventCamSettings(loadedData);

            //    UnityEngine.Debug.Log($"-->> <color=green>AR Config Load Success</color> <color=white>Load config data with name :</color> <color=cyan>{loadedData.nameTag.ToString()}.</color>");
            //});
        }

        private void InitializeTextures()
        {
            #region Header

            headerSectionTexture = new Texture2D(1, 1);
            headerSectionTexture.SetPixel(0, 0, headerSectionColor);
            headerSectionTexture.Apply();

            #endregion

            #region Icon

            iconTexture = Resources.Load<Texture2D>("Editor UI/Windows");

            #endregion

            #region Settings

            settingsSectionTexture = new Texture2D(1, 1);
            settingsSectionTexture.SetPixel(0, 0, settingsSectionColor);
            settingsSectionTexture.Apply();

            settingsSectionContentTexture = new Texture2D(1, 1);
            settingsSectionContentTexture.SetPixel(0, 0, settingsSectionContentColor);
            settingsSectionContentTexture.Apply();

            #endregion
        }

        private void InitializeLayoutStyles()
        {
            #region Settings Header

            settingsHeaderStyle.normal.textColor = Color.white;
            settingsHeaderStyle.fontSize = 15;
            settingsHeaderStyle.fontStyle = FontStyle.Bold;
            settingsHeaderStyle.padding.top = 40;
            settingsHeaderStyle.padding.left = 50;
            settingsHeaderStyle.alignment = TextAnchor.LowerCenter;
            settingsHeaderContent.text = "AR Scene Template Builder";

            #endregion

            #region Settings Content

            settingContentStyle.normal.textColor = Color.white;
            settingContentStyle.fontSize = 12;
            settingContentStyle.alignment = TextAnchor.LowerLeft;
            settingContentStyle.padding.left = 25;

            #endregion
        }

        private void InitializeContentData()
        {
            arSceneRootSettings = CreateInstance<ARSceneRootObject>();

            storageDataInfo.fileName = fileName;
            storageDataInfo.folderName = folderName;
        }

        #endregion

        #region Main

        /// <summary>
        /// Draws window layouts.
        /// </summary>
        private void OnWindowUpdates()
        {
            DrawLayouts();
            DrawSettingsLayout();
        }

        private void DrawLayouts()
        {
            #region Header Section

            headerSectionRect.x = 0;
            headerSectionRect.y = 0;
            headerSectionRect.width = Screen.width;
            headerSectionRect.height = 100;
            GUI.DrawTexture(headerSectionRect, headerSectionTexture);

            #endregion

            #region Icon

            iconRect.width = 100;
            iconRect.height = 100;
            iconRect.x = 10;
            iconRect.y = 0;

            GUI.DrawTexture(iconRect, iconTexture);
            GUILayout.Label(settingsHeaderContent, settingsHeaderStyle);

            #endregion

            #region Settings Section

            settingsSectionRect.x = 0;
            settingsSectionRect.y = headerSectionRect.height;
            settingsSectionRect.width = Screen.width;
            settingsSectionRect.height = Screen.height - headerSectionRect.height;

            GUI.DrawTexture(settingsSectionRect, settingsSectionTexture);

            float settingsSectionContentX = Screen.width - (Screen.width / 4);

            settingsSectionContentRect.x = 15;
            settingsSectionContentRect.y = settingsSectionRect.y;
            settingsSectionContentRect.width = settingsSectionContentX;
            settingsSectionContentRect.height = settingsSectionRect.height;

            GUI.DrawTexture(settingsSectionContentRect, settingsSectionContentTexture);

            #endregion
        }

        private void DrawSettingsLayout()
        {
            GUILayout.BeginArea(settingsSectionContentRect);
            GUILayout.Space(25);

            GUILayout.Label("This Tool Creates AR Scene Root Templates.");
            GUILayout.Space(15);

            GUILayout.BeginHorizontal();

            GUILayout.Label("Add Custom Root Content");
            addCustomSceneRootContent = EditorGUILayout.Toggle(addCustomSceneRootContent);

            GUILayout.EndHorizontal();

            if (addCustomSceneRootContent)
            {
                SerializedObject arSceneContentSerializedObject = new SerializedObject(arSceneRootSettings);
                SerializedProperty arSceneContentSerializedObjectProperty = arSceneContentSerializedObject.FindProperty("content");
                EditorGUILayout.PropertyField(arSceneContentSerializedObjectProperty, true);
                arSceneContentSerializedObject.ApplyModifiedProperties();
            }

            GUILayout.Space(10);

            SerializedObject sceneRootSerializedObject = new SerializedObject(arSceneRootSettings);
            SerializedProperty sceneRootSerializedObjectProperty = sceneRootSerializedObject.FindProperty("settings");
            EditorGUILayout.PropertyField(sceneRootSerializedObjectProperty, true);
            sceneRootSerializedObject.ApplyModifiedProperties();

            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();

            // contentType = (ContentType)EditorGUILayout.EnumPopup("Content Type", contentType);

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();

            if (FindObjectOfType<ARSceneRoot>() == false)
            {
                if (GUILayout.Button("Create AR Scene Root", GUILayout.Height(45)))
                {
                    OnRootBuilderCreateAction(arSceneRootSettings.settings, arSceneRootSettings.content);
                }
            }

            if (FindObjectOfType<ARSceneRoot>() == true)
            {
                if (GUILayout.Button("Update AR Scene Root", GUILayout.Height(45)))
                {
                    OnRootBuilderUpdateAction(arSceneRootSettings.settings, arSceneRootSettings.content);
                }
            }

            if(FindObjectOfType<ARSceneRoot>())
            {
                if (GUILayout.Button("Remove Created Root", GUILayout.Height(45)))
                {
                    OnRootBuilderRemoveAction(arSceneRootSettings.settings, arSceneRootSettings.content);
                }
            }

            EditorGUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        #region Root Builder Actions

        private void OnRootBuilderCreateAction(ARSceneRootSettings rootSettings, ARSceneRootContent content)
        {
            if (FindObjectOfType<ARSceneRoot>()) return;

            string name = string.IsNullOrEmpty(arSceneRootSettings.content.nameTag) ? "_3ridge AR Scene Root" : arSceneRootSettings.content.nameTag;

            ARSceneRootEditor.CreateNewARSceneRoot(name, rootSettings, content, (createdSettings, results) =>
            {
                if(results.error)
                {
                    UnityEngine.Debug.LogWarning(results.errorValue);
                    return;
                }

                if (results.success == true)
                {
                    Storage.JsonFiles.Save(storageDataInfo, createdSettings, (save) =>
                    {
                        if(save.error == true)
                        {
                            UnityEngine.Debug.LogWarning(save.errorValue);
                            return;
                        }

                        if(save.success == true)
                        {
                            UnityEngine.Debug.Log(save.successValue);
                        }
                    });
                }

                if (rootSettings.focusHandler == null) return;

                string path = EditorUtility.SaveFilePanelInProject("Save AR Focus Asset", name, "asset", "Save Created AR Scene Focus Asset");

                if (string.IsNullOrEmpty(path)) return;
            });
        }

        private void OnRootBuilderUpdateAction(ARSceneRootSettings rootSettings, ARSceneRootContent? content = null)
        {
            UnityEngine.Debug.Log("-->> Attempting To Update Root");

            ARSceneRootEditor.UpdateARSceneRoot(name, rootSettings, content, updated =>
            {
                UnityEngine.Debug.Log("-->> Root Updated");
            });
        }
        private void OnRootBuilderRemoveAction(ARSceneRootSettings rootSettings, ARSceneRootContent? content = null)
        {
            RemoveSceneRoot(removed =>
            {
                if (removed)
                {
                    UnityEngine.Debug.Log("-->> Root Removed");
                }
            });
        }

        #endregion

        private static void RemoveSceneRoot(Action<bool> callback)
        {
            if (FindObjectOfType<ARSceneRoot>() == null) return;

            #region Load AR Scene Event Camera data

            // Storage.JsonFiles.Load(storageDataInfo, );

            //if (ARSceneRootEditor.usedExistingSceneEventCamera)
            //{
            //    Camera arCam = ARSceneRootEditor.arSceneEventCamera;
            //    arCam.name = ARSceneRootEditor.GetPreviousEventCamSettings().nameTag;

            //    arCam.clearFlags = ARSceneRootEditor.GetPreviousEventCamSettings().clearFlags;
            //    arCam.cullingMask = ARSceneRootEditor.GetPreviousEventCamSettings().cullingMask;
            //    arCam.backgroundColor = ARSceneRootEditor.GetPreviousEventCamSettings().backgroundColor;

            //    arCam.cameraType = ARSceneRootEditor.GetPreviousEventCamSettings().cameraType;
            //    arCam.usePhysicalProperties = ARSceneRootEditor.GetPreviousEventCamSettings().usePhysicalProperties;

            //    arCam.fieldOfView = ARSceneRootEditor.GetPreviousEventCamSettings().fieldOfView;
            //    arCam.nearClipPlane = ARSceneRootEditor.GetPreviousEventCamSettings().nearClipPlane;
            //    arCam.farClipPlane = ARSceneRootEditor.GetPreviousEventCamSettings().farClipPlane;

            //    // arCam.transform.SetParent(ARSceneRootEditor.GetPreviousEventCamSettings().parent, false);

            //    arCam.transform.localPosition = new Vector3(ARSceneRootEditor.GetPreviousEventCamSettings().position.x, ARSceneRootEditor.GetPreviousEventCamSettings().position.y, ARSceneRootEditor.GetPreviousEventCamSettings().position.z);
            //    arCam.transform.localRotation = new Quaternion(ARSceneRootEditor.GetPreviousEventCamSettings().rotation.x, ARSceneRootEditor.GetPreviousEventCamSettings().rotation.y, ARSceneRootEditor.GetPreviousEventCamSettings().rotation.z, ARSceneRootEditor.GetPreviousEventCamSettings().rotation.w);

            //    if (arCam.gameObject.GetComponent<ARPoseDriver>()) DestroyImmediate(arCam.gameObject.GetComponent<ARPoseDriver>());
            //    if (arCam.gameObject.GetComponent<ARCameraBackground>()) DestroyImmediate(arCam.gameObject.GetComponent<ARCameraBackground>());
            //    if(arCam.gameObject.GetComponent<ARCameraManager>()) DestroyImmediate(arCam.gameObject.GetComponent<ARCameraManager>());
            //}

            #endregion

            #region AR Scene Light

            if (ARSceneRootEditor.usedExistingSceneLight)
            {

            }

            #endregion

            #region Delete AR Scene Root Object

            //DeleteARSceneRoot(FindObjectOfType<ARSceneRoot>().gameObject, deletedSuccessfully => 
            //{
            //    if(deletedSuccessfully == false)
            //    {
            //        UnityEngine.Debug.LogError($"-->> <color=orange>AR Scene Root Delete Failed : </color> <color=white>AR scene root content failed to delete.</color>");
            //        return;
            //    }

            //    //DeleteSettingsData((directory, deleted) =>
            //    //{
            //    //    if (deleted == false)
            //    //    {
            //    //        UnityEngine.Debug.LogError($"-->> <color=orange>Directory Removal Failed : </color> <color=white>AR root config settings failed to delete directory : </color><color=cyan>{directory}.</color>");
            //    //        return;
            //    //    }

            //    //    if (deleted == true)
            //    //    {
            //    //        UnityEngine.Debug.Log($"-->> <color=green>Directory Removed Successfully : </color><color=white>AR root config settings deleted successfully from directory :</color> <color=cyan>{directory}</color>");
            //    //    }
            //    //});
            //});

            #endregion
        }

        private static void DeleteARSceneRoot(GameObject sceneContent, Action<bool> callback)
        {
            DestroyImmediate(sceneContent);

            if (FindObjectOfType<ARSceneRoot>() == null)
            {
                callback.Invoke(true);
            }
            else
            {
                callback.Invoke(false);
            }
        }

        #endregion
    }
}
