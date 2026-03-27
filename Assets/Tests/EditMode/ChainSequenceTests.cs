using System.Collections.Generic;
using NUnit.Framework;
using ChainPattern;

namespace ChainPattern.Tests
{
    public class ChainSequenceTests
    {
        [Test]
        public void EmptySequence_CompletesImmediately()
        {
            bool completed = false;
            var chain = new ChainSequence();
            chain.SetCompleteCallback(() => completed = true);
            chain.Start();
            Assert.IsTrue(completed);
        }

        [Test]
        public void SingleNop_CompletesImmediately()
        {
            bool completed = false;
            var chain = new ChainSequence(new ChainNop());
            chain.SetCompleteCallback(() => completed = true);
            chain.Start();
            Assert.IsTrue(completed);
        }

        [Test]
        public void MultipleNops_CompletesImmediately()
        {
            bool completed = false;
            var chain = new ChainSequence(new ChainNop(), new ChainNop(), new ChainNop());
            chain.SetCompleteCallback(() => completed = true);
            chain.Start();
            Assert.IsTrue(completed);
        }

        [Test]
        public void ChainsExecuteInOrder()
        {
            var order = new List<int>();
            var chain = new ChainSequence(
                new ChainAction(() => order.Add(1)),
                new ChainAction(() => order.Add(2)),
                new ChainAction(() => order.Add(3))
            );
            chain.Start();
            Assert.AreEqual(new[] { 1, 2, 3 }, order.ToArray());
        }

        [Test]
        public void Add_BeforeStart_AddsToSequence()
        {
            var order = new List<int>();
            var chain = new ChainSequence();
            chain.Add(new ChainAction(() => order.Add(1)));
            chain.Add(new ChainAction(() => order.Add(2)));
            chain.Start();
            Assert.AreEqual(new[] { 1, 2 }, order.ToArray());
        }

        [Test]
        public void Add_ReturnsChainSequenceForFluent()
        {
            var chain = new ChainSequence();
            var result = chain.Add(new ChainNop());
            Assert.AreSame(chain, result);
        }

        [Test]
        public void WaitsForChainToComplete_BeforeStartingNext()
        {
            bool secondStarted = false;
            var work = new ChainWork();
            var chain = new ChainSequence(work, new ChainAction(() => secondStarted = true));
            chain.Start();
            Assert.IsFalse(secondStarted);
            work.End();
            Assert.IsTrue(secondStarted);
        }

        [Test]
        public void Skip_AfterStart_DoesNotInvokeCompleteCallback()
        {
            bool completed = false;
            var chain = new ChainSequence(new ChainHalt());
            chain.SetCompleteCallback(() => completed = true);
            chain.Start();
            chain.Skip();
            Assert.IsFalse(completed);
        }

        [Test]
        public void PropagatesFastForwardToChildren()
        {
            bool? childFastForward = null;
            var child = new ChainAction((ff) => childFastForward = ff);
            var chain = new ChainSequence(child);
            chain.SetIsFastForward(true);
            chain.Start();
            Assert.AreEqual(true, childFastForward);
        }
    }
}
