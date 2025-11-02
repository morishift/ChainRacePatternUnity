using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChainPattern
{
    /// <summary>
    /// Chains that completes when any of its child chains completes first
    /// </summary>
    public class ChainRace : Chain
    {
        List<Chain> chainList = new List<Chain>();
        List<Chain> startedChainList = new List<Chain>();
        bool isEnabled;
        bool isStarting;
        bool isStarted;
        bool isConsuming;

        public ChainRace(params Chain[] chains)
        {
            isEnabled = true;
            chainList.AddRange(chains);
        }

        /// <summary>
        /// Adds a chain to the race
        /// </summary>
        public ChainRace Add(Chain chain)
        {
            if (!isEnabled)
            {
                // Ignore
            }
            else if (isStarting || isConsuming)
            {
                chainList.Add(chain);
            }
            else if (isStarted)
            {
                startedChainList.Add(chain);
                chain.SetCompleteCallback(() => OnChainComplete(chain));
                chain.SetIsWillSkip(isWillSkip);
                chain.Start();
            }
            else
            {
                chainList.Add(chain);
            }
            return this;
        }

        /// <summary>
        /// Starts execution
        /// </summary>
        protected override void StartInternal()
        {
            isStarting = true;
            while (chainList.Count > 0 && isEnabled)
            {
                Chain c = chainList[0];
                chainList.RemoveAt(0);
                startedChainList.Add(c);
                c.SetCompleteCallback(() => OnChainComplete(c));
                c.SetIsWillSkip(isWillSkip);                
                c.Start();
            }
            if (isEnabled)
            {
                isStarting = false;
                isStarted = true;
            }
        }

        /// <summary>
        /// Called when skipped
        /// </summary>
        protected override void SkipInternal()
        {
            isConsuming = true;
            ConsumeStartedChainListAndChainList();
            isEnabled = false;
            isStarting = false;
            isStarted = false;
            isConsuming = false;
        }

        /// <summary>
        /// Callback invoked when a chain completes
        /// </summary>        
        private void OnChainComplete(Chain chain)
        {
            if (startedChainList.Contains(chain))
            {
                startedChainList.Remove(chain);
                isConsuming = true;
                ConsumeStartedChainListAndChainList();
                isEnabled = false;
                isStarting = false;
                isStarted = false;
                isConsuming = false;
                Complete();
            }
        }

        /// <summary>
        /// Consumes(completes or skips) all started and pending chains
        /// </summary>
        private void ConsumeStartedChainListAndChainList()
        {
            while (startedChainList.Count > 0)
            {
                Chain c = startedChainList[0];
                startedChainList.RemoveAt(0);
                c.Skip();
            }
            while (chainList.Count > 0)
            {
                Chain c = chainList[0];
                chainList.RemoveAt(0);
                bool end = false;
                c.SetCompleteCallback(() => end = true);
                c.SetIsWillSkip(true);
                c.Start();
                if (!end)
                {
                    c.Skip();
                }
            }
        }
    }
}
