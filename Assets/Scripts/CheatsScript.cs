using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretScript : MonoBehaviour
{
    public UnityEngine.UI.Text text;
    
    public void DisplayCheat()
    {
        text.text = "Hold down [G] [M] [T] [K] to skip the menu intro!";
    }
}
