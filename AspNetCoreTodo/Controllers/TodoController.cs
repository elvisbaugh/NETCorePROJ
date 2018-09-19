using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreTodo.Models;
using AspNetCoreTodo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreTodo.Controllers
{
    [Authorize]
    public class TodoController : Controller
    {
        private readonly ITodoItemService _todoItemService;
        private readonly UserManager<ApplicationUser> _userManager;

        public TodoController(ITodoItemService todoItemService, UserManager<ApplicationUser> userManager)
        {
            _todoItemService = todoItemService;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null) return Challenge();

            var items = await _todoItemService.GetIncompleteItemAsync(currentUser);

            var model = new TodoViewModel()
            {
                Items = items
            };

            return View(model);
        }
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>AddItem(TodoItem newItem)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if(currentUser == null)
            {
                return Challenge();
            }

            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }

            var successful = await _todoItemService.AddItemAsync(newItem, currentUser);

            if(!successful)
            {
                return BadRequest(new { error = "Could not add item." });
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> MarkDone(Guid id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if(currentUser == null)
            {
                return Challenge();
            }
            if(id == Guid.Empty)
            {
                return RedirectToAction("Index");
            }

            var successful = await _todoItemService.MarkDoneAsync(id, currentUser);

            if(!successful)
            {
                return BadRequest("Could not mark the item as done");
            }

            return RedirectToAction("Index");
        }
    }
}