using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    bool displayLine = true;
    LineRenderer line;
    public Vector2 direction;
    AntController ac;
    Transform head;

	void Start () {
        ac = FindObjectOfType<AntController>();
        GameObject lineGO = GameObject.Find("Direction Line");
        if (lineGO != null) {
            line = lineGO.GetComponent<LineRenderer>();
        }
        if (lineGO == null || line == null ) {
            Debug.LogError("Could not find line renderer");
        }
        line.gameObject.SetActive(true);
        line.SetPosition(0, new Vector3(transform.position.x, transform.position.y, transform.position.z - 1));
        line.SetPosition(1, new Vector3(transform.position.x, transform.position.y, transform.position.z - 1));
        line.enabled = true;

        head = transform.FindChild("Head");

        Time.timeScale = 0.0f;
	}
	
	void Update () {
        if (displayLine) {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = transform.position.z - 1;
            
            if ((line.GetPosition(0) - pos).magnitude > 1) {
                // TODO limit line length and angle(?)
            }
            line.SetPosition(1, pos);

            if (Input.GetMouseButtonDown(0)) {
                displayLine = false;
                line.enabled = false;

                // move head to direction, set it as target
                head.transform.position = (Vector2)(transform.position + 0.1f * (pos - transform.position).normalized);
                ac.AddNodeToPath(head.gameObject, gameObject);

                Time.timeScale = 1.0f;

            }
        }
	}

}
