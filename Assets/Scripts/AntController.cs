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

    public override string ToString() {
        return base.ToString() + " " + go.name + " prev: " + (prev==null) + " next: " + (next==null);
    }
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
    GroundFloor ground;

    public Action<Vector2> dig;
    Nest nest;


	// Use this for initialization
	void Start () {
        ants = new List<Ant>();
        paths = new Dictionary<GameObject, Node>();
        ProcessSprites();
        nest = FindObjectOfType<Nest>();
        ground = FindObjectOfType<GroundFloor>();
	}

    void ProcessSprites() {
        itemSprites = new Dictionary<string, List<Sprite>>();
        foreach(Sprite sprite in sprites) {
            string key;
            string[] keys = sprite.name.Split('_');
            if (keys.Length == 2) {
                key = keys[0];
            } else {
                key = keys[0] + "_" + keys[1];
            }
            if (!itemSprites.ContainsKey(key)) {
                itemSprites[key] = new List<Sprite>();   
            }
            itemSprites[key].Add(sprite);
        }

        //foreach (string k in itemSprites.Keys) {
        //    Debug.Log(k);
        //}
    }


    public void AddAnt(GameObject go, Transform spawner) {
        Ant larva = new Ant(go, defaultHealth * UnityEngine.Random.Range(0.9f, 1.1f));
        if (!paths.ContainsKey(spawner.gameObject)) {
            Debug.LogError(spawner.name);
        }
        Node spawnerNode = paths[spawner.gameObject];
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
                        if (ground.bounds.Contains(ant.NextTargetPosition)) {
                            // move head
                            if (AntDig(ant)) {
                                ant.NextTarget.go.transform.Translate((ant.NextTarget.go.transform.position - ant.NextTarget.prev.go.transform.position).normalized * Level.digSpeed, Space.World);
                                if (dig != null) {
                                    dig(ant.NextTarget.go.transform.position);
                                }
                            }
                            if (ant.remove) {
                                ants.RemoveAt(i);
                            }
                        }
                    }
                    ant.homing = true;
                    ant.SetTarget(ant.NextTarget.prev);

                } else if (ant.homing && ant.NextTarget.prev == null) {
                    // home
                    nest.NestAnt(ant);
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


    public void AddNodeToPath(GameObject to, GameObject from) {
        Node target;
        if (paths.ContainsKey(to)) {
            target = paths[to];
        } else {
            target = new Node(to, null, null);
            paths[to] = target;
        }
        
        if (!paths.ContainsKey(from)) {
            paths[from] = new Node(from, target, null);
        } else {
            paths[from].next = target;
        }

        target.prev = paths[from];

        if (dig != null) {
            dig(from.transform.position);
            dig(from.transform.position);
        }

    }


    readonly string[] itemTypes = { "", "dirt", "stick", "dirt", "stone", "dirt", "food", "" };
    bool AntDig(Ant ant) {
        string key = "";

        bool rt = true;

        /* check if we are near something */
        RaycastHit2D hit = Physics2D.Raycast(ant.go.transform.position, Vector3.forward, Mathf.Infinity, 1 << LayerMask.NameToLayer("AntsCare"));
        if (hit.collider != null) {

            /* pick up item */
            if (hit.transform.CompareTag("Item")) {
                Item itemObject = hit.transform.GetComponent<Item>();
                if (itemObject == null) {
                    Debug.LogError("Found Item object without Item script");
                }

                key = itemObject.DigItem();
                rt = false;

            } else if (hit.transform.CompareTag("Offsite")) {
                rt = !nest.NestAntOffsite(ant, hit.transform);
                if (!rt) {
                    ant.remove = true;
                }
                return rt;
            }
        } else {
            /* random dig from the ground */
            key = itemTypes[UnityEngine.Random.Range(0, itemTypes.Length)];
        }
        
        if (key == "") return true;

        List<Sprite> spriteList = itemSprites[key];
        SpriteRenderer sr = ant.go.transform.Find("Item").GetComponent<SpriteRenderer>();
        sr.sprite = spriteList[UnityEngine.Random.Range(0, spriteList.Count)];
        sr.enabled = true;
        ant.carrying = key;

        return rt;
    }

}
