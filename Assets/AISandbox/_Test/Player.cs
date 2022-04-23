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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawRay(transform.position, transform.forward * 5);
    }
}
