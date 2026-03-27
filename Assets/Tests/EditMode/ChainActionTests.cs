using NUnit.Framework;
using ChainPattern;

namespace ChainPattern.Tests
{
    public class ChainActionTests
    {
        [Test]
        public void Start_InvokesAction()
        {
            bool called = false;
            var chain = new ChainAction(() => called = true);
            chain.Start();
            Assert.IsTrue(called);
        }

        [Test]
        public void Start_CompletesAfterAction()
        {
            bool completed = false;
            var chain = new ChainAction(() => { });
            chain.SetCompleteCallback(() => completed = true);
            chain.Start();
            Assert.IsTrue(completed);
        }

        [Test]
        public void Start_WithNullAction_StillCompletes()
        {
            bool completed = false;
            var chain = new ChainAction();
            chain.SetCompleteCallback(() => completed = true);
            chain.Start();
            Assert.IsTrue(completed);
        }

        [Test]
        public void Start_WithFastForwardAction_ReceivesTrueWhenFastForward()
        {
            bool? received = null;
            var chain = new ChainAction((ff) => received = ff);
            chain.SetIsFastForward(true);
            chain.Start();
            Assert.AreEqual(true, received);
        }

        [Test]
        public void Start_WithFastForwardAction_ReceivesFalseWhenNotFastForward()
        {
            bool? received = null;
            var chain = new ChainAction((ff) => received = ff);
            chain.Start();
            Assert.AreEqual(false, received);
        }

        [Test]
        public void SetAction_ReplacesConstructorAction()
        {
            bool firstCalled = false;
            bool secondCalled = false;
            var chain = new ChainAction(() => firstCalled = true);
            chain.SetAction(() => secondCalled = true);
            chain.Start();
            Assert.IsFalse(firstCalled);
            Assert.IsTrue(secondCalled);
        }

        [Test]
        public void Skip_BeforeStart_DoesNotThrow()
        {
            var chain = new ChainAction(() => { });
            Assert.DoesNotThrow(() => chain.Skip());
        }
    }
}
