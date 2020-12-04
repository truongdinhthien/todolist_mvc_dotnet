using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoListMVC.Data.Entities;
using TodoListMVC.Shared;

namespace TodoListMVC.Models
{
    public class HomeViewModel
    {
        public List<Todo> PrivateTodoes { get; set; }
        public List<Todo> AssignedTodoes{ get; set; }
        public List<Todo> PublicTodoes { get; set; }
    }
}
