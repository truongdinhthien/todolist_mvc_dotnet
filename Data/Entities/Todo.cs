using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoListMVC.Shared;
using System.ComponentModel;

namespace TodoListMVC.Data.Entities
{
    public enum TodoStatus
    {
        New,
        InProgress,
        Resolved,
        OverDue
    }

    public enum TodoScope
    {
        Public,
        Private,
    }
    public class Todo : AuditableEntity
    {
        [DisplayName("Mã")]
        public int Id { get; set; }
        [DisplayName("Tiêu đề")]
        public string Title { get; set; }
        [DisplayName("Tên file")]
        public string FileName { get; set; }
        [DisplayName("Nội dung")]
        public string Content { get; set; }
        [DisplayName("Trạng thái")]
        public TodoStatus Status { get; set; }
        [DisplayName("Phạm vi")]
        public TodoScope Scope { get; set; }
        [DisplayName("Ngày bắt đầu")]
        public DateTime StartDate { get; set; }
        [DisplayName("Ngày kết thúc")]
        public DateTime DueDate { get; set; }
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
    }
}
