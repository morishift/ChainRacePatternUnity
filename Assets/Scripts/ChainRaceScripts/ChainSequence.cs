using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChainPattern
{
    /// <summary>
    /// Chain that executes multiple chain sequentially
    /// </summary>
    public class ChainSequence : Chain
    {
        List<Chain> chainList = new List<Chain>();
        Chain currentChain;
        bool enableFlg;        

        public ChainSequence(params Chain[] chains)
        {
            enableFlg = true;
            chainList.AddRange(chains);
        }

        /// <summary>
        /// Adds a chain to the sequence
        /// </summary>        
        public ChainSequence Add(Chain chain)
        {
            if (enableFlg)
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
                bool end = false;
                c.SetCompleteCallback(() => end = true);
                c.SetIsWillSkip(true);
                c.Start();
                if (!end)
                {
                    c.Skip();
                }
            }
            enableFlg = false;
        }

        /// <summary>
        /// Executes the next chain in the sequence
        /// </summary>
        private void NextChain()
        {
            if (chainList.Count <= 0)
            {
                enableFlg = false;
                Complete();
                return;
            }
            currentChain = chainList[0];
            chainList.RemoveAt(0);
            currentChain.SetIsWillSkip(isWillSkip);
            currentChain.SetCompleteCallback(() =>
            {
                currentChain = null;
                NextChain();
            });
            currentChain.Start();
        }
    }
}


