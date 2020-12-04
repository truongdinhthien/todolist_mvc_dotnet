using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TodoListMVC.Data;
using TodoListMVC.Data.Entities;
using TodoListMVC.Models;
using TodoListMVC.Services;
using TodoListMVC.Shared;

namespace TodoListMVC.Controllers
{
    public class TodoesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileStorageService _fileStorageService;
        private readonly ICurrentUserService _currentUserService;

        public TodoesController(ApplicationDbContext context, IFileStorageService fileStorageService, ICurrentUserService currentUserService)
        {
            _context = context;
            _fileStorageService = fileStorageService;
            _currentUserService = currentUserService;
        }
        public async Task<IActionResult> Index(TodoStatus? todoStatusFilter,
            DateTime? startDateFilter,
            DateTime? endDateFilter,
            int pageIndex = 1)
        {
            var todoes = _context.Todos.Where(t =>
                    (todoStatusFilter == null || t.Status == todoStatusFilter) &&
                    ((startDateFilter == null || endDateFilter == null) ||
                    (startDateFilter <= t.StartDate && t.StartDate <= endDateFilter))).AsQueryable();

            var page = await PaginatedList<Todo>.CreateAsync(todoes, pageIndex, 2);

            var todoViewModel = new TodoViewModel {
                Todoes = page,
                StartDateFilter = startDateFilter,
                EndDateFilter = endDateFilter,
            };
            return View(todoViewModel);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var todo = await _context.Todos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (todo == null)
            {
                return NotFound();
            }

            return View(todo);
        }
        public async Task<IActionResult> Create()
        {
            var listUser = await _context.Users.Distinct().ToListAsync();
            var selectListUser = listUser.Select(u => new SelectListItem() {
                Text = u.UserName,
                Value = u.Id,
            });
            ViewBag.ListUsers = selectListUser;

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TodoSaveModel todoSaveModel)
        {
            if (ModelState.IsValid)
            {
                //Init todo
                var todo = new Todo() {
                    Title = todoSaveModel.Title,
                    Content = todoSaveModel.Content,
                    StartDate = todoSaveModel.StartDate,
                    DueDate = todoSaveModel.DueDate,
                    Scope = todoSaveModel.Scope,
                    Status = TodoStatus.New,
                };
                //Add file
                if (todoSaveModel.File != null)
                {
                    todo.FileName = await _fileStorageService.SaveFileAsync(todoSaveModel.File);
                }
                //Add Assignment
                var listAssignment = todoSaveModel.SelectedListUserId.Select(ele => new Assignment {
                    UserId = ele.ToString(),
                    AssignmentDate = DateTime.UtcNow
                }).ToList();

                listAssignment.Add(new Assignment {
                    UserId = _currentUserService.UserId,
                    AssignmentDate = DateTime.UtcNow,
                }); // add self to todo
                todo.Assignments = listAssignment;

                _context.Add(todo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(todoSaveModel);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var todo = await _context.Todos.Include(t=>t.Assignments).FirstOrDefaultAsync(t => t.Id == id);
            if (todo == null)
            {
                return NotFound();
            }

            var todoSaveModel = new TodoSaveModel {
                Title = todo.Title,
                Content = todo.Content,
                StartDate = todo.StartDate,
                DueDate = todo.DueDate,
                Scope = todo.Scope,
                Status = todo.Status,
            };
            //Get selectlist
            todoSaveModel.SelectedListUserId = todo.Assignments.Select(a => a.UserId).ToList();
            //load all
            var listUser = await _context.Users.Distinct().ToListAsync();
            var selectListUser = listUser.Select(u => new SelectListItem() {
                Text = u.UserName,
                Value = u.Id,
            });
            ViewBag.ListUsers = selectListUser;
            return View(todoSaveModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] TodoSaveModel todoSaveModel)
        {
            if (id != todoSaveModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var todo = await _context.Todos.FindAsync(id);
                    todo.Title = todoSaveModel.Title;
                    todo.Content = todoSaveModel.Content;
                    todo.StartDate = todoSaveModel.StartDate;
                    todo.DueDate = todoSaveModel.DueDate;
                    todo.Scope = todoSaveModel.Scope;
                    todo.Status = todoSaveModel.Status;
                    if (todoSaveModel.File != null)
                    {
                        todo.FileName = await _fileStorageService.SaveFileAsync(todoSaveModel.File);
                    }
                    var newListAssignments = todoSaveModel.SelectedListUserId.Select(ele => new Assignment {
                        TodoId = id,
                        UserId = ele,
                        AssignmentDate = DateTime.UtcNow,
                    }).ToList();

                    var oldListAssignments = await _context.Assignments.Where(t => t.TodoId == id).ToListAsync();
                    var listToRemove = oldListAssignments.Where(o => !newListAssignments.Any(n => n.UserId == o.UserId)).ToList();
                    var listToAdd = newListAssignments.Where(o => !oldListAssignments.Any(n => n.UserId == o.UserId)).ToList();

                    _context.RemoveRange(listToRemove);
                    _context.AddRange(listToAdd);

                    _context.Update(todo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TodoExists(todoSaveModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(todoSaveModel);
        }

        // GET: Todoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var todo = await _context.Todos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (todo == null)
            {
                return NotFound();
            }

            return View(todo);
        }

        // POST: Todoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var todo = await _context.Todos.FindAsync(id);
            if(todo.FileName != null)
            {
                await _fileStorageService.DeleteFileAsync(todo.FileName);
            }    
            _context.Todos.Remove(todo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TodoExists(int id)
        {
            return _context.Todos.Any(e => e.Id == id);
        }
    }
}
