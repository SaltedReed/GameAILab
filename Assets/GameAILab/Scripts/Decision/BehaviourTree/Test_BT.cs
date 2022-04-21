using GameAILab.Decision.BT;
using GameAILab.Decision.BTVariants;

public class SuccessNode : Behaviour
{
    public SuccessNode(string bhvName)
        : base(bhvName)
    {

    }

    protected override BehaviourState OnUpdate()
    {
        return BehaviourState.Success;
    }
}

public class FailureNode : Behaviour
{
    public FailureNode(string bhvName)
        : base(bhvName)
    {

    }

    protected override BehaviourState OnUpdate()
    {
        return BehaviourState.Failure;
    }
}

public class RunningNode : Behaviour
{
    public int frames;
    public BehaviourState result;

    private int m_cur = 0;

    public RunningNode(string bhvName, int fm, BehaviourState rs = BehaviourState.Success)
        : base(bhvName)
    {
        frames = fm;
        result = rs;
    }

    public RunningNode(int fm, BehaviourState rs = BehaviourState.Success)
    {
        frames = fm;
        result = rs;
    }

    protected override void OnInit()
    {
        base.OnInit();
        m_cur = 0;
    }

    protected override BehaviourState OnUpdate()
    {
        if (m_cur++ >= frames)
        {
            return result;
        }
        return BehaviourState.Running;
    }
}

public class UB1 : UtBehaviour
{

    public UB1() { }

    public UB1 (string nm)
        : base(nm) { }

    protected override float DoCalculation()
    {
        return 5.0f;
    }

    protected override BehaviourState OnUpdate()
    {
        return BehaviourState.Failure;
    }
}


public class UB2 : UtBehaviour
{
    public UB2() { }

    public UB2(string nm)
        : base(nm) { }

    protected override float DoCalculation()
    {
        return 2.0f;
    }

    protected override BehaviourState OnUpdate()
    {
        return BehaviourState.Failure;
    }
}



public class Test_BT : UnityEngine.MonoBehaviour
{
    private BehaviourTree m_bt;

    private void Start()
    {
        SuccessNode succ1 = new SuccessNode("succ1");
        FailureNode fail1 = new FailureNode("fail1");
        RunningNode run1 = new RunningNode("run1", 3);
        UB1 ub1 = new UB1("ub1");
        UB2 ub2 = new UB2("ub2");

#if false

        BehaviourTree m_bt = new BehaviourTree();

        Sequence seq1 = new Sequence("seq1");
        Selector slc1 = new Selector("slc1");
        
        slc1.AddChild(fail1);
        slc1.AddChild(run1);
        seq1.AddChild(succ1);
        seq1.AddChild(slc1);
        m_bt.Root = seq1;
#endif

        BehaviourTreeBuilder builder = new BehaviourTreeBuilder();
#if false
        m_bt = builder.Tree("t1")
                            .Sequence("seq1")
                                .Task(succ1)
                                .Selector("slc1")
                                    .Task(fail1)
                                    .Task(run1)
                                .EndSelector()
                            .EndSequence()
                      .EndTree();
#endif
        m_bt = builder.Tree("ut1")
                            .Sequence("seq1")
                                .UtSelector("uslc1", UtSelector.Policy.WeightedRandom)
                                    .UtBehaviour(ub2)
                                    .UtBehaviour(ub1)
                                .EndUtSelector()
                                .Task(fail1)
                            .EndSequence()
                       .EndTree();

    }

    private void Update()
    {
        m_bt.Tick();
    }
}
