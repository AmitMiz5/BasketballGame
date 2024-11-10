using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StartScript : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField; // שדה להזנת קוד המשחק
    [SerializeField] private SpriteRenderer SpriteRenderer; // רקע של מסך פתיחה
    [SerializeField] private TextMeshProUGUI txt; // טקסט הנחיות להזנת קוד
    [SerializeField] private GameObject allStartScreen; // כלל האובייקטים של מסך פתיחה
    [SerializeField] private GameObject startButton; // כפתור בדיקת קוד משתמש
    public GameManagerScript gameManager; //  קישור לסקריפט גיים מנג׳ר
    public OpenAnim openAnim; //  קישור לסקריפט אנימציית פתיחה
    public ServerManagerScript serverManager;

    public void StartOver() // פונצקיה לתחילת משחק חדש- בלחיצה על כפתור
    {
        gameManager.madHitkadmut.SetActive(false);//הסתרת מד התקדמות
        allStartScreen.SetActive(true); // כלל האובייקטים יופיעו על המסך
        startButton.SetActive(true);//הצגת כפתור התחלה

    }


    public void clearBG() // בדיקת קוד המשתמש
    {


        serverManager.CheckCode(inputField.text); //קריאה לפונקציה שבודקת את הקוד מתוך סקריפט החיבור לשרת


    }
}
