using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{ // WARNING spaghetti code ahead, enjoy
    public enum GameStage
    {
        start,observe,cards,playerChoice
    }

    #region Variables
    public static GameManager instance;

    [Header("References")]
    public GameObject pauseMenu;

    public UnityEngine.UI.Button continueButton;
    public UnityEngine.UI.Button controlButton;
    
    public UnityEngine.UI.Image tooltip;
    public UnityEngine.UI.Text tooltipName;
    public UnityEngine.UI.Text tooltipBody;
    public UnityEngine.UI.Text stageName;
    public UnityEngine.UI.Text continueText;
    public Shutter shutter;


    [Header("Variables")]
    public int roundNumber = 0;
    public bool gameEnded = false;
    public bool isPaused = false;

    //public List<string> roundExperimentNames;
    //public List<string> roundExperimentDescs;

    public GameStage stage = GameStage.cards;

    public Person playerChoice;
    public Experiment experimentChoice;

    private int whoWon = 0;

    private bool noMoreCM = false;


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

    }

    void Start()
    {
        //if(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name.Equals("Menu"))
        if(Application.loadedLevel == 0)
        {
            AudioManager.instance.Play("glitchy");

            AudioManager.instance.Play("fan");
            AudioManager.instance.Play("static");
        }
        //else if(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name.Equals("Game"))
        else if(Application.loadedLevel == 1)
        {   
            StartCoroutine(DelayOpen());
        }
    }

    private IEnumerator DelayOpen()
    {
        yield return new WaitForSeconds(0.4f);

        shutter.Open();
        
        yield return new WaitForSeconds(Shutter.shutterTimer);

        AdvanceRoundStage();
    }

    public void TryAdvanceButton()
    {
        TryToAdvanceRoundStage();
    }

    public string TryToAdvanceRoundStage()
    {
        string ret = "";

        if(stage == GameStage.observe)
        {
            AdvanceRoundStage();

        }
        else if(stage == GameStage.cards)
        {
            if(experimentChoice != null)
            {
                AdvanceRoundStage();
            }
            else
            {
                ret = "Pick an experiment card.";
            }
        }
        else if(stage == GameStage.playerChoice)
        {
                AdvanceRoundStage();
        }

        // Debug.LogWarning(ret);

        return ret;
    }

    public GameStage AdvanceRoundStage() // Really needed? yeeeeeeeeees
    {
        stage = (GameStage)(((int)stage + 1)%4);

        if(stage == GameStage.start)
        {
            stageName.text = "Experimenting in progress...";
            continueText.text = "";

            StartCoroutine(HandleNewRound());

            controlButton.interactable = false; // Really needed?

            roundNumber += 1;
        }
        else if(stage == GameStage.observe)
        {
            stageName.text = "Observation";
            continueText.text = "Click To Proceed";
        }
        else if(stage == GameStage.cards)
        {
            stageName.text = "Experiment Choice";
            continueText.text = "Choose To Proceed";

            controlButton.interactable = true;

            CardManager.instance.Pick3CardsAny3Card(roundNumber);
        }
        else if(stage == GameStage.playerChoice)
        {
            stageName.text = "Sentencing";
            continueText.text = "Kill Someone or Not";

        }


        return stage;
    }

    private IEnumerator HandleNewRound()
    {

        shutter.Close();

        yield return new WaitForSeconds(Shutter.shutterTimer);

        if(experimentChoice == null)
        {
            Debug.LogError("WARNING, experiment choice null");
            LoadMenu();
        }

        if(playerChoice != null)
            GroupManager.instance.KillPerson(playerChoice, 0);
        else
            GroupManager.instance.psychopath.AffectKillIntent(10);

        GroupManager.instance.AffectGroup(experimentChoice);

        GroupManager.instance.psychopath.AttemptKill();

        GroupManager.instance.TemptDecision();

        GroupManager.instance.GroupCheckup();

        // Add round to timemachine /// For the future, hence the name lmao


        int livingCM = GroupManager.instance.controlMembersLeft + GroupManager.instance.CMAlive;

        int threshold = GroupManager.instance.groupSize/3 - 1;

        if(GroupManager.instance.numDead > threshold || livingCM < 1) // Basically if number of dead people outweighs the living minus control group
        {
            Debug.Log("No alive CMs or enough people. Counter: " + GroupManager.instance.numDead + " CMs: " + livingCM);
            noMoreCM = true;
            DecideGame(false);
        }
        
        
        AudioManager.instance.Play("footsteps");

        yield return new WaitForSeconds(3.5f);


        shutter.Open();

        yield return new WaitForSeconds(Shutter.shutterTimer);


        if(whoWon == 1)
        {
            PaperDrawer.instance.canFlip = false;
            PaperDrawer.instance.paperTitle.text = "Results";
            PaperDrawer.instance.paperBody.text = "Nice work Detective.\nI'm glad you were able to catch onto him soon enough.\n\nOnly " 
            + GroupManager.instance.numDead + " people died and you enabled " + (4 - GroupManager.instance.controlMembersLeft) + " Control Members.\n\nGood job keeping things from getting,\n\nOut Of Control.";

            AudioManager.instance.Play("page", 0.95f);

            StartCoroutine(WaitToShowCanvas());

        }
        else if(whoWon == -1)
        {
            PaperDrawer.instance.canFlip = false;
            PaperDrawer.instance.paperTitle.text = "Results";
            if(noMoreCM)
                PaperDrawer.instance.paperBody.text = "Sadly, the Psychopath was many steps ahead of us and got to all of out Control Members.\nWe will be shutting this down until further notice.\n\nGood luck.";
            else
                PaperDrawer.instance.paperBody.text = "Unfortunately, we didn't find the Psychopath soon enough.\n\nWe will be shut down soon but there'll always be a bigger fish out there waiting for us.\n\nStay safe out there.\n\nSee ya, partner.";

            AudioManager.instance.Play("page", 0.90f);

            StartCoroutine(WaitToShowCanvas());

        }
        else if(whoWon == 0)
        {
        AdvanceRoundStage();

        }

        playerChoice = null;
        experimentChoice = null;
    }

    public void DecideGame(bool won) // TODO
    {
        gameEnded = true;

        if(won)
            whoWon = 1;
        else
            whoWon = -1;
        
        pauseMenu.transform.GetChild(1).GetChild(1).GetComponent<UnityEngine.UI.Button>().interactable = false;

    }

    private IEnumerator WaitToShowCanvas()
    {
        yield return new WaitForSeconds(5f);

        pauseMenu.gameObject.SetActive(true);
    }

    #region Scene Management

    public void Play()
    {
        //UnityEditor.SceneManagement.EditorSceneManager.LoadScene("Game");
        Application.LoadLevel(1);

        AudioManager.instance.Stop("glitchy");

        stage = GameStage.start;
        roundNumber = 1;
        gameEnded = false;
    }

    public void LoadMenu()
    {
        shutter.Close();
    
        StartCoroutine(NowLoadIt());
    }

    private IEnumerator NowLoadIt()
    {
        yield return new WaitForSeconds(Shutter.shutterTimer);

        //UnityEditor.SceneManagement.EditorSceneManager.LoadScene("Menu");
        Application.LoadLevel(0);
    }

    public void Quit()
    {
        AudioManager.instance.Stop();
        UnityEngine.Application.Quit();
    }

    #endregion
}
