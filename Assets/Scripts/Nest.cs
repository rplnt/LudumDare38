using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Consumables {
    public int food;
    public int dirt;
    public int stones;
    public int sticks;
}

public class Nest : MonoBehaviour {

    [Header("Ants")]
    public int antCount;
    public int nestedCount;
    public int nestCapacity;
    public int minNested;
    public int toNest;

    [Header("Ant Properties")]
    public GameObject prefab;

    [Header("Spawn")]
    public float releaseDelay;
    private float lastRelease;
    public float birthDelay;
    private float lastBirth;

    Consumables resources;

    int[] levelMultipliers = { 1, 2, 4, 5, 8 };

    int level = 0;

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

        ObjectPool.CreatePool(prefab, 50);

        resources = new Consumables();

        ground = GameObject.Find("Ground/Ants");
    }


    void Update() {
        // create ant
        if (nestedCount < nestCapacity && lastBirth + birthDelay < Time.time && resources.food > 0) {
            resources.food--;
            nestedCount++;
            antCount++;
            lastBirth = Time.time;
        }

        // release ant
        if (nestedCount > toNest && lastRelease + releaseDelay < Time.time) {
            //spawnerIndex = (spawnerIndex + 1) % activeSpawners;
            //GameObject larva = prefab.Spawn(ground.transform, spawners[spawnerIndex].transform.position, Random.rotation);
            //larva.transform.Find("Item").GetComponent<SpriteRenderer>().enabled = true;
            //antControl.AddAnt(larva, spawners[spawnerIndex]);
            //nestedCount--;
            //lastRelease = Time.time;
        }
    }


    void Upgrade() {
        level++;
        transform.FindChild("Level " + level);

    }

    float lowAlpha = 0.2f;
    void PulseSpawners() {

    }

}
