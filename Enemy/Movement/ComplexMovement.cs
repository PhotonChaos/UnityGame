using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ComplexMovement : MonoBehaviour
{    
    public float Speed;

    [Min(0)]
    public float StartDelay;

    [Min(0)]
    public float LoopDelay;

    [Min(0)]
    public float StepDelay;

    public Transform[] Waypoints;
    public bool Patrol = true;

    public Vector3 Target;
    public Vector3 MoveDirection;
    public Vector2 Velocity;

    public int CurrentWaypoint;

    private bool started = false;

    private void Update() {
        StartCoroutine(MovementLoop());
    }

    IEnumerator MovementLoop() {
        if(!started) {
            started = true;
            yield return new WaitForSeconds(StartDelay);
        } else if(CurrentWaypoint == 0) {
            yield return new WaitForSeconds(LoopDelay);
        } else {
            yield return new WaitForSeconds(StepDelay);
        }

        if (CurrentWaypoint < Waypoints.Length) {
            Target = Waypoints[CurrentWaypoint].position;
            MoveDirection = Target - transform.position;
            Velocity = GetComponent<Rigidbody2D>().velocity;

            if (MoveDirection.magnitude < 1) {
                CurrentWaypoint++;
            } else {
                Velocity = MoveDirection.normalized * Speed;
            }
        } else {
            if (Patrol) {
                CurrentWaypoint = 0;
            } else {
                Velocity = Vector2.zero;
            }
        }

        GetComponent<Rigidbody2D>().velocity = Velocity;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            other.gameObject.GetComponent<PlayerController>().Die();
        }
    }
}