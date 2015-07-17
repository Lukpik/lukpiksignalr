using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Lukpik
{
    [HubName("allHubs")]
    public class MainHub : Hub
    {

        public void Hello()
        {
            Clients.All.hello();
        }

        public void activecall()
        {
        }

        public void testMethod(string name, string clientID)
        {
            name = "praveen" + name;
            Clients.Client(clientID).testJS(name);
        }
    }
}