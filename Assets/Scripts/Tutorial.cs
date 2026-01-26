using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public Image[] tutorialImage;
    private int nowInform;

    private void OpenImage(int nowInformIndex)
    {
        foreach (var image in tutorialImage)
        {
            image.enabled = false;
        }
        tutorialImage[nowInformIndex].enabled = true;

    }


    public void Button_GoNext()
    {
        if (nowInform +1< tutorialImage.Length)
        {
            nowInform++;
            OpenImage(nowInform);
        }
    }

    public void Button_BackTo()
    {
        if (nowInform > 0) 
        {
            nowInform--;
            OpenImage(nowInform);
        }
    }

    public void Button_Close()
    {
        gameObject.SetActive(false);
    }
}
