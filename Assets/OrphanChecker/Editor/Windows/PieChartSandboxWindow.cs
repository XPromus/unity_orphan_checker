using System;
using System.Collections.Generic;
using System.Linq;
using OrphanChecker.Editor.Controls;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace OrphanChecker.Editor.Windows
{
    public class PieChartSandboxWindow : EditorWindow
    {
        private enum Metric
        {
            Count,
            Size,
        }

        private readonly OrphanDatabase _orphanDatabase = OrphanDatabaseInstance.GetInstance();

        private Metric _metric = Metric.Count;
        private bool _useSampleData;

        private PieChart _pieChart;
        private VisualElement _legendContainer;
        private Label _emptyLabel;

        [MenuItem("Tools/Pie Chart Sandbox")]
        public static void ShowWindow()
        {
            var window = GetWindow<PieChartSandboxWindow>();
            window.titleContent = new GUIContent("Pie Sandbox");
            window.minSize = new Vector2(320f, 420f);
        }

        public void CreateGUI()
        {
            var toolbar = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    alignItems = Align.Center,
                    marginBottom = 8f,
                }
            };
            var metricField = new EnumField("Metric", _metric) { style = { flexGrow = 1f } };
            metricField.RegisterValueChangedCallback(evt =>
            {
                _metric = (Metric)evt.newValue;
                Refresh();
            });
            toolbar.Add(metricField);
            toolbar.Add(new Button(ToggleSampleData) { text = "Sample Data" });

            _pieChart = new PieChart { style = { height = 260f, marginTop = 10f } };
            _legendContainer = new VisualElement { style = { marginTop = 10f, marginLeft = 20f } };
            _emptyLabel = new Label("Nothing to display.") { style = { unityTextAlign = TextAnchor.MiddleCenter } };

            rootVisualElement.Add(toolbar);
            rootVisualElement.Add(_pieChart);
            rootVisualElement.Add(_legendContainer);
            rootVisualElement.Add(_emptyLabel);

            Refresh();
        }

        private void ToggleSampleData()
        {
            _useSampleData = !_useSampleData;
            Refresh();
        }

        private void Refresh()
        {
            var visibleGroups = CollectGroups()
                .Where(g => g.total > 0)
                .OrderByDescending(g => g.total)
                .ToList();
            var grandTotal = visibleGroups.Sum(g => g.total);
            var hasData = grandTotal > 0;

            _pieChart.style.display = hasData ? DisplayStyle.Flex : DisplayStyle.None;
            _legendContainer.style.display = hasData ? DisplayStyle.Flex : DisplayStyle.None;
            _emptyLabel.style.display = hasData ? DisplayStyle.None : DisplayStyle.Flex;
            if (!hasData)
            {
                _pieChart.SetData(Array.Empty<(string, string, float)>());
                return;
            }

            var data = new List<(string label, string tooltip, float value)>(visibleGroups.Count);
            foreach (var group in visibleGroups)
            {
                var percent = group.total / grandTotal * 100f;
                var detail = _metric == Metric.Size ? Utils.FormatBytes(group.total) : $"{group.orphans.Length}";
                data.Add((group.label, $"{group.label} — {detail} ({percent:F1}%)", group.total));
            }

            _pieChart.SetData(data);

            _legendContainer.Clear();
            for (var i = 0; i < data.Count; i++)
            {
                _legendContainer.Add(CreateLegendRow(data[i].label, data[i].tooltip, _pieChart.GetSliceColor(i)));
            }
        }

        private List<(string label, Data.Orphan[] orphans, long total)> CollectGroups()
        {
            if (_useSampleData)
            {
                return new List<(string, Data.Orphan[], long)>
                {
                    ("Sample Type 1", CreateSampleOrphans(14, 2048), 2048L * 14),
                    ("Sample Type 2", CreateSampleOrphans(8, 512000), 512000L * 8),
                    ("Sample Type 3", CreateSampleOrphans(5, 1024), 1024L * 5),
                    ("Sample Type 4", CreateSampleOrphans(3, 7340032), 7340032L * 3),
                    ("Sample Type 5", CreateSampleOrphans(1, 4096), 4096L),
                };
            }

            return _orphanDatabase.GetAllOrphanCounts()
                .Select(g => (g.FilterType, g.Orphans.ToArray(), g.Orphans.Sum(o => o.Size)))
                .ToList();
        }

        private static VisualElement CreateLegendRow(string label, string tooltipText, Color color)
        {
            var row = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    alignItems = Align.Center,
                    marginBottom = 4f,
                }
            };
            row.RegisterCallback<TooltipEvent>(evt => evt.tooltip = tooltipText);
            row.Add(new VisualElement
            {
                style =
                {
                    width = 12f,
                    height = 12f,
                    marginRight = 6f,
                    borderTopLeftRadius = 2f,
                    borderTopRightRadius = 2f,
                    borderBottomLeftRadius = 2f,
                    borderBottomRightRadius = 2f,
                    backgroundColor = color,
                }
            });
            row.Add(new Label(label));
            return row;
        }

        private static Data.Orphan[] CreateSampleOrphans(int count, long size)
        {
            var orphans = new Data.Orphan[count];
            for (var i = 0; i < count; i++)
            {
                orphans[i] = new Data.Orphan { Size = size };
            }
            return orphans;
        }
    }
}
