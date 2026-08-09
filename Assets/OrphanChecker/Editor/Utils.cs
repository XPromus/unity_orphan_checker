using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace OrphanChecker.Editor
{
    public static class Utils
    {
        public static VisualElement CreateDivider()
        {
            return new VisualElement
            {
                style =
                {
                    height = 1,
                    width = Length.Percent(100),
                    backgroundColor = EditorGUIUtility.isProSkin 
                        ? new Color(0.5f, 0.5f, 0.5f, 0.5f)
                        : new Color(0.1f, 0.1f, 0.1f, 0.5f),
                    marginTop = 6,
                    marginBottom = 6
                }
            };
        }
        
        public static void PingAsset(string path)
        {
            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Object>(path));
        }

        public static string FormatBytes(long bytes)
        {
            string[] units = { "B", "KB", "MB", "GB", "TB" };
            double size = bytes;
            var unitIndex = 0;

            while (size >= 1024 && unitIndex < units.Length - 1)
            {
                size /= 1024;
                unitIndex++;
            }

            return unitIndex == 0
                ? $"{bytes} {units[0]}"
                : $"{size:0.##} {units[unitIndex]}";
        }
    }
}