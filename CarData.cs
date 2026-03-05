using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CarData
{
    public string[] Colors_ = new string[3];
    public float[] MetallicValues_ = new float[3];
    public float[] GlossyValues_ = new float[3];
    
    public int boost_type;
    public int tyreCompound;
    public int tune;
    public int weightReduction;
    public int suspension;

    public int[] ownedBoost = new int[5];
    public int[] ownedTyreCompound = new int[4];
    public int[] ownedTune = new int[4];
    public int[] ownedWeightReduction = new int[4];
    public int[] ownedSuspension = new int[4];

    public float[] susAdders = new float[4];


    public CarData(CarStats carStats) {
        Colors_[0] = carStats.Colors[0];
        Colors_[1] = carStats.Colors[1];
        Colors_[2] = carStats.Colors[2];

        MetallicValues_[0] = carStats.MetallicValues[0];
        MetallicValues_[1] = carStats.MetallicValues[1];
        MetallicValues_[2] = carStats.MetallicValues[2];

        GlossyValues_[0] = carStats.GlossyValues[0];
        GlossyValues_[1] = carStats.GlossyValues[1];
        GlossyValues_[2] = carStats.GlossyValues[2];

        boost_type = carStats.boost_type;
        tyreCompound = carStats.tyreCompound;
        tune = carStats.tune;
        weightReduction = carStats.weightReduction;
        suspension = carStats.suspension;

        ownedBoost = carStats.ownedBoost;
        ownedTyreCompound = carStats.ownedTyreCompound;
        ownedTune = carStats.ownedTune;
        ownedWeightReduction = carStats.ownedWeightReduction;
        ownedSuspension = carStats.ownedSuspension;
        
        susAdders = carStats.susHeightsAdder;
    }
}
