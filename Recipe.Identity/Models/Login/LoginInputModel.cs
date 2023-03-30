using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Recipe.Identity.Models.Login
{
    public class LoginInputModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string? ReturnUrl { get; set; }
        public bool RememberLogin { get; set; }
    }
}