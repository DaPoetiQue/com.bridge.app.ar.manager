using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.ARFoundation;
using Bridge.Core.Debug;

namespace Bridge.Core.App.AR.Manager
{
    [RequireComponent(typeof(Light))]

    public class ARSceneLightEstimation : MonoDebug
    {
        #region Components

        [HideInInspector]
        public ARCameraManager cameraManager;

        private ReflectionProbe m_ReflectionProbe;
        private Light sceneLight;

        private float? brightness;
        private float? colorTemperature;
        private Color? colorCorrection;
        private Vector3? mainLightDirection;
        private Color? mainLightColor;
        private float? mainLightIntensityLumens;
        private SphericalHarmonicsL2? sphericalHarmonics;

        #endregion

        #region Unity

        void OnEnable()
        {
            if (cameraManager != null) cameraManager.frameReceived += ARSceneFrameChanged;
        }

        void OnDisable()
        {
            if (cameraManager != null) cameraManager.frameReceived -= ARSceneFrameChanged;
        }

        void Awake() => Init();

        #endregion

        #region Initialization

        #endregion

        #region Main

        #endregion

        private void Init()
        {
            sceneLight = GetComponent<Light>();

            if(cameraManager == null)
            {
                Log(LogLevel.Warning, this, "Camera manager can not be null.");
                return;
            }
        }

        void ARSceneFrameChanged(ARCameraFrameEventArgs frameData)
        {
            if (frameData.lightEstimation.averageBrightness.HasValue)
            {
                brightness = frameData.lightEstimation.averageBrightness.Value;
                if (brightness.Value < 0.2)
                {
                    sceneLight.intensity = brightness.Value;
                    m_ReflectionProbe.intensity = brightness.Value;
                    RenderSettings.ambientIntensity = brightness.Value;
                }
            }

            if (frameData.lightEstimation.averageColorTemperature.HasValue)
            {
                colorTemperature = frameData.lightEstimation.averageColorTemperature.Value;
                sceneLight.colorTemperature = colorTemperature.Value;

            }

            if (frameData.lightEstimation.colorCorrection.HasValue)
            {
                colorCorrection = frameData.lightEstimation.colorCorrection.Value;
                sceneLight.color = colorCorrection.Value;
                RenderSettings.skybox.color = colorCorrection.Value;
            }

            if (frameData.lightEstimation.mainLightDirection.HasValue)
            {
                mainLightDirection = frameData.lightEstimation.mainLightDirection;
                sceneLight.transform.rotation = Quaternion.LookRotation(mainLightDirection.Value);
            }

            if (frameData.lightEstimation.mainLightColor.HasValue)
            {
                mainLightColor = frameData.lightEstimation.mainLightColor;

                #if PLATFORM_ANDROID

                    sceneLight.color = mainLightColor.Value / Mathf.PI;
                    sceneLight.color = sceneLight.color.gamma;

                    var camera = cameraManager.GetComponentInParent<Camera>();
                    if (camera == null || !camera.allowHDR)
                    {
                       Log(LogLevel.Debug, this, $"HDR Rendering is not allowed.  Color values returned could be above the maximum representable value.");
                    }

                #endif
            }

            if (frameData.lightEstimation.mainLightIntensityLumens.HasValue)
            {
                mainLightIntensityLumens = frameData.lightEstimation.mainLightIntensityLumens;
                sceneLight.intensity = frameData.lightEstimation.averageMainLightBrightness.Value;
            }

            if (frameData.lightEstimation.ambientSphericalHarmonics.HasValue)
            {
                sphericalHarmonics = frameData.lightEstimation.ambientSphericalHarmonics;
                RenderSettings.ambientMode = AmbientMode.Skybox;
                RenderSettings.ambientProbe = sphericalHarmonics.Value;
            }
        }
    }
}
