using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameAILab.Decision.FSM
{
    public abstract class State
    {
        public string Name { get; protected set; }
        public GameObject Owner { get; set; }

        public State(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("state name cannot be null or empty");
            }

            Name = name;
        }

        public virtual void OnEnter() { }
        public abstract string OnUpdate();
        public virtual void OnExit() { }
    }


    public class StateMachine : State
    {
        public State CurrentState { get; protected set; }

        protected Dictionary<string, State> m_states = new Dictionary<string, State>();

        public StateMachine(string name, GameObject owner, State defaultState)
            : base(name)
        {
            Owner = owner;

            if (defaultState != null)
            {
                SetDefaultState(defaultState);
            }
        }

        #region State Management

        public void SetDefaultState(State state)
        {
            if (state != null)
            {
                AddState(state);
                CurrentState = state;
            }
        }

        public bool AddState(State state)
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

        public void RemoveState(State state)
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

        public void ChangeState(State state)
        {
            CurrentState?.OnExit();
            CurrentState = state;
            CurrentState?.OnEnter();
        }

        public void ChangeStateByName(string name)
        {
            State state;
            if (m_states.TryGetValue(name, out state))
            {
                ChangeState(state);
            }
            else
            {
                throw new Exception("invalid state name");
            }
        }

        #endregion


        public void Start()
        {
            if (CurrentState is null)
            {
                Debug.LogWarning($"{Name} does not have a default state");
            }

            CurrentState?.OnEnter();
        }

        public void Shutdown()
        {
            CurrentState?.OnExit();
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

    }

}