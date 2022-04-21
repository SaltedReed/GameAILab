using GameAILab.Core;

namespace GameAILab.Perception
{

    public enum StimuliType
    {
        None = 0,
        Sight,
        Damage
    }


    public class Stimuli
    {
        public StimuliType type;
        public IActor sourceActor;
        public bool isSensed;
    }


}