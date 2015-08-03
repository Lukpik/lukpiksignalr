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
using SD = System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Drawing;
using System.Configuration;
using System.Net;
namespace Lukpik
{
    [HubName("allHubs")]
    public class MainHub : Hub
    {
        int retailerProductLimit = 20;
        public void Hello()
        {

            Clients.All.hello();
        }

        public void activecall()
        {

        }

        public string hubstart(string clientID)
        {
            return "5";
        }

        public string imgLocationSave(string fileName, string encodedFileName, string clientID, string pageName, string imgNo)
        {
            string rootPath = System.Configuration.ConfigurationManager.AppSettings.GetValues("RootPath").First().ToString();

            if (fileName != encodedFileName)
            {
                fileName = Regex.Replace(fileName.Substring(0, fileName.LastIndexOf('.')), "[.;]", "_") + fileName.Substring(fileName.LastIndexOf('.'), (fileName.Length - fileName.LastIndexOf('.')));
                if (File.Exists(rootPath + fileName))
                    File.Delete(rootPath + fileName);
                System.IO.File.Move(rootPath + encodedFileName, rootPath + fileName);
                if (pageName == "AddProduct")
                {
                    //productImgSave(fileName, imgNo);
                }
            }
            return "1";

        }


        public void corpimg(string x, string y, string h, string w, string r, string filename, string clientID, string phone)
        {
            string rootPath = System.Configuration.ConfigurationManager.AppSettings.GetValues("RootPath").First().ToString();

            int w1 = Convert.ToInt32(w);
            int h1 = Convert.ToInt32(h);
            int x1 = Convert.ToInt32(x);
            int y1 = Convert.ToInt32(y);

            byte[] CropImage = Crop(rootPath + filename, w1, h1, x1, y1);
            using (MemoryStream ms = new MemoryStream(CropImage, 0, CropImage.Length))
            {
                ms.Write(CropImage, 0, CropImage.Length);
                using (SD.Image CroppedImage = SD.Image.FromStream(ms, true))
                {
                    if (File.Exists(rootPath + filename))
                        File.Delete(rootPath + filename);
                    string SaveTo = rootPath + filename;
                    CroppedImage.Save(SaveTo, CroppedImage.RawFormat);
                    //pnlCrop.Visible = false;
                    //pnlCropped.Visible = true;
                    //imgCropped.ImageUrl = "images/crop" + ImageName;
                }
            }

            //test image save code
            string FileName = rootPath + filename;
            byte[] ImageData;
            FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            ImageData = br.ReadBytes((int)fs.Length);
            br.Close();
            fs.Close();
            MySQLBusinessLogic bl = new MySQLBusinessLogic();
            bool ImgUpdate = false;
            if (ImageData.Length < 16777216)
            {
                int storeID = GetStoreIDbyPhone(phone);
                ImgUpdate = bl.UpdateStoreImage(ImageData, storeID, DateTime.Now);
            }
            else
                Clients.Client(clientID).storeImgPath("", "0");
            //end
            if (ImgUpdate == true)
            {
                updateImage(phone, clientID, "");
                File.Delete(rootPath + filename);
            }
            else
                Clients.Client(clientID).storeImgPath("", "0");



            //Clients.Client(clientID).storeImgPath(pt);
        }

        public void updateImage(string phone, string clientID, string isfrom)
        {
            try
            {

                MySQLBusinessLogic bl = new MySQLBusinessLogic();
                int storeID = GetStoreIDbyPhone(phone);
                DataTable dt = bl.GetStoreImage(storeID);
                byte[] bytes = (Byte[])dt.Rows[0].ItemArray[0];
                if (bytes.Length < 16777216)
                {
                    string base64String = Convert.ToBase64String(bytes, 0, bytes.Length);
                    string ImageUrl = "data:image/png;base64," + base64String;
                    Clients.Client(clientID).storeImgPath(ImageUrl, "1", isfrom);
                }
                else
                    Clients.Client(clientID).storeImgPath("", "0", isfrom);

            }
            catch (Exception ex)
            {
                Clients.Client(clientID).storeImgPath("", "0", isfrom);
            }
        }

        static byte[] Crop(string Img, int Width, int Height, int X, int Y)
        {
            try
            {
                using (SD.Image OriginalImage = SD.Image.FromFile(Img))
                {
                    using (SD.Bitmap bmp = new SD.Bitmap(Width, Height))
                    {
                        bmp.SetResolution(OriginalImage.HorizontalResolution, OriginalImage.VerticalResolution);
                        using (SD.Graphics Graphic = SD.Graphics.FromImage(bmp))
                        {
                            Graphic.SmoothingMode = SmoothingMode.AntiAlias;
                            Graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            Graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
                            Graphic.DrawImage(OriginalImage, new SD.Rectangle(0, 0, Width, Height), X, Y, Width, Height, SD.GraphicsUnit.Pixel);
                            MemoryStream ms = new MemoryStream();
                            bmp.Save(ms, OriginalImage.RawFormat);
                            return ms.GetBuffer();
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                throw (Ex);
            }
        }


        public void testMethod(string name, string clientID)
        {
            //name = "praveen" + name;
            //Clients.Client(clientID).testJS(name);

            string body = "<html> <head> <meta http-equiv='Content-Type' content='text/html; charset=utf-8'/> <meta name='viewport' content='width=device-width, initial-scale=1.0'/> <title>Responsive email</title> <style type='text/css'> body{margin: 10px 0; padding: 0 10px; background: #F9F2E7; font-size: 13px;}table{border-collapse: collapse;}td{font-family: Calibri; color: #333333;}@media only screen and (max-width: 480px){body,table,td,p,a,li,blockquote{-webkit-text-size-adjust:none !important;}table{width: 100% !important;}.responsive-image img{height: auto !important; max-width: 50% !important; width: 50% !important;}}</style> </head> <body> <table border='0' cellpadding='0' cellspacing='0' width='100%'> <tr> <td> <table border='0' cellpadding='0' cellspacing='0' align='center' width='640' bgcolor='#FFFFFF'> <tr> <td style='font-size: 0; line-height: 0; padding: 0 10px; background:url(http://www.lukpik.com/img/comingsoon.png)' height='140' align='center' class='responsive-image'> <img src='http://www.lukpik.com/img/logowhite.png' width='200' alt=''/> </td></tr><tr><td style='font-size: 0; line-height: 0;' height='30'>&nbsp;</td></tr><tr> <td style='padding: 10px 10px 20px 10px;'> <div style='font-size: 25px;color:rgb(42, 177, 237);text-align:center;font-weight:bold;'>We are happy to have you on board !</div><br/> <div style='font-size: 20px;color:rgb(91, 90, 90);text-align:center;'> Get ready to wipe the boot stains of your store floor more often. </div><br/> <div style='font-family:Calibri; font-size: 25px;color:rgb(91, 90, 90);text-align:center;font-weight:bold;'> Customers are coming ! </div><br/> <div style='font-family:Calibri; font-size: 18px;color:rgb(91, 90, 90);text-align:justify;'> You are almost there , please login to lukpik and fill in details about your store and with in few minutes you can start uploading products you want to sell. </div></td></tr><tr><td style='font-size: 0; line-height: 0;' height='1' bgcolor='#F9F9F9'>&nbsp;</td></tr><tr><td style='font-size: 0; line-height: 0;' height='30'>&nbsp;</td></tr><tr> <td> </td></tr><tr><td style='font-size: 0; line-height: 0;' height='20'>&nbsp;</td></tr><tr> <td bgcolor='#485465'> <table border='0' cellpadding='0' cellspacing='0' width='100%'> <tr><td style='font-size: 0; line-height: 0;' height='15'>&nbsp;</td></tr><tr> <td style='padding: 0 10px; color: #FFFFFF;'> Lukpik </td></tr><tr><td style='font-size: 0; line-height: 0;' height='15'>&nbsp;</td></tr></table> </td></tr></table> </td></tr></table> </body></html>";


            string subject = "You have changed you password.";
            //SendEMail("support@lukpik.com", "nagarjuna.kendyala@hotmail.com", subject, body);
        }

        #region ADD / UPDATE STORE AND PASSWORD GENERATION
        public void addStore(string fname, string lname, string email, string storename, string phonenum, string phonenum2, string city, string clientID)
        {
            //string passsword
            Random rnd = new Random();
            int length = rnd.Next(6, 12); // creates a number between 1 and 12
            string generatedPassword = CreatePassword(length);
            string password = Encrypt(generatedPassword);
            MySQLBusinessLogic bl = new MySQLBusinessLogic();
            bool isEmailNull = false;
            if (email == "")
            {
                //string store = storename.Trim().Replace(" ", "");
                //email = store + "." + phonenum + "@lukpik.com";
                isEmailNull = true;
            }
            int result = bl.AddStore(storename, city, phonenum,phonenum2, DateTime.Now, DateTime.Now, email, fname, lname, 0, 0, password, 1, 0, 1);
            if (result == 1)
            {
                Clients.Client(clientID).testJS("1");
                string loginRedirect = System.Configuration.ConfigurationManager.AppSettings.GetValues("LoginRedirect").FirstOrDefault().ToString();
                //string body = "Dear " + fname + ",<br> Thanks for Registering with us. Your credentials to proceed with us is as follows.<br>Username: " + email + "<br>Password: " + generatedPassword + "<br><a href='" + loginRedirect + "' target='_blank'>Click here to login</a>";

                string body1 = "<html><head> <meta http-equiv='Content-Type' content='text/html; charset=utf-8'/> <meta name='viewport' content='width=device-width, initial-scale=1.0'/> <title>Lukpik</title> <style type='text/css'> table{border-collapse: collapse;}td{font-family: Calibri; color: #333333;}@media only screen and (max-width: 480px){body, table, td, p, a, li, blockquote{-webkit-text-size-adjust: none !important;}table{width: 100% !important;}.responsive-image img{height: auto !important; max-width: 100% !important; width: 100% !important;}}</style></head><body style='margin: 10px 0; padding: 0 10px; background: #F5F7FA; font-size: 13px;'> <table border='0' cellpadding='0' cellspacing='0' width='100%'> <tr> <td> <table border='0' cellpadding='0' cellspacing='0' align='center' width='640' bgcolor='#FFFFFF'> <tr> <td style='font-size: 0; line-height: 0; padding: 0;' align='center' class='responsive-image'> <img src='http://www.lukpik.com/img/emailicons/retailermailbanner.png' width='100%' alt=''/> </td></tr><tr><td style='font-size: 0; line-height: 0;' height='10'>&nbsp;</td></tr><tr> <td style='padding: 10px 20px 20px 20px;'> <div style='font-size: 25px;color:rgb(42, 177, 237);text-align:center;font-weight:bold;'>Happy to have you on board !</div><br/> <div style='font-size: 20px;color:rgb(91, 90, 90);text-align:center;'> Get ready to wipe the boot stains of your store floor more often. </div><br/> <div style='font-family:Calibri; font-size: 23px;color:rgb(91, 90, 90);text-align:center;font-weight:bold;'> Customers are coming ! </div><br/> <div style='font-family:Calibri; font-size: 16px;color:rgb(91, 90, 90);text-align:justify;'> Please login to lukpik and fill in details about your store. In next few minutes you can start uploading products you want to sell. </div><br/> <div style='font-family:Calibri; font-size: 16px;color:rgb(91, 90, 90);text-align:justify;'>";

                string adminbody = " User login credentials:<br/><br/> <b>Username : </b>" + phonenum + "<br/> <b>Password : </b>" + generatedPassword + "<br/><br/>";

                string userbody = "Your credentials has been sent to your mobile number ending with XXXXXXX" + phonenum.Substring(7) + ".<br/><br/>";

                string body2= " </div><br/> <div style='font-family:Calibri; font-size: 16px;color:rgb(91, 90, 90);text-align:center;'> <a type='button' style='-webkit-border-radius: 0; -moz-border-radius: 0; border-radius: 0px; color: #ffffff; font-size: 20px; background: #22c9ad; padding: 10px 20px 10px 20px; text-decoration: none;' target='_blank' href='" + loginRedirect + "'>Click here to login</a> </div><br/> <br/> <div style='font-family:Calibri; font-size: 16px;color:rgb(91, 90, 90);text-align:justify;'> For any further assistance. Just drop us a mail at <a href='mailto:support@lukpik.com' target='_top'>support@lukpik.com</a>. We will get back to you as soon as we can. <br/><br/> We'll be in touch periodically with additional resources and important updates. <br/><br/>Sincerely,<br/><b>Lukpik Team</b><br/> </div></td></tr><tr> <td></td></tr><tr><td style='font-size: 0; line-height: 0;' height='20'>&nbsp;</td></tr><tr> <td bgcolor='#485465'> <table border='0' cellpadding='0' cellspacing='0' width='100%'> <tr><td style='font-size: 0; line-height: 0;' height='15'>&nbsp;</td></tr><tr> <td style='padding: 0 10px; color: #FFFFFF;'> <table border='0' width='100%'> <tr> <td style='color:white;' width='20%'> <a href='http://www.lukpik.com' target='_blank' style='font-size:15px;color:white;'>Lukpik</a> </td><td style='color:white;' width='80%' align='right'> <div style='text-align:right;'> <a href='mailto:support@lukpik.com' style='text-decoration:none;'> <img src='http://www.lukpik.com/img/emailicons/email.png' width='35' style='margin-right:5px;'/> </a> <a href='http://www.facebook.com/Itslukpik' target='_blank' style='text-decoration:none;'> <img src='http://www.lukpik.com/img/emailicons/facbk.png' width='35' style='margin-right:5px;'/> </a> <a href='https://twitter.com/Its_lukpik' target='_blank' style='text-decoration:none;'> <img src='http://www.lukpik.com/img/emailicons/twitter.png' width='35' style='margin-right:5px;'/> </a> <a href='http://www.lukpik.com' target='_blank' style='text-decoration:none;'> <img src='http://www.lukpik.com/img/emailicons/website.png' width='35' style='margin-right:5px;'/> </a> </div></td></tr></table> </td></tr><tr><td style='font-size: 0; line-height: 0;' height='15'>&nbsp;</td></tr></table> </td></tr></table> </td></tr></table></body></html>";

                string subject = storename + "- Thanks for Registering with us.";

                string userSMSText = "Welcome to Lukpik. Your credentials as follows:\nUsername : " + phonenum + "\nPassword : " + generatedPassword;
                if (phonenum != "")
                    SendSMS(userSMSText, phonenum);

                SendEMail("support@lukpik.com", "lukpik.store@gmail.com", subject, (body1 + adminbody + body2));
                if (email != "" && !isEmailNull)
                    SendEMail("support@lukpik.com", email, subject, (body1 + userbody + body2));
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

        public void updateStores(string storename, string storetype, string shortdesc, string storedesc, string brands, string othercategories, string iscreditcard, string istrail, string ishomedelivery, string ownerfirstname, string ownerlastname, string phone, string email, string addline1, string addline2, string city, string state, string country, string pincode, string latitude, string longitude, string websiteurl, string fburl, string twitterurl, string googleurl, string clientID)
        {
            //1. update store table
            //2. add brands to brand table
            //3. add categories ro categories table
            try
            {
                MySQLBusinessLogic bl = new MySQLBusinessLogic();
                //Check for Email availability
                bool emailAvailability = false;
                //if (email != "")
                //    emailAvailability = bl.CheckEmailExistance(email);

                //bool moblenumberAvailability = bl.CheckPhoneExistance(storename, phone);

                //if (!emailAvailability)
                //{

                //    //Check for Mobile availability
                //    if (!moblenumberAvailability)
                //    {
                       
                //    }
                //    else
                //    {
                //        //mobile
                //        Clients.Client(clientID).updatedStores("4");
                //    }
                //}
                //else
                //{
                //    //email id 
                //    Clients.Client(clientID).updatedStores("3");
                //}
                int storeID = GetStoreIDbyPhone(phone);
                bool result = bl.UpdateStoreDetails(storename, storetype, shortdesc, storedesc, brands, othercategories, Convert.ToInt32(iscreditcard), Convert.ToInt32(istrail), Convert.ToInt32(ishomedelivery), ownerfirstname, ownerlastname, phone, email, addline1, addline2, city, state, country, pincode, Convert.ToDouble(latitude), Convert.ToDouble(longitude), websiteurl, fburl, twitterurl, googleurl, DateTime.Now, storeID);

                //Update Brands

                //Update Other categories

                bool catResult = bl.UpdateOtherCategories(storeID, othercategories);
                bool brabdResult = bl.UpdateBrands(storeID, brands);

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
        private void CreateIfMissing()
        {
            try
            {
                //var dir = new DirectoryInfo(@"..\\");
                //File.Create(dir.FullName + "\\file.ext");

                string st = System.Configuration.ConfigurationManager.AppSettings.GetValues("RootPath").First().ToString();
                st = st.Replace("file:\\", "");
                var dir = new DirectoryInfo(@st + "\\Lukpik\\StoreImages\\");
                File.Create(dir.FullName + "\\file.ext");

            }
            catch (Exception ex)
            {
            }
        }
        public void login(string username, string pwd, string clientID)
        {
            try
            {
                int storeID = GetStoreIDbyPhone(username);
                MySQLBusinessLogic bl = new MySQLBusinessLogic();
                int result = bl.LoginUser(username, Encrypt(pwd));
                //string email = res.Split('_')[0];
                //int result = Convert.ToInt32(res.Split('_')[1]);
                if (result == 1)
                {
                    //If success

                    Clients.Client(clientID).loginResult(username, "1");
                    bl.AddtoRetailerLoginHistory(storeID, DateTime.Now);
                }
                else if (result == 2)
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
                Clients.Client(clientID).loginResult(username, "0");
            }
        }

        public int GetStoreIDbyPhone(string phone)
        {
            int retVal = 0;
            try
            {
                MySQLBusinessLogic bl = new MySQLBusinessLogic();
                DataTable dt = new DataTable();
                dt = bl.GetStoreID(phone);
                retVal = Convert.ToInt32(dt.Rows[0].ItemArray[0]);
            }
            catch (Exception ex)
            {
                retVal = 0;
            }
            return retVal;
        }
        public void checkFirstTimeLogin(string phone, string clinetID)
        {
            try
            {
                MySQLBusinessLogic bl = new MySQLBusinessLogic();
                int storeID = GetStoreIDbyPhone(phone);
                if (bl.RetailerLoginCount(storeID) == 1)
                    Clients.Client(clinetID).isfirstTime("1");


            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        public void getRetailerDetails(string phone, string clientID)
        {
            try
            {
                int storeID = GetStoreIDbyPhone(phone);
                MySQLBusinessLogic bl = new MySQLBusinessLogic();
                DataTable storeDetails = new DataTable();
                storeDetails = bl.GetStoreRetailerDetails(storeID);
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
        public void changeStorePassword(string phone, string oldpwd, string newpwd, string clientID)
        {
            try
            {
                // 1 - success
                // 0 - fail
                int storeID = GetStoreIDbyPhone(phone);
                MySQLBusinessLogic bl = new MySQLBusinessLogic();
                int result = bl.ChangePassword(storeID, phone, Encrypt(oldpwd), Encrypt(newpwd));
                if (result == 1)
                {
                    Clients.Client(clientID).changedPassword("1");
                    DataTable dt = new DataTable();
                    dt = bl.GetStoreOwnerName(storeID);
                    if (dt.Rows.Count == 1)
                    {
                        string loginRedirect = System.Configuration.ConfigurationManager.AppSettings.GetValues("LoginRedirect").FirstOrDefault().ToString();
                        string name = dt.Rows[0].ItemArray[0].ToString();// + " " + dt.Rows[0].ItemArray[0].ToString();
                        string email = dt.Rows[0].ItemArray[2].ToString();
                        string midText = "Dear " + name + ",<br> It is to intimate you, that you have recently changed your account password.<br>";
                        string body = AttachTexttoMailTemplate(midText, loginRedirect, "Intimation of changing password !", false);
                        string subject = "You have changed you password.";
                        SendSMS("Hai " + name + ", your password has been changed successfully.", phone);
                        SendEMail("support@lukpik.com", email, subject, body);
                        SendEMail("support@lukpik.com", "lukpik.store@gmail.com", subject, body);
                        //Notification 
                        
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
                //MailMessage msgobj = new MailMessage();
                //msgobj.IsBodyHtml = true;
                //msgobj.From = new MailAddress(from);
                //msgobj.To.Add(to);
                //msgobj.Subject = subject;
                //msgobj.Body = body;
                //msgobj.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                //SmtpClient client = new SmtpClient();
                //client.Port=
                //client.Send(msgobj);
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                var mail = new MailMessage();
                mail.From = new MailAddress("support@lukpik.com");
                mail.To.Add(to.Trim());
                mail.Subject = subject;
                mail.IsBodyHtml = true;
                mail.Body = body;
                SmtpServer.Port = 587;
                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.Credentials = new System.Net.NetworkCredential("support@lukpik.com", "lukpik123");
                SmtpServer.EnableSsl = true;
                SmtpServer.Send(mail);

                return "success";
            }
            catch (Exception ex)
            {

                return ex.Message;
            }
        }

        public void SendSMS(string messageContent, string mobilenumber)
        {
            try
            {
                string username = "demo7788";
                string password = "demo7788";

                string sUrl = "http://sms99.co.in/pushsms.php?username=" + username + "&password=" + password + "&sender=CAMERA&message=" + messageContent + "&numbers=" + mobilenumber + "";
                string response = GetResponse(sUrl);
            }
            catch (Exception ex)
            {
            }
        }

        public static string GetResponse(string sURL)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sURL); request.MaximumAutomaticRedirections = 4;
            request.Credentials = CredentialCache.DefaultCredentials;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse(); Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8); string sResponse = readStream.ReadToEnd();
                response.Close();
                readStream.Close();
                return sResponse;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        private string AttachTexttoMailTemplate(string middleText, string loginRedirect, string heading, bool disableButton)
        {
            //hide button when disableButton is true
            //show button when disableButton is false
            string st = "";
            try
            {
                st = "<html><head> <meta http-equiv='Content-Type' content='text/html; charset=utf-8'/> <meta name='viewport' content='width=device-width, initial-scale=1.0'/> <title>Lukpik</title> <style type='text/css'> table{border-collapse: collapse;}td{font-family: Calibri; color: #333333;}@media only screen and (max-width: 480px){body, table, td, p, a, li, blockquote{-webkit-text-size-adjust: none !important;}table{width: 100% !important;}.responsive-image img{height: auto !important; max-width: 100% !important; width: 100% !important;}}</style></head><body style='margin: 10px 0; padding: 0 10px; background: #F5F7FA; font-size: 13px;'> <table border='0' cellpadding='0' cellspacing='0' width='100%'> <tr> <td> <table border='0' cellpadding='0' cellspacing='0' align='center' width='640' bgcolor='#FFFFFF'> <tr> <td style='font-size: 0; line-height: 0; padding: 0;' align='center' class='responsive-image'> <img src='http://www.lukpik.com/img/emailicons/retailermailbanner.png' width='100%' alt=''/> </td></tr><tr><td style='font-size: 0; line-height: 0;' height='10'>&nbsp;</td></tr><tr> <td style='padding: 10px 20px 20px 20px;'> <div style='font-size: 25px;color:rgb(42, 177, 237);text-align:center;font-weight:bold;'>" + heading + "</div><br/> <br/> <div style='font-family:Calibri; font-size: 16px;color:rgb(91, 90, 90);text-align:justify;'> " + middleText + " </div><br/>";
                if (!disableButton)
                {
                    st = st + "<br/> <div style='font-family:Calibri; font-size: 16px;color:rgb(91, 90, 90);text-align:center;'> <a type='button' style='-webkit-border-radius: 0; -moz-border-radius: 0; border-radius: 0px; color: #ffffff; font-size: 20px; background: #22c9ad; padding: 10px 20px 10px 20px; text-decoration: none;' target='_blank' href='" + loginRedirect + "'>Click here to login</a> </div>";
                }
                st = st + "<br/> <br/> <div style='font-family:Calibri; font-size: 16px;color:rgb(91, 90, 90);text-align:justify;'> For any further assistance. Just drop us a mail at <a href='mailto:support@lukpik.com' target='_top'>support@lukpik.com</a>. We will get back to you as soon as we can. <br/><br/>Sincerely,<br/><b>Lukpik Team</b><br/> </div></td></tr><tr> <td></td></tr><tr><td style='font-size: 0; line-height: 0;' height='20'>&nbsp;</td></tr><tr> <td bgcolor='#485465'> <table border='0' cellpadding='0' cellspacing='0' width='100%'> <tr><td style='font-size: 0; line-height: 0;' height='15'>&nbsp;</td></tr><tr> <td style='padding: 0 10px; color: #FFFFFF;'> <table border='0' width='100%'> <tr> <td style='color:white;' width='20%'> <a href='http://www.lukpik.com' target='_blank' style='font-size:15px;color:white;'>Lukpik</a> </td><td style='color:white;' width='80%' align='right'> <div style='text-align:right;'> <a href='mailto:support@lukpik.com' style='text-decoration:none;'> <img src='http://www.lukpik.com/img/emailicons/email.png' width='35' style='margin-right:5px;'/> </a> <a href='http://www.facebook.com/Itslukpik' target='_blank' style='text-decoration:none;'> <img src='http://www.lukpik.com/img/emailicons/facbk.png' width='35' style='margin-right:5px;'/> </a> <a href='https://twitter.com/Its_lukpik' target='_blank' style='text-decoration:none;'> <img src='http://www.lukpik.com/img/emailicons/twitter.png' width='35' style='margin-right:5px;'/> </a> <a href='http://www.lukpik.com' target='_blank' style='text-decoration:none;'> <img src='http://www.lukpik.com/img/emailicons/website.png' width='35' style='margin-right:5px;'/> </a> </div></td></tr></table> </td></tr><tr><td style='font-size: 0; line-height: 0;' height='15'>&nbsp;</td></tr></table> </td></tr></table> </td></tr></table></body></html>";
            }
            catch (Exception ex)
            {
            }
            return st;
        }
        #endregion

        public void getAllSelectedCategories(string phone, string clientID)
        {
            try
            {
                string json1 = "";//list of all categories
                string json2 = "";//list of categories selected by user
                int storeID = GetStoreIDbyPhone(phone);

                DataTable dtAllCategories = new DataTable();
                MySQLBusinessLogic bl = new MySQLBusinessLogic();
                dtAllCategories = bl.GetOtherCategories();
                if (dtAllCategories.Rows.Count > 0)
                    json1 = ConvertDataTabletoString(dtAllCategories);

                List<string> lstSelected = new List<string>();
                
                lstSelected = bl.GetOtherCategoriesbyStoreID(storeID);

                var jsonSerialiser = new JavaScriptSerializer();
                json2 = jsonSerialiser.Serialize(lstSelected);

                Clients.Client(clientID).gotAllCatAndSelectedCat(json1, json2);
            }
            catch (Exception ex)
            {
            }
        }

        public void getAllSelectedBrands(string phone, string clientID)
        {
            try
            {
                string json1 = "";
                MySQLBusinessLogic bl = new MySQLBusinessLogic();
                List<string> lstSelected = new List<string>();
                int storeID = GetStoreIDbyPhone(phone);
                lstSelected = bl.GetBrandsbyStoreID(storeID);
                //var jsonSerialiser = new JavaScriptSerializer();
                //json1 = jsonSerialiser.Serialize(lstSelected);
                var str = String.Join(",", lstSelected);
                Clients.Client(clientID).gotAllSelectedBrands(str);
            }
            catch (Exception ex)
            {
            }
        }

        #region FORGOT PASSWORD
        public void getRetailerPassword(string phone, string clientID)
        {
            try
            {
                MySQLBusinessLogic bl = new MySQLBusinessLogic();
                DataTable dt = new DataTable();
                dt = bl.GetPassword(phone);
                if (dt.Rows.Count == 1)
                {
                    //Send Email
                    string pwd = Decrypt(dt.Rows[0].ItemArray[0].ToString());

                    string fname = dt.Rows[0].ItemArray[1].ToString();
                    string email = dt.Rows[0].ItemArray[2].ToString();
                    string loginRedirect = System.Configuration.ConfigurationManager.AppSettings.GetValues("LoginRedirect").FirstOrDefault().ToString();
                    //Mail section
                    
                    string userText = "Dear " + fname + ",<br> You have requested for your password. We sent your password to your registered mobile number ending with XXXXXXX"+phone.Substring(7)+".<br>";

                    string adminText = "Username of:  " + fname + ",<br>  have requested for password. Please find the password below<br>Username: " + phone + "<br>Password: " + pwd + "<br>";

                    string subject = "Forgot Password.";
                    Clients.Client(clientID).gotPassword("1");
                    //user body
                    string body = AttachTexttoMailTemplate(userText, loginRedirect, "Here is your password !", false);

                    //admin body
                    string adminBody = AttachTexttoMailTemplate(userText, loginRedirect, "Here is your password !", false);

                    if (email != "" || email != null)
                        SendEMail("support@lukpik.com", email, subject, body);
                    SendEMail("support@lukpik.com", "lukpik.store@gmail.com", "User :" + phone + ": Requested for password", adminBody);

                    //SMS section
                    string userSMSText="Hai "+fname+", here is your password : "+pwd;
                    SendSMS(userSMSText, phone);
                }
                else
                {
                    //Email ID doesnt exists
                    Clients.Client(clientID).gotPassword("2");
                }
            }
            catch (Exception ex)
            {
                Clients.Client(clientID).gotPassword("0");
            }
        }
        #endregion

        #region CONTACT US & SUBSCRIPTION
        public void contactUs(string name, string email, string phone, string message, string clientID)
        {
            try
            {
                Clients.Client(clientID).mailSent("1");
                string fname = name;
                string midText = "Dear " + fname + ",<br> Thank you for contacting us. We will get back to you shortly.";
                string subject = "Thanks for contacting us.";
                string body = AttachTexttoMailTemplate(midText, "", "Thank you for contacting us.", true);
                SendEMail("support@lukpik.com", email, subject, body);

                string adminsub = "Contact us";
                string adminbody = "Details are: <br>Name:" + name + "<br>Phone:" + phone + "<br>Email:" + email + "<br>Message:" + message + "";
                SendEMail("support@lukpik.com", "support@lukpik.com", adminsub, adminbody);
            }
            catch (Exception ex)
            {
                Clients.Client(clientID).mailSent("0");
            }
        }

        public void subscribe(string email, string clientID)
        {
            try
            {
                
                MySQLBusinessLogic bl = new MySQLBusinessLogic();
                int result = bl.SubscriptionEmail(email, DateTime.Now, 1);
                if (result == 1)
                {
                    //Subscribed
                    Clients.Client(clientID).subscribed("1");
                }
                else if (result == 2)
                {
                    //Already registered
                    Clients.Client(clientID).subscribed("2");
                }
                else
                {
                    Clients.Client(clientID).subscribed("0");
                }
            }
            catch (Exception ex)
            {
                Clients.Client(clientID).subscribed("0");
            }
        }
        #endregion

        #region PRODUCTS

        public void addProduct(string productname, string gender, string productFamilyId, string productdescription, string price, string quantity, string size, string color, string visibility, string productCat_Sub_ID, string brandID, string collection, string images, string email, string clientID, string fileNames1, string ecommecelink)
        {
            try
            {
                int productCategoryID = Convert.ToInt32(productCat_Sub_ID.Split('_')[1]);
                int productSubCategoryID = Convert.ToInt32(productCat_Sub_ID.Split('_')[0]);

                var fileNames = fileNames1.Split(',');
                var fileimages = images.Split(',');
                byte[] PrvImg = null;
                List<byte[]> lstByte = new List<byte[]>();
                string rootPath = System.Configuration.ConfigurationManager.AppSettings.GetValues("RootPath").First().ToString();


                for (int i = 0; i < fileNames.Length; i++)
                {

                    string fileName = fileNames[i];
                    string encodedFileName = fileimages[i];
                    if (fileName != encodedFileName)
                    {
                        fileName = Regex.Replace(fileName.Substring(0, fileName.LastIndexOf('.')), "[.;]", "_") + fileName.Substring(fileName.LastIndexOf('.'), (fileName.Length - fileName.LastIndexOf('.')));
                        if (File.Exists(rootPath + fileName))
                            File.Delete(rootPath + fileName);
                        System.IO.File.Move(rootPath + encodedFileName, rootPath + fileName);
                        if (i == 0)
                        {
                            Image image = Image.FromFile(rootPath + fileName);
                            Image thumb = image.GetThumbnailImage(100, 100, () => false, IntPtr.Zero);
                            thumb.Save(Path.ChangeExtension(rootPath + fileName.Split('.')[0], "thumb"));
                            System.IO.File.Move(rootPath + fileName.Split('.')[0] + ".thumb", rootPath + "New" + fileName);
                            FileStream fs1 = new FileStream(rootPath + "New" + fileName, FileMode.Open, FileAccess.Read);
                            BinaryReader br1 = new BinaryReader(fs1);
                            PrvImg = br1.ReadBytes((int)fs1.Length);
                            br1.Close();
                            fs1.Close();
                            File.Delete(rootPath + "New" + fileName);
                        }

                    }
                    //test image save code
                    string FileName = rootPath + fileName;
                    byte[] ImageData;
                    FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(fs);
                    ImageData = br.ReadBytes((int)fs.Length);
                    br.Close();
                    fs.Close();

                    if (ImageData.Length < 16777216)
                        lstByte.Add(ImageData);

                }





                MySQLBusinessLogic bl = new MySQLBusinessLogic();
                //int brandID = 0;
                //brandID = bl.GetBrandNameID(brand);
                //if (brandID == 0)
                //{
                //    brandID = bl.InsertBrandandGetID(brand);
                //}

                DataTable dt = bl.GetStoreID(email);
                int storeID = Convert.ToInt32(dt.Rows[0].ItemArray[0]);
                int retVal = bl.AddProduct(productname, gender, Convert.ToInt32(productFamilyId), productdescription, Convert.ToDouble(price), quantity, size, color, Convert.ToInt32(visibility), productCategoryID, productSubCategoryID, Convert.ToInt32(brandID), collection, images, storeID, DateTime.Now, email, lstByte, PrvImg, ecommecelink);
                if (size != "" || color != "" || collection != "")
                {
                    bl.AddSpecification(email, color, size, collection);
                }
                if (retVal == 1)
                    Clients.Client(clientID).addedProduct("1");
                else
                    Clients.Client(clientID).addedProduct("0");
            }
            catch (Exception ex)
            {
                Clients.Client(clientID).addedProduct("0");
            }
        }

        public void updateProduct(string productID, string productname, string gender, string productFamilyId, string productdescription, string price, string quantity, string size, string color, string visibility, string productCat_Sub_ID, string brandID, string collection, string images, string email, string clientID, string fileNames1, string ecommecelink)
        {
            try
            {
                int productCategoryID = Convert.ToInt32(productCat_Sub_ID.Split('_')[1]);
                int productSubCategoryID = Convert.ToInt32(productCat_Sub_ID.Split('_')[0]);
                int pro_ID = 0;
                if (productID != "")
                    pro_ID = Convert.ToInt32(productID);
                MySQLBusinessLogic bl = new MySQLBusinessLogic();
                DataTable dt = bl.GetStoreID(email);
                int storeID = Convert.ToInt32(dt.Rows[0].ItemArray[0]);
                int result = bl.UpdateProduct(pro_ID, productname, gender, Convert.ToInt32(productFamilyId), productdescription, Convert.ToDouble(price), quantity, size, color, Convert.ToInt32(visibility), productCategoryID, productSubCategoryID, Convert.ToInt32(brandID), collection, storeID, DateTime.Now, email, ecommecelink);
                if (result == 1)
                    Clients.Client(clientID).productUpdated("1");
                else
                    Clients.Client(clientID).productUpdated("0");

            }
            catch (Exception ex)
            {
                Clients.Client(clientID).addedProduct("0");
            }
        }
        public void removeProduct(string productID, string trID, string clientID)
        {
            try
            {
                if (productID != "")
                {
                    int pro_id = Convert.ToInt32(productID);
                    MySQLBusinessLogic bl = new MySQLBusinessLogic();
                    if (bl.RemoveProduct(pro_id))
                    {
                        Clients.Client(clientID).removedProduct("1", trID);
                    }
                    else
                    {
                        Clients.Client(clientID).removedProduct("0", trID);
                    }
                }
            }
            catch (Exception ex)
            {
                Clients.Client(clientID).removedProduct("0", trID);
            }
        }
        public void getProductDetails(string email, string productID, string clientID)
        {
            try
            {
                int prod_ID = 0;
                if (productID != "")
                    prod_ID = Convert.ToInt32(productID);
                MySQLBusinessLogic bl = new MySQLBusinessLogic();
                DataTable dtStore = bl.GetStoreID(email);
                int storeID = Convert.ToInt32(dtStore.Rows[0].ItemArray[0]);

                DataTable dt = new DataTable();
                dt = bl.GetProductDetails(storeID, prod_ID);

                DataTable dtImage = new DataTable();
                dtImage = bl.GetProductImageDetails(storeID, prod_ID);
                string json2 = "";
                if (dt.Rows.Count > 0)
                {
                    string json = ConvertDataTabletoString(dt);
                    if (dtImage.Rows.Count > 0)
                    {
                        for (int i = 0; i < dtImage.Rows.Count; i++)
                        {
                            if (dtImage.Rows[i].ItemArray[0].GetType().Name != "DBNull")
                            {
                                byte[] bytes = (Byte[])dtImage.Rows[i].ItemArray[0];
                                if (bytes.Length < 16777216)
                                {
                                    string base64String = Convert.ToBase64String(bytes, 0, bytes.Length);
                                    string ImageUrl = "data:image/png;base64," + base64String;
                                    json2 += ", \"" + ImageUrl + "\"";
                                }
                                else
                                    json2 += ",\"NoImage\"";
                            }
                            else
                            {
                                json2 += ",\"NoImage\"";
                            }
                        }
                    }
                    string ImageData = "{\"ImageUrl\":[" + json2.Remove(0, 1) + "]}";

                    Clients.Client(clientID).productDetails(json, ImageData);
                }
                else
                {
                    Clients.Client(clientID).productDetails("");
                }
            }
            catch (Exception ex)
            {
                Clients.Client(clientID).productDetails("");
            }
        }



        public void getProductFamily(string clientID)
        {
            try
            {
                MySQLBusinessLogic bl = new MySQLBusinessLogic();
                DataTable dt = new DataTable();
                dt = bl.GetProductFamily();
                if (dt.Rows.Count > 0)
                {
                    string json = "";
                    json = ConvertDataTabletoString(dt);
                    Clients.Client(clientID).gotProductFamily(json);
                }
                else
                    Clients.Client(clientID).gotProductFamily("");
            }
            catch (Exception ex)
            {
                Clients.Client(clientID).gotProductFamily("");
            }
        }


        public void getProductTypes(string clientID)
        {
            try
            {
                MySQLBusinessLogic bl = new MySQLBusinessLogic();
                DataTable dt = new DataTable();
                dt = bl.GetProductType();
                if (dt.Rows.Count > 0)
                {
                    string json = "";
                    json = ConvertDataTabletoString(dt);
                    Clients.Client(clientID).gotProductTypes(json);
                }
                else
                    Clients.Client(clientID).gotProductTypes("");
            }
            catch (Exception ex)
            {
                Clients.Client(clientID).gotProductTypes("");
            }
        }

        public void getAllBrands(string clientID)
        {
            try
            {
                MySQLBusinessLogic bl = new MySQLBusinessLogic();
                DataTable dt = new DataTable();
                dt = bl.GetAllBrandsDataTable();
                if (dt.Rows.Count > 0)
                {
                    string json = "";
                    json = ConvertDataTabletoString(dt);
                    Clients.Client(clientID).gotAllBrands(json);
                }
                else
                    Clients.Client(clientID).gotAllBrands("");
            }
            catch (Exception ex)
            {
                Clients.Client(clientID).gotAllBrands("");
            }
        }

        public void changeOptions(string gender, string productFamilyID, string clientID)
        {
            try
            {
                MySQLBusinessLogic bl = new MySQLBusinessLogic();
                DataTable dt = new DataTable();
                dt = bl.GetProductCategory_SubCategory(gender, Convert.ToInt32(productFamilyID));
                List<List<string>> lstCh = new List<List<string>>();
                var tt = (from c in dt.AsEnumerable() select c[0].ToString()).Distinct().ToList();
                string mainStr = "";
                for (int i = 0; i < tt.Count; i++)
                {
                    var tt1 = (from t in dt.AsEnumerable() where t[0].ToString() == tt[i].ToString() select t[1]).ToList();
                    //lstCh.Add(tt1);
                    var jsonSerialiser = new JavaScriptSerializer();
                    string js = jsonSerialiser.Serialize(tt1);
                    mainStr += ",{\"P\":\"" + tt[i].ToString() + "\",\"c\":" + js + "}";
                }
                mainStr = "[" + mainStr.Remove(0, 1) + "]";
                Clients.Client(clientID).changedOptions(mainStr);
            }
            catch (Exception ex)
            {
                Clients.Client(clientID).changedOptions("");
            }
        }

        #endregion

        #region CAPTCHA GENERATION
        public void fillCapctha(string clientID)
        {
            try
            {
                string str22 = GetCaptchaImage(GenerateRandomCode());
                Clients.Client(clientID).showCaptcha(str22.Split('-').FirstOrDefault(), str22.Split('-').Last());
            }
            catch (Exception ex)
            {

            }
        }

        private string GenerateRandomCode()
        {
            string s = "";
            try
            {
                Random _crandom = new Random();
                for (int i = 0; i < 6; i++)
                    s = String.Concat(s, _crandom.Next(10).ToString());
            }
            catch (Exception ex)
            {
            }
            return s;
        }

        private string GetCaptchaImage(string imageText)
        {
            return GenerateCaptchaImage(imageText);
        }

        private byte[] ReadBytes(string fileName)
        {
            byte[] b = new byte[0];
            try
            {
                if (File.Exists(fileName))
                {
                    try
                    {
                        FileStream s = File.OpenRead(fileName);
                        byte[] bytes = new byte[s.Length];
                        s.Read(bytes, (int)0, (int)s.Length);
                        b = bytes;
                    }
                    catch (IOException e)
                    {
                        throw new IOException(e.Message);
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return b;
        }

        private String GenerateCaptchaImage(string imageText)
        {
            try
            {
                Font imgFont;
                int iIterate;
                Bitmap raster;
                DeleteCaptchaImages();
                Graphics graphicsObject;
                System.Drawing.Image imageObject = System.Drawing.Image.FromFile(System.Configuration.ConfigurationManager.AppSettings.GetValues("RootPath").FirstOrDefault().ToString() + @"images\imageback.png");

                // Create the Raster Image Object
                raster = new Bitmap(imageObject);

                //Create the Graphics Object
                graphicsObject = Graphics.FromImage(raster);

                //Instantiate object of brush with black color
                SolidBrush imgBrush = new SolidBrush(Color.White);

                //Add the characters to the image
                for (iIterate = 0; iIterate <= imageText.Length - 1; iIterate++)
                {
                    imgFont = new Font("Arial", 30, FontStyle.Bold);
                    String str = imageText.Substring(iIterate, 1);
                    graphicsObject.DrawString(str, imgFont, imgBrush, iIterate * 40, 15);
                    graphicsObject.Flush();
                }

                // Generate a uniqiue file name to save image as
                String fileName = new Random().Next().ToString() + ".gif";
                if (!(Directory.Exists(System.Configuration.ConfigurationManager.AppSettings.GetValues("RootPath").FirstOrDefault().ToString() + @"captcha")))
                    Directory.CreateDirectory(System.Configuration.ConfigurationManager.AppSettings.GetValues("RootPath").FirstOrDefault().ToString() + @"captcha");
                else
                    raster.Save((System.Configuration.ConfigurationManager.AppSettings.GetValues("RootPath").FirstOrDefault().ToString() + @"captcha\" + fileName), System.Drawing.Imaging.ImageFormat.Gif);

                raster.Dispose();
                graphicsObject = null;

                return fileName + "-" + imageText;
            }
            catch (Exception)
            {
                return "";
            }
        }

        void DeleteCaptchaImages()
        {
            try
            {
                string directoryPath = System.Configuration.ConfigurationManager.AppSettings.GetValues("RootPath").FirstOrDefault().ToString() + @"captcha\";
                Directory.GetFiles(directoryPath).ToList().ForEach(File.Delete);
                // Directory.GetDirectories(directoryPath).ToList().ForEach(Directory.Delete);
            }
            catch (Exception ex)
            {
            }
        }

        #endregion

        public void isLimitReached(string email, string clientID)
        {
            try
            {
                MySQLBusinessLogic bl = new MySQLBusinessLogic();

                DataTable dt = bl.GetStoreID(email);
                int storeID = Convert.ToInt32(dt.Rows[0].ItemArray[0]);



                int result = bl.ProductLimitReached(retailerProductLimit, email, storeID);

                // 1- Limit reached
                // 2- about to reach - nearby
                // 3- not reached limit

                if (result == 1)
                    Clients.Client(clientID).limitReached("1", retailerProductLimit);
                else if (result == 2)
                    Clients.Client(clientID).limitReached("2", retailerProductLimit);
                else if (result == 3)
                    Clients.Client(clientID).limitReached("3", retailerProductLimit);
                else
                    Clients.Client(clientID).limitReached("0", retailerProductLimit);

            }
            catch (Exception ex)
            {
                Clients.Client(clientID).limitReached("0");
            }
        }

    }
}