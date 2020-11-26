using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoListMVC.Data.Entities;
using TodoListMVC.Shared.Constants;

namespace TodoListMVC.Data
{
    public static class SeedData
    {
        public static async Task SeedApp(ApplicationDbContext context)
        {
            if(!context.Todos.Any())
            {
                context.Todos.Add(new Todo
                {
                    Title = "Todo 1",
                    Content = "Content 1",
                    StartDate = DateTime.UtcNow,
                    DueDate = DateTime.UtcNow.AddDays(5),
                    Scope = TodoScope.Public,
                    Status = TodoStatus.New,
                });
                await context.SaveChangesAsync();
            }    
        }

        public static async Task SeedIdentity(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {

            //Seed roles
            if(!await roleManager.RoleExistsAsync(Roles.Leader))
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.Leader));
            }
            if (!await roleManager.RoleExistsAsync(Roles.Staff))
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.Staff));
            }
            //Seed user

            if (await userManager.FindByNameAsync("username1@gmail.com") == null)
            {
                var user1 = new User
                {
                    Id = "idseed1",
                    FirstName = "firstname1",
                    LastName = "lastname1",
                    UserName = "username1@gmail.com",
                    Email = "username1@gmail.com",
                };
                var result = await userManager.CreateAsync(user1, AuthorizationConstants.DEFAULT_PASSWORD);
                if(result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user1, Roles.Leader);
                }    
            }
            if (await userManager.FindByNameAsync("username2@gmail.com") == null)
            {
                var user2 = new User
                {
                    Id = "idseed2",
                    FirstName = "firstname2",
                    LastName = "lastname2",
                    UserName = "username2@gmail.com",
                    Email = "username2@gmail.com",
                };
                var result = await userManager.CreateAsync(user2, AuthorizationConstants.DEFAULT_PASSWORD);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user2, Roles.Staff);
                }
            }
            if (await userManager.FindByNameAsync("username3@gmail.com") == null)
            {
                var user3 = new User
                {
                    Id = "idseed3",
                    FirstName = "firstname3",
                    LastName = "lastname3",
                    UserName = "username3@gmail.com",
                    Email = "username3@gmail.com",
                };
                var result = await userManager.CreateAsync(user3, AuthorizationConstants.DEFAULT_PASSWORD);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user3, Roles.Staff);
                }
            }

        }
    }
}
