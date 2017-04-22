using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AntController : MonoBehaviour {

    [Header("Ant Properties")]
    public GameObject prefab;
    public float speed;
    private float randomSpeed = 0.1f;
    public float rotateSpeed;
    public float health;
    public float damage;

    List<Ant> ants;
    int Total {
        get {
            return Nested + Spawned;
        }
    }
    int Nested;
    int Spawned {
        get {
            return ants.Count; 
        }
    }

	// Use this for initialization
	void Start () {
        ants = new List<Ant>();

        //for (int i = 0; i < 100; i++) {

        //    Ant larva = new Ant(Instantiate(prefab, Vector2.zero, Quaternion.identity), health * Random.Range(0.9f, 1.1f));
        //    ants.Add(larva);
        //}
	}
	

	void Update () {

        foreach (Ant ant in ants) {
            /* calculate rotation */
            Vector3 targetVector = ant.NextTarget - ant.go.transform.position;
            float angle = Mathf.Atan2(targetVector.y, targetVector.x) * Mathf.Rad2Deg - 90;
            Quaternion targetRotation = Quaternion.RotateTowards(ant.go.transform.rotation, Quaternion.AngleAxis(angle, Vector3.forward), rotateSpeed * Time.deltaTime);

            /* rotate or move */
            if (ant.go.transform.rotation != targetRotation) {
                ant.go.transform.rotation = targetRotation;
            } else {
                ant.go.transform.position = Vector2.MoveTowards(ant.go.transform.position, ant.NextTarget, speed * (ant.Health / health) * Time.deltaTime);
            }

            /* TEST set new target */
            if (ant.go.transform.position == ant.NextTarget) {
                //ant.SetTarget(points[Random.Range(0, points.Length)]);
            }
        }
	}

}
