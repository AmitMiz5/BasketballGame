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
    public GameObject skipButton;


    // Start is called before the first frame update
    void Start()
    {

        playableDirector = GetComponent<PlayableDirector>();
        playableDirector.stopped += OnTimelineStopped;

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayOpenAnim() //פונקציה להתחלת אנימציית פתיחה
    {

        Debug.Log("Starting opening animation");
        allOpenAnim.SetActive(true);
        skipButton.SetActive(true);
        playableDirector.Play();
        Debug.Log("Timeline started");




    }

    public void Skip()
    {
        playableDirector.Stop();
        allOpenAnim.SetActive(false); // כלל האובייקטים יוסרו מהמסך
        skipButton.SetActive(false);
        //gameManager.StartGame();// קריאה לפונקציה שמתחילה את המשחק מתוך הגיים מנג'ר


    }

    private void OnTimelineStopped(PlayableDirector director)
    {
        allOpenAnim.SetActive(false); // כלל האובייקטים יוסרו מהמסך
        Debug.Log("Timeline finished, starting the game");
        //allGameManager.SetActive(true);
        //gameManager.madHitkadmut.SetActive(true);
        gameManager.StartGame();// קריאה לפונקציה שמתחילה את המשחק מתוך הגיים מנג'ר

    }


}
