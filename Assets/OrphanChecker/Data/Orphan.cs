namespace OrphanChecker.Data
{
    public struct Orphan
    {
        public string Guid;
        public string Path;
        public string FilterType;
        public long Size;
        public OrphanType Type;
        public bool Toggled;
    }
}