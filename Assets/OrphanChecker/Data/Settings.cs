using System.Collections.Generic;

namespace OrphanChecker.Data
{
    public class Settings
    {
        public float Scale = 1f;
        public List<string> Types = new() { "t:Material", "t:Prefab", "t:MonoScript" };
    }

    public static class SettingsInstance
    {
        private static readonly Settings Instance = new();
        public static Settings GetInstance() => Instance;
    }
}