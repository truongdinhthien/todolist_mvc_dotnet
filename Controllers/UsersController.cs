using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoListMVC.Data;
using TodoListMVC.Data.Entities;
using TodoListMVC.Models;
using TodoListMVC.Services;
using TodoListMVC.Shared;
using TodoListMVC.Shared.Constants;

namespace TodoListMVC.Controllers
{
    [Authorize(Roles = Roles.Leader)]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ICurrentUserService _currentUserService;

        public UsersController(ApplicationDbContext context, ICurrentUserService currentUserService, UserManager<User> userManager)
        {
            _context = context;
            _currentUserService = currentUserService;
            _userManager = userManager;
        }

        public async Task<ActionResult> Staffs(string email, string firstName, string lastName, int pageIndex = 1)
        {

            var staffs = await _userManager.GetUsersInRoleAsync(Roles.Staff);

            var filtered = staffs
                .Where(u =>
                   (string.IsNullOrWhiteSpace(email) || u.Email.Contains(email)) &&
                   (string.IsNullOrWhiteSpace(firstName) || u.FirstName.Contains(firstName)) &&
                   (string.IsNullOrWhiteSpace(lastName) || u.LastName.Contains(lastName))
                ).ToList();

            var pageSize = 10;

            var count = filtered.Count;
            var items = filtered.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            var page = new PaginatedList<User>(items, count, pageIndex, pageSize);


            var userViewModel = new UserViewModel {
                Users = page,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
            };
            return View(userViewModel);
        }

        public async Task<ActionResult> Leaders(string email, string firstName, string lastName, int pageIndex = 1)
        {

            var leaders = await _userManager.GetUsersInRoleAsync(Roles.Leader);

            var filtered = leaders
                .Where(u =>
                  (string.IsNullOrWhiteSpace(email) || u.Email.Contains(email)) &&
                   (string.IsNullOrWhiteSpace(firstName) || u.FirstName.Contains(firstName)) &&
                   (string.IsNullOrWhiteSpace(lastName) || u.LastName.Contains(lastName))
                ).ToList();

            var pageSize = 10;

            var count = filtered.Count;
            var items = filtered.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            var page = new PaginatedList<User>(items, count, pageIndex, pageSize);


            var userViewModel = new UserViewModel {
                Users = page,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
            };
            return View(userViewModel);
        }


        public IActionResult CreateStaff()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStaff(UserSaveModel userSaveModel)
        {
            if (!ModelState.IsValid)
            {
                return View(userSaveModel);
            }

            var duplicate = await _userManager.FindByNameAsync(userSaveModel.Email);
            if (duplicate != null)
            {
                //adding error message to ModelState
                ModelState.AddModelError("Email", "Email đã được người khác sử dụng.");
                return View(userSaveModel);
            }

            var user = new User() {
                FirstName = userSaveModel.Firstname,
                LastName = userSaveModel.Lastname,
                UserName = userSaveModel.Email,
                Email = userSaveModel.Email,
                PhoneNumber = userSaveModel.PhoneNumber,
            };

            var result = await _userManager.CreateAsync(user, userSaveModel.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, Roles.Staff);
            }

            return RedirectToAction(nameof(Staffs));
        }

        public IActionResult CreateLeader()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateLeader(UserSaveModel userSaveModel)
        {
            if (!ModelState.IsValid)
            {
                return View(userSaveModel);
            }

            var duplicate = await _userManager.FindByNameAsync(userSaveModel.Email);
            if (duplicate != null)
            {
                //adding error message to ModelState
                ModelState.AddModelError("Email", "Email đã được người khác sử dụng.");
                return View(userSaveModel);
            }

            var user = new User() {
                FirstName = userSaveModel.Firstname,
                LastName = userSaveModel.Lastname,
                UserName = userSaveModel.Email,
                Email = userSaveModel.Email,
                PhoneNumber = userSaveModel.PhoneNumber,
            };

            var result = await _userManager.CreateAsync(user, userSaveModel.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, Roles.Leader);
            }

            return RedirectToAction(nameof(Leaders));
        }

        public async Task<IActionResult> EditStaff(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            var isStaff = await _userManager.IsInRoleAsync(user, Roles.Staff);
            if (!isStaff)
            {
                return NotFound();
            }

            var userSaveModel = new UserSaveModel {
                Firstname = user.FirstName,
                Lastname = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };

            return View(userSaveModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStaff(string id, [FromForm] UserSaveModel userSaveModel)
        {
            if (ModelState.IsValid)
            {
                return View(userSaveModel);
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            var isStaff = await _userManager.IsInRoleAsync(user, Roles.Staff);
            if (!isStaff)
            {
                return NotFound();
            }

            user.FirstName = userSaveModel.Firstname;
            user.LastName = userSaveModel.Lastname;
            user.PhoneNumber = userSaveModel.PhoneNumber;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Staffs));
        }

        public async Task<IActionResult> EditLeader(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            var isLeader = await _userManager.IsInRoleAsync(user, Roles.Leader);
            if (!isLeader)
            {
                return NotFound();
            }

            var userSaveModel = new UserSaveModel {
                Firstname = user.FirstName,
                Lastname = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };

            return View(userSaveModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLeader(string id, [FromForm] UserSaveModel userSaveModel)
        {
            if (ModelState.IsValid)
            {
                return View(userSaveModel);
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            var isLeader = await _userManager.IsInRoleAsync(user, Roles.Leader);
            if (!isLeader)
            {
                return NotFound();
            }

            user.FirstName = userSaveModel.Firstname;
            user.LastName = userSaveModel.Lastname;
            user.PhoneNumber = userSaveModel.PhoneNumber;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Leaders));
        }
        [AllowAnonymous]
        [Authorize]
        public async Task<IActionResult> DetailStaff(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            var isStaff = await _userManager.IsInRoleAsync(user, Roles.Staff);
            if (!isStaff)
            {
                return NotFound();
            }

            return View(user);
        }
        [AllowAnonymous]
        [Authorize]
        public async Task<IActionResult> DetailLeader(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            var isLeader = await _userManager.IsInRoleAsync(user, Roles.Leader);
            if (!isLeader)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> LockStaff(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            var isStaff = await _userManager.IsInRoleAsync(user, Roles.Staff);
            if (!isStaff)
            {
                return NotFound();
            }

            user.LockoutEnabled = true;
            user.LockoutEnd = DateTime.MaxValue;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Staffs));
        }

        [HttpPost]
        public async Task<IActionResult> LockLeader(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            var isLeader = await _userManager.IsInRoleAsync(user, Roles.Leader);
            if (!isLeader)
            {
                return NotFound();
            }

            user.LockoutEnabled = true;
            user.LockoutEnd = DateTime.MaxValue;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Leaders));
        }


        [HttpPost]
        public async Task<IActionResult> UnLockStaff(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            var isStaff = await _userManager.IsInRoleAsync(user, Roles.Staff);
            if (!isStaff)
            {
                return NotFound();
            }

            user.LockoutEnd = null;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Staffs));
        }

        [HttpPost]
        public async Task<IActionResult> UnLockLeader(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            var isLeader = await _userManager.IsInRoleAsync(user, Roles.Leader);
            if (!isLeader)
            {
                return NotFound();
            }

            user.LockoutEnd = null;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Leaders));
        }
    }
}
