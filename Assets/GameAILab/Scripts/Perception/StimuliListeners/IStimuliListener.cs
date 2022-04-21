using UnityEngine;
using GameAILab.Core;

namespace GameAILab.Perception
{

    public interface IStimuliListener
    {
        GameObject Go { get; set; }
        AffiliationType TargetAffiliations { get; set; }

        void OnSenseUpdate(Stimuli stimuli);
    }



}
