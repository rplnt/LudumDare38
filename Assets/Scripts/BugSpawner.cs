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
        GameObject bug = Instantiate(bugPrefab, transform.position + new Vector3(0.0f, Random.Range(-3.0f, +3.0f), 0.0f), Quaternion.identity, ground.transform);

        lastSpawn = Time.time;
        spawnDelay = Mathf.Max(5.0f, spawnDelay - 0.25f);
        spawned++;

        BugAI ai = bug.GetComponent<BugAI>();


        if (spawned % 10 == 0) {
            /* boss */
            ai.health = (spawned / 10) * 2 * ai.health;
            ai.damage = (ai.damage + (spawned / 5)) * 3.0f;
            ai.speed = 0.666f * ai.speed;
            bug.transform.localScale *= 1.6f;

        } else {
            float healthModifier = Random.Range(0.0f, 1.0f);
            bug.transform.localScale *= (1.0f + healthModifier/10.0f);
            ai.health = ai.health + 2.0f * (1.0f + healthModifier) * spawned;
            ai.damage = ai.damage + (spawned / 5);
        }

        if (BugSpawned != null) {
            BugSpawned(spawned);
        }
    }
}
