using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Node {
    public Node(GameObject _go, Node next, Node prev) {
        this.go = _go;
        this.next = next;
        this.prev = prev;
    }
    public GameObject go;
    public Node next;
    public Node prev;
}


public class AntController : MonoBehaviour {

    [Header("Ant Properties")]
    public float rotateSpeed;
    public float defaultHealth;

    [Header("Consumables")]
    public Sprite[] sprites;
    Dictionary<string, List<Sprite>> itemSprites;


    List<Ant> ants;
    Dictionary<GameObject, Node> paths;

    public Action<GameObject> dig;

	// Use this for initialization
	void Start () {
        ants = new List<Ant>();
        paths = new Dictionary<GameObject, Node>();
	}

    void ProcessSprites() {
        itemSprites = new Dictionary<string, List<Sprite>>();
        foreach(Sprite sprite in sprites) {
            char[] suckit = { '_' };
            string[] keys = sprite.name.Split(suckit, 2);
            if (!itemSprites.ContainsKey(keys[0])) {
                itemSprites[keys[0]] = new List<Sprite>();   
            }
            itemSprites[keys[0]].Add(sprite);
        }
    }


    public void AddAnt(GameObject go, GameObject spawner) {
        Ant larva = new Ant(go, defaultHealth * UnityEngine.Random.Range(0.9f, 1.1f));
        Node spawnerNode = paths[spawner];
        if (spawnerNode == null) {
            Debug.LogError("Ant spawned from spawner with no path");
            return;
        }
        larva.SetTarget(spawnerNode.next);
        ants.Add(larva);
    }
	

	void Update () {
        /* Move ants */
        //foreach (Ant ant in ants) {
        for (int i = ants.Count - 1; i >= 0; i--) {
            Ant ant = ants[i];
            if (ant.go == null) {
                Debug.LogError("Zombie ant");
                continue;
            }
            /* calculate rotation */
            Vector3 targetVector = ant.NextTargetPosition - ant.go.transform.position;
            float angle = Mathf.Atan2(targetVector.y, targetVector.x) * Mathf.Rad2Deg - 90;
            Quaternion targetRotation = Quaternion.RotateTowards(ant.go.transform.rotation, Quaternion.AngleAxis(angle, Vector3.forward), rotateSpeed * Time.deltaTime);

            /* rotate or move */
            if (ant.go.transform.rotation != targetRotation) {
                ant.go.transform.rotation = targetRotation;
            } else {
                ant.go.transform.position = Vector2.MoveTowards(ant.go.transform.position, ant.NextTargetPosition, ant.speed * Time.deltaTime);
            }

            /* At current target */
            if (ant.go.transform.position == ant.NextTargetPosition) {
                if (!ant.homing && ant.NextTarget.next == null) {
                    // reached end
                    if (ant.NextTarget.go.CompareTag("Head")) {
                        if (dig != null) {
                            dig(ant.NextTarget.go);  // TODO paint dirt, move head
                            PickUp(ant);
                        }
                    }
                    ant.homing = true;
                    ant.SetTarget(ant.NextTarget.prev);

                } else if (ant.homing && ant.NextTarget.prev == null) {
                    // home
                    NestAnt(ant);
                    ants.RemoveAt(i);
                } else if (ant.homing) {
                    // walk home
                    ant.SetTarget(ant.NextTarget.prev);
                } else {
                    // walk somewhere
                    ant.SetTarget(ant.NextTarget.next);
                }
            }
        }
	}


    string[] itemTypes = { "dirt", "stick", "dirt", "stone", "dirt", "food" };
    void PickUp(Ant ant) {
        string key = itemTypes[UnityEngine.Random.Range(0, itemTypes.Length)];
        List<Sprite> spriteList = itemSprites[key];
        SpriteRenderer sr = ant.go.transform.Find("Item").GetComponent<SpriteRenderer>();
        sr.sprite = spriteList[UnityEngine.Random.Range(0, spriteList.Count)];
        sr.enabled = true;
        ant.carrying = true;

        return;
    }


    void NestAnt(Ant ant) {
        Debug.Log("Nesting...");
        ant.go.Recycle();
        // TODO! ants.Remove(ant);
    }

}
