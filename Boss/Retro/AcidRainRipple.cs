using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidRainRipple : MonoBehaviour
{
    private void FixedUpdate() {
        if(transform.position.y < -3) {
            StageController.AudioManager.PlaySFX("shootbullet", 0.4f);
            StartCoroutine(SpellCardBase.CircleBulletStatic("BubbleGreen", 7, 11f, new Vector3(transform.position.x, -4, 0)));
            Destroy(gameObject);
        } else if(transform.position.x > 14.7f) {
            StageController.AudioManager.PlaySFX("shootbullet", 0.4f);
            StartCoroutine(SpellCardBase.CircleBulletStatic("BubbleGreen", 9, 11f, new Vector3(14.6f, transform.position.y, 0)));
            Destroy(gameObject);
        } else if(transform.position.x < -14.6f) {
            StageController.AudioManager.PlaySFX("shootbullet", 0.4f);
            StartCoroutine(SpellCardBase.CircleBulletStatic("BubbleGreen", 8, 11f, new Vector3(-14.6f, transform.position.y, 0)));
            Destroy(gameObject);
        }
    }
}
