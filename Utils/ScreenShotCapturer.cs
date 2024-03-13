#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor.Recorder;
using UnityEditor.Recorder.Input;
using UnityEngine;

namespace ArchCore.Utils
{
    
    [Serializable]
    public class ScreenShotSetting
    {
        public string prefix;
        public int width = 1000, height = 1000;
    }
    
    public class ScreenShotCapturer : MonoBehaviour
    {

        [SerializeField] private KeyCode hotKey = KeyCode.KeypadEnter;
        [SerializeField] private ScreenShotSetting[] screenShotSettings;
        
        List<(ScreenShotSetting setting, RecorderController controller)> recorderControllers;

        public ScreenShotSetting[] ScreenShotSettings => screenShotSettings;

        [SerializeField] private OutputPath outputPath;
        private MethodInfo setCustomSizeMethod;
        private MethodInfo addSizeMethod;
        private MethodInfo selectSizeMethod;
        private MethodInfo backupCurrentSizeMethod;
        private MethodInfo restoreSizeMethod;

        private bool initialized;

        void OnEnable()
        {
            recorderControllers = new List<(ScreenShotSetting setting, RecorderController controller)>();
            foreach (var screenShotSetting in ScreenShotSettings)
            {
                var controllerSettings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
                var recorderController = new RecorderController(controllerSettings);
                
                // Image
                var imageRecorder = ScriptableObject.CreateInstance<ImageRecorderSettings>();
                imageRecorder.name = $"Screen Capturer {screenShotSetting.prefix}";
                imageRecorder.Enabled = true;
                imageRecorder.OutputFormat = ImageRecorderSettings.ImageRecorderOutputFormat.PNG;
                imageRecorder.CaptureAlpha = false;

                imageRecorder.OutputFile = $"{Path.Combine(outputPath.GetFullPath(), screenShotSetting.prefix, "shot")}_{DateTime.UtcNow.Ticks}_{DefaultWildcard.Take}";

                imageRecorder.imageInputSettings = new GameViewInputSettings
                {
                    OutputWidth = screenShotSetting.width,
                    OutputHeight = screenShotSetting.height,
                };
                
                // Setup Recording
                controllerSettings.AddRecorderSettings(imageRecorder);
                controllerSettings.SetRecordModeToSingleFrame(0);
                
                recorderControllers.Add((screenShotSetting, recorderController));
            }
            
           
        }

        private void Update()
        {
            if (Input.GetKeyDown(hotKey))
            {
                Shot();
            }
        }

        public void Shot()
        {
            if (!initialized)
            {
                Debug.LogError("Not initialized!");
                return;
            }
            
            StartCoroutine(Take());
        }


        void TryInitialize()
        {
            try
            {
                var type = typeof(RecorderController).Assembly.GetTypes()
                    .First(t => t.FullName.Contains("GameViewSize"));
                setCustomSizeMethod = type.GetMethod("SetCustomSize");
                addSizeMethod = type.GetMethod("AddSize");
                selectSizeMethod = type.GetMethod("SelectSize");
                backupCurrentSizeMethod = type.GetMethod("BackupCurrentSize");
                restoreSizeMethod = type.GetMethod("RestoreSize");
            }
            catch (Exception e)
            {
                Debug.LogError($"Can't get methods in GameViewSize with exception: {e}");
                initialized = false;
                return;
            }

            initialized = true;
        }

        private void Awake()
        {
            TryInitialize();
        }

        IEnumerator Take()
        {
            backupCurrentSizeMethod.Invoke(null, new object[0]);
            Time.timeScale = 0;
            
            yield return null;
            
            foreach (var pair in recorderControllers)
            {
                var setting = pair.setting;
                
                var size = setCustomSizeMethod.Invoke(null, new object[]{setting.width, setting.height});
                if (size == null)
                    size = addSizeMethod.Invoke(null, new object[]{setting.width, setting.height});
                selectSizeMethod.Invoke(null, new object[]{size});

                yield return null;
                yield return null;
                yield return null;
                yield return null;
                
                pair.controller.PrepareRecording();
                pair.controller.StartRecording();
                
                yield return null;
               
            }
            
            Time.timeScale = 1;
            restoreSizeMethod.Invoke(null, new object[0]);
        }
    }
}

 #endif
