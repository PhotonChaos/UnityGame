using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyShot : MonoBehaviour
{
    public enum ShotType {
        DIRECT,
        RANDOM,
        TARGET,
        CIRCLE,
        SPIRAL
    }

    public string BulletID;
    public Color BulletHue;
    public ShotType Shot;

    [Min(0)]
    public float Frequency;
    public float Velocity;

    [Min(-1)]
    public int Count;

    public float MinY;

    #region Direct
    public bool AimAtPoint;
    public Vector2 AimPoint;

    public float DirectOffset;  // offset in degrees

    #endregion
    #region Random
    public float GravScale;
    #endregion
    #region Target
    public bool Left;
    public bool Direct;
    public bool Right;

    [Range(0, 180)]
    public float LeftOffset;

    [Range(0, 180)]
    public float RightOffset;
    #endregion
    #region Circle

    public int NumInCircle;
    public int CircleCount;
    public float WaveDelay;

    private int waveNum = 0;

    #endregion

    [HideInInspector]
    public string args = "";

    // Start is called before the first frame update
    void Start() {
        HandleArgs(args);
        StartCoroutine(Shoot());
    }

    private IEnumerator Shoot() {
        float time = 1 / Mathf.Max(Frequency, 0.01f);

        while (true) {
            switch (Shot) {
                case ShotType.DIRECT:
                    GameObject bullet = ObjectPooler.Spawn(BulletID, transform.position, Quaternion.identity);

                    if (bullet == null) {
                        yield return new WaitForSeconds(0.001f);
                        continue;
                    }

                    bullet.transform.Rotate(new Vector3(0, 0, 180 + DirectOffset));
                    Renderer rend = bullet.GetComponent<Renderer>();

                    bullet.GetComponent<Rigidbody2D>().velocity = rend.transform.up * 10;
                    break;

                case ShotType.TARGET:
                    if (Direct) {
                        bullet = ObjectPooler.Spawn(BulletID, transform.position, Quaternion.identity);

                        if (bullet == null) {
                            yield return new WaitForSeconds(0.001f);
                            continue;
                        }

                        // Start of copy-paste
                        Vector3 dir = StageController.Player.transform.position - transform.position;
                        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                        bullet.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
                        // End of Copy-Paste

                        bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.up * Velocity;
                    }

                    if (Left) {
                        bullet = ObjectPooler.Spawn(BulletID, transform.position, Quaternion.identity);

                        if (bullet == null) {
                            yield return new WaitForSeconds(0.001f);
                            continue;
                        }

                        // Start of copy-paste
                        Vector3 dir = StageController.Player.transform.position - transform.position;
                        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                        bullet.transform.rotation = Quaternion.AngleAxis(angle - 90 - LeftOffset, Vector3.forward);
                        // End of Copy-Paste

                        bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.up * Velocity;
                    }

                    if (Right) {
                        bullet = ObjectPooler.Spawn(BulletID, transform.position, Quaternion.identity);

                        if (bullet == null) {
                            yield return new WaitForSeconds(0.001f);
                            continue;
                        }

                        // Start of copy-paste
                        Vector3 dir = StageController.Player.transform.position - transform.position;
                        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                        bullet.transform.rotation = Quaternion.AngleAxis(angle - 90 + RightOffset, Vector3.forward);
                        // End of Copy-Paste

                        bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.up * Velocity;
                    }

                    break;

                case ShotType.RANDOM:
                    for (int i = 0; i < Count; i++) {
                        bullet = ObjectPooler.Spawn(BulletID, transform.position, Quaternion.identity);

                        if (bullet == null) {
                            yield return new WaitForSeconds(0.001f);
                            continue;
                        }

                        Vector2 rand = Random.insideUnitCircle;
                        float rand_angle = Mathf.Atan2(rand.y, rand.x) * Mathf.Rad2Deg;

                        bullet.transform.Rotate(0, 0, rand_angle);

                        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

                        rb.velocity = bullet.transform.up * Velocity;
                        rb.gravityScale = GravScale;
                    }
                    break;

                case ShotType.CIRCLE:
                    // StartCoroutine(SpawnCircle(transform.position, BulletID, NumInCircle, CircleCount));
                    // print(waveNum);
                    // print(Count);
                    if (waveNum >= Count) break;

                    waveNum++;

                    for (int j = 0; j < CircleCount; j++) {
                        for (int i = 0; i < NumInCircle; i++) {
                            bullet = ObjectPooler.Spawn(BulletID, transform.position, Quaternion.identity);
                            bullet.transform.Rotate(new Vector3(0, 0, (360 / NumInCircle * i) - 90));
                            bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.up * Velocity;
                        }

                        yield return new WaitForSeconds(WaveDelay);
                    }
                    
                    break;
            }

            yield return new WaitForSeconds(time);
        }

    }

    public void HandleArgs(string args) {
        if (string.IsNullOrWhiteSpace(args)) return;

        string[] argv = args.Split();
        
        BulletID = argv[0];
        Frequency = float.Parse(argv[1]);
        Velocity = float.Parse(argv[2]);

        switch (Shot) {
            case ShotType.DIRECT:
                DirectOffset = float.Parse(argv[3]);
                break;

            case ShotType.TARGET:
                string lcr_flag = argv[3];
                Left = lcr_flag[0] == '1';
                Direct = lcr_flag[1] == '1';
                Right = lcr_flag[2] == '1';

                LeftOffset = float.Parse(argv[4]);
                RightOffset = float.Parse(argv[5]);
                break;

            case ShotType.CIRCLE:
                NumInCircle = int.Parse(argv[3]);
                Count = int.Parse(argv[4]);
                CircleCount = int.Parse(argv[5]);
                WaveDelay = float.Parse(argv[6]);
                break;

            case ShotType.RANDOM:
                Count = int.Parse(argv[3]);
                GravScale = float.Parse(argv[4]);
                break;
        }
    }

    private IEnumerator SpawnCircle(Vector3 pos, string bulletId, int circleDensity, int circleCount = 1, float timeBetweenCircle = 1f, float timeBetweenShot = 0) {
        for (int j = 0; j < circleCount; j++) {
            for (int i = 0; i < circleDensity; i++) {
                GameObject bullet = ObjectPooler.Spawn(bulletId, pos, Quaternion.identity);
                bullet.transform.Rotate(new Vector3(0, 0, (360 / circleDensity * i) - 90));
                bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.up * Velocity;

                if(timeBetweenShot > 0) {
                    yield return new WaitForSeconds(timeBetweenShot);
                }
            }

            yield return new WaitForSeconds(timeBetweenCircle);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(EnemyShot))]
public class EnemyShot_Editor : Editor
{
    public override void OnInspectorGUI() {
        EnemyShot script = target as EnemyShot;

        script.BulletID = EditorGUILayout.TextField("Bullet ID", script.BulletID);
        script.BulletHue = EditorGUILayout.ColorField("Bullet Hue", script.BulletHue);
        script.Shot = (EnemyShot.ShotType)EditorGUILayout.EnumPopup("Shot Type", script.Shot);

        script.Frequency = EditorGUILayout.FloatField("Frequency", Mathf.Max(0.001f, script.Frequency));
        script.Velocity = EditorGUILayout.FloatField("Velocity", script.Velocity);

        switch(script.Shot) {
            case EnemyShot.ShotType.DIRECT:
                script.DirectOffset = EditorGUILayout.Slider("Shot Angle (Deg)", script.DirectOffset, 0, 360);
                break;

            case EnemyShot.ShotType.CIRCLE:
                script.NumInCircle = EditorGUILayout.IntField("Bullets per circle", Mathf.Max(0, script.NumInCircle));
                script.CircleCount = EditorGUILayout.IntField("Circles Per Wave", Mathf.Max(0, script.CircleCount));
                script.Count = EditorGUILayout.IntField("Wave Count", Mathf.Max(-1, script.Count));
                script.WaveDelay = EditorGUILayout.FloatField("Time Between Circles", Mathf.Max(0, script.WaveDelay));
                break;

            case EnemyShot.ShotType.RANDOM:
                script.Count = EditorGUILayout.IntField("Bullet Count", script.Count);
                script.GravScale = EditorGUILayout.FloatField("Bullet Gravity Scale", script.GravScale);
                break;

            case EnemyShot.ShotType.TARGET:
                script.Left = EditorGUILayout.Toggle("Aim Left", script.Left);
                script.Direct = EditorGUILayout.Toggle("Aim Directly", script.Direct);
                script.Right = EditorGUILayout.Toggle("Aim Right", script.Right);

                if (script.Left) {
                    script.LeftOffset = EditorGUILayout.Slider("Left Offset (Deg)", script.LeftOffset, 0, 180);
                }

                if (script.Right) {
                    script.RightOffset = EditorGUILayout.Slider("Right Offset (Deg)", script.RightOffset, 0, 180);
                }
                break;

            case EnemyShot.ShotType.SPIRAL:
                break;
        }
    }
}
#endif