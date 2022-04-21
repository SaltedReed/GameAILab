using GameAILab.Perception;

namespace GameAILab.Sandbox
{

    public class AISenseSystem
    {
        public SightSense SightSensor { get; protected set; }
        public DamageSense DamageSensor { get; protected set; }

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