using Conversa.Runtime;
using Conversa.Runtime.Events;
using Conversa.Runtime.Nodes.MathOperators;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Conversa.Editor
{
    public class ConcatNodeView : BaseNodeView<ConcatNode>
    {
        protected override string Title => "Concat text";

        public ConcatNodeView(Conversation conversation) : base(new ConcatNode(), conversation) { }

        public ConcatNodeView(ConcatNode data, Conversation conversation) : base(data, conversation) { }

        protected override void SetBody()
        {
            string lang = EC.Localization.LocalizationSystem.ActiveLanguageEditor;

            VisualElement options = new VisualElement();
            options.style.marginRight = 5;
            options.style.marginLeft = 5;
            options.style.marginTop = 5;

            VisualElement optionsTitle = new VisualElement();
            optionsTitle.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
            Label optionsLabel = new Label("Options");
            optionsLabel.style.unityFontStyleAndWeight = new StyleEnum<FontStyle>(FontStyle.Bold);
            optionsTitle.Add(optionsLabel);
            Dictionary<string, VisualElement> optionElementsDict = new Dictionary<string, VisualElement>();
            Button addBtn = new Button(() =>
            {
                var newOption = new SimplePortDefinition<BaseNode>(System.Guid.NewGuid().ToString(), new EC.Localization.LocalizationElement<string>());
                Data.options.Add(newOption);
                AddOption(newOption);
            })
            { text = "+" };
            addBtn.style.marginTop = -3;
            optionsTitle.Add(addBtn);
            options.Add(optionsTitle);

            foreach (var option in Data.options)
                AddOption(option);
            bodyContainer.Add(options);

            void AddOption(SimplePortDefinition<BaseNode> option)
            {
                if (optionElementsDict.ContainsKey(option.Guid))
                    return;

                // Добавляем в ноду, если не добавлен
                if (!Data.options.Contains(option))
                    Data.options.Add(option);

                VisualElement item = new VisualElement();
                item.style.flexDirection = FlexDirection.Row;
                item.style.alignItems = Align.Center;

                TextOption optionElement = new TextOption(option);
                item.Add(optionElement);

                // Регистрация порта
                RegisterPort(optionElement.port, option.Guid);

                TextField itemField = new TextField();
                itemField.style.width = 130;
                if (optionElement.port.connected)
                    itemField.style.display = DisplayStyle.None;
                itemField.RegisterValueChangedCallback(e => option.Value.SetValue(e.newValue, EC.Localization.LocalizationSystem.ActiveLanguageEditor));
                itemField.SetValueWithoutNotify(option.Value.GetValue(lang));
                itemField.isDelayed = true;
                item.Add(itemField);

                EC.Localization.LocalizationSystem.SubscribeChangeEditor((value) =>
                {
                    itemField.SetValueWithoutNotify(option.Value.GetValue(value));
                });

                Button removeBtn = new Button(() =>
                {
                    if (GetPort(option.Guid, out Port portToRemove))
                        GraphView.DeleteElements(portToRemove.connections.ToList());

                    Data.options.Remove(option);
                    options.Remove(item);
                    optionElementsDict.Remove(option.Guid);
                })
                { text = "X" };
                removeBtn.style.marginTop = -3;
                item.Add(removeBtn);

                options.Add(item);
                optionElementsDict[option.Guid] = item;
            }

        }
    }
    public class TextOption : VisualElement
    {
        public readonly SimplePortDefinition<BaseNode> portDefinition;
        public readonly Port port;
        private readonly Label label;

        public TextOption(SimplePortDefinition<BaseNode> portDefinition)
        {
            this.portDefinition = portDefinition;
            port = Port.Create<Edge>(
                Orientation.Horizontal,
                Direction.Input,
                Port.Capacity.Single,
                typeof(EC.Localization.LocalizationElement<string>)
            );
            port.portColor = new Color(0.85f, 0.65f, 0f);
            port.portName = "";
            Add(port);
        }

        public void Update()
        {
            label.text = portDefinition.Value.GetValue(EC.Localization.LocalizationSystem.ActiveLanguageEditor);
        }
    }
}