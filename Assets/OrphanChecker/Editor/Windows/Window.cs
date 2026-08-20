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
            var stylesheet = AssetDatabase.GUIDToAssetPath("69014e32dbd5459f96d8b3f732693620");
            return AssetDatabase.LoadAssetAtPath<StyleSheet>(stylesheet);

        }
    }
}