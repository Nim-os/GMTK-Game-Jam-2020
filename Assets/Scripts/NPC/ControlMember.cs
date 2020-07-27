using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlMember : Person
{

    void Awake()
    {
        SetAttributes();

        //GetComponent<SpriteRenderer>().color = Color.cyan;
    }
    
    
    public override string GetDescription()
    {
        if(!alive)
        {
            return "Dead.";
        }

        return "Feeling " + fear + "% scared (" + BehaviourStateDetailed(0) + "), " 
        + (100-sanity) + "% sane (" + BehaviourStateDetailed(1) + "), and " 
        + stress + "% stressed ("+ BehaviourStateDetailed(2) + ").";
    }

    public override void Kill(int who)
    { // TODO Only die if psychopath kills them
        if(who == 0)
        {
            Debug.Log("Can't kill a Control Member!");
            return;
        }
        
        GroupManager.instance.KillPerson(this, who);
    }
}
