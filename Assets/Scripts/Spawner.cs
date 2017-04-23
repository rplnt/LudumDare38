using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    bool displayLine = true;
    LineRenderer line;
    public Vector2 direction;
    AntController ac;
    Nest nest;
    Transform head;

	void Start () {
        nest = FindObjectOfType<Nest>();

        head = transform.FindChild("Head");

        Time.timeScale = 0.0f;
	}

    public void Open() {
        nest.OpenSpawner(transform);
    }
	

}
