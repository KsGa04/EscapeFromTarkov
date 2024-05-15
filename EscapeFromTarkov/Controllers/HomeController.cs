﻿using EscapeFromTarkov.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

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
        public IActionResult PrivateAccAsync(string login, string password, int survival, int death, int lost, int count, int murders, int murdersChVK, IFormFile image)
        {
            Пользователь пользователь = db.Пользовательs.Where(x => x.ПользовательId == CurrentUser.CurrentClientId).FirstOrDefault();
            пользователь.Логин = login;
            пользователь.Пароль = password;
            пользователь.Выживания = survival;
            пользователь.Смерти = death;
            пользователь.ПотерянБезвести = lost;
            пользователь.КоличествоРейдов = count;
            пользователь.Убийства = murders;
            пользователь.УбийстваЧвк = murdersChVK;
            if (image != null && image.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    image.CopyToAsync(memoryStream);
                    пользователь.Доказательство = memoryStream.ToArray();
                    db.SaveChanges();
                    return View(пользователь);
                }
            }
            else
            {
                ModelState.AddModelError("", "Изображение не выбрано");
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
        public IActionResult Chat()
        {
            return View();
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