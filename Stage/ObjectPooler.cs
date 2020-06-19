using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
        public bool group;
    }

    #region Constants
    public const string ICE = "IceCrystal";
    public const string ENERGY_BALL = "EnergyBall";
    #endregion

    public List<Pool> Pools;

    public Dictionary<string, Queue<GameObject>> PoolDict;
    public Dictionary<string, GameObject> PoolGroups;
    private Dictionary<string, int> PoolMax;

    private void Start() {
        PoolDict = new Dictionary<string, Queue<GameObject>>();
        PoolGroups = new Dictionary<string, GameObject>();
        PoolMax = new Dictionary<string, int>();

        foreach(Pool pool in Pools) {
            Queue<GameObject> objPool = new Queue<GameObject>();

            GameObject poolGameStore = null;

            PoolMax.Add(pool.tag, pool.size);

            if (pool.group) {
                poolGameStore = new GameObject(pool.tag);
                PoolGroups.Add(pool.tag, poolGameStore);
            }

            for (int i = 0; i < pool.size; i++) {
                GameObject p = Instantiate(pool.prefab);
                p.SetActive(false);
                objPool.Enqueue(p);

                if(pool.group) p.transform.SetParent(poolGameStore.transform);
            }

            PoolDict.Add(pool.tag, objPool);
        }
    }

    #region Singleton
    public static ObjectPooler Instance;

    private void Awake() {
        Instance = this;
    }
    #endregion

    public GameObject SpawnFromPool(string ptag, Vector3 position, Quaternion rotation) {
        if (PoolDict == null) return null;
        
        if (!PoolDict.ContainsKey(ptag)) {
            Debug.LogWarning($"Pool with tag {ptag} does not exist!");
            return null; 
        }
        
        GameObject o;

        if (PoolDict[ptag].Count == 0) {
            Pool p = GetPool(ptag);
            o = Instantiate(p.prefab);

            if (p.group) o.transform.SetParent(PoolGroups[ptag].transform);
            PoolMax[ptag]++;
            Debug.LogWarning($"Pool for {ptag} was too small! It needs to have a size of {PoolMax[ptag]}");

            PoolDict[ptag].Enqueue(o);
        }

        o = PoolDict[ptag].Dequeue();  // otherwise use safe pool

        o.SetActive(true);

        o.transform.position = position;
        o.transform.rotation = rotation;

        SpriteRenderer sr = o.GetComponent<SpriteRenderer>();

        if(sr != null) {
            sr.sortingOrder = 1;
        }

        return o;
    }

    public void Destroy(GameObject obj) {
        if (!obj.activeSelf) return; // Don't re-destroy an object

        PooledObject o = obj.GetComponent<PooledObject>();

        if(o == null) {
            Object.Destroy(obj);
            return; // Don't try and pool non-poolables.
        }

        obj.SetActive(false);

        Boundary.WarpCount.Remove(obj);

        if(obj.GetComponent<Rigidbody2D>() != null) obj.GetComponent<Rigidbody2D>().gravityScale = 0;

        string ptag = o.Tag;
        Pool p = Pools.Find(x => x.tag == ptag);

        if(p == null) {
            Debug.LogWarning($"There is no pool for bullet {o.Tag}!");
            return;
        }

        SpriteRenderer ospr = obj.GetComponent<SpriteRenderer>();
        SpriteRenderer pspr = p.prefab.GetComponent<SpriteRenderer>();

        if (ospr != null && pspr != null) {
            ospr.color = pspr.color;  // reset color.
        }

        o.transform.localScale = p.prefab.transform.localScale; // Reset scale

        SimpleMovement sm = p.prefab.GetComponent<SimpleMovement>();
        SimpleMovement osm = obj.GetComponent<SimpleMovement>();

        if(sm != null && osm != null) {
            osm.UpdateAcceleration(sm.XAcceleration, sm.YAcceleration);
        }

        foreach (Component comp in obj.GetComponents(typeof(Component))) {
            if (p.prefab.GetComponent(comp.GetType()) == null) Destroy(comp);
        }

        if (o != null) {
            PoolDict[o.Tag].Enqueue(obj);
        }
    }

    private Pool GetPool(string poolTag) {
        foreach(Pool p in Pools) {
            if (p.tag == poolTag) return p;
        }

        return null;
    }

    public static GameObject Spawn(string tag, Vector3 position, Quaternion rotation, bool sfx = false) {
        if(sfx) StageController.AudioManager.PlaySFX("shootbullet", 0.2f);
        return Instance.SpawnFromPool(tag, position, rotation);
    }

    public static void Disable(GameObject obj) {
        Instance.Destroy(obj);
    }
}
