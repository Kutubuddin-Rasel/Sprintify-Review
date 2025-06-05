using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Sprintify.Context;
using Sprintify.Models;
using Sprintify.Services;

namespace Sprintify.Controllers
{
	public class IssueLabelController : Controller
	{
		private readonly IssueLabelService _service;
		public IssueLabelController()
		{
			_service = new IssueLabelService();
		}

		public async Task<ActionResult> Index(int issueId)
		{
			var issue = await new IssueService().GetByIdAsync(issueId);
			if (issue == null) return HttpNotFound();

			ViewBag.IssueId = issueId;
			ViewBag.IssueKey = issue.Key;

			var labels = await _service.GetAllByIssueAsync(issueId);
			return View(labels);  
		}

		public async Task<ActionResult> Details(int issueId, string label)
		{
			var record = await _service.GetByIdAsync(issueId, label);
			if (record == null) return HttpNotFound();
			return View(record); 
		}

		public async Task<ActionResult> Form(int issueId, string label = null)
		{
			var issue = await new IssueService().GetByIdAsync(issueId);
			if (issue == null) return HttpNotFound();

			ViewBag.IssueId = issueId;
			ViewBag.IssueKey = issue.Key;

			if (string.IsNullOrEmpty(label))
			{
				var newLabel = new IssueLabel { IssueId = issueId };
				return View(newLabel);
			}
			else
			{
				var existing = await _service.GetByIdAsync(issueId, label);
				if (existing == null) return HttpNotFound();

				return View(existing);
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Form(
			[Bind(Include = "IssueId,Label")] IssueLabel issueLabel,
			string originalLabel = null
		)
		{
			var issue = await new IssueService().GetByIdAsync(issueLabel.IssueId);
			if (issue == null) return HttpNotFound();

			if (!ModelState.IsValid)
			{
				ViewBag.IssueId = issueLabel.IssueId;
				ViewBag.IssueKey = issue.Key;
				return View(issueLabel);
			}

			try
			{
				if (string.IsNullOrEmpty(originalLabel))
				{
					await _service.CreateAsync(issueLabel);
				}
				else
				{
					if (originalLabel != issueLabel.Label)
					{
						await _service.UpdateAsync(issueLabel.IssueId, originalLabel, issueLabel.Label);
					}
				}
			}
			catch (ArgumentException ex)
			{
				ModelState.AddModelError("", ex.Message);
				ViewBag.IssueId = issueLabel.IssueId;
				ViewBag.IssueKey = issue.Key;
				return View(issueLabel);
			}
			catch (InvalidOperationException ex)
			{
				ModelState.AddModelError("", ex.Message);
				ViewBag.IssueId = issueLabel.IssueId;
				ViewBag.IssueKey = issue.Key;
				return View(issueLabel);
			}
			catch (KeyNotFoundException)
			{
				return HttpNotFound();
			}

			return RedirectToAction("Index", new { issueId = issueLabel.IssueId });
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> DeleteConfirmed(int issueId, string label)
		{
			bool success = await _service.DeleteAsync(issueId, label);
			if (!success) return HttpNotFound();

			return RedirectToAction("Index", new { issueId });
		}
	}
}
