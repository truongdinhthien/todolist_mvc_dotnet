using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TodoListMVC.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace TodoListMVC.Models
{
    public class TodoSaveModel
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public TodoStatus Status { get; set; }
        [Required]
        public TodoScope Scope { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime DueDate { get; set; }
        public List<string> SelectedListUserId { get; set; }
        public IFormFile File { get; set; }
    }
}
