using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace TodoListMVC.Data.Entities
{
    public class User : IdentityUser
    {
        [DisplayName("Tên")]
        public string FirstName { get; set; }

        [DisplayName("Họ")]
        public string LastName { get; set; }
    }
}
