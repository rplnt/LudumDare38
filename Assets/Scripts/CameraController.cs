using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    [Header("Zoom")]
    public float minZoom;
    float maxZoom = 5.0f;
    public float defaultZoom;

    [Header("Movement")]
    [Range(0.5f, 5f)]
    public float movementScale;
    [Range(0, 50)]
    public int mouseBorders;

    float hBorder, vBorder;

    Camera cam;
    Bounds bounds;


	void Start () {
        cam = this.GetComponent<Camera>();
        if (cam == null) {
            cam = Camera.main;
        }

        cam.orthographicSize = defaultZoom;

        GameObject dirt = GameObject.Find("Dirt Renderer");
        if (dirt != null) {
            bounds = dirt.GetComponent<SpriteRenderer>().bounds;
        }

        hBorder = Screen.width * mouseBorders / 100.0f;
        vBorder = Screen.height * mouseBorders / 100.0f;
	}
	

	void Update () {
        Zoom();
        MoveAndClamp();
	}


    void MoveAndClamp() {
        /* keyboard */
        Vector2 moved = new Vector2(cam.transform.position.x + Time.deltaTime * Input.GetAxis("Horizontal") * movementScale, cam.transform.position.y + Time.deltaTime * Input.GetAxis("Vertical") * movementScale);

        /* mouse */
        if (Input.mousePosition.x >= 0 && Input.mousePosition.x < hBorder) {
            moved -= new Vector2(((hBorder - Input.mousePosition.x) / hBorder) * 5.0f * Time.deltaTime, 0.0f);
        } else if (Input.mousePosition.x <= Screen.width && Input.mousePosition.x > Screen.width - hBorder) {
            moved += new Vector2(((hBorder - (Screen.width - Input.mousePosition.x)) / hBorder) * 5.0f * Time.deltaTime, 0.0f);
        }
        if (Input.mousePosition.y >= 0 && Input.mousePosition.y < vBorder) {
            moved -= new Vector2(0.0f, ((vBorder - Input.mousePosition.y) / vBorder) * 5.0f * Time.deltaTime);
        } else if (Input.mousePosition.y <= Screen.height && Input.mousePosition.y > Screen.height - vBorder) {
            moved += new Vector2(0.0f, ((vBorder - (Screen.height - Input.mousePosition.y)) / vBorder) * 5.0f * Time.deltaTime);
        }

        cam.transform.position = new Vector3(
            Mathf.Clamp(moved.x, bounds.min.x + cam.orthographicSize * cam.aspect, bounds.max.x - cam.orthographicSize * cam.aspect),
            Mathf.Clamp(moved.y, bounds.min.y + cam.orthographicSize, bounds.max.y - cam.orthographicSize),
            -10.0f);
    }


    void Zoom() {
        /* mouse wheel */
        float zoomBy = -Input.GetAxis("Mouse ScrollWheel");

        /* keyboard */
        if (Input.GetKey(KeyCode.Plus) || Input.GetKey(KeyCode.KeypadPlus)) {
            zoomBy = -0.1f;
        }
        if (Input.GetKey(KeyCode.Minus) || Input.GetKey(KeyCode.KeypadMinus)) {
            zoomBy = 0.1f;
        }

        // TODO non-linear zoom
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize + zoomBy, minZoom, maxZoom);

        /* reset */
        if (Input.GetKeyDown(KeyCode.KeypadMultiply)) {
            cam.orthographicSize = defaultZoom;
        }

        
    }
}
