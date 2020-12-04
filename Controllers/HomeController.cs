using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TodoListMVC.Data;
using TodoListMVC.Data.Entities;
using TodoListMVC.Models;
using TodoListMVC.Services;

namespace TodoListMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public HomeController(ApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }
        [Authorize]
        public IActionResult Index()
        {
            var privateTodoes = _context.Todos
                .Where(t => t.CreatedBy == _currentUserService.UserId)
                .ToList();

            // https://stackoverflow.com/a/3547706
            var assignedTodoes = _context.Todos
                .Where(t => t.CreatedBy != _currentUserService.UserId)
                .Join(_context.Assignments, t => t.Id, a => a.TodoId, (t, a) => new {t, a})
                .Where(o => o.a.UserId == _currentUserService.UserId)
                .Select(o => o.t)
                .ToList();

            var publicTodoes = _context.Todos
                .Where(t => t.CreatedBy != _currentUserService.UserId && t.Scope == TodoScope.Public)
                .ToList();

            var homeViewModel = new HomeViewModel {
                PrivateTodoes = privateTodoes,
                AssignedTodoes = assignedTodoes,
                PublicTodoes = publicTodoes
            };

            return View(homeViewModel);
        }
        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
