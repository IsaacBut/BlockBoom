using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public Image[] tutorialImage;
    [SerializeField] private int nowInform;

    private void OpenImage(int nowInformIndex)
    {
        foreach (var image in tutorialImage)
        {
            image.gameObject.SetActive(false);
        }
        tutorialImage[nowInformIndex].gameObject.SetActive(true);

    }

    private void Start()
    {
        OpenImage(nowInform);
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
