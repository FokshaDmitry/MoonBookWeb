﻿namespace MoonBookWeb.Models
{
    public class RegUserModel
    {
        //tmp reg model
        public String? Name { get; set; }
        public String? Surname { get; set; }
        public String? Status { get; set; }
        public DateTime DateOfBith { get; set; }
        public String? Login { get; set; }
        public String? Email { get; set; }
        public String? Password { get; set; }
        public String? ConfirmPass { get; set; }
        public IFormFile? Avatar { get; set; }

    }
}
