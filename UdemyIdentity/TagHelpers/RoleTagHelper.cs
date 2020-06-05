using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UdemyIdentity.Context;

namespace UdemyIdentity.TagHelpers
{
    [HtmlTargetElement("RolGoster")]
    public class RoleTagHelper : TagHelper
    {
        private readonly UserManager<AppUser> _usermanager;
        public RoleTagHelper(UserManager<AppUser> usermanager)
        {
            _usermanager = usermanager;
        }
        public int UserId { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var user = _usermanager.Users.FirstOrDefault(I => I.Id == UserId);
            var roles = await _usermanager.GetRolesAsync(user);
            var builder = new StringBuilder();
            foreach (var item in roles)
            {
                builder.Append($"<strong>{item}</strong>");
            }
            output.Content.SetHtmlContent(builder.ToString());
        }
    }
}
