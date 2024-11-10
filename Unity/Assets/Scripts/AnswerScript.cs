using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class AnswerScript : MonoBehaviour
{
    public TextMeshPro answerTxt; // משתנה ששומר את טקסט המסיח
    public SpriteRenderer answerImage; // משתנה ששומר את תמונת המסיח
    public bool isCorrect; // משתנה שמייצג את התשובה הנכונה
    public GameManagerScript gameManager; // קישור לסקריפט גיים מנג׳ר
    public PlayerScript player; // קישור לסקריפט פלייר-שחקן
    public Progress progressBar;



   // private Animator animator;
    [SerializeField] private GameObject x; // תשובה לא נכונה
    [SerializeField] private GameObject v; // תשובה נכונה
    [SerializeField] private AudioClip correctSound; // סאונד לתשובה נכונה
    [SerializeField] private AudioClip incorrectSound; // סאונד לתשובה שגויה
    [SerializeField] public AudioSource feedbackAudio; // אובייקט להפעלת סאונד
    [SerializeField] private SpriteRenderer podium; // פודיום של המסיחים
    public GameObject ball;
    

    private bool spacePressed = false; // משתנה ששומר את המצב בו כפתור הרווח לא לחוץ
    private Vector3 originalImageScale; // גודל מקורי של התמונה
    private Vector3 enlargedImageScale; // גודל מוגדל של התמונה

    void Start()
    {
        player = GameObject.FindObjectOfType<PlayerScript>(); // מציאת מיקום השחקן בתחילת המשחק
        //animator = GetComponent<Animator>();
        originalImageScale = answerImage.transform.localScale; // שמירת הגודל המקורי של התמונה
        enlargedImageScale = originalImageScale * 1.5f; // חישוב הגודל המוגדל של התמונה (פי 1.5 מהגודל המקורי)
    }

    void Update()
    {
        if (!spacePressed && IsPlayerOnPodium() && Input.GetKeyDown(KeyCode.Space) && IsPlayerStopped())// תנאי שבודק האם השחקן עצר, עומד על הפודיום ולוחץ רווח לסימון התשובה
        {
            spacePressed = true; // שינוי משתנה ששומר את מצב כפתור הרווח ל״נכון״ (לחוץ)
            //animator.SetTrigger("holdBall");
            player.StopPlayer(); // קריאה לפונקציית עצירת שחקן מתוך סקריפט של השחקן

            CheckAnswer(); // קריאה לפונקציה הבודקת האם התשובה נכונה
            gameManager.nextBtn.SetActive(true); // הצגת כפתור ״שאלה הבאה״
        }
    }

    void OnMouseEnter()
    {
        // הגדלת תמונה במידה וקיימת תמונה במסיח
        if (answerImage != null && answerImage.sprite != null)
        {
            answerImage.transform.localScale = enlargedImageScale; // הגדלת רק התמונה
        }
    }

    void OnMouseExit()
    {
        // הקטנת התמונה לגודל המקורי כשהעכבר יוצא משטח המסיח 
        if (answerImage != null && answerImage.sprite != null)
        {
            answerImage.transform.localScale = originalImageScale;
        }
    }


    bool IsPlayerStopped()//פונקציה שבודקת האם השחקן הפסיק לנוע
    {
        return Vector2.Distance(player.transform.position, player.targetPosition) < 0.01f;
    }

    bool IsPlayerOnPodium() // בדיקה האם השחקן עומד על הפודיום
    {
        return podium.color == Color.grey; // כאשר השחקן נוגע בפודיום הפודיום נצבע באפור
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        spacePressed = false;//כפתור רווח לא נלחץ
        podium.color = Color.grey; // השחקן נכנס לפודיום- צבע משתנה לאפור
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        podium.color = Color.white; // השחקן יוצא מהפודיום- צבע הפודיום חוזר ללבן
        spacePressed = false;// כפתור רווח לא נלחץ
    }

    public bool CheckAnswer()//פונקציה לבדיקת תשובה
    {
        
        player.DisallowPlayerToMove(); // קריאה לפונקציה מתוך הסקריפט של השחקן שלא מאפשרת לא לזוז בזמן הבדיקה
        gameManager.SetTimerInactive(); // עצירת הטיימר
        
        
        ball.SetActive(false); //הסתרת הכדור מהפודיום לפני הפעלת אנימציה של זריקה לסל

        if (isCorrect)// אם התשובה נכונה
        {
            gameManager.crowdBlur.SetActive(false);//הסתרת הקהל הסטטי
            gameManager.crowdCheer.SetActive(true);// הפעלת קהל לתשובה נכונה
            gameManager.correct.SetActive(true); //הפעלת טקסט תשובה נכונה
            v.SetActive(true); // הצגת וי ירוק לסימון התשובה הנכונה
            
            if (gameManager.audioSound)
            {
                feedbackAudio.clip = correctSound; // הגדרת הסאונד לתשובה נכונה
                feedbackAudio.Play(); // הפעלת הסאונד
            }

            gameManager.ProgressCounter(); // הוספת תשובה נכונה לספירת ההתקדמות במשחק
            gameManager.UpdateScore(true);
        }
        else// אם התשובה לא נכונה
        {
            
            gameManager.totalWrongAnswers++; // הוספת תשובה לא נכונה לסכימה של כמות התשובות בהן המשתמש טעה
            x.SetActive(true); // הצגת איקס לסימון התשובה השגויה
            gameManager.incorrect.SetActive(true); //הפעלת טקסט תשובה לא נכונה


            if (gameManager.audioSound)
            {
                feedbackAudio.clip = incorrectSound; // הגדרת הסאונד לתשובה שגויה
                feedbackAudio.Play(); // הפעלת הסאונד
            }
            gameManager.UpdateScore(false);
        }

        return isCorrect; // החזרת ערך בוליאני (true אם התשובה נכונה, אחרת false)
    }
}