using UnityEngine;

[RequireComponent(typeof(Steering))]
public class Flee : MonoBehaviour
{
    public Transform Target;
    public float MaxSpeed = 3.0f;

    private Steering m_Steer;


    public void UpdateVelocities()
    {
        Vector3 v = (transform.position - Target.position).normalized;

        v.y = transform.position.y;
        m_Steer.Orientation = Quaternion.LookRotation(v);

        v.y = 0;
        m_Steer.Velocity = v * MaxSpeed;
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
