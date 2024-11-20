using System;
using System.Collections.Generic;
using System.IO;
using Commands;
using Core;
using CustomBehaviours;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Features
{
    public class LoadFileService
    {
        public class FileDataModel
        {
            public string name;
            public byte[] data;
        }

        private ManagerDataViewService _managerDataViewService => Locator<ManagerDataViewService>.Instance;
        
        public void ClearPersistentDataPath()
        {
            string path = Application.persistentDataPath;

            if (Directory.Exists(path))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(path);

                foreach (FileInfo file in directoryInfo.GetFiles())
                {
                    file.Delete();
                }

                foreach (DirectoryInfo dir in directoryInfo.GetDirectories())
                {
                    dir.Delete(true);
                }

                Debug.Log("Cleared folder: " + path);
            }
            else
            {
                Debug.LogWarning("Directory does not exist: " + path);
            }
        }

        public async UniTask LoadData(List<FileDataModel> filesData, TMP_Text txtLoading)
        {
            Debug.Log("Loading data...");
            
            for (int i = 0; i < filesData.Count; i++)
            {
                txtLoading.text = $"Loading {i}/{filesData.Count} files...";
                Debug.Log($"Loading {i}/{filesData.Count} files...");
                var loadResult = await LoadModel(filesData[i]);
                if (!loadResult)
                {
                    Debug.Log($"Error loading file {filesData[i].name}");
                }
            }
            
            Debug.Log("Data loaded.");
            _managerDataViewService.StartView();
        }

        private async UniTask<bool> LoadModel(FileDataModel model)
        {
            var result = await new LoadFileFromStreamDataCommand(model.data, model.name, null).ExecuteAsync();
            if(result == null)
                return false;
            await UniTask.NextFrame();
            if(result.RootGameObject == null)
                return false;
            _managerDataViewService.AddFile(new FileModel()
            {
                Model = result.RootGameObject,
                Name = result.Filename,
                // Sagittal = textureMappers[0],
                // Coronal = textureMappers[1],
                // Transverse = textureMappers[2]
            });
            return true;
        }

        //         public async UniTask<List<string>> StoredFiles(List<FileDataModel> fileDatas)
//         {
//             Debug.Log("Storing files...");
//             var storedFiles = new List<string>();
//             var taskList = new List<UniTask>();
//             foreach (var data in fileDatas)
//             {
//                 string destinationFilePath = Path.Combine(Application.persistentDataPath, Path.GetFileName(data.name));
//                 storedFiles.Add(destinationFilePath);
//                 
//                 
//                 // Save the file bytes to the destination file path
// // #if UNITY_EDITOR
// //                 taskList.Add(UniTask.RunOnThreadPool(async () =>
// //                 {
// //                     await File.WriteAllBytesAsync(destinationFilePath, data.data);
// //                     await UniTask.CompletedTask;
// //                 } ));
// // #else
// //                 File.WriteAllBytes(destinationFilePath, data.data);
// // #endif
//             }
// // #if UNITY_EDITOR
// //             await UniTask.WhenAll(taskList);
// // #else
// //             await UniTask.Delay(TimeSpan.FromSeconds(2));
// // #endif
//             Debug.Log("Files stored.");
//             return storedFiles;
//         }
        
        
        public async UniTask LoadFile(string path)
        {
            var result = await new LoadFileFromPathCommand(path).ExecuteAsync();
            if(result == null)
                return;
            await UniTask.NextFrame();
            if(result.RootGameObject == null)
                return;

            var textureMappers = new List<TextureMap>(3);
            
            //Iterate the loaded textures list
            foreach (var kvp in result.LoadedTextures)
            {
                //FinalPath contains the loaded texture filename
                string finalPath = kvp.Key.Filename;
                if (string.IsNullOrEmpty(finalPath)) continue;
                textureMappers.Add(new TextureMap()
                {
                    Name = finalPath,
                    Texture = (Texture2D)kvp.Value.UnityTexture
                });
            }
            
            while (textureMappers.Count < 3)
            {
                textureMappers.Add(new TextureMap());
            }
            
            _managerDataViewService.AddFile(new FileModel()
            {
                Model = result.RootGameObject,
                Name = result.Filename,
                Sagittal = textureMappers[0],
                Coronal = textureMappers[1],
                Transverse = textureMappers[2]
            });
        }
    }
}