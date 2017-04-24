using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Nest : MonoBehaviour {

    [Header("Ants")]
    public int antCount;
    public int nestedCount;

    [Header("Ant Properties")]
    public GameObject antPrefab;

    [Header("Spawn")]
    public GameObject spawnPrefab;
    public int antFoodCost;
    private float lastRelease;
    private float lastBirth;

    [Header("Stats")]
    public int totalAnts;
    public int totalNests;
    public int killedBugs;

    Transform[] slots = new Transform[Level.maxLevel + 1];
    List<Transform> spawners = new List<Transform>();

    Consumables items = new Consumables(20, 10, 5, 5);

    int[] levelMultipliers = { 1, 2, 4, 5, 8 };

    public System.Action<Consumables> resourcesUpdated;
    public System.Action<int> antCountChanged;
    public System.Action<int> antNestedCountChanged;
    public System.Action<bool> enoughResourcesToUpgrade;
    public System.Action leveledUp;
    public System.Action<bool> build;
    public System.Action newNest;
    public System.Action GameOver;

    public int currentLevel = 0;

    [Header("Objects")]
    public LineRenderer nestedAntsBar;
    public GameObject offsitePrefab;
    Head offsite;

    float lastAttack = 0.0f;
    float attackBLockade = 1.0f;

    //float feromones;

    GameObject ground;
    AntController antControl;

    void Start() {
        nestedCount = antCount;
        lastBirth = Time.time;
        lastRelease = Time.time;
        antControl = FindObjectOfType<AntController>();
        if (antControl == null) {
            Debug.LogError("Err: could not find controller");
        }

        antNestedCountChanged += UpdateBar;

        ObjectPool.CreatePool(antPrefab, 50);

        ground = GameObject.Find("Ground/Ants");

        /* spawners */
        foreach (Transform levelTransform in transform) {
            if (levelTransform.name.StartsWith("Level")) {
                string[] spawnLevels = levelTransform.name.Split(' ');
                int tempLevel;
                if (spawnLevels.Length == 2 && int.TryParse(spawnLevels[1], out tempLevel)) {
                    slots[tempLevel] = levelTransform.FindChild("Slot");
                }
            }
        }

        if (resourcesUpdated != null) {
            resourcesUpdated(items);
        }
        if (antCountChanged != null) {
            antCountChanged(antCount);
        }
        if (antNestedCountChanged != null) {
            antNestedCountChanged(nestedCount);
        }
    }

    void UpdateBar(int count) {
        if (nestedAntsBar == null) return;

        float ratio = (float)count/Level.limits[currentLevel];
        nestedAntsBar.SetPosition(1, new Vector3(-0.4f + 1.5f * ratio, 0.4f, -5.0f));
    }

    public void Turn() {
        //items.dirt -= 2;
    }

    public float Attack() {
        nestedCount -= 1;
        if (nestedCount <= 0) {
            Debug.Log("GAME OVER");
            /* GAME OVER*/
            nestedAntsBar.enabled = false;
            if (GameOver != null) {
                GameOver();
                Time.timeScale = 0.0f;
            }

            return 0.0f;
        }

        antCount -= 1;
        lastAttack = Time.time;

        if (antCountChanged != null) {
            antCountChanged(antCount);
        }
        if (antNestedCountChanged != null) {
            antNestedCountChanged(nestedCount);
        }

        return Mathf.Max(0.75f * nestedCount, 0.0f);
    }


    void Update() {
        // create ant
        if (nestedCount < Level.limits[currentLevel] && lastBirth + Level.birthDelay[currentLevel] < Time.time && items.food >= antFoodCost) {
            items.food -= antFoodCost;
            nestedCount++;
            antCount++;
            lastBirth = Time.time;

            if (resourcesUpdated != null) {
                resourcesUpdated(items);
            }
            if (antCountChanged != null) {
                antCountChanged(antCount);
            }
            if (antNestedCountChanged != null) {
                antNestedCountChanged(nestedCount);
            }
            totalAnts++;
        }

        // release ant
        if (nestedCount > Level.limits[currentLevel] / 2 && lastRelease + Level.releaseDelay[currentLevel] < Time.time && spawners.Count > 0 && Time.time > lastAttack + attackBLockade) {
            Transform spawner = spawners[Random.Range(0, spawners.Count)];
            GameObject larva = antPrefab.Spawn(ground.transform, spawner.position, Quaternion.identity);
            larva.transform.Find("Item").GetComponent<SpriteRenderer>().enabled = false;
            antControl.AddAnt(larva, spawner);
            nestedCount--;
            lastRelease = Time.time;
            if (antNestedCountChanged != null) {
                antNestedCountChanged(nestedCount);
            }
        }

        if (currentLevel < Level.maxLevel && items.IsEnough(Level.costs[currentLevel + 1])) {
            if (enoughResourcesToUpgrade != null) {
                enoughResourcesToUpgrade(true);
            }
        }

        PulseSlots();
    }

    public void LevelUp() {
        if (currentLevel + 1 > Level.maxLevel) {
            Debug.LogError("OVERLEVELED!");
            return;
        }

        if (!items.Consume(Level.costs[currentLevel + 1])) {
            enoughResourcesToUpgrade(false);
            return;
        }

        currentLevel++;
        if (leveledUp != null) {
            leveledUp();
        }

        Transform subnest = transform.FindChild("Level " + currentLevel);
        if (subnest != null) {
            subnest.gameObject.SetActive(true);
        }

    }

    void PulseSlots() {
        foreach (Transform slot in slots) {
            if (slot != null && slot.CompareTag("Slot") && slot.gameObject.activeInHierarchy) {
                SpriteRenderer sr = slot.GetComponent<SpriteRenderer>();
                if (sr != null) {
                    sr.color = new Color(1.0f, 1.0f, 1.0f, 0.2f + Mathf.Repeat(Time.time, 0.8f));
                }
            }
        }
    }


    public void NestAnt(Ant ant) {
        items.Add(ant.carrying);
        nestedCount++;
        ant.go.Recycle();

        if (resourcesUpdated != null) {
            resourcesUpdated(items);
        }

        if (antNestedCountChanged != null) {
            antNestedCountChanged(nestedCount);
        }
    }

    public bool NestAntOffsite(Ant ant, Transform location) {
        OffsiteManager om = location.GetComponent<OffsiteManager>();
        if (!om.NestAnt()) {
            return false;
        }
        ant.go.Recycle();
        return true;
    }

    //public void KillFreeAnt(Ant ant) {
    //    ant.go.Recycle();
    //    antCount--;
    //    if (antCountChanged != null) {
    //        antCountChanged(antCount);
    //    }
    //}

    public void KilledOffsiteAnt() {
        antCount--;

        if (antCountChanged != null) {
            antCountChanged(antCount);
        }
    }

    public void CreateSpawner(GameObject slot) {
        slot.SetActive(false);
        GameObject spawner = Instantiate(spawnPrefab, slot.transform.position, slot.transform.rotation, slot.transform.parent);
    }



    
    public void OpenSpawner(Transform t) {
        spawners.Add(t);
    }

    
    public void BuildOffsite(Head buildLocation) {
        Time.timeScale = 0.0f;
        offsite = buildLocation;
        if (build != null) {
            build(items.IsEnough(Level.costs[0]));
        }
    }

    public void BuildFortress(bool build) {
        if (build) {
            if (items.Consume(Level.costs[0])) {
                Instantiate(offsitePrefab, offsite.transform.position, Quaternion.identity, transform);
                totalNests++;
            } else {
                return;
            }
            if (newNest != null) {
                newNest();
            }
        }

        offsite.SpawnNewHead();

    }

}
