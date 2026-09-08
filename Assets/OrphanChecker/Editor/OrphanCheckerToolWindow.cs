using System.Collections.Generic;
using System.Linq;
using OrphanChecker.Editor.Windows;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace OrphanChecker.Editor
{
    public class OrphanCheckerToolWindow : EditorWindow
    {
        private readonly Dictionary<string, Window> _windows = new();
        private readonly Dictionary<string, Tab> _tabs = new();
        
        [MenuItem("Tools/Orphan Checker")]
        public static void ShowWindow()
        {
            var window = GetWindow<OrphanCheckerToolWindow>();
            window.titleContent = new GUIContent("Orphan Checker");
            window.minSize = new Vector2(300f, 200f);
        }

        public void CreateGUI()
        {
            var tabs = WindowRegistry.GetTabs();
            if (tabs.Count == 0)
            {
                Debug.LogError("[OrphanChecker] No [Window] types found.");
                return;
            }
            
            var tabView = new TabView();
            
            foreach (var entry in tabs)
            {
                var newTab = new Tab(entry.Label);
                newTab.Add(entry.Instance.Create());
                tabView.Add(newTab);

                _windows[entry.Label] = entry.Instance;
                _tabs[entry.Label] = newTab;
            }

            tabView.activeTabChanged += (_, newTab) =>
            {
                _windows[newTab.label].FullReload();
            };

            var defaultLabel = tabs.FirstOrDefault(t => t.IsDefault).Label ?? tabs[0].Label;
            tabView.activeTab = _tabs[defaultLabel];
            rootVisualElement.Add(tabView);
        }
    }
}
