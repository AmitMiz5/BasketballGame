using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeEnd : MonoBehaviour
{
    [SerializeField] private GameObject allTimeEnd; // כלל האובייקטים של מסך הפופ-אפ נגמר הזמן
    [SerializeField] private TextMeshProUGUI timerCounter; // טקסט של ספירת הזמן הכללית של השאלה
    [SerializeField] private TextMeshProUGUI AnswersCounter; // טקסט של סכימת השאלות הנכונות
    public GameManagerScript gameManager; // קישור לסקריפט גיים מנג׳ר  

    public void timeEndSetActive() // פונקציה המפעילה את האובייקטים של מסך נגמר הזמן 
    {
        allTimeEnd.SetActive(true); // כלל האובייקטים יופיעו על המסך
        timerCounter.gameObject.SetActive(false); // מלל של טיימר- יוסתרו מהמסך
        AnswersCounter.gameObject.SetActive(false);// מלל של כמות תשובות נכונות- יוסתרו מהמסך
    }

    public void timeEndSetActivefalse() //פונקציה המסתירה את האובייקטים של מסך נגמר הזמן 
    {
        allTimeEnd.SetActive(false); // כלל האובייקטים יוסתרו מהמסך
        timerCounter.gameObject.SetActive(true); // מלל של טיימר- יופיעו על המסך
        AnswersCounter.gameObject.SetActive(true); // מלל של כמות תשובות נכונות- יופיעו על המסך
    }
}
