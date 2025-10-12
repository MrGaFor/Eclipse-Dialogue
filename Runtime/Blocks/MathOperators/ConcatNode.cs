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
    public class ConcatNode : BaseNode, IValueNode
    {
        public List<SimplePortDefinition<BaseNode>> options = new List<SimplePortDefinition<BaseNode>>
        {
            new SimplePortDefinition<BaseNode>("yes", new EC.Localization.LocalizationElement<string>()),
            new SimplePortDefinition<BaseNode>("no", new EC.Localization.LocalizationElement<string>())
        };

        public T GetValue<T>(string portGuid, Conversation conversation)
        {
            if (portGuid != "out") return default;
            var output = new LocalizationElement<string>();

            foreach (var lang in LanguageConfig.Languages)
            {
                string value = "";
                foreach (var op in options)
                {
                    var connectedValue = conversation.GetConnectedValueTo<LocalizationElement<string>>(this, op.Guid);
                    if (connectedValue != null)
                        value += connectedValue.GetValue(lang);
                    else
                        value += op.Value.GetValue(lang);
                }
                output.SetValue(value, lang);
            }

            return Converter.ConvertValue<LocalizationElement<string>, T>(output);
        }


        public override bool ContainsPort(string portId, Flow flow)
        {
            if (base.ContainsPort(portId, flow)) return true;
            return flow == Flow.In && options.Any(option => option.Guid == portId);
        }
    }
}