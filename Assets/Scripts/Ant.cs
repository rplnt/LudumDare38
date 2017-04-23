using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ant {
    public GameObject go;
    public Node NextTarget;
    public bool homing = false;
    public float speed {
        get {
            return 1.0f * ((carrying == null)?1.0f:0.7f);
        }
    }
    public string carrying = null;
    Vector2 offset;

    public Vector3 NextTargetPosition {
        get {
            return NextTarget.go.transform.position + (Vector3)offset;
        }
    }

    public float Health { get; protected set; }

    public Ant(GameObject _go, float health) {
        go = _go;
        Health = health;
        offset = Random.insideUnitCircle / 10.0f;
    }

    public void SetTarget(Node node) {
        NextTarget = node;
    }

    public void TakeDamage(float damage) {
        // TODO death;
        Health -= damage;
    }

    
}
