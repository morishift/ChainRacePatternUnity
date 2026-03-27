using NUnit.Framework;
using ChainPattern;

namespace ChainPattern.Tests
{
    public class ChainNopTests
    {
        [Test]
        public void Start_CompletesImmediately()
        {
            bool completed = false;
            var chain = new ChainNop();
            chain.SetCompleteCallback(() => completed = true);
            chain.Start();
            Assert.IsTrue(completed);
        }

        [Test]
        public void Skip_BeforeStart_DoesNotThrow()
        {
            var chain = new ChainNop();
            Assert.DoesNotThrow(() => chain.Skip());
        }

        [Test]
        public void Start_CalledTwice_DoesNotThrow()
        {
            var chain = new ChainNop();
            chain.Start();
            Assert.DoesNotThrow(() => chain.Start());
        }

        [Test]
        public void CompleteCallback_CalledExactlyOnce()
        {
            int callCount = 0;
            var chain = new ChainNop();
            chain.SetCompleteCallback(() => callCount++);
            chain.Start();
            Assert.AreEqual(1, callCount);
        }
    }
}
