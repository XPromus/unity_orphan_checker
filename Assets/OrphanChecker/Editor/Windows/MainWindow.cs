using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace OrphanChecker.Editor.Windows
{
    public class MainWindow : Window
    {
        private readonly OrphanDatabase _orphanDatabase = OrphanDatabaseInstance.GetInstance();
        
        private VisualElement _orphanListContainer;
        private ScrollView _orphanListScrollView;
        
        private const int DefaultHeaderFontSize = 20;
        private const float Spacing = 10;
        
        public override VisualElement Create()
        {
            var checkButton = new Button(FullReload)
            {
                text = "Check",
                style =
                {
                    width = Length.Percent(100),
                    marginBottom = Spacing,
                }
            };
            Container.Add(checkButton);
            
            _orphanListContainer = new VisualElement
            {
                style =
                {
                    marginRight = 10,
                }
            };
            _orphanListScrollView = new ScrollView(ScrollViewMode.Vertical)
            {
                style =
                {
                    flexGrow = 1,
                    width = Length.Percent(100)
                }
            };
            _orphanListScrollView.Add(_orphanListContainer);
            Container.Add(_orphanListScrollView);

            var selectAllButton = new Button(SelectAll)
            {
                text = "Select All",
                style =
                {
                    width = Length.Percent(100),
                    marginTop = Spacing,
                }
            };
            Container.Add(selectAllButton);
            
            var clearSelectedButton = new Button(() =>
            {
                for (var i = 0; i < _orphanDatabase.Orphans.Count; i++)
                {
                    var orphan = _orphanDatabase.Orphans[i];
                    orphan.Toggled = false;
                    _orphanDatabase.Orphans[i] = orphan;    
                }
                
                RebuildOrphanList();
            })
            {
                text = "Clear Selected",
                style =
                {
                    width = Length.Percent(100)
                }
            };
            Container.Add(clearSelectedButton);

            var moveSelectedToTrashButton = new Button(() =>
            {
                var toDelete = _orphanDatabase.Orphans.Where(o => o.Toggled).Select(o => o.Path).ToArray();
                var failedDeletes = new List<string>();
                var success = AssetDatabase.DeleteAssets(toDelete, failedDeletes);
                AssetDatabase.Refresh();
                
                FullReload();
            })
            {
                text = "Selected To Trash",
                style =
                {
                    width = Length.Percent(100),
                }
            };
            moveSelectedToTrashButton.AddToClassList("deleteButton");
            Container.Add(moveSelectedToTrashButton);
            
            var deleteSelectedButton = new Button(() =>
            {
                var toDelete = _orphanDatabase.Orphans.Where(o => o.Toggled).Select(o => o.Path).ToArray();
                var failedDeletes = new List<string>();
                var success = AssetDatabase.MoveAssetsToTrash(toDelete, failedDeletes);
                AssetDatabase.Refresh();

                FullReload();
            })
            {
                text = "Delete Selected",
                style =
                {
                    width = Length.Percent(100),
                }
            };
            deleteSelectedButton.AddToClassList("deleteButton");
            Container.Add(deleteSelectedButton);
            
            FullReload();
            return Container;
        }
        
        private void RebuildOrphanList()
        {
            _orphanListContainer.Clear();

            var containerDictionary = new Dictionary<string, VisualElement>();
            foreach (var settingsCommonFileType in Settings.CommonFileTypes)
            {
                if (settingsCommonFileType.Active)
                {
                    var newOrphanContainer = CreateOrphanContainer(settingsCommonFileType.HeaderText, settingsCommonFileType.TypeString);
                    _orphanListContainer.Add(newOrphanContainer);
                    containerDictionary.Add(settingsCommonFileType.TypeString, newOrphanContainer);
                }
            }
            
            for (var i = 0; i < _orphanDatabase.Orphans.Count; i++)
            {
                var orphanEntry = CreateOrphanEntry(i);
                var filterType = _orphanDatabase.Orphans[i].FilterType;
                if (containerDictionary.ContainsKey(filterType))
                {
                    containerDictionary[filterType].Add(orphanEntry);
                }
            }
        }

        private VisualElement CreateOrphanContainer(string title, string type)
        {
            var container = new VisualElement
            {
                style =
                {
                    //backgroundColor = Color.gray2,
                    borderTopWidth = 1f,
                    borderBottomWidth = 1f,
                    borderLeftWidth = 1f,
                    borderRightWidth = 1f,
                    borderTopColor = Color.white,
                    borderBottomColor = Color.white,
                    borderLeftColor = Color.white,
                    borderRightColor = Color.white,
                    marginBottom = Spacing,
                    paddingTop = 10,
                    paddingBottom = 10,
                    paddingLeft = 10,
                    paddingRight = 10,
                    borderTopLeftRadius = 5,
                    borderTopRightRadius = 5,
                    borderBottomRightRadius = 5,
                    borderBottomLeftRadius = 5,
                }
            };
            container.Add(new Label(title)
            {
                style =
                {
                    fontSize = DefaultHeaderFontSize * Settings.Scale,
                    unityFontStyleAndWeight = FontStyle.Bold
                }
            });
            var orphanTypeCounter = _orphanDatabase.GetOrphanCountByType(type);
            var orphanTypeCounterLabel = new Label
            {
                text = $"{orphanTypeCounter} orphans",
                style =
                {
                    fontSize = 16,
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
            
            var toggle = new Toggle { value = _orphanDatabase.Orphans[index].Toggled };
            toggle.RegisterValueChangedCallback(evt =>
            {
                var orphan = _orphanDatabase.Orphans[index];
                orphan.Toggled = evt.newValue;
                _orphanDatabase.Orphans[index] = orphan;
                            
                RebuildOrphanList();
            });
            container.Add(toggle);
            
            var showButton = new Button(() =>
            {
                Utils.PingAsset(_orphanDatabase.Orphans[index].Path);
            })
            {
                text = "Show"
            };
            container.Add(showButton);

            var deleteButton = new Button(() =>
            {
                AssetDatabase.DeleteAsset(_orphanDatabase.Orphans[index].Path);
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
                AssetDatabase.MoveAssetToTrash(_orphanDatabase.Orphans[index].Path);
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
                text = Path.GetFileNameWithoutExtension(_orphanDatabase.Orphans[index].Path),
                style =
                {
                    unityFontStyleAndWeight = _orphanDatabase.Orphans[index].Toggled ? FontStyle.Bold : FontStyle.Normal
                }
            };
            container.Add(orphanLabel);
            
            return container;
        }

        public override void FullReload()
        {
            _orphanDatabase.UpdateOrphanList();
            RebuildOrphanList();
        }

        private void SelectAll()
        {
            for (var i = 0; i < _orphanDatabase.Orphans.Count; i++)
            {
                var orphan = _orphanDatabase.Orphans[i];
                orphan.Toggled = true;
                _orphanDatabase.Orphans[i] = orphan;
            }
            
            RebuildOrphanList();
        }
    }
}