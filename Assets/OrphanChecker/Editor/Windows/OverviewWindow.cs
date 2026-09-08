using System.Linq;
using UnityEngine.UIElements;

namespace OrphanChecker.Editor.Windows
{
    [Window("Overview", 0)]
    public class OverviewWindow : Window
    {
        private readonly OrphanDatabase _orphanDatabase = OrphanDatabaseInstance.GetInstance();
        
        public override VisualElement Create()
        {
            Container.Add(new Label("Overview"));

            Container.Add(CreateSizeOfOrphansPanel());
            
            /*
            foreach (var orphan in sortedOrphanCategories)
            {
                var orphanContainer = new VisualElement();
                orphanContainer.Add(new Label(orphan.FilterType));
                orphanContainer.Add(new Label(orphan.Orphans.Count.ToString()));
                orphanContainer.Add(new Label()));
                Container.Add(orphanContainer);
            }
            */
            
            return Container;
        }

        private VisualElement CreateSizeOfOrphansPanel()
        {
            var sortedOrphanCategories = _orphanDatabase.GetAllOrphanCounts();
            
            var container = new VisualElement();
            var size = Utils.FormatBytes(sortedOrphanCategories.Sum(orphanType =>
            {
                return orphanType.Orphans.Sum(orphan => orphan.Size);
            }));
            
            container.Add(new Label($"Size of all orphans: {size}"));
            
            return container;
        }

        public override void FullReload()
        {
            Container.Clear();
            Create();
        }
    }
}