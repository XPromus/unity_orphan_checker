using UnityEngine.UIElements;

namespace OrphanChecker.Editor.Windows
{
    public class OverviewWindow : Window
    {
        public override VisualElement Create()
        {
            Container.Add(new Label("Overview"));
            return Container;
        }

        public override void FullReload()
        {
            throw new System.NotImplementedException();
        }
    }
}