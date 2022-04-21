using UnityEngine;

public class Steering : MonoBehaviour
{
    public Quaternion Orientation;

    public Vector3 Velocity = Vector3.zero;
    public float AngularSpeed = 300.0f;

    public Vector3 Accele = Vector3.zero;
    public float AngularAccele = 0.0f;


    private void Start()
    {
        Orientation = transform.rotation;
    }

    private void Update()
    {
        float delta = Time.deltaTime;

        transform.position += Velocity * delta;        
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Orientation, AngularSpeed * delta);

        Velocity += Accele * delta;
        AngularSpeed += AngularAccele * delta;
    }
}
