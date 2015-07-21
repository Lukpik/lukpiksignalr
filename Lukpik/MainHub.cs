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

        #region ADD STORE AND PASSWORD GENERATION
        public void addStore(string fname, string lname, string email, string storename, string phonenum, string city, string clientID)
        {
            //string passsword
            Random rnd = new Random();
            int length = rnd.Next(6, 12); // creates a number between 1 and 12
            string password = CreatePassword(length);
            password = Encrypt(password);
            MySQLBusinessLogic bl = new MySQLBusinessLogic();
            int result = bl.AddStore(storename, city, phonenum, DateTime.Now, DateTime.Now, email, fname, lname, 0, 0, password, 1, 0, 1);
            if (result == 1)
                Clients.Client(clientID).testJS("1");
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

        #endregion

        #region LOGIN
        public void login(string username, string pwd, string clientID)
        {
            try
            {
                MySQLBusinessLogic bl = new MySQLBusinessLogic();
                if (bl.LoginUser(username, Encrypt(pwd)) == true)
                {
                    Clients.Client(clientID).loginResult(username, "1");
                }
                else
                {
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
    }
}