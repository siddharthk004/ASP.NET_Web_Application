using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sanmol_Management.Models;

namespace Sanmol_Management.Controllers
{
    public class HomeController : Controller
    {
        private SanmolEn db = new SanmolEn();

        public ActionResult Index()
        {
            // Dashboard statistics
            ViewBag.TotalEmployees = db.Employees.Count();
            ViewBag.TotalCompanies = db.Companies.Count();
            ViewBag.TotalWorkDays = db.Companies.Sum(c => (int?)c.CWorkDay) ?? 0;
            ViewBag.TotalEmployeeCount = db.Companies.Average(c => (int?)c.CEmpCnt) ?? 0;

            // Recent employees (last 5)
            ViewBag.RecentEmployees = db.Employees.OrderByDescending(e => e.Eid).Take(3).ToList();

            // Employee types breakdown
            ViewBag.EmployeeTypes = db.Employees
                .GroupBy(e => e.Etype)
                .Select(g => new { Type = g.Key, Count = g.Count() })
                .ToList();

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public JsonResult ChatBot(string message)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(message))
                {
                    return Json(new { success = false, response = "Please enter a message." });
                }

                string response = ProcessChatMessage(message.ToLower().Trim());
                return Json(new { success = true, response = response });
            }
            catch (Exception ex)
            {
                // Log the error for debugging
                System.Diagnostics.Debug.WriteLine($"ChatBot Error: {ex.Message}");
                return Json(new { success = false, response = "Sorry, I encountered an error. Please try again." });
            }
        }

        private string ProcessChatMessage(string message)
        {
            // Employee count queries
            if (message.Contains("how many employee") || message.Contains("total employee") || message.Contains("count employee"))
            {
                int count = db.Employees.Count();
                return $"We currently have <strong>{count}</strong> employees in our system.";
            }

            // Company count queries
            if (message.Contains("how many compan") || message.Contains("total compan") || message.Contains("count compan"))
            {
                int count = db.Companies.Count();
                return $"We have <strong>{count}</strong> companies registered in our system.";
            }

            // List all employees
            if (message.Contains("list employee") || message.Contains("show employee") || message.Contains("all employee"))
            {
                var employees = db.Employees.Take(10).ToList();
                if (employees.Any())
                {
                    var empList = string.Join("<br/>", employees.Select(e => 
                        $"• <strong>{e.Ename}</strong> - {e.Etype} ({e.Edesign})"));
                    return $"Here are some employees:<br/>{empList}" + 
                           (db.Employees.Count() > 10 ? "<br/><em>...and more</em>" : "");
                }
                return "No employees found in the system.";
            }

            // List all companies
            if (message.Contains("list compan") || message.Contains("show compan") || message.Contains("all compan"))
            {
                var companies = db.Companies.Take(10).ToList();
                if (companies.Any())
                {
                    var compList = string.Join("<br/>", companies.Select(c => 
                        $"• <strong>Company #{c.Cid}</strong> - {c.CEmpCnt} employees, {c.CWorkDay} work days"));
                    return $"Here are our companies:<br/>{compList}" +
                           (db.Companies.Count() > 10 ? "<br/><em>...and more</em>" : "");
                }
                return "No companies found in the system.";
            }

            // Employee types
            if (message.Contains("employee type") || message.Contains("types of employee") || message.Contains("employee breakdown"))
            {
                var types = db.Employees
                    .Where(e => e.Etype != null)
                    .GroupBy(e => e.Etype)
                    .Select(g => new { Type = g.Key, Count = g.Count() })
                    .ToList();
                
                if (types.Any())
                {
                    var typeList = string.Join("<br/>", types.Select(t => 
                        $"• <strong>{t.Type}</strong>: {t.Count} employees"));
                    return $"Employee breakdown by type:<br/>{typeList}";
                }
                return "No employee type data available.";
            }

            // Search specific employee
            if (message.Contains("find employee") || message.Contains("search employee") || message.Contains("employee named"))
            {
                var words = message.Split(' ');
                var searchTerm = words.LastOrDefault();
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    var employee = db.Employees.FirstOrDefault(e => e.Ename.ToLower().Contains(searchTerm));
                    if (employee != null)
                    {
                        return $"Found employee:<br/>" +
                               $"<strong>Name:</strong> {employee.Ename}<br/>" +
                               $"<strong>Type:</strong> {employee.Etype}<br/>" +
                               $"<strong>Designation:</strong> {employee.Edesign}<br/>" +
                               $"<strong>Mobile:</strong> {employee.Emob}<br/>" +
                               $"<strong>Address:</strong> {employee.Eaddr}";
                    }
                }
                return "I couldn't find that employee. Try searching by name.";
            }

            // Recent employees
            if (message.Contains("recent employee") || message.Contains("latest employee") || message.Contains("new employee"))
            {
                var recent = db.Employees.OrderByDescending(e => e.Eid).Take(5).ToList();
                if (recent.Any())
                {
                    var recentList = string.Join("<br/>", recent.Select(e => 
                        $"• <strong>{e.Ename}</strong> - {e.Etype}"));
                    return $"Recent employees:<br/>{recentList}";
                }
                return "No employees found.";
            }

            // Work days statistics
            if (message.Contains("work day") || message.Contains("working day"))
            {
                int totalWorkDays = db.Companies.Sum(c => (int?)c.CWorkDay) ?? 0;
                double avgWorkDays = db.Companies.Average(c => (int?)c.CWorkDay) ?? 0;
                return $"<strong>Total work days:</strong> {totalWorkDays}<br/>" +
                       $"<strong>Average work days per company:</strong> {avgWorkDays:F1}";
            }

            // Statistics
            if (message.Contains("statistic") || message.Contains("stat") || message.Contains("summary") || message.Contains("overview"))
            {
                return $"<strong>📊 System Statistics:</strong><br/>" +
                       $"• Employees: {db.Employees.Count()}<br/>" +
                       $"• Companies: {db.Companies.Count()}<br/>" +
                       $"• Total Work Days: {db.Companies.Sum(c => (int?)c.CWorkDay) ?? 0}<br/>" +
                       $"• Avg Employees/Company: {(db.Companies.Average(c => (int?)c.CEmpCnt) ?? 0):F1}";
            }

            // Help
            if (message.Contains("help") || message.Contains("what can you do") || message.Contains("command"))
            {
                return "I can help you with:<br/>" +
                       "• <em>Employee information</em> (count, list, search, recent)<br/>" +
                       "• <em>Company information</em> (count, list, work days)<br/>" +
                       "• <em>Statistics and overview</em><br/>" +
                       "• <em>Employee types breakdown</em><br/><br/>" +
                       "Try asking: 'How many employees?', 'List companies', 'Show statistics', etc.";
            }

            // Default response
            return "I'm not sure how to help with that. Type <strong>'help'</strong> to see what I can do! 🤖";
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}