using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ant {
    Vector2 _target;
    public GameObject go;

    public Ant(GameObject _go, float health) {
        go = _go;
        Health = health;
    }
    
    public Vector3 NextTarget {
        get {
            return _target;
        }
    }

    public void SetTarget(GameObject go) {
        _target = (Vector2)go.transform.position + (Random.insideUnitCircle / 10.0f);
    }

    public float Health { get; protected set; }

    public void TakeDamage(float damage) {
        // TODO death;
        Health -= damage;
    }
}
