using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugAI : MonoBehaviour {

    public float speed;
    public float rotateSpeed;
    public float maxHealthBase;
    float currentHealth;
    public float attackDelay;
    float lastAttack = 0.0f;

    bool dead = false;

    public Color[] colors;

    float slowDown = 1.0f;

    GameObject[] targets;
    GameObject currentTarget;
    public LineRenderer healthBar;
    Transform graphics;
    SpriteRenderer sr;

    public GameObject itemPrefab;
    public Sprite[] bugParts;

    Nest ns;

    
	// Use this for initialization
	void Start () {
        currentHealth = maxHealthBase;
        UpdateTargets();
        graphics = transform.FindChild("Graphics");
        sr = graphics.GetComponent<SpriteRenderer>();
        sr.color = colors[Random.Range(0, colors.Length)];
        ns = FindObjectOfType<Nest>();
        ns.newNest += UpdateTargets;
	}
	
	// Update is called once per frame
	void Update () {
        if (dead) {
            Color c = sr.color;
            c.a -= 0.01f * Time.deltaTime;
            if (c.a < 0.1f) {
                Destroy(gameObject);
            }
            sr.color = c;

            return;
        }

        if (currentTarget == null) {
            Debug.LogError("BugAI: No target!");
            UpdateTargets();
            return;
        }

        Vector3 targetVector = currentTarget.transform.position - transform.position;

        if (targetVector.magnitude < 0.1f) {
            // ATTACK
            if (Time.time > lastAttack + attackDelay) {
                lastAttack = Time.time;
                if (currentTarget.transform.parent.CompareTag("Offsite")) {
                    OffsiteManager om = currentTarget.transform.parent.GetComponent<OffsiteManager>();
                    currentHealth -= om.Attack();
                } else {
                    currentHealth -= ns.Attack();
                }

                UpdateHealthBar();

                if (currentHealth < 0.0f) {
                    /* DEATH */
                    ns.killedBugs++;
                    Animator anim = gameObject.GetComponentInChildren<Animator>();
                    if (anim != null) {
                        anim.enabled = false;
                    }
                    healthBar.enabled = false;
                    sr.color = Color.white;
                    dead = true;

                    /* change to item */
                    Item item = gameObject.AddComponent<Item>();
                    item.itemType = "food_bug";
                    item.capacity = 10;
                    gameObject.tag = "Item";
                    gameObject.layer = 8;
                }
            }
        } else {
            // MOVE
            float angle = Mathf.Atan2(targetVector.y, targetVector.x) * Mathf.Rad2Deg - 90;
            Quaternion targetRotation = Quaternion.RotateTowards(graphics.rotation, Quaternion.AngleAxis(angle, Vector3.forward), rotateSpeed * Time.deltaTime * slowDown);

            /* rotate or move */
            if (graphics.rotation != targetRotation) {
                graphics.rotation = targetRotation;
            } else {
                transform.position = Vector2.MoveTowards(transform.position, currentTarget.transform.position, Time.deltaTime * speed * slowDown);
            }
        }
	}


    void UpdateHealthBar() {
        healthBar.SetPosition(1, new Vector3(-0.5f + (currentHealth/maxHealthBase), 0.5f, -4.0f));
    }

    void UpdateTargets() {
        targets = GameObject.FindGameObjectsWithTag("Nest");
        currentTarget = null;
        float minDistance = Mathf.Infinity;
        foreach (GameObject target in targets) {
            float distance = Vector2.Distance(transform.position, target.transform.position);
            if (distance < minDistance) {
                minDistance = distance;
                currentTarget = target;
            }
        }
    }
}
