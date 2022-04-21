using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAILab.Perception;

public class Test_SenseInstance : MonoBehaviour
{
    public static SightSense sight;
    public static DamageSense damage;

    private void Awake()
    {
        sight = new SightSense();
        damage = new DamageSense();
    }

    private void Update()
    {
        sight.Tick();
        damage.Tick();
    }
}
