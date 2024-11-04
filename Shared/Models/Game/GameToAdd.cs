using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace template.Shared.Models.Game
{
    public class GameToAdd
    {
        [Required(ErrorMessage = "שדה זה חובה")]
        [MinLength(2, ErrorMessage = "יש להזין לפחות 2 תווים")]
        public string GameName { get; set; }
    }
}
