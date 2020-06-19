using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningRed : MonoBehaviour
{
    private float t = 0.5f;
    private bool up = true;
    private Material mat;

    private void Start() {
        mat = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if (t > 0) t -= Time.deltaTime;
        if (t <= 0) {
            if (!up) ObjectPooler.Disable(gameObject);

            t = 0.5f;
            up = false;
        }

        if(up) {
            mat.SetFloat("_MAT_ALPHA", 0.5f - t);
        } else {
            mat.SetFloat("_MAT_ALPHA", t);
        }
    }
}
