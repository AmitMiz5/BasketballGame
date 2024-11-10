using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

using System.Threading.Tasks; //הוספת אפשרות להוספת נתיב א-סינכרוני
using UnityEngine.Networking;

public class ServerManagerScript : MonoBehaviour
{

	[SerializeField] GameObject startButton; //כפתור התחלה
	[SerializeField] GameManagerScript gameManager;// קישור לסקריפט גיים מנג׳ר
	[SerializeField] Sprite tempImage;//שמירת תמונה
	[SerializeField] private TextMeshProUGUI codeMessage;//טקסט להודעת שגיאה בקוד

	string projectURL = "./../"; // הנתיב לשרת - בילד

	//string projectURL = "https://localhost:7022/"; הנתיב לשרת - לוקאלי
	string apiURL = "api/Unity/GetGameDetails/"; //הנתיב לקונטרולר שיצרתם
	string imagesURL = "uploadedFiles/"; //הנתיב לתיקיית התמונות


	public async void CheckCode(string curCode)//פונקצייה לבדיקת תקינות הקוד
	{
		string code = curCode;//שמירת הקוד שהוקלד כמשתנה
		startButton.SetActive(false);//הסתרת כפתור התחלה

		GameData unityGame = await getDataFromServer(code);//שליפת נתוני המשחק מהשרת לפי הקוד

		if (unityGame == null) // בדיקה אם נתוני המשחק לא קיימים
		{
			codeMessage.text = "לא קיים משחק עם קוד זה";
			//הצגת שגיאה
			startButton.SetActive(true); // הצגת כפתור להקלדת ובדיקת קוד חדש
			return;
		}

		if (unityGame.isPublish == false) // בדיקה האם המשחק קיים אך לא פורסם
		{
			codeMessage.text = "המשחק קיים אך לא פורסם";
			//הצגת שגיאה למשתמש
			startButton.SetActive(true); // הצגת כפתור להקלדת ובדיקת קוד חדש
			return;
		}


		gameManager.GetGame(unityGame);//קריאה לפונקציה מהגיים מנג׳ר של בניית המשחק לפי הנתונים מהשרת




	}
	private async Task<GameData> getDataFromServer(string code) //קבלת הקוד שהוקלד ושליפת הנתונים לפיו
	{
		string endPoint = projectURL + apiURL + code;
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
			UnityGame.isPublish = serverGame.isPublish;
			UnityGame.gameName = serverGame.gameName;//server game לפי השמות בדאטה בייס
			UnityGame.questionTime = serverGame.questionTime;//קבלת זמן לכל שאלה
			UnityGame.questionList = new List<QuestionData>();//קבלת רשימת השאלות
			foreach (ServerQuestion question in serverGame.questions)
			{
				QuestionData unityQuestion = new QuestionData();
				unityQuestion.content = question.questionText;//קבלת טקסט לשאלות
				if (string.IsNullOrEmpty(question.questionPhoto) == false)//בדיקה האם קיימת תמונה לשאלה
				{
					unityQuestion.spriteContent = await LoadImage(question.questionPhoto);
				}

				unityQuestion.answerList = new List<AnswerData>();//רשימת התשובות
				foreach (ServerAnswer answer in question.answers)//לולאה לבדיקה בכל תשובה
				{
					AnswerData unityAnswer = new AnswerData();
					unityAnswer.isCorrect = answer.isCorrect;//מה היא התשובה הנכונה

					if (answer.isPhoto == true) //בדיקה האם תוכן התשובה הוא תמונה
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


			return UnityGame; // החזרת משחק


		}
		else
		{
			string errorMsg = http.downloadHandler.ToString();
			return null;
		}
	}

	private async Task<Sprite> LoadImage(string imageName) //טעינת תמונה מהשרת לפי שם התמונה
	{
		if (string.IsNullOrEmpty(imageName) == true) //בדיקה האם שם התמונה קיים
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