using System;
using System.Linq;
using Conversa.Runtime;
using Conversa.Runtime.Interfaces;
using UnityEngine;

[Serializable]
[Port("Previous", "previous", typeof(BaseNode), Flow.In, Capacity.Many)]
[Port("Next", "next", typeof(BaseNode), Flow.Out, Capacity.One)]
public class SimpleDelayNode : BaseNode, IEventNode
{
    public float delay;

    [Slot("Delay", "delay", Flow.In, Capacity.One)]
    public float Delay 
    { 
        get => delay; 
        set => delay = value; 
    }

    public SimpleDelayNode() { }

    public void Process(Conversation conversation, ConversationEvents conversationEvents)
    {
        void Advance()
        {
            var nextNode = conversation.GetOppositeNodes(GetNodePort("next")).FirstOrDefault();
            conversation.Process(nextNode, conversationEvents);
        }

        var e = new SimpleDelayEvent(delay, Advance);
        conversationEvents.OnConversationEvent.Invoke(e);
    }
}
    public class SimpleDelayEvent : IConversationEvent
    {
    public float Delay { get; }
    public Action Advance { get; }

    public SimpleDelayEvent(float delay, Action advance)
    {
        Delay = delay;
        Advance = advance;
    }
}
