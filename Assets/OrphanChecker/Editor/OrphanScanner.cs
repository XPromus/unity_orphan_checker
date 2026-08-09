using System;
using System.Collections.Generic;
using System.IO;
using OrphanChecker.Data;
using UnityEngine;
using UnityEditor;

namespace OrphanChecker.Editor
{
    public static class OrphanScanner
    {
        public static readonly string[] ScannableExtensions =
        {
            ".prefab", ".unity", ".mat", ".asset", ".anim", ".controller", 
            ".overrideController", ".playable", ".spriteatlas", ".physicMaterial"
        };

        public static Dictionary<string, int> BuildReferenceCounts()
        {
            var counts = new Dictionary<string, int>();
            var assetPaths = AssetDatabase.GetAllAssetPaths();
            foreach (var assetPath in assetPaths)
            {
                if 
                (
                    !assetPath.StartsWith("Assets/") || 
                    assetPath.EndsWith(".meta") || 
                    AssetDatabase.IsValidFolder(assetPath) || 
                    !IsScannable(assetPath)
                ) continue;

                var text = File.ReadAllText(assetPath);
                var i = text.IndexOf("guid: ", System.StringComparison.Ordinal);
                while (i >= 0)
                {
                    var guid = text.Substring(i + 6, 32);
                    counts.TryGetValue(guid, out var c);
                    counts[guid] = c + 1;
                    i = text.IndexOf("guid: ", i + 6, System.StringComparison.Ordinal);
                }
            }
            return counts;
        }
        
        public static List<Orphan> FindOrphans(Dictionary<string, int> counts)
        {
            var orphans = new List<Orphan>();
            AddOrphans(counts, orphans, "t:Material", OrphanType.Material);
            AddOrphans(counts, orphans, "t:Prefab", OrphanType.Prefab);
            AddOrphans(counts, orphans, "t:MonoScript", OrphanType.Script, IsMonoBehaviourScript);
            return orphans;
        }

        private static void AddOrphans
        (
            Dictionary<string, int> counts,
            List<Orphan> orphans,
            string filter,
            OrphanType type,
            Func<string, bool> isValid = null
        )
        {
            foreach (var guid in AssetDatabase.FindAssets(filter, new[] { "Assets" }))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (isValid != null && !isValid(path)) continue;
                if (!counts.ContainsKey(guid))
                {
                    orphans.Add(new Orphan
                    {
                        Guid = guid,
                        Path = AssetDatabase.GUIDToAssetPath(guid),
                        FilterType = filter,
                        Size = new FileInfo(path).Length,
                        Type = type
                    });
                }
            }
        }

        private static bool IsMonoBehaviourScript(string path)
        {
            var type = AssetDatabase.LoadAssetAtPath<MonoScript>(path)?.GetClass();
            return type != null && typeof(MonoBehaviour).IsAssignableFrom(type);
        }

        private static bool IsScannable(string path)
        {
            return System.Array.IndexOf(ScannableExtensions, Path.GetExtension(path)) >= 0;
        }
    }
}