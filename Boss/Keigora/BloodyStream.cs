using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodyStream : SpellCardBase
{
    public override IEnumerator StartCooldown() {
        gameObject.GetComponent<Rigidbody2D>().angularVelocity = 300.0f;

        const float BULLET_SPEED = 19f;
        const float WIDTH_MOD = 0.5f;

        while (true) {
            ShootSFX();

            for (int i = 0; i < 5; i++) {
                GameObject row = new GameObject("row_holder");
                row.transform.position = transform.position;
                List<GameObject> rowl = new List<GameObject>(i + 1);

                for (int j = 0; j < i; j++) {
                    GameObject bullet = ObjectPooler.Spawn("FireShardRed", transform.position, Quaternion.identity);

                    bullet.transform.Translate(bullet.transform.right * -(i / 2f) * WIDTH_MOD + bullet.transform.right * j * WIDTH_MOD);
                    bullet.transform.SetParent(row.transform);
                    rowl.Add(bullet);
                }

                row.transform.Rotate(0, 0, transform.rotation.eulerAngles.z);

                for (int j = 0; j < rowl.Count; j++) {
                    rowl[j].GetComponent<Rigidbody2D>().velocity = rowl[j].transform.up * (BULLET_SPEED - i);
                }
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    public override IEnumerator StartSpell() {
        const int WIDTH = 5;

        int c = 0;
        GameObject bullet;

        while (true) { 
            for(int i = 0; i < WIDTH; i++) {
                if (c % 5 == 1 && (i == 0 || i == WIDTH-1)) {
                    bullet = ObjectPooler.Spawn("BubbleEye", new Vector3(15 + (-WIDTH + i * 2f), 24, 0), Quaternion.identity);
                    bullet.GetComponent<EyeShot>().onTop = i == 0;
                } else {
                    bullet = ObjectPooler.Spawn("BubbleRed", new Vector3(15 + (-WIDTH + i * 2f), 24, 0), Quaternion.identity);
                }

                bullet.AddComponent<SimpleMovement>().UpdateAcceleration(-6, -6);
            }

            c++;
            yield return new WaitForSeconds(0.1f);
        }
    }
}
