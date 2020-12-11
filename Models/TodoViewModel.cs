using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TodoListMVC.Data.Entities;
using TodoListMVC.Shared;

namespace TodoListMVC.Models
{
    public enum TodoStatusVM
    {
        New,
        InProgress,
        Resolved,
        OverDue
    }
    public class TodoViewModel
    {
        public PaginatedList<Todo> Todoes { get; set; }
        public TodoStatusVM TodoStatusFilter { get; set; }
        public DateTime? StartDateFilter { get; set; }
        public DateTime? EndDateFilter { get; set; }
    }
}
