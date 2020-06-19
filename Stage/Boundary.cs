using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boundary : MonoBehaviour
{
    public static Dictionary<GameObject, int> WarpCount = new Dictionary<GameObject, int>();

    public static bool Reflect = false;
    public static bool Warp = false;
    public static int WarpThreshold = 1;

    public bool KillWall = false;

    public static void ResetBoundary() {
        Reflect = false;
        Warp = false;
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.tag == "Enemy") {
            other.gameObject.GetComponent<Shootable>().Die(false);
            return;
        }

        if(other.CompareTag("EnemyBullet")) {
            if(KillWall) {
                ObjectPooler.Disable(other.gameObject);
                return;
            }

            if(Reflect) {

            } else if(Warp) {
                // Increment the warp count
                if (!WarpCount.ContainsKey(other.gameObject)) WarpCount.Add(other.gameObject, 1);
                else WarpCount[other.gameObject]++;

                // If the warp count is beyondthe threshold destroy bullet
                if(WarpCount[other.gameObject] > WarpThreshold) {
                    ObjectPooler.Disable(other.gameObject);
                    return;
                }

                other.transform.SetPositionAndRotation(new Vector3(-other.transform.position.x, other.transform.position.y, other.transform.position.z), other.transform.rotation);

                float top_limit = gameObject.transform.position.y + gameObject.transform.localScale.y / 2;
                float bottom_limit = gameObject.transform.position.y - gameObject.transform.localScale.y / 2;

                if (other.transform.position.y > top_limit || other.transform.position.y < bottom_limit) {
                    // Destroy the bullet if it goes out of bounds vertically
                    ObjectPooler.Disable(other.gameObject);
                }
            } else {
                ObjectPooler.Disable(other.gameObject);
            }
            return;
        }

        Destroy(other.gameObject);
    }
}
