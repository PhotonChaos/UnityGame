using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaveOfRevelry : SpellCardBase
{
    private int MoveSkew = 0;

    public override IEnumerator StartCooldown() {
        // Music starts, fake with bad shot until 25 seconds exactly
        User.SetDefenseMod(0.08f);
        Coroutine fakeout = StartCoroutine(FakeoutShoot());
        yield return new WaitForSeconds(25);
        StopCoroutine(fakeout);
        StageController.Stage.KillAllEnemyBullets();
        StageController.AudioManager.PlaySFX("spellstart", 0.5f);
        yield return new WaitForSeconds(1f);

        User.SetDefenseMod(1);

        while(true) {
            for(int i = 0; i < 4; i++) {
                ShootSFX();

                if(i % 2 == 0) {
                    StartCoroutine(CircleBullet("KunaiPurple", 15, Random.Range(9.5f, 11.5f), rotation: Random.Range(0f, 360f)));
                }

                StartCoroutine(CircleBullet("KunaiPink", 20, 8.5f, rotation:9));

                yield return new WaitForSeconds(0.3f);
            }

            // Movement
            int dir = Random.Range(-1, 2);

            if(dir + MoveSkew > 0) {
                User.GoToWaypoint(transform.position + new Vector3(Random.Range(3, 5), 0, 0));
            } else if(dir + MoveSkew < 0) {
                User.GoToWaypoint(transform.position + new Vector3(Random.Range(-4, -2), 0, 0));
            }

            if(transform.position.x <= -6) {
                MoveSkew = 1;
            } else if(transform.position.x >= 6) {
                MoveSkew = -1;
            } else {
                MoveSkew = 0;
            }
        }
    }

    public override IEnumerator StartSpell() {
        int x = 0;
        
        const float target_comp = 23;
        const float b = 1.17f;
        const float start_comp = 12;
        const float comp_speed = 0.09f;

        const float path_threshold = 3f;

        float current_comp;
        float comp_accuracy;

        bool calcp = true;

        while(true) {
            ShootSFX();

            if(calcp) {
                comp_accuracy = start_comp*Mathf.Pow(b, -x*comp_speed);
                if (comp_accuracy < 0.001f) {
                    calcp = false;
                }

                current_comp = target_comp - comp_accuracy;
            } else {
                current_comp = target_comp;
            }

            float path = 7 * Mathf.Sin(current_comp * x * Mathf.Deg2Rad) + 9;

            int layerNum = 1;
            bool flip = false;

            for(int y = -3; y < 24; y++) {
                if (Mathf.Abs(y - path) <= path_threshold) {
                    flip = true;
                    continue;
                }

                GameObject bullet;

                if(Mathf.Abs(y + 1 - path) <= path_threshold || Mathf.Abs(y - 1 - path) <= path_threshold) {
                    // If on edges, use the edge bullet
                    bullet = ObjectPooler.Spawn("LevelBarEnd", new Vector3(15, y, 0), Quaternion.identity);
                } else {
                    bullet = ObjectPooler.Spawn("LevelBarBody", new Vector3(15, y, 0), Quaternion.identity);
                }
                
                if(flip) {
                    bullet.transform.Rotate(0, 0, 180);
                }

                SpriteRenderer rend = bullet.GetComponent<SpriteRenderer>();
                rend.sortingOrder = layerNum++;

                System.Drawing.Color c = System.Drawing.Color.FromArgb((int)(rend.color.r*255), (int)(rend.color.g * 255), (int)(rend.color.b * 255));

                float hue = (c.GetHue() + x*1.5f) % 360 / 360f;
                float sat = c.GetSaturation();
                // float bri = c.GetBrightness();

                rend.color = Color.HSVToRGB(hue, sat, 1);

                bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(-4, 0);
            }

            x++;
            yield return new WaitForSeconds(0.25f);
        }
    }

    private IEnumerator FakeoutShoot() {
        Vector3 p;

        string[] tag = { "KunaiGreen", "KunaiOrange" };

        while(!onSpell) {
            do {
                p = new Vector3(Random.Range(-12f, 12f), Random.Range(0, 20f), 0);
            } while ((StageController.Player.transform.position - p).sqrMagnitude < 81f) ;

            ShootSFX();
            StartCoroutine(CircleBullet(tag[Random.Range(0, tag.Length)], 10, Random.Range(8f, 10f), pos:p));
            yield return new WaitForSeconds(Random.Range(0.5f, 1f));
        }
    }
}
