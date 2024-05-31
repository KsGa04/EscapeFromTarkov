using EscapeFromTarkov.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.Swagger.Annotations;

namespace EscapeFromTarkov.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssemblingAPI : ControllerBase
    {
        EscapeFromTarkovContext db = new EscapeFromTarkovContext();

        [HttpPost]
        [Route("Assembling/addAssembling")]
        public async Task<IActionResult> AssemblingAdd(string name, string idWeapon, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Файл не выбран");
            }
            else
            {
                if (name == "" || idWeapon == "")
                {
                    return BadRequest("Не все данные заполнены");
                }
                else
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await file.CopyToAsync(memoryStream);
                        byte[] imageBytes = memoryStream.ToArray();
                        Сборка босс = new Сборка()
                        {
                            Наименование = name,
                            ОружиеId = Convert.ToInt32( idWeapon),
                            Изображение = imageBytes
                        };
                        db.Сборкаs.Add(босс);
                        db.SaveChanges();
                        return Ok("Данные добавлены");
                    }
                }
            }
        }

        [HttpPut]
        [Route("Assembling/updateAssembling")]
        [SwaggerOperation("Загрузить изображение")]
        [SwaggerResponse(200, "Изображение успешно загружено")]
        public async Task<IActionResult> AssemblingUpdate(int id, string name, string idWeapon, IFormFile file)
        {

            var boss = db.Сборкаs.Where(x => x.СборкаId == id).FirstOrDefault();
            if (boss == null)
            {
                return NotFound("Данная сборка не найдена");
            }
            else
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("Файл не выбран");
                }
                else
                {
                    if (name == "" || idWeapon == "")
                    {
                        return BadRequest("Не все данные заполнены");
                    }
                    else
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await file.CopyToAsync(memoryStream);
                            byte[] imageBytes = memoryStream.ToArray();
                            boss = db.Сборкаs.Where(x => x.СборкаId == id).FirstOrDefault();
                            boss.Наименование = name;
                            boss.ОружиеId = Convert.ToInt32(idWeapon);
                            boss.Изображение = imageBytes;
                            db.SaveChanges();
                            return Ok("Данные добавлены");
                        }
                    }
                }
            }
        }
        [HttpDelete]
        [Route("Assembling/deleteAssembling")]
        public async Task<IActionResult> AssemblingDelete(int id)
        {
            var boss = db.Сборкаs.Where(x => x.СборкаId == id).FirstOrDefault();
            if (boss == null)
            {
                return NotFound("Данная сборка не найдена");
            }
            else
            {
                boss = db.Сборкаs.Where(x => x.СборкаId == id).FirstOrDefault();
                db.Сборкаs.Remove(boss);
                db.SaveChanges(true);
                return Ok("Удаление совершено успешно");
            }
        }
    }
}
