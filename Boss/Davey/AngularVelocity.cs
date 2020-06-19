using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngularVelocity : SpellCardBase
{
    public override IEnumerator StartCooldown() {
        while(true) {
            YoYoShoot.ChainShotRate = 2f;
            StartCoroutine(CircleBullet("BubbleYoYo", 10, 10, 1, 1));
            yield return new WaitForSeconds(4.0f);
        }
    }

    public override IEnumerator StartSpell() {
        YoYoShoot.BulletsTargetPlayer = false;
        YoYoShoot.ChainShotRate = 8;
        YoYoShoot.RingShotRate = 1.6f;

        StartCoroutine(CircleBullets());

        while(true) {
            for(int i = 0; i < 3; i++) {
                Vector2 dir = (StageController.Player.transform.position - transform.position).normalized;
                GameObject bullet = ObjectPooler.Spawn("BubbleYoYo", transform.position, Quaternion.identity);

                BulletResize br = bullet.AddComponent<BulletResize>();
                br.ScaleXRate = 0.06f;
                br.ScaleYRate = 0.06f;

                br.GetComponent<Rigidbody2D>().velocity = dir * 12;

                yield return new WaitForSeconds(0.4f);
            }

            yield return new WaitForSeconds(1.6f);
        }
    }

    private IEnumerator CircleBullets() {
        while(cardAlive) {
            float rot = Random.Range(0.0f, 90.0f);
            StartCoroutine(CircleBullet("BubbleYoYoShoot", 7, 10, rotation: rot));
            yield return new WaitForSeconds(3.0f);
        }

    }
}
