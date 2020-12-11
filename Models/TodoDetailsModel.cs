using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TodoListMVC.Data.Entities;
using TodoListMVC.Shared;

namespace TodoListMVC.Models
{
    public class TodoDetailsModel
    {
        public Todo Todo { get; set; }
        [Display(Name = "Bình luận")]
        public List<Comment> Comments { get; set; }
        [Display(Name = "Thành viên")]
        public List<Assignment> Assignments { get; set; }
        [Display(Name = "Lịch sử chỉnh sửa")]
        public List<TodoHistory> Histories { get; set; }
    }
}
