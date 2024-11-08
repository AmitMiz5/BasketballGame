using UnityEngine;
using UnityEngine.UI;

public class QuestionImageScript : MonoBehaviour
{
    public SpriteRenderer questionImage;
    private Vector3 originalImageScale;
    private Vector3 enlargedImageScale;

    void Start()
    {

        if (questionImage != null)
        {
            originalImageScale = questionImage.transform.localScale;
            enlargedImageScale = originalImageScale * 2.5f; // You can adjust this multiplier as needed
        }
    }

    void OnMouseEnter()
    {
        if (questionImage != null && questionImage.sprite != null)
        {
            questionImage.transform.localScale = enlargedImageScale;
            Debug.Log("im in");
        }
        Debug.Log("game");
    }

    void OnMouseExit()
    {
        if (questionImage != null && questionImage.sprite != null)
        {
            questionImage.transform.localScale = originalImageScale;
        }
    }

    public void ResetScale()
    {
        if (questionImage != null)
        {
            questionImage.transform.localScale = originalImageScale;
        }
    }
}