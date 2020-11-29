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

namespace TodoListMVC.Controllers
{
    public class TodoesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileStorageService _fileStorageService;

        public TodoesController(ApplicationDbContext context, IFileStorageService fileStorageService)
        {
            _context = context;
            _fileStorageService = fileStorageService;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.Todos.ToListAsync());
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
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TodoSaveModel todoSaveModel)
        {
            if (ModelState.IsValid)
            {
                var todo = new Todo() {
                    Title = todoSaveModel.Title,
                    Content = todoSaveModel.Content,
                    FileName = await _fileStorageService.SaveFileAsync(todoSaveModel.File),
                    StartDate = todoSaveModel.StartDate,
                    DueDate = todoSaveModel.DueDate,
                    Scope = todoSaveModel.Scope,
                    Status = todoSaveModel.Status,
                };
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

            var todo = await _context.Todos.FindAsync(id);
            if (todo == null)
            {
                return NotFound();
            }

            var todoSaveModle = new TodoSaveModel {
                Title = todo.Title,
                Content = todo.Content,
                StartDate = todo.StartDate,
                DueDate = todo.DueDate,
                Scope = todo.Scope,
                Status = todo.Status,
            };
            return View(todoSaveModle);
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
            if(todo.FileName != "")
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
