using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace OrphanChecker.Editor
{
    public static class AssetTypeCatalog
    {
        private static List<Type> _typeCache;
        
        /// <summary>
        /// This string is copied from the project tab search bar with all type filters enabled
        /// </summary>
        public static readonly string[] CommonTypes = "t:AnimationClip t:AudioClip t:AudioMixer t:ComputeShader t:Font t:GUISkin t:GraphAsset t:VisualEffectAsset t:ScriptGraphAsset t:Material t:Mesh t:Model t:PhysicsMaterial t:Prefab t:Scene t:Script t:Shader t:Sprite t:Texture t:VideoClip t:VisualEffectSubgraph".Split(" ");
        
        public static List<string> GetAssetTypeTokens()
        {
            if (_typeCache == null || _typeCache.Count == 0)
            {
                ReloadTypeCache();
            }

            if (_typeCache == null) throw new Exception("No types found.");
            var usable = _typeCache
                .Where(t => AssetDatabase.FindAssets($"t:{t.Name}", new[] { "Assets" }).Length > 0)
                .Select(t => $"t:{t.Name}")
                .ToList();

            return usable;
        }

        public static void ReloadTypeCache()
        {
            _typeCache = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(GetTypesSafe)
                .Where(t => t.IsClass && !t.IsAbstract && !t.IsGenericType)
                .Where(t => typeof(UnityEngine.Object).IsAssignableFrom(t))
                .OrderBy(t => t.Name)
                .ToList();
        }

        private static IEnumerable<Type> GetTypesSafe(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null);
            }
        }
    }
}