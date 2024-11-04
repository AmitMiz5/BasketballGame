using Microsoft.AspNetCore.Mvc;
using template.Server.Helpers;
using template.Server.Data;
using Microsoft.AspNetCore.Http;
using template.Shared.Models.Media;
using template.Shared.Models.Question;
using Microsoft.IdentityModel.Tokens;


namespace template.Server.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class MediaController : ControllerBase
    {
        private readonly DbRepository _db;
        private readonly FilesManage _filesManage;
        private readonly string containerName;

        public MediaController(FilesManage filesManage, DbRepository db)
        {
            _filesManage = filesManage;
            _db = db;
            containerName = "uploadedFiles";
        }


        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(UploadFile uploadDto)
        {

            if (string.IsNullOrEmpty(uploadDto.ImageBase64))
            {
                return BadRequest("Invalid data.");
            }

            // Save the Base64 image and get the image name
            string imgName = await _filesManage.SaveFile(uploadDto.ImageBase64, "png", "uploadedFiles");

            if (!string.IsNullOrEmpty(imgName))
            {
                return Ok(imgName);
            }
            else
            {
                return BadRequest("Error saving image.");
            }


        }

        [HttpPost("deleteImages")]
        public async Task<IActionResult> DeleteImages([FromBody] List<string> images)
        {
            var countFalseTry = 0;
            foreach (string img in images)
            {
                _filesManage.DeleteFile(img, containerName);
                //if (_filesManage.DeleteFile(img, "") == false)
                //{
                //    countFalseTry++;
                //}
            }

            if (countFalseTry > 0)
            {
                //לעורך, אין גישה לשרת ולא יוכל לתקן דבר. הבדיקה משמשת אותנו לצורך בדיקות
                return BadRequest("problem with " + countFalseTry.ToString() + " images");
            }
            return Ok("deleted");
        }


    }
}
