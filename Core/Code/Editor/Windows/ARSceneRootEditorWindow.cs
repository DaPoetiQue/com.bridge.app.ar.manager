using System;
using UnityEngine;
using UnityEditor;
using Bridge.Core.App.AR.Manager;
using Bridge.Core.App.Data.Storage;
using UnityEngine.XR.ARFoundation;
using Bridge.Core.Debug;

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
        private static SceneRootObject sceneRootObject;

        #region Storage Data

        private static StorageData.DirectoryInfoData storageDataInfo = new StorageData.DirectoryInfoData() 
        { 
            fileName = "ARSceenRootData", 
            folderName = "3ridge App Data"
        };

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
            sceneRootObject = CreateInstance<SceneRootObject>();
        }

        #endregion

        #region Main

        /// <summary>
        /// Draws window layouts.
        /// </summary>
        private void OnWindowUpdates()
        {
            DrawLayouts();
            OnEditorWindowUpdate();
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

        private void OnEditorWindowUpdate()
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
                SerializedObject arSceneContentSerializedObject = new SerializedObject(sceneRootObject);
                SerializedProperty arSceneContentSerializedObjectProperty = arSceneContentSerializedObject.FindProperty("content");
                EditorGUILayout.PropertyField(arSceneContentSerializedObjectProperty, true);
                arSceneContentSerializedObject.ApplyModifiedProperties();
            }

            GUILayout.Space(10);

            SerializedObject sceneRootSerializedObject = new SerializedObject(sceneRootObject);
            SerializedProperty sceneRootSerializedObjectProperty = sceneRootSerializedObject.FindProperty("settings");
            EditorGUILayout.PropertyField(sceneRootSerializedObjectProperty, true);
            sceneRootSerializedObject.ApplyModifiedProperties();

            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();

            if (FindObjectOfType<ARSceneRoot>() == false)
            {
                if (GUILayout.Button("Create AR Scene Root", GUILayout.Height(45)))
                {
                    CreateARSceneRoot(sceneRootObject);
                }
            }

            if (FindObjectOfType<ARSceneRoot>() == true)
            {
                if (GUILayout.Button("Update AR Scene Root", GUILayout.Height(45)))
                {
                    OnRootBuilderUpdateAction(sceneRootObject.content, sceneRootObject.content);
                }
            }

            if(FindObjectOfType<ARSceneRoot>())
            {
                if (GUILayout.Button("Remove Created Root", GUILayout.Height(45)))
                {
                    OnRootBuilderRemoveAction(sceneRootObject.content, sceneRootObject.content);
                }
            }

            EditorGUILayout.EndHorizontal();

            Storage.Directory.DataPathExists(storageDataInfo, (loadedFileData, filesCallBackResults) => 
            {
                if(filesCallBackResults.success == true)
                {
                    if (GUILayout.Button("Open Saved File Location", GUILayout.Height(45)))
                    {
                        EditorUtility.RevealInFinder(loadedFileData.filePath);
                    }

                    if (GUILayout.Button("Delete Saved File", GUILayout.Height(45)))
                    {
                        Storage.Directory.DeleteDirectory(storageDataInfo, callBackResults =>
                        {
                            if (callBackResults.success == true)
                            {
                                UnityEngine.Debug.Log(callBackResults.successValue);
                            }
                        });
                    }
                }
            });

            GUILayout.EndArea();
        }

        #region Root Builder Actions

        /// <summary>
        /// This functions creates a new ar scene root.
        /// </summary>
        /// <param name="sceneRootObject"></param>
        private void CreateARSceneRoot(SceneRootObject sceneRootObject)
        {
            try
            {
                if (FindObjectOfType<ARSceneRoot>())
                {
                    DebugConsole.Log(LogLevel.Warning, this, "A scene root aready exists in the current scene.");
                    return;
                }

                sceneRootObject.content.nameTag = string.IsNullOrEmpty(ARSceneRootEditorWindow.sceneRootObject.content.nameTag) ? "_3ridge AR Scene Root" : ARSceneRootEditorWindow.sceneRootObject.content.nameTag;

                ARSceneRootEditor.CreateARSceneRoot(sceneRootObject, (createdSettings, results) =>
                {
                    if (results.error)
                    {
                        UnityEngine.Debug.LogWarning(results.errorValue);
                        return;
                    }

                    if (results.success == true)
                    {
                        Storage.JsonData.Save(storageDataInfo, createdSettings, (savedDataResults) =>
                        {
                            if (savedDataResults.error == true)
                            {
                                DebugConsole.Log(LogLevel.Error, savedDataResults.errorValue);
                            }

                            if (savedDataResults.success == true)
                            {
                                DebugConsole.Log(LogLevel.Success, savedDataResults.successValue);
                            }
                        });
                    }

                    if (sceneRootObject == null) return;
                });
            }
            catch(Exception exception)
            {
                throw exception;
            }
        }
        private void OnRootBuilderUpdateAction(SceneRootBuilderData rootSettings, SceneRootBuilderData? content = null)
        {
            UnityEngine.Debug.Log("-->> Attempting To Update Root");

            ARSceneRootEditor.UpdateARSceneRoot(name, rootSettings, content, updated =>
            {
                UnityEngine.Debug.Log("-->> Root Updated");
            });
        }
        private void OnRootBuilderRemoveAction(SceneRootBuilderData rootSettings, SceneRootBuilderData? content = null)
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

        private void RemoveSceneRoot(Action<bool> callback)
        {
            if (FindObjectOfType<ARSceneRoot>() == null) return;

            #region Load AR Scene Event Camera data

            if(storageDataInfo.isLoaded)
            {
                ARSceneRootEditor.SetPreviousEventCamSettings(LoadSceneObjectData());

                //if (ARSceneRootEditor.GetPreviousEventCamSettings().useExistingCamera)
                //{
                //    Camera arCam = ARSceneRootEditor.arSceneEventCamera;
                //    arCam.name = ARSceneRootEditor.GetPreviousEventCamSettings().nameTag;
                //    arCam.clearFlags = ARSceneRootEditor.GetPreviousEventCamSettings().clearFlags;
                //    arCam.cullingMask = ARSceneRootEditor.GetPreviousEventCamSettings().cullingMask;
                //    arCam.backgroundColor = ARSceneRootEditor.GetPreviousEventCamSettings().backgroundColor;

                //    arCam.fieldOfView = ARSceneRootEditor.GetPreviousEventCamSettings().fieldOfView;
                //    arCam.nearClipPlane = ARSceneRootEditor.GetPreviousEventCamSettings().nearClipPlane;
                //    arCam.farClipPlane = ARSceneRootEditor.GetPreviousEventCamSettings().farClipPlane;
                //    StorageData.DirectoryInfoData directoryInfoData = new StorageData.DirectoryInfoData();
                //    directoryInfoData.sceneAssetPath = LoadSceneObjectData().sceneAssetPath;

                //    Storage.AssetData.LoadSceneAsset(directoryInfoData, (loadedParent, callBackResults) =>
                //    {
                //        arCam.transform.SetParent((Transform)loadedParent, false);
                //    });

                //    arCam.transform.localPosition = StorageData.SerializableData.Vector3(ARSceneRootEditor.GetPreviousEventCamSettings().serializablePosition);
                //    arCam.transform.localRotation = StorageData.SerializableData.Quaternion(ARSceneRootEditor.GetPreviousEventCamSettings().serializableRotation);

                //    if (arCam.gameObject.GetComponent<ARPoseDriver>()) DestroyImmediate(arCam.gameObject.GetComponent<ARPoseDriver>());
                //    if (arCam.gameObject.GetComponent<ARCameraBackground>()) DestroyImmediate(arCam.gameObject.GetComponent<ARCameraBackground>());
                //    if (arCam.gameObject.GetComponent<ARCameraManager>()) DestroyImmediate(arCam.gameObject.GetComponent<ARCameraManager>());
                //}
            }

            #endregion

            #region AR Scene Light

            #endregion

            #region Delete AR Scene Root Object

            DeleteARSceneRoot(FindObjectOfType<ARSceneRoot>().gameObject, deleted =>
            {
                if (deleted == false)
                {
                    UnityEngine.Debug.LogError($"-->> <color=orange>AR Scene Root Delete Failed : </color> <color=white>AR scene root content failed to delete.</color>");
                    return;
                }
            });

            #endregion
        }

        /// <summary>
        /// This is an action call back function for deleting scene root objects.
        /// </summary>
        /// <param name="sceneContent"></param>
        /// <param name="callback"></param>
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

        #region Serilaizations

        private SceneCameraData.SerializableCameraData LoadSceneObjectData()
        {
            var sceneCameraData = new SceneCameraData.SerializableCameraData();

            Storage.Directory.DataPathExists(storageDataInfo, (loadedFileData, filesCallBackResults) =>
            {
                if (filesCallBackResults.success == true)
                {
                    Storage.JsonData.Load<SceneCameraData.SerializableCameraData>(loadedFileData, (loadedDataResults, callBackResults) =>
                    {
                        if (callBackResults.error)
                        {
                            UnityEngine.Debug.LogError(callBackResults.errorValue);
                            return;
                        }

                        if (callBackResults.success)
                        {
                            UnityEngine.Debug.Log(callBackResults.successValue);

                            sceneCameraData = loadedDataResults;
                        }
                    });

                    storageDataInfo.isLoaded = filesCallBackResults.success;
                }
            });

            return sceneCameraData;
        }

        #endregion

        #endregion
    }
}
