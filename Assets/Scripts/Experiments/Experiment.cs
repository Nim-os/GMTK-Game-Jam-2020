using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="New Experiment")]
public class Experiment : ScriptableObject
{
    public string experimentName;
    public Sprite artwork;

    [Space]
    public string desc;

    [Space]
    public int fearIncrease;
    public int sanityIncrease;
    public int stressIncrease;
    
}
