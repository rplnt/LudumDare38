using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputController : MonoBehaviour {

    Nest nest;

    public Sprite nestOpened;
    public Sprite nestClosed;

    void Start() {
        nest = FindObjectOfType<Nest>();
    }

	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);

            if (hit.collider != null) {
                if (EventSystem.current.IsPointerOverGameObject()) return;

                if (hit.transform.CompareTag("Slot")) {
                    /* clicked on slot */
                    Nest nest = hit.transform.GetComponentInParent<Nest>();
                    if (nest != null) {
                        nest.CreateSpawner(hit.transform.gameObject);
                    }
                } else if (hit.transform.CompareTag("Head")) {
                    //Debug.Log("InputController:Update:Head");
                    Head head = hit.transform.gameObject.GetComponent<Head>();
                    if (!head) return;
                    if (head.snooze > Time.time) return;

                    //head.clicked = true;

                    nest.BuildOffsite(head);
                } else if (hit.transform.CompareTag("Spawner Control")) {
                    OpenClose(hit.transform.gameObject);
                }
            }
        }
	}


    void OpenClose(GameObject go) {
        SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
        if (sr == null) return;
        if (sr.sprite.name == "nest_opened") {
            nest.CloseSpawner(go.transform.parent);
            sr.sprite = nestClosed;
            return;
        } else if (sr.sprite.name == "nest_closed") {
            nest.OpenSpawner(go.transform.parent);
            sr.sprite = nestOpened;
            return;
        }

        Debug.LogError("Not a spawner");
    }

}