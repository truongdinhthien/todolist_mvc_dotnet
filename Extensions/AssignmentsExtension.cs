using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoListMVC.Data;
using TodoListMVC.Data.Entities;

namespace TodoListMVC.Extensions
{
    public static class AssignmentsExtension
    {
        public static async Task<string> ToUserStringAsync(this List<Assignment> assignments, ApplicationDbContext context)
        {
            var result = "";
            foreach (var item in assignments)
            {
                var user = await context.Users.FindAsync(item.UserId);
                if (user.UserName != "")
                {
                    if (assignments.IndexOf(item) == assignments.Count - 1)
                    {
                        result += user.UserName;
                    }
                    else result += $"{user.UserName}, ";
                }
                
            }
            return result;
        }
    }
}
