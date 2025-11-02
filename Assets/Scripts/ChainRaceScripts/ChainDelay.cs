
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace ChainPattern
{
    /// <summary>
    /// Chain that waits for a specified duration
    /// </summary>
    public class ChainDelay : Chain
    {
        float delaySeconds;
        float endTime;        

        /// <summary>
        /// Creates a delay chain with duration in seconds
        /// </summary>
        public ChainDelay(float seconds)
        {
            delaySeconds = seconds;
        }

        /// <summary>
        /// Starts execution
        /// </summary>
        protected override void StartInternal()
        {
            if (isWillSkip)
            {
                // Do nothing if it will be skipped immediately
                return;
            }
            endTime = Time.time + delaySeconds;
            CustomUpdateComponent.AddUpdateListener(OnCustomUpdate);
        }

        /// <summary>
        /// Called when skipped
        /// </summary>
        protected override void SkipInternal()
        {
            CustomUpdateComponent.RemoveUpdateListener(OnCustomUpdate);
        }

        /// <summary>
        /// Called every frame by CustomUpdateComponent
        /// </summary>
        private void OnCustomUpdate()
        {            
            if (endTime <= Time.time)
            {                
                CustomUpdateComponent.RemoveUpdateListener(OnCustomUpdate);
                Complete();
            }
        }
    }
}
