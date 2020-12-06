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
        [Display(Name = "Danh sách bình luận")]
        public List<Comment> Comments { get; set; }
        [Display(Name = "Danh sách thành viên")]
        public List<Assignment> Assignments { get; set; }
        [Display(Name = "Lịch sử chỉnh sửa")]
        public List<TodoHistory> Histories { get; set; }
    }
}
