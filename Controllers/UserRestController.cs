using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TodoListMVC.Data;

namespace TodoListMVC.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserRestController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public UserRestController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> SearchSuggestion (string term)
        {
            //var names = 
            return Ok();
        }
    }
}
