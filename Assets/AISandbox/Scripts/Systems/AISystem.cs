using System.Collections.Generic;
using GameAILab.Decision.FSM;
using GameAILab.Perception;

namespace GameAILab.Sandbox
{

    public class AISystem
    {

        public AISenseSystem SenseSys { get; protected set; }

        protected Dictionary<StateMachineOwner<AIActor>, StateMachine<AIActor>> m_fsms = new Dictionary<StateMachineOwner<AIActor>, StateMachine<AIActor>>();

        
        public void RegisterFsm(StateMachineOwner<AIActor> owner, StateMachine<AIActor> fsm)
        {
            if (owner is null)
                throw new System.ArgumentNullException();
            if (fsm is null)
                throw new System.ArgumentNullException();

            m_fsms.Add(owner, fsm);
        }

        public void UnregisterFsm(StateMachineOwner<AIActor> owner)
        {
            if (owner != null)
            {
                m_fsms.Remove(owner);
            }
        }

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
    }

}