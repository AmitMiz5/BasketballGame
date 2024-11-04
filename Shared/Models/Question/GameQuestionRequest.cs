using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using template.Shared.Models.Media;

namespace template.Shared.Models.Question
{
    public class GameQuestionRequest
    {
        public int GameID { get; set; }
        public QuestionToAdd NewQuestion { get; set; }
        public UploadFile UploadDto { get; set; }
    }
}
