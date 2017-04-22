using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GroundFloor : MonoBehaviour {

    public int width;
    public int height;

    public Color[] dirtColors;

    public SpriteRenderer dirtSprite;

    public GameObject nest;
    public List<GameObject> visibleConsumables;

	// Use this for initialization
	void Start () {
        visibleConsumables = new List<GameObject>();
        PaintDirt();
	}


    void PlaceNest() {
        // find spot, move
    }

    void PaintDirt() {
        Texture2D dirt = new Texture2D(width, height);
        dirt.filterMode = FilterMode.Point;
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                dirt.SetPixel(x, y, dirtColors[Random.Range(0, dirtColors.Length)]);
            }
        }
        dirt.Apply();
        dirtSprite.sprite = Sprite.Create(dirt, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
    }
	
}
