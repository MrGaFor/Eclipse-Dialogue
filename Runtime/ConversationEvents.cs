using Conversa.Runtime.Events;
using Conversa.Runtime.Interfaces;
using UnityEngine.Events;

namespace Conversa.Runtime
{
	public class ConversationEvents
	{
		#region deprecated

		public UnityEvent<SimpleMessageEvent> OnMessage { get; } = new UnityEvent<SimpleMessageEvent>();
		public UnityEvent<SimpleChoiceEvent> OnChoice { get; } = new UnityEvent<SimpleChoiceEvent>();
		public UnityEvent<SimpleEventEvent> OnUserEvent { get; } = new UnityEvent<SimpleEventEvent>();
		public UnityEvent OnEnd { get; } = new UnityEvent();

		#endregion

		public UnityEvent<IConversationEvent> OnConversationEvent { get; } = new UnityEvent<IConversationEvent>();
	}
}