using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    [Header("Header")]
    public Text ants;
    public Text food;
    public Text dirt;
    public Text stones;
    public Text sticks;
    public Text nested;

    [Header("Upgrade")]
    public Text dirtUpgradeCost;
    public Text sticksUpgradeCost;
    public Text stonesUpgradeCost;
    public Text buttonText;

    Nest nest;

	// Use this for initialization
	void Start () {
        nest = FindObjectOfType<Nest>();
        nest.resourcesUpdated += UpdateResources;
        nest.antCountChanged += UpdateAnts;
        nest.antNestedCountChanged += UpdateNested;
        nest.enoughResourcesToUpgrade += EnoughResources;
        nest.leveledUp += LevelUp;

        UpdateLevelCost();
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    void UpdateAnts(int count) {
        ants.text = count.ToString();
    }

    void UpdateNested(int count) {
        nested.text = count.ToString();
    }

    void UpdateResources(Consumables c) {
        food.text = c.food.ToString();
        dirt.text = c.dirt.ToString();
        stones.text = c.stones.ToString();
        sticks.text = c.sticks.ToString();
    }

    void EnoughResources(bool enough) {
        if (enough) {
            buttonText.text = "LEVEL UP!";
            buttonText.color = Color.red;
        } else {
            LevelUp();
        }
    }

    void UpdateLevelCost() {
        dirtUpgradeCost.text = Level.costs[nest.currentLevel + 1].dirt.ToString();
        sticksUpgradeCost.text = Level.costs[nest.currentLevel + 1].sticks.ToString();
        stonesUpgradeCost.text = Level.costs[nest.currentLevel + 1].stones.ToString();
    }

    void LevelUp() {
        buttonText.text = "Level " + (nest.currentLevel + 1).ToString();
        buttonText.color = Color.black;
        UpdateLevelCost();
    }
}