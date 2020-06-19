using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletResize : MonoBehaviour
{
    public float ScaleXRate = 0.0f;
    public float ScaleYRate = 0.0f;

    void FixedUpdate() {
        gameObject.transform.localScale += new Vector3(ScaleXRate, ScaleYRate, 0);
    }
}
