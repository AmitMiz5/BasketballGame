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
using System.Timers;
using UnityEditor;


public class EndAnim : MonoBehaviour
{

    public GameManagerScript gameManager; //  קישור לסקריפט גיים מנג׳ר
    [SerializeField] private GameObject allCloseAnim; // כלל האובייקטים של אנימציית הפתיחה
    [SerializeField] private GameObject allGameManager; // כלל האובייקטים של המשחק
    [SerializeField] private GameObject player; //  שחקנית

    [SerializeField] private PlayableDirector playableDirector; // To control the timeline
    public GameObject skipButton;
    public EndScript end; //קישור לסקריפט סיום המשחק

    
    void Start()
    {
        playableDirector = GetComponent<PlayableDirector>();//מציאת ושמירת playableDirector כמשתנה חדש
        playableDirector.stopped += OnTimelineStopped;//חיבור הפונקצייה לארוע עצירה שישמש בעצירה או דילוג על האנימצייה
    }

    public void PlayCloseAnim() //פונקציה להתחלת אנימציית סיום
    {

        allGameManager.SetActive(false);//הסתרת כלל האובייקטים של המשחק
        gameManager.madHitkadmut.SetActive(false);//הסתרת מד התקדמות
        player.SetActive(false);//הסתרת השחקנית
        allCloseAnim.SetActive(true);//הצגת האובייקטים של אנימציית סיום
        skipButton.SetActive(true);//הצגת כפתור דילוג
        gameManager.timer.enabled = false;// הסתרת טיימר
        gameManager.progress.enabled = false;//הסתרת כמות תשובות נכונות
        playableDirector.Play();//הפעלת הטיימליין של האנימציה


    }

    public void Skip()//פונקציה לעצירת האנימציה
    {
        playableDirector.Stop();


    }

    private void OnTimelineStopped(PlayableDirector director) //כאשר האנימצייה נעצרה
    {
        skipButton.SetActive(false);//הסתרת כפתור דילוג
        allCloseAnim.SetActive(false); // כלל האובייקטים של אנימציית הסיום יוסרו מהמסך
        Debug.Log("Timeline finished, ending the game");
        end.EndGameScreen();//קריאה לפונקציה המציגה את מסך הסיום

    }
   
}
