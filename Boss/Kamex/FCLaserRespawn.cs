using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FCLaserRespawn : MonoBehaviour
{
    [HideInInspector]
    public FlashCannon fc;

    private float maxScale;
    private float startGrow = 2.0f;
    private float growTime;
    private CapsuleCollider2D cld;

    private bool shrinking = false;

    private void Start() {
        maxScale = transform.localScale.x;
        cld = GetComponent<CapsuleCollider2D>();
        cld.enabled = false;

        transform.localScale = new Vector3(0, transform.localScale.y, transform.localScale.z);
    }

    private void Update() {
        if (growTime > 0) growTime -= Time.deltaTime;
        else if (growTime < 0) growTime = 0;

        if(!shrinking) {
            // Only enable the collider once the player has had time to react
            if (!cld.enabled && growTime / startGrow > 0.5f) {
                cld.enabled = true;
            }

            Vector3 sc = transform.localScale;
            sc.x = Mathf.Lerp(0, maxScale, 1 - growTime / startGrow);
            transform.localScale = sc;
        } else {
            Vector3 sc = transform.localScale;
            sc.x = Mathf.Lerp(maxScale, 0, 1 - growTime / startGrow);
            transform.localScale = sc;
        }
    }

    public void Grow() {
        shrinking = false;
        growTime = startGrow;
    }

    public void Shrink() {
        shrinking = true;
        growTime = startGrow;
    }
}
