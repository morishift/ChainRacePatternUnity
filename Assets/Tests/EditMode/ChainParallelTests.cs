using NUnit.Framework;
using ChainPattern;

namespace ChainPattern.Tests
{
    public class ChainParallelTests
    {
        [Test]
        public void EmptyParallel_CompletesImmediately()
        {
            bool completed = false;
            var chain = new ChainParallel();
            chain.SetCompleteCallback(() => completed = true);
            chain.Start();
            Assert.IsTrue(completed);
        }

        [Test]
        public void AllNops_CompletesImmediately()
        {
            bool completed = false;
            var chain = new ChainParallel(new ChainNop(), new ChainNop(), new ChainNop());
            chain.SetCompleteCallback(() => completed = true);
            chain.Start();
            Assert.IsTrue(completed);
        }

        [Test]
        public void WaitsForAllChains_NotCompletedUntilLastDone()
        {
            bool completed = false;
            var work = new ChainWork();
            var chain = new ChainParallel(new ChainNop(), work);
            chain.SetCompleteCallback(() => completed = true);
            chain.Start();
            Assert.IsFalse(completed);
            work.End();
            Assert.IsTrue(completed);
        }

        [Test]
        public void CompletesOnlyWhenAllDone()
        {
            bool completed = false;
            var work1 = new ChainWork();
            var work2 = new ChainWork();
            var chain = new ChainParallel(work1, work2);
            chain.SetCompleteCallback(() => completed = true);
            chain.Start();
            work1.End();
            Assert.IsFalse(completed);
            work2.End();
            Assert.IsTrue(completed);
        }

        [Test]
        public void Add_ReturnsChainParallelForFluent()
        {
            var chain = new ChainParallel();
            var result = chain.Add(new ChainNop());
            Assert.AreSame(chain, result);
        }

        [Test]
        public void Skip_AfterStart_SkipsAllRunningChains()
        {
            bool work1SkipCalled = false;
            bool work2SkipCalled = false;
            var work1 = new ChainWork();
            var work2 = new ChainWork();
            work1.onSkip += () => work1SkipCalled = true;
            work2.onSkip += () => work2SkipCalled = true;

            var chain = new ChainParallel(work1, work2);
            chain.Start();
            chain.Skip();

            Assert.IsTrue(work1SkipCalled);
            Assert.IsTrue(work2SkipCalled);
        }

        [Test]
        public void Skip_AfterStart_DoesNotInvokeCompleteCallback()
        {
            bool completed = false;
            var chain = new ChainParallel(new ChainWork(), new ChainWork());
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
            var chain = new ChainParallel(child);
            chain.SetIsFastForward(true);
            chain.Start();
            Assert.AreEqual(true, childFastForward);
        }
    }
}
