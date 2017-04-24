using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class GroundFloor : MonoBehaviour {

    public int width;
    public int height;
    Texture2D dirt;
    public Bounds gameBounds;
    float worldSizeX;
    float worldSizeY;

    public GameObject itemPrefab;
    public int itemCount;

    public SpriteRenderer dirtSprite;
    public Color[] dirtColors;

    public int patchSize;

    AntController ac;

	// Use this for initialization
	void Start () {
        PaintDirt();

        gameBounds = new Bounds(new Vector3(dirtSprite.bounds.center.x, dirtSprite.bounds.center.y - 0.5f, dirtSprite.bounds.center.z), new Vector3(dirtSprite.bounds.size.x - 1.0f, dirtSprite.bounds.size.y - 1.5f, 1.0f));
        worldSizeX = dirtSprite.bounds.max.x - dirtSprite.bounds.min.x;
        worldSizeY = dirtSprite.bounds.max.y - dirtSprite.bounds.min.y;
     

        ac = FindObjectOfType<AntController>();
        ac.dig += Dig;

        SpawnItems(gameBounds);
	}


    void SpawnItems(Bounds bounds) {
        for (int i = 0; i < itemCount; i++) {
            Instantiate(itemPrefab, new Vector2(Random.Range(bounds.min.x, bounds.max.x), Random.Range(bounds.min.y, bounds.max.y)), Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)), transform);
        }
    }


    void Dig(Vector2 position) {
        int x = Mathf.RoundToInt(width * ((position.x - dirtSprite.bounds.min.x) / worldSizeX));
        int y = Mathf.RoundToInt(height * ((position.y - dirtSprite.bounds.min.y) / worldSizeY));

        if (x - patchSize < 0 || x + patchSize > dirt.width || y - patchSize < 0 || y + patchSize > height) {
            return;
        }

        for (int b = -patchSize/2; b < patchSize/2; b++) {
            for (int a = -patchSize/2; a < patchSize/2; a++) {
                Color pixel = dirt.GetPixel(x + a, y + b);
                float dimm = 1.07f - Mathf.Clamp(((Mathf.Abs(a) + Mathf.Abs(b) - 1)/(patchSize * 1.0f))/10.0f, 0.0f, 0.07f);
                dirt.SetPixel(x + a, y + b, new Color(pixel.r * dimm, pixel.g * dimm, pixel.b * dimm));
            }
            
        }
        //dirt.SetPixels(x - (patchSize / 2), y - (patchSize / 2), patchSize, patchSize, patch);
        dirt.Apply();
    }

    void PaintDirt() {
        dirt = new Texture2D(width, height);
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
