using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalRchat.Models;
using SignalRchat.Models.DataModels;
using SignalRChat.Hubs;

namespace SignalRchat.Controllers
{
    public class HomeController : Controller
    {
        private IHangmanRepository repository;
        private IHubContext<ChatHub> hub;
        private readonly List<Question> questionList = new List<Question>();
        // using DI to inject the DBcontext and the HUB instance in the controller (we do not use the hub instance)
        public HomeController(IHangmanRepository repo, IHubContext<ChatHub> hubContext)
        {
            repository = repo;
            hub = hubContext;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        // take the string name from the Index form and pass it to the view in a ViewBag variable
        public IActionResult Game(string name)
        {
            ViewBag.User = name;
            // get all the questions and send them to the view
            List<Question> questionList = repository.Questions.ToList();
            return View(questionList);
        }
    }
}
