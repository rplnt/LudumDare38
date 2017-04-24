using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugAI : MonoBehaviour {

    public float speed;
    public float rotateSpeed;
    public float health;
    public float currentHealth;
    public float attackDelay;
    public float damage;
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
        UpdateTargets();
        graphics = transform.FindChild("Graphics");
        sr = graphics.GetComponent<SpriteRenderer>();
        sr.color = colors[Random.Range(0, colors.Length)];
        ns = FindObjectOfType<Nest>();
        ns.newNest += UpdateTargets;
        currentHealth = health;
        //currentHealth = health + Random.Range(0.0f, 2.0f * ns.killedBugs);
	}

    IEnumerator Dead(int feedNest, float delay) {
        float elapsed = 0.0f;
        int fed = 0;

        Color c = sr.color;

        while (elapsed < delay) {
            elapsed += Time.deltaTime;
            if (fed < Mathf.Lerp(0, feedNest, elapsed / delay)) {
                ns.Feed(1);
                fed++;
            }
            c.a = Mathf.Lerp(1.0f, 0.0f, elapsed / delay);
            sr.color = c;
            yield return null;
        }

        Destroy(gameObject);
    }
	
	// Update is called once per frame
	void Update () {
        if (dead) {
            return;
        }

        if (currentTarget == null) {
            //Debug.LogError("BugAI: No target!");
            UpdateTargets();
            return;
        }

        Vector3 targetVector = currentTarget.transform.position - transform.position;

        if (targetVector.magnitude < 0.25f) {
            // ATTACK
            if (Time.time > lastAttack + attackDelay) {
                lastAttack = Time.time;
                if (currentTarget.transform.parent.CompareTag("Offsite")) {
                    //Debug.Log(gameObject.name + " attacking offsite nest from " + gameObject.transform.position);
                    OffsiteManager om = currentTarget.transform.parent.GetComponent<OffsiteManager>();
                    currentHealth -= om.AttackOffsite(Mathf.RoundToInt(damage));
                } else {
                    //Debug.Log(gameObject.name + " attacking main nest from " + gameObject.transform.position);
                    currentHealth -= ns.Attack(Mathf.RoundToInt(damage));
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
                    ns.newNest -= UpdateTargets;

                    if (currentTarget.transform.parent.CompareTag("Offsite")) {
                        StartCoroutine(Dead(6, 5.0f));
                    } else {
                        StartCoroutine(Dead(9, 5.0f));
                    }

                    
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
        healthBar.SetPosition(1, new Vector3(-0.5f + (currentHealth/health), 0.5f, -4.0f));
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
