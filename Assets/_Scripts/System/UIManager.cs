using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UIManager : Singleton<UIManager>
{
    [Header("Next Queue")]
    [SerializeField] private RectTransform nextQueueFrame = null;
    [SerializeField] private GameObject nextQueueImage = null;
    [Space]

    [SerializeField] private TextMeshProUGUI highScoreText = null;
    [SerializeField] private TextMeshProUGUI currentScoreText = null;
    [SerializeField] private TextMeshProUGUI timerText = null;

    private Queue<Image> nextQueueImgs = null;

    public void SetHighScoreText(uint score) => highScoreText.text = score.ToString("D5");
    public void SetCurrentScoreText(uint score) => currentScoreText.text = score.ToString("D5");
    public void SetTimerText(string text) => timerText.text = text;

    public void InitNextQueueBall(uint count, Sprite[] sprites)
    {
        foreach (Transform child in nextQueueFrame.transform)
        {
            Destroy(child.gameObject);
        }
        nextQueueImgs = new Queue<Image>();
        for (int index = 0; index < count; index++)
        {
            var nextQueueBall = Instantiate(nextQueueImage, nextQueueFrame);
            nextQueueBall.transform.SetParent(nextQueueFrame, false);
            Image img = nextQueueBall.GetComponent<Image>();
            img.sprite = sprites[index];
            nextQueueImgs.Enqueue(img);
        }
    }

    public void AddNewQueueImg(Sprite sprite)
    {
        Image firstImg = nextQueueImgs.Dequeue();
        firstImg.sprite = sprite;
        firstImg.rectTransform.SetAsLastSibling();
        nextQueueImgs.Enqueue(firstImg);
    }
}
