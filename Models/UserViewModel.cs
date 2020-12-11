using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoListMVC.Data.Entities;
using TodoListMVC.Shared;

namespace TodoListMVC.Models
{
    public class UserViewModel
    {
        public PaginatedList<User> Users { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
