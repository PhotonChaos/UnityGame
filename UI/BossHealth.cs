using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealth : MonoBehaviour
{
    [HideInInspector]
    public GameObject Boss;

    private float startingWidth;
    private float startingX;

    private void Start() {
        startingWidth = transform.localScale.x;
        startingX = transform.position.x;
        print($"{startingX}");
    }

    // Update is called once per frame
    void Update()
    {
        if (Boss == null) {
            Destroy(gameObject);
            return;
        }

        float perc = Boss.GetComponent<Boss>().Health / Boss.GetComponent<Boss>().StartingHealth;
        
        transform.localScale = new Vector3(startingWidth * perc, transform.localScale.y, transform.localScale.z);
        transform.position = new Vector3(startingX * perc, transform.position.y, transform.position.z);
    }
}
