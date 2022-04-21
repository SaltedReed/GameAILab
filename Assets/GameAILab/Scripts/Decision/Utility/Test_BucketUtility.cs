using UnityEngine;
using GameAILab.Decision.BucketUt;


public class A1 : Action
{
    int f = 0;

    public A1(string name)
        : base(name) { }

    protected override float DoCalculation()
    {
        return Time.frameCount%2==1 ? 0.3f : 0.6f;
    }

    protected override void OnInit()
    {
        base.OnInit();
        f = 0;
    }

    protected override ActionState OnUpdate()
    {
        return f++%3==2 ? ActionState.Success : ActionState.Running;
    }
}

public class A2 : Action
{
    public A2(string name)
        : base(name) { }

    protected override float DoCalculation()
    {
        return Time.frameCount % 2 == 1 ? 0.6f : 0.3f;
    }

    protected override ActionState OnUpdate()
    {
        return ActionState.Success;
    }
}


public class Test_BucketUtility : MonoBehaviour
{
    //public BucketUtility.ActionChoosePolicy choosePolicy;

    BucketUtilityAI ai;

    private void Start()
    {
        A1 a1 = new A1("A1");
        A2 a2 = new A2("A2");
        A1 a3 = new A1("A3");
        A2 a4 = new A2("A4");

        BucketUtilityBuilder builder = new BucketUtilityBuilder();
        ai = builder.Utility("u1"/*, choosePolicy*/)
                .Bucket("B1", (Bucket b) => { return 1.0f; })
                    .Action(a1)
                    .Action(a2)
                .EndBucket()
                .Bucket("B2", (Bucket b) => { return 0.5f; })
                    .Action(a3)
                    .Action(a4)
                .EndBucket()
            .EndUtility();
    }

    private void Update()
    {
        ai.Tick();
        Debug.Log(ai.CurrentAction is null ? "null" : ai.CurrentAction.Name);
    }
}
