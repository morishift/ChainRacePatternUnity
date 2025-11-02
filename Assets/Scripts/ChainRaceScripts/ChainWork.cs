
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChainPattern
{
    /// <summary>
    /// Chain with onStart, onSkip, onUpdate events
    /// </summary>
    public class ChainWork : Chain
    {
        /// <summary>
        /// Event invoked when execution starts
        /// </summary>
        public event Action onStart;
        /// <summary>
        /// Event invoked when skipped
        /// </summary>
        public event Action onSkip;
        /// <summary>
        /// Event invoked every frame after ChainWork starts
        /// </summary>
        public event Action onUpdate;

        bool isStarted;

        public ChainWork()
        {            
        }

        /// <summary>
        /// Indicates whether the work will be skipped
        /// </summary>
        public bool isWorkWillSkip => isWillSkip;

        /// <summary>
        /// Ends the work execution
        /// </summary>
        public void End()
        {
            if (isStarted)
            {
                isStarted = false;
                CustomUpdateComponent.RemoveUpdateListener(OnCustomUpdateComponent);
                Complete();
            }
        }

        /// <summary>
        /// Starts execution
        /// </summary>
        protected override void StartInternal()
        {
            isStarted = true;
            CustomUpdateComponent.AddUpdateListener(OnCustomUpdateComponent);
            onStart?.Invoke();
        }

        /// <summary>
        /// Called when skipped
        /// </summary>
        protected override void SkipInternal()
        {
            isStarted = false;
            CustomUpdateComponent.RemoveUpdateListener(OnCustomUpdateComponent);
            onSkip?.Invoke();
        }

        /// <summary>
        /// Called every frame after ChainWork starts
        /// </summary>
        private void OnCustomUpdateComponent()
        {
            onUpdate?.Invoke();
        }
    }
}


