using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAILab.Perception;
using GameAILab.Core;

public class Test_DamageListener : MonoBehaviour, IDamageStimuliListener, IActor
{
    public int Id { get; set; }
    public GameObject Go { get => gameObject; set { } }

    public AffiliationType TargetAffiliations { get => targetAffiliation; set => targetAffiliation = value; }
    public AffiliationType targetAffiliation = AffiliationType.Hostile;

    public AffiliationType Affiliation { get => affiliation; set => affiliation = value; }
    public AffiliationType affiliation = AffiliationType.Friendly;

    public void OnSenseUpdate(Stimuli stimuli)
    {
        Debug.Log("damaged: " + stimuli.isSensed);
    }
}
