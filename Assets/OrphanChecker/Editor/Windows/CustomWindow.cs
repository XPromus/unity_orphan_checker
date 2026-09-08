using UnityEngine.UIElements;

namespace OrphanChecker.Editor.Windows
{
    [Window("Custom", 2)]
    public class CustomWindow : Window
    {
        public override VisualElement Create()
        {
            Container.Add(new Label("Custom Types"));
            return Container;
        }

        public override void FullReload()
        {
            Container.Clear();
            Create();
        }
    }
}