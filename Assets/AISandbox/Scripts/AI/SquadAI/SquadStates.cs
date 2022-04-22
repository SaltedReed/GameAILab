using GameAILab.Decision.FSM;


namespace GameAILab.Sandbox
{

    public abstract class SquadState : State
    {
        public AISquad OwnerSquad { get; protected set; }

        public SquadState(string name, AISquad squad)
            : base(name)
        {
            OwnerSquad = squad;
        }
    }


    public class SquadRelaxedState : SquadState
    {
        public SquadRelaxedState(string name, AISquad squad)
            : base(name, squad) { }

        public override string OnUpdate()
        {
            throw new System.NotImplementedException();
        }
    }


    public class SquadCombatState : SquadState
    {
        public SquadCombatState(string name, AISquad squad)
            : base(name, squad) { }

        public override string OnUpdate()
        {
            throw new System.NotImplementedException();
        }
    }

}