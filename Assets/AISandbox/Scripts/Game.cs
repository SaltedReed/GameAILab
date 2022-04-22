

namespace GameAILab.Sandbox
{

    public class Game : SingletonMono<Game>
    {
        public ActorSystem ActorSys { get; protected set; }
        public AISystem AISys { get; protected set; }

        protected override void Awake()
        {
            base.Awake();

            ActorSys = new ActorSystem();  
            
            AISys = new AISystem();
            AISys.PreInit();
        }

        protected virtual void Update()
        {
            AISys.Tick();
        }

        protected virtual void OnDestroy()
        {
            AISys.Shutdown();
        }
    }

}
