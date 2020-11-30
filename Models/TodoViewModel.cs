using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TodoListMVC.Data.Entities;
using TodoListMVC.Shared;

namespace TodoListMVC.Models
{
    public class TodoViewModel
    {
        public PaginatedList<Todo> Todoes { get; set; }
        public TodoStatus TodoStatusFilter { get; set; }
        public DateTime? StartDateFilter { get; set; }
        public DateTime? EndDateFilter { get; set; }
    }
}
