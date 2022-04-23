using System.Collections.Generic;
using UnityEngine;

namespace GameAILab.Perception
{

    // NOTE: if a sight stimuli source changed its affiliation after registered itself as a stimuli source,
    // it must unregister and re-register itself as a stimuli source again
    public class SightSense : Sense
    {
        protected class QueryEntry
        {
            public StimuliSource source;
            public bool lastSenseResult;

            public QueryEntry(StimuliSource src)
            {
                source = src;
                lastSenseResult = false;
            }
        }

        protected class QueryRecord
        {
            public IStimuliListener listener;
            // store the stimuli sources that have been filtered (that are possible to be sensed by the listener)
            public List<QueryEntry> entries;

            public QueryRecord(IStimuliListener ls)
            {
                listener = ls;
                entries = new List<QueryEntry>();
            }
        }


        protected List<QueryRecord> m_records = new List<QueryRecord>();


        protected override void OnRegisterListener(IStimuliListener listener)
        {
            base.OnRegisterListener(listener);

            QueryRecord record = new QueryRecord(listener);
            m_records.Add(record);

            // filter
            foreach (StimuliSource src in m_stimuliSources)
            {
                if ((listener.TargetAffiliations & src.Affiliation) == src.Affiliation)
                {
                    record.entries.Add(new QueryEntry(src));
                }
            }
        }

        protected override void OnUnregisterListener(IStimuliListener listener)
        {
            base.OnUnregisterListener(listener);

            int index = m_records.FindIndex((QueryRecord r) => { return r.listener == listener; });
            if (index >= 0)
            {
                m_records.RemoveAt(index);
            }
        }

        protected override void OnRegisterStimuliSource(StimuliSource src)
        {
            base.OnRegisterStimuliSource(src);

            // filter
            foreach (QueryRecord record in m_records)
            {
                if ((record.listener.TargetAffiliations & src.Affiliation) == src.Affiliation)
                {
                    record.entries.Add(new QueryEntry(src));
                }
            }
        }

        protected override void OnUnregisterStimuliSource(StimuliSource src)
        {
            base.OnUnregisterStimuliSource(src);

            // update sense if necessary, and remove src from the record list and the source list
            foreach (QueryRecord record in m_records)
            {
                int ei = record.entries.FindIndex((QueryEntry e) => { return e.source.actor == src.actor; });
                if (ei < 0)
                    continue;

                QueryEntry entry = record.entries[ei];
                if (entry.lastSenseResult)
                {
                    Stimuli st = new Stimuli { type = StimuliType.Sight, sourceActor = src.actor, isSensed = false };
                    record.listener.OnSenseUpdate(st);
                }

                record.entries.RemoveAt(ei);
            }

            m_stimuliSources.Remove(src);
        }


        public override void Tick()
        {
            for (int ri = 0; ri < m_records.Count; ++ri)
            {
                QueryRecord record = m_records[ri];

                for (int ei = 0; ei < record.entries.Count; ++ei)
                {
                    QueryEntry entry = record.entries[ei];

                    bool sensed = CanSee(record.listener as ISightStimuliListener, entry.source);
                    if (sensed == entry.lastSenseResult)
                        continue;

                    entry.lastSenseResult = sensed;

                    Stimuli st = new Stimuli { type = StimuliType.Sight, sourceActor = entry.source.actor, isSensed = sensed };
                    record.listener.OnSenseUpdate(st);
                }
            }
        }

        protected virtual bool CanSee(ISightStimuliListener sightLsn, StimuliSource source)
        {
            if (sightLsn is null)
                return false;

            // test sources
            Vector3 lsnPos = sightLsn.Go.transform.position;
            Vector3 srcPos = source.Position;
            float ydst = srcPos.y - lsnPos.y;
            if (ydst > sightLsn.Height || ydst < -1) // todo: parameterize lowerHeight
                return false;

            float dst = (lsnPos - srcPos).magnitude; // temp
            if (dst > sightLsn.Radius)
                return false;

            Vector3 lsnFwdDir = sightLsn.Go.transform.forward;
            Vector3 lsnSrcDir = (srcPos - lsnPos).normalized;
            if (Vector3.Angle(lsnFwdDir, lsnSrcDir) > sightLsn.HalfAngleDegrees)
                return false;

            // raycast
            RaycastHit hit;
            Ray ray = new Ray { origin = lsnPos, direction = lsnSrcDir };
            if (Physics.Raycast(ray, out hit, dst))
            {
                if (hit.collider.gameObject != source.actor.Go)
                    return false;
            }

            return true;
        }

    }

}