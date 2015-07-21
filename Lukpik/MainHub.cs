using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Lukpik.Cl;
using System.Text;
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
        public void testInsert(string fname, string lname, string email, string storename, string phonenum, string city, string clientID)
        {
            //string passsword
            string password = CreatePassword(8);
            MySQLBusinessLogic bl = new MySQLBusinessLogic();
            if (bl.AddStore(storename, city, phonenum, DateTime.Now, email, fname, lname, 0, 0, password, 1, 0, 1) == true)
                Clients.Client(clientID).testJS("success");
            else
                Clients.Client(clientID).testJS("error");
        }
        public string CreatePassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890#%@+_*";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }
    }
}