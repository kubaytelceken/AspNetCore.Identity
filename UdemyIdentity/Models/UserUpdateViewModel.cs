using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UdemyIdentity.Models
{
    public class UserUpdateViewModel
    {
        [Display(Name="Email :")]
        [Required(ErrorMessage ="Email alanı gereklidir.")]
        [EmailAddress(ErrorMessage ="Lütfen geçerli bir email adresi giriniz.")]
        public string Email { get; set; }
        [Display(Name = "Telefon :")]
        public string PhoneNumber { get; set; }

        public string PictureUrl { get; set; }

        public IFormFile Picture { get; set; }
        [Display(Name = "İsim :")]
        [Required(ErrorMessage = "Name alanı gereklidir.")]
        public string Name { get; set; }
        [Display(Name = "Soyisim :")]
        [Required(ErrorMessage = "Surname alanı gereklidir.")]
        public string SurName { get; set; }
    }
}
