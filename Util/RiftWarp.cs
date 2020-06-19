using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiftWarp : MonoBehaviour
{
    public static List<GameObject> Destinations = new List<GameObject>();
    public static bool SelfDestruct = false;

    [HideInInspector]
    public GameObject BossUser = null;
    
    public bool IsSource = false;

    private void Start() {
        if(!IsSource) {
            Destinations.Add(gameObject);
        }
    }

    private void Update() {
        if (SelfDestruct || BossUser == null) Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.CompareTag("EnemyBullet")) {
            PooledObject pool = collision.gameObject.GetComponent<PooledObject>();

            if(pool != null) {
                if (pool.Tag == "CircleBallGreen") return;
            }

            collision.gameObject.transform.position = Destinations[Random.Range(0, Destinations.Count)].transform.position;

            Vector3 dir = StageController.Player.transform.position - collision.transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            collision.transform.rotation = Quaternion.AngleAxis(angle - 90 + Random.Range(-90f, 90f), Vector3.forward);

            collision.gameObject.GetComponent<Rigidbody2D>().velocity = collision.transform.up * Random.Range(7f, 9.5f);

            YoYoShoot yoyo = collision.gameObject.GetComponent<YoYoShoot>();

            if(yoyo != null) {
                yoyo.SpawnChain = true;
            }
        }
    }
}
