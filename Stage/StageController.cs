using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageController : MonoBehaviour
{
    #region GeneralInfo
    public static StageController Stage;
    public static GameObject Player;
    public static AudioClipManager AudioManager;

    private int EnemiesBeat;

    public int StageNumber;

    public GameObject BackgroundMusicPlayer;
    public GameObject UICanvas;
    public GameObject BossDisplay;
    public GameObject BulletParticles;
    #endregion

    public WaypointMovement Waypoints;

    #region Powerups
    public GameObject PointItem;
    public GameObject PowerItem;
    public GameObject BigPowerItem;
    public GameObject FullPowerItem;
    public GameObject BombItem;
    public GameObject LifeItem;
    #endregion

    #region Enemy Testing
    public bool TestingMode;

    public GameObject TestingBoss;

    [Min(0)]
    public int Count;

    [Min(0)]
    public float StartDelay;
    
    [Min(0.1f)]
    public float Delay;

    public GameObject Enemy;
    public Vector3 SpawnArea;
    #endregion

    #region Level
    [HideInInspector]
    public string LevelTitle;

    [HideInInspector]
    public string LevelSubtitle;

    public List<EnemyLibEntry> LibraryEntries;

    public Dictionary<string, GameObject> EnemyLibrary;

    public List<GameObject> Enemies;
    public List<GameObject> Bosses;

    public List<TextAsset> Chapters;
    public List<AudioClip> BGM;
    #endregion

    private void Start() {
        EnemiesBeat = 0;
        Stage = this;
        StageNumber = 0;

        AudioManager = BackgroundMusicPlayer.GetComponent<AudioClipManager>();

        EnemyLibrary = new Dictionary<string, GameObject>(LibraryEntries.Count);

        PlayerBullet.BulletDestroyParticles = BulletParticles;

        if(TestingMode) {
            Debug.Log("Starting in test mode");
            StartCoroutine(SpawnEnemies());
        } else {
            Debug.Log("Loading enemies...");

            // Load enemies into lib
            foreach(EnemyLibEntry entry in LibraryEntries) {
                EnemyLibrary.Add(entry.Id, entry.Enemy);
            }

            Debug.Log("Loaded enemies.");

            Debug.Log("Starting in play mode...");
            StartCoroutine(PlayLevel());
        }
    }   

    IEnumerator SpawnEnemies() {
        yield return new WaitForSeconds(StartDelay);

        while (true) {
            for (int i = 0; i < Count; i++) {
                GameObject enemy = Instantiate(Enemy, new Vector3(Random.Range(-SpawnArea.x, SpawnArea.x), SpawnArea.y, SpawnArea.z), Quaternion.identity);

                yield return new WaitForSeconds(Delay);
            }

            if(Count == 0) {
                yield return new WaitForSeconds(Delay);  // prevent freezing
            }
        }
    }
    
    /// <summary>
    /// This method starts spawning the enemies according to the script
    /// </summary>
    /// <returns></returns>
    IEnumerator PlayLevel() {
        LevelLoader level = new LevelLoader(Chapters[StageNumber]);
        level.ProcessLevel();

        // TODO: Display title and subtitle
        Debug.Log($"Now entering stage {StageNumber+1}:\n{LevelTitle}\n{LevelSubtitle}");

        while(true) {
            string[] ins = level.NextInstruction();

            if (ins == null) break;

            switch(ins[0]) {
                case "spawn":
                    int ptr = 1;
                    string enemy = ins[ptr++];
                    float hp = float.Parse(ins[ptr++]);
                    int count = int.Parse(ins[ptr++]);
                    float pauseBetween = float.Parse(ins[ptr++]);
                    float x = float.Parse(ins[ptr++]);
                    float y = float.Parse(ins[ptr++]);
                    float vx = float.Parse(ins[ptr++]);
                    float vy = float.Parse(ins[ptr++]);
                    float ax = float.Parse(ins[ptr++]);
                    float ay = float.Parse(ins[ptr++]);

                    string args = "";

                    for(int i = ptr; i < ins.Length; i++) {
                        args += $"{ins[i]} ";
                    }

                    args = args.Trim();

                    // TODO:
                    // Optional StopAt co-ordinates so that the enemy stops at a certain point

                    StartCoroutine(SpawnEnemies(enemy, hp, count, pauseBetween, x, y, vx, vy, ax, ay, args));
                    break;
                case "pause":
                    yield return new WaitForSeconds(float.Parse(ins[1]));
                    break;
                case "hue":
                    break;
                case "spawnboss":
                    SpawnBoss(int.Parse(ins[1]));
                    break;
                case "clear":
                    foreach(GameObject e in Enemies) {
                        Destroy(e);
                    }

                    KillAllEnemyBullets(false);
                    break;
            }

            yield return new WaitForSeconds(0.0001f);
        }
    }

    public IEnumerator SpawnEnemies(string enemy, float hp, int count, float timeBetween, float x, float y, float vx, float vy, float ax = 0.0f, float ay = 0.0f, string args = "") {
        for (int i = 0; i < count; i++) {
            GameObject newEnemy = Instantiate(EnemyLibrary[enemy], new Vector3(x, y, 0), Quaternion.identity);
            Enemies.Add(newEnemy);

            SimpleMovement enemyMovement = newEnemy.GetComponent<SimpleMovement>();
            EnemyShot shot = newEnemy.GetComponent<EnemyShot>();

            newEnemy.GetComponent<Shootable>().StartingHealth = hp;
            enemyMovement.UpdateVelocity(vx, vy);
            enemyMovement.UpdateAcceleration(ax, ay);

            if(shot != null && !string.IsNullOrWhiteSpace(args)) {
                shot.args = args;
            }

            yield return new WaitForSeconds(timeBetween);
        }
    }

    public static void SpawnBoss(int boss_num = 1) {
        if(boss_num < 1) {
            Debug.LogError($"Invalid boss number! Number: {boss_num}");
        }

        GameObject boss;
        if (Stage.TestingMode) {
            boss = Instantiate(Stage.TestingBoss, new Vector3(0, 18, 0), Quaternion.identity);
        } else {
            boss = Instantiate(Stage.Bosses[boss_num], new Vector3(0, 18, 0), Quaternion.identity);
        }

        GameObject bossH = Instantiate(Stage.BossDisplay, Stage.UICanvas.transform);
        bossH.transform.SetParent(Stage.UICanvas.transform);
        bossH.GetComponent<BossUI>().UIBoss = boss;
    }

    public int GetEnemiesBeat() {
        return EnemiesBeat; 
    }

    public void BeatEnemy() {
        EnemiesBeat++;
        // CHECK FOR SPECIAL NUMBERS
    }

    public static GameObject GetItem(EnumItemType itemType) {
        switch(itemType) {
            case EnumItemType.POINT:
                return Stage.PointItem;

            case EnumItemType.POWER:
                return Stage.PowerItem;

            case EnumItemType.BIGPOWER:
                return Stage.BigPowerItem;

            case EnumItemType.FULLPOWER:
                return Stage.FullPowerItem;

            case EnumItemType.BOMB:
                return Stage.BombItem;

            case EnumItemType.LIFE:
                return Stage.LifeItem;

            default:
                return null;
        }
    }

    public static GameObject DropItem(EnumItemType item, Vector2 pos, bool wide = false) {
        GameObject drop = Instantiate(GetItem(item), pos, Quaternion.identity);

        float x = 0;

        if(wide) {
            x = Random.Range(-2f, 2f);
        }

        drop.GetComponent<Rigidbody2D>().velocity = new Vector2(x, Random.Range(0, 6f));
        drop.transform.Translate(Random.Range(-1f, 1f), 0, 0);

        return drop;
    }

    public void KillAllEnemyBullets(bool points = true) {
        foreach (GameObject bullet in GameObject.FindGameObjectsWithTag("EnemyBullet")) {
            ObjectPooler.Disable(bullet);
            if(points) Player.GetComponent<PlayerController>().AddPoints(100);
        }
    }
}

[System.Serializable]
public class EnemyLibEntry
{
    public string Id;
    public GameObject Enemy;
}