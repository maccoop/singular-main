using System;
using System.IO;
using Cysharp.Threading.Tasks;
using TriLibCore;
using Unity.VisualScripting;
using UnityEngine;

namespace Commands
{
    public class LoadFileFromStreamDataCommand : ICommand<AssetLoaderContext>
    {
        private byte[] fileData;
        private string fileName;
        private Action<AssetLoaderContext,float> onProgress;

        private bool isDone;
        public LoadFileFromStreamDataCommand(byte[] fileData, string fileName, Action<AssetLoaderContext, float> onProgress)
        {
            this.fileData = fileData;
            this.fileName = fileName;
            this.onProgress = onProgress;
        }

        public AssetLoaderContext Result { get; private set; }

        public async UniTask<AssetLoaderContext> ExecuteAsync()
        {
            var memoryStream = new MemoryStream(fileData);
            var fileExtension = Path.GetExtension(fileName);
            var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions(false, true);
// #if UNITY_WEBGL
//             assetLoaderOptions.GetCompatibleTextureFormat = false;
//             assetLoaderOptions.EnforceAlphaChannelTextures = false;
//             assetLoaderOptions.UseUnityNativeNormalCalculator = true;
// #endif
            Result = AssetLoader.LoadModelFromStream(memoryStream, fileName, fileExtension, onLoad, onMaterialsLoad,
                delegate (AssetLoaderContext assetLoaderContext, float progress) { onProgress?.Invoke(assetLoaderContext, 0.5f + progress * 0.5f); },
                onError, null, assetLoaderOptions);
            await UniTask.WaitUntil(()=>isDone);
            return Result;
        }

        private void onError(IContextualizedError obj)
        {
            isDone = true;
        }

        private void onMaterialsLoad(AssetLoaderContext obj)
        {
            isDone = true;
        }

        private void onLoad(AssetLoaderContext obj)
        {
            Debug.Log($"Start loading model... {obj.Filename}");
        }
    }
}