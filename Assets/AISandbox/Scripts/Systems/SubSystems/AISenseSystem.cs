using GameAILab.Perception;
using GameAILab.Core;

namespace GameAILab.Sandbox
{

    public class AISenseSystem
    {
        public SightSense SightSensor { get; protected set; }
        public DamageSense DamageSensor { get; protected set; }

        #region Sight

        public void RegisterSightStimuliListener(ISightStimuliListener listener)
        {
            SightSensor.RegisterListener(listener);
        }

        public void UnregisterSightStimuliListener(ISightStimuliListener listener)
        {
            SightSensor.UnregisterListener(listener);
        }

        public void RegisterSightStimuliSource(IActor src)
        {
            SightSensor.RegisterStimuliSource(src);
        }

        public void UnregisterSightStimuliSource(IActor src)
        {
            SightSensor.UnregisterStimuliSource(src);
        }

        #endregion

        public void PreInit()
        {
            SightSensor = new SightSense();
            DamageSensor = new DamageSense();
        }

        public void Tick()
        {
            SightSensor.Tick();
            DamageSensor.Tick();
        }
    }

}