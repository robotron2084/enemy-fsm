using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace com.enemyhideout.fsm.tests
{
    public class FsmCoreTest
    {

        public enum TestState
        {
            Idle,
            Running,
            Dead
        }

        
        [Test]
        public void FsmCoreTestGenerateStates()
        {
            Dictionary<TestState, FsmState<TestState>> states = FsmCore<TestState>.GenerateStates();
            foreach (var testState in Enum.GetValues(typeof(TestState)))
            {
                FsmState<TestState> state = states[(TestState) testState];
                Assert.AreEqual(state.State, testState);
            }
        }
        
        [Test]
        public void FsmCoreTestChangeState()
        {
            Dictionary<TestState, FsmState<TestState>> allStates = FsmCore<TestState>.GenerateStates();
            FsmState<TestState> currentState = allStates[TestState.Idle];
            FsmState<TestState> newState = FsmCore<TestState>.ChangeState(TestState.Dead, currentState, allStates);
            Assert.AreEqual(newState, allStates[TestState.Dead]);
            
            // it should still be dead!
            newState = FsmCore<TestState>.ChangeState(TestState.Dead, currentState, allStates);
            Assert.AreEqual(newState, allStates[TestState.Dead]);
        }

        [Test]
        public void FsmCoreSetDelegateFuncTest()
        {

            Func<CancellationToken, Task> TestDelegate = null;
            bool noErrorsFound = FsmCore<FsmCoreTest>.SetDelegateFor("PublicFunction", this, ref TestDelegate);
            Assert.IsTrue(noErrorsFound);
            Assert.IsNotNull(TestDelegate);

            TestDelegate = null;
            noErrorsFound = FsmCore<FsmCoreTest>.SetDelegateFor("PrivateFunction", this, ref TestDelegate);
            Assert.IsTrue(noErrorsFound);
            Assert.IsNotNull(TestDelegate);
            
            //todo: Test virtual/protected functions.
            
            // invalid parameters
            TestDelegate = null;
            noErrorsFound = FsmCore<FsmCoreTest>.SetDelegateFor("TaskWithoutToken", this, ref TestDelegate);
            Assert.IsFalse(noErrorsFound);
            Assert.IsNull(TestDelegate);
            
            TestDelegate = null;
            noErrorsFound = FsmCore<FsmCoreTest>.SetDelegateFor("InvalidReturnTypeFunction", this, ref TestDelegate);
            Assert.IsFalse(noErrorsFound);
            Assert.IsNull(TestDelegate);
            
            // It is ok to not set the delegate.
            TestDelegate = null;
            noErrorsFound = FsmCore<FsmCoreTest>.SetDelegateFor("DoesntExist", this, ref TestDelegate);
            Assert.IsTrue(noErrorsFound);
            Assert.IsNull(TestDelegate);

        }

        public void InvalidReturnTypeFunction(CancellationToken token)
        {
            
        }

        public Task PublicFunction(CancellationToken token)
        {
            return Task.CompletedTask;
        }

        private Task PrivateFunction(CancellationToken token)
        {
            return Task.CompletedTask;
        }

        public Task TaskWithoutToken()
        {
            return Task.CompletedTask;
        }
        
        
        [Test]
        public void FsmCoreSetDelegateActionTest()
        {
            Action testAction = null;
            bool noErrorsFound = FsmCore<FsmCoreTest>.SetDelegateFor("PublicAction", this, ref testAction);
            Assert.IsTrue(noErrorsFound);
            Assert.IsNotNull(testAction);

            testAction = null;
            noErrorsFound = FsmCore<FsmCoreTest>.SetDelegateFor("PrivateAction", this, ref testAction);
            Assert.IsTrue(noErrorsFound);
            Assert.IsNotNull(testAction);
            
            //todo: Test virtual/protected functions.
            
            // invalid parameters
            testAction = null;
            noErrorsFound = FsmCore<FsmCoreTest>.SetDelegateFor("ActionWithArgs", this, ref testAction);
            Assert.IsFalse(noErrorsFound);
            Assert.IsNull(testAction);
            
            testAction = null;
            noErrorsFound = FsmCore<FsmCoreTest>.SetDelegateFor("InvalidReturnTypeAction", this, ref testAction);
            Assert.IsFalse(noErrorsFound);
            Assert.IsNull(testAction);
            
            // It is ok to not set the delegate.
            testAction = null;
            noErrorsFound = FsmCore<FsmCoreTest>.SetDelegateFor("DoesntExist", this, ref testAction);
            Assert.IsTrue(noErrorsFound);
            Assert.IsNull(testAction);

        }
        
        public void PublicAction()
        {
        }

        public void PrivateAction()
        {
        }

        public bool InvalidReturnTypeAction()
        {
            return true;
        }

        public void ActionWithArgs(string msg)
        {
            
        }
        
        [Test]
        public void FsmCoreSetDelegateExceptionTest()
        {
            Action testAction = null;
            Func<CancellationToken, Task> testFunc = null;
            FsmCore<FsmCoreTest>.SetDelegateFor("PublicAction", this, ref testFunc, ref testAction);
            Assert.IsNull(testFunc);
            Assert.IsNotNull(testAction);

            
        }
        
        


    }
}
