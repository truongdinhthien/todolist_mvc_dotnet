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
using TodoListMVC.Extensions;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace TodoListMVC.Controllers
{
    [Authorize]
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
        public async Task<IActionResult> Index(TodoStatusVM? todoStatusFilter,
            DateTime? startDateFilter,
            DateTime? endDateFilter,
            int pageIndex = 1)
        {
            var todoes = _context.Todos
                    .Where(t =>
                    (User.FindFirstValue(ClaimTypes.Role) == "leader" || t.Scope == TodoScope.Public || t.CreatedBy == _currentUserService.UserId 
                     || t.Assignments.Any(a => a.UserId == _currentUserService.UserId && a.TodoId == t.Id)) &&
                    ((todoStatusFilter == null || todoStatusFilter == TodoStatusVM.OverDue) ||
                    (t.Status == (TodoStatus)todoStatusFilter)) &&
                    ((todoStatusFilter == null || todoStatusFilter != TodoStatusVM.OverDue) ||
                    (t.DueDate <= DateTime.Now)) &&
                    ((startDateFilter == null || endDateFilter == null) ||
                    (startDateFilter <= t.StartDate && t.StartDate <= endDateFilter))).AsQueryable();

            var page = await PaginatedList<Todo>.CreateAsync(todoes, pageIndex, 3);

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

            var comments = _context.Comments.Where(c => c.TodoId == todo.Id).ToList();
            var assignments = _context.Assignments.Where(a => a.TodoId == todo.Id).ToList();
            var histories = _context.TodoHistories.Where(h => h.TodoId == todo.Id).ToList();
            TodoDetailsModel todoDetails = new TodoDetailsModel {
                Todo = todo,
                Comments = comments,
                Assignments = assignments,
                Histories = histories
            };
            var view = View(todoDetails);
            view.ViewData["user"] = _currentUserService.UserId;
            return View(todoDetails);
        }
        public async Task<IActionResult> Create()
        {
            var listUser = await _context.Users.Distinct().ToListAsync();
            var selectListUser = listUser.Where(l => l.Id != _currentUserService.UserId).Select(u => new SelectListItem() {
                Text = u.UserName,
                Value = u.Id,
            });
            ViewBag.ListUsers = selectListUser;

            return View();
        }
        public IActionResult CreateComment(int? id)
        {
            var todos = _context.Todos.Select(t => new SelectListItem() {
                Text = t.Title,
                Value = t.Id.ToString(),
            }).ToList();
            Comment defaultComment = new Comment() {
                Id = 0,
                TodoId = id.Value,
                Created = DateTime.Now
            };
            ViewBag.ListTodos = todos;
            return View(defaultComment);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateComment(Comment cm)
        {
            if (ModelState.IsValid)
            {
                //Init todo
                var comment = new Comment() {
                    TodoId = cm.TodoId,
                    Content = cm.Content,
                    Created = DateTime.Now,
                    CreatedBy = _currentUserService.UserId,
                    LastModified = DateTime.Now,
                    LastModifiedBy = _currentUserService.UserId,
                };
                _context.Add(comment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cm);
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
                    Created = DateTime.Now,
                    CreatedBy = _currentUserService.UserId,
                    LastModified = DateTime.Now,
                    LastModifiedBy = _currentUserService.UserId,
                };
                //Add file
                if (todoSaveModel.File != null)
                {
                    todo.FileName = await _fileStorageService.SaveFileAsync(todoSaveModel.File);
                }
                //Add Assignment
                var listAssignment = new List<Assignment>();
                if (todoSaveModel.SelectedListUserId != null)
                {
                    listAssignment = todoSaveModel.SelectedListUserId.Select(ele => new Assignment {
                        UserId = ele.ToString(),
                        AssignmentDate = DateTime.UtcNow
                    }).ToList();
                }

                listAssignment.Add(new Assignment {
                    UserId = _currentUserService.UserId,
                    AssignmentDate = DateTime.UtcNow,
                }); // add self to todo
                todo.Assignments = listAssignment;

                _context.Add(todo);
                var successCreateTodo = await _context.SaveChangesAsync() > 0;
                if (successCreateTodo)
                {
                    //Get user entity
                    //var currentUser = await _context.Users.FindAsync(_currentUserService.UserId);
                    var todoHistory = new TodoHistory {
                        TodoId = todo.Id,
                        LogContent = $"Khởi tạo công việc",
                    };
                    _context.Add(todoHistory);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(todoSaveModel);
        }
        public async Task<IActionResult> EditComment(int? id)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == id);
            var todolist = await _context.Todos.Distinct().ToListAsync();
            ViewBag.ListTodos = todolist.Select(t => new SelectListItem() {
                Text = t.Title,
                Value = t.Id.ToString(),
            });
            return View(comment);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditComment(int id, [FromForm] Comment comment)
        {
            if (id != comment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var cm = await _context.Comments.FindAsync(id);
                    cm.TodoId = comment.TodoId;
                    cm.Content = comment.Content;
                    cm.LastModified = comment.LastModified;
                    cm.LastModifiedBy = comment.LastModifiedBy;
                    cm.Created = comment.Created;
                    cm.CreatedBy = comment.CreatedBy;
                    _context.Update(cm);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TodoExists(comment.TodoId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Details));
            }
            return View(comment);
        }
        public async Task<IActionResult> DeleteComment(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // POST: Todoes/Delete/5
        [HttpPost, ActionName("DeleteComment")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCommentConfirmed(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            //return RedirectToAction(nameof(Details));
            return RedirectToAction(nameof(Details), new { id = comment.TodoId });
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var todo = await _context.Todos.Include(t => t.Assignments).FirstOrDefaultAsync(t => t.Id == id);
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
                    var oldTodo = await _context.Todos.AsNoTracking().Where(t => t.Id == id).FirstOrDefaultAsync();
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
                    var successUpdateTodo = await _context.SaveChangesAsync() > 0;
                    if (successUpdateTodo)
                    {
                        //Get user entity
                        //var currentUser = await _context.Users.FindAsync(_currentUserService.UserId);
                        var LogContent = "";

                        if (todo.Title != oldTodo.Title)
                        {
                            LogContent += $"+ Sửa tiêu để từ {oldTodo.Title} thành {todo.Title} \n";
                        }
                        if (todo.Content != oldTodo.Content)
                        {
                            LogContent += $"+ Sửa nội dung từ {oldTodo.Content} thành {todo.Content} \n";
                        }

                        if (todo.Status != oldTodo.Status)
                        {
                            LogContent += $"+ Sửa trạng thái từ {oldTodo.Status} thành {todo.Status} \n";
                        }

                        if (todo.FileName != oldTodo.FileName)
                        {
                            LogContent += $"+ Sửa file từ {oldTodo.FileName} thành {todo.FileName} \n";
                        }

                        if (todo.StartDate != oldTodo.StartDate)
                        {
                            LogContent += $"+ Sửa thời gian bắt đầu từ {oldTodo.StartDate} thành {todo.StartDate} \n";
                        }

                        if (todo.DueDate != oldTodo.DueDate)
                        {
                            LogContent += $"+ Sửa thời gian kết thúc từ {oldTodo.DueDate} thành {todo.DueDate} \n";
                        }

                        var newAssignmentUserString = await newListAssignments.ToUserStringAsync(_context);
                        var oldAssignmentUserString = await oldListAssignments.ToUserStringAsync(_context);

                        if (newAssignmentUserString != oldAssignmentUserString)
                        {
                            LogContent += $"+ Sửa thành viên từ {oldAssignmentUserString} thành {newAssignmentUserString} \n";
                        }

                        if (LogContent != "")
                        {
                            var todoHistory = new TodoHistory {
                                TodoId = todo.Id,
                                LogContent = LogContent,
                            };
                            _context.Add(todoHistory);
                            await _context.SaveChangesAsync();
                        }

                    }
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
            if (todo.FileName != null)
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
