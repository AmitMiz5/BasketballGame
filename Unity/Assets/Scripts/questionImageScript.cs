using UnityEngine;
using UnityEngine.UI;

public class QuestionImageScript : MonoBehaviour
{
    public SpriteRenderer questionImage; // משתנה לשמירת הספרייט רנדרר של תמונות הנמצאות בשאלה
    private Vector3 originalImageScale; // משתנה לשמירת קנה המידה המקורי של התמונה
    private Vector3 enlargedImageScale; // משתנה לשמירת קנה מידה מוגדל של התמונה
    void Start()
    {

        if (questionImage != null) // בדיקה אם יש רכיב questionImage כדי למנוע שגיאות
        {
            originalImageScale = questionImage.transform.localScale; // שמירה של קנה המידה המקורי
            enlargedImageScale = originalImageScale * 2.5f; // קנה מידה מוגדל, שניתן לשנות על ידי שינוי המקדם 2.5f
        }
    }

    void OnMouseEnter() // פונקציה שנקראת כאשר העכבר נכנס לאזור התמונה
    {
        if (questionImage != null && questionImage.sprite != null) // בדיקה שהתמונה קיימת וטעונה
        {
            questionImage.transform.localScale = enlargedImageScale; // הגדלת התמונה לגודל המוגדר
            Debug.Log("im in");
        }
        Debug.Log("game");
    }

    void OnMouseExit() // פונקציה שנקראת כאשר העכבר יוצא מאזור התמונה
    {
        if (questionImage != null && questionImage.sprite != null) // בדיקה שהתמונה קיימת וטעונה
        {
            questionImage.transform.localScale = originalImageScale; // החזרת התמונה לגודל המקורי
        }
    }

    public void ResetScale() // פונקציה שמאפשרת לאפס את גודל התמונה לגודל המקורי
    {
        if (questionImage != null)
        {
            questionImage.transform.localScale = originalImageScale; // החזרת קנה המידה למקורי
        }
    }
}