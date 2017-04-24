using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffsiteManager : MonoBehaviour {

    public LineRenderer healthBar;
    public System.Action NestDestroyed;

    public int antCount;

    void Start() {
        UpdateHealthBar();
    }

    public bool NestAnt() {
        if (antCount >= Level.limits[0]) {
            return false;
        }

        antCount++;
        UpdateHealthBar();
        return true;
    }

    void UpdateHealthBar() {
        if (healthBar == null) return;

        float ratio = (float)antCount / Level.limits[0];
        healthBar.SetPosition(1, new Vector3(-0.5f + ratio, 0.35f, -4.0f));
    }


    public float Attack() {
        antCount--;
        if (antCount <= 0) {
            if (NestDestroyed != null) {
                NestDestroyed();
            }

            Destroy(gameObject);
            return -1.0f;
        }
        UpdateHealthBar();
        return Mathf.Max(0.5f * antCount, 0.0f);
    }
}
