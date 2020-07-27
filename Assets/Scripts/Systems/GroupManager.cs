using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupManager : MonoBehaviour
{
    #region Variables
    public static GroupManager instance;

    [Header("References")]
    public GameObject personPrefab;
    public GameObject controlMemberPrefab;
    public GameObject chairPrefab;
    public Sprite chair;
    public Sprite blood;
    public Sprite note;
    public Sprite rightAnswer;
    public Experiment psychopathExperiment;

    public UnityEngine.UI.Text controlText;


    [Header("Variables")]
    public Person[] group; // Array of the Person components
    public int groupSize;
    public Psychopath psychopath; // Reference to Psychopath

    public GameObject[] seats; // Array of the GameObjects of the people // Really needed???
    
    
    public int controlMembersLeft = 0;
    public int numDead = 0;
    
    public float spacing;

    public int[] occupiedSeats; // Array of the seats that have someone in them already

    public int CMAlive = 0; // CM still alive
    public int CMRound = 0; // How many control members added this round
    private GameObject[] chairs;

    #endregion

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }

        #region Allocation

        groupSize = 15;

        controlMembersLeft = (int)(groupSize * 0.33f); // Always have one less than 1/3 of the participants

        int numSeats = groupSize + controlMembersLeft + 1;

        group = new Person[numSeats]; // Do we really need a whole list for all the Person scripts? yeeeeeeees
        seats = new GameObject[numSeats];

        occupiedSeats = new int[numSeats];
        chairs = new GameObject[numSeats];


        #endregion


        PopulateGroup();
    }

    #region Group Mechanics

    public void AddControlMember()
    {
        if(controlMembersLeft <= 0)
            return;
        
        controlMembersLeft -= 1;
        
        controlText.text = "" + controlMembersLeft;

        if(controlMembersLeft == 0)
        {
            Destroy(controlText.transform.parent);
        }

        //int place = group.Length + 4 - controlMembersLeft; // NOTE Not scalable, manual changes needed

        int place = 0;

        for(int i = 0; i < group.Length; i++)
        {
            if(group[i] == null)
            {
                place = i;
                break;
            }
        }

        group[place] = Instantiate(controlMemberPrefab, transform.position, Quaternion.identity).AddComponent<ControlMember>();
        
        DetermineSeatNumber(place);

        group[place].transform.position = CalculatePosition(group[place].seatNumber);

        group[place].name = "Control Member: " + group[place].personName;

        // for(int i = 0; i < occupiedSeats.Length; i++)
        // {
        //     if(occupiedSeats[i] == 0)
        //     {
        //         group[i].seatNumber = i;

        //         occupiedSeats[i] = 1;

        //         group[i].transform.parent = transform;

        //         break;
        //     }
        // }

        psychopath.AffectStress(2);
        psychopath.AffectKillIntent(5 + 2 * CMRound);

        CMRound += 1;
        CMAlive += 1;

        return;
    }

    public void GroupCheckup()
    {
        foreach(Person p in group)
        {
            if(p != null && p.alive)
            {
                p.MentalCheckup();
            }
        }
        
        if(CMRound > 0)
            CMRound -= 1;
    }

    public void AffectGroup(Experiment experiment)
    {
        if(experiment == null)
            return;
        
        foreach(Person p in group)
        {
            if(p == null || !p.alive)
                continue;
            
            p.AffectFear(experiment.fearIncrease);

            p.AffectSanity(experiment.sanityIncrease);

            p.AffectStress(experiment.stressIncrease);

        }
    }

    public void KillPerson(Person person, int who) // 0 player, 1 psychopath, 2 group
    {
        if(person is Psychopath)
        {
            person.Kill(who);

            return;
        }

        switch(who)
        {
            case 0:
                person.transform.GetComponent<SpriteRenderer>().sprite = note;
                person.transform.localScale = new Vector3(1.5f,1.5f,1.5f);
                break;

            case 1:
            case 2:
                person.transform.GetComponent<SpriteRenderer>().sprite = blood;
                break;
            
            default:
                Debug.LogError("Error: KillPerson default case hit. Who: " + who);
                break;
        }

        person.alive = false;

        person.DrawCircle(false);

        if(person is ControlMember)
            CMAlive -= 1;
        else
            numDead += 1;

        if(who == 0)
        {
            psychopath.AffectFear(1);
        }

        psychopathExperiment.fearIncrease = 7;
        psychopathExperiment.sanityIncrease = 3;
        psychopathExperiment.stressIncrease = 5;

        AffectGroup(psychopathExperiment);

        Debug.Log("\'Removed\' " + person.personName + " from existence");
    }

    public void TemptDecision()
    {
        Person theWilling = null;

        foreach(Person p in group)
        {
            if(p != null && p.alive)
            {
                if(psychopath.seatNumber != p.seatNumber && p.WillingToKill())
                {
                    theWilling = p;

                    p.AffectFear(-10);
                    p.AffectSanity(-5);
                    p.AffectStress(-25);

                    break;
                }
            }
        }

        if(theWilling == null)
            return;

        Person deadToMe = DecideWho(theWilling);

        KillPerson(deadToMe, 2);

        Debug.Log("The Willing has decided to kill " + deadToMe.personName);
    }

    private Person DecideWho(Person theWilling) // Pick someone who is not the psychopath
    {
        Person p = group[Random.Range(1,group.Length)];

        if(p == null || !p.alive)
        {
            if(p.seatNumber != theWilling.seatNumber)
                return p;

            return DecideWho(theWilling);
        }

        return DecideWho(theWilling);
    }

    #endregion

    #region Group Setup
    public void PopulateGroup() 
    {
        if(seats[0] != null)
        {
            Debug.Log("Group already populated."); // If you want to be able to clean sweep the group, you have to clear occupiedSeats else StackOverflow exception

            return;
        }

        Debug.Log("Populating group...");

        psychopath = SetupPerson(0);

        for(int i = 1; i < groupSize; i++)
        {
            SetupPerson(i);            
        }

        MusicalChairs(); // Jesus why the fuck

        controlText.text = "" + controlMembersLeft;

    }

    private Psychopath SetupPerson(int index)
    {
        seats[index] = Instantiate(personPrefab, transform.position, Quaternion.identity); // Instantiate Person

        if(index == 0)
            group[0] = seats[0].AddComponent<Psychopath>();
        else
            group[index] = seats[index].AddComponent<Person>();

        seats[index].name = "Person: " + group[index].personName; // Set GameObject name

        seats[index].transform.parent = transform; // Set GameObject to be child of GroupManager

        DetermineSeatNumber(index);

        seats[index].transform.position = CalculatePosition(group[index].seatNumber);

        if(index == 0)
            return (Psychopath)group[0];

        return null;
    }

    private void DetermineSeatNumber(int index)
    {
        group[index].seatNumber = Random.Range(0,group.Length); // Choose random seat for person

        while(occupiedSeats[group[index].seatNumber] == 1) // Increase seatNumber if current seat is occupied 
        {
            group[index].seatNumber = (group[index].seatNumber + 1)%group.Length;
        }

        occupiedSeats[group[index].seatNumber] = 1; // Set seat to occupied
    }

    private void MusicalChairs()
    {
        for(int index = 0; index < seats.Length; index++)
        {
            Instantiate(chairPrefab, CalculatePosition(index) + Vector3.forward * 0.1f, Quaternion.identity).transform.parent = transform;
        }
    }

    private Vector3 CalculatePosition(int index)
    {
        return transform.position + new Vector3(index%5 * spacing,index/-5 * spacing,0.2f * index/-5);
    }

    #endregion


    // void OnDrawGizmos()
    // {
    //     for(int i = 0; i < 20; i++)
    //     {
    //         Gizmos.color = Color.grey;
            
    //         if(Input.GetKey(KeyCode.M) && occupiedSeats[i] == 1) // Colours in which seats are filled and who's who
    //         {
    //             Gizmos.color = Color.green;
    //             if(psychopath.seatNumber == i)
    //                 Gizmos.color = Color.red;
    //         }

    //         Gizmos.DrawSphere(transform.position + new Vector3(i%5,i/-5,0.2f * i/-5), 0.1f);
    //     }
    // }

}
