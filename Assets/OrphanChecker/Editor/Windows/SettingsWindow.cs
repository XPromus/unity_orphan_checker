using OrphanChecker.Data;
using UnityEngine;
using UnityEngine.UIElements;

namespace OrphanChecker.Editor.Windows
{
    public class SettingsWindow
    {
        private Settings _settings;
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
                style =
                {
                    fontSize = DefaultHeaderFontSize,
                    marginBottom = 10
                }
            });

            for (var i = 0; i < _settings.Types.Count; i++)
            {
                var index = i;
                var type = _settings.Types[index];
                var typeContainer = new VisualElement
                {
                    style =
                    {
                        flexDirection = FlexDirection.Row,
                        width = Length.Percent(100)
                    }
                };
                typeContainer.Add(new Label
                {
                    text = type.TypeString,
                    style = { flexGrow = 1}
                });
                
                var activeToggle = new Toggle
                {
                    value = _settings.Types[index].Active,
                    label = "Active",
                    style = { flexShrink = 1 }
                };
                activeToggle.RegisterValueChangedCallback(evt =>
                {
                    var fileType = _settings.Types[index];
                    fileType.Active = evt.newValue;
                    _settings.Types[index] = fileType;
                });
                typeContainer.Add(activeToggle);
                
                typeContainer.Add(new Button(() =>
                {
                    _settings.Types.RemoveAt(index);
                    RenderInterface();
                })
                {
                    text = "Remove",
                    style = { flexBasis = Length.Percent(20) }
                });
                container.Add(typeContainer);
            }

            var newTypeInput = new TextField
            {
                label = "New Type",
                style =
                {
                    width = Length.Percent(100),
                    marginTop = 10
                }
            };
            container.Add(newTypeInput);
            var addNewTypeInputButton = new Button(() =>
            {
                var inputContent = newTypeInput.value;

                if (inputContent.Length < 3)
                {
                    Debug.LogError("Types must have at least 3 characters");
                    return;
                }
                if (!inputContent.StartsWith("t:"))
                {
                    Debug.LogError("Types must start with t:");
                    return;
                }

                var newFileType = new FileType
                {
                    TypeString = inputContent,
                    Active = true,
                };
                _settings.Types.Add(newFileType);
                RenderInterface();
            })
            {
                text = "Add",
                style = { width = Length.Percent(100) }
            };
            container.Add(addNewTypeInputButton);
            
            return container;
        }
    }
}