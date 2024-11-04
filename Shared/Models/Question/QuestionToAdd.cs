using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using template.Shared.Models.Answer;

namespace template.Shared.Models.Question
{
    public class QuestionToAdd
    {
        [Required(ErrorMessage = "שדה זה חובה")]
        [MinLength(2, ErrorMessage = "יש להזין לפחות 2 תווים")]
        public string QuestionText { get; set; }
        public string QuestionPhoto { get; set; }

        public int GameID { get; set; }
        public int? ID { get; set; }
        public List<AnswerToAdd> Answers { get; set; }

    }
}
