using System;
using System.IO;
using Cysharp.Threading.Tasks;
using TriLibCore;
using TriLibCore.Mappers;
using TriLibCore.Samples;
using UnityEngine;

namespace Commands
{
    public class LoadFileFromPathCommand : ICommand<AssetLoaderContext>
    {
        private string path;
        private bool isDone;

        public LoadFileFromPathCommand(string path)
        {
            this.path = path;
        }

        private AssetLoaderOptions assetLoaderOptions;
        public AssetLoaderContext Result { get; private set; } = null;
        public async UniTask<AssetLoaderContext> ExecuteAsync()
        {
            var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions(false, true);
            assetLoaderOptions.ExternalDataMapper = ScriptableObject.CreateInstance<ExternalDataMapperSample>();
            assetLoaderOptions.TextureMappers = new TextureMapper[] { ScriptableObject.CreateInstance<TextureMapperSample>() };
            AssetLoader.LoadModelFromFile(path, OnLoad, OnMaterialsLoad, OnProgress, OnError, null, assetLoaderOptions);
            await UniTask.WaitUntil(()=>isDone);
            return Result;
        }

        private void OnError(IContextualizedError obj)
        {
            Debug.Log("An error occurred while loading your Model: " + obj.GetInnerException());
            isDone = true;
        }

        private void OnProgress(AssetLoaderContext arg1, float arg2)
        {
            //Debug.Log("Loading Model. Progress: " + arg2);
        }

        private void OnMaterialsLoad(AssetLoaderContext obj)
        {
            Result = obj;
            Debug.Log("Materials loaded!");
            isDone = true;
        }

        private void OnLoad(AssetLoaderContext obj)
        {
            Result = obj;
        }
    }
}