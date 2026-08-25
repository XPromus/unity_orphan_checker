using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace OrphanChecker.Editor.Controls
{
    public class PieChart : VisualElement
    {
        private struct Slice
        {
            public string Label;
            public string Tooltip;
            public float Value;
            public Color Color;
        }

        private const float SegmentAngleDegrees = 2.5f;
        private const float RadiusPadding = 0.95f;
        private const float HoverBrightness = 0.35f;

        private static readonly Color[] Pallete =
        {
            new(0.26f, 0.62f, 0.88f),
            new(0.95f, 0.61f, 0.27f),
            new(0.47f, 0.78f, 0.36f),
            new(0.83f, 0.33f, 0.33f),
            new(0.58f, 0.40f, 0.74f),
            new(0.24f, 0.71f, 0.71f),
            new(0.90f, 0.76f, 0.27f),
            new(0.55f, 0.55f, 0.60f),
            new(0.93f, 0.44f, 0.66f),
            new(0.42f, 0.55f, 0.92f),
            new(0.72f, 0.53f, 0.04f),
            new(0.30f, 0.69f, 0.43f),
        };

        private readonly List<Slice> _slices = new();
        private int _hoverIndex = -1;

        public PieChart()
        {
            generateVisualContent += DrawPie;
            RegisterCallback<GeometryChangedEvent>(_ => MarkDirtyRepaint());
            RegisterCallback<PointerMoveEvent>(OnPointerMove);
            RegisterCallback<PointerLeaveEvent>(_ => SetHover(-1));
            RegisterCallback<TooltipEvent>(OnTooltip);
        }

        public void SetData(IList<(string label, string tooltip, float value)> data)
        {
            _slices.Clear();
            var colorIndex = 0;
            foreach (var entry in data)
            {
                if (entry.value <= 0f) continue;
                _slices.Add(new Slice
                {
                    Label = entry.label,
                    Tooltip = entry.tooltip,
                    Value = entry.value,
                    Color = Pallete[colorIndex % Pallete.Length],
                });
                colorIndex++;
            }
            SetHover(-1);
            MarkDirtyRepaint();
        }

        public Color GetSliceColor(int index)
        {
            return index >= 0 && index < _slices.Count ? _slices[index].Color : Color.clear;
        }

        private void DrawPie(MeshGenerationContext context)
        {
            var rect = contentRect;
            if (rect.width < 1f || rect.height < 1f || _slices.Count == 0) return;

            var (center, radius, total) = GetGeometry();

            var segmentCounts = new int[_slices.Count];
            var vertexCount = 0;
            var indexCount = 0;
            for (var i = 0; i < _slices.Count; i++)
            {
                var segments = SegmentsForSlice(i, total);
                segmentCounts[i] = segments;
                vertexCount += segments + 2;
                indexCount += segments * 3;
            }

            var mesh = context.Allocate(vertexCount, indexCount);
            var vertexBase = 0;
            var angle = -90f;

            for (var i = 0; i < _slices.Count; i++)
            {
                var slice = _slices[i];
                var segments = segmentCounts[i];
                var span = slice.Value / total * 360f;
                var color = i == _hoverIndex
                    ? Color.Lerp(slice.Color, Color.white, HoverBrightness)
                    : slice.Color;

                mesh.SetNextVertex(new Vertex { position = new Vector3(center.x, center.y, Vertex.nearZ), tint = color });
                for (var s = 0; s <= segments; s++)
                {
                    var radians = Mathf.Deg2Rad * (angle + span * s / segments);
                    var position = center + new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)) * radius;
                    mesh.SetNextVertex(new Vertex { position = new Vector3(position.x, position.y, Vertex.nearZ), tint = color });
                }

                for (var s = 0; s < segments; s++)
                {
                    mesh.SetNextIndex((ushort)vertexBase);
                    mesh.SetNextIndex((ushort)(vertexBase + 1 + s));
                    mesh.SetNextIndex((ushort)(vertexBase + 2 + s));
                }

                vertexBase += segments + 2;
                angle += span;
            }
        }

        private void OnPointerMove(PointerMoveEvent evt)
        {
            if (_slices.Count == 0)
            {
                SetHover(-1);
                return;
            }

            var (center, radius, total) = GetGeometry();
            var delta = evt.localPosition - new Vector3(center.x, center.y);
            SetHover(delta.magnitude > radius ? -1 : FindSliceAtPointer(delta, total));
        }

        private int FindSliceAtPointer(Vector2 delta, float total)
        {
            var degrees = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg + 90f;
            if (degrees < 0f) degrees += 360f;

            var accumulated = 0f;
            for (var i = 0; i < _slices.Count; i++)
            {
                accumulated += _slices[i].Value / total * 360f;
                if (degrees < accumulated) return i;
            }
            return _slices.Count - 1;
        }

        private (Vector2 center, float radius, float total) GetGeometry()
        {
            var rect = contentRect;
            return (
                rect.center,
                Mathf.Min(rect.width, rect.height) * 0.5f * RadiusPadding,
                _slices.Sum(s => s.Value)
            );
        }

        private int SegmentsForSlice(int index, float total)
        {
            var span = _slices[index].Value / total * 360f;
            return Mathf.Max(1, Mathf.CeilToInt(span / SegmentAngleDegrees));
        }

        private void SetHover(int index)
        {
            if (_hoverIndex == index) return;
            _hoverIndex = index;
            MarkDirtyRepaint();
        }

        private void OnTooltip(TooltipEvent evt)
        {
            if (_hoverIndex < 0) return;
            evt.tooltip = _slices[_hoverIndex].Tooltip;
        }
    }
}
