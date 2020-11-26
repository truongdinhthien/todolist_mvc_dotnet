using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoListMVC.Shared;

namespace TodoListMVC.Data.Entities
{
    public class TodoHistory : AuditableEntity
    {
        public int Id { get; set; }
        public int TodoId { get; set; }
        public Todo Todo { get; set; }
        public string LogContent { get; set; }
    }
}
