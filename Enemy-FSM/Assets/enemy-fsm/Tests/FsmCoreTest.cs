using System;
using System.Collections;
using System.Collections.Generic;
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

    }
}
