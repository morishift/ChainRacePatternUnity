using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChainPattern
{
    /// <summary>
    /// Chain class that halts execution after starting
    /// </summary>
    public class ChainHalt: Chain
    {
        /// <summary>
        /// Starts execution
        /// </summary>
        protected override void StartInternal()
        {
            // Does not call Complete()
        }

        /// <summary>
        /// Called when skipped
        /// </summary>
        protected override void SkipInternal()
        {
        }
    }
}

