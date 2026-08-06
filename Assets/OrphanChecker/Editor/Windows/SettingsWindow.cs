using OrphanChecker.Data;
using UnityEngine.UIElements;

namespace OrphanChecker.Editor.Windows
{
    public class SettingsWindow
    {
        private readonly Settings _settings;
        private VisualElement _settingsContainer;
        
        private const int DefaultHeaderFontSize = 24;

        public SettingsWindow(Settings settings)
        {
            _settings = settings;
        }
        
        public VisualElement Create()
        {
            var container = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Column,
                    paddingBottom = 10,
                    paddingTop = 10,
                    paddingLeft = 10,
                    paddingRight = 10
                }
            };
            
            _settingsContainer = new VisualElement();
            
            RenderInterface();
            
            var applyButton = new Button(RenderInterface)
            {
                text = "Apply",
                style = { width = Length.Percent(100) }
            };
            
            container.Add(_settingsContainer);
            container.Add(Utils.CreateDivider());
            container.Add(applyButton);
            
            return container;
        }

        private void RenderInterface()
        {
            _settingsContainer.Clear();
            
            var fontScaleInput = new FloatField
            {
                value = 1f,
                tooltip = "Set the scale of the front. Default is 1",
                style =
                {
                    width = Length.Percent(100),
                },
                label = "Font Scale"
            };
            fontScaleInput.RegisterValueChangedCallback(evt =>
            {
                _settings.Scale = evt.newValue;
            });
            
            _settingsContainer.Add(fontScaleInput);
            _settingsContainer.Add(Utils.CreateDivider());
            _settingsContainer.Add(RenderFileTypeToCheckContainer());
        }

        private VisualElement RenderFileTypeToCheckContainer()
        {
            var container = new VisualElement
            {
                style = { width = Length.Percent(100) }
            };
            
            container.Add(new Label("Filetypes")
            {
                style = { fontSize = DefaultHeaderFontSize }
            });
            foreach (var settingsType in _settings.Types)
            {
                var typeContainer = new VisualElement
                {
                    style = { flexDirection = FlexDirection.Row }
                };
                typeContainer.Add(new Label
                {
                    text = settingsType,
                    style = { width = Length.Percent(80) }
                });
                typeContainer.Add(new Button
                {
                    text = "Remove",
                    style = { width = Length.Percent(20) }
                });
                container.Add(typeContainer);
            }

            var newTypeInput = new TextField
            {
                label = "New Type",
                style = { width = Length.Percent(100) }
            };
            container.Add(newTypeInput);
            var addNewTypeInputButton = new Button
            {
                text = "Add",
                style = { width = Length.Percent(100) }
            };
            container.Add(addNewTypeInputButton);
            
            return container;
        }
    }
}