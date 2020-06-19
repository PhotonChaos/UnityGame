using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideShot : MonoBehaviour
{
    public enum Type
    {
        FIRE,
        PLASMA
    }

    private const float Speed = 45.0f;
    private const float WispRate = 10.0f;
    private const float PlasmaRate = 2.0f;
    private const float PauseUnits = 4.0f;

    public GameObject Player;

    private PlayerController PlayerStats;

    public static bool Shooting = false;
    public static int Power = 0;

    public Type ShotType;
    public GameObject PlasmaBullet;
    public GameObject WispBullet;

    private List<Rigidbody2D> Bullets = new List<Rigidbody2D>();

    private Transform Target;

    // Start is called before the first frame update
    void Start() {
        Target = GetClosestEnemy();
        StartCoroutine(Fire());
    }

    // Update is called once per frame
    void FixedUpdate() {
        if(ShotType == Type.FIRE) {
            if(Target == null) Target = GetClosestEnemy();
            
            List<Rigidbody2D> toRem = new List<Rigidbody2D>();

            if (Target == null) {
                foreach (Rigidbody2D bullet in Bullets) {
                    if(bullet == null) {
                        toRem.Add(bullet);
                        continue;
                    }

                    bullet.velocity = new Vector3(0, Speed, 0);
                }
            } else {
                foreach (Rigidbody2D bullet in Bullets) {
                    if (bullet == null) {
                        toRem.Add(bullet);
                        continue;
                    }

                    Vector3 dist = Target.position - bullet.transform.position;

                    if(dist.sqrMagnitude > (Target.position - transform.position).sqrMagnitude) {
                        bullet.velocity = (GetClosestEnemy(bullet.transform, EnemyHomable.EnemyPositions).position - bullet.transform.position).normalized * Speed;
                        return;
                    }

                    bullet.velocity = dist.normalized * Speed;
                }
            }

            foreach(Rigidbody2D rem in toRem) {
                Bullets.Remove(rem);
            }
        } 
    }

    public IEnumerator Fire() {
        int count;

        if (ShotType == Type.FIRE) {
            while (true) {
                if (Power >= 4) {
                    count = 2;
                } else if (Power >= 2) {
                    count = 1;
                } else {
                    count = 0;
                }

                if (Shooting) {
                    for (int i = 0; i < count; i++) {
                        StartCoroutine(SpawnBullet());
                    }
                }

                yield return new WaitForSeconds(1 / WispRate);
            }
        } else {
            while (true) {
                if (Power >= 4) {
                    count = 3;
                } else if (Power >= 3) {
                    count = 2;
                } else if (Power > 1) {
                    count = 1;
                } else {
                    count = 0;
                }

                if (Shooting) {
                    for (int i = 0; i < count; i++) {
                        GameObject go = Instantiate(PlasmaBullet);
                        Vector2 rand = Random.insideUnitCircle;
                        Vector3 rand3 = new Vector3(rand.x, rand.y, 0) * 0.6f;

                        go.transform.position = transform.position + Vector3.up + rand3;
                    }
                }

                yield return new WaitForSeconds(1/(PlasmaRate+count/2));
            }
        }
    }

    public IEnumerator SpawnBullet() {
        GameObject go = Instantiate(WispBullet);
        Rigidbody2D rb = go.GetComponent<Rigidbody2D>();

        Vector2 r = Random.insideUnitCircle;
        Vector3 rand = new Vector3(r.x, r.y, 0) * 0.6f;

        go.transform.position = transform.position + Vector3.up + rand;
        rb.angularVelocity = 1.0f;
        rb.velocity = Vector3.up * Speed;

        yield return new WaitForSeconds(PauseUnits/Speed);

        Bullets.Add(rb);
    }

    private Transform GetClosestEnemy() {
        return GetClosestEnemy(transform, EnemyHomable.EnemyPositions);
    }

    private Transform GetClosestEnemy(Transform pos, List<Transform> Enemies) {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;

        foreach (Transform potentialTarget in EnemyHomable.EnemyPositions) {
            Vector3 directionToTarget = potentialTarget.position - pos.position;
            float dSqrToTarget = directionToTarget.sqrMagnitude;

            if (dSqrToTarget < closestDistanceSqr) {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }

        return bestTarget;
    }
}
