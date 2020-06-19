using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHomable : MonoBehaviour
{
    public static List<GameObject> Enemies = new List<GameObject>();
    public static List<Transform> EnemyPositions = new List<Transform>();

    // Start is called before the first frame update
    void Start()
    {
        Enemies.Add(gameObject);
        EnemyPositions.Add(transform);
    }

    private void OnDestroy() {
        Enemies.Remove(gameObject);
        EnemyPositions.Remove(transform);
    }
}
