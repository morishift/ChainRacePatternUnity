using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChainPattern
{
    /// <summary>
    /// 一つの関数を実行するChain
    /// </summary>
    public class ChainAction : Chain
    {
        Action calledAction;

        public ChainAction()
        {         
        }

        /// <summary>
        /// 関数を指定したChain
        /// </summary>        
        public ChainAction(Action a)
        {
            calledAction = a;
        }

        /// <summary>
        /// isWillSkipを引数に取る関数を実行するChain
        /// </summary>        
        public ChainAction(Action<bool> action)
        { 
            calledAction = () => action?.Invoke(isWillSkip);
        }

        /// <summary>
        /// 関数指定
        /// </summary>
        public void SetAction(Action a)
        {
            calledAction = a;
        }

        /// <summary>
        /// 開始
        /// </summary>
        protected override void StartInternal()
        {
            calledAction?.Invoke();
            calledAction = null;
            Complete();
        }

        /// <summary>
        /// スキップ時
        /// </summary>
        protected override void SkipInternal()
        {            
            calledAction = null;
        }
    }
}
