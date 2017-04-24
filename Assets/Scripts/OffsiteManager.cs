using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffsiteManager : MonoBehaviour {

    public LineRenderer healthBar;
    public System.Action NestDestroyed;
    int offsiteLevel;
    Nest nest;

    public int nestedAntCount;

    void Start() {
        //Debug.Log("OffsiteManager:Start");
        UpdateHealthBar();
        nest = FindObjectOfType<Nest>();
        offsiteLevel = nest.currentLevel;

        for (int i = 1; i <= offsiteLevel; i++) {
            Transform orb = transform.FindChild("Orbs/Orb " + i);
            if (orb != null) {
                orb.gameObject.SetActive(true);
            }
        }

        //if (offsiteLevel > 0) {

        //    Transform orbs = transform.FindChild("Orbs");
        //    if (orbs != null) {
        //        foreach (Transform orb in orbs) {
                    
        //        }
        //    }
        //}
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
        damage = Mathf.Min(damage, nestedAntCount);
        float returnDamage = Mathf.Max(nestedAntCount * 0.75f, 0.0f);
        nestedAntCount = nestedAntCount - damage;
        nest.KilledOffsiteAnts(damage);
        
        if (nestedAntCount <= 0) {
            if (NestDestroyed != null) {
                NestDestroyed();
            }

            Destroy(gameObject);
            return (float)Level.offsiteLevelsModifier[offsiteLevel] * damage;
        }
        UpdateHealthBar();

        return (float)Level.offsiteLevelsModifier[offsiteLevel] * returnDamage;
    }
}
