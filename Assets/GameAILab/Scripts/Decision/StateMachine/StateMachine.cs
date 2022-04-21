using System;
using System.Collections.Generic;

namespace GameAILab.Decision.FSM
{

    public abstract class State<T>
    {
        public string Name { get; protected set; }
        public T Owner { get; set; }

        public State(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("state name cannot be null or empty");
            }

            Name = name;
        }

        public virtual void OnEnter() { UnityEngine.Debug.Log("enter " + Name); }
        public abstract string OnUpdate();
        public virtual void OnExit() { UnityEngine.Debug.Log("exit " + Name); }
    }


    public class StateMachine<T> : State<T>
    {
        public State<T> CurrentState { get; protected set; }

        protected Dictionary<string, State<T>> m_states = new Dictionary<string, State<T>>();

        public StateMachine(string name, T owner, State<T> defaultState)
            : base(name)
        {
            Owner = owner;

            if (defaultState != null)
            {
                SetDefaultState(defaultState);
            }
        }

        public void SetDefaultState(State<T> state)
        {
            if (state != null)
            {
                AddState(state);
                ChangeState(state);
            }
        }

        public bool AddState(State<T> state)
        {
            if (state is null)
            {
                throw new ArgumentNullException();
            }
            if (string.IsNullOrEmpty(state.Name))
            {
                throw new ArgumentException("state name cannot be null or empty");
            }            
            if (m_states.ContainsKey(state.Name))
            {
                throw new Exception($"fsm already contains state {state.Name}");
            }

            state.Owner = Owner;
            m_states.Add(state.Name, state);
            return true;
        }

        public void RemoveState(State<T> state)
        {
            if (state != null)
            {
                RemoveState(state.Name);
            }
        }

        public void RemoveState(string name)
        {
            m_states.Remove(name);
        }

        public void ChangeState(State<T> state)
        {
            CurrentState?.OnExit();
            CurrentState = state;
            CurrentState?.OnEnter();
        }

        public void ChangeStateByName(string name)
        {
            State<T> state;
            if (m_states.TryGetValue(name, out state))
            {
                ChangeState(state);
            }
            else
            {
                throw new Exception("invalid state name");
            }
        }

        public override string OnUpdate()
        {
            if (CurrentState is null)
                return Name;

            string newStateName = CurrentState.OnUpdate();
            if (newStateName != CurrentState.Name)
            {
                ChangeStateByName(newStateName);
            }

            return Name;
        }

        public void OnShutdown()
        {
            ChangeState(null);
        }
    }

}