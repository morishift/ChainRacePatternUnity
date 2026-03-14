using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChainPattern
{
    /// <summary>
    /// Chain that executes multiple chains in parallel
    /// </summary>
    public class ChainParallel : Chain
    {
        List<Chain> chainList = new List<Chain>();
        List<Chain> startedChainList = new List<Chain>();

        enum ParallelState
        {
            Ready,
            Starting,
            Started,
            Consuming,
            Finished,
        }
        ParallelState parallelState;

        public ChainParallel(params Chain[] chains)
        {
            parallelState = ParallelState.Ready;
            chainList.AddRange(chains);
        }

        /// <summary>
        /// Adds a chain to the parallel execution
        /// </summary>        
        public ChainParallel Add(Chain chain)
        {
            if (parallelState == ParallelState.Finished)
            {
                // Ignore
            }
            else if (parallelState == ParallelState.Started)
            {
                startedChainList.Add(chain);
                chain.SetCompleteCallback(() => OnChainComplete(chain));
                chain.SetIsFastForward(isFastForward);
                chain.Start();
            }
            else
            {
                // For all states except Started/Finished, queue into pending list
                // During Consuming, Add() may still happen reentrantly from chains being skipped.
                // Queue it into chainList so it will also be consumed in this pass.
                chainList.Add(chain);
            }
            return this;
        }

        /// <summary>
        /// Starts execution
        /// </summary>
        protected override void StartInternal()
        {
            parallelState = ParallelState.Starting;
            while (chainList.Count > 0 && parallelState == ParallelState.Starting)
            {
                Chain c = chainList[0];
                chainList.RemoveAt(0);
                startedChainList.Add(c);
                c.SetCompleteCallback(() => OnChainComplete(c));
                c.SetIsFastForward(isFastForward);
                c.Start();
            }
            if (parallelState == ParallelState.Starting)
            {
                parallelState = ParallelState.Started;
            }
        }

        /// <summary>
        /// Called when skipped
        /// </summary>
        protected override void SkipInternal()
        {
            parallelState = ParallelState.Consuming;
            ConsumeStartedAndPendingChains();
            parallelState = ParallelState.Finished;
        }

        /// <summary>
        /// Callback invoked when a chain completes
        /// </summary>        
        private void OnChainComplete(Chain chain)
        {
            if (startedChainList.Contains(chain))
            {
                startedChainList.Remove(chain);
                if (chainList.Count <= 0 && startedChainList.Count <= 0)
                {
                    parallelState = ParallelState.Finished;
                    Complete();
                }
            }
        }

        /// <summary>
        /// Consumes (completes or skips) all started and pending chains
        /// </summary>
        private void ConsumeStartedAndPendingChains()
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
                bool complete = false;
                c.SetCompleteCallback(() => complete = true);
                c.SetIsFastForward(true);
                c.Start();
                if (!complete)
                {
                    c.Skip();
                }
            }
        }
    }
}
