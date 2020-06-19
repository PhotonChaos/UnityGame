using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TickTock : SpellCardBase
{
    private const float FROZEN_TIME_SCALE = 0.001f;

    public override IEnumerator StartCooldown() {
        while (User.IsMoving()) yield return new WaitForSeconds(0.0001f);
        StageController.AudioManager.PlaySFX("chargeup");
        yield return new WaitForSeconds(2.0f);

        string[] tags = { "BubbleLightBlue", "BubblePink", "BubblePurple" };

        const int NUM_CIRCLES = 8;
        const int NUM_IN_CIRCLE = 12;

        while(true) {
            ShootSFX();
            for (int i = 0; i < NUM_CIRCLES; i++) {
                float rot = 180f / NUM_IN_CIRCLE * i;
                StartCoroutine(CircleBullet(tags[Mathf.RoundToInt(i/(float)NUM_CIRCLES * (tags.Length-1))], NUM_IN_CIRCLE, 23 - 2f*i, rotation: rot));
            }

            yield return new WaitForSeconds(0.5f);

            switch(Random.Range(0, 3)) {
                case 0:
                    User.GoToWaypoint(StageController.Stage.Waypoints.TOP_LEFT);
                    break;

                case 1:
                    User.GoToWaypoint(StageController.Stage.Waypoints.TOP_RIGHT);
                    break;

                case 2:
                    User.GoToWaypoint(StageController.Stage.Waypoints.TOP_CENTER);
                    break;
            }

            yield return new WaitForSeconds(1.0f);
        }
    }

    public override IEnumerator StartSpell() {
        const int NUM_IN_RING = 14;
        const int NUM_RINGS = 4;
        const float BULLET_ACCELERATION = 0.8f;

        while(true) {
            ShootSFX();
            
            for(int i = 0; i < 6; i++) {
                float rot = (i % 2 == 0) ? 0 : 11.25f;
                StartCoroutine(CircleBullet("ButterflyPink", 14, 22 - i, rotation:rot));
            }

            yield return new WaitForSeconds(0.5f);

            // "Stop" time
            Time.timeScale = FROZEN_TIME_SCALE;

            List<GameObject> bulletsTime = new List<GameObject>();
            float lastRot = 0f;

            for (int i = 0; i < NUM_RINGS; i++) {
                ShootSFX();

                GameObject ring = new GameObject("KuaniRing");
                ring.transform.position = StageController.Player.transform.position;


                foreach (Vector2 pos in Bomb.GetShapePoints(NUM_IN_RING, i*1.3f + 6.5f, StageController.Player.transform.position.x, StageController.Player.transform.position.y)) {
                    GameObject bullet = ObjectPooler.Spawn("KunaiPink", pos, Quaternion.identity);

                    Vector3 dir = StageController.Player.transform.position - bullet.transform.position;
                    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;
                    
                        Destroy(bullet.GetComponent<SimpleMovement>());
                    bullet.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                    bullet.transform.Rotate(0, 0, angle);
                    bullet.transform.SetParent(ring.transform);
                    bulletsTime.Add(bullet);
                }

                float r = 360*2f / NUM_IN_RING / NUM_IN_RING;
                ring.transform.Rotate(0, 0, lastRot + r);
                lastRot += r;

                yield return new WaitForSeconds(0.15f * FROZEN_TIME_SCALE);
            }

            Time.timeScale = 1;

            yield return new WaitForSeconds(0.2f);

            foreach (GameObject b in bulletsTime) {
                if (b.GetComponent<SimpleMovement>() != null) {
                    b.GetComponent<SimpleMovement>().UpdateAcceleration(b.transform.up.x * BULLET_ACCELERATION, b.transform.up.y * BULLET_ACCELERATION);
                } else {
                    b.AddComponent<SimpleMovement>().UpdateAcceleration(b.transform.up.x * BULLET_ACCELERATION, b.transform.up.y * BULLET_ACCELERATION);
                }
            }

            switch (Random.Range(0, 4)) {
                case 0:
                    User.GoToWaypoint(StageController.Stage.Waypoints.TOP_LEFT);
                    break;

                case 1:
                    User.GoToWaypoint(StageController.Stage.Waypoints.TOP_RIGHT);
                    break;

                case 2:
                    User.GoToWaypoint(StageController.Stage.Waypoints.TOP_CENTER);
                    break;
            }

            yield return new WaitForSeconds(3.0f);
        }
    }

    private void OnDestroy() {
        Time.timeScale = 1;
    }
}
