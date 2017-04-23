using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffsiteManager : MonoBehaviour {

    public LineRenderer healthBar;

    public int antCount;

    void Start() {
        UpdateHealthBar();
    }

    public bool NestAnt() {
        Debug.Log("NEST");
        if (antCount >= Level.limits[0]) {
            Debug.Log("FULL");
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
}
