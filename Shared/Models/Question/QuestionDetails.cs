using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using template.Shared.Models.Answer;

namespace template.Shared.Models.Question
{
    public class QuestionDetails
    {
        public int ID { get; set; }
        public string QuestionText { get; set; }
        public string QuestionPhoto { get; set; }
        public List<AnswerDetails> Answers { get; set; }
    }
}
