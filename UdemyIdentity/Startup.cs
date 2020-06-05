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
                
                opt.Password.RequireDigit = false;//say� olma zorunlulugnu kald�r
                opt.Password.RequireLowercase = false;//k�c�k harf olma zorunlulugu kalks�n
                opt.Password.RequiredLength = 1;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;

                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);//10 dakika kilitlensin.
                opt.Lockout.MaxFailedAccessAttempts = 3; // 3 kere yanl�� girince blokla. buradan say�s� belirtiroyurz.
                //opt.SignIn.RequireConfirmedEmail = true;
                
            }).AddErrorDescriber<CustomIdentityValidator>().AddPasswordValidator<CustomPasswordValidator>().AddEntityFrameworkStores<UdemyContext>();//identity taraf� eklendi.
            services.ConfigureApplicationCookie(opt =>
            {
                //expiration = cookie ne kadar ayakta kalacak.fakat buradan belirlenm�yor.
                //httpOnly = ilgli cookie javascript taraf�ndan �ekilsinmi. document.cookie ile ulasmas� i�in true yoksa false
                //samesite = lax ise bu cookie yi payla��rs�n�z.di�er sitelerde bu cookien�n verilerine eri�ir.
                //strict = subdomainlerinizde dahil olmak �zere bu cookiye eri�emezsiniz.
                //securypolice = always her zaman https �zerinden �al���r. sameasrequrest http ise http , https ise https �zerinden cal�s�r.

                opt.LoginPath = new PathString("/Home/Index");
                opt.AccessDeniedPath = new PathString("/Home/AccessDenied");
                opt.Cookie.HttpOnly = true;
                opt.Cookie.Name = "UdemyCookie";
                opt.Cookie.SameSite = SameSiteMode.Strict;
                opt.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                opt.ExpireTimeSpan = TimeSpan.FromDays(20);//20 g�n ayakta kals�n cookie.


            });
            //claim bazl� yetkilendirme
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
            app.UseStaticFiles(); // wwwroot klas�r�n� d��ar�ya a�mak i�in kullan�l�r.
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
