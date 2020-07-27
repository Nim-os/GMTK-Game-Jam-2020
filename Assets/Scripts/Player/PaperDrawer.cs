using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperDrawer : MonoBehaviour
{
    public static PaperDrawer instance;
    public Paper[] papers;

    public int page = -1;
    public Paper currentPaper;
    public UnityEngine.UI.Text paperTitle;
    public UnityEngine.UI.Text paperBody;

    public bool canFlip = true;


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
        UpdatePage(0);
    }

    public void FlipPage()
    {
            FlipPage(1);
    }

    public void FlipPage(int dir)
    {
        if(!canFlip)
            return;

        page = (page + dir)%papers.Length;

        UpdatePage(page);

        if(dir == 1)
            AudioManager.instance.Play("page");
        else
            AudioManager.instance.Play("page", 0.95f);

    }

    private void UpdatePage(int index)
    {
        currentPaper = papers[index];

        paperTitle.text = currentPaper.title;
        paperBody.text = currentPaper.body.Replace("\\n","\n");
    }
}
