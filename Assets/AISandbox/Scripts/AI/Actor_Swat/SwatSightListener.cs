using GameAILab.Perception;


namespace GameAILab.Sandbox
{


    public class SwatSightListener : SightStimuliListener
    {
        public override void OnSenseUpdate(Stimuli stimuli)
        {
            AIActor aIActor = GetComponent<AIActor>();
            if (aIActor is null || aIActor.Squad is null)
                return;

            if (stimuli.isSensed)
            {
                aIActor.Squad.OnMemberSeePlayer(stimuli.sourceActor);
            }
            else
            {
                aIActor.Squad.OnMemberLosePlayer();
            }
        }
    }

}
