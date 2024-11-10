using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace template.Shared.Models.Game
{
    public class GameTable
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "שדה זה חובה")]
        [MinLength(2, ErrorMessage = "השם חייב לכלול לפחות 2 תווים שאחד מהם הוא לא רווח")]
        [MaxLength(40, ErrorMessage = "יש לתת שם עד 40 תווים ")]
        public string GameName { get; set; }
        public int GameCode { get; set; }
        public bool IsPublish { get; set; }
        public bool CanPublish { get; set; }
        public double QuestionTime { get; set; }

        public DateTime LastUpdated { get; set; }

        public int QuestionsAmount { get; set; }

    }
}
