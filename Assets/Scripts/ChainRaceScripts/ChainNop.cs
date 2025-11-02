using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChainPattern
{
    /// <summary>
    /// Chain class that completes immediately after starting (no operation)
    /// </summary>
    public class ChainNop : Chain
    {
        /// <summary>
        /// Starts execution
        /// </summary>
        protected override void StartInternal()
        {
            Complete();
        }

        /// <summary>
        /// Called when skipped
        /// </summary>
        protected override void SkipInternal()
        {
        }
    }
}


