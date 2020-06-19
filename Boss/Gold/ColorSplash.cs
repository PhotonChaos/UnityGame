using System.Collections;
using UnityEngine;

public class ColorSplash : SpellCardBase
{
    private char state = 'c';

    public override IEnumerator StartCooldown() {
        while (true) {
            int attack = Random.Range(1, 3);

            switch (attack) {
                case 1:
                    ShootSFX();
                    StartCoroutine(CircleBullet("PastelYellow", 15, 9.5f, 1));
                    break;

                case 2:
                    // Random burst of pink
                    for (int i = 0; i < 12; i++) {
                        GameObject bullet = ObjectPooler.Spawn("PastelRed", transform.position, Quaternion.identity);

                        Vector2 velocity = Random.insideUnitCircle * 9;

                        if (velocity.sqrMagnitude < 16) velocity = velocity.normalized * 6;

                        bullet.GetComponent<Rigidbody2D>().velocity = velocity;

                        if (i % 2 == 0) ShootSFX();
                    }
                    break;
            }

            if (Random.Range(0, 2) == 0) {
                // left
                if (state == 'c') {
                    User.GoToWaypoint(StageController.Stage.Waypoints.MID_LEFT);
                    state = 'l';
                } else if (state == 'r') {
                    User.GoToWaypoint(StageController.Stage.Waypoints.TOP_CENTER);
                    state = 'c';
                }
            } else {
                // right
                if (state == 'c') {
                    User.GoToWaypoint(StageController.Stage.Waypoints.MID_RIGHT);
                } else if (state == 'l') {
                    User.GoToWaypoint(StageController.Stage.Waypoints.TOP_CENTER);
                }
            }

            yield return new WaitForSeconds(3.0f);
        }
    }

    public override IEnumerator StartSpell() {
        Vector2[] poses = { new Vector2(0, 20), new Vector2(4, 14), new Vector2(-4, 14) };
        string[] names = { "PastelYellow", "PastelRed", "PastelBlue" };

        while (true) {
            bool burst = Random.Range(0, 2) == 0;
            ShootSFX();

            if(burst) {
                int c = Random.Range(0, 3);
                int r = Random.Range(0, 3);

                for (int i = 0; i < 14; i++) {
                    GameObject bullet = ObjectPooler.Spawn(names[c], poses[r], Quaternion.identity);

                    Vector2 velocity = Random.insideUnitCircle * 9;

                    if (velocity.sqrMagnitude < 16) velocity = velocity.normalized * 6;

                    bullet.GetComponent<Rigidbody2D>().velocity = velocity;
                }
            } else {
                StartCoroutine(CircleBullet(names[Random.Range(0, 3)], 14, 8, 1, 1, poses[Random.Range(0, 3)]));
            }
            yield return new WaitForSeconds(0.75f);
        }
    }
}
