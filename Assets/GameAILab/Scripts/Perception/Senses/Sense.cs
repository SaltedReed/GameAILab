using System;
using System.Collections.Generic;
using GameAILab.Core;


namespace GameAILab.Perception
{

    public abstract class Sense
    {

        protected List<StimuliSource> m_stimuliSources = new List<StimuliSource>();
        protected List<IStimuliListener> m_listeners = new List<IStimuliListener>();

        public abstract void Tick();

        protected virtual void OnRegisterListener(IStimuliListener listener) { }

        protected virtual void OnUnregisterListener(IStimuliListener listener) { }

        protected virtual void OnRegisterStimuliSource(StimuliSource src) { }

        protected virtual void OnUnregisterStimuliSource(StimuliSource src) { }


        // Do NOT check redundancy
        public void RegisterListener(IStimuliListener listener)
        {
            if (listener is null)
                throw new ArgumentNullException();

            m_listeners.Add(listener);
            OnRegisterListener(listener);
        }

        // If 'listener' is registerd more than once, it will remove the first one 
        public void UnregisterListener(IStimuliListener listener)
        {
            if (listener is null)
                return;

            OnUnregisterListener(listener);
            m_listeners.Remove(listener);
        }

        // Do NOT check redundancy
        public void RegisterStimuliSource(IActor srcActor)
        {
            if (srcActor is null)
                throw new ArgumentNullException();

            StimuliSource src = new StimuliSource { actor = srcActor };
            m_stimuliSources.Add(src);
            OnRegisterStimuliSource(src);
        }

        // If 'srcActor' is registerd more than once, it will remove the first one 
        public void UnregisterStimuliSource(IActor srcActor)
        {
            if (srcActor is null)
                return;

            int index = m_stimuliSources.FindIndex((StimuliSource s) => { return s.actor == srcActor; });
            if (index < 0)
                return;

            StimuliSource src = m_stimuliSources[index];
            OnUnregisterStimuliSource(src);
            m_stimuliSources.Remove(src);
        }

    }

}