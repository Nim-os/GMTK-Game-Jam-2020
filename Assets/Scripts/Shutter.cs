using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shutter : MonoBehaviour
{
    
    private Vector3 shutterOpen = new Vector3(0,7.05f,-5);
    private Vector3 shutterClosed = new Vector3(0,0,-5);
    
    public bool inUse = false;


    public const float shutterTimer = 5.2f;
    private float ticker = 0;
    private bool goingUp = true;

    void Update()
    {
        if(inUse)
        {
            if(ticker <= 0)
            {
                inUse = false;
                AudioManager.instance.Play("slam");
            }

            transform.position = Vector3.Lerp(shutterClosed, shutterOpen, goingUp ?  1 - ticker/shutterTimer : ticker/shutterTimer);

            ticker -= Time.deltaTime;
        }
        
    }

    public void Open()
    {
        Use();
        goingUp = true;
    }

    public void Close()
    {
        Use();
        goingUp = false;
    }

    private void Use()
    {
        if(inUse)
        {
            AudioManager.instance.Stop("shutter");
        }

        ticker = shutterTimer;
        inUse = true;

        AudioManager.instance.Play("shutter");

    }
}
