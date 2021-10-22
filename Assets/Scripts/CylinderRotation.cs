using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CylinderRotation : MonoBehaviour
{
    Rigidbody rb;
    public bool isLeftRotation;
    public float speed = 0.1f;
    public float setMaxAngVel = 5f;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = setMaxAngVel;
    }

    // Update is called once per frame
    void Update()
    {
        if (isLeftRotation)
        {
            rb.AddTorque(transform.up * speed);
        }
        else
        {
            rb.AddTorque(-transform.up * speed);
        }
    }
}
