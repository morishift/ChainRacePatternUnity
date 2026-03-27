using NUnit.Framework;
using ChainPattern;

namespace ChainPattern.Tests
{
    public class ChainRaceTests
    {
        [Test]
        public void EmptyRace_CompletesImmediately()
        {
            bool completed = false;
            var chain = new ChainRace();
            chain.SetCompleteCallback(() => completed = true);
            chain.Start();
            Assert.IsTrue(completed);
        }

        [Test]
        public void SingleNop_CompletesImmediately()
        {
            bool completed = false;
            var chain = new ChainRace(new ChainNop());
            chain.SetCompleteCallback(() => completed = true);
            chain.Start();
            Assert.IsTrue(completed);
        }

        [Test]
        public void FirstToCompleteWins_OthersAreSkipped()
        {
            bool completed = false;
            bool workSkipCalled = false;
            var work = new ChainWork();
            work.onSkip += () => workSkipCalled = true;

            var chain = new ChainRace(new ChainNop(), work);
            chain.SetCompleteCallback(() => completed = true);
            chain.Start();

            Assert.IsTrue(completed);
            Assert.IsTrue(workSkipCalled);
        }

        [Test]
        public void WaitsForFirstCompletion()
        {
            bool completed = false;
            var work1 = new ChainWork();
            var work2 = new ChainWork();
            var chain = new ChainRace(work1, work2);
            chain.SetCompleteCallback(() => completed = true);
            chain.Start();
            Assert.IsFalse(completed);
            work1.End();
            Assert.IsTrue(completed);
        }

        [Test]
        public void WhenFirstCompletes_RemainingAreSkipped()
        {
            bool work2SkipCalled = false;
            var work1 = new ChainWork();
            var work2 = new ChainWork();
            work2.onSkip += () => work2SkipCalled = true;

            var chain = new ChainRace(work1, work2);
            chain.Start();
            work1.End();

            Assert.IsTrue(work2SkipCalled);
        }

        [Test]
        public void Add_ReturnsChainRaceForFluent()
        {
            var chain = new ChainRace();
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

            var chain = new ChainRace(work1, work2);
            chain.Start();
            chain.Skip();

            Assert.IsTrue(work1SkipCalled);
            Assert.IsTrue(work2SkipCalled);
        }

        [Test]
        public void Skip_AfterStart_DoesNotInvokeCompleteCallback()
        {
            bool completed = false;
            var chain = new ChainRace(new ChainWork(), new ChainWork());
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
            var chain = new ChainRace(child);
            chain.SetIsFastForward(true);
            chain.Start();
            Assert.AreEqual(true, childFastForward);
        }
    }
}
