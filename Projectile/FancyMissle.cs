using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FancyMissle : MonoBehaviour
{
    public float Acceleration;
    public float AngularVelocity;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        rb.angularVelocity = new Vector3(0, AngularVelocity, 0);
    }

    private void FixedUpdate() {
        rb.velocity += new Vector3(0, Acceleration * Time.deltaTime, 0);
    }
}
