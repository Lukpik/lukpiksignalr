using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Lukpik.Cl;
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
        public void testInsert(string value1, string value2, string clientID)
        {
            MySQLBusinessLogic bl = new MySQLBusinessLogic();
            if (bl.InsertValues(value1, value2) == true)
                Clients.Client(clientID).testJS("success");
            else
                Clients.Client(clientID).testJS("error");
        }

    }
}