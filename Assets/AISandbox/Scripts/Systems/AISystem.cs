using System.Collections.Generic;
using UnityEngine;
using GameAILab.Core;
using GameAILab.Decision.FSM;
using GameAILab.Perception;


namespace GameAILab.Sandbox
{

    public class AISystem
    {

        public AISenseSystem SenseSys { get; protected set; }

        protected Dictionary<StateMachineOwner, StateMachine> m_fsms = new Dictionary<StateMachineOwner, StateMachine>();


        #region Sight

        public void RegisterSightStimuliListener(ISightStimuliListener listener)
        {
            SenseSys.RegisterSightStimuliListener(listener);
        }

        public void UnregisterSightStimuliListener(ISightStimuliListener listener)
        {
            SenseSys.UnregisterSightStimuliListener(listener);
        }

        public void RegisterSightStimuliSource(IActor src)
        {
            SenseSys.RegisterSightStimuliSource(src);
        }

        public void UnregisterSightStimuliSource(IActor src)
        {
            SenseSys.UnregisterSightStimuliSource(src);
        }

        #endregion


        #region FSM

        public void RegisterFsm(StateMachineOwner owner, StateMachine fsm)
        {
            if (owner is null)
                throw new System.ArgumentNullException();
            if (fsm is null)
                throw new System.ArgumentNullException();

            m_fsms.Add(owner, fsm);
            fsm.Start();
            Debug.Log($"{owner.gameObject.name} registered fsm {fsm.Name}");
        }

        public void UnregisterFsm(StateMachineOwner owner)
        {
            if (owner is null)
                return;

            StateMachine fsm;
            if (m_fsms.TryGetValue(owner, out fsm))
            {
                fsm.Shutdown();
                m_fsms.Remove(owner);
                Debug.Log($"{owner.gameObject.name} unregistered fsm {fsm.Name}");
            }
        }

        #endregion


        public void PreInit()
        {
            SenseSys = new AISenseSystem();
            SenseSys.PreInit();
        }

        public void Tick()
        {
            SenseSys.Tick();

            foreach (var pair in m_fsms)
            {
                pair.Value.OnUpdate();
                pair.Key.OnAIUpdate(pair.Value);
            }
        }

        public void Shutdown()
        {
            foreach (var fsm in m_fsms.Values)
            {
                fsm.Shutdown();
            }
            m_fsms.Clear();
        }
    }

}