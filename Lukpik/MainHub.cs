using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Lukpik.Cl;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Data;
using System.Net.Mail;
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

        #region ADD / UPDATE STORE AND PASSWORD GENERATION
        public void addStore(string fname, string lname, string email, string storename, string phonenum, string city, string clientID)
        {
            //string passsword
            Random rnd = new Random();
            int length = rnd.Next(6, 12); // creates a number between 1 and 12
            string generatedPassword = CreatePassword(length);
            string password = Encrypt(generatedPassword);
            MySQLBusinessLogic bl = new MySQLBusinessLogic();
            int result = bl.AddStore(storename, city, phonenum, DateTime.Now, DateTime.Now, email, fname, lname, 0, 0, password, 1, 0, 1);
            if (result == 1)
            {
                Clients.Client(clientID).testJS("1");
                string body = "Dear " + fname + ",<br> Thanks for Registering with us. Your credentials to proceed with us is as follows.<br>Username: " + email + "<br>Password: " + generatedPassword + "<br><a href='http://localhost:1532/retailers/login.html' target='_blank'>Click here to login</a>";
                string subject = "Thanks for Registering with us.";
                SendEMail("mail@hainow.com", email, subject, body);
                //SendMail(email, subject, body);
                
            }
            else if (result == 2)
                Clients.Client(clientID).testJS("2");
            else
                Clients.Client(clientID).testJS("0");
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

        private string Encrypt(string clearText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            string dec = Decrypt(clearText);
            return clearText;
        }

        private string Decrypt(string cipherText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        public void updateStores(string storename,string storetype,string shortdesc,string storedesc,string brands,string othercategories,string iscreditcard,string istrail,string ishomedelivery,string ownerfirstname,string ownerlastname,string phone,string email,string addline1,string addline2,string city,string state,string country,string pincode,string latitude,string longitude,string websiteurl,string fburl,string twitterurl,string googleurl,string clientID)
        {
            //1. update store table
            //2. add brands to brand table
            //3. add categories ro categories table
            try
            {
                MySQLBusinessLogic bl = new MySQLBusinessLogic();
                bool result = bl.UpdateStoreDetails(storename, storetype, shortdesc, storedesc, brands, othercategories, Convert.ToInt32(iscreditcard), Convert.ToInt32(istrail), Convert.ToInt32(ishomedelivery), ownerfirstname, ownerlastname, phone, email, addline1, addline2, city, state, country, pincode, Convert.ToDouble(latitude), Convert.ToDouble(longitude), websiteurl, fburl, twitterurl, googleurl,DateTime.Now);
                if (result == true)
                    Clients.Client(clientID).updatedStores("1");
                else
                    Clients.Client(clientID).updatedStores("0");
            }
            catch (Exception ex)
            {
                Clients.Client(clientID).updatedStores("0");
            }
        }
        #endregion

        #region LOGIN
        public void login(string username, string pwd, string clientID)
        {
            try
            {
                MySQLBusinessLogic bl = new MySQLBusinessLogic();
                int result=bl.LoginUser(username, Encrypt(pwd));
                if (result == 1)
                {
                    //If success
                    Clients.Client(clientID).loginResult(username, "1");
                }
                else if (result==2)
                {
                    //if  username or pwd doent exist
                    Clients.Client(clientID).loginResult(username, "2");
                }
                else 
                {
                    //if something gone wrong(exception)
                    Clients.Client(clientID).loginResult(username, "0");
                }
            }
            catch (Exception ex)
            {
            }
        }
        #endregion

        public void getRetailerDetails(string username,string clientID)
        {
            try
            {
                MySQLBusinessLogic bl=new MySQLBusinessLogic();
                DataTable storeDetails = new DataTable();
                storeDetails = bl.GetStoreRetailerDetails(username);
                string jsonStr = ConvertDataTabletoString(storeDetails);
                Clients.Client(clientID).gotDetails(jsonStr, "1");
            }
            catch (Exception ex)
            {
                Clients.Client(clientID).gotDetails("", "0");
            }
        }

        #region CONVERTION TO JSON
        public string ConvertDataTabletoString(DataTable dt)
        {
            
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            foreach (DataRow dr in dt.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
            return serializer.Serialize(rows);

        }
        #endregion

        #region CHANGE PASSWORD
        public void changeStorePassword(string email,string oldpwd,string newpwd, string clientID)
        {
            try
            {
                // 1 - success
                // 0 - fail
                MySQLBusinessLogic bl = new MySQLBusinessLogic();
                int result = bl.ChangePassword(email, Encrypt(oldpwd), Encrypt(newpwd));
                if (result == 1)
                {
                    Clients.Client(clientID).changedPassword("1");
                    DataTable dt = new DataTable();
                    dt = bl.GetStoreOwnerName(email);
                    if (dt.Rows.Count == 1)
                    {
                        string name = dt.Rows[0].ItemArray[0].ToString() + " " + dt.Rows[0].ItemArray[0].ToString();
                        string body = "Dear " + name + ",<br> It is to intimate you, that you have recently changed your account password.<br><a href='http://localhost:1532/retailers/login.html' target='_blank'>Click here to login</a>";
                        string subject = "You have changed you password.";
                        SendEMail("mail@hainow.com", email, subject, body);
                        //SendMail(email, subject, body);
                    }
                }
                else if (result == 2)
                {
                    Clients.Client(clientID).changedPassword("2");
                }
                else
                    Clients.Client(clientID).changedPassword("0");
                
            }
            catch (Exception ex)
            {
                Clients.Client(clientID).changedPassword("0");
            }
        }
        #endregion

        #region SEND MAIL

        public string SendEMail(string from, string to, string subject, string body)
        {
            //MailMessage mail = new MailMessage();
            //mail.To = "me@mycompany.com";
            //mail.From = "you@yourcompany.com";
            //mail.Subject = "this is a test email.";
            //mail.Body = "this is my test email body";
            //SmtpMail.SmtpServer = "localhost";  //your real server goes here
            //SmtpMail.Send(mail);
            try
            {
                MailMessage msgobj = new MailMessage();
                msgobj.IsBodyHtml = true;
                msgobj.From = new MailAddress(from);
                msgobj.To.Add(to);
                msgobj.Subject = subject;
                msgobj.Body = body;
                msgobj.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                SmtpClient client = new SmtpClient();
                client.Send(msgobj);
                return "success";
            }
            catch (Exception ex)
            {

                return ex.Message;
            }
        }

        private void SendMail(string toEmailID, string subject, string htmlBody)
        {
            
            try
            {
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                var mail = new MailMessage();
                mail.From = new MailAddress("praveenkumar.vakalapudi@gmail.com");
                mail.To.Add(toEmailID.Trim());
                mail.Subject = subject;
                mail.IsBodyHtml = true;
                mail.Body = htmlBody;
                SmtpServer.Port = 587;
                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.Credentials = new System.Net.NetworkCredential("praveenkumar.vakalapudi@gmail.com", "9014802155");
                SmtpServer.EnableSsl = true;
                //SmtpServer.Send(mail);
                //isSend = true;
                try
                {
                    SmtpServer.Send(mail);
                }
                catch (SmtpFailedRecipientsException ex)
                {
                    for (int i = 0; i < ex.InnerExceptions.Length; i++)
                    {
                        SmtpStatusCode status = ex.InnerExceptions[i].StatusCode;
                        if (status == SmtpStatusCode.MailboxBusy || status == SmtpStatusCode.MailboxUnavailable)
                        {
                            //"Delivery failed - retrying in 5 seconds."
                            System.Threading.Thread.Sleep(5000);
                            SmtpServer.Send(mail);
                        }
                        else
                        {
                            //"Failed to deliver message"                                
                        }
                    }
                }

            }
            catch (Exception ex)
            {
            
            }
            
        }

        #endregion
    }
}