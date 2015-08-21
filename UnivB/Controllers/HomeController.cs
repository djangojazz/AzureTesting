using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;

namespace UnivB.Controllers
{
    public class HomeController : Controller
    {
        private string ConnectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
        private string qname = "signups";

        public ActionResult Index()
        {
            ViewBag.Message = "Newsletter Signups.";

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Newsletter(string email)
        {
            var nm = NamespaceManager.CreateFromConnectionString(ConnectionString);

            QueueDescription qd = new QueueDescription(qname);
            qd.MaxSizeInMegabytes = 2048; 
            qd.DefaultMessageTimeToLive = new TimeSpan(0, 5, 0);
            
            if (!nm.QueueExists(qname))
            {
                nm.CreateQueue(qd);
            }

            //Send to the queue
            QueueClient qc = QueueClient.CreateFromConnectionString(ConnectionString, qname);

            // Create amessage with email property
            var bm = new BrokeredMessage();
            bm.Properties["email"] = email;
            qc.Send(bm);

            ViewBag.email = email;

            return View();
        }
    }
}