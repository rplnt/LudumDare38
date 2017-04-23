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
        return this.food > cost.food && this.dirt > cost.dirt && this.sticks > cost.sticks && this.stones > cost.stones;
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
        switch (item) {
            case "food": food += 1; break;
            case "dirt": dirt += 1; break;
            case "stick": sticks += 1; break;
            case "stone": stones += 1; break;
        }
    }


}


public static class Level {

    public static float digSpeed = 0.025f;

    public static int maxLevel = 4;

    public static readonly Consumables[] costs = {
        new Consumables(0, 10, 5, 5),
        new Consumables(0, 1, 2, 3),
        new Consumables(0, 1, 2, 3),
        new Consumables(0, 1, 2, 3),
        new Consumables(0, 1, 2, 3)
    };

    public static readonly int[] limits = {
        20, 50, 80, 120, 170
    };

    public static readonly float[] releaseDelay = {
        0.7f, 0.5f, 0.4f, 0.2f, 0.1f
    };

    public static readonly float[] birthDelay = {
        3.5f, 3.0f, 2.0f, 1.0f, 0.8f
    };




}
