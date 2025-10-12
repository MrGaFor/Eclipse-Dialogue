using Conversa.Editor;
using Conversa.Runtime;
using UnityEngine;
using UnityEngine.UIElements;

public class SimpleDelayNodeView : BaseNodeView<SimpleDelayNode>
{
    protected override string Title => "SimpleDelay";

    // Constructors

    public SimpleDelayNodeView(Conversation conversation) : base(new SimpleDelayNode(), conversation) { }

    public SimpleDelayNodeView(SimpleDelayNode data, Conversation conversation) : base(data, conversation) { }

    protected override void SetBody()    
    {
        
    }

}