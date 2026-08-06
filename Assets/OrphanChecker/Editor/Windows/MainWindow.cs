using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using OrphanChecker.Data;
using UnityEditor;
using UnityEditor.VersionControl;
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
            
            var checkButton = new Button(() =>
            {
                FullReload();
            })
            {
                text = "Check"
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
                text = "Clear Selected"
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
                text = "Selected To Trash"
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
            };
            deleteSelectedButton.AddToClassList("deleteButton");
            container.Add(deleteSelectedButton);
            
            RebuildOrphanList();
            return container;
        }
        
        private void RebuildOrphanList()
        {
            _orphanListContainer.Clear();

            var materialContainer = new VisualElement();
            materialContainer.Add(new Label("Materials")
            {
                style =
                {
                    fontSize = 24,
                    unityFontStyleAndWeight = FontStyle.Bold
                }
            });
            var materialOrphanCounter = GetOrphanCount(OrphanType.Material);
            var materialOrphanCounterLabel = new Label
            {
                text = $"{materialOrphanCounter} orphans",
                style =
                {
                    fontSize = 18,
                    unityFontStyleAndWeight = FontStyle.Bold,
                }
            };
            materialOrphanCounterLabel.AddToClassList(materialOrphanCounter == 0 ? "good" : "danger");
            materialContainer.Add(materialOrphanCounterLabel);

            var scriptsContainer = new VisualElement();
            scriptsContainer.Add(new Label("Scripts")
            {
                style =
                {
                    fontSize = 24,
                    unityFontStyleAndWeight = FontStyle.Bold
                }
            });
            var scriptOrphanCounter = GetOrphanCount(OrphanType.Script);
            var scriptOrphanCounterLabel = new Label
            {
                text = $"{scriptOrphanCounter} orphans",
                style =
                {
                    fontSize = 18,
                    unityFontStyleAndWeight = FontStyle.Bold
                }
            };
            scriptOrphanCounterLabel.AddToClassList(scriptOrphanCounter == 0 ? "good" : "danger");
            scriptsContainer.Add(scriptOrphanCounterLabel);
            
            for (var i = 0; i < _orphans.Count; i++)
            {
                var newOrphanContainer = new VisualElement
                {
                    style =
                    {
                        flexDirection = FlexDirection.Row,
                        alignItems = Align.Center
                    }
                };
                switch (_orphans[i].Type)
                {
                    case OrphanType.Script:
                        newOrphanContainer.Add(new Label(Path.GetFileNameWithoutExtension(_orphans[i].Path)));
                        scriptsContainer.Add(newOrphanContainer);
                        break;
                    case OrphanType.Material:
                        var index = i;
                        
                        var toggle = new Toggle { value = _orphans[index].Toggled };
                        toggle.RegisterValueChangedCallback(evt =>
                        {
                            var orphan = _orphans[index];
                            orphan.Toggled = evt.newValue;
                            _orphans[index] = orphan;
                            
                            RebuildOrphanList();
                        });
                        newOrphanContainer.Add(toggle);


                        var showButton = new Button(() =>
                        {
                            Utils.PingAsset(_orphans[index].Path);
                        })
                        {
                            text = "Show"
                        };
                        newOrphanContainer.Add(showButton);

                        var deleteButton = new Button(() =>
                        {
                            AssetDatabase.MoveAssetToTrash(_orphans[index].Path);
                            AssetDatabase.Refresh();
                            FullReload();
                        })
                        {
                            text = "Delete"
                        };
                        deleteButton.AddToClassList("deleteButton");
                        newOrphanContainer.Add(deleteButton);

                        var orphanLabel = new Label
                        {
                            text = Path.GetFileNameWithoutExtension(_orphans[index].Path),
                            style =
                            {
                                unityFontStyleAndWeight = _orphans[index].Toggled ? FontStyle.Bold : FontStyle.Normal
                            }
                        };
                        newOrphanContainer.Add(orphanLabel);
                        materialContainer.Add(newOrphanContainer);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            
            _orphanListContainer.Add(Utils.CreateDivider());
            _orphanListContainer.Add(scriptsContainer);
            _orphanListContainer.Add(Utils.CreateDivider());
            _orphanListContainer.Add(materialContainer);
            _orphanListContainer.Add(Utils.CreateDivider());
        }

        private void FullReload()
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