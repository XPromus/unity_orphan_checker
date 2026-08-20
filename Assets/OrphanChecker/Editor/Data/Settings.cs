using System.Collections.Generic;
using System.Linq;

namespace OrphanChecker.Editor.Data
{
    public class Settings
    {
        public float Scale = 1f;
        public List<FileType> Types = new()
        {
            new FileType { TypeString = "t:Material", HeaderText = "Materials", Active = true }, 
            new FileType { TypeString = "t:Prefab", HeaderText = "Prefabs", Active = true }, 
            new FileType { TypeString = "t:MonoScript", HeaderText = "Scripts", Active = true },
        };

        public readonly List<FileType> CommonFileTypes = new();

        public Settings()
        {
            var commonTokens = AssetTypeCatalog.CommonTypes;
            foreach (var commonToken in commonTokens)
            {
                CommonFileTypes.Add(FileTypeFromToken(commonToken));
            }
        }

        public void FillTokensFromProject()
        {
            var tokens = AssetTypeCatalog.GetAssetTypeTokens();
            var uniqueTokens = tokens.Where(t => !CommonFileTypes.Select(c => c.TypeString).Contains(t)).ToArray();
            Types.AddRange(uniqueTokens.Select(FileTypeFromToken));
        }

        private static FileType FileTypeFromToken(string token)
        {
            return new FileType
            {
                TypeString = token,
                HeaderText = $"{token.Split(":")[^1]}s",
                Active = true,
            };
        }
    }

    public static class SettingsInstance
    {
        private static readonly Settings Instance = new();
        public static Settings GetInstance() => Instance;
    }
}