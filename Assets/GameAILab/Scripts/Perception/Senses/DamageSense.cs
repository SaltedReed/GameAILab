using System;
using System.Collections.Generic;
using UnityEngine;
using GameAILab.Core;

namespace GameAILab.Perception
{

    public class DamageEvent
    {
        public IActor damagedActor;
        public IActor instigator;
        public float amount;
        public Vector3 hitPos;
    }


    public class DamageSense : Sense
    {
        protected List<DamageEvent> m_events = new List<DamageEvent>();

        public override void Tick()
        {
            foreach (DamageEvent ev in m_events)
            {
                IDamageStimuliListener listener = ev.damagedActor as IDamageStimuliListener;
                if (listener is null)
                {
                    continue;
                }

                Stimuli st = new Stimuli { type = StimuliType.Damage, sourceActor = ev.instigator, isSensed = true };
                listener.OnSenseUpdate(st);
            }

            m_events.Clear();
        }

        public void RegisterEvent(DamageEvent ev)
        {
            if (ev is null)
                throw new ArgumentNullException();

            m_events.Add(ev);
        }

    }

}