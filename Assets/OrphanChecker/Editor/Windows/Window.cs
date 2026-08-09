using OrphanChecker.Data;
using UnityEditor;
using UnityEngine.UIElements;

namespace OrphanChecker.Editor.Windows
{
    public abstract class Window
    {
        protected readonly Settings Settings = SettingsInstance.GetInstance();
        private readonly StyleSheet _styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/OrphanChecker/Editor/OrphanChecker.uss");
        
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
    }
}