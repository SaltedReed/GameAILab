using UnityEngine;

[RequireComponent(typeof(Steering))]
public class Seek : MonoBehaviour
{
    public Transform Target;
    public float Radius = 2.0f;
    [Header("Update Velocities")]
    public float MaxSpeed = 3.0f;
    [Header("Update Accelerations")]
    public float MaxAccele = 3.0f;
    


    private Steering m_Steer;


    public void UpdateVelocities()
    {
        Vector3 d = (Target.position - transform.position).normalized;

        d.y = transform.position.y;
        m_Steer.Orientation = Quaternion.LookRotation(d);

        if (Vector3.Distance(transform.position, Target.position) <= Radius)
        {
            d = Vector3.zero;
        }
        d.y = 0;
        m_Steer.Velocity = d * MaxSpeed;
    }


    private void Start()
    {
        m_Steer = GetComponent<Steering>();
    }

    private void Update()
    {
        UpdateVelocities();
    }
}
