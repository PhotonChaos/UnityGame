using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMovement : MonoBehaviour
{
    public float XSpeed;
    public float YSpeed;
    public float XAcceleration;
    public float YAcceleration;

    public float MaxVelocity = 1000000;
    public float MinVelocity = 0;

    private Rigidbody2D rb;

    private Vector2 StopAtPos;
    private float StopForTime;
    private Vector2 VelocityAfterStop;

    private void Awake() {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    void Start() {
        rb.velocity = new Vector2(XSpeed, YSpeed);
    }

    void FixedUpdate() {
        rb.velocity += new Vector2(XAcceleration * Time.deltaTime, YAcceleration * Time.deltaTime);

        if (rb.velocity.sqrMagnitude > Mathf.Pow(MaxVelocity, 2)) rb.velocity = rb.velocity.normalized * MaxVelocity;
        else if (rb.velocity.sqrMagnitude < MinVelocity) rb.velocity = rb.velocity.normalized * MinVelocity;
    }

    public void UpdateVelocity(float vx, float vy) {
        XSpeed = vx;
        YSpeed = vy;
        rb.velocity = new Vector2(vx, vy);
    }

    public void UpdateAcceleration(float ax, float ay) {
        XAcceleration = ax;
        YAcceleration = ay;
    }
}
