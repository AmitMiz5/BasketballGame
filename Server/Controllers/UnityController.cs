using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using template.Shared.Models.Question;
using template.Shared.Models.Answer;
using template.Shared.Models.Game;
using template.Server.Data; // קישור DB רפוזיטורי



namespace template.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnityController : Controller
    {
        private readonly DbRepository _db;
        public UnityController(DbRepository db)
        {
            _db = db;
        }

        [HttpGet("All")]
        public async Task<IActionResult> GetAll()
        {
            object param = new { };
            string query = "SELECT * from Games";


            var records = await _db.GetRecordsAsync<GameDetails>(query, param);

            List<GameDetails> game = records.ToList();

            if (game.Count > 0)
            {
                return Ok(game);
            }
            return BadRequest("the list is empty");
        }

        [HttpGet("GetGameDetails/{gameCode}")]

        public async Task<IActionResult> GetGameDetails(int gameCode)
        {
            object param = new
            {
                GameCode = gameCode
            };

            string queryCheckExistence = "SELECT ID FROM Games WHERE GameCode = @GameCode";
            var existingGame = await _db.GetRecordsAsync<GameDetails>(queryCheckExistence, param);
            var gameCodeCheck = existingGame.FirstOrDefault();

            if (gameCodeCheck == null)
            {
                return BadRequest("אין משחק עם קוד " + gameCode);
            }

            string queryCheck = "SELECT ID, GameName, QuestionTime FROM Games WHERE GameCode = @GameCode AND IsPublish = 1";
            var record = await _db.GetRecordsAsync<GameDetails>(queryCheck, param);
            var gameCheck = record.FirstOrDefault();

            if (gameCheck == null)
            {
                return BadRequest("המשחק קיים אך לא פורסם");
            }

            var paramQuestions = new { GameID = gameCheck.ID };
            string queryQuestions = @"SELECT ID, QuestionText, QuestionPhoto FROM Questions WHERE GameID = @GameID";
            var questions = await _db.GetRecordsAsync<QuestionDetails>(queryQuestions, paramQuestions);

            foreach (var question in questions)
            {
                var paramAnswers = new { QuestionID = question.ID };
                string queryAnswers = @"
            SELECT ID, AnswerContent, IsPhoto, IsCorrect 
            FROM Answers 
            WHERE QuestionID = @QuestionID";
                var answers = await _db.GetRecordsAsync<AnswerDetails>(queryAnswers, paramAnswers);
                question.Answers = answers.ToList();
            }

            gameCheck.Questions = questions.ToList();

            return Ok(gameCheck);
        }
    }










}

