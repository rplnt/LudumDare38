using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Head : MonoBehaviour {

    bool displayLine = true;
    SpriteRenderer sr;
    ParticleSystem ps;
    LineRenderer line;
    AntController ac;
    Nest nest;

    public GameObject headPrefab;

    GameObject parent;

    float snooze;

    void Start() {
        ac = FindObjectOfType<AntController>();
        nest = FindObjectOfType<Nest>();

        sr = GetComponent<SpriteRenderer>();
        sr.enabled = false;
        sr.color = new Color(1, 1, 1, 0.0f);

        ps = transform.GetComponentInChildren<ParticleSystem>();

        snooze = Time.time + 3.0f;
        Time.timeScale = 0.0f;


        SetupLine();
    }

    void Update() {
        if (displayLine) {
            DirectHead();
        }
    }


    void SetupLine() {
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
    }


    void DirectHead() {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = transform.position.z - 1;
            
            line.SetPosition(1, pos);

            if (Input.GetMouseButtonDown(0)) {
                displayLine = false;
                line.enabled = false;

                // move head to direction, set it as target
                transform.position = (Vector2)(transform.position + 0.1f * (pos - transform.position).normalized);

                // TODO
                ac.AddNodeToPath(gameObject, transform.parent.gameObject);

                /* first head, notify nest */
                if (transform.parent.CompareTag("Spawner")) {
                    nest.OpenSpawner(transform.parent);
                }

                Time.timeScale = 1.0f;
            }
    }

    public void SpawnNewHead() {
        GameObject headGo = Instantiate(headPrefab, transform.position, Quaternion.identity, transform);
        Destroy(sr);
        Destroy(ps);
        Destroy(this);
        //this.enabled = false;

        //ac.AddNodeToPath(headGo, gameObject);
    }

    void OnMouseEnter() {
        if (snooze > Time.time) return;
        StartCoroutine(ShowSprite(sr.color.a, 1.0f, 0.2f));
    }

    void OnMouseExit() {
        StartCoroutine(HideSprite(sr.color.a, 0.0f, 0.2f));
    }

    IEnumerator ShowSprite(float startAlpha, float endAlpha, float duration) {
        Color c = sr.color;
        float elapsed = 0.0f;
        c.a = startAlpha;
        sr.color = c;
        sr.enabled = true;
        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(startAlpha, endAlpha, elapsed/duration);
            sr.color = c;
            yield return null;
        }
    }

    IEnumerator HideSprite(float startAlpha, float endAlpha, float duration) {
        Color c = sr.color;
        float elapsed = 0.0f;
        c.a = startAlpha;
        sr.color = c;
        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            sr.color = c;
            yield return null;
        }

        sr.enabled = false;
    }
    
}
