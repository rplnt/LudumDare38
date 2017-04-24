using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffsiteManager : MonoBehaviour {

    public LineRenderer healthBar;
    public System.Action NestDestroyed;
    Nest nest;

    public int nestedAntCount;

    void Start() {
        //Debug.Log("OffsiteManager:Start");
        UpdateHealthBar();
        nest = FindObjectOfType<Nest>();
    }

    public bool NestAnt() {
        if (nestedAntCount >= Level.limits[0]) {
            return false;
        }

        nestedAntCount++;
        UpdateHealthBar();
        return true;
    }

    void UpdateHealthBar() {
        if (healthBar == null) return;

        float ratio = (float)nestedAntCount / Level.limits[0];
        healthBar.SetPosition(1, new Vector3(-0.5f + ratio, 0.35f, -4.0f));
    }


    public float AttackOffsite(int damage) {
        damage = Mathf.Min(damage * 2, nestedAntCount);

        nestedAntCount = nestedAntCount - damage;
        nest.KilledOffsiteAnts(damage);
        
        if (nestedAntCount <= 0) {
            if (NestDestroyed != null) {
                NestDestroyed();
            }

            Destroy(gameObject);
            return 1f;
        }
        UpdateHealthBar();
        //Debug.Log(nestedAntCount * 0.75f);
        return Mathf.Max(nestedAntCount * 0.75f, 0.0f);
    }
}
