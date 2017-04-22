using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    bool displayLine = true;
    public LineRenderer line;
    public Vector2 direction;

	void Start () {
        line.gameObject.SetActive(true);
        line.SetPosition(0, new Vector3(transform.position.x, transform.position.y, transform.position.z - 1));
        line.SetPosition(1, new Vector3(transform.position.x, transform.position.y, transform.position.z - 1));
        line.enabled = true;
	}
	
	void Update () {
        if (displayLine) {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = transform.position.z - 1;
            
            if ((line.GetPosition(0) - pos).magnitude > 1) {
                // TODO limit line length    
            }
            line.SetPosition(1, pos);

            if (Input.GetMouseButtonDown(0)) {
                // move head to direction, set it as target
                displayLine = false;
                line.enabled = false;
            }
        }
	}
}
