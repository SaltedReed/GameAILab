using UnityEngine;

namespace GameAILab.Decision.FSM
{
    public class StateMachineBuilder
    {
        public StateMachine FSM { get; protected set; }

        public StateMachineBuilder StateMachine(string name, GameObject owner, State defaultState = null)
        {
            FSM = new StateMachine(name, owner, defaultState);
            return this;
        }

        public StateMachine EndStateMachine()
        {
            return FSM;
        }

        public StateMachineBuilder DefaultState(State state)
        {
            FSM.SetDefaultState(state);
            return this;
        }

        public StateMachineBuilder State(State state)
        {
            FSM.AddState(state);
            return this;
        }

    }

}