using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Person : MonoBehaviour // MB so that it can be a component
{
    
    public enum BehaviourState
    {
        low,medium,high,max // Max isn't actually max
    }
    #region Variables

    public bool alive = true;
    public int seatNumber = -1;

    //public List<string> roundDescriptions;


    #region Attributes of a Person
    public string personName;

    public BehaviourState fearState;
    protected int fear;
    protected float fearMultiplier;

    public BehaviourState stressState;
    protected int stress;
    protected float stressMultiplier;

    public BehaviourState sanityState;
    protected int sanity;
    protected float sanityMultiplier;


    #endregion

    #endregion

    void Start()
    {
        SetAttributes();

    }

    protected void SetAttributes()
    {
        SetName();

        fear = Random.Range(ImportantVariables.instance.fearInitialMin,ImportantVariables.instance.fearInitialMax);

        fearMultiplier = Random.Range(ImportantVariables.instance.fearMultiplierMin, ImportantVariables.instance.fearMultiplierMax);



        sanity = Random.Range(ImportantVariables.instance.sanityInitialMin,ImportantVariables.instance.sanityInitialMax);

        sanityMultiplier = Random.Range(ImportantVariables.instance.sanityMultiplierMin, ImportantVariables.instance.sanityMultiplierMax);



        stress = Random.Range(ImportantVariables.instance.stressInitialMin,ImportantVariables.instance.stressInitialMax);

        stressMultiplier = Random.Range(ImportantVariables.instance.stressMultiplierMin, ImportantVariables.instance.stressMultiplierMax);

    }

    protected void SetName()
    {
        //personName = "" + (char)Random.Range(65,123); // TODO

        personName = GenerateName(Random.Range(4,9));
    }


    #region Affectors

    public void AffectFear(float val)
    {
        fear += (int)(val * fearMultiplier - 0.1f * GroupManager.instance.CMAlive);

        fear = Mathf.Clamp(fear, 0, 100);
    }

    public void AffectSanity(float val)
    {
        sanity += (int)(val * sanityMultiplier - 0.1f * GroupManager.instance.CMAlive);

        sanity = Mathf.Clamp(sanity, 0, 100);
    }

    public void AffectStress(int val)
    {
        stress += (int)(val * stressMultiplier - 0.1f * GroupManager.instance.CMAlive);

        stress = Mathf.Clamp(stress, 0, 100);
    }


    #endregion

    public void MentalCheckup()
    {
        fearState = CalculateBehaviourState(fear);

        stressState = CalculateBehaviourState(stress);

        sanityState = CalculateBehaviourState(sanity);
    }

    

    void OnMouseEnter() // TODO Tooltip on hover
    {
        if(!GameManager.instance.isPaused && !GameManager.instance.gameEnded)
        {
            GetComponent<SpriteRenderer>().color = Color.grey;

            GameManager.instance.tooltipName.text = personName;
            GameManager.instance.tooltipBody.text = GetDescription();
        }
    }

    void OnMouseExit()
    {
            GetComponent<SpriteRenderer>().color = Color.white;

            GameManager.instance.tooltipName.text = "";
            GameManager.instance.tooltipBody.text = "";
    }

    public virtual void Kill(int who)
    {
        Debug.Log("Killing " + personName);

        // GroupManager.instance.KillPerson(this, who);
    }

    public bool WillingToKill()
    {
        if(fearState != BehaviourState.max)
            return false;
        if(sanityState != BehaviourState.max)
            return false;
        if(stressState != BehaviourState.max)
            return false;

        return true;
    }

    public void DrawCircle()
    {
        if(this is ControlMember)
            return;
        
        AudioManager.instance.Play("marker");

        transform.GetChild(0).gameObject.SetActive(!transform.GetChild(0).gameObject.activeSelf);
    }

    public void DrawCircle(bool on)
    {
        if(this is ControlMember)
            return;

        AudioManager.instance.Play("marker");

        transform.GetChild(0).gameObject.SetActive(on);
    }

    #region Helper

    public virtual string GetDescription()
    {
        if(!alive)
        {
            return "Dead.";
        }

        // Yes this is a stupid way to do it but I also don't want to think rn

        


        
        return "Feeling " + BehaviourStateDetailed(0) + ", " + BehaviourStateDetailed(1) + ", and " + BehaviourStateDetailed(2) + ".";
    }

    protected string BehaviourStateDetailed(int att) // 0 f, 1 sa, 2 st
    {
        string str = "";
        
        if(att == 0)
        {
            switch(fearState)
            {
                case BehaviourState.low:
                    str = "confident";
                    break;
                case BehaviourState.medium:
                    str = "nervous";
                    break;
                case BehaviourState.high:
                    str = "afraid";
                    break;
                case BehaviourState.max:
                    str = "horrified";
                    break;
                
                default:
                    str = "DEFAULT CASE";
                    break;
            }
        }
        
        else if(att == 1)
        {
            switch(sanityState)
            {
                case BehaviourState.low:
                    str = "sane";
                    break;
                case BehaviourState.medium:
                    str = "incoherent";
                    break;
                case BehaviourState.high:
                    str = "irrational";
                    break;
                case BehaviourState.max:
                    str = "chaotic";
                    break;
                
                default:
                    str = "DEFAULT CASE";
                    break;
            }
        }

        else if(att == 2)
        {
            switch(stressState)
            {
                case BehaviourState.low:
                    str = "calm";
                    break;
                case BehaviourState.medium:
                    str = "anxious";
                    break;
                case BehaviourState.high:
                    str = "stressed";
                    break;
                case BehaviourState.max:
                    str = "is having a panic attack";
                    break;
                
                default:
                    str = "DEFAULT CASE";
                    break;
            }
        }



        return str;
    }

    private BehaviourState CalculateBehaviourState(int val)
    {
        if(val < ImportantVariables.instance.lowThreshold)
        {
            return BehaviourState.low;
        }
        else if(val < ImportantVariables.instance.medThreshold)
        {
            return BehaviourState.medium;
        }
        else if(val < ImportantVariables.instance.hghThreshold)
        {
            return BehaviourState.high;
        }

        return BehaviourState.max;
    }
    protected string GenerateName(int len)
    { 
        System.Random r = new System.Random(Random.Range(0,int.MaxValue));
        string[] consonants = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "l", "n", "p", "q", "r", "s", /*"sh", "zh",*/ "t", "v", "w", /*"x"*/ };
        string[] vowels = { "a", "e", "i", "o", "u", "ae", "y" };
        string Name = "";
        Name += consonants[r.Next(consonants.Length)].ToUpper();
        Name += vowels[r.Next(vowels.Length)];
        int b = 2; //b tells how many times a new letter has been added. It's 2 right now because the first two letters are already in the name.
        while (b < len)
        {
            Name += consonants[r.Next(consonants.Length)];
            b++;
            Name += vowels[r.Next(vowels.Length)];
            b++;
        }

        return Name;


     }

    #endregion
}
