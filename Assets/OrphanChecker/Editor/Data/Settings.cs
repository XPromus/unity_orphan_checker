using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace OrphanChecker.Editor.Data
{
    [Serializable]
    public class Settings
    {
        public float scale = 1f;
        public List<FileType> types = new();
        public List<FileType> commonFileTypes = new();

        private static string FilePath => Path.Combine
        (
            Application.dataPath,
            "..",
            "ProjectSettings",
            "OrphanCheckerSettings.json"
        );
        
        public Settings()
        {
            var commonTokens = AssetTypeCatalog.CommonTypes;
            foreach (var commonToken in commonTokens)
            {
                commonFileTypes.Add(FileTypeFromToken(commonToken));
            }
        }

        public void FillTokensFromProject()
        {
            var tokens = AssetTypeCatalog.GetAssetTypeTokens();
            var uniqueTokens = tokens.Where(t => !commonFileTypes.Select(c => c.typeString).Contains(t)).ToArray();
            types.AddRange(uniqueTokens.Select(FileTypeFromToken));
        }

        public void Save()
        {
            File.WriteAllText(FilePath, JsonUtility.ToJson(this, true));
        }

        public static Settings Load()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    var loaded = JsonUtility.FromJson<Settings>(File.ReadAllText(FilePath));
                    loaded.ReconcileCommonFileTypes();
                    return loaded;
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"OrphanChecker: Could not load settings ({e.Message}). Using defaults.");
            }

            return new Settings();
        }

        private void ReconcileCommonFileTypes()
        {
            foreach (var token in AssetTypeCatalog.CommonTypes)
            {
                if (commonFileTypes.All(c => c.typeString != token))
                {
                    commonFileTypes.Add(FileTypeFromToken(token));
                }
            }
        }

        private static FileType FileTypeFromToken(string token)
        {
            return new FileType
            {
                typeString = token,
                headerText = $"{token.Split(":")[^1]}s",
                active = true,
            };
        }
    }

    public static class SettingsInstance
    {
        private static readonly Settings Instance = Settings.Load();
        public static Settings GetInstance() => Instance;
    }
}