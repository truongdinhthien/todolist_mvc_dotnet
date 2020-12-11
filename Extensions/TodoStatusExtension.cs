using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoListMVC.Data.Entities;

namespace TodoListMVC.Extensions
{
    public static class TodoStatusExtension
    {
        public static string ToColor(this TodoStatus todoStatus)
        {
            switch (todoStatus)
            {
                case TodoStatus.New: return "primary";
                case TodoStatus.InProgress: return "warning";
                case TodoStatus.Resolved: return "success";
                case TodoStatus.OverDue: return "danger";
            }
            return string.Empty;
        }

        public static string ToColor(this TodoScope todoScoprs)
        {
            switch (todoScoprs)
            {
                case TodoScope.Public: return "success";
                case TodoScope.Private: return "danger";
            }
            return string.Empty;
        }
    }
}

