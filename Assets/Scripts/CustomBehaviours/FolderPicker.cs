using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Commands;
using Core;
using CustomBehaviours.Elements;
using Cysharp.Threading.Tasks;
using Features;
using Michsky.MUIP;
using TMPro;
using TriLibCore;
using TriLibCore.Mappers;
using TriLibCore.Samples;
using UnityEngine;
using UnityEngine.Assertions;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CustomBehaviours
{
    public class FolderPicker : MonoBehaviour
    {
        [Serializable]
        public class FileData
        {
            public string name;
            public string data;
        }

        LoadFileService _service = new LoadFileService();

        [SerializeField] private WindowManager _windowManager;

        [SerializeField] private TMP_Text _textLoading;
        [SerializeField] private TMP_Text _textFolder;

        [SerializeField] private IModelView[] modelViews;

        [DllImport("__Internal")]
        private static extern void OpenFolderPicker(string callbackObjectName,
            string callbackFunctionName,
            string onSuccessCallbackFunctionName,
            string onPreLoadCallbackFunctionName);

        [DllImport("__Internal")]
        private static extern void OpenFilePicker(string callbackObjectName,
            string callbackFunctionName,
            string onSuccessCallbackFunctionName,
            string onPreLoadCallbackFunctionName);

        List<LoadFileService.FileDataModel> _fileDataModels = new List<LoadFileService.FileDataModel>();

        public async void OnChooseFolderClick()
        {
            _fileDataModels = new List<LoadFileService.FileDataModel>();
            _service.ClearPersistentDataPath();
#if UNITY_WEBGL && !UNITY_EDITOR
        OpenFolderPicker(gameObject.name, "OnFilePicked","OnReadFileSuccess","OnPreLoad");
#elif UNITY_EDITOR
            string path = EditorUtility.OpenFolderPanel("Select Folder", "", "");
            if (!string.IsNullOrEmpty(path))
            {
                Debug.Log("Selected folder: " + path);
                _textFolder.text = path;
                // You can now use the selected path
                string[] files = Directory.GetFiles(path);
                _windowManager.OpenWindow("Loading");
                foreach (var file in files)
                {
                    byte[] fileBytes = await File.ReadAllBytesAsync(file);
                    var fileDataModel = new LoadFileService.FileDataModel
                    {
                        name = Path.GetFileName(file),
                        data = fileBytes
                    };
                    _fileDataModels.Add(fileDataModel);
                }

                await _service.LoadData(_fileDataModels, _textLoading);
                await OnLoadComplete();

                _windowManager.PrevWindow();
            }
#endif
        }

        public void OnOpenFilePicker()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            OpenFilePicker(gameObject.name, "OnFilePicked","OnReadFileSuccess","OnPreLoad");
#endif
        }

        public void OnPreLoad(string message)
        {
            _textLoading.text = $"{message}...";
            _windowManager.OpenWindow("Loading");
        }

        public async void OnReadFileSuccess(string result)
        {
            Debug.Log("Read file success: " + result);
            await _service.LoadData(_fileDataModels, _textLoading);
            await OnLoadComplete();
            _windowManager.PrevWindow();
        }

        public void OnFilePicked(string json)
        {
            FileData file = JsonUtility.FromJson<FileData>(json);
            if (file == null)
            {
                Debug.Log($"csharp reading: empty files");
                return;
            }

            Debug.Log($"csharp reading:  {file.name}");

            // Convert base64 data to byte array
            byte[] fileBytes = Convert.FromBase64String(file.data);
            var fileDataModel = new LoadFileService.FileDataModel
            {
                name = Path.GetFileName(file.name),
                data = fileBytes
            };
            _fileDataModels.Add(fileDataModel);
        }

        // Receives the selected folder's file paths
        public async void OnFolderPicked(string json)
        {
            _windowManager.OpenWindow("Loading");
            FileData[] files = JsonHelper.FromJson<FileData>(json);
            if (files == null || files.Length == 0)
            {
                _windowManager.PrevWindow();
                _textFolder.text = "No files selected";
                return;
            }

            _textFolder.text = Directory.GetDirectoryRoot(files[0].name);

            foreach (var file in files)
            {
                // Convert base64 data to byte array
                byte[] fileBytes = Convert.FromBase64String(file.data);
                var fileDataModel = new LoadFileService.FileDataModel
                {
                    name = Path.GetFileName(file.name),
                    data = fileBytes
                };
                _fileDataModels.Add(fileDataModel);
            }

            await _service.LoadData(_fileDataModels, _textLoading);
            await OnLoadComplete();
            _windowManager.PrevWindow();
        }

        private async UniTask OnLoadComplete()
        {
            foreach (var modelView in modelViews)
            {
                await modelView.SetupView(false);
                await UniTask.NextFrame();
            }
        }
    }
}