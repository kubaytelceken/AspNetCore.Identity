using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UdemyIdentity.Context;
using UdemyIdentity.CustomValidator;

namespace UdemyIdentity
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddDbContext<UdemyContext>(); //database eklendi
            services.AddIdentity<AppUser, AppRole>(opt=>
            {
                
                opt.Password.RequireDigit = false;//sayý olma zorunlulugnu kaldýr
                opt.Password.RequireLowercase = false;//kücük harf olma zorunlulugu kalksýn
                opt.Password.RequiredLength = 1;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;

                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);//10 dakika kilitlensin.
                opt.Lockout.MaxFailedAccessAttempts = 3; // 3 kere yanlýþ girince blokla. buradan sayýsý belirtiroyurz.
                //opt.SignIn.RequireConfirmedEmail = true;
                
            }).AddErrorDescriber<CustomIdentityValidator>().AddPasswordValidator<CustomPasswordValidator>().AddEntityFrameworkStores<UdemyContext>();//identity tarafý eklendi.
            services.ConfigureApplicationCookie(opt =>
            {
                //expiration = cookie ne kadar ayakta kalacak.fakat buradan belirlenmýyor.
                //httpOnly = ilgli cookie javascript tarafýndan çekilsinmi. document.cookie ile ulasmasý için true yoksa false
                //samesite = lax ise bu cookie yi paylaþýrsýnýz.diðer sitelerde bu cookienýn verilerine eriþir.
                //strict = subdomainlerinizde dahil olmak üzere bu cookiye eriþemezsiniz.
                //securypolice = always her zaman https üzerinden çalýþýr. sameasrequrest http ise http , https ise https üzerinden calýsýr.

                opt.LoginPath = new PathString("/Home/Index");
                opt.AccessDeniedPath = new PathString("/Home/AccessDenied");
                opt.Cookie.HttpOnly = true;
                opt.Cookie.Name = "UdemyCookie";
                opt.Cookie.SameSite = SameSiteMode.Strict;
                opt.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                opt.ExpireTimeSpan = TimeSpan.FromDays(20);//20 gün ayakta kalsýn cookie.


            });
            //claim bazlý yetkilendirme
            services.AddAuthorization(opt =>
            {
                opt.AddPolicy("FemalePolicy", cnf =>
                 {
                     cnf.RequireClaim("gender", "female");
                 });
            });
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseStaticFiles(); // wwwroot klasörünü dýþarýya açmak için kullanýlýr.
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
