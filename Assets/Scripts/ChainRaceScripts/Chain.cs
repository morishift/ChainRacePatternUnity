
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

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

        TaskCompletionSource<bool> currentTcs;
        State state;        
        Action onComplete;

        public Chain()
        {
            state = State.Ready;
        }

        /// <summary>
        /// Starts execution of the Chain
        /// </summary>
        public Task Start()
        {
            if (state != State.Ready)
            {
                // If already started, return the existing task to wait for it
                return currentTcs?.Task ?? Task.CompletedTask;
            }
            currentTcs = new TaskCompletionSource<bool>();
            state = State.Started;
            StartInternal();
            return currentTcs.Task;
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
            currentTcs?.TrySetResult(true);
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
        public void SetIsWillSkip(bool willSkip)
        {
            isWillSkip = willSkip;
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
            currentTcs?.TrySetResult(true);
        }

        /// <summary>
        /// Indicates whether this Chain will be skipped immediately after Start
        /// </summary>
        protected bool isWillSkip
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

