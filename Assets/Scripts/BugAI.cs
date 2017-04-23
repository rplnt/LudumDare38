﻿using System.Collections;
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

    Nest ns;

    
	// Use this for initialization
	void Start () {
        currentHealth = maxHealthBase;
        UpdateTargets();
        graphics = transform.FindChild("Graphics");
        sr = graphics.GetComponent<SpriteRenderer>();
        sr.color = colors[Random.Range(0, colors.Length)];
        ns = FindObjectOfType<Nest>();
	}
	
	// Update is called once per frame
	void Update () {
        if (dead) {
            Color c = sr.color;
            c.a -= 0.1f * Time.deltaTime;
            if (c.a < 0.1f) {
                Destroy(gameObject);
            }
            Debug.Log(c);
            sr.color = c;

            return;
        }

        if (currentTarget == null) {
            Debug.LogError("BugAI: No target!");
            return;
        }

        Vector3 targetVector = currentTarget.transform.position - transform.position;

        if (targetVector.magnitude < 0.1f) {
            // ATTACK
            if (Time.time > lastAttack + attackDelay) {
                lastAttack = Time.time;
                currentHealth -= ns.Attack(currentTarget.transform.parent);
                UpdateHealthBar();
                if (currentHealth < 0.0f) {
                    /* DEATH */
                    Animator anim = gameObject.GetComponentInChildren<Animator>();
                    if (anim != null) {
                        anim.enabled = false;
                    }
                    healthBar.enabled = false;
                    sr.color = Color.white;
                    dead = true;
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
