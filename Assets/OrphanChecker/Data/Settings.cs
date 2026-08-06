using System.Collections.Generic;

namespace OrphanChecker.Data
{
    public class Settings
    {
        public float Scale = 1f;
        public List<FileType> Types = new()
        {
            new FileType { TypeString = "t:Material", Active = true }, 
            new FileType { TypeString = "t:Prefab", Active = true }, 
            new FileType { TypeString = "t:MonoScript", Active = true },
        };
    }

    public static class SettingsInstance
    {
        private static readonly Settings Instance = new();
        public static Settings GetInstance() => Instance;
    }
}