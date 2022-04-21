namespace GameAILab.Decision.FSM
{

    public class StateMachineBuilder<T>
    {
        public StateMachine<T> FSM { get; protected set; }

        public StateMachineBuilder<T> StateMachine(string name, T owner, State<T> defaultState = null)
        {
            FSM = new StateMachine<T>(name, owner, defaultState);
            return this;
        }

        public StateMachine<T> EndStateMachine()
        {
            return FSM;
        }

        public StateMachineBuilder<T> DefaultState(State<T> state)
        {
            FSM.SetDefaultState(state);
            return this;
        }

        public StateMachineBuilder<T> State(State<T> state)
        {
            FSM.AddState(state);
            return this;
        }

    }

}