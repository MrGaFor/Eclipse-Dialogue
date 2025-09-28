using Conversa.Runtime.Events;
using Conversa.Runtime.Interfaces;
using UnityEngine.Events;

namespace Conversa.Runtime
{
	public class ConversationEvents
	{
		public UnityEvent<IConversationEvent> OnConversationEvent { get; } = new UnityEvent<IConversationEvent>();
	}
}