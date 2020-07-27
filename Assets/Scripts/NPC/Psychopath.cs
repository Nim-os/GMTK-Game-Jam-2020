using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Psychopath : Person
{
    private int killIntent = 1;

    void Start()
    {
        SetName();
    
        fear = Random.Range(ImportantVariables.instance.psychoFearInitialMin,ImportantVariables.instance.psychoFearInitialMax);

        fearMultiplier = Random.Range(ImportantVariables.instance.psychoFearMultiplierMin, ImportantVariables.instance.psychoFearMultiplierMax);



        sanity = Random.Range(ImportantVariables.instance.psychoSanityInitialMin,ImportantVariables.instance.psychoSanityInitialMax);

        sanityMultiplier = Random.Range(ImportantVariables.instance.psychoSanityMultiplierMin, ImportantVariables.instance.psychoSanityMultiplierMax);



        stress = Random.Range(ImportantVariables.instance.psychoStressInitialMin,ImportantVariables.instance.psychoStressInitialMax);

        stressMultiplier = Random.Range(ImportantVariables.instance.psychoStressMultiplierMin, ImportantVariables.instance.psychoStressMultiplierMax);

    }

    public void AttemptKill()
    {
        if(killIntent + 5 * GroupManager.instance.CMRound + 15 * GroupManager.instance.CMAlive > ImportantVariables.instance.killIntentThreshold)
        {
            killIntent = killIntent/3;
            if(killIntent > 100)
                killIntent = 50;

            Person deadMan = null;

            if(Random.Range(0f,1.0f) > 1 - ImportantVariables.instance.killControlChance 
            + GroupManager.instance.CMRound * 0.01f 
            + GroupManager.instance.CMAlive * 0.035f) // 25% base chance + 1% per addition this round (decay) + 4% per control members alive
            {
                deadMan = TryingToKillControl();
            }
            
            if(deadMan == null)
            {
                deadMan = TryingToKillPerson(0);
            }

            if(deadMan == null)
            {
                Debug.Log("Psycho didn't kill anyone..");
                AffectStress(3);
                AffectKillIntent(15);
                return;
            }

            GroupManager.instance.KillPerson(deadMan, 1);

            AffectStress(-5);
        }

        AffectKillIntent(1);
    }

    private Person TryingToKillPerson(int depth=0)
    {
        if(depth > 100)
            return null;
        
        Person p = GroupManager.instance.group[Random.Range(1,GroupManager.instance.groupSize)];

        if(p == null || !p.alive) // Find someone else if null or not alive
        {
            return TryingToKillPerson(depth + 1);

        }

        return p;
    }

    private Person TryingToKillControl()
    {
        int index = GroupManager.instance.groupSize;

        Person p = null;

        for(; index < GroupManager.instance.group.Length; index++)
        {
            p = GroupManager.instance.group[index];
            if(p == null)
            {
                return null;
            }
            else if(p.alive)
            {
                return p;
            }
        }

        return p;
    }

    public void AffectKillIntent(int val)
    {
        killIntent += (int)(val + stress/3 + fear/2);

        killIntent = Mathf.Clamp(killIntent, 0, 100);
    }

    public override void Kill(int who)
    {
        if(who != 0)
            return;

        //GetComponent<SpriteRenderer>().color = Color.red;

        GetComponent<SpriteRenderer>().sprite = GroupManager.instance.rightAnswer;
        transform.localScale = new Vector3(1.5f,1.5f,1.5f);

        DrawCircle(false);

        GameManager.instance.DecideGame(true);
    }
}
