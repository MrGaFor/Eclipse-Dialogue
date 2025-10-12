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
    public class StringLocalizationNodeView : BaseNodeView<StringLocalizationNode>
    {
        protected override string Title => "String > Local...";

        public StringLocalizationNodeView(Conversation conversation) : base(new StringLocalizationNode(), conversation) { }

        public StringLocalizationNodeView(StringLocalizationNode data, Conversation conversation) : base(data, conversation) { }

        protected override void SetBody()
        {
            
        }
    }
}