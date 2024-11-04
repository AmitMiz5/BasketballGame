using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using template.Shared.Models.Question;

namespace template.Shared.Models.Game
{
    public class GameDetails
    {
        public int ID { get; set; }
        public string GameName { get; set; }
        public double QuestionTime { get; set; }
        public List<QuestionDetails> Questions { get; set; }
        public bool IsPublish { get; set; }
    }
}
