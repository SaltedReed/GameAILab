using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAILab.Perception;
using GameAILab.Core;

public class Test_StimuliSource : MonoBehaviour, IActor
{
    public int Id { get; set; }
    public GameObject Go { get => gameObject; set { } }
    public AffiliationType Affiliation { get => targetAffiliation; set => targetAffiliation = value; }
    public AffiliationType targetAffiliation = AffiliationType.Hostile;

    public GameObject sightGo;
    public GameObject damageGo;

    public ISightStimuliListener sightListener;
    public IDamageStimuliListener damageListener;


    private void Start()
    {
        sightListener = sightGo.GetComponent<ISightStimuliListener>();
        damageListener = damageGo.GetComponent<IDamageStimuliListener>();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("reg sight source"))
        {
            Test_SenseInstance.sight.RegisterStimuliSource(this);
            Debug.Log("reg sight source");
        }

        if (GUILayout.Button("unreg sight source"))
        {
            Test_SenseInstance.sight.UnregisterStimuliSource(this);
            Debug.Log("unreg sight source");
        }

        if (GUILayout.Button("reg sight listener"))
        {
            Test_SenseInstance.sight.RegisterListener(sightListener);
            Debug.Log("reg sight listener");
        }

        if (GUILayout.Button("unreg sight listener"))
        {
            Test_SenseInstance.sight.UnregisterListener(sightListener);
            Debug.Log("unreg sight listener");
        }

        if (GUILayout.Button("damage stimuli"))
        {
            Test_SenseInstance.damage.RegisterEvent(new DamageEvent
            { amount = 1, damagedActor = damageListener as IActor, instigator = this, hitPos = Vector3.zero });
            Debug.Log("damage stimuli");
        }
    }
}
