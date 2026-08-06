using OrphanChecker.Editor.Windows;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace OrphanChecker.Editor
{
    public class OrphanCheckerToolWindow : EditorWindow
    {
        private MainWindow _mainWindow;
        private SettingsWindow _settingsWindow;
        
        [MenuItem("Tools/Orphan Checker")]
        public static void ShowWindow()
        {
            var window = GetWindow<OrphanCheckerToolWindow>();
            window.titleContent = new GUIContent("Orphan Checker");
            window.minSize = new Vector2(300f, 200f);
        }

        public void CreateGUI()
        {
            _mainWindow = new MainWindow();
            _settingsWindow = new SettingsWindow();
            
            var tabView = new TabView();
            var mainTab = new Tab("Main");
            mainTab.Add(_mainWindow.Create());
            var settingsTab = new Tab("Settings");
            settingsTab.Add(_settingsWindow.Create());
            
            tabView.Add(mainTab);
            tabView.Add(settingsTab);
            
            rootVisualElement.Add(tabView);
        }
    }
}
