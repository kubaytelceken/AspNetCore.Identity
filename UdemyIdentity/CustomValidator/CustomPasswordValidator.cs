using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UdemyIdentity.Context;

namespace UdemyIdentity.CustomValidator
{
    public class CustomPasswordValidator : IPasswordValidator<AppUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user, string password)
        {
            //success veya failed gönderilecek.
            //success olursa validasyon işleminden geçmiş oluyor.
            //failed olursa hatalı.

            /*
             * if()
             * if()
             */
            //parola kullanıcı adı içermemeli.
            List<IdentityError> errors = new List<IdentityError>();
            
            if (password.ToLower().Contains(user.Name.ToLower()))
            {
                errors.Add(new IdentityError()
                {
                    Code="PasswordContainsUserName",
                    Description="Parola kullanıcı adı içeremez."

                });
            }
            if (errors.Count > 0)
            {
                return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
            }
            else
            {
                return Task.FromResult(IdentityResult.Success);
            }
        }
    }
}
