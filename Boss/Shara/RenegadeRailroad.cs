using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenegadeRailroad : SpellCardBase
{
    private int MoveSkew = 0;

    public override IEnumerator StartCooldown() {
        // Dense walls, with gaps
        while(true) {
            ShootSFX();
            BulletStar("ButterflyPink", transform.position, 11, rotation: Random.Range(0f, 20f), velocity:Random.Range(5f, 7f));

            // movement
            int dir = Random.Range(-1, 2);

            if (dir + MoveSkew > 0) {
                User.GoToWaypoint(transform.position + new Vector3(Random.Range(3, 5), 0, 0));
            } else if (dir + MoveSkew < 0) {
                User.GoToWaypoint(transform.position + new Vector3(Random.Range(-4, -2), 0, 0));
            }

            if (transform.position.x <= -6) {
                MoveSkew = 1;
            } else if (transform.position.x >= 6) {
                MoveSkew = -1;
            } else {
                MoveSkew = 0;
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    public override IEnumerator StartSpell() {
        const int NUM_SHOTS = 100;
        const float LERP_SCALE = NUM_SHOTS / 8f;
        const float STARTING_ANGLE = 80;
        const float TARGET_ANGLE = 15;
        const float TRACK_VELOCITY = 7;

        while(true) {
            float trackAngle = STARTING_ANGLE;
            // Track walls
            for(int i = 0; i < NUM_SHOTS; i++) {
                trackAngle = Mathf.Lerp(STARTING_ANGLE, TARGET_ANGLE, i / LERP_SCALE);

                GameObject bulletLeft = ObjectPooler.Spawn("ButterflyPink", transform.position, Quaternion.identity);
                GameObject bulletRight = ObjectPooler.Spawn("ButterflyPink", transform.position, Quaternion.identity);

                bulletLeft.transform.Rotate(0, 0, 180 - trackAngle);
                bulletRight.transform.Rotate(0, 0, 180 + trackAngle);

                bulletLeft.GetComponent<Rigidbody2D>().velocity = bulletLeft.transform.up * TRACK_VELOCITY;
                bulletRight.GetComponent<Rigidbody2D>().velocity = bulletRight.transform.up * TRACK_VELOCITY;

                yield return new WaitForSeconds(0.2f);
            }

            yield return new WaitForSeconds(0.001f);
        }
    }

    public IEnumerator TrackBars() {
        yield return null;
    }
}
