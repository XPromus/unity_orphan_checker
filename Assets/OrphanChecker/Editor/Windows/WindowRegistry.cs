using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace OrphanChecker.Editor.Windows
{
    public static class WindowRegistry
    {
        public static IReadOnlyList<(string Label, Window Instance, bool IsDefault)> GetTabs()
        {
            var entries = new List<(int Order, string Label, Window Instance, bool IsDefault)>();

            foreach (var type in TypeCache.GetTypesDerivedFrom<Window>())
            {
                if (type.IsAbstract) continue;

                var attribute = type.GetCustomAttribute<WindowAttribute>(false);
                if (attribute == null)
                {
                    Debug.LogWarning($"[OrphanChecker] Skipping window type {type.Name}: missing [Window] attribute.");
                    continue;
                }
                
                entries.Add(
                    (
                        attribute.Order, 
                        attribute.Label, 
                        (Window) Activator.CreateInstance(type), 
                        attribute.IsDefault
                    )
                );
            }

            return entries
                .OrderBy(e => e.Order)
                .Select(e => (e.Label, e.Instance, e.IsDefault))
                .ToList();
        }
    }
}