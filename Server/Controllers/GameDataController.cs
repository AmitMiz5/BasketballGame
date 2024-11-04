using Microsoft.AspNetCore.Mvc;
using template.Server.Data;
using template.Server.Helpers;
using template.Shared.Models.Game;
using template.Shared.Models.Question;
using template.Shared.Models.Answer;
using template.Shared.Models.Media;

namespace template.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(AuthCheck))]
    public class GameDataController : Controller
    {
        private readonly DbRepository _db;
        private readonly FilesManage _filesManage;
        private readonly string containerName;
        public GameDataController(DbRepository db, FilesManage filesManage)
        {
            _db = db;
            _filesManage = filesManage;
            containerName = "uploadedFiles";
        }

        [HttpGet("getQuestion/{questionId}")] // שליפת שאלה לפי מספר מזהה- להוסיף בדיקה ליוזר המחובר
        public async Task<IActionResult> GetQuestion(int questionId)
        {
            if (questionId <= 0)
            {
                return BadRequest("Invalid question ID");
            }

            // Query to get the question
            string getQuestionQuery = @"
        SELECT ID, QuestionText,QuestionPhoto
        FROM Questions 
        WHERE ID = @QuestionID;";

            var questionParam = new { QuestionID = questionId };
            var question = await _db.GetRecordsAsync<QuestionDetails>(getQuestionQuery, questionParam);

            if (question == null)
            {
                return NotFound("Question not found");
            }

            // Query to get the answers associated with the question
            string getAnswersQuery = @"
        SELECT ID, AnswerContent, IsPhoto, IsCorrect 
        FROM Answers 
        WHERE QuestionID = @QuestionID;";

            var answers = await _db.GetRecordsAsync<AnswerDetails>(getAnswersQuery, questionParam);

            var questionToAdd = question.FirstOrDefault();
            var response = new
            {
                questionToAdd,
                Answers = answers
            };

            return Ok(response);
        }


        [HttpPost("addQuestion")] // הוספת שאלה חדשה
        public async Task<IActionResult> addQuestion(int authUserId, QuestionToAdd newQuestion)
        {

            if (authUserId <= 0)
            {
                return Unauthorized("User is not authenticated");
            }

            if (string.IsNullOrEmpty(newQuestion.QuestionText) || newQuestion.QuestionText.Length < 2)
            {
                return BadRequest("Question text must be at least 2 characters long");
            }

            string questionPhoto = string.IsNullOrEmpty(newQuestion.QuestionPhoto) ? null : newQuestion.QuestionPhoto;

            object questionParam = new
            {
                QuestionText = newQuestion.QuestionText,
                QuestionPhoto = questionPhoto,
                GameID = newQuestion.GameID
            };

            string insertQuestionQuery = @"
        INSERT INTO Questions (QuestionText, QuestionPhoto, GameID) 
        VALUES (@QuestionText, @QuestionPhoto, @GameID);
        SELECT last_insert_rowid();";

            int newQuestionId = await _db.InsertReturnIdAsync(insertQuestionQuery, questionParam);
            if (newQuestionId <= 0)
            {
                return BadRequest("Failed to add the question");
            }

            List<int> newAnswerIds = new List<int>();
            foreach (var answer in newQuestion.Answers)
            {
                object answerParam = new
                {
                    AnswerContent = answer.AnswerContent,
                    IsPhoto = answer.IsPhoto,
                    IsCorrect = answer.IsCorrect,
                    QuestionID = newQuestionId
                };

                string insertAnswerQuery = @"
            INSERT INTO Answers (AnswerContent, IsPhoto, IsCorrect, QuestionID) 
            VALUES (@AnswerContent, @IsPhoto, @IsCorrect, @QuestionID);
            SELECT last_insert_rowid();";

                int newAnswerId = await _db.InsertReturnIdAsync(insertAnswerQuery, answerParam);
                if (newAnswerId > 0)
                {
                    newAnswerIds.Add(newAnswerId);
                }
            }

            if (newAnswerIds.Count != newQuestion.Answers.Count)
            {
                // Some answers failed to insert
                return BadRequest($"{newQuestion.Answers.Count - newAnswerIds.Count} answers failed to add");
            }

            await UpdateGameLastUpdated(newQuestion.GameID);
            return Ok(new { QuestionId = newQuestionId, AnswerIds = newAnswerIds });
        }


        [HttpGet("GetGameData/{GameID}")]
        public async Task<IActionResult> GetGameData(int GameID)
        {
            object param = new
            {
                GameID = GameID
            };

            string gameQuery = @"SELECT g.ID, g.GameName, g.GameCode, g.IsPublish, g.CanPublish,g.LastUpdated, g.QuestionTime                     FROM Games g
                     WHERE g.ID=@GameID";


            var gameRecord = await _db.GetRecordsAsync<GameTable>(gameQuery, param);
            var game = gameRecord.FirstOrDefault();

            bool canPublish = await TableController.CanPublishFunc(GameID, _db);
            game.CanPublish = canPublish;
            return Ok(game);
        }


        [HttpPost("GetQuestionList")] //שליפת רשימת השאלות 
        public async Task<IActionResult> GetQuestionList(int authUserId, GameQuestionRequest gameQuestionDto)
        {
            if (authUserId > 0)
            {
                object param = new
                {
                    UserId = authUserId,
                    GameID = gameQuestionDto.GameID
                };


                string questionQuery = "SELECT q.ID, q.QuestionText, q.QuestionPhoto FROM Questions q JOIN Games g ON q.GameID = g.ID WHERE g.UserId = @UserId AND q.GameID = @GameID";

                var questionRecords = await _db.GetRecordsAsync<QuestionDetails>(questionQuery, param);
                List<QuestionDetails> questionList = questionRecords.ToList();


                if (questionList.Count > 0)
                {
                    foreach (var question in questionList)
                    {

                        question.Answers = await GetAnswersForQuestion(question.ID);
                    }
                    await UpdateGameLastUpdated(gameQuestionDto.GameID);
                    return Ok(questionList);
                }
                else
                {
                    return BadRequest("No questions found for this user and game ID: ");
                }
            }
            else
            {
                return Unauthorized("User is not authenticated: " + authUserId);
            }
        }

        [HttpDelete("deleteQuestion/{gameID}/{questionID}")]
        public async Task<IActionResult> deleteQuestion(int gameID, int questionID)
        {
            if (gameID < 0 || questionID < 0)
            {
                return BadRequest("game ID or question ID in missing");
            }

            var deleteQuestionParam = new
            {
                GameID = gameID,
                ID = questionID
            };

            string deleteQuestionImgsQuery = "SELECT QuestionPhoto FROM Questions WHERE QuestionPhoto IS NOT NULL AND QuestionPhoto != ''";



            string deleteQuestionQuery = "DELETE from Questions WHERE ID =@ID and GameID =@GameID";
            int isDeleted = await _db.SaveDataAsync(deleteQuestionQuery, deleteQuestionParam);
            if (isDeleted > 0)
            {
                await UpdateGameLastUpdated(gameID);
                return Ok(isDeleted);
            }
            else
            {
                return BadRequest("question wasn't deleted");
            }

        }
        public async Task<List<AnswerDetails>> GetAnswersForQuestion(int questionId)
        {
            object param = new { QuestionID = questionId };

            // Query to fetch answers for the specific question
            string answerQuery = "SELECT a.ID, a.AnswerContent, a.IsPhoto, a.IsCorrect FROM Answers a WHERE a.QuestionID = @QuestionID";

            var answerRecords = await _db.GetRecordsAsync<AnswerDetails>(answerQuery, param);
            return answerRecords.ToList();
        }

        private async Task UpdateGameLastUpdated(int gameId)
        {
            string updateLastUpdatedQuery = "UPDATE Games SET LastUpdated = CURRENT_TIMESTAMP WHERE ID = @ID";
            var updateParams = new { ID = gameId };

            await _db.SaveDataAsync(updateLastUpdatedQuery, updateParams);
        }

    }



}





