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
            case "food": food += 1; break;
            case "dirt": dirt += 1; break;
            case "stick": sticks += 1; break;
            case "stone": stones += 1; break;
        }
    }


}


public static class Level {

    public static float digSpeed = 0.04f;

    public static int maxLevel = 6;

    public static readonly Consumables[] costs = {
        new Consumables(0, 30, 20, 50),
        new Consumables(0, 50, 15, 10),
        new Consumables(0, 90, 30, 20),
        new Consumables(0, 150, 50, 30),
        new Consumables(0, 250, 90, 50),
        new Consumables(0, 400, 120, 100),
        new Consumables(0, 800, 150, 120)
    };

    public static readonly int[] limits = {
        20, 50, 80, 120, 170, 200, 250
    };

    public static readonly float[] releaseDelay = {
        0.6f, 0.3f, 0.15f, 0.1f, 0.05f, 0.03f, 0.02f
    };

    public static readonly float[] birthDelay = {
        3.5f, 3.0f, 2.0f, 1.0f, 0.5f, 0.3f, 0.02f
    };




}
