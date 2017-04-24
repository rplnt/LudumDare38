using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugSpawner : MonoBehaviour {

    public GameObject bugPrefab;
    GameObject ground;

    int spawned = 0;

    public float spawnDelay;
    float lastSpawn;

    public System.Action<int> BugSpawned;

	// Use this for initialization
	void Start () {
        ground = GameObject.Find("Ground");
        lastSpawn = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
        if (lastSpawn + spawnDelay < Time.time) {
            Spawn();
        }
	}

    void Spawn() {
        Instantiate(bugPrefab, transform.position + new Vector3(0.0f, Random.RandomRange(-3.0f, +3.0f), 0.0f), Quaternion.identity, ground.transform);
        lastSpawn = Time.time;
        spawnDelay = Mathf.Max(5.0f, spawnDelay - 0.25f);
        spawned++;

        if (BugSpawned != null) {
            BugSpawned(spawned);
        }
    }
}
