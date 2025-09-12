using System.Linq;
using Conversa.Runtime;
using Conversa.Runtime.Nodes;
using Conversa.Runtime.Properties;
using unity_conversa.Editor.Nodes;

namespace Conversa.Editor
{
	public class BasicNodesMenuModifier
	{
		[NodeMenuModifier]
		private static void ModifyMenu(NodeMenuTree tree, Conversation conversation)
		{
			tree.AddGroup("Basics");
			tree.AddMenuEntry<SimpleMessageNodeView>("Message", 2);
			tree.AddMenuEntry<SimpleChoiceNodeView>("Choice", 2);
			tree.AddMenuEntry<SimpleEventNodeView>("Event", 2);
            tree.AddMenuEntry<ConditionalNodeView>("Branch", 2);
			tree.AddMenuEntry<BookmarkNodeView>("Bookmark", 2);

			tree.AddGroup("Jumps", 2);
			conversation.AllNodes.OfType<BookmarkNode>().ToList().ForEach(bookmark =>
			{
				BookmarkJumpNodeView Callback()
				{
					var node = new BookmarkJumpNode { BookmarkName = bookmark.Name, BookmarkGuid = bookmark.Guid };
					var view = new BookmarkJumpNodeView(node, conversation);
					return view;
				}

				tree.AddMenuEntry("Jump: " + bookmark.Name, Callback, 3);
			});

			tree.AddGroup("Literals");
			tree.AddMenuEntry<LiteralStringNodeView>("String", 2);
			tree.AddMenuEntry<AbsoluteFloatNodeView>("Float", 2);
			tree.AddMenuEntry<AbsoluteIntNodeView>("Integer", 2);
            tree.AddMenuEntry<AbsoluteBoolNodeView>("Boolean", 2);
			tree.AddMenuEntry<SimpleMassiveNodeView>("Massive", 2);
        }
	}
}