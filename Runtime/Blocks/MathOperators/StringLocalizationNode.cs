using Conversa.Runtime.Events;
using Conversa.Runtime.Interfaces;
using EC.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Conversa.Runtime.Nodes.MathOperators
{
    [MovedFrom(true, null, "Assembly-CSharp")]
    [Serializable]
    [Port("Out", "out", typeof(LocalizationElement<string>), Flow.Out, Capacity.Many)]
    public class StringLocalizationNode : BaseNode, IValueNode
    {
        public const string DefaultInVar = "";
        [SerializeField] private string inVar = DefaultInVar;

        [Slot("In", "in", Flow.In, Capacity.One)]
        public string In
        {
            get => inVar;
            set => inVar = value;
        }

        public T GetValue<T>(string portGuid, Conversation conversation)
        {
            if (portGuid != "out") return default;
            var output = new LocalizationElement<string>();
            string input = conversation.GetConnectedValueTo<string>(this, "in");
            foreach (var lang in LanguageConfig.Languages)
            {
                output.SetValue(input, lang);
            }
            return Converter.ConvertValue<LocalizationElement<string>, T>(output);
        }
    }
}