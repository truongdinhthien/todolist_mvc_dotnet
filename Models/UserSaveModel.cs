using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TodoListMVC.Models
{
    public class UserSaveModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Tên")]
        public string Firstname { get; set; }

        [Required]
        [Display(Name = "Họ")]
        public string Lastname { get; set; }

        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} phải ít nhất {2} ký tự.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Nhập lại mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu không khớp, vui lòng nhập lại")]
        public string ConfirmPassword { get; set; }
    }
}
