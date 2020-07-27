using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImportantVariables : MonoBehaviour
{
    
    public static ImportantVariables instance;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        DontDestroyOnLoad(gameObject);
    }

    #region General

    public bool hardMode {get; set;} = false;
    public int lowThreshold {get; set;} = 30;
    public int medThreshold {get; set;} = 55;
    public int hghThreshold {get; set;} = 85;

    #endregion


    #region Person

    public int fearInitialMin {get; set;} = 0;
    public int fearInitialMax {get; set;} = 20;

    public float fearMultiplierMin {get; set;} = 0.4f;
    public float fearMultiplierMax {get; set;} = 1.8f;


    public int sanityInitialMin {get; set;} = 0;
    public int sanityInitialMax {get; set;} = 10;

    public float sanityMultiplierMin {get; set;} = 0.8f;
    public float sanityMultiplierMax {get; set;} = 1.5f;


    public int stressInitialMin {get; set;} = 0;
    public int stressInitialMax {get; set;} = 30;

    public float stressMultiplierMin {get; set;} = 0.7f;
    public float stressMultiplierMax {get; set;} = 1.6f;


    // public int willingnessToKillFear {get; set;} = 70;
    // public int willingnessToKillSanity {get; set;} = 80;
    // public int willingnessToKillStress {get; set;} = 80;

    #endregion


    #region Psycho

    public int killIntentThreshold {get; set;} = 60;
    public float killControlChance {get; set;} = 0.25f;


    public int psychoFearInitialMin {get; set;} = 0;
    public int psychoFearInitialMax {get; set;} = 10;

    public float psychoFearMultiplierMin {get; set;} = 0.5f;
    public float psychoFearMultiplierMax {get; set;} = 0.7f;


    public int psychoSanityInitialMin {get; set;} = 12;
    public int psychoSanityInitialMax {get; set;} = 16;

    public float psychoSanityMultiplierMin {get; set;} = 1.2f;
    public float psychoSanityMultiplierMax {get; set;} = 1.4f;


    public int psychoStressInitialMin {get; set;} = 10;
    public int psychoStressInitialMax {get; set;} = 20;

    public float psychoStressMultiplierMin {get; set;} = 0.8f;
    public float psychoStressMultiplierMax {get; set;} = 1.1f;
    
    #endregion
}
