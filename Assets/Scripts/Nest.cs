using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Nest : MonoBehaviour {

    public int cheat;

    [Header("Ants")]
    public int antCount;
    public int nestedCount;
    public int nestedOffsiteCount;

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
    //List<Transform> spawners = new List<Transform>();
    Dictionary<Transform, float> spawners = new Dictionary<Transform, float>();
    int openedSpawners = 0;

    Consumables items;

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

    GameObject ground;
    AntController antControl;

    void Start() {
        items = new Consumables(20 + cheat, 15 + cheat, 5 + cheat, 5 + cheat);
        antCount = Level.limits[0];
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

        float ratio = Mathf.Clamp01((float)count/Level.limits[currentLevel]);
        nestedAntsBar.SetPosition(1, new Vector3(-0.4f + 1.5f * ratio, 0.4f, -5.0f));
    }

    public void Turn() {
        //items.dirt -= 2;
    }

    public float Attack(int damage) {
        if (Random.Range(0.0f, 1.0f) < 0.1f) {
            // critical TODO
            damage *= 2;
        }

        lastAttack = Time.time;
        float effectivity = 1.0f - (float)nestedCount / Level.limits[currentLevel];
        float attack = 0.3f * nestedCount + effectivity * 0.6f * nestedCount;

        int lostInCombat = Mathf.CeilToInt(attack * 0.05f);
        damage += lostInCombat;
        damage = Mathf.Min(damage, nestedCount);

        antCount -= damage;
        nestedCount -= damage;
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

        if (antCountChanged != null) {
            antCountChanged(antCount);
        }
        if (antNestedCountChanged != null) {
            antNestedCountChanged(nestedCount);
        }

        return Mathf.Max(attack, 0.0f);
    }


    void Update() {
        // create ant
        if (lastBirth + Level.birthDelay[currentLevel] < Time.time && nestedCount < Level.limits[currentLevel] && items.food >= antFoodCost) {
            items.food -= antFoodCost;
            nestedCount++;
            antCount++;
            lastBirth = Time.time;
            totalAnts++;

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

        // release ant
        if (lastRelease + Level.releaseDelay[currentLevel] < Time.time && openedSpawners > 0 && Time.time > lastAttack + attackBLockade) {
            Transform leastRecentSpawner = null;
            float leastRecentTime = Mathf.Infinity;
            foreach(KeyValuePair<Transform, float> spawnerData in spawners) {
                if (spawnerData.Value < leastRecentTime) {
                    leastRecentTime = spawnerData.Value;
                    leastRecentSpawner = spawnerData.Key;
                }
            }
            if (leastRecentTime < Time.time - Level.singleReleaseDelay) {
                ReleaseAnt(leastRecentSpawner);
            }
        }

        if (currentLevel < Level.maxLevel && items.IsEnough(Level.costs[currentLevel + 1])) {
            if (enoughResourcesToUpgrade != null) {
                enoughResourcesToUpgrade(true);
            }
        }

        PulseSlots();
    }


    public void ReleaseAnt(Transform spawner) {
        if (spawner == null) {
            Debug.LogError("Failed release");
            return;
        }
        /* update release time */
        spawners[spawner] = Time.time;

        if (nestedCount <= 1) return;
        GameObject larva = antPrefab.Spawn(ground.transform, spawner.position, Quaternion.identity);
        larva.transform.Find("Item").GetComponent<SpriteRenderer>().enabled = false;
        antControl.AddAnt(larva, spawner);
        nestedCount--;
        lastRelease = Time.time;
        if (antNestedCountChanged != null) {
            antNestedCountChanged(nestedCount);
        }
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
        ant.go.Recycle();
        nestedCount++;

        if (nestedCount > Level.limits[currentLevel]) {
            if (ant.NextTarget != null) {
                ReleaseAnt(ant.NextTarget.go.transform);
            } else {
                Debug.LogError("Lost ant at nest?");
            }
        }

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
        nestedOffsiteCount++;
        ant.go.Recycle();
        return true;
    }


    public void KilledOffsiteAnts(int count=1) {
        antCount -= count;
        nestedOffsiteCount -= count;

        if (antCountChanged != null) {
            antCountChanged(antCount);
        }
    }


    public void CreateSpawner(GameObject slot) {
        slot.SetActive(false);
        Instantiate(spawnPrefab, slot.transform.position, slot.transform.rotation, slot.transform.parent);
    }


    public void OpenSpawner(Transform spawner) {
        openedSpawners++;
        spawners[spawner] = Time.time;
    }

    public void CloseSpawner(Transform spawner) {
        openedSpawners--;
        spawners[spawner] = Mathf.Infinity;
    }

    
    public void BuildOffsite(Head buildLocation) {
        //Debug.Log("Nest:BuildOffsite");
        if (offsite != null) return;
        if (buildLocation == null) {
            Debug.LogError("Nest:BuildOffsite: null head");
            return;
        }
        Time.timeScale = 0.0f;
        offsite = buildLocation;
        if (build != null) {
            build(items.IsEnough(Level.costs[0]));
        }
    }

    public void Feed(int count) {
        items.food += count;
        if (resourcesUpdated != null) {
            resourcesUpdated(items);
        }
    }

    public void BuildFortress(bool doBuild) {
        //Debug.Log("Nest:BuildFortress:doBuild" + doBuild);
        if (doBuild) {
            if (items.Consume(Level.costs[0])) {
                Instantiate(offsitePrefab, offsite.transform.position, Quaternion.identity, transform);
                totalNests++;
            } else {
                offsite = null;
                return;
            }
            if (newNest != null) {
                newNest();
            }
        }
        if (offsite != null) {
            offsite.SpawnNewHead();
        } else {
            Debug.LogError("Could not find offsite, building? " + doBuild);
        }

        offsite = null;
    }

}
