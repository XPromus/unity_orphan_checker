using OrphanChecker.Data;
using UnityEngine;
using UnityEngine.UIElements;

namespace OrphanChecker.Editor.Windows
{
    public class SettingsWindow : Window
    {
        private VisualElement _settingsContainer;
        
        private const int DefaultHeaderFontSize = 24;
        private const int DefaultSubHeaderFontSize = 20;
        
        public override VisualElement Create()
        {
            _settingsContainer = new VisualElement();
            
            RenderInterface();
            
            var applyButton = new Button(RenderInterface)
            {
                text = "Apply",
                style = { width = Length.Percent(100) }
            };
            
            Container.Add(_settingsContainer);
            Container.Add(Utils.CreateDivider());
            Container.Add(applyButton);
            
            return Container;
        }

        public override void FullReload()
        {
            throw new System.NotImplementedException();
        }

        private void RenderInterface()
        {
            _settingsContainer.Clear();
            
            _settingsContainer.Add(GetSettingsHeader("Visuals"));
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
                Settings.Scale = evt.newValue;
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
            
            container.Add(GetSettingsHeader("Filetypes"));
            container.Add(GetSettingsSubHeader("Common"));

            for (var i = 0; i < Settings.CommonFileTypes.Count; i++)
            {
                var type = Settings.CommonFileTypes[i];
                container.Add(GetFileTypeListEntry(type, i, true));
            }

            var customSubHeader = GetSettingsSubHeader("Custom");
            customSubHeader.style.marginTop = 10;
            container.Add(customSubHeader);
            for (var i = 0; i < Settings.Types.Count; i++)
            {
                var type = Settings.Types[i];
                container.Add(GetFileTypeListEntry(type, i, false));
            }

            var newTypeInput = new TextField
            {
                label = "New Type",
                style =
                {
                    width = Length.Percent(100),
                    marginTop = 10,
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
                    HeaderText = CreateHeaderText(inputContent),
                    Active = true,
                };
                Settings.Types.Add(newFileType);
                RenderInterface();
            })
            {
                text = "Add",
                style = { width = Length.Percent(100) }
            };
            container.Add(addNewTypeInputButton);

            var checkProjectForTypesButton = new Button(() =>
            {

            })
            {
                text = "Check Project Types"
            };
            container.Add(checkProjectForTypesButton);
            
            return container;
        }

        private VisualElement GetFileTypeListEntry(FileType type, int index, bool common)
        {
            var container = new VisualElement
            {
                style = { flexDirection = FlexDirection.Row }
            };
            
            var fileTypeStringInput = new TextField
            {
                tooltip = "The filetype for internal searching.",
                value = type.TypeString,
                style =
                {
                    flexBasis = Length.Percent(30),
                    marginRight = 10
                }
            };
            fileTypeStringInput.RegisterValueChangedCallback((evt) =>
            {
                var fileType = Settings.CommonFileTypes[index];
                fileType.TypeString = evt.newValue;
                Settings.CommonFileTypes[index] = fileType;
            });
            container.Add(fileTypeStringInput);

            var headerStringInput = new TextField
            {
                tooltip = "Header text, that will be shown in the main window.",
                value = type.HeaderText,
                style =
                {
                    flexBasis = Length.Percent(30),
                    marginRight = 10
                }
            };
            headerStringInput.RegisterValueChangedCallback((evt) =>
            {
                var fileType = Settings.CommonFileTypes[index];
                fileType.HeaderText = evt.newValue;
                Settings.CommonFileTypes[index] = fileType;
            });
            container.Add(headerStringInput);
            
            var activeToggle = new Toggle
            {
                value = Settings.CommonFileTypes[index].Active,
                label = "Active",
                style = { flexGrow = 1 }
            };
            activeToggle.RegisterValueChangedCallback(evt =>
            {
                var fileType = Settings.CommonFileTypes[index];
                fileType.Active = evt.newValue;
                Settings.CommonFileTypes[index] = fileType;
            });
            container.Add(activeToggle);

            if (!common)
            {
                container.Add(new Button(() =>
                {
                    Settings.Types.RemoveAt(index);
                    RenderInterface();
                })
                {
                    text = "Remove",
                    style = { flexGrow = 1}
                });
            }
            
            return container;
        }
        
        private static Label GetSettingsHeader(string text)
        {
            return new Label
            {
                text = text,
                style =
                {
                    fontSize = DefaultHeaderFontSize,
                    marginBottom = 10,
                }
            };
        }
        
        private static Label GetSettingsSubHeader(string text)
        {
            return new Label
            {
                text = text,
                style =
                {
                    fontSize = DefaultSubHeaderFontSize,
                    marginBottom = 10,
                }
            };
        }

        private static string CreateHeaderText(string text)
        {
            return $"{text.Split(":")[^1]}s";
        }
    }
}