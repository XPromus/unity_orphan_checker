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
    }
}