using System;
using OrphanChecker.Editor.Data;
using UnityEditor;
using UnityEngine.UIElements;

namespace OrphanChecker.Editor.Windows
{
    public abstract class Window
    {
        protected readonly Settings Settings = SettingsInstance.GetInstance();
        private readonly StyleSheet _styleSheet = LoadStyleSheet();
        
        protected VisualElement Container = new()
        {
            style =
            {
                flexDirection = FlexDirection.Column,
                paddingBottom = 10,
                paddingTop = 10,
                paddingLeft = 10,
                paddingRight = 10
            }
        };

        protected Window()
        {
            Container.styleSheets.Add(_styleSheet);
        }

        public abstract VisualElement Create();

        public abstract void FullReload();

        private static StyleSheet LoadStyleSheet()
        {
            var scriptPath = AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets($"t:Script {nameof(Window)}")[0]);
            var directory = System.IO.Path.GetDirectoryName(scriptPath);
            return AssetDatabase.LoadAssetAtPath<StyleSheet>(System.IO.Path.Combine(directory ?? throw new InvalidOperationException(), "OrphanChecker.uss"));
        }
    }
}