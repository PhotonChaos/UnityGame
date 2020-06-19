using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidRain : SpellCardBase
{
    private RetroBullets rbullets;

    public override IEnumerator StartCooldown() {
        const int shots = 3;
        const float shotang = 80;
        const float spd = 15f;

        while(true) {
            Vector3 dir = StageController.Player.transform.position - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;

            for (int i = 0; i < shots; i++) {
                ShootSFX();
                Shotgun("BubbleGreen", 5, 30, spd, angle - shotang / 2f + i * (shotang / (shots-1)));
                Shotgun("BubbleCyan", 4, 25, spd - 2, angle - shotang / 2f + i * (shotang / (shots - 1)));
                Shotgun("BubbleLightBlue", 3, 20, spd - 3, angle - shotang / 2f + i * (shotang / (shots - 1)));
                Shotgun("FireShardBlue", 4, 10, spd - 4, angle - shotang / 2f + i * (shotang / (shots - 1)));
                Shotgun("FireShardBlue", 3, 7.5f, spd - 5, angle - shotang / 2f + i * (shotang / (shots - 1)));
                Shotgun("FireShardBlue", 2, 5, spd - 6, angle - shotang / 2f + i * (shotang / (shots - 1)));
                yield return new WaitForSeconds(0.25f);
            }

            int move = Random.Range(0, 9);

            switch(move) {
                case 0:
                    User.GoToWaypoint(StageController.Stage.Waypoints.TOP_LEFT);
                    break;

                case 1:
                    User.GoToWaypoint(StageController.Stage.Waypoints.TOP_CENTER);
                    break;

                case 2:
                    User.GoToWaypoint(StageController.Stage.Waypoints.TOP_RIGHT);
                    break;
            }

            while (User.IsMoving()) yield return new WaitForSeconds(0.0001f);
            yield return new WaitForSeconds(1.0f);
        }
    }

    public override IEnumerator StartSpell() {
        rbullets = User.GetComponent<RetroBullets>();

        while(true) {
            int fireballCount = 0;
            ShootSFX();
            for(int i = 0; i < 7; i++) {
                GameObject obj;
                
                if(Random.Range(0, 17) == 4 && fireballCount <= 3) {
                    fireballCount++;
                    obj = Instantiate(rbullets.GreenFireball, transform.position, Quaternion.identity);
                    obj.AddComponent<AcidRainRipple>();
                } else {
                    obj = ObjectPooler.Spawn("ShardGreen", transform.position, Quaternion.identity);
                }

                obj.transform.Rotate(0, 0, Random.Range(-40f, 40f));
                
                Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
                rb.gravityScale = 0.5f;
                rb.velocity = obj.transform.up * Random.Range(10.8f, 11.8f);
            }

            yield return new WaitForSeconds(0.32f);
        }
    }
}
