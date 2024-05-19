using EscapeFromTarkov.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace EscapeFromTarkov.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private EscapeFromTarkovContext db = new EscapeFromTarkovContext();

        public HomeController(ILogger<HomeController> logger, EscapeFromTarkovContext db)
        {
            _logger = logger;
            this.db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult MainWindow()
        {
            return View();
        }
        public IActionResult PrivateAcc()
        {
            Пользователь пользователь = db.Пользовательs.Where(x => x.ПользовательId == CurrentUser.CurrentClientId).FirstOrDefault();
            return View(пользователь);
        }
        [HttpPost]
        public IActionResult PrivateAcc(string login, string password, int survival, int death, int lost, int count, int murders, int murdersChVK, IFormFile photo)
        {
            MemoryStream ms = new MemoryStream();
            photo.CopyTo(ms);
            Пользователь пользователь = db.Пользовательs.Where(x => x.ПользовательId == CurrentUser.CurrentClientId).FirstOrDefault();
            try
            {
                
                пользователь.Логин = login;
                пользователь.Пароль = password;
                пользователь.Выживания = survival;
                пользователь.Смерти = death;
                пользователь.ПотерянБезвести = lost;
                пользователь.КоличествоРейдов = count;
                пользователь.Убийства = murders;
                пользователь.УбийстваЧвк = murdersChVK;
                пользователь.Доказательство = ms.ToArray();
                пользователь.Одобрено = false;
                db.SaveChanges();
                    return View(пользователь);
            }
            catch (Exception ex)
            {
                // Обработайте исключение и отображайте соответствующее сообщение пользователю
                ModelState.AddModelError("", $"Произошла ошибка при загрузке файла: {ex.Message}");
                return View(пользователь);
            }
        }
        public class PrivateAccViewModel
        {
            public IEnumerable<Персонажи>? NPS { get; set; } 
            public IEnumerable<Босс>? Boss { get; set; }
            public IEnumerable<Карта>? Card { get; set; }
            public IEnumerable<Оружие>? Weapon { get; set; }
            public IEnumerable<Товары>? Product { get; set; }
            public string name;
            public string image;
            public string description;
        }
        public IActionResult NPS()
        {
            var персонажи = db.Персонажиs.ToList();
            var ViewModel = new PrivateAccViewModel()
            {
                NPS = персонажи
            };
            return View(ViewModel);
        }
        public IActionResult GetBossInfo(string bossName)
        {
            Босс boss = db.Боссs.FirstOrDefault(x => x.Наименование == bossName);

            if (boss != null)
            {
                var imageBase64 = Convert.ToBase64String(boss.Изображение);
                var bossInfo = new
                {
                    name = boss.Наименование,
                    image = "data:image/jpeg;base64," + imageBase64, // здесь указывается тип изображения и сама строка Base64
                    description = boss.Описание
                };

                return Json(bossInfo);
            }
            else
            {
                return NotFound();
            }
        }
        public IActionResult GetNPSInfo(string bossName)
        {
            Персонажи boss = db.Персонажиs.FirstOrDefault(x => x.Наименование == bossName);

            if (boss != null)
            {
                var imageBase64 = Convert.ToBase64String(boss.Изображение);
                var bossInfo = new
                {
                    name = boss.Наименование,
                    image = "data:image/jpeg;base64," + imageBase64, // здесь указывается тип изображения и сама строка Base64
                    description = boss.Описание
                };

                return Json(bossInfo);
            }
            else
            {
                return NotFound();
            }
        }
        public IActionResult GetCardInfo(string bossName)
        {
            Карта boss = db.Картаs.FirstOrDefault(x => x.Наименование == bossName);

            if (boss != null)
            {
                var imageBase64 = Convert.ToBase64String(boss.Изображение);
                var bossInfo = new
                {
                    name = boss.Наименование,
                    image = "data:image/jpeg;base64," + imageBase64, // здесь указывается тип изображения и сама строка Base64
                    description = boss.Описание
                };

                return Json(bossInfo);
            }
            else
            {
                return NotFound();
            }
        }
        public IActionResult Bosses()
        {
            var персонажи = db.Боссs.ToList();
            var ViewModel = new PrivateAccViewModel()
            {
                Boss = персонажи
            };
            return View(ViewModel);
        }
        public IActionResult Cards()
        {
            var персонажи = db.Картаs.ToList();
            var ViewModel = new PrivateAccViewModel()
            {
                Card = персонажи
            };
            return View(ViewModel);
        }
        [AllowAnonymous]
        public IActionResult Authorization()
        {
            return View();
        }
        [AllowAnonymous]
        public IActionResult Registration()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Authorization(string login, string password)
        {
            Пользователь пользователь = db.Пользовательs.Where(x => x.Логин == login && x.Пароль == password).FirstOrDefault();
            CurrentUser.CurrentClientId = пользователь.ПользовательId;
            if (CurrentUser.CurrentClientId > 0 && пользователь.РолиId == 1)
            {
                var claims = new List<Claim> { new Claim(ClaimTypes.Name, "test"),
                new Claim(ClaimTypes.Email, "testc@mail.ru")};
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");

                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                return RedirectToAction("MainWindow", "Home");
            }
            else if (CurrentUser.CurrentClientId > 0 && пользователь.РолиId == 2)
            {
                var claims = new List<Claim> { new Claim(ClaimTypes.Name, "test"),
                new Claim(ClaimTypes.Email, "testc@mail.ru")};
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");

                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                return RedirectToAction("AdminPanelBoss", "Home");
            }
            else if (CurrentUser.CurrentClientId > 0 && пользователь.РолиId == 3)
            {
                var claims = new List<Claim> { new Claim(ClaimTypes.Name, "test"),
                new Claim(ClaimTypes.Email, "testc@mail.ru")};
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");

                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                return RedirectToAction("ModeratorWindow", "Home");
            }
            else
            {
                var result = new SuccessResponse
                {
                    Success = false,
                    Message = "Данные неверные",
                };
                return ViewBag.Enter = result.Message;
            }
        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Registration(string login, string password)
        {
            Пользователь Clients = (from c in db.Пользовательs where c.Логин == login select c).FirstOrDefault();
            if (Clients != null)
            {
                return RedirectToAction("Authorization", "Home");
            }
            else
            if (password == "" || login == "")
            {
                var result = new SuccessResponse
                {
                    Success = false,
                    Message = "Некоторые поля пустые",
                };
                return ViewBag.Enter = result.Message;
            }
            else
            {
                Пользователь пользователь = new Пользователь()
                {
                    Логин = login,
                    Пароль = password,
                    РолиId = 1
                };
                db.Add(пользователь);
                db.SaveChanges();
                return RedirectToAction("Authorization", "Home");
            }
        }

        public IActionResult AdminPanelBoss()
        {
            var персонажи = db.Боссs.ToList();
            var ViewModel = new PrivateAccViewModel()
            {
                Boss = персонажи
            };
            return View(ViewModel);
        }
        public IActionResult AdminPanelCard()
        {
            var персонажи = db.Картаs.ToList();
            var ViewModel = new PrivateAccViewModel()
            {
                Card = персонажи
            };
            return View(ViewModel);
        }
        public IActionResult AdminPanelNPS()
        {
            var персонажи = db.Персонажиs.ToList();
            var ViewModel = new PrivateAccViewModel()
            {
                NPS = персонажи
            };
            return View(ViewModel);
        }
        public IActionResult AdminPanelWeapon()
        {
            var персонажи = db.Оружиеs.ToList();
            var ViewModel = new PrivateAccViewModel()
            {
                Weapon = персонажи
            };
            return View(ViewModel);
        }
        public IActionResult AdminPanelProduct()
        {
            var персонажи = db.Товарыs.ToList();
            var ViewModel = new PrivateAccViewModel()
            {
                Product = персонажи
            };
            return View(ViewModel);
        }
        public IActionResult Exit()
        {
            CurrentUser.CurrentAdminId = 0;
            CurrentUser.CurrentClientId = 0;
            return RedirectToAction("Authorization", "Home");
        }
        public class PrivateAccModel
        {
            public List<Пользователь> Users = new List<Пользователь>();
            public List<Карта> Cards = new List<Карта>();
        }
        public IActionResult Chat()
        {
            var users = db.Пользовательs.Where(u => u.Онлайн == true && u.ПользовательId != CurrentUser.CurrentClientId && u.РолиId == 1).ToList();
            var responce = new PrivateAccModel
            {
                Users = users,
                Cards = db.Картаs.ToList()
            };

            return View(responce);
        }
        [HttpPost("ChangeCard")]
        public ActionResult ChangeCard([FromBody] string selectedCardName)
        {
            Карта карта = db.Картаs.Where(x => x.Наименование == selectedCardName).FirstOrDefault();
            if (карта == null)
            {
                var users = db.Пользовательs.Where(u => u.Онлайн == true && u.ПользовательId != CurrentUser.CurrentClientId && u.РолиId == 1).ToList();

                return Json(users);
            }
            else
            {
                Пользователь пользователь = db.Пользовательs.Where(x => x.ПользовательId == CurrentUser.CurrentClientId).FirstOrDefault();
                пользователь.КартаId = карта.КартаId;
                db.SaveChanges();
                var users = db.Пользовательs.Where(u => u.Онлайн == true && u.ПользовательId != CurrentUser.CurrentClientId && u.РолиId == 1 && u.КартаId == карта.КартаId).ToList();
                return Json(users);
            }
            
        }
        public class MessageResponse
        {
            public int TotalCount { get; set; }
            public List<Message> Messages { get; set; }

        }
        public class Message
        {
            public string Сообщение { get; set; }
            public string Позиция { get; set; }
        }
        [HttpPost("GetMessages")]
        public ActionResult GetMessages([FromBody] string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                // Обработка ошибки, если userName равен null или пустая строка
                return Json(new { error = "Invalid userName" });
            }

            var currentUser = db.Пользовательs.Where(x => x.ПользовательId == CurrentUser.CurrentClientId).FirstOrDefault();

            if (currentUser == null)
            {
                // Обработка ошибки, если текущий пользователь не найден
                return Json(new { error = "Current user not found" });
            }

            // Получаем сообщения для данного пользователя из базы данных
            var messages = db.Общениеs
                .Where(m => (m.ОтправительNavigation.Логин == userName && m.ПолучательNavigation.Логин == currentUser.Логин) || (m.ОтправительNavigation.Логин == currentUser.Логин && m.ПолучательNavigation.Логин == userName))
                .Select(m => new Message
                {
                    Сообщение = m.Сообщение,
                    Позиция = (m.ОтправительNavigation.Логин == currentUser.Логин) ? "left" : "right"
                })
                .ToList();
            var response = new MessageResponse
            {
                TotalCount = messages.Count,
                Messages = messages
            };
            // Возвращаем сообщения в формате JSON
            return Json(response);
        }
        [HttpPost("GetInformation")]
        public ActionResult GetInformation([FromBody] string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                // Обработка ошибки, если userName равен null или пустая строка
                return Json(new { error = "Invalid userName" });
            }

            var User = db.Пользовательs.Where(x => x.Логин ==userName).FirstOrDefault();

            return Json(User);
        }
        public class MessageData
        {
            public string userName { get; set; }
            public string formattedDate { get; set; }
            public string message { get; set; }
        }
        [HttpPost("AddMessages")]
        public ActionResult AddMessages([FromBody] MessageData messageData)
        {
            if (string.IsNullOrEmpty(messageData.userName))
            {
                // Обработка ошибки, если userName равен null или пустая строка
                return Json(new { error = "Invalid userName" });
            }

            var recipient = db.Пользовательs.Where(x => x.Логин == messageData.userName).FirstOrDefault();

            if (recipient == null)
            {
                // Обработка ошибки, если текущий пользователь не найден
                return Json(new { error = "Recipient not found" });
            }

            Общение общение = new Общение();
            общение.Сообщение = messageData.message;
            общение.Отправитель = CurrentUser.CurrentClientId;
            общение.Получатель = recipient.ПользовательId;
            общение.ВремяОтправки = Convert.ToDateTime( messageData.formattedDate);
            db.Общениеs.Add(общение);
            db.SaveChanges();
            return Ok();
        }
        public IActionResult ModeratorWindow()
        {
            var users = db.Пользовательs.Where(u => u.Одобрено == false && u.РолиId == 1).ToList();
            return View(users);
        }
        public class Acc
        {
            public string логин { get; set; }
            public int? выживания { get; set; }
            public int? смерти { get; set; }
            public int? потерянБезвести { get; set; }
            public int? количествоРейдов { get; set; }
            public int? убийства { get; set; }
            public int? убийстваЧвк { get; set; }
            public bool? одобрено { get; set; }
        }
        [HttpPost("SaveChangesModer")]
        public ActionResult SaveChangesModer([FromBody] Acc data)
        {
            Пользователь пользователь = db.Пользовательs.Where(x => x.Логин == data.логин).FirstOrDefault();
            пользователь.Выживания = data.выживания;
            пользователь.Смерти = data.смерти;
            пользователь.ПотерянБезвести = data.потерянБезвести;
            пользователь.КоличествоРейдов = data.количествоРейдов;
            пользователь.Убийства = data.убийства;
            пользователь.УбийстваЧвк = data.убийстваЧвк;
            db.SaveChanges();
            return Ok();
        }

            public IActionResult ChatBot()
        {
            return View();
        }

        public class Gift
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Price { get; set; }
            public string Trader { get; set; }
        }

        public class GiftResponse
        {
            public int TotalCount { get; set; }
            public List<Gift> Gifts { get; set; }
        }

        private List<Gift> allProducts = new List<Gift>();
        private int count_random = 0;

        [HttpPost("GetProduct")]
        public ActionResult GetProduct([FromBody] Dictionary<string, string> userAnswers)
        {
            var recipient = userAnswers["Какой товар вам необходимо найти?"];

            // Получаем все товары, соответствующие запросу
            var products = db.Товарыs.Where(x => x.Название.Contains(recipient) || x.Торговец.Contains(recipient))
                                     .Select(x => new Gift { Name = x.Название, Price = x.Цена.ToString(), Trader = x.Торговец, Id = x.Udid })
                                     .ToList();
            allProducts.AddRange(products);
            if (allProducts.Count > 5)
            {
                var selectedGifts = allProducts.OrderBy(x => Guid.NewGuid())
                                              .Where(x => allProducts.IndexOf(x) >= count_random)
                                              .Take(5)
                                              .ToList();
                count_random += 5;
                var response = new GiftResponse
                {
                    TotalCount = allProducts.Count,
                    Gifts = selectedGifts
                };
                return Ok(response);
            }
            else
            {
                var response = new GiftResponse
                {
                    TotalCount = allProducts.Count,
                    Gifts = allProducts
                };
                return Ok(response);
            }
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}