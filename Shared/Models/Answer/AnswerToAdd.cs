using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace template.Shared.Models.Answer
{
    public class AnswerToAdd
    {
        public string AnswerContent { get; set; }
        public bool IsPhoto { get; set; }
        public bool IsCorrect { get; set; }
        public int QuestionID { get; set; }
        public int? ID { get; set; }

    }
}
