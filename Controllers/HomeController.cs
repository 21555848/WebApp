using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.IO;
using WebApp.Models;

namespace WebApp.Controllers
{
   public class calendarEvent
    {

        public string Summary { get; set; }        
        public string Organizer { get; set; }
        public string  Description { get; set; }
        public string StartTime  { get; set; }
        public string  EndTime { get; set; }

    }
    public class HomeController : Controller
    {
        public List<calendarEvent> GoogleEvents = new List<calendarEvent>();
        static string[] Scopes = { CalendarService.Scope.CalendarReadonly };
        static string ApplicationName = "Google Calender API .NET Quickstart";
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Payemnt()
        {
            return View();
        }
        public IActionResult OnlinePayemnt()
        {
            return View();
        }
        public IActionResult CalendarIndex()
        {
            CalendarEvents();
            ViewBag.EventList = GoogleEvents;
            return View();
        }

        public void CalendarEvents()
        {
           UserCredential creditials;
            string path = Path.GetFullPath("creditials.json");
            using (var stream = 
                new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                string credPath = "token.json";
                creditials = GoogleWebAuthorizationBroker.AuthorizeAsync(GoogleClientSecrets.Load(stream).Secrets, Scopes, "user", CancellationToken.None, new FileDataStore(credPath, true)).Result;
                
            }
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = creditials,
                ApplicationName = ApplicationName,
            });

            EventsResource.ListRequest request = service.Events.List("primary");
            request.TimeMin = DateTime.Now;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.MaxResults = 10;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            Events events = request.Execute();
            Console.WriteLine("Upcoming events");
            if (events.Items != null && events.Items.Count > 0)
            {
                foreach (var eventItem in events.Items)
                {
                    var calendarevent = new calendarEvent();
                    calendarevent.Organizer = eventItem.Organizer.Email;
                    calendarevent.Summary = eventItem.Summary;
                    calendarevent.StartTime = eventItem.Start.DateTime.ToString();
                    calendarevent.EndTime = eventItem.End.DateTime.ToString();
                    calendarevent.Description = eventItem.Description;
                    GoogleEvents.Add(calendarevent);
                }

            }
        }
    }
}