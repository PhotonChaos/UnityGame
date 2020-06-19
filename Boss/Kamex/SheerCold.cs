using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheerCold : SpellCardBase
{
    public GameObject IceCrystal;

    private float speed = 9.5f;

    public override IEnumerator StartCooldown() {
        string[] stars = { "StarBlue", "StarWhite" };

        while(true) {
            for(int i = 0; i < 6; i++) {
                ShootSFX();
                StartCoroutine(CircleBullet(stars[Random.Range(0, stars.Length)], 15, 9.5f, rotation: Random.Range(-45f, 45f)));
                yield return new WaitForSeconds(0.1f);
            }

            switch(Random.Range(0, 6)) {
                case 0:
                    User.GoToWaypoint(StageController.Stage.Waypoints.TOP_LEFT);
                    break;

                case 1:
                    User.GoToWaypoint(StageController.Stage.Waypoints.TOP_CENTER);
                    break;

                case 2:
                    User.GoToWaypoint(StageController.Stage.Waypoints.TOP_RIGHT);
                    break;

                case 3:
                    User.GoToWaypoint(StageController.Stage.Waypoints.MID_CENTER);
                    break;

                case 4:
                    User.GoToWaypoint(StageController.Stage.Waypoints.MID_LEFT);
                    break;

                case 5:
                    User.GoToWaypoint(StageController.Stage.Waypoints.MID_RIGHT);
                    break;
            }

            yield return new WaitForSeconds(2.0f);
        }
    }

    public override IEnumerator StartSpell() {
        yield return new WaitForSeconds(0.8f);
        while (User.IsMoving()) {
            yield return new WaitForSeconds(0.0001f);
        }

        int shards = 20;

        GetComponent<Rigidbody2D>().AddTorque(130.0f);

        StartCoroutine(Spiral("IceCrystal", 5, 4, 6.25f, 0.2f));

        while (true) {
            for (int j = 0; j < 3; j++) {
                ShootSFX();
                for (int i = 0; i < shards; i++) {
                    // Rings
                    GameObject blueStar = ObjectPooler.Spawn("StarBlue", transform.position, Quaternion.identity);
                    blueStar.transform.Rotate(new Vector3(0, 0, (360 / shards * i) - 90));
                    blueStar.transform.localScale = new Vector3(1.25f, 1.25f, 1);

                    Rigidbody2D rb = blueStar.GetComponent<Rigidbody2D>();
                    rb.velocity = blueStar.transform.up * speed;
                    rb.angularVelocity = Random.Range(50f, 80f) * (Random.Range(0,2) == 1 ? -1:1);
                }

                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitForSeconds(1f);
        }
    }
}
