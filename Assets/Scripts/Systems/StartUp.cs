using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartUp : MonoBehaviour
{
    public UnityEngine.UI.Image panel;
    public UnityEngine.UI.Image logo;

    public UnityEngine.UI.Image title;

    private float ticker = 10;


    void Update()
    {
        if(Input.GetKey(KeyCode.G) && Input.GetKey(KeyCode.M) && Input.GetKey(KeyCode.T) && Input.GetKey(KeyCode.K))
        {
            ticker = 0;

            title.color = new Color32(255,255,255,255);

            panel.transform.gameObject.SetActive(false);
            logo.transform.gameObject.SetActive(false);

            Destroy(this);
            return;
        }

        if(ticker <= 0)
        {
            panel.transform.gameObject.SetActive(false);

            Destroy(this);
            return;
        }
        else if(ticker < 2f)
        {
            float trueAlpha = Mathf.Lerp(0f, 1f, ticker/2);

            byte alpha = (byte)(256*trueAlpha);

            panel.color = new Color32(0,0,0,alpha);
        }
        else if(ticker <= 5f && ticker > 4f)
        {
            float trueAlpha = Mathf.Lerp(1f, 0f, ticker-4f);

            byte alpha = (byte)(256*trueAlpha);

            title.color = new Color32(255,255,255,alpha);
        }
        else if(ticker <= 6f)
        {
            float trueAlpha = Mathf.Lerp(0f, 1f, ticker-5f);

            byte alpha = (byte)(256*trueAlpha);

            //Debug.Log(trueAlpha + ", " + alpha);

            logo.color = new Color32(255,255,255,alpha);
        }

        ticker -= Time.deltaTime;
    }
}
