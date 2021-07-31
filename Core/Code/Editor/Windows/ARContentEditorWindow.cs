using UnityEngine;
using UnityEditor;
using Bridge.Core.App.AR.Manager;
using UnityEngine.XR.ARFoundation;
using System.IO;
using System;

namespace Bridge.Core.UnityEditor.AR.Manager
{
    public class ARContentEditorWindow : EditorWindow
    {
        #region Components

        private static ARContentEditorWindow window;

        #endregion

        #region Open Editor Window

        [MenuItem("Window/3ridge/AR Content Manager %&a")]
        private static void OpenARManagerEditor()
        {
            window = GetWindow<ARContentEditorWindow>("AR Content Editor");
            window.minSize = new Vector2(400, 600);

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

            LoadSettingsData((loadedData, success) => 
            {
                if(success == false)
                {
                    UnityEngine.Debug.LogWarning("-->> Config data failed to load data from given path.");
                }

                ARSceneRootEditor.SetPreviousEventCamSettings(loadedData);

                UnityEngine.Debug.Log($"-->> <color=green>AR Config Load Success</color> <color=white>Load config data with name :</color> <color=cyan>{loadedData.nameTag.ToString()}.</color>");
            });
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

            GUILayout.Space(40);

            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();

            if(FindObjectOfType<ARSceneRoot>())
            {
                if (GUILayout.Button("Update AR Scene Root", GUILayout.Height(45)))
                {
                    CreateContent(false, arSceneRootSettings.settings, arSceneRootSettings.content);
                }
            }
            else
            {
                if (GUILayout.Button("Create AR Scene Root", GUILayout.Height(45)))
                {
                    CreateContent(true, arSceneRootSettings.settings, arSceneRootSettings.content);
                }
            }

            if(FindObjectOfType<ARSceneRoot>())
            {
                if (GUILayout.Button("Remove Created Root", GUILayout.Height(45)))
                {
                    RevertARSceneRoot(removed => 
                    {
                        if(removed)
                        {

                        }
                    });
                }
            }

            EditorGUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        private void CreateContent(bool createNew, ARSceneRootSettings settings, ARSceneRootContent content)
        {
            if (FindObjectOfType<ARSceneRoot>()) return;

            string name = string.IsNullOrEmpty(arSceneRootSettings.content.nameTag) ? "_3ridge AR Scene Root" : arSceneRootSettings.content.nameTag;

            if (createNew)
            {
                ARSceneRootEditor.CreateNewARSceneRoot(name, settings, content, results =>
                {
                    if(results == true)
                    {
                        SaveSettingsData(ARSceneRootEditor.GetPreviousEventCamSettings(), (filePath, saved) =>
                        {
                            if(saved == false)
                            {
                                UnityEngine.Debug.LogWarning($"-->> <color=orange>Config Data Save Failed</color> <color=white>AR root config settings data failed to save at path :</color> <color=cyan>{filePath}</color>");
                                return;
                            }
                            else
                            {
                                UnityEngine.Debug.Log($"-->> <color=green>Config Data Save Success</color> <color=white>AR root config settings data saved successfully at path :</color> <color=cyan>{filePath}</color>");
                            }
                        });
                    }
                });

                if (settings.focusHandler == null) return;
                
                string path = EditorUtility.SaveFilePanelInProject("Save AR Focus Asset", name, "asset", "Save Created AR Scene Focus Asset");

                if (string.IsNullOrEmpty(path)) return;

                // AssetDatabase.CreateAsset();
            }
            else
            {
                ARSceneRootEditor.UpdateARSceneRoot(name, settings, content);
            }
        }

        private static void RevertARSceneRoot(Action<bool> callback)
        {
            if (FindObjectOfType<ARSceneRoot>() == null) return;

            #region AR Scene Event Camera

            if (ARSceneRootEditor.usedExistingSceneEventCamera)
            {
                Camera arCam = ARSceneRootEditor.arSceneEventCamera;
                arCam.name = ARSceneRootEditor.GetPreviousEventCamSettings().nameTag;

                arCam.clearFlags = ARSceneRootEditor.GetPreviousEventCamSettings().clearFlags;
                arCam.cullingMask = ARSceneRootEditor.GetPreviousEventCamSettings().cullingMask;
                arCam.backgroundColor = ARSceneRootEditor.GetPreviousEventCamSettings().backgroundColor;

                arCam.cameraType = ARSceneRootEditor.GetPreviousEventCamSettings().cameraType;
                arCam.usePhysicalProperties = ARSceneRootEditor.GetPreviousEventCamSettings().usePhysicalProperties;

                arCam.fieldOfView = ARSceneRootEditor.GetPreviousEventCamSettings().fieldOfView;
                arCam.nearClipPlane = ARSceneRootEditor.GetPreviousEventCamSettings().nearClipPlane;
                arCam.farClipPlane = ARSceneRootEditor.GetPreviousEventCamSettings().farClipPlane;

                arCam.transform.parent = null;
                arCam.transform.localPosition = ARSceneRootEditor.GetPreviousEventCamSettings().position;
                arCam.transform.localScale = ARSceneRootEditor.GetPreviousEventCamSettings().scale;
                arCam.transform.localRotation = ARSceneRootEditor.GetPreviousEventCamSettings().rotation;

                if (arCam.gameObject.GetComponent<ARPoseDriver>()) DestroyImmediate(arCam.gameObject.GetComponent<ARPoseDriver>());
                if (arCam.gameObject.GetComponent<ARCameraBackground>()) DestroyImmediate(arCam.gameObject.GetComponent<ARCameraBackground>());
                if(arCam.gameObject.GetComponent<ARCameraManager>()) DestroyImmediate(arCam.gameObject.GetComponent<ARCameraManager>());
            }

            #endregion

            #region AR Scene Light

            if (ARSceneRootEditor.usedExistingSceneLight)
            {

            }

            #endregion

            #region Delete AR Scene Root Object

            DeleteARSceneRoot(FindObjectOfType<ARSceneRoot>().gameObject, deletedSuccessfully => 
            {
                if(deletedSuccessfully == false)
                {
                    UnityEngine.Debug.LogError($"-->> <color=orange>AR Scene Root Delete Failed : </color> <color=white>AR scene root content failed to delete.</color>");
                    return;
                }

                DeleteSettingsData((directory, deleted) =>
                {
                    if (deleted == false)
                    {
                        UnityEngine.Debug.LogError($"-->> <color=orange>Directory Removal Failed : </color> <color=white>AR root config settings failed to delete directory : </color><color=cyan>{directory}.</color>");
                        return;
                    }

                    if (deleted == true)
                    {
                        UnityEngine.Debug.Log($"-->> <color=green>Directory Removed Successfully : </color><color=white>AR root config settings deleted successfully from directory :</color> <color=cyan>{directory}</color>");
                    }
                });
            });

            #endregion
        }

        #endregion

        #region Persistant Data

        private static void SaveSettingsData(SceneEventCameraSettings settings, Action<string, bool> callback = null)
        {
            string configDataPath = string.Empty;

            GetConfigurationDataDirectory((directory, exists) =>
            {
                if (exists == false)
                {
                    UnityEngine.Debug.LogWarning($"-->> <color=orange></color> <color=white>AR root config settings directory :</color> <color=cyan>{directory}</color> <color=white>doesn't exist.</color>");
                    return;
                }

                string configData = JsonUtility.ToJson(settings);
                configDataPath = Path.Combine(directory, "arconfigdata.json");

                if(File.Exists(configDataPath) == true)
                {
                    File.Delete(configDataPath);
                }

                File.WriteAllText(configDataPath, configData);
                callback.Invoke(configDataPath, File.Exists(configDataPath));
            });
        }

        private static void LoadSettingsData(Action<SceneEventCameraSettings, bool> callback)
        {
            string path = Path.Combine(Application.persistentDataPath, "arconfigdata.json");

            UnityEngine.Debug.Log($"-->> AR root config settings saved successfully at path : {path}");

            GetConfigurationDataPath((path, exists) => 
            {
                if(exists == false)
                {
                    UnityEngine.Debug.LogWarning($"-->> <color=orange> Load AR Root Config Failed - </color> <color=white>AR root config settings data file missing, not found at path :</color> <color=cyan>{path}</color>");
                    return;
                }

                string configDataFile = File.ReadAllText(path);
                SceneEventCameraSettings configDataObject = JsonUtility.FromJson<SceneEventCameraSettings>(configDataFile);

                callback.Invoke(configDataObject, string.IsNullOrEmpty(configDataFile));
            });
        }

        private static void DeleteARSceneRoot(GameObject sceneContent, Action<bool> callback)
        {
            DestroyImmediate(sceneContent);

            if(FindObjectOfType<ARSceneRoot>() == null)
            {
                callback.Invoke(true);
            }
            else
            {
                callback.Invoke(false);
            }
        }

        private static void DeleteSettingsData(Action<string, bool> callback)
        {
            GetConfigurationDataDirectory((directory, exists) =>
            {
                if (exists == false)
                {
                    UnityEngine.Debug.LogWarning($"-->> <color=orange></color> <color=white>AR root config settings directory :</color> <color=cyan>{directory}</color> <color=white>doesn't exist.</color>");
                    return;
                }

                Directory.Delete(directory);
                callback.Invoke(directory, Directory.Exists(directory));
            });
        }

        private static void GetConfigurationDataDirectory(Action<string, bool> callback)
        {
            string folder = Path.Combine(Application.persistentDataPath, "3ridge AR Configuration Data");

            if (Directory.Exists(folder) == false)
            {
                Directory.CreateDirectory(folder);
            }

            callback.Invoke(folder, Directory.Exists(folder));
        }

        private static void GetConfigurationDataPath(Action<string, bool> callback)
        {
            string configDataPath = string.Empty;

            GetConfigurationDataDirectory((directory, exists) =>
            {
                if (exists == true)
                {
                    configDataPath = Path.Combine(directory, "arconfigdata.json");
                }
            });

            callback.Invoke(configDataPath, File.Exists(configDataPath));
        }

        #endregion
    }
}
