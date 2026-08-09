using System.Collections.Generic;
using System.Linq;
using OrphanChecker.Data;

namespace OrphanChecker.Editor
{
    public class OrphanDatabase
    {
        public List<Orphan> Orphans;
        public Dictionary<string, int> ReferenceCount;

        public OrphanDatabase()
        {
            ReferenceCount = OrphanScanner.BuildReferenceCounts();
            Orphans = OrphanScanner.FindOrphans(ReferenceCount);
        }

        public void UpdateOrphanList()
        {
            UpdateReferenceCount();
            Orphans = OrphanScanner.FindOrphans(ReferenceCount);
        }

        public void UpdateReferenceCount()
        {
            ReferenceCount = OrphanScanner.BuildReferenceCounts();
        }

        public void Serialize()
        {
            
        }

        public int GetOrphanCountByType(string orphanType)
        {
            return Orphans.Count(o => o.FilterType.Equals(orphanType));
        }

        public List<(string FilterType, List<Orphan> Orphans)> GetAllOrphanCounts()
        {
            return Orphans
                .GroupBy(o => o.FilterType)
                .Select(g => (FilterType: g.Key, Orphans: g.ToList()))
                .OrderByDescending(g => g.Orphans.Count)
                .ToList();
        }
    }

    public static class OrphanDatabaseInstance
    {
        private static readonly OrphanDatabase Instance = new();
        public static OrphanDatabase GetInstance() => Instance;
    }
}