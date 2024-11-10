using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using System;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using Unity.Mathematics;
using UnityEngine.UI;


public class OpenAnim : MonoBehaviour
{
    public GameManagerScript gameManager; //  קישור לסקריפט גיים מנג׳ר
    [SerializeField] private GameObject allOpenAnim; // כלל האובייקטים של אנימציית הפתיחה
    [SerializeField] private GameObject allGameManager; //כלל האובייקטים של המשחק
    [SerializeField] private PlayableDirector playableDirector; // To control the timeline
    public GameObject skipButton; //כפתור דילוג


    void Start()
    {

        playableDirector = GetComponent<PlayableDirector>();//מציאת ושמירת playableDirector כמשתנה חדש
        playableDirector.stopped += OnTimelineStopped;//חיבור הפונקצייה לארוע עצירה שישמש בעצירה או דילוג על האנימצייה

    }

   

    public void PlayOpenAnim() //פונקציה להתחלת אנימציית פתיחה
    {
        Debug.Log("Starting opening animation");
        allOpenAnim.SetActive(true);//הצגת כל האובייקטים של אנימציית הפתיחה
        skipButton.SetActive(true);//הצגת כפתור דילוג
        playableDirector.Play();//הפעלת הטיימליין של האנימצייה
        Debug.Log("Timeline started");




    }

    public void Skip()
    {
        playableDirector.Stop(); //עצירת הטיימליין
        allOpenAnim.SetActive(false); // כלל האובייקטים יוסרו מהמסך
        skipButton.SetActive(false);
        //gameManager.StartGame();// קריאה לפונקציה שמתחילה את המשחק מתוך הגיים מנג'ר


    }

    private void OnTimelineStopped(PlayableDirector director) //פונקצייה שנקראת כאשר הטיימליין נעצר או באופן טבעי לאחר הרצה מלאה או לאחר לחיצה על כפתור דלג
    {
        allOpenAnim.SetActive(false); // כלל האובייקטים יוסרו מהמסך
        Debug.Log("Timeline finished, starting the game");
        gameManager.StartGame();// קריאה לפונקציה שמתחילה את המשחק מתוך הגיים מנג'ר

    }


}
