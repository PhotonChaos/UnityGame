using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public bool UseEfficientHitbox;

    public PlayerName Type;
    public GameObject ArcyBomb;
    public GameObject CarbonBomb;
    public GameObject CarbonAlternate;
    public Sprite Star2;
    public Sprite Star3;

    private List<GameObject> BombFX;

    private void Start() {
        // Bomb was spawned, so create the objects

        BombFX = new List<GameObject>();

        if(Type == PlayerName.ARCY) {
            // ARCY BOMB - Inferno Overdrive
            InfernoOverdrive();
        } else {
            // Carbon Bomb - Stardust Reverie
            if(UseEfficientHitbox) {
                StardustReverieAlt();
            } else {
                StardustReverie();
            }
        }

    }

    private void Update() {
        if(transform.childCount == 0) {
            Destroy(gameObject);
        }
    }

    private void StardustReverie() {
        const float power = 50.0f;

        List<Vector2> big = GetShapePoints(5, 1, transform.position.x, transform.position.z);

        int c = 0;

        // BIG STARS
        foreach(Vector2 pos in big) {
            GameObject star = Instantiate(CarbonBomb, new Vector3(pos.x, 0, pos.y), Quaternion.identity);
            star.transform.Rotate(new Vector3(90, 0, 0));
            star.GetComponent<Rigidbody>().AddTorque(0, Random.value * 30, 0);
            star.GetComponent<Rigidbody>().velocity = (transform.position - new Vector3(pos.x, 0, pos.y)) * 5f;

            print($"A: {new Vector3(pos.x, 0, pos.y).ToString()}\nB: {transform.position.ToString()}");

            star.transform.localScale = new Vector3(4f, 4f, 1f);
            
            star.GetComponent<BombProjectile>().Power = power;

            BombFX.Add(star);

            c++; 
        }

        // Medium stars
        for(int i = 0; i < 40; i++) {
            GameObject star = Instantiate(CarbonBomb, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);

            star.transform.Rotate(new Vector3(90, 0, 0));
            star.GetComponent<Rigidbody>().AddTorque(0, Random.value * 90, 0);

            Vector2 v = Random.insideUnitCircle;

            if (v.x < 0.2f && v.x > -0.2f && v.y < 0.2f && v.y > -0.2f) v = new Vector2(Random.value > 0 ? -0.6f : 0.6f, Random.value > 0 ? -0.6f : 0.6f);

            star.GetComponent<Rigidbody>().velocity = new Vector3(v.x, 0, v.y) * 10;

            star.GetComponent<SpriteRenderer>().sprite = Star2;
            star.transform.localScale = new Vector3(2, 2, 0);
            star.GetComponent<SphereCollider>().radius = 1;

            BombFX.Add(star);
        }

        // Small stars
        for (int i = 0; i < 80; i++) {
            GameObject star = Instantiate(CarbonBomb, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);

            star.transform.Rotate(new Vector3(90, 0, 0));
            star.GetComponent<Rigidbody>().AddTorque(0, Random.value * 90, 0);

            Vector2 v = Random.insideUnitCircle;

            if (v.x < 0.2f && v.x > -0.2f && v.y < 0.2f && v.y > -0.2f) v = new Vector2(Random.value > 0 ? -0.6f : 0.6f, Random.value > 0 ? -0.6f : 0.6f);

            star.GetComponent<Rigidbody>().velocity = new Vector3(v.x, 0, v.y) * 20;
            star.GetComponent<SphereCollider>().radius = 0.5f;

            star.GetComponent<SpriteRenderer>().sprite = Star3;
            star.transform.localScale = new Vector3(1, 1, 0);

            BombFX.Add(star);
        }

        foreach(GameObject star in BombFX) {
            star.transform.parent = transform;
        }
    }

    private void StardustReverieAlt() {
        const float power = 50.0f;

        List<Vector2> big = GetShapePoints(5, 1, transform.position.x, transform.position.y);

        int c = 0;

        // BIG STARS
        foreach (Vector2 pos in big) {
            GameObject star = Instantiate(CarbonAlternate, new Vector3(pos.x, pos.y, 0), Quaternion.identity);
            star.GetComponent<Rigidbody2D>().AddTorque((Random.value >= 0 ? 1:-1) * 30);
            Vector3 v3 = transform.position - new Vector3(pos.x, pos.y, 0);

            Vector2 v = new Vector2(v3.x, v3.y);

            star.GetComponent<Rigidbody2D>().velocity = v * 5f;

            star.transform.localScale = new Vector3(4f, 4f, 1f);

            star.GetComponent<BombProjectile>().Power = power;

            BombFX.Add(star);

            c++;
        }

        
        // Medium stars
        for (int i = 0; i < 40; i++) {
            GameObject star = Instantiate(CarbonAlternate, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);

            star.GetComponent<Rigidbody2D>().AddTorque(Random.value * 90);

            Vector2 v = Random.insideUnitCircle;

            if (v.x < 0.2f && v.x > -0.2f && v.y < 0.2f && v.y > -0.2f) v = new Vector2(Random.value > 0 ? -0.6f : 0.6f, Random.value > 0 ? -0.6f : 0.6f);

            star.GetComponent<Rigidbody2D>().velocity = new Vector2(v.x, v.y) * 10;

            star.GetComponent<SpriteRenderer>().sprite = Star2;
            star.transform.localScale = new Vector3(2, 2, 0);
            star.GetComponent<CircleCollider2D>().radius = 1;

            BombFX.Add(star);
        }

        // Small stars
        for (int i = 0; i < 80; i++) {
            GameObject star = Instantiate(CarbonAlternate, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);

            star.GetComponent<Rigidbody2D>().AddTorque(Random.value * 90);

            Vector2 v = Random.insideUnitCircle;

            if (v.x < 0.2f && v.x > -0.2f && v.y < 0.2f && v.y > -0.2f) v = new Vector2(Random.value > 0 ? -0.6f : 0.6f, Random.value > 0 ? -0.6f : 0.6f);

            star.GetComponent<Rigidbody2D>().velocity = new Vector2(v.x, v.y) * 20;
            star.GetComponent<CircleCollider2D>().radius = 0.5f;

            star.GetComponent<SpriteRenderer>().sprite = Star3;
            star.transform.localScale = new Vector3(1, 1, 0);

            BombFX.Add(star);
        }

        

        foreach (GameObject star in BombFX) {
            star.transform.parent = transform;
        }
    }

    private void InfernoOverdrive() {
        transform.parent = StageController.Player.transform;

        GameObject flame = Instantiate(ArcyBomb, transform.position, transform.rotation);
        flame.transform.parent = transform.parent.transform;
        flame.transform.localPosition = new Vector3(0, 30, 0);
    }

    /// <summary>
    /// Gets the vertices of a regular polygon with n sides of radius r, with a center at (x, y)
    /// </summary>
    /// <param name="n">The number of sides the polygon has</param>
    /// <param name="r">The radius of the polygon</param>
    /// <param name="x">The X co-ordinate of the center</param>
    /// <param name="y">The Y co-ordinate of the center</param>
    /// <returns></returns>
    public static List<Vector2> GetShapePoints(int n, float r, float x, float y) {
        List<Vector2> ret = new List<Vector2>();

        for (int i = 0; i < n; i++) {
            ret.Add(new Vector2(x + r * Mathf.Cos(2 * Mathf.PI * i / n), y + r * Mathf.Sin(2 * Mathf.PI * i / n)));
        }

        return ret; 
    }
}