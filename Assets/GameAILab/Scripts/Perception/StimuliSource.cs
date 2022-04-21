using UnityEngine;
using GameAILab.Core;

namespace GameAILab.Perception
{

    public class StimuliSource
    {
        public IActor actor;

        public Vector3 Position => actor.Go.transform.position;
        public AffiliationType Affiliation => actor.Affiliation;
    }


}