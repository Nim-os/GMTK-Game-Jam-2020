using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static CardManager instance;

    public Experiment[] cards;

    public GameObject[] objs = new GameObject[3];

    [Space]

    public GameObject cardPrefab;
    public Sprite cardBack;
    public Sprite cardFrame;

    private Vector3 card1 = new Vector3(-1.5f,3f,-8f);
    private Vector3 card2 = new Vector3(2f,0.3f,-8f);
    private Vector3 card3 = new Vector3(-1.5f,-2.7f,-8f);


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
    }


    public void Pick3CardsAny3Card(int roundNumber)
    {
        DispelObjs();
        SummonObjs();

        AudioManager.instance.Play("card");

        Experiment ex1,ex2,ex3; // Sure bad code but I have been awake for 36 hours so please slack

        switch(roundNumber)
        {
            case 1:
            case 2:
                ex1 = RandomCard(3);
                ex2 = RandomCard(3);
                ex3 = RandomCard(3);
                break;
            
            case 3: // tierB is a yes
            case 4:
                ex1 = RandomCard(6);
                ex2 = RandomCard(6);
                ex3 = RandomCard(6);
                break;

            default: // Any 3 random
                ex1 = RandomCard(9);
                ex2 = RandomCard(9);
                ex3 = RandomCard(9);
                break;
        }

        HandleCardSetting(objs[0], ex1);
        HandleCardSetting(objs[1], ex2);
        HandleCardSetting(objs[2], ex3);
    }

    public void GoodbyeCards()
    {
        AudioManager.instance.Play("card",0.9f);
        
        DispelObjs();
    }

    public Experiment RandomCard(int max)
    {
        return cards[Random.Range(0,max)];
    }

    private void SummonObjs()
    {
        objs[0] = Instantiate(cardPrefab,card1,Quaternion.identity);
        objs[1] = Instantiate(cardPrefab,card2,Quaternion.identity);
        objs[2] = Instantiate(cardPrefab,card3,Quaternion.identity);
    }

    private void DispelObjs()
    {
        foreach(GameObject obj in objs)
        {
            if(obj == null)
                continue;
            
            Destroy(obj);
        }
    }

    private void HandleCardSetting(GameObject obj, Experiment experiment)
    {
        obj.GetComponent<Card>().experiment = experiment;

        obj.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = experiment.artwork;

        obj.transform.GetChild(1).GetComponent<TextMesh>().text = experiment.experimentName;

        obj.transform.GetChild(2).GetComponent<TextMesh>().text = experiment.desc.Replace("\\n","\n");
    }
}
