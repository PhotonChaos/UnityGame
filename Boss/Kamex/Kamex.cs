using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kamex : MonoBehaviour
{
    public GameObject IceCrystal;
    public GameObject MetalShard;

    private Boss kamex;

    private Coroutine spiral;

    void Start() {
        kamex = GetComponent<Boss>();  // for ease of use
        // StartCoroutine(BulletCycle());  // now start shooting
    }

    IEnumerator BulletCycle() {
        // DO SHOT PATTERNS HERE

        spiral = StartCoroutine(SpiralShard(1000, 0.2f, 0.5f));

        yield return null;
    }

    IEnumerator SpiralShard(int count, float rad, float delay) {
        // DO SHOT PATTERNS HERE
        //for(int i = 0; i < count; i++) {

        //}

        yield return null;
    }

    public void Interrupt() {
        StopCoroutine(spiral);
    }
}
