using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.UI;

public class GameManagerScript : MonoBehaviour
{

    public int totalQuestionCount; // כמות סך כל השאלות

    
    [SerializeField] private QuestionImageScript questionImageScript; //קישור לסקריפט הגדלת תמונות בשאלה

    [SerializeField] private GameObject answerPrefab; // פריפאבים של התשובות
    [SerializeField] private TextMeshPro questionTxt; // טקסט השאלה
    [SerializeField] private SpriteRenderer questionImage; // תמונת השאלה
    [SerializeField] public TextMeshProUGUI progress; // התקדמות במשחק- חישוב התשובות הנכונות מתוך כלל השאלות במשחק
    [SerializeField] public TextMeshProUGUI timer; // טקסט של טיימר עבור כל שאלה
    [SerializeField] public GameObject nextBtn; // כפתור שאלה הבאה
    [SerializeField] private TextMeshProUGUI nextBtnTxt; // טקסט של כפתור שאלה הבאה
    [SerializeField] public GameObject pauseButton; // כפתור להשהיית המשחק
    [SerializeField] private GameObject continueButton; // כפתור לחזרה למשחק
    [SerializeField] private float distance; // מרחק בין אובייקטים
    [SerializeField] private EndScript endSpcript; // קישור לסקירפט סיום
    [SerializeField] private TimeEnd timeEndScript; // קישור לסקריפט של נגמר זמן לשאלה
    [SerializeField] private StartScript startGameScript; // קישור לסקריפט של תחילת המשחק
    public GameData game; // נתוני המשחק - שאלות תשובות
    public QuestionData question; // נתוני השאלות
    [SerializeField] public AudioClip bgSound; // סאונד רקע
    [SerializeField] public AudioSource bgSoundSource; // מקור הסאונד

    public PlayerScript player; // סקריפט השחקן
    public AnswerScript answerScript; // סקריפט התשובות
    public bool audioSound = true; // האם הסאונד פעיל
    public List<QuestionData> tempQusetionList; //רשימת שאלות זמנית על מנת לא לשנות את הרשימה המקורית
    public float questionTime; // זמן השאלה
    public DateTime startTime;// בדיקה מתי המשחק התחיל לחישוב זמן כולל למשחק
    public List<GameObject> ansOnScreen; // רשימת המסיחים על המסך
    public bool activeTimer; // בדיקה האם הטיימר פועל
    public int countCorrectAnswer; // כמות תשובות נכונה 
    public int totalWrongAnswers; // כמות שאלות לא נכונה
    public float timeUsedInCurQues;// חישוב זמן כולל למשחק
    public float totalTimeAllowedPerQuestion = 30f; // הגדרת הזמן לכל שאלה
    public int randomIndex; // אינדקס רנדומלי לשאלות
    public int ansrandomIndex; // אינקס רנדומלי לתשובות
    private bool isGameStarted; // בדיקה האם משחק התחיל
    [SerializeField] private Slider progressSlider; //אובייקט מסוג סליידר -מד ההתקדמות
    public GameObject madHitkadmut;// משתנה לשמירת מד התקדמות
    public Progress progressBar;//יכיל את הסליידר ואת כמות השאלות
    public float score;//משתנה לשמירת הציון
    //[SerializeField] TextMeshProUGUI scoreTxt;//טקסט בו יופיע הציון
    private int initialQuestionCount;//כמות השאלות המקורית
    public GameObject crowdBlur;// קהל סטטי
    public GameObject crowdCheer;// קהל תשובה נכונה
    public GameObject correct;// חיווי טקסט תשובה נכונה
    public GameObject incorrect;// חיווי טקסט תשובה לא נכונה
    public GameObject gamePaused;// רקע השהיית משחק
    public EndAnim endAnim; // קישור לסקריפט אנימציית סיום

    [SerializeField] private TMP_InputField inputField; // שדה להזנת קוד המשחק
    [SerializeField] private GameObject allStartScreen; // כלל האובייקטים של מסך פתיחה
    [SerializeField] private GameObject allGameManager; // כלל האובייקטים של מסך פתיחה
    [SerializeField] private GameObject playerObj; // כלל האובייקטים של מסך פתיחה

    public OpenAnim openAnim; //  קישור לסקריפט אנימציית פתיחה




    void Start() // פונקצית התחילה
    {

        bgSoundSource.clip = bgSound; // הגדרת מקור השמע לסאונד רקע
        SetAoudioBG(); // קישור לפונקציה המפעילה את הסאונד
        if (questionImageScript != null) //בדיקה שסקריפט הגדלת תמונה מחובר
        {
            questionImageScript.questionImage = questionImage;
        }
        else
        {
            Debug.LogError("QuestionImageScript is not assigned in the GameManagerScript!");
        }

    }

    void Update()
    {
        if (activeTimer == true && isGameStarted == true) // אם הטיימר פעיל
        {
            questionTime -= Time.deltaTime; //ממיר שניות בתוך האפדייט
            timer.text = Mathf.Ceil(questionTime).ToString();// עיגול המספר כלפי מעלה ושרשור כטקסט לתוך הטיימר על המסך
            timeUsedInCurQues = 30 - questionTime;

            if (questionTime <= 0) //אם נגמר הזמן
            {
                activeTimer = false;//להפוך את הטיימר ללא פעיל
                questionTxt.text = "נגמר הזמן";
                player.StopPlayer(); // קריאה לפונקציית עצירת שחקן מתוך סקריפט של השחקן
                nextBtn.SetActive(true);// הפעלת כפתור השאלה הבאה
                timeEndScript.timeEndSetActive(); // הפעלת מסך נגמר הזמן

            }
        }

    }

    public void PauseGame()// פונקציה לעצירת משחק באמצע
    {
        gamePaused.SetActive(true);//הצגת הרקע של עצירת המשחק
        activeTimer = false; //טיימר לא פעיל
        continueButton.SetActive(true);//הצגת כפתור לחזרה למשחק
        pauseButton.SetActive(false);//הסתרת כפתור עצירה
        timer.enabled = false;// הסתרת טיימר
        progress.enabled = false;//הסתרת כמות תשובות נכונות

    }

    public void ContinueGame()// פונקציה להמשך משחק
    {
        gamePaused.SetActive(false);//הסתרת רקע של עצירת המשחק
        continueButton.SetActive(false);//הסתרת כפתור של חזרה למשחק
        pauseButton.SetActive(true);//החזרת כפתור עצירה
        timer.enabled = true;//החזרת טיימר
        progress.enabled = true;//החזרת כמות תשובות נכונות
        NextQuestion();// קריאה לפונקציית שאלה הבאה

    }

    public void SetAoudioBG() // פונקציה להפעלת סאונד
    {
        audioSound = true; // עדכון כי כפתור הסאונד לחוץ ויש להשמיע את המוזיקה

        bgSoundSource.Play(); // הפעלת המוזיקה
        answerScript.feedbackAudio.Play(); //   הפעלת המוזיקה של תשובה נכונה או לא נכונה


    }

    public void StopAoudioBG() // פונקציה להפסקת הסאונד
    {
        audioSound = false; // עדכון כי כפתור הסאונד אינו לחוץ ויש להשמיע את המוזיקה
        bgSoundSource.Stop(); // הפסקת המוזיקה
        answerScript.feedbackAudio.Stop(); // הפסקת מוזיקה של תשובה נכונה או לא נכונה
    }


    public void StartGame() // פונקציה להפעלת משחק
    {
        madHitkadmut.SetActive(true);//הצגת מד ההתקדמות על המסך
        pauseButton.SetActive(true);//הצגת כפתור עצירה על המסך
        allGameManager.SetActive(true);// הצגת כלל האובייקטים על המסך
        playerObj.SetActive(true);//הצגת השחקנית על המסך
        timer.enabled = true;// הצגת טיימר
        progress.enabled = true;//הצגת כמות תשובות נכונות
        progressBar = new Progress(progressSlider, totalQuestionCount);//יצירת אובייקט חדש שינהל את מד ההתקדמות
        countCorrectAnswer = 0; // איפוס כמות התשובות הנכונות
        nextBtnTxt.text = "שאלה הבאה";// הגדרת מלל בפתור לשאלה הבאה
        activeTimer = false; //טיימר לא פעיל
        tempQusetionList.Clear(); //  ניקוי רשימת השאלות הזמנית 
        foreach (var question in game.questionList) // לולאה על כל השאלות במשחק
        {
            tempQusetionList.Add(question); // הוספת השאלות לרשימה הזמנית
        }

        totalQuestionCount = tempQusetionList.Count;// עדכון מספר השאלות הכולל
        initialQuestionCount = totalQuestionCount;//שמירת המספר ההתחלתי של השאלות ברשימה הזמנית
        progress.text = "0/" + totalQuestionCount.ToString(); //  עדכון סך השאלות הנכונות שנענו במשחק מתוך סך השאלות 
        startTime = DateTime.Now; //  הזמן של התחלת המשחק
        isGameStarted = true; // סימן שהמשחק התחיל
        CreateQuestion(); // קריאה לפונקצית יצירת שאלה
        progressBar.resetProgressBar(); // אתחול מד ההתקדמות לתחילת המשחק
        resetScore(); // איפוס הניקוד לתחילת המשחק




    }

    public void StartGameOver() // פונקציה להתחלת משחק מחדש
    {

        startGameScript.StartOver(); // קריאה לפונקציה מסקריפט הפתיחה לאיפוס משחק



    }



    public void RemoveQuestion() // הסרת השאלות שנענו נכון 
    {
        int indexToRemove = (int)randomIndex; // אינדקס להסרת השאלה

        if (indexToRemove >= 0 && indexToRemove < tempQusetionList.Count) // בדיקת תקינות האינדקס
        {
            tempQusetionList.RemoveAt(indexToRemove); // הסרת השאלה מהרשימה הזמנית
        }

    }

    public void SetTimerActive() // הפעלת טיימר
    {
        activeTimer = true; // הפעלת טיימר
    }

    public void SetTimerInactive() // הפסקת הטיימר
    {
        activeTimer = false; // הפסקת הטיימר
    }


    private List<AnswerData> RandomAnswers(List<AnswerData> originalList) // רשיממת מסיחים רנדומלית
    {
        List<AnswerData> randomList = new List<AnswerData>(originalList); // יצירת רשימת תשובות רנדומלית

        for (int i = 0; i < randomList.Count; i++) // לולאה על כל התשובות ברשימה
        {
            int randomIndex = UnityEngine.Random.Range(i, randomList.Count); // בחירת אינדקס רנדומלי
            AnswerData temp = randomList[i]; // שמירת התשובה הנוכחית
            randomList[i] = randomList[randomIndex]; // העברת התשובה הרנדומלית למיקום הנוכחי
            randomList[randomIndex] = temp; // העברת התשובה למיקום הרנדומלי
        }
        return randomList; // החזרת הרשימה הרנדומלית
    }

    public void ProgressCounter()  // כמות תשובות נכונות
    {
        countCorrectAnswer++; // העלאת כמות התשובות הנכונות באחד
        if (countCorrectAnswer == totalQuestionCount) // בדיקה האם כמות התשובות הנכונה שווה לכמות שאלות במשחק
        {
            nextBtnTxt.text = "סיים משחק"; // שינוי טקסט כפתור לסיים משחק במקום שאלה הבאה
        }
        progress.text = (countCorrectAnswer).ToString() + "/" + totalQuestionCount.ToString(); // עדכון טקסט של התקדמות המשחק
        progressBar.incrementProgress(initialQuestionCount); //עדכון מד ההתקדמות לפי התקדמות התשובות הנכונות


    }
    private void CreateQuestion() // יצירת שאלה
    {
        SetTimerActive(); // הפעלת טיימר לזמן מענה על השאלה
        randomIndex = UnityEngine.Random.Range(0, tempQusetionList.Count); // בחירת שאלה רנדומלית
        ansOnScreen = new List<GameObject>(); // ניקוי רשימת התשובות מהמסך
        QuestionData CurrentQuestion = tempQusetionList[randomIndex]; // השאלה הנוכחית מתוך הרשימה הזמנית
        questionTxt.text = CurrentQuestion.content; // הצגת תוכן השאלה
        questionImage.sprite = CurrentQuestion.spriteContent; // הצגת תמונת השאלה
        List<AnswerData> answerList = RandomAnswers(CurrentQuestion.answerList); // רשימת התשובות הרנדומלית
        int answerCount = answerList.Count; // מספר המסיחים
        float distance = 3.0f; // מרחק בין המסיחים

        for (int i = 0; i < answerCount; i++) // לולאה על כל התשובות
        {
            AnswerData theAnswer = answerList[i]; // התשובה הנוכחית
            float offset = (answerCount - 1) * distance * 0.5f; // מיקום התשובה על המסך
            float x = i * distance - offset;// מיקום אופקי
            Vector2 position = new Vector2(x, -3);// מיקום תשובה חדשה
            GameObject newAnswerFab = Instantiate(answerPrefab, position, Quaternion.identity); // יצירת מיקום תשובה חדשה

            AnswerScript curAnsScript = newAnswerFab.GetComponent<AnswerScript>(); // רשימת המסיחים של השאלה הנוכחית
            if (theAnswer.spriteContent != null) // בדיקה האם קיימת תמונה במסיח
            {
                curAnsScript.answerImage.sprite = theAnswer.spriteContent; // הצגת התמונה
                curAnsScript.answerImage.gameObject.SetActive(true); //אקטיב לתמונה
            }
            else
            {
                curAnsScript.answerTxt.text = theAnswer.textContent; // הצגת טקסט
            }

            curAnsScript.isCorrect = theAnswer.isCorrect; // בדיקה האם התשובה נכונה
            curAnsScript.gameManager = this; // קישור לגיים מנג'ר
            ansOnScreen.Add(newAnswerFab); // הוספת הפריפאבים על המסך


        }


        questionTime = game.questionTime; //כמה זמן יש לכל שאלה


        questionImage.sprite = CurrentQuestion.spriteContent; // הגדרת תמונה לשאלה הנוכחית

        if (questionImageScript != null)
        {
            questionImageScript.ResetScale(); // איפוס גודל התמונה
        }
        

    }

    public void NextQuestion() // מעבר לשאלה הבאה
    {
        foreach (GameObject answer in ansOnScreen) // לולאה על כל התשובות במסך
        {
            Destroy(answer); // הסרתן מהמסך
        }
        incorrect.SetActive(false);//הסתרת טקסט
        correct.SetActive(false);// הסתרת טקסט
        crowdCheer.SetActive(false); //הסתרת הקהל של תשובה נכונה
        crowdBlur.SetActive(true);// החזרת קהל סטטי
        nextBtn.SetActive(false); // הסתרת הכפתור לשאלה הבאה
        player.ReturnPlayerToDefaultPosition(); // החזרת השחקן למיקום הראשי
        player.AllowPlayerToMove(); // הרשאת תנועת השחקן

        if (countCorrectAnswer == totalQuestionCount) //אם כמות התשובות הנכונות שווה לכמות השאלות במשחק
        {
            // שינוי טקסט השאלה ל״המשחק הסתיים״
            questionTxt.text = "המשחק הסתיים";

            if (endAnim != null)
            {
                endAnim.PlayCloseAnim(); //הפעלת אנימציית סיום
            }
            else 
            {
                Debug.LogError("EndAnim reference is missing! Please assign it in the Inspector.");
            }
        }
        else // קישור לפונקציית יצירת שאלה נוספת
        {
            CreateQuestion();
        }
    }


    public Vector2 GetNextPodiumInDirection(bool isLeft) // קבלת המיקום הבא לשחקן בכיוון מסוים
    {

        Vector2 currentPlayerPosition = player.transform.position; // מיקום השחקן הנוכחי
        GameObject closestPodium = FindClosestPodium(currentPlayerPosition, isLeft); // מציאת הפודיום הקרוב ביותר לשחקן

        if (closestPodium != null) // אם נמצא פודיום
        {
            float nextXPosition = closestPodium.transform.position.x; // המיקום האפשרי הבא בציר X
            return new Vector2(nextXPosition, currentPlayerPosition.y); // החזרת מיקום חדש
        }
        else // אחרת
        {
            return currentPlayerPosition;// החזרת המיקום הנוכחי של השחקן
        }
    }

    private GameObject FindClosestPodium(Vector2 playerPosition, bool isLeft) // מציאת הפודיום הקרוב ביותר לשחקן
    {
        GameObject closestPodium = null; // פודיום הקרוב ביותר
        float closestDistance = Mathf.Infinity; // המרחק הקרוב ביותר

        foreach (GameObject podium in ansOnScreen) // לולאה על כל הפודיומים במסך
        {
            Vector2 podiumPosition = podium.transform.position; // מיקום הפודיום
            float distanceToPodium = Mathf.Abs(podiumPosition.x - playerPosition.x);  // המרחק לפודיום

            // בדיקת התנאים למציאת הפודיום הקרוב ביותר בכיוון המבוקש
            if ((isLeft && podiumPosition.x < playerPosition.x && distanceToPodium < closestDistance) ||
                (!isLeft && podiumPosition.x > playerPosition.x && distanceToPodium < closestDistance))
            {
                closestPodium = podium; // עדכון פודיום הקרוב ביותר
                closestDistance = distanceToPodium; // עדכון המרחק הקרוב ביותר
            }
        }

        return closestPodium; // החזרת הפודיום הקרוב ביותר לשחקן
    }

    public void UpdateScore(bool answer) // עדכון ניקוד
    {
        Debug.Log("question number" + randomIndex);
        Debug.Log("answer: " + answer);

        if (answer == true) // אם התשובה נכונה
        {
            Debug.Log("Points for current question: " + 100 / (tempQusetionList[randomIndex].tryCount * totalQuestionCount));
            // חישוב הניקוד שניתן לשאלה זו
            score += 100 / (tempQusetionList[randomIndex].tryCount * totalQuestionCount);
            tempQusetionList.RemoveAt(randomIndex);  // הסרת השאלה הנוכחית מהרשימה כדי שלא תחזור שוב
        }
        else // אם התשובה שגויה, נעדכן את ספירת הניסיונות עבור השאלה
        {
            tempQusetionList[randomIndex].tryCount++;
        }

        Debug.Log("score:" + score);

    }

    public void resetScore() //איפוס הציון
    {
        score = 0;
    }


    public void GetGame(GameData gameFromServer) // פונקציה לקבלת נתוני משחק מהשרת
    {
        game = gameFromServer; // שמירת נתוני המשחק שהתקבלו מהשרת במשתנה המקומי game
        randomIndex = 0; // אתחול האינדקס הרנדומלי ל - 0
        allStartScreen.SetActive(false); // העלמת כל האובייקטים
        inputField.text = ""; // ניקוי תיבת הטקסט
        openAnim.PlayOpenAnim(); // קריאה לפונקציה שמתחילה את אנימציית הפתיחה
    }

    public void RepeatGame() //פונקצייה להתחלת אותו המשחק מחדש
    {
        endSpcript.allEndScreen.SetActive(false);//הסתרת כל האובייקטים של מסך הסיום

        StartGame();// קריאה לפונקציית התחלת המשחק
    }

}




[System.Serializable]
public class AnswerData // תוכן תשובות
{
    public string textContent;  // תוכן התשובה בפורמט טקסט
    public Sprite spriteContent; // תוכן התשובה בפורמט תמונה
    public bool isCorrect;// בדיקה האם נכונה או לא
}

[System.Serializable]
public class QuestionData // תוכן שאלות
{
    public string content;// תוכן השאלה בפורמט טקסט
    public Sprite spriteContent;// תוכן השאלה בפורמט תמונה
    public List<AnswerData> answerList; // רשימת התשובות לשאלה זו
    public int tryCount = 1;//ספירת כמות נסיונות לשאלה זו
}

[System.Serializable]
public class GameData // תוכן המשחק
{
    public string gameName; // שם המשחק
    public int questionTime; // הזמן המקסימלי לכל שאלה
    public List<QuestionData> questionList; // רשימת השאלות במשחק
    public bool isPublish;


}