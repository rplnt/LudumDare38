using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct Consumables {
    public int food;
    public int dirt;
    public int stones;
    public int sticks;


    public Consumables(int food, int dirt, int sticks, int stones) {
        this.food = food;
        this.dirt = dirt;
        this.sticks = sticks;
        this.stones = stones;
    }


    public override string ToString() {
        return base.ToString() + "[food: " + food + " dirt: " + dirt + " sticks: "+ sticks + " stones: " + stones +"]";
    }


    public bool IsEnough(Consumables cost) {
        return this.food >= cost.food && this.dirt >= cost.dirt && this.sticks >= cost.sticks && this.stones >= cost.stones;
    }

    public bool Consume(Consumables cost) {
        if (!IsEnough(cost)) return false;
        this.food -= cost.food;
        this.dirt -= cost.dirt;
        this.sticks -= cost.sticks;
        this.stones -= cost.stones;

        return true;
    }

    public void Add(string item) {
        if (item == null) return;
        string[] prefix = item.Split('_');
        switch (prefix[0]) {
            case "food": food += 2; break;
            case "dirt": dirt += 1; break;
            case "stick": sticks += 1; break;
            case "stone": stones += 1; break;
        }
    }
}


public static class Level {

    public static float digSpeed = 0.045f;

    public static int maxLevel = 6;

    public static readonly Consumables[] costs = {
        new Consumables(0, 5, 5, 15),
        new Consumables(0, 40, 12, 10),
        new Consumables(0, 60, 25, 15),
        new Consumables(0, 100, 45, 25),
        new Consumables(0, 160, 80, 40),
        new Consumables(0, 280, 120, 60),
        new Consumables(0, 400, 190, 90)
    };

    public static readonly int[] limits = {
        20, 50, 90, 140, 200, 280, 400
    };

    public static readonly float[] offsiteLevelsModifier = {
        1.0f, 1.2f, 1.5f, 2.0f, 2.7f, 3.5f, 4.5f
    };

    public static readonly float[] releaseDelay = {
        0.7f, 0.4f, 0.3f, 0.25f, 0.12f, 0.07f, 0.04f
    };

    public static float singleReleaseDelay = 0.23f;

    public static readonly float[] birthDelay = {
        2.2f, 1.8f, 1.4f, 1.1f, 0.8f, 0.6f, 0.4f
    };




}
