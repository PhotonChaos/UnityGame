using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrimsonLandslide : SpellCardBase
{
    public override IEnumerator StartCooldown() {
        const float BULLET_SPEED = 19f;
        const float WIDTH_MOD = 0.8f;

        int c = 0;

        while(true) {
            if (c != 0 && c % 5 == 0) yield return new WaitForSeconds(2.0f);

            c++;

            Vector3 dir = StageController.Player.transform.position - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;

            ShootSFX();

            for (int i = 0; i < 7; i++) {
                GameObject row = new GameObject("row_holder");
                row.transform.position = transform.position;
                List<GameObject> rowl = new List<GameObject>(i+1);

                for(int j = 0; j < i; j++) {
                    GameObject bullet = ObjectPooler.Spawn("FireShardRed", transform.position, Quaternion.identity);

                    bullet.transform.Translate(bullet.transform.right * -(i/2f) * WIDTH_MOD + bullet.transform.right * j * WIDTH_MOD);
                    bullet.transform.SetParent(row.transform);
                    rowl.Add(bullet);
                }

                row.transform.Rotate(0, 0, angle);

                for(int j = 0; j < rowl.Count; j++) {
                    rowl[j].GetComponent<Rigidbody2D>().velocity = rowl[j].transform.up * (BULLET_SPEED-i);
                }
            }

            yield return new WaitForSeconds(0.4f);
        }
    }

    public override IEnumerator StartSpell() {
        yield return new WaitForSeconds(2.0f);

        int last_y = -1;

        while (true) {
            int y;
            do {
                y = Random.Range(0, 3);
            } while (y == last_y);

            last_y = y;

            for(int j = 0; j < 30; j++) {
                ShootSFX();

                for (int k = 0; k < 3; k++) {
                    if (k == y) continue;
                    for (int i = -3; i <= 3; i++) {
                        GameObject bullet = ObjectPooler.Spawn("BubbleRed", new Vector3(-14, k*10 + i * 2), Quaternion.identity);
                        bullet.AddComponent<SimpleMovement>().UpdateAcceleration(22, -2);
                    }
                }

                yield return new WaitForSeconds(0.07f);
            }

            yield return new WaitForSeconds(2.0f);
        }
    }
}