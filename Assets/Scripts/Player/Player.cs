using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Variables
    [Header("References")]
    public Camera cam;
    public GameObject pauseMenu;
    public LayerMask npcLayer;
    public Sprite markerX;
    public Sprite markerCircle;


    [Header("Misc")]
    public bool isPaused;

    private bool lastHadMarker = false;
    private int lastSeatNumber;

    #endregion

    void Update()
    {
        if(!isPaused)
        {
            if(Input.GetMouseButtonDown(0))
            {
                if(GameManager.instance.stage == GameManager.GameStage.playerChoice)
                {
                    RaycastHit2D hit = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition), cam.transform.rotation.eulerAngles, 1f, npcLayer);

                    if(hit != false)
                    {
                        Person p = hit.transform.GetComponent<Person>();

                        if(p != null && !(p is ControlMember))
                        {
                            if(p.seatNumber == lastSeatNumber)
                            {
                                CleanPlayerBuffer(p);

                                return;
                            }
                            
                            
                            if(lastSeatNumber != -1)
                            {
                                GameManager.instance.playerChoice.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = markerCircle; // Save if person had squiggled on someone's head instead
                            
                                if(!lastHadMarker)
                                {
                                    GameManager.instance.playerChoice.DrawCircle(false);
                                }
                            }


                            GameManager.instance.playerChoice = p;

                            lastSeatNumber = p.seatNumber;
                            lastHadMarker = p.transform.GetChild(0).gameObject.activeSelf;

                            p.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = markerX;

                            p.DrawCircle(true);

                            GameManager.instance.continueText.text = "Click To Proceed";
                        }
                    }
                }
                if(GameManager.instance.stage == GameManager.GameStage.cards)
                {
                    lastSeatNumber = -1;
                    lastHadMarker = false;

                    RaycastHit2D hit = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition), cam.transform.rotation.eulerAngles, 1f, npcLayer);

                    if(hit != false)
                    {
                        Card ex = hit.transform.GetComponent<Card>();

                        if(ex != null)
                        {
                            GameManager.instance.experimentChoice = ex.experiment;

                            CardManager.instance.GoodbyeCards();

                            GameManager.instance.TryAdvanceButton();
                        }
                    }
                }
            }

            if(Input.GetMouseButtonDown(1)) // woooooooo markerrrrrrsssssssss
            {
                RaycastHit2D hit = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition), cam.transform.rotation.eulerAngles, 1f, npcLayer);

                if(hit != false)
                {
                    Person p = hit.transform.GetComponent<Person>();

                    if(p != null && !(p is ControlMember))
                    {
                        if(p.seatNumber == lastSeatNumber)
                        {
                            CleanPlayerBuffer(p);

                            return;
                        }


                        p.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = markerCircle;

                        p.DrawCircle();
                    }

                }
            }
        }

        if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P)) // Pause game
        {
            HandlePause();
        }
    }

    private void CleanPlayerBuffer(Person p)
    {
        p.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = markerCircle;
                                
        p.DrawCircle(false);

        
        GameManager.instance.playerChoice = null;

        lastSeatNumber = -1;
        lastHadMarker = false;
    }

    public void HandlePause()
    {
        if(GameManager.instance.gameEnded)
            return;
        
        isPaused = !isPaused;

        GameManager.instance.isPaused = isPaused;

        if(isPaused)
        {
            pauseMenu.gameObject.SetActive(true);
        }
        else
        {
            pauseMenu.gameObject.SetActive(false);
        }
    }


    // void OnDrawGizmos()
    // {
    //     Gizmos.DrawSphere(cam.ScreenToWorldPoint(Input.mousePosition), 1f);
    // }

}
