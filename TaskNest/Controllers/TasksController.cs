using Microsoft.AspNetCore.Mvc;
using TaskNest.Models;
using TaskNest.Services;

namespace TaskNest.Controllers;

public sealed class TasksController(TaskStore store) : Controller
{
    [HttpGet]
    public IActionResult Index(string? search, string? status, string? priority)
    {
        var items = store.All().AsEnumerable();
        if (!string.IsNullOrWhiteSpace(search))
            items = items.Where(t => t.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                     t.Description.Contains(search, StringComparison.OrdinalIgnoreCase));
        if (status == "active") items = items.Where(t => !t.IsCompleted);
        if (status == "completed") items = items.Where(t => t.IsCompleted);
        if (priority is "Low" or "Medium" or "High") items = items.Where(t => t.Priority == priority);
        ViewBag.Search = search;
        ViewBag.Status = status;
        ViewBag.Priority = priority;
        ViewBag.Total = store.All().Count;
        ViewBag.Completed = store.All().Count(t => t.IsCompleted);
        return View(items.OrderBy(t => t.IsCompleted).ThenBy(t => t.DueDate ?? DateTime.MaxValue).ToList());
    }

    [HttpGet]
    public IActionResult Create() => View("Form", new TaskItem());

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Create(TaskItem task)
    {
        ValidateChoices(task);
        if (!ModelState.IsValid) return View("Form", task);
        store.Add(task);
        TempData["Notice"] = "Task created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var task = store.Get(id);
        return task is null ? NotFound() : View("Form", task);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Edit(int id, TaskItem task)
    {
        if (id != task.Id) return BadRequest();
        ValidateChoices(task);
        if (!ModelState.IsValid) return View("Form", task);
        if (!store.Update(task)) return NotFound();
        TempData["Notice"] = "Task updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Toggle(int id)
    {
        if (!store.Toggle(id)) return NotFound();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        if (!store.Delete(id)) return NotFound();
        TempData["Notice"] = "Task deleted.";
        return RedirectToAction(nameof(Index));
    }

    private void ValidateChoices(TaskItem task)
    {
        if (task.Priority is not ("Low" or "Medium" or "High"))
            ModelState.AddModelError(nameof(task.Priority), "Select a valid priority.");
        if (task.Category is not ("Work" or "Personal" or "Study" or "General"))
            ModelState.AddModelError(nameof(task.Category), "Select a valid category.");
    }
}
