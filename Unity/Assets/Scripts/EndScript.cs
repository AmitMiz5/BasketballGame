using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;


public class EndScript : MonoBehaviour

{
    [SerializeField] public GameObject allEndScreen; // כלל האובייקטים של מסך הפופ-אפ נגמר הזמן
    [SerializeField] private TextMeshProUGUI timerCounter; // טקסט של ספירת הזמן הכללית של המשחק
    [SerializeField] private TextMeshProUGUI wrongAnswersCounter;// טקסט של סכימת השאלות הלא נכונות
    [SerializeField] private TextMeshProUGUI timerQustionCounter; // טקסט של ספירת הזמן הכללית של כל שאלה
    [SerializeField] private TextMeshProUGUI AnswersCounter; // טקסט של סכימת השאלות הנכונות
    [SerializeField] private TextMeshProUGUI scoreText;
    public GameManagerScript gameManager; // קישור לסקריפט גיים מנג׳ר  
    public void EndGameScreen() // פונקציה המפעילה את האובייקטים של מסך סיכום המשחק 
    {
        gameManager.madHitkadmut.SetActive(false);//הסתרת מד התקדמות
        allEndScreen.SetActive(true);// כלל האובייקטים יופיעו על המסך
        TimeSpan elapsedTime = DateTime.Now - gameManager.startTime; // חישוב הזמן הכולל של המשחק
        string formattedTime = string.Format("{0:00}:{1:00}", elapsedTime.Minutes, elapsedTime.Seconds); // הצגת זמן המשחק הכולל לפי פורמט שניות ודקות
        timerCounter.text = formattedTime; // שינוי הטקסט לזמן הכולל
        wrongAnswersCounter.text = gameManager.totalWrongAnswers.ToString(); // הצגת כמות התשובות השגויות של המשתמש
        scoreText.text = Mathf.Round(gameManager.score).ToString();
        HideTextProggres();
    }

    private void HideTextProggres()
    {
        timerQustionCounter.text = "";
        AnswersCounter.text = "";
    }

    public void HideAllObjects() // הסתרת האובייקטים לאחר לחיצה על משחק חדש
    {
        allEndScreen.SetActive(false); // הסתרת כל האובייקטים על המסך
    }

}
