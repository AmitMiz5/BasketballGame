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

    // Start is called before the first frame update
    void Start()
    {
        playableDirector = GetComponent<PlayableDirector>();
        playableDirector.stopped += OnTimelineStopped;
    }

    public void PlayCloseAnim() //פונקציה להתחלת אנימציית פתיחה
    {

        allGameManager.SetActive(false);
        gameManager.madHitkadmut.SetActive(false);//הסתרת מד התקדמות
        player.SetActive(false);
        allCloseAnim.SetActive(true);
        skipButton.SetActive(true);
        gameManager.timer.enabled = false;// הסתרת טיימר
        gameManager.progress.enabled = false;//הסתרת כמות תשובות נכונות
        playableDirector.Play();



    }

    public void Skip()
    {
        playableDirector.Stop();
        //allCloseAnim.SetActive(false); // כלל האובייקטים יוסרו מהמסך
        //skipButton.SetActive(false);
        //gameManager.StartGame();// קריאה לפונקציה שמתחילה את המשחק מתוך הגיים מנג'ר


    }

    private void OnTimelineStopped(PlayableDirector director)
    {
        skipButton.SetActive(false);
        allCloseAnim.SetActive(false); // כלל האובייקטים יוסרו מהמסך
        Debug.Log("Timeline finished, ending the game");
        end.EndGameScreen();

    }
    // Update is called once per frame
    void Update()
    {

    }
}
