using System;
using System.Collections.Generic;
using Conversa.Runtime;
using Conversa.Runtime.Events;
using Conversa.Runtime.Interfaces;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EC.Dialogue
{
    public class DialogueContainer : MonoBehaviour
    {
        public Conversation Dialogue;
        [HideInEditorMode, ReadOnly] public ConversationRunner Runner;

        protected readonly Queue<SimpleMessageEvent> _messages = new();
        protected readonly Queue<SimpleChoiceEvent> _choices = new();
        protected readonly Queue<SimpleEventEvent> _users = new();

        public bool IsActive { get; private set; }

        public virtual void Initialize(Conversation dialogue)
        {
            Dialogue = dialogue;
            Initialize();
        }

        public virtual void Initialize()
        {
            if (Dialogue == null) throw new InvalidOperationException("Dialogue is null");
            Runner = new ConversationRunner(Dialogue);
            Runner.OnConversationEvent.AddListener(HandleEvent);
#if UNITY_EDITOR
            if (_debugging == IsOnOff.On)
                Debug.Log($"OnInitialized");
#endif
        }

        public virtual async UniTask StartDialogue()
        {
            if (Runner == null) Initialize();
            IsActive = true;
#if UNITY_EDITOR
            if (_debugging == IsOnOff.On)
                Debug.Log($"OnStartDialogue");
#endif
            await OnStarted();
            Runner.Begin();
        }

        public virtual async UniTask StopDialogue()
        {
            if (Runner != null) Runner.OnConversationEvent.RemoveListener(HandleEvent);
            Runner = null;
            IsActive = false;
#if UNITY_EDITOR
            if (_debugging == IsOnOff.On)
                Debug.Log($"OnStopDialogue");
#endif
            _messages.Clear();
            _choices.Clear();
            _users.Clear();
            await OnStopped();
        }

        protected virtual void HandleEvent(IConversationEvent e)
        {
            switch (e)
            {
                case SimpleMessageEvent m:
                    _messages.Enqueue(m);
#if UNITY_EDITOR
                    if (_debugging == IsOnOff.On)
                        Debug.Log($"OnAddMessage: {m.Actor.GetValue()} [{m.Emotion}]: {m.Message.GetValue()}");
#endif
                    OnMessageQueued(m).Forget();
                    break;

                case SimpleChoiceEvent c:
                    _choices.Enqueue(c);
#if UNITY_EDITOR
                    if (_debugging == IsOnOff.On)
                        Debug.Log($"OnAddChoose: {c.Actor.GetValue()} [{c.Emotion}]: {c.Message.GetValue()}. ID`s count: {c.Options.Count}");
#endif
                    OnChoiceQueued(c).Forget();
                    break;

                case SimpleEventEvent u:
                    _users.Enqueue(u);
#if UNITY_EDITOR
                    if (_debugging == IsOnOff.On)
                        Debug.Log($"OnAddUserEvent: {u.Tag}");
#endif
                    OnUserEventQueued(u).Forget();
                    break;

                /*case Conversa.Runtime.Nodes.BookmarkNode j:
#if UNITY_EDITOR
                    if (_debugging == IsOnOff.On)
                        Debug.Log($"OnJumpBookmark: {j.Name}");
#endif
                    Runner.Begin(j.Name);
                    break;*/

                case EndEvent:
#if UNITY_EDITOR
                    if (_debugging == IsOnOff.On)
                        Debug.Log($"OnEndDialogue");
#endif
                    StopDialogue().ContinueWith(() => OnEnd()).Forget();
                    break;
            }
        }

        public void ProcessNextMessage() => OnProcessNextMessage().Forget();
        public async UniTask OnProcessNextMessage()
        {
            if (_messages.Count == 0) return;
            var m = _messages.Dequeue();
#if UNITY_EDITOR
            if (_debugging == IsOnOff.On)
                Debug.Log($"OnProcessMessage: {m.Actor.GetValue()} [{m.Emotion}]: {m.Message.GetValue()}");
#endif
            await OnMessageUse(m);
            m.Advance();
        }

        public void ProcessAllMessages() => OnProcessAllMessages().Forget();
        public async UniTask OnProcessAllMessages()
        {
            while (_messages.Count > 0) 
            { 
                await OnProcessNextMessage(); 
                await UniTask.WaitForEndOfFrame(); 
            }
        }

        public void Choose(int optionIndex) => OnChoose(optionIndex).Forget();
        public async UniTask OnChoose(int optionIndex)
        {
            if (_choices.Count == 0) return;
            var c = _choices.Dequeue();
#if UNITY_EDITOR
            if (_debugging == IsOnOff.On)
                Debug.Log($"OnProcessChoose: {c.Actor.GetValue()} [{c.Emotion}]: {c.Message.GetValue()}. Use ID: {optionIndex}");
#endif
            await OnChoiceUse(c, optionIndex);
            c.Options[optionIndex].Advance();
        }

        public void ProcessNextUserEvent() => OnProcessNextUserEvent().Forget();
        public async UniTask OnProcessNextUserEvent()
        {
            if (_users.Count == 0) return;
            var u = _users.Dequeue();
#if UNITY_EDITOR
            if (_debugging == IsOnOff.On)
                Debug.Log($"OnProcessUserEvent: {u.Tag}");
#endif
            await OnUserEventUse(u);
            u.Advance();
        }

#pragma warning disable CS1998
        // Process
        protected virtual async UniTask OnStarted() { }
        protected virtual async UniTask OnStopped() { }
        protected virtual async UniTask OnEnd() { }

        // Add Block
        protected virtual async UniTask OnMessageQueued(SimpleMessageEvent e) { }
        protected virtual async UniTask OnChoiceQueued(SimpleChoiceEvent e) { }
        protected virtual async UniTask OnUserEventQueued(SimpleEventEvent e) { }

        // Use Block
        protected virtual async UniTask OnMessageUse(SimpleMessageEvent e) { }
        protected virtual async UniTask OnChoiceUse(SimpleChoiceEvent e, int optionIndex) { }
        protected virtual async UniTask OnUserEventUse(SimpleEventEvent e) { }
#pragma warning restore CS1998


#if UNITY_EDITOR
        private enum IsOnOff { On, Off }
        [BoxGroup("Debugging", false), SerializeField] private IsOnOff _debugging = IsOnOff.Off;

        [BoxGroup("Debugging"), SerializeField, Button("Initialize"), ShowIf("_debugging", IsOnOff.On), DisableInEditorMode] private void DebugInitialize() => Initialize();
        [BoxGroup("Debugging"), SerializeField, Button("Start Dialogue"), ShowIf("_debugging", IsOnOff.On), DisableInEditorMode] private void DebugStartDialogue() => StartDialogue().Forget();
        [BoxGroup("Debugging"), SerializeField, Button("Stop Dialogue"), ShowIf("_debugging", IsOnOff.On), DisableInEditorMode] private void DebugStopDialogue() => StopDialogue().Forget();
        [BoxGroup("Debugging"), SerializeField, Button("Use Message"), ShowIf("_debugging", IsOnOff.On), DisableInEditorMode] private void DebugUseMessage() => ProcessNextMessage();
        [BoxGroup("Debugging"), SerializeField, Button("Use Choice"), HorizontalGroup("Debugging/choice", Order = 1), DisableInEditorMode, ShowIf("_debugging", IsOnOff.On)] private void DebugUseChoice() => ProcessNextMessage();
        [BoxGroup("Debugging"), SerializeField, HorizontalGroup("Debugging/choice", 50), HideLabel, ShowIf("_debugging", IsOnOff.On)] private int _debugChooseId = 0;
#endif
    }
}
