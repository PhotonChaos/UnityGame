using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeShot : MonoBehaviour
{
    [HideInInspector]
    public bool onTop;

    private const float TIME_INTERVAL = 0.1f;
    private const float BULLET_SPEED = 10f;
    private float t = TIME_INTERVAL;

    // Update is called once per frame
    void FixedUpdate() {
        t -= Time.deltaTime;

        if (Random.Range(0, 13) == 2) t -= 0.01f;

        if(t <= 0) {
            t = TIME_INTERVAL;

            if (Random.Range(0, 2) == 1) {
                GameObject bullet = ObjectPooler.Spawn("EnergyBall", transform.position, Quaternion.identity);

                if (onTop) {
                    bullet.transform.Rotate(0, 0, 45);
                } else {
                    bullet.transform.Rotate(0, 0, -125);
                }

                bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.up.normalized * BULLET_SPEED;
            }
        }
    }
}
