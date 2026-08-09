using System.Linq;
using UnityEngine.UIElements;

namespace OrphanChecker.Editor.Windows
{
    public class OverviewWindow : Window
    {
        private readonly OrphanDatabase _orphanDatabase = OrphanDatabaseInstance.GetInstance();
        
        public override VisualElement Create()
        {
            Container.Add(new Label("Overview"));

            var sortedOrphanCategories = _orphanDatabase.GetAllOrphanCounts();
            foreach (var orphan in sortedOrphanCategories)
            {
                var orphanContainer = new VisualElement();
                orphanContainer.Add(new Label(orphan.FilterType));
                orphanContainer.Add(new Label(orphan.Orphans.Count.ToString()));
                orphanContainer.Add(new Label(Utils.FormatBytes(orphan.Orphans.Sum(o => o.Size))));
                Container.Add(orphanContainer);
            }
            
            return Container;
        }

        public override void FullReload()
        {
            Container.Clear();
            Create();
        }
    }
}