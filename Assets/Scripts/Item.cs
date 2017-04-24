using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

    public Sprite[] sprites;

    public string itemType = "";
    public int capacity;
    public SpriteRenderer sr;

    void Start() {
        Sprite sprite = sprites[Random.Range(0, sprites.Length)];
        switch (sprite.name) {
            case "stones":
            case "stone_big":
                itemType = "stone";
                break;
            case "sticks":
            case "stick_big":
                itemType = "stick";
                break;
            case "leaf":
                itemType = "food_leaf";
                break;
            default:
                Debug.LogError("Unknown item spawned!");
                gameObject.SetActive(false);
                break;
        }

        sr = this.GetComponent<SpriteRenderer>();
        sr.sprite = sprite;
    }

    public string DigItem() {
        capacity--;
        if (capacity <= 0) {
            Destroy(gameObject);
        }
        return itemType;
    }
}
