using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UdemyIdentity.Context;
using UdemyIdentity.Models;

namespace UdemyIdentity.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class HomeController : Controller
    {
        //dependency injection yoluyla UserManager'A bağlanmak lazım
        private readonly UserManager<AppUser> _usermanager;//kullanıcı kayıt için usermanager kullanılır.
        private readonly SignInManager<AppUser> _signInManager;// giriş için siginmanager kullanılır.
        public HomeController(UserManager<AppUser> usermanager, SignInManager<AppUser> signInManager)
        {
            _usermanager = usermanager;
            _signInManager = signInManager;
        }
        public IActionResult Index()
        {
            return View(new UserSignInViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GirisYap(UserSignInViewModel model)
        {
            if (ModelState.IsValid)
            {
                //ispersistent = kullanıcıyı hatırlamak için kullanılır.
                //lockout = kullanıcıbelirli bir sayıda şifresini yanlıs girdi. bloklayayım mı diye kulanılır.
              var identityResult= await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, true);
                //isLockedOut = hesabın kilitli olup olmadıgına bakıyor veritabanında var.
                //successed = başarılı giriş yapma durumu
                //isNotAllowed = bir kullanıcının sistemde aktif bir üye olması için emaili doğrulamasını isteyebilirsiniz.email aktivasyonu
                //iki aşamalı doğrulama = email veya telefonunuza sms gelmesi (twofactor)

                if (identityResult.IsLockedOut)
                {
                    var gelen =  await _usermanager.GetLockoutEndDateAsync(await _usermanager.FindByNameAsync(model.UserName));
                    var kisitlananSure = gelen.Value;
                    var kalanDakika = kisitlananSure.Minute - DateTime.Now.Minute;
                    ModelState.AddModelError("", $"3 kere yanlış girdiğiniz için hesabınız {kalanDakika} dakika kilitlenmiştir.");
                    return View("Index", model);
                }
                
                if (identityResult.IsNotAllowed)
                {
                    ModelState.AddModelError("", "Lütfen email adresinizi doğrulayınız.");
                    return View("Index", model);
                }
               
                if (identityResult.Succeeded)
                {
                    //başarılı ise panelcontrollerdaki indexe git.
                    return RedirectToAction("Index", "Panel");
                }
                var yanlisGirilmeSayisi = await _usermanager.GetAccessFailedCountAsync(await _usermanager.FindByNameAsync(model.UserName));
                ModelState.AddModelError("", $"Kullanıcı adı veya şifre hatalı{3-yanlisGirilmeSayisi} kadar yanlış girerseniz hesabınız bloklanacak.");
            }
            return View("Index",model);
        }

        public IActionResult KayitOl()
        {
            return View(new UserSignUpViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> KayitOl(UserSignUpViewModel model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new AppUser
                {
                    Email = model.Email,
                    Name = model.Name,
                    SurName = model.SurName,
                    UserName = model.UserName,
                };
                var result = await _usermanager.CreateAsync(user,model.Password);

                if (result.Succeeded)
                {

                    return RedirectToAction("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
