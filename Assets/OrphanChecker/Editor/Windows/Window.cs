using OrphanChecker.Data;
using UnityEngine.UIElements;

namespace OrphanChecker.Editor.Windows
{
    public abstract class Window
    {
        protected readonly Settings Settings = SettingsInstance.GetInstance();
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

        public abstract VisualElement Create();

        public abstract void FullReload();
    }
}