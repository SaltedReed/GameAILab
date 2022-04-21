using System;
using System.Collections.Generic;

namespace GameAILab.Sandbox
{

    public class ActorSystem
    {
        protected List<Actor> m_actors = new List<Actor>();
        protected int m_nextId = 0;

        public void Register(Actor actor)
        {
            if (actor is null)
                return;

            if (!m_actors.Contains(actor))
            {
                actor.Id = m_nextId++;
                m_actors.Add(actor);
            }
        }

        public void Unregister(Actor actor)
        {
            if (actor is null)
                return;

            if (m_actors.Contains(actor))
            {
                actor.Id = -1;
                m_actors.Remove(actor);
            }
        }

        public bool Contains(Actor actor)
        {
            return actor != null && m_actors.Contains(actor);
        }

        public Actor[] GetActors()
        {
            return m_actors.ToArray();
        }

        public Actor[] GetActors(Predicate<Actor> match)
        {
            return m_actors.FindAll(match).ToArray();
        }

    }

}