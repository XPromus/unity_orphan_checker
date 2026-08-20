using OrphanChecker.Editor.Windows;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace OrphanChecker.Editor
{
    public class OrphanCheckerToolWindow : EditorWindow
    {
        private Window _overviewWindow;
        private Window _mainWindow;
        private Window _settingsWindow;
        
        [MenuItem("Tools/Orphan Checker")]
        public static void ShowWindow()
        {
            var window = GetWindow<OrphanCheckerToolWindow>();
            window.titleContent = new GUIContent("Orphan Checker");
            window.minSize = new Vector2(300f, 200f);
        }

        public void CreateGUI()
        {
            _overviewWindow = new OverviewWindow();
            _mainWindow = new MainWindow();
            _settingsWindow = new SettingsWindow();
            
            var tabView = new TabView();
            var overviewTab = new Tab("Overview");
            overviewTab.Add(_overviewWindow.Create());
            var mainTab = new Tab("Main");
            mainTab.Add(_mainWindow.Create());
            var settingsTab = new Tab("Settings");
            settingsTab.Add(_settingsWindow.Create());

            tabView.activeTabChanged += (_, newTab) =>
            {
                if (newTab.label.Equals("Overview"))
                {
                    _overviewWindow.FullReload();
                }
                if (newTab.label.Equals("Main"))
                {
                    _mainWindow.FullReload();
                }
            };
            
            tabView.Add(overviewTab);
            tabView.Add(mainTab);
            tabView.Add(settingsTab);
            
            tabView.activeTab = mainTab;
            rootVisualElement.Add(tabView);
        }
    }
}
