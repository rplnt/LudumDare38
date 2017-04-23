using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public Text ants;
    public Text food;
    public Text dirt;
    public Text stones;
    public Text sticks;

    Nest nest;

	// Use this for initialization
	void Start () {
        nest = FindObjectOfType<Nest>();
        nest.resourcesUpdated += UpdateResources;
        nest.antCountChanged += UpdateAnts;
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    void UpdateAnts(int count) {
        ants.text = count.ToString();
    }



    void UpdateResources(Consumables c) {
        food.text = c.food.ToString();
        dirt.text = c.dirt.ToString();
        stones.text = c.stones.ToString();
        sticks.text = c.sticks.ToString();
    }
}