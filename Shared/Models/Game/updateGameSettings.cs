using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace template.Shared.Models.Game
{
    public class updateGameSettings
    {
        [Required(ErrorMessage = "שדה זה חובה")]
        [MinLength(2, ErrorMessage = "השם חייב לכלול לפחות 2 תווים שאחד מהם הוא לא רווח")]
        public string GameName { get; set; }
        public int ID { get; set; }
        public double QuestionTime { get; set; }
    }
}
