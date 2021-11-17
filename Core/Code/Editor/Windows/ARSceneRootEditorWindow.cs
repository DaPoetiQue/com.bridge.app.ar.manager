using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEditor.Build.Reporting;
using UnityEngine.XR;
using Bridge.Core.Debug;
using Bridge.Core.UnityEditor.Debugger;
using Bridge.Core.App.AR.Manager;
using Bridge.Core.App.Data.Storage;

namespace Bridge.Core.UnityEditor.AR.Manager
{
    public class ARSceneRootEditorWindow : EditorWindow
    {
        #region Components

        private static ARSceneRootEditorWindow window;

        #endregion

        #region Open Editor Window

        [MenuItem("Window/3ridge/AR Content Manager %&a")]
        public static void OpenARManagerEditor()
        {
            var windowInstance = GetWindow<ARSceneRootEditorWindow>("AR Content Editor");
            windowInstance.minSize = new Vector2(350, 400);
            windowInstance.maxSize = new Vector2(500, 600);
            windowInstance.Show();
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

        #region Data

        private static Vector2 scrollPosition;

        #endregion

        #endregion

        #region Window Content

        private GUIContent settingsHeaderContent = new GUIContent();

        #endregion

        #region Settings

        private static BuildSettings appSettings;
        private static bool RunAppOnCmpletion;
        private static AndroidPreferredInstallLocation installLocation;

        private static bool addCustomSceneRootContent;
        private static SceneRootObject sceneRootObject;

        #region Storage Data

        private static StorageData.DirectoryInfoData appInfoStorageData = new StorageData.DirectoryInfoData()
        { 
            fileName = "artoolkit",
            folderName = "Editor Data",
            extensionType = StorageData.ExtensionType.json
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

            iconTexture = Resources.Load<Texture2D>("Editor/Windows");

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
            settingsHeaderContent.text = "AR ToolKit Master";

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
            appSettings = CreateInstance<BuildSettings>();

            // If config not loaded. set default settings.
            appSettings.appInfo.appVersion = "1.0";
            appSettings.configurations.platform = BuildTarget.Android; // This will be loaded from a json file called buildSettings.json
            appSettings.androidSettings.SdkVersion = AndroidSdkVersions.AndroidApiLevel24;

            // This will be loaded from a json file called sceneSetup.json
            sceneRootObject = CreateInstance<SceneRootObject>();
            // sceneRootObject.settings.estimatedLighting =;
            sceneRootObject.settings.lightShadowType = LightShadows.Hard;

            sceneRootObject.settings.sceneFocusData = CreateInstance<ARSceneFocusData>();
        }

        #endregion

        #region Main

        /// <summary>
        /// Draws window layouts.
        /// </summary>
        private void OnWindowUpdates()
        {
            if (window == null)
            {
                appSettings.appInfo = GetBuildSettings().appInfo;

                window = GetWindow<ARSceneRootEditorWindow>();
                DebugConsole.Log(LogLevel.Debug, $"Window Refreshed!.");
            }

            DrawLayouts();
            OnEditorWindowUpdate();
        }

        private void DrawLayouts()
        {
            if(headerSectionTexture == null)
            {
                DebugConsole.Log(LogLevel.Warning, "Header Texture Missing");
            }

            #region Header Section

            headerSectionRect.x = 0;
            headerSectionRect.y = 0;
            headerSectionRect.width = Screen.width;
            headerSectionRect.height = 100;

            if(headerSectionTexture == null)
            {
                InitializeTextures();
            }

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
            settingsSectionRect.width = window.position.width;
            settingsSectionRect.height = window.position.height - headerSectionRect.height;

            GUI.DrawTexture(settingsSectionRect, settingsSectionTexture, ScaleMode.ScaleToFit);

            settingsSectionContentRect.x = 0;
            settingsSectionContentRect.y = settingsSectionRect.y;
            settingsSectionContentRect.width = settingsSectionRect.width;
            settingsSectionContentRect.height = settingsSectionRect.height;

            GUI.DrawTexture(settingsSectionContentRect, settingsSectionContentTexture);

            #endregion
        }

        private void OnEditorWindowUpdate()
        {
            GUILayout.BeginArea(settingsSectionRect);

            #region Menu Content Area

            #region Menu Content and Style Update

            if (appSettings == null)
            {
                InitializeContentData();
            }

            GUIStyle style = new GUIStyle();
            style.padding = new RectOffset(10, 10, 25, 25);

            var layout = new GUILayoutOption[2];
            layout[0] = GUILayout.Width(settingsSectionRect.width);
            layout[1] = GUILayout.ExpandHeight(true);

            #endregion

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, style ,layout);

            #region Scroll Area

            #region App Info & Configurations

            GUILayout.Space(10);
            SerializedObject appInfoSerializedObject = new SerializedObject(appSettings);
            SerializedProperty appInfoSerializedObjectProperty = appInfoSerializedObject.FindProperty("appInfo");
            EditorGUILayout.PropertyField(appInfoSerializedObjectProperty, true);
            appInfoSerializedObject.ApplyModifiedProperties();

            GUILayout.Space(10);
            SerializedObject appConfigSerializedObject = new SerializedObject(appSettings);
            SerializedProperty appConfigSerializedObjectProperty = appConfigSerializedObject.FindProperty("configurations");
            EditorGUILayout.PropertyField(appConfigSerializedObjectProperty, true);
            appConfigSerializedObject.ApplyModifiedProperties();

            if (appSettings.configurations.platform == BuildTarget.Android)
            {
                GUILayout.Space(10);

                SerializedObject androidConfigSerializedObject = new SerializedObject(appSettings);
                SerializedProperty androidConfigSerializedObjectProperty = androidConfigSerializedObject.FindProperty("androidSettings");
                EditorGUILayout.PropertyField(androidConfigSerializedObjectProperty, true);
                androidConfigSerializedObject.ApplyModifiedProperties();
            }

            #endregion

            #region Custom Content

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();

            GUILayout.Label("Add Custom Root Content");
            addCustomSceneRootContent = EditorGUILayout.Toggle(addCustomSceneRootContent);

            GUILayout.EndHorizontal();

            if (addCustomSceneRootContent)
            {
                GUILayout.Space(10);

                SerializedObject arSceneContentSerializedObject = new SerializedObject(sceneRootObject);
                SerializedProperty arSceneContentSerializedObjectProperty = arSceneContentSerializedObject.FindProperty("content");
                EditorGUILayout.PropertyField(arSceneContentSerializedObjectProperty, true);
                arSceneContentSerializedObject.ApplyModifiedProperties();
            }

            #endregion

            #region App Settings

            GUILayout.Space(10);

            SerializedObject sceneRootSerializedObject = new SerializedObject(sceneRootObject);
            SerializedProperty sceneRootSerializedObjectProperty = sceneRootSerializedObject.FindProperty("settings");
            EditorGUILayout.PropertyField(sceneRootSerializedObjectProperty, true);
            sceneRootSerializedObject.ApplyModifiedProperties();

            #endregion

            #region Root Creation Buttons

            GUILayout.Space(15);

            EditorGUILayout.BeginHorizontal();

            if (FindObjectOfType<ARSceneRoot>() == false)
            {
                if (GUILayout.Button("Create AR Scene Root", GUILayout.Height(45)))
                {
                    if(EditorUserBuildSettings.activeBuildTarget == appSettings.configurations.platform)
                    {
                        if (FindObjectOfType<Camera>() == true)
                        {
                            if (EditorUtility.DisplayDialog("A Camera Aready Exist", $"Do you want to use the existing camera as a AR root scene camera?.", "Use Existing", "Create New"))
                            {
                                bool state = sceneRootObject.content.createRootCamera;
                                sceneRootObject.content.createRootCamera = false;
                                CreateARSceneRoot(sceneRootObject);
                                sceneRootObject.content.createRootCamera = state;
                            }
                            else
                            {
                                bool state = sceneRootObject.content.createRootCamera;
                                sceneRootObject.content.createRootCamera = true;
                                CreateARSceneRoot(sceneRootObject);
                                sceneRootObject.content.createRootCamera = state;
                            }
                        }
                        else
                        {
                            bool state = sceneRootObject.content.createRootCamera;
                            sceneRootObject.content.createRootCamera = true;
                            CreateARSceneRoot(sceneRootObject);
                            sceneRootObject.content.createRootCamera = state;
                        }
                    }
                    else
                    {
                        if (EditorUtility.DisplayDialog("Build Target Not Supported", "Switch to a supported target platform.", "Switch", "Cancel"))
                        {
                            switch(appSettings.configurations.platform)
                            {
                                case BuildTarget.Android:

                                    EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, appSettings.configurations.platform);

                                    break;

                                case BuildTarget.iOS:

                                    EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, appSettings.configurations.platform);

                                    break;
                            }

                            DebugConsole.Log(LogLevel.Debug, "Switching to available supported platform.");
                        }
                        else
                        {
                            DebugConsole.Log(LogLevel.Warning, "Build target switch canceled. AR builder not created.");
                        }
                    }

                    ApplyAppSettings(appSettings);
                }
            }

            if (FindObjectOfType<ARSceneRoot>() == true)
            {
                if (GUILayout.Button("Update AR Scene Root", GUILayout.Height(45)))
                {
                    // OnRootBuilderUpdateAction(sceneRootObject.content, sceneRootObject.content);
                }

                GUILayout.Space(2);

                if (GUILayout.Button("Remove Created Root", GUILayout.Height(45)))
                {
                    OnRootBuilderRemoveAction(sceneRootObject.content, sceneRootObject.content);
                }
            }

            EditorGUILayout.EndHorizontal();

            #endregion

            #region App Builder

            GUILayout.Space(4);

            if (FindObjectOfType<ARSceneRoot>())
            {
                if (GUILayout.Button("Install & Launch App", GUILayout.Height(45)))
                {
                    BuildApp(appSettings, RunAppOnCmpletion);
                }

                if(File.Exists(appSettings.configurations.buildLocation) == true)
                {
                    GUILayout.Space(10);

                    if (GUILayout.Button("Open Build Folder", GUILayout.Height(45)))
                    {
                        OpenFileInExplorer(appSettings.configurations.buildLocation);
                    }
                }
            }

            #endregion

            #endregion

            EditorGUILayout.EndScrollView();

            #endregion

            GUILayout.EndArea();
        }

        #region Root Builder Actions

        private static BuildSettings GetBuildSettings()
        {
            BuildSettings settings = appSettings;

            #region App Info

            settings.appInfo.companyName = PlayerSettings.companyName;
            settings.appInfo.appName = PlayerSettings.productName;
            settings.appInfo.appVersion = PlayerSettings.bundleVersion;
            settings.appInfo.appIcon = PlayerSettings.GetIconsForTargetGroup(BuildTargetGroup.Unknown)[0];

            #endregion

            #region Configurations

            settings.configurations.allowedOrientation = PlayerSettings.defaultInterfaceOrientation;
            settings.configurations.platform = EditorUserBuildSettings.activeBuildTarget;
            settings.configurations.allowDebugging = EditorUserBuildSettings.allowDebugging;
            settings.configurations.developmentBuild = EditorUserBuildSettings.development;

            #endregion

            return settings;
        }

        /// <summary>
        /// This method create build settings for the selected platform.
        /// </summary>
        /// <param name="buildSettings"></param>
        private static void ApplyAppSettings(BuildSettings buildSettings)
        {
            DebugConsole.Log(LogLevel.Debug, $"XR Loaded : {XRSettings.loadedDeviceName}");

            if (string.IsNullOrEmpty(buildSettings.appInfo.companyName))
            {
                DebugConsole.Log(LogLevel.Warning, "App info's company name field is empty. Please assign company name in the <color=red>[AR Tool Kit Master]</color> inspector panel.");
                return;
            }

            if (string.IsNullOrEmpty(buildSettings.appInfo.appName))
            {
                DebugConsole.Log(LogLevel.Warning, "App info's app name field is empty. Please assign app name in the <color=red>[AR Tool Kit Master]</color> inspector panel.");
                return;
            }

            

            PlayerSettings.companyName = (string.IsNullOrEmpty(buildSettings.appInfo.companyName)) ? PlayerSettings.companyName : buildSettings.appInfo.companyName;
            PlayerSettings.productName = (string.IsNullOrEmpty(buildSettings.appInfo.appName)) ? PlayerSettings.productName : buildSettings.appInfo.appName;
            PlayerSettings.bundleVersion = (string.IsNullOrEmpty(buildSettings.appInfo.appVersion)) ? PlayerSettings.bundleVersion : buildSettings.appInfo.appVersion;

            string companyName = buildSettings.appInfo.companyName;

            if (companyName.Contains(" "))
            {
                companyName = companyName.Replace(" ", "");

            }

            string appName = buildSettings.appInfo.appName;

            if (appName.Contains(" "))
            {
                appName = appName.Replace(" ", "");
            }

            buildSettings.appInfo.appIdentifier = $"com.{companyName}.{appName}";

            switch (buildSettings.configurations.allowedOrientation)
            {
                case UIOrientation.AutoRotation:

                    PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;

                    break;

                case UIOrientation.Portrait:

                    PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;

                    break;

                case UIOrientation.PortraitUpsideDown:

                    PlayerSettings.defaultInterfaceOrientation = UIOrientation.PortraitUpsideDown; ;

                    break;

                case UIOrientation.LandscapeLeft:

                    PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;

                    break;

                case UIOrientation.LandscapeRight:

                    PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeRight;

                    break;
            }

            if (buildSettings.appInfo.splashScreen != null)
            {
                PlayerSettings.SplashScreen.background = buildSettings.appInfo.splashScreen;
                PlayerSettings.SplashScreen.backgroundColor = Color.black;
            }

            if (buildSettings.appInfo.appIcon != null)
            {
                PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Unknown, new Texture2D[] { buildSettings.appInfo.appIcon });
            }

            switch (EditorUserBuildSettings.activeBuildTarget)
            {
                case BuildTarget.Android:

                    PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, buildSettings.appInfo.appIdentifier);

                    GraphicsDeviceType[] graphicsDeviceType = new GraphicsDeviceType[1];
                    graphicsDeviceType[0] = GraphicsDeviceType.OpenGLES3;
                    PlayerSettings.SetGraphicsAPIs(buildSettings.configurations.platform, graphicsDeviceType);

                    PlayerSettings.SetMobileMTRendering(BuildTargetGroup.Android, false);

                    PlayerSettings.Android.minSdkVersion = buildSettings.androidSettings.SdkVersion;
                    PlayerSettings.Android.androidTVCompatibility = false;
                    PlayerSettings.Android.preferredInstallLocation = buildSettings.androidSettings.installLocation;
                    PlayerSettings.Android.ARCoreEnabled = true;

                    EditorUserBuildSettings.buildAppBundle = buildSettings.androidSettings.buildAppBundle;

                    DebugConsole.Log(LogLevel.Debug, $"{XRSettings.loadedDeviceName}");

                    break;

                case BuildTarget.iOS:

                    PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, buildSettings.appInfo.appIdentifier);

                    break;
            }

            EditorUserBuildSettings.allowDebugging = buildSettings.configurations.allowDebugging;
            EditorUserBuildSettings.development = buildSettings.configurations.developmentBuild;

            EditorUserBuildSettings.SetBuildLocation(buildSettings.configurations.platform, buildSettings.configurations.buildLocation);
        }

        private static void BuildApp(BuildSettings buildSettings, bool runApp)
        {
            switch(buildSettings.configurations.platform)
            {
                case BuildTarget.Android:

                    BuildAndroid(buildSettings, runApp);

                    break;

                case BuildTarget.iOS:


                    if (BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.iOS, BuildTarget.iOS))
                    {
                    
                    }

                    break;
            }
        }

        private static void BuildAndroid(BuildSettings settings, bool runApp)
        {
            BuildPlayerOptions buildOptions = new BuildPlayerOptions();
            buildOptions.scenes = new[] {SceneManager.GetActiveScene().path};
            settings.configurations.buildLocation = EditorUtility.SaveFilePanel("Choose A Build Folder", Application.dataPath + "/../", settings.appInfo.appName, "apk");

            if(string.IsNullOrEmpty(settings.configurations.buildLocation))
            {
                return;
            }

            if (File.Exists(settings.configurations.buildLocation))
            {
                File.Delete(settings.configurations.buildLocation);
            }

            buildOptions.locationPathName = settings.configurations.buildLocation;

            buildOptions.target = BuildTarget.Android;

            if(runApp)
            {
                buildOptions.options = BuildOptions.AutoRunPlayer;
            }
            else
            {
                buildOptions.options = BuildOptions.None;
            }

            BuildReport report = BuildPipeline.BuildPlayer(buildOptions);
            BuildSummary summary = report.summary;

            if(summary.result == BuildResult.Succeeded)
            {
                EditorWindow.FocusWindowIfItsOpen<ARSceneRootEditorWindow>();
                DebugConsole.Log(LogLevel.Success, "App build completed successfully.");
            }

            if (summary.result == BuildResult.Failed)
            {
                DebugConsole.Log(LogLevel.Success, $"App build failed.");
            }
        }

        private static void OpenFileInExplorer(string assetPat)
        {
            DebugConsole.Log(LogLevel.Debug, $"Path : {assetPat}");
        }    

        /// <summary>
        /// This functions creates a new ar scene root.
        /// </summary>
        /// <param name="sceneRootObject"></param>
        public static void CreateARSceneRoot(SceneRootObject sceneRootObject)
        {
            try
            {
                if (FindObjectOfType<ARSceneRoot>())
                {
                    DebugConsole.Log(LogLevel.Warning, "A scene root aready exists in the current scene.");
                    return;
                }

                sceneRootObject.content.nameTag = string.IsNullOrEmpty(ARSceneRootEditorWindow.sceneRootObject.content.nameTag) ? "_3ridge AR Scene Root" : ARSceneRootEditorWindow.sceneRootObject.content.nameTag;

                ARSceneRootEditor.CreateARSceneRoot(sceneRootObject, (createdSettings, results) =>
                {
                    if (results.error)
                    {
                        DebugConsole.Log(LogLevel.Error, results.errorValue);
                        return;
                    }

                    if (sceneRootObject == null) return;
                });
            }
            catch(Exception exception)
            {
                throw exception;
            }
        }
        private void OnRootBuilderUpdateAction(SceneRootBuilderData rootSettings, SceneRootBuilderData content = null)
        {
            UnityEngine.Debug.Log("-->> Attempting To Update Root");

            ARSceneRootEditor.UpdateARSceneRoot(name, rootSettings, content, updated =>
            {
                UnityEngine.Debug.Log("-->> Root Updated");
            });
        }
        private void OnRootBuilderRemoveAction(SceneRootBuilderData rootSettings, SceneRootBuilderData content = null)
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

            //if(storageDataInfo.isLoaded)
            //{
            //   // ARSceneRootEditor.SetPreviousEventCamSettings(LoadSceneObjectData());

            //    //if (ARSceneRootEditor.GetPreviousEventCamSettings().useExistingCamera)
            //    //{
            //    //    Camera arCam = ARSceneRootEditor.arSceneEventCamera;
            //    //    arCam.name = ARSceneRootEditor.GetPreviousEventCamSettings().nameTag;
            //    //    arCam.clearFlags = ARSceneRootEditor.GetPreviousEventCamSettings().clearFlags;
            //    //    arCam.cullingMask = ARSceneRootEditor.GetPreviousEventCamSettings().cullingMask;
            //    //    arCam.backgroundColor = ARSceneRootEditor.GetPreviousEventCamSettings().backgroundColor;

            //    //    arCam.fieldOfView = ARSceneRootEditor.GetPreviousEventCamSettings().fieldOfView;
            //    //    arCam.nearClipPlane = ARSceneRootEditor.GetPreviousEventCamSettings().nearClipPlane;
            //    //    arCam.farClipPlane = ARSceneRootEditor.GetPreviousEventCamSettings().farClipPlane;
            //    //    StorageData.DirectoryInfoData directoryInfoData = new StorageData.DirectoryInfoData();
            //    //    directoryInfoData.sceneAssetPath = LoadSceneObjectData().sceneAssetPath;

            //    //    Storage.AssetData.LoadSceneAsset(directoryInfoData, (loadedParent, callBackResults) =>
            //    //    {
            //    //        arCam.transform.SetParent((Transform)loadedParent, false);
            //    //    });

            //    //    arCam.transform.localPosition = StorageData.SerializableData.Vector3(ARSceneRootEditor.GetPreviousEventCamSettings().serializablePosition);
            //    //    arCam.transform.localRotation = StorageData.SerializableData.Quaternion(ARSceneRootEditor.GetPreviousEventCamSettings().serializableRotation);

            //    //    if (arCam.gameObject.GetComponent<ARPoseDriver>()) DestroyImmediate(arCam.gameObject.GetComponent<ARPoseDriver>());
            //    //    if (arCam.gameObject.GetComponent<ARCameraBackground>()) DestroyImmediate(arCam.gameObject.GetComponent<ARCameraBackground>());
            //    //    if (arCam.gameObject.GetComponent<ARCameraManager>()) DestroyImmediate(arCam.gameObject.GetComponent<ARCameraManager>());
            //    //}
            //}

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

        #endregion
    }
}
