// SPDX-License-Identifier: MIT
// Copyright (c) 2026 Kenichi Morishita

using System.Collections.Generic;

namespace ChainPattern
{
    /// <summary>
    /// Chain that executes multiple chains sequentially
    /// </summary>
    public class ChainSequence : Chain
    {
        List<Chain> chainList = new List<Chain>();
        Chain currentChain;
        bool isEnabled;

        public ChainSequence(params Chain[] chains)
        {
            isEnabled = true;
            chainList.AddRange(chains);
        }

        /// <summary>
        /// Adds a chain to the sequence
        /// </summary>        
        public ChainSequence Add(Chain chain)
        {
            if (isEnabled)
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
            NextChain();
        }

        /// <summary>
        /// Called when skipped
        /// </summary>
        protected override void SkipInternal()
        {
            currentChain?.Skip();
            currentChain = null;
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
            isEnabled = false;
        }

        /// <summary>
        /// Executes the next chain in the sequence
        /// </summary>
        private void NextChain()
        {
            if (chainList.Count <= 0)
            {
                isEnabled = false;
                Complete();
                return;
            }
            currentChain = chainList[0];
            chainList.RemoveAt(0);
            currentChain.SetIsFastForward(isFastForward);
            currentChain.SetCompleteCallback(() =>
            {
                currentChain = null;
                NextChain();
            });
            currentChain.Start();
        }
    }
}


