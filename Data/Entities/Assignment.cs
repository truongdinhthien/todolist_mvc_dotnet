using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoListMVC.Data.Entities
{
    public class Assignment
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public int TodoId { get; set; }
        public Todo Todo { get; set; }
        public DateTime AssignmentDate { get; set; }
    }
}
