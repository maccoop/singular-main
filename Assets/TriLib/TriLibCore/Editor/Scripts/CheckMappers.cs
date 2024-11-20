using TriLibCore.Mappers;
using TriLibCore.Utils;
using UnityEditor;
using UnityEngine;

namespace TriLibCore.Editor
{
    /// <summary>
    /// Represents a series of Material Mapper utility methods.
    /// </summary>
    public static class CheckMappers
    {
        [MenuItem("Tools/TriLib/Select Material Mappers based on Rendering Pipeline")]
        private static void AutoSelect()
        {
            for (var i = 0; i < MaterialMapper.RegisteredMappers.Count; i++)
            {
                var materialMapperName = MaterialMapper.RegisteredMappers[i];
                TriLibSettings.SetBool(materialMapperName, false);
            }
            var materialMapper = GetCompatibleMaterialMapper();
            SelectMapper(materialMapper);
        }

        /// <summary>
        /// Tries to find the best Material Mapper depending on the Rendering Pipeline.
        /// </summary>
        public static void EnableCompatibleMaterialMapper()
        {
            var hasAnyMapper = false;
            for (var i = 0; i < MaterialMapper.RegisteredMappers.Count; i++)
            {
                var materialMapperName = MaterialMapper.RegisteredMappers[i];
                if (TriLibSettings.GetBool(materialMapperName))
                {
                    hasAnyMapper = true;
                    break;
                }
            }
            if (!hasAnyMapper)
            {
                var materialMapper = GetCompatibleMaterialMapper();
                SelectMapper(materialMapper);
            }
        }

        [InitializeOnEnterPlayMode]
        [InitializeOnLoadMethod]
        public static void Initialize()
        {
            EnableCompatibleMaterialMapper();
        }
        public static void SelectMapper(string materialMapper)
        {
            Debug.Log($"TriLib is configured to use the '{materialMapper}' Material Mapper. If you want to use different Material Mappers, you can change this setting on the Project Settings/TriLib menu.");
            TriLibSettings.SetBool(materialMapper, true);
        }

        private static string GetCompatibleMaterialMapper()
        {
            string materialMapper;
            if (GraphicsSettingsUtils.IsUsingHDRPPipeline)
            {
                materialMapper = "HDRPMaterialMapper";
            }
            else if (GraphicsSettingsUtils.IsUsingUniversalPipeline)
            {
                materialMapper = "UniversalRPMaterialMapper";
            }
            else
            {
                materialMapper = "StandardMaterialMapper";
            }
            return materialMapper;
        }
    }
}