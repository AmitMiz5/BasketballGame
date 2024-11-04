using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

using System.Threading.Tasks; //הוספת אפשרות להוספת נתיב א-סינכרוני
using UnityEngine.Networking;

public class ServerManagerScript : MonoBehaviour
{
    //[SerializeField] TMP_InputField codeInput;
    [SerializeField] GameObject startButton;
    [SerializeField] GameManagerScript gameManager;
    [SerializeField] StartScript startScript;
    [SerializeField] Sprite tempImage;
    [SerializeField] private TextMeshProUGUI codeMessage;

    string projectURL = "https://localhost:7022/"; //הנתיב לפרוייקט
    string apiURL = "api/Unity/GetGameDetails/"; //הנתיב לקונטרולר שיצרתם
    string imagesURL = "uploadedFiles/"; //הנתיב לתיקיית התמונות


    public async void CheckCode(string curCode)
    {
        string code = curCode;
        startButton.SetActive(false);

        GameData unityGame = await getDataFromServer(code);

        if (unityGame == null) // NEW: Check if game data was not retrieved
        {
            codeMessage.text = "לא קיים משחק עם קוד זה"; // NEW: Display error to user
            startButton.SetActive(true); // NEW: Reactivate start button for retry
            return;
        }

        if (unityGame.isPublish == false) // NEW: Check if game is not published
        {
            codeMessage.text = "המשחק קיים אך לא פורסם"; // NEW: Display error to user
            startButton.SetActive(true); // NEW: Reactivate start button for retry
            return;
        }


        gameManager.GetGame(unityGame);




    }
    private async Task<GameData> getDataFromServer(string code)
    {
        string endPoint = projectURL + apiURL + code;
        Debug.Log(projectURL + apiURL + code);
        using var http = UnityWebRequest.Get(endPoint);
        var get = http.SendWebRequest();

        while (!get.isDone)
        {
            await Task.Yield();
        }
        if (http.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = http.downloadHandler.text;
            ServerGame serverGame = JsonUtility.FromJson<ServerGame>(jsonResponse);
            GameData UnityGame = new GameData(); //יצירת משחק חדש
            UnityGame.gameName = serverGame.gameName;//server game לפי השמות בדאטה בייס
            UnityGame.questionTime = serverGame.questionTime;
            UnityGame.questionList = new List<QuestionData>();
            foreach (ServerQuestion question in serverGame.questions)
            {
                Debug.Log("here: " + question);
                QuestionData unityQuestion = new QuestionData();
                unityQuestion.content = question.questionText;
                if (string.IsNullOrEmpty(question.questionPhoto) == false)
                {
                    unityQuestion.spriteContent = await LoadImage(question.questionPhoto);
                }

                unityQuestion.answerList = new List<AnswerData>();
                foreach (ServerAnswer answer in question.answers)
                {
                    AnswerData unityAnswer = new AnswerData();
                    unityAnswer.isCorrect = answer.isCorrect;

                    if (answer.isPhoto == true)
                    {
                        unityAnswer.spriteContent = await LoadImage(answer.answerContent);
                    }
                    else
                    {
                        unityAnswer.textContent = answer.answerContent;
                    }
                    unityQuestion.answerList.Add(unityAnswer);
                }
                UnityGame.questionList.Add(unityQuestion);
            }
            Debug.Log(jsonResponse);

            // Start the game after successful data retrieval
            //startScript.StartGame(); // מתחיל את המשחק לאחר שליפה מוצלחת
            return UnityGame; // החזרת משחק


        }
        else
        {
            string errorMsg = http.downloadHandler.ToString();
            Debug.Log(errorMsg);
            return null;
        }
    }

    private async Task<Sprite> LoadImage(string imageName)
    {
        if (string.IsNullOrEmpty(imageName) == true)
        {
            return null;
        }

        string endPointImg = projectURL + imagesURL + imageName;
        using var httpImg = UnityWebRequestTexture.GetTexture(endPointImg);
        var get = httpImg.SendWebRequest();

        while (get.isDone == false)
        {
            await Task.Yield();
        }
        if (httpImg.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(httpImg);
            if (texture == null)
            {
                Debug.Log("no image");
                return tempImage;
            }

            Rect spriteRect = new Rect(0, 0, texture.width, texture.height);// מיקום
            Vector2 spritePivot = new Vector2(0.5f, 0.5f);// דחיפת התמונה לאמצע המיקום
            Sprite sprite = Sprite.Create(texture, spriteRect, spritePivot);
            return sprite;
        }
        else
        {
            Debug.Log("no image");
            return tempImage;
        }
    }

}

[System.Serializable]
public class ServerGame
{
    public int iD;
    public string gameCode;
    public string gameName;
    public bool isPublish;
    public int questionTime;
    public List<ServerQuestion> questions;
}

[System.Serializable]
public class ServerQuestion
{
    public int iD;
    public string questionText;
    public string questionPhoto;
    public int gameID;
    public List<ServerAnswer> answers;
}

[System.Serializable]
public class ServerAnswer
{
    public int iD;
    public int questionID;
    public string answerContent;
    public bool isPhoto;
    public bool isCorrect;
}