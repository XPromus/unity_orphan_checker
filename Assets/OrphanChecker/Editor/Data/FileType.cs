using System;

namespace OrphanChecker.Editor.Data
{
    [Serializable]
    public struct FileType
    {
        public string typeString;
        public string headerText;
        public bool active;
    }
}