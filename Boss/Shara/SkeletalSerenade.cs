using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletalSerenade : SpellCardBase
{
    public static List<GameObject> ZephyrBullets = new List<GameObject>();

    private int MoveSkew = 0;

    public override IEnumerator StartCooldown() {
        const int modNum = 4;
        const int numInCircle = modNum*5;
        const int numCircles = 3;
        const float scale = 3f;

        int layer = 2;

        string[] tags = { "BubbleCyan", "BubbleLightBlue", "BubbleBlue" };

        while (true) {
            ShootSFX();

            for (int j = numCircles-1; j >= 0; j--) {
                for (int i = 0; i < numInCircle; i++) {
                    float velocity = Mathf.Abs(i % modNum - modNum / 2f)*3f + 10f - j;

                    GameObject bullet = ObjectPooler.Spawn(tags[j], transform.position, Quaternion.identity);

                    bullet.transform.localScale = new Vector3(scale, scale, 1);
                    bullet.transform.Rotate(new Vector3(0, 0, (360f / numInCircle * i) - 90));
                    bullet.GetComponent<SpriteRenderer>().sortingOrder = layer++;
                    bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.up * velocity;
                }
            }

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

            yield return new WaitForSeconds(0.8f);
        }
    }

    public override IEnumerator StartSpell() {
        yield return new WaitForSeconds(1f);

        const int numInCircle = 8;

        PlayerBounds b = StageController.Player.GetComponent<PlayerController>().Bounds;

        Vector2 posBL = new Vector2(b.xMin, b.yMin);
        Vector2 posTR = new Vector2(b.xMax, b.yMax);

        User.GoToWaypoint(Vector2.Lerp(posBL, posTR, 0.5f));

        string[] tags = { "BubbleLightBlue", "BubbleOrange", 
                            "BubbleRed", "BubblePink", 
                            "BubbleWhite", "BubbleGray", 
                            "BubbleRed", "BubbleYellow", 
                            "BubblePurple", "BubbleOrange", 
                            "BubbleBlue", "BubbleOrange",
                            "Star", "BubbleGray"};
        int[] angleOffsets = { -50, -50,
                                -12, -95, 
                                0, -45, 
                                -41, -41, 
                                -35, -19, 
                                37, -62,
                                0, 0 };

        int skipCount = 14;
         
        for (int i = 0; i < System.Enum.GetNames(typeof(SkeleBulletType)).Length - 3; i++) {
            if(skipCount > 0) {
                skipCount--;
                continue;
            }

            if (i >= tags.Length) break;

            GameObject bullet = ObjectPooler.Spawn(tags[i], (i % 2 == 0) ? posBL : posTR, Quaternion.identity);

            GravitateMovement gm = bullet.AddComponent<GravitateMovement>();

            gm.card = this;

            if(i % 2 == 1) {
                gm.AngleOffset = 180;
                gm.EndCircleDelay = 0.5f;
                gm.EndCircleRotation = 360f / numInCircle / 2;
            } 

            gm.AngleOffset += angleOffsets[i];
            gm.BulletType = (SkeleBulletType)i;

            if (i % 2 == 1) yield return new WaitForSeconds(20);
        }

        Vector2 posBC = Vector2.Lerp(posBL, new Vector2(b.xMax, b.yMin), 0.5f);
        Vector2 posTL = new Vector2(b.xMin, b.yMax);

        Vector2 dest = new Vector2((posBC.x + posTL.x + posTR.x) / 3f, (posBC.y + posTL.y + posTR.y) / 3f);

        User.GoToWaypoint(dest);

        GameObject chronos = ObjectPooler.Spawn("BubbleLightBlue", posTL, Quaternion.identity);
        GameObject cider = ObjectPooler.Spawn("BubblePink", posTR, Quaternion.identity);
        GameObject zephyr = ObjectPooler.Spawn("BubbleGreen", posBC, Quaternion.identity);

        GravitateMovement gm_chronos = chronos.AddComponent<GravitateMovement>();
        GravitateMovement gm_cider = cider.AddComponent<GravitateMovement>();
        GravitateMovement gm_zephyr = zephyr.AddComponent<GravitateMovement>();

        gm_chronos.card = this;
        gm_cider.card = this;
        gm_zephyr.card = this;

        gm_chronos.OverrideDest = true;
        gm_cider.OverrideDest = true;
        gm_zephyr.OverrideDest = true;

        gm_chronos.dest = dest;
        gm_cider.dest = dest;
        gm_zephyr.dest = dest;

        gm_zephyr.AngleOffset = 180;
        gm_cider.AngleOffset = 90;

        gm_cider.EndCircleDelay = 0.5f;
        gm_zephyr.EndCircleDelay = 1f;

        gm_chronos.BulletType = SkeleBulletType.CHRONOS;
        gm_cider.BulletType = SkeleBulletType.CIDER;
        gm_zephyr.BulletType = SkeleBulletType.ZEPHYR;

        // TODO: Custom Circles
    }

    public void ShootCirclesStart(float EndCircleDelay, string BulletTag, float EndCircleRotation, Vector2 pos, Color? rainbowcol = null, SkeleBulletType type = SkeleBulletType.SANS) {
        StartCoroutine(ShootCircles(EndCircleDelay, BulletTag, EndCircleRotation, pos, rainbowcol, type));
    }

    public IEnumerator ShootCircles(float EndCircleDelay, string BulletTag, float EndCircleRotation, Vector2 pos, Color? rainbowcol = null, SkeleBulletType type = SkeleBulletType.SANS) {
        yield return new WaitForSeconds(EndCircleDelay);

        StageController.AudioManager.PlaySFX("shootbullet", 0.3f);

        const float velocity = 5.5f;
        const int numInCircle = 8;

        for (int i = 0; i < numInCircle; i++) {
            GameObject bullet = ObjectPooler.Spawn(BulletTag, pos, Quaternion.identity);

            if(BulletTag == "Kunai" || BulletTag == "Star") {
                switch(type) {
                    case SkeleBulletType.INK:
                        float hue = 360f / numInCircle * i / 360f;
                        rainbowcol = Color.HSVToRGB(hue, 0.7f, 1);
                        break;

                    case SkeleBulletType.ERROR:
                        int r = Random.Range(0, 7);

                        if (r == 0) rainbowcol = Color.blue;
                        else if (r == 1) rainbowcol = Color.yellow;
                        else if (r == 2) rainbowcol = Color.red;
                        else rainbowcol = GravitateMovement.GetCol(74, 74, 74);
                        break;

                    case SkeleBulletType.DREAM:
                        rainbowcol = GravitateMovement.GetCol(255, 252, 186);
                        break;
                }

                bullet.GetComponent<SpriteRenderer>().color = rainbowcol.Value;
            }

            bullet.transform.Rotate(new Vector3(0, 0, (360 / numInCircle * i) - 90 + EndCircleRotation));
            bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.up * velocity;

            if(type == SkeleBulletType.ZEPHYR) {
                ZephyrBullets.Add(bullet);
            }
        }

        if (type == SkeleBulletType.ZEPHYR) StartCoroutine(ZephyrCircle());
    }

    public IEnumerator ZephyrCircle() {
        yield return new WaitForSeconds(1);

        const int numInCircle = 8;

        for (int i = 0; i < ZephyrBullets.Count; i++) {
            Vector3 dir = StageController.Player.transform.position - ZephyrBullets[i].transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;

            GameObject b2 = ObjectPooler.Spawn("KunaiYellow", ZephyrBullets[i].transform.position, Quaternion.identity);
            b2.transform.Rotate(0, 0, angle);
            b2.GetComponent<Rigidbody2D>().velocity = b2.transform.up * 9f;

            ObjectPooler.Disable(ZephyrBullets[i]);
        }

        ZephyrBullets.Clear();
    }
}
