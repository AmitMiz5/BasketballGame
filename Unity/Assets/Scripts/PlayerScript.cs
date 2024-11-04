using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerScript : MonoBehaviour
{
    private Rigidbody2D playerRB; // הגדרת ריג׳יד בודי לשחקן
    public float speed; // משתנה שישלוט על מהירות השחקן
    public Vector2 startPos; // מיקום התחלתי של השחקן
    public AnswerScript answerScript; //קישור לסקריפט של התשובה
    public GameManagerScript gameManager; // קישור לסקריפט גיים מנג׳ר
    public bool playerAllowedToMove = true; // משתנה ששומר את תזוזת/עצירת השחקן
    public Vector2 targetPosition; // המיקום שאליו נרצה שהשחקן יגיע בתזוזה עם החצים (כל פעם לפודיום הבא)
    [SerializeField] private SpriteRenderer playerSprite;
    private Animator animator;
    void Start()
    {
        playerRB = GetComponent<Rigidbody2D>(); // בתחילת המשחק המשתנה ישמור את הריג׳יד בודי של השחקן
        startPos = transform.position; // שמירת המיקום ההתחלתי
        targetPosition = transform.position; // שמירת המיקום שאליו השחקן יזוז
        animator = GetComponent<Animator>(); // שמירת האנימייטור להפעלת אנימציות
    }
    void Update()
    {
        if (playerAllowedToMove) //אם השחקן יכול לזוז
        {
            if (Input.GetKeyDown(KeyCode.RightArrow)) // אם המשתמש לוחץ חץ ימינה
            {
                Vector2 nextPosition = gameManager.GetNextPodiumInDirection(false); // תזוזה לפודיום הבא מצד ימין
                targetPosition = nextPosition; // ?
                playerSprite.flipX = false;
                animator.SetTrigger("moveRight");
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow)) // אם המשתמש לוחץ חץ שמאלה
            {
                Vector2 nextPosition = gameManager.GetNextPodiumInDirection(true); // קבלת התנועה הבאה לפי המנהרה הקודמת
                targetPosition = nextPosition; // ?
                playerSprite.flipX = true;
                animator.SetTrigger("moveRight");
            }
        }
    }
    void FixedUpdate()
    {
        if (playerAllowedToMove)
        {
            Vector2 newPosition = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.fixedDeltaTime); // העברת השחקן למיקום המטרה
            playerRB.MovePosition(newPosition); // תנועת השחקן למיקום החדש
        }
    }
    public void ReturnPlayerToDefaultPosition()
    {
        transform.position = startPos; // החזרת השחקן למיקום ההתחלתי
        targetPosition = startPos; // הגדרת המיקום אליו השחקן יזוז כמיקום ההתחלתי
        playerRB.velocity = Vector2.zero; // איפוס המהירות של השחקן
    }
    public void StopPlayer()
    {
        targetPosition = transform.position; // עצירת תנועת השחקן
        animator.SetTrigger("holdBall");

    }
    public void AllowPlayerToMove()
    {
        playerAllowedToMove = true; // פונקציה המגדירה שהשחקן יכול לזוז
    }
    public void DisallowPlayerToMove()
    {
        playerAllowedToMove = false; // פונקציה המגדירה שהשחקן לא יכול לזוז
        

    }
}