using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OrphanChecker.Data;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace OrphanChecker.Editor.Windows
{
    public class MainWindow
    {
        private List<Orphan> _orphans = new();
        private Dictionary<string, int> _referenceCounts;
        private VisualElement _orphanListContainer;

        private StyleSheet _styleSheet;

        private readonly Settings _settings;
        private const int DefaultHeaderFontSize = 24;

        public MainWindow(Settings settings)
        {
            _settings = settings;
        }
        
        public VisualElement Create()
        {
            var container = new VisualElement
            {
                style =
                {
                    paddingBottom = 10,
                    paddingTop = 10,
                    paddingLeft = 10,
                    paddingRight = 10
                }
            };

            _styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/OrphanChecker/Editor/OrphanChecker.uss");
            container.styleSheets.Add(_styleSheet);
            
            var checkButton = new Button(FullReload)
            {
                text = "Check",
                style = { width = Length.Percent(100) }
            };
            
            container.Add(checkButton);

            _orphanListContainer = new VisualElement();
            container.Add(_orphanListContainer);

            var clearSelectedButton = new Button(() =>
            {
                for (var i = 0; i < _orphans.Count; i++)
                {
                    var orphan = _orphans[i];
                    orphan.Toggled = false;
                    _orphans[i] = orphan;    
                }
                
                RebuildOrphanList();
            })
            {
                text = "Clear Selected",
                style = { width = Length.Percent(100) }
            };
            container.Add(clearSelectedButton);

            var moveSelectedToTrashButton = new Button(() =>
            {
                var toDelete = _orphans.Where(o => o.Toggled).Select(o => o.Path).ToArray();
                var failedDeletes = new List<string>();
                var success = AssetDatabase.DeleteAssets(toDelete, failedDeletes);
                AssetDatabase.Refresh();
                
                FullReload();
            })
            {
                text = "Selected To Trash",
                style = { width = Length.Percent(100) }
            };
            moveSelectedToTrashButton.AddToClassList("deleteButton");
            container.Add(moveSelectedToTrashButton);
            
            var deleteSelectedButton = new Button(() =>
            {
                var toDelete = _orphans.Where(o => o.Toggled).Select(o => o.Path).ToArray();
                var failedDeletes = new List<string>();
                var success = AssetDatabase.MoveAssetsToTrash(toDelete, failedDeletes);
                AssetDatabase.Refresh();

                FullReload();
            })
            {
                text = "Delete Selected",
                style = { width = Length.Percent(100) }
            };
            deleteSelectedButton.AddToClassList("deleteButton");
            container.Add(deleteSelectedButton);
            
            FullReload();
            return container;
        }
        
        private void RebuildOrphanList()
        {
            _orphanListContainer.Clear();

            var scriptsContainer = CreateOrphanContainer("Materials", OrphanType.Script);
            var prefabsContainer = CreateOrphanContainer("Prefabs", OrphanType.Prefab);
            var materialContainer = CreateOrphanContainer("Materials", OrphanType.Material);
            
            for (var i = 0; i < _orphans.Count; i++)
            {
                switch (_orphans[i].Type)
                {
                    case OrphanType.Script:
                        scriptsContainer.Add(CreateOrphanEntry(i));
                        break;
                    case OrphanType.Prefab:
                        prefabsContainer.Add(CreateOrphanEntry(i));
                        break;
                    case OrphanType.Material:
                        materialContainer.Add(CreateOrphanEntry(i));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            
            _orphanListContainer.Add(Utils.CreateDivider());
            _orphanListContainer.Add(scriptsContainer);
            _orphanListContainer.Add(Utils.CreateDivider());
            _orphanListContainer.Add(prefabsContainer);
            _orphanListContainer.Add(Utils.CreateDivider());
            _orphanListContainer.Add(materialContainer);
            _orphanListContainer.Add(Utils.CreateDivider());
        }

        private VisualElement CreateOrphanContainer(string title, OrphanType type)
        {
            var container = new VisualElement();
            container.Add(new Label(title)
            {
                style =
                {
                    fontSize = DefaultHeaderFontSize * _settings.Scale,
                    unityFontStyleAndWeight = FontStyle.Bold
                }
            });
            var orphanTypeCounter = GetOrphanCount(type);
            var orphanTypeCounterLabel = new Label
            {
                text = $"{orphanTypeCounter} orphans",
                style =
                {
                    fontSize = 18,
                    unityFontStyleAndWeight = FontStyle.Bold,
                }
            };
            orphanTypeCounterLabel.AddToClassList(orphanTypeCounter == 0 ? "good" : "danger");
            container.Add(orphanTypeCounterLabel);

            return container;
        }
        
        private VisualElement CreateOrphanEntry(int index)
        {
            var container = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    alignItems = Align.Center
                }
            };
            
            var toggle = new Toggle { value = _orphans[index].Toggled };
            toggle.RegisterValueChangedCallback(evt =>
            {
                var orphan = _orphans[index];
                orphan.Toggled = evt.newValue;
                _orphans[index] = orphan;
                            
                RebuildOrphanList();
            });
            container.Add(toggle);
            
            var showButton = new Button(() =>
            {
                Utils.PingAsset(_orphans[index].Path);
            })
            {
                text = "Show"
            };
            container.Add(showButton);

            var deleteButton = new Button(() =>
            {
                AssetDatabase.DeleteAsset(_orphans[index].Path);
                AssetDatabase.Refresh();
                FullReload();
            })
            {
                text = "Delete"
            };
            deleteButton.AddToClassList("deleteButton");
            container.Add(deleteButton);

            var trashButton = new Button(() =>
            {
                AssetDatabase.MoveAssetToTrash(_orphans[index].Path);
                AssetDatabase.Refresh();
                FullReload();
            })
            {
                text = "Trash"
            };
            trashButton.AddToClassList("deleteButton");
            container.Add(trashButton);
            
            var orphanLabel = new Label
            {
                text = Path.GetFileNameWithoutExtension(_orphans[index].Path),
                style =
                {
                    unityFontStyleAndWeight = _orphans[index].Toggled ? FontStyle.Bold : FontStyle.Normal
                }
            };
            container.Add(orphanLabel);
            
            return container;
        }

        public void FullReload()
        {
            _referenceCounts = OrphanScanner.BuildReferenceCounts();
            _orphans = OrphanScanner.FindOrphans(_referenceCounts);
            RebuildOrphanList();
        }

        private int GetOrphanCount(OrphanType type)
        {
            return _orphans.Sum((orphan) => orphan.Type == type ? 1 : 0);
        }
    }
}