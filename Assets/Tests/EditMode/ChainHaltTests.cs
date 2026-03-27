using NUnit.Framework;
using ChainPattern;

namespace ChainPattern.Tests
{
    public class ChainHaltTests
    {
        [Test]
        public void Start_DoesNotComplete()
        {
            bool completed = false;
            var chain = new ChainHalt();
            chain.SetCompleteCallback(() => completed = true);
            chain.Start();
            Assert.IsFalse(completed);
        }

        [Test]
        public void Skip_BeforeStart_DoesNotThrow()
        {
            var chain = new ChainHalt();
            Assert.DoesNotThrow(() => chain.Skip());
        }

        [Test]
        public void Skip_AfterStart_DoesNotInvokeCompleteCallback()
        {
            bool completed = false;
            var chain = new ChainHalt();
            chain.SetCompleteCallback(() => completed = true);
            chain.Start();
            chain.Skip();
            Assert.IsFalse(completed);
        }

        [Test]
        public void Skip_AfterStart_DoesNotThrow()
        {
            var chain = new ChainHalt();
            chain.Start();
            Assert.DoesNotThrow(() => chain.Skip());
        }
    }
}
