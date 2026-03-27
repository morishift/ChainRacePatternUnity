using NUnit.Framework;
using ChainPattern;

namespace ChainPattern.Tests
{
    public class ChainWorkTests
    {
        [Test]
        public void Start_InvokesOnStart()
        {
            bool startCalled = false;
            var chain = new ChainWork();
            chain.onStart += () => startCalled = true;
            chain.Start();
            Assert.IsTrue(startCalled);
        }

        [Test]
        public void Start_DoesNotCompleteImmediately()
        {
            bool completed = false;
            var chain = new ChainWork();
            chain.SetCompleteCallback(() => completed = true);
            chain.Start();
            Assert.IsFalse(completed);
        }

        [Test]
        public void End_CompletesChain()
        {
            bool completed = false;
            var chain = new ChainWork();
            chain.SetCompleteCallback(() => completed = true);
            chain.Start();
            chain.End();
            Assert.IsTrue(completed);
        }

        [Test]
        public void End_BeforeStart_DoesNotComplete()
        {
            bool completed = false;
            var chain = new ChainWork();
            chain.SetCompleteCallback(() => completed = true);
            chain.End();
            Assert.IsFalse(completed);
        }

        [Test]
        public void Skip_AfterStart_InvokesOnSkip()
        {
            bool skipCalled = false;
            var chain = new ChainWork();
            chain.onSkip += () => skipCalled = true;
            chain.Start();
            chain.Skip();
            Assert.IsTrue(skipCalled);
        }

        [Test]
        public void Skip_AfterStart_DoesNotInvokeCompleteCallback()
        {
            bool completed = false;
            var chain = new ChainWork();
            chain.SetCompleteCallback(() => completed = true);
            chain.Start();
            chain.Skip();
            Assert.IsFalse(completed);
        }

        [Test]
        public void End_AfterSkip_DoesNotComplete()
        {
            bool completed = false;
            var chain = new ChainWork();
            chain.SetCompleteCallback(() => completed = true);
            chain.Start();
            chain.Skip();
            chain.End();
            Assert.IsFalse(completed);
        }

        [Test]
        public void IsWorkFastForward_ReflectsSetValue()
        {
            var chain = new ChainWork();
            chain.SetIsFastForward(true);
            Assert.IsTrue(chain.isWorkFastForward);
        }
    }
}
