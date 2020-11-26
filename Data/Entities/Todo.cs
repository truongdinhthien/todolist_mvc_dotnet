using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoListMVC.Shared;

namespace TodoListMVC.Data.Entities
{
    public enum TodoStatus
    {
        New,
        InProgress,
        Resolved,
        Closed,
        OverDue
    }

    public enum TodoScope
    {
        Public,
        Private,
    }
    public class Todo : AuditableEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string FileName { get; set; }
        public string Content { get; set; }
        public TodoStatus Status { get; set; }
        public TodoScope Scope { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
