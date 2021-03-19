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
        public void FsmCoreSetDelegateFuncTest([ValueSource("DelegateFuncTestCases")] DelegateFuncTestCase testCase)
        {

            Func<CancellationToken, Task> TestDelegate = null;
            FsmCore<FsmCoreTest>.SetDelegateFor(testCase.FunctionName, this, ref TestDelegate);
            Assert.AreEqual(testCase.ShouldHaveDelegate, TestDelegate != null);
        }

        public static List<DelegateFuncTestCase> DelegateFuncTestCases = new List<DelegateFuncTestCase>()
        {
            new DelegateFuncTestCase()
            {
                Description = "Public Function",
                FunctionName = "PublicFunction",
                ShouldHaveDelegate = true
            },
            new DelegateFuncTestCase()
            {
                Description = "Private Function",
                FunctionName = "PrivateFunction",
                ShouldHaveDelegate = true
            },
            new DelegateFuncTestCase()
            {
                Description = "Task without a Cancellation Token",
                FunctionName = "TaskWithoutToken",
                ShouldHaveDelegate = false
            },
            new DelegateFuncTestCase()
            {
                Description = "Invalid Return Type test",
                FunctionName = "InvalidReturnTypeFunction",
                ShouldHaveDelegate = false
            },
            new DelegateFuncTestCase()
            {
                Description = "Non-existent Function",
                FunctionName = "DoesntExist",
                ShouldHaveDelegate = false
            },
            new DelegateFuncTestCase()
            {
                Description = "Public Action",
                FunctionName = "PublicAction",
                ShouldHaveDelegate = false
            }
            
        };
        

        

        public class DelegateFuncTestCase
        {
            public string Description;
            public string FunctionName;
            public bool ShouldHaveDelegate;
            public bool ShouldHaveAction;
            
            public override string ToString()
            {
                return $"[{Description}]";
            }
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
        public void FsmCoreSetDelegateActionTest([ValueSource("DelegateActionTestCases")] DelegateFuncTestCase testCase)
        {
            Action testAction = null;
            FsmCore<FsmCoreTest>.SetDelegateFor(testCase.FunctionName, this, ref testAction);
            Assert.AreEqual(testCase.ShouldHaveAction, testAction != null);
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
        
        public static List<DelegateFuncTestCase> DelegateActionTestCases = new List<DelegateFuncTestCase>()
        {
            new DelegateFuncTestCase()
            {
                Description = "Public Action",
                FunctionName = "PublicAction",
                ShouldHaveAction = true
            },
            new DelegateFuncTestCase()
            {
                Description = "Private Action",
                FunctionName = "PrivateAction",
                ShouldHaveAction = true
            },
            new DelegateFuncTestCase()
            {
                Description = "Action With Args",
                FunctionName = "ActionWithArgs",
                ShouldHaveAction = false
            },
            new DelegateFuncTestCase()
            {
                Description = "Invalid Return Type Action",
                FunctionName = "InvalidReturnTypeAction",
                ShouldHaveAction = false
            },
            new DelegateFuncTestCase()
            {
                Description = "Non-existent Function",
                FunctionName = "DoesntExist",
                ShouldHaveAction = false
            }
            
        };
        
        [Test]
        public void FsmCoreSetDelegateTest([ValueSource("SetDelegateTestCases")] DelegateFuncTestCase testCase)
        {
            Action testAction = null;
            Func<CancellationToken, Task> testFunc = null;
            FsmCore<FsmCoreTest>.SetDelegateFor(testCase.FunctionName, this, ref testFunc, ref testAction);
            Assert.AreEqual(testCase.ShouldHaveAction, testAction != null);
            Assert.AreEqual(testCase.ShouldHaveDelegate, testFunc != null);
        }
        
        [Test]
        public void FsmCoreSetDelegateExceptionTest([ValueSource("SetDelegateExceptionTestCases")] DelegateFuncTestCase testCase)
        {
            Action testAction = null;
            Func<CancellationToken, Task> testFunc = null;
            Assert.Catch<InvalidCastException>(() =>
                FsmCore<FsmCoreTest>.SetDelegateFor(testCase.FunctionName, this, ref testFunc, ref testAction));
        }
        
        
        
        public static List<DelegateFuncTestCase> SetDelegateTestCases = new List<DelegateFuncTestCase>()
        {
            new DelegateFuncTestCase()
            {
                Description = "Public Function",
                FunctionName = "PublicFunction",
                ShouldHaveDelegate = true,
                ShouldHaveAction = false
            },
            new DelegateFuncTestCase()
            {
                Description = "Public Action",
                FunctionName = "PublicAction",
                ShouldHaveDelegate = false,
                ShouldHaveAction = true
            },
            new DelegateFuncTestCase()
            {
                Description = "Private Function",
                FunctionName = "PrivateFunction",
                ShouldHaveDelegate = true,
                ShouldHaveAction = false
            },
            new DelegateFuncTestCase()
            {
                Description = "Private Action",
                FunctionName = "PrivateAction",
                ShouldHaveDelegate = false,
                ShouldHaveAction = true
            },
            new DelegateFuncTestCase()
            {
                Description = "Non-existent Action",
                FunctionName = "DoesNotExist",
                ShouldHaveDelegate = false,
                ShouldHaveAction = false
            },
        };
        
        public static List<DelegateFuncTestCase> SetDelegateExceptionTestCases = new List<DelegateFuncTestCase>()
        {
            new DelegateFuncTestCase()
            {
                Description = "Invalid Return Type Action",
                FunctionName = "InvalidReturnTypeAction",
            },
            new DelegateFuncTestCase()
            {
                Description = "Invalid Return Type Function",
                FunctionName = "InvalidReturnTypeFunction",
            },
            new DelegateFuncTestCase()
            {
                Description = "Invalid Args Function",
                FunctionName = "TaskWithoutToken",
            },
            new DelegateFuncTestCase()
            {
                Description = "Invalid Args Action",
                FunctionName = "ActionWithArgs",
            },

        };
        
        
        
        


    }
}
