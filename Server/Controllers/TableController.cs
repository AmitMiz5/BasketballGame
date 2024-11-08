using Microsoft.AspNetCore.Mvc;
using template.Server.Helpers;
using template.Shared.Models.Game;
using template.Shared.Models.Question;
using template.Shared.Models.Answer;
using template.Server.Data;
using template.Client.Pages; // קישור DB רפוזיטורי

namespace template.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(AuthCheck))]
    public class TableController : Controller
    {
        private readonly DbRepository _db;
        public TableController(DbRepository db)
        {
            _db = db;
        }


        [HttpPost("updateGameSettings")]
        public async Task<IActionResult> updateGameSettings(int authUserId, GameTable game)
        {
            if (authUserId > 0)
            {
                object settingsParam = new
                {
                    game.GameName,
                    game.ID,
                    game.QuestionTime,
                    UserId = authUserId

                };

                string updateGameQuery = "UPDATE Games SET GameName=@GameName , QuestionTime=@QuestionTime,LastUpdated=CURRENT_TIMESTAMP WHERE ID=@ID AND UserId=@UserId";

                int updatedRecords = await _db.SaveDataAsync(updateGameQuery, settingsParam);
                bool UpdateGameSuccess = updatedRecords > 0;

                if (UpdateGameSuccess)
                {
                    return Ok(game);
                }
                else
                {
                    return BadRequest("Game Settings not change");
                }
            }
            else
            {
                return Unauthorized("user is not authenticated");
            }


        }



        [HttpPost("updateGameIsPublish")]
        public async Task<IActionResult> updateGameIsPublish(int authUserId, [FromBody] PublishGame game)
        {

            if (authUserId > 0)
            {
                object param = new
                {
                    UserId = authUserId,
                    gameID = game.ID
                };


                string checkQuery = "SELECT GameName FROM Games WHERE UserId = @UserId and ID=@gameID";
                var checkRecords = await _db.GetRecordsAsync<string>(checkQuery, param);
                string gameName = checkRecords.FirstOrDefault();

                if (gameName != null)
                {
                    if (game.IsPublish == true)
                    {

                        bool canPublish = await CanPublishFunc(game.ID, _db);

                        if (canPublish == false)
                        {
                            return BadRequest("thisGameCannotBePublished");
                        }

                    }

                    object updateParam = new
                    {
                        IsPublish = game.IsPublish,
                        ID = game.ID
                    };
                    string updateQuery = "UPDATE Games SET IsPublish=@IsPublish,LastUpdated=CURRENT_TIMESTAMP WHERE ID=@ID";
                    int isUpdate = await _db.SaveDataAsync(updateQuery, updateParam);
                    if (isUpdate == 1)
                    {
                        return Ok("Updated");
                    }
                    return BadRequest("Update Failed");

                }

                return BadRequest("It's Not Your Game or Game doesnt exsist");
            }
            else
            {
                return Unauthorized("user is not authenticated");
            }

        }




        [HttpPost] // שיטה לבדיקה של הפונקציה 
        [Route("canPublish")]
        public async Task<IActionResult> CanPublish([FromBody] int gameId)
        {
            bool result = await CanPublishFunc(gameId, _db);
            return Ok(result);
        }

        public static async Task<bool> CanPublishFunc(int gameId, DbRepository db)
        {
            int minQuestions = 2;
            bool canPublish = false;

            var param = new { ID = gameId };

            string queryQuestionCount = "SELECT Count(ID) from Questions WHERE GameID=@ID";
            var recordQuestionCount = await db.GetRecordsAsync<int>(queryQuestionCount, param);
            int numberOfQuestions = recordQuestionCount.FirstOrDefault();

            string queryAnswersCount = "SELECT COUNT(*) FROM (SELECT q.ID FROM Questions q, Answers a WHERE q.ID = a.QuestionID AND q.GameID=@ID GROUP BY q.ID HAVING COUNT(a.ID)>= 2)";
            var recordAnswersCountt = await db.GetRecordsAsync<int>(queryAnswersCount, param);
            int numberOfAnswers = recordAnswersCountt.FirstOrDefault();

            string updateQuery;

            if (numberOfQuestions >= minQuestions && numberOfAnswers >= minQuestions && numberOfQuestions == numberOfAnswers)
            {
                canPublish = true;
                updateQuery = "UPDATE Games SET CanPublish=true WHERE ID=@ID";
            }
            else
            {
                updateQuery = "UPDATE Games SET IsPublish=false, CanPublish=false WHERE ID=@ID";
            }

            await db.SaveDataAsync(updateQuery, param);
            return canPublish;
        }

        [HttpPost("deletGame")] // פעולה למחיקת משחק
        public async Task<IActionResult> DeleteGame(int authUserId, GameDelete gameDeleteDto)
        {
            if (authUserId > 0)
            {
                Console.WriteLine(gameDeleteDto.GameCode);

                if (gameDeleteDto.GameCode > 0)
                {
                    object param = new
                    {
                        GameCode = gameDeleteDto.GameCode,
                        UserId = authUserId
                    };
                    string queryCheckExistence = "SELECT ID FROM Games WHERE UserId = @UserId and GameCode=@GameCode";
                    var gameExistence = await _db.GetRecordsAsync<GameDelete>(queryCheckExistence, param);
                    var game = gameExistence.FirstOrDefault();

                    Console.WriteLine(game);

                    if (game == null)
                    {
                        return BadRequest("אין משחק עם קוד " + gameDeleteDto.GameCode);
                    }
                    int gameId = game.ID;
                    var paramGame = new
                    {
                        ID = gameId,
                        UserId = authUserId
                    };
                    string deleteGameQuery = "DELETE FROM Games WHERE ID = @ID AND UserId=@UserId";
                    int isDeleted = await _db.SaveDataAsync(deleteGameQuery, paramGame);

                    if (isDeleted > 0)
                    {
                        return Ok("המשחק נמחק בהצלחה");
                    }
                    return BadRequest("לא ניתן למחוק את המשחק");
                }
                else
                {
                    return BadRequest("הזן מספר גדול מ-0" + gameDeleteDto.GameCode);
                }
            }
            else
            {
                return Unauthorized("user is not authenticated");
            }


        }


        [HttpPost("addGame")] // הוספת משחק חדש לרשימת המשחקים
        public async Task<IActionResult> AddGame(int authUserId, GameToAdd gameToAdd)
        {
            if (authUserId > 0)
            {
                object newGameParam = new
                {
                    GameName = gameToAdd.GameName,
                    GameCode = 0,
                    IsPublish = false,
                    QuestionTime = 60,
                    UserId = authUserId,
                    CanPublish = false
                };
                string insertGameQuery = "INSERT INTO Games (GameName, GameCode, IsPublish, QuestionTime, UserId, CanPublish) " +
                        "VALUES (@GameName, @GameCode, @IsPublish, @QuestionTime, @UserId, @CanPublish)";
                int newGameId = await _db.InsertReturnIdAsync(insertGameQuery, newGameParam);
                if (newGameId != 0)
                {
                    int gameCode = newGameId + 1000;
                    object updateParam = new
                    {
                        ID = newGameId,
                        GameCode = gameCode
                    };
                    string updateCodeQuery = "UPDATE Games SET GameCode = @GameCode,LastUpdated=CURRENT_TIMESTAMP WHERE ID=@ID";
                    int isUpdate = await _db.SaveDataAsync(updateCodeQuery, updateParam);
                    if (isUpdate > 0)
                    {
                        object param2 = new
                        {
                            ID = newGameId
                        };
                        string gameQuery = "SELECT ID, GameName, GameCode, IsPublish, CanPublish FROM Games WHERE ID = @ID";
                        var gameRecord = await _db.GetRecordsAsync<GameTable>(gameQuery, param2);
                        GameTable newGame = gameRecord.FirstOrDefault();
                        return Ok(newGame);
                    }
                    return BadRequest("Game code not created");
                }
                return BadRequest("Game not created");
            }
            else
            {
                return Unauthorized("user is not authenticated");
            }
        }


        [HttpPost("copyGame/{originalGameId}")]
        public async Task<IActionResult> CopyGame(int authUserId, int originalGameId)
        {
            Console.WriteLine(originalGameId);
            if (authUserId <= 0)
            {
                return Unauthorized("User is not authenticated");
            }

            object param = new
            {
                ID = originalGameId
            };

            string gameQuery = "SELECT * FROM Games WHERE ID = @ID";
            var originalGame = await _db.GetRecordsAsync<GameTable>(gameQuery, param);
            var gameToCopy = originalGame.FirstOrDefault();

            if (gameToCopy == null)
                return NotFound("Original game not found");

            object newGameParam = new
            {
                GameName = gameToCopy.GameName + " copy",
                GameCode = 0,
                IsPublish = false,
                QuestionTime = gameToCopy.QuestionTime,
                UserId = authUserId,
                CanPublish = gameToCopy.CanPublish
            };

            string insertGameQuery = "INSERT INTO Games (GameName, GameCode, IsPublish, QuestionTime, UserId, CanPublish) " +
                                     "VALUES (@GameName, @GameCode, @IsPublish, @QuestionTime, @UserId, @CanPublish)";
            int newGameId = await _db.InsertReturnIdAsync(insertGameQuery, newGameParam);

            if (newGameId == 0)
            {
                return BadRequest("Game copy not created");
            }


            int gameCode = newGameId + 1000;
            string updateCodeQuery = "UPDATE Games SET GameCode = @GameCode, LastUpdated = CURRENT_TIMESTAMP WHERE ID = @ID";
            await _db.SaveDataAsync(updateCodeQuery, new { ID = newGameId, GameCode = gameCode });

            string questionQuery = "SELECT ID, QuestionText, QuestionPhoto FROM Questions WHERE GameID = @GameID";
            var questionsToCopy = await _db.GetRecordsAsync<QuestionDetails>(questionQuery, new { GameID = originalGameId });


            foreach (var question in questionsToCopy)
            {
                object newQuestionParam = new
                {
                    GameID = newGameId,
                    QuestionText = question.QuestionText,
                    QuestionPhoto = question.QuestionPhoto
                };

                string insertQuestionQuery = "INSERT INTO Questions (GameID, QuestionText, QuestionPhoto) VALUES (@GameID, @QuestionText, @QuestionPhoto)";
                int newQuestionId = await _db.InsertReturnIdAsync(insertQuestionQuery, newQuestionParam);

                if (newQuestionId > 0)
                {
                    var answers = await GetAnswersForQuestion(question.ID);
                    foreach (var answer in answers)
                    {
                        object newAnswerParam = new
                        {
                            QuestionID = newQuestionId,
                            AnswerText = answer.AnswerContent,
                            IsCorrect = answer.IsCorrect,
                            IsPhoto = answer.AnswerContent?.Contains(".png") == true
                        };

                        string insertAnswerQuery = "INSERT INTO Answers (QuestionID, AnswerContent, IsCorrect, IsPhoto) " +
                                                   "VALUES (@QuestionID, @AnswerText, @IsCorrect, @IsPhoto)";
                        await _db.SaveDataAsync(insertAnswerQuery, newAnswerParam);
                    }
                }
            }

            string newGameQuery = @"SELECT g.ID, g.GameName, g.GameCode, g.IsPublish, g.CanPublish,g.LastUpdated, g.QuestionTime, 
                     COUNT(q.ID) AS QuestionsAmount
                     FROM Games g
                     LEFT JOIN Questions q ON g.ID = q.GameId
                     WHERE g.ID = @ID
                     GROUP BY g.ID, g.GameName, g.GameCode, g.IsPublish, g.CanPublish, g.QuestionTime, g.LastUpdated";

            var newGame = await _db.GetRecordsAsync<GameTable>(newGameQuery, new { ID = newGameId });
            return Ok(newGame.FirstOrDefault());
        }

        public async Task<List<AnswerDetails>> GetAnswersForQuestion(int questionId)
        {
            object param = new { QuestionID = questionId };

            string answerQuery = "SELECT a.ID, a.AnswerContent, a.IsCorrect FROM Answers a WHERE a.QuestionID = @QuestionID";
            var answerRecords = await _db.GetRecordsAsync<AnswerDetails>(answerQuery, param);
            var answers = answerRecords.ToList();

            foreach (var answer in answers)
            {
                answer.IsPhoto = answer.AnswerContent?.Contains(".png") == true;
            }

            return answers;
        }



        [HttpGet("GetUserGames")] //שליפת רשימת המשחקים
        public async Task<IActionResult> GetUserGames(int authUserId)
        {

            //בדיקה שיש משתמש מחובר
            if (authUserId > 0)
            {
                //יצירת פרמטר עם המזהה של המשתמש
                object param = new
                {
                    UserId = authUserId
                };
                //שליפת המשחקים של המשתמש
                string gameQuery = @"SELECT g.ID, g.GameName, g.GameCode, g.IsPublish, g.CanPublish,g.LastUpdated, g.QuestionTime, 
                     COUNT(q.ID) AS QuestionsAmount
                     FROM Games g
                     LEFT JOIN Questions q ON g.ID = q.GameId
                     WHERE g.UserId = @UserId
                     GROUP BY g.ID, g.GameName, g.GameCode, g.IsPublish, g.CanPublish, g.QuestionTime, g.LastUpdated ORDER BY g.ID DESC" ;


                var gamesRecords = await _db.GetRecordsAsync<GameTable>(gameQuery, param);
                List<GameTable> GamesList = gamesRecords.ToList();
                //במידה ויש משחקים - החזרתם
                if (GamesList.Count > 0)
                {
                    foreach (var game in GamesList)
                    {
                        bool canPublish = await CanPublishFunc(game.ID, _db);
                        game.CanPublish = canPublish;
                    }

                    return Ok(GamesList);

                }
                else
                {
                    return BadRequest("No games for this user" + authUserId);
                }
            }
            else
            {
                return Unauthorized("user is not authenticated" + authUserId);
            }
        }
    }







}
