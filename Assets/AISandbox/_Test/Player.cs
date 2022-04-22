using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAILab.Perception;
using GameAILab.Sandbox;
using GameAILab.Core;

public class Player : Actor
{
    protected override void Start()
    {
        base.Start();
        Game.Instance.AISys.RegisterSightStimuliSource(this);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        Game.Instance.AISys.UnregisterSightStimuliSource(this);
    }
}
