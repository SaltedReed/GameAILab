using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAILab.Perception;
using GameAILab.Core;

public class Test_SightListener : MonoBehaviour, ISightStimuliListener, IActor
{
    public float HalfAngleDegrees { get => halfAngleDegrees; set => halfAngleDegrees = value; }
    public float halfAngleDegrees;

    public float Radius { get => radius; set => radius = value; }
    public float radius;

    public float Height { get => height; set => height = value; }
    public float height;

    public int Id { get; set; }

    public GameObject Go { get => gameObject; set { } }
    
    public AffiliationType TargetAffiliations { get => targetAffiliation; set => targetAffiliation = value; }
    public AffiliationType targetAffiliation = AffiliationType.Hostile;

    public AffiliationType Affiliation { get => affiliation; set => affiliation = value; }
    public AffiliationType affiliation = AffiliationType.Friendly;

    public void OnSenseUpdate(Stimuli stimuli)
    {
        Debug.Log("see: " + stimuli.isSensed);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Vector3 pos = transform.position;
        Vector3 left = Quaternion.AngleAxis(-halfAngleDegrees, Vector3.up) * transform.forward;
        Vector3 right = Quaternion.AngleAxis(halfAngleDegrees, Vector3.up) * transform.forward;

        Gizmos.DrawRay(pos, left * Radius);
        Gizmos.DrawRay(pos, right * Radius);

        Gizmos.DrawRay(pos + new Vector3(0, height, 0), left * Radius);
        Gizmos.DrawRay(pos + new Vector3(0, height, 0), right * Radius);
    }
}
