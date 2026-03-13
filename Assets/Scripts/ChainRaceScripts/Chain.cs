
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace ChainPattern
{
    /// <summary>
    /// Base class for Chain classes
    /// </summary>
    public abstract class Chain
    {
        enum State
        {
            Ready,
            Started,
            Skipped,
            Completed,
        }

        UniTaskCompletionSource<bool> currentUtcs;
        State state;        
        Action onComplete;

        public Chain()
        {
            state = State.Ready;
        }

        /// <summary>
        /// Starts execution of the Chain
        /// </summary>
        public UniTask Start()
        {
            if (state != State.Ready)
            {
                // If already started, return the existing task to wait for it
                return currentUtcs?.Task ?? UniTask.CompletedTask;
            }
            currentUtcs = new UniTaskCompletionSource<bool>();
            state = State.Started;
            StartInternal();
            return currentUtcs.Task;
        }

        /// <summary>
        /// Skips the Chain and completes it immediately
        /// </summary>
        public void Skip()
        {
            if (state != State.Started)
            {
                return;
            }
            state = State.Skipped;
            onComplete = null; // Don't call completion callback when skipped
            SkipInternal();            
            currentUtcs?.TrySetResult(true);
        }

        /// <summary>
        /// Sets the completion callback
        /// </summary>
        public void SetCompleteCallback(Action callback)
        {
            onComplete = callback;
        }

        /// <summary>
        /// Sets whether this Chain will be skipped immediately after starting
        /// </summary>
        public void SetIsFastForward(bool fastForward)
        {
            isFastForward = fastForward;
        }

        /// <summary>
        /// Marks the Chain as completed. Called from derived classes
        /// </summary>
        protected void Complete()
        {
            if (state != State.Started)
            {
                return;
            }
            state = State.Completed;
            onComplete?.Invoke();
            onComplete = null;
            currentUtcs?.TrySetResult(true);
        }

        /// <summary>
        /// Indicates whether this Chain will be skipped immediately after Start
        /// </summary>
        protected bool isFastForward
        {
            get;
            private set;
        }

        /// <summary>
        /// Internal start implementation. Must be implemented by derived classes
        /// </summary>
        protected abstract void StartInternal();

        /// <summary>
        /// Internal skip implementation. Must be implemented by derived classes
        /// </summary>
        protected abstract void SkipInternal();
    }
}

