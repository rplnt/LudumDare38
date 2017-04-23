using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour {
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            // TODO PAUSE MENU
        }
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);

            if (hit.collider != null) {
                if (hit.transform.CompareTag("Slot")) {
                    /* clicked on slot */
                    Nest nest = hit.transform.GetComponentInParent<Nest>();
                    if (nest != null) {
                        nest.CreateSpawner(hit.transform.gameObject);
                    }
                } else if (hit.transform.CompareTag("Head")) {

                }
            }
        }
	}
}