using NUnit.Framework;
using ChainPattern;

namespace ChainPattern.Tests
{
    public class ChainDelayTests
    {
        [Test]
        public void Start_WithFastForward_CompletesImmediately()
        {
            bool completed = false;
            var chain = new ChainDelay(10f);
            chain.SetIsFastForward(true);
            chain.SetCompleteCallback(() => completed = true);
            chain.Start();
            Assert.IsTrue(completed);
        }

        [Test]
        public void Start_WithoutFastForward_DoesNotCompleteImmediately()
        {
            bool completed = false;
            var chain = new ChainDelay(10f);
            chain.SetCompleteCallback(() => completed = true);
            chain.Start();
            Assert.IsFalse(completed);
        }

        [Test]
        public void Skip_AfterStart_DoesNotInvokeCompleteCallback()
        {
            bool completed = false;
            var chain = new ChainDelay(10f);
            chain.SetCompleteCallback(() => completed = true);
            chain.Start();
            chain.Skip();
            Assert.IsFalse(completed);
        }
    }
}
