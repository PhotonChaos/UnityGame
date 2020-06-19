using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbsoluteZero : SpellCardBase
{
    private int bulletSpawns = 0;

    private List<GameObject> Bullets = new List<GameObject>();

    public override IEnumerator StartCooldown() {
        int i = 0;

        while(true) {
            ShootSFX();
            StartCoroutine(CircleBullet("SteelBall", 14, 8f, rotation: Random.Range(-45f, 45f)));
            yield return new WaitForSeconds(0.3f);

            if (i++ % 13 == 0) yield return new WaitForSeconds(2.0f);
        }
    }

    public override IEnumerator StartSpell() {
        const int maxlvl = 4;
        List<Vector2> pos = new List<Vector2>();

        for (int level = 1; level <= maxlvl; level++) {
            for (int x = -13; x <= 13; x++) {
                for (int y = -2; y <= 22; y++) {
                    if (x % 2 == 0 || y % 2 == 0) continue;

                    if (Random.Range(0, 4) < level) {
                        pos.Add(new Vector2(x, y));
                    }
                }
            }

            int q = 0;
            float xOff = Random.Range(-0.5f, 0.5f);
            float yOff = Random.Range(-0.5f, 0.5f);

            foreach (Vector2 p in pos) {
                if (q++ % 50 == 0) ShootSFX();
                StartCoroutine(WarnSpawn(new Vector2(p.x + xOff, p.y + yOff)));
                yield return new WaitForSeconds(0.001f);
            }


            while (bulletSpawns > 0) {
                yield return new WaitForSeconds(0.0001f);
            }

            for (int i = 0; i < Bullets.Count; i++) {
                if(i % 2 == 0) {
                    Bullets[i].AddComponent<SimpleMovement>().UpdateAcceleration(3, 0);
                } else {
                    Bullets[i].AddComponent<SimpleMovement>().UpdateAcceleration(0, 3);
                }
            }

            Bullets.Clear();

            yield return new WaitForSeconds(8f);
        }
    }

    private IEnumerator WarnSpawn(Vector2 pos) {
        bulletSpawns++;
        GameObject g = ObjectPooler.Spawn("WarningRed", pos, Quaternion.identity);
        yield return new WaitForSeconds(1.0f);
        Bullets.Add(ObjectPooler.Spawn("IceBlock", pos, Quaternion.identity));
        bulletSpawns--;
    }
}
