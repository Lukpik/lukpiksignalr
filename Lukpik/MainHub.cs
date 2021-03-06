﻿using System;
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

            FileStream fs = new FileStream(rootPath + filename, FileMode.Open, FileAccess.Read);
            using (var image1 = Image.FromStream(fs))
            {
                var newWidth = (int)(image1.Width * 0.5);
                var newHeight = (int)(image1.Height * 0.5);
                var thumbnailImg = new Bitmap(newWidth, newHeight);
                var thumbGraph = Graphics.FromImage(thumbnailImg);
                thumbGraph.CompositingQuality = CompositingQuality.HighQuality;
                thumbGraph.SmoothingMode = SmoothingMode.HighQuality;
                thumbGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                var imageRectangle = new Rectangle(0, 0, newWidth, newHeight);
                thumbGraph.DrawImage(image1, imageRectangle);
                thumbnailImg.Save(rootPath + "New" + filename, image1.RawFormat);
                FileStream fs2 = new FileStream(rootPath + "New" + filename, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs2);
                ImageData = br.ReadBytes((int)fs2.Length);
                br.Close();
                fs2.Close();
            }
            fs.Close();
            File.Delete(rootPath + "New" + filename);
            //FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read);
            //BinaryReader br = new BinaryReader(fs);
            //ImageData = br.ReadBytes((int)fs.Length);
            //br.Close();
            //fs.Close();
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
            int length = 6;//rnd.Next(6, 12); // creates a number between 1 and 12
            string generatedPassword = CreatePassword(length);

            //Master password
            string masterPassword = CreatePassword(length);// Actual master password
            //string masterPwdEncrypted = Encrypt(masterPassword);// Encrypted master password

            string password = Encrypt(generatedPassword);
            MySQLBusinessLogic bl = new MySQLBusinessLogic();
            bool isEmailNull = false;
            if (email == "")
            {
                //string store = storename.Trim().Replace(" ", "");
                //email = store + "." + phonenum + "@lukpik.com";
                isEmailNull = true;
            }
            int result = bl.AddStore(storename, city, phonenum, phonenum2, DateTime.Now, DateTime.Now, email, fname, lname, 0, 0, password, 1, 0, 1, masterPassword);
            if (result == 1)
            {
                Clients.Client(clientID).testJS("1");
                string loginRedirect = System.Configuration.ConfigurationManager.AppSettings.GetValues("LoginRedirect").FirstOrDefault().ToString();
                //string body = "Dear " + fname + ",<br> Thanks for Registering with us. Your credentials to proceed with us is as follows.<br>Username: " + email + "<br>Password: " + generatedPassword + "<br><a href='" + loginRedirect + "' target='_blank'>Click here to login</a>";

                string body1 = "<html><head> <meta http-equiv='Content-Type' content='text/html; charset=utf-8'/> <meta name='viewport' content='width=device-width, initial-scale=1.0'/> <title>Lukpik</title> <style type='text/css'> table{border-collapse: collapse;}td{font-family: Calibri; color: #333333;}@media only screen and (max-width: 480px){body, table, td, p, a, li, blockquote{-webkit-text-size-adjust: none !important;}table{width: 100% !important;}.responsive-image img{height: auto !important; max-width: 100% !important; width: 100% !important;}}</style></head><body style='margin: 10px 0; padding: 0 10px; background: #F5F7FA; font-size: 13px;'> <table border='0' cellpadding='0' cellspacing='0' width='100%'> <tr> <td> <table border='0' cellpadding='0' cellspacing='0' align='center' width='640' bgcolor='#FFFFFF'> <tr> <td style='font-size: 0; line-height: 0; padding: 0;' align='center' class='responsive-image'> <img src='http://www.lukpik.com/img/emailicons/retailermailbanner.png' width='100%' alt=''/> </td></tr><tr><td style='font-size: 0; line-height: 0;' height='10'>&nbsp;</td></tr><tr> <td style='padding: 10px 20px 20px 20px;'> <div style='font-size: 25px;color:rgb(42, 177, 237);text-align:center;font-weight:bold;'>Happy to have you on board !</div><br/> <div style='font-size: 20px;color:rgb(91, 90, 90);text-align:center;'> Get ready to wipe the boot stains of your store floor more often. </div><br/> <br/> <div style='font-family:Calibri; font-size: 16px;color:rgb(91, 90, 90);text-align:justify;'> Please login to lukpik and fill in details about your store. In next few minutes you can start uploading products you want to sell. </div><br/> <div style='font-family:Calibri; font-size: 16px;color:rgb(91, 90, 90);text-align:justify;'>";

                string adminbody = " User login credentials:<br/><br/> <b>Username : </b>" + phonenum + "<br/> <b>Password : </b>" + generatedPassword + "<br/><br/>";

                string userbody = "Your credentials has been sent to your mobile number ending with XXXXXXX" + phonenum.Substring(7) + ".<br/><br/>";

                string body2 = " </div><br/> <div style='font-family:Calibri; font-size: 16px;color:rgb(91, 90, 90);text-align:center;'> <a type='button' style='-webkit-border-radius: 0; -moz-border-radius: 0; border-radius: 0px; color: #ffffff; font-size: 20px; background: #22c9ad; padding: 10px 20px 10px 20px; text-decoration: none;' target='_blank' href='" + loginRedirect + "'>Click here to login</a> </div><br/> <br/> <div style='font-family:Calibri; font-size: 16px;color:rgb(91, 90, 90);text-align:justify;'> For any further assistance. Just drop us a mail at <a href='mailto:support@lukpik.com' target='_top'>support@lukpik.com</a>. We will get back to you as soon as we can. <br/><br/> We'll be in touch periodically with additional resources and important updates. <br/><br/>Sincerely,<br/><b>Lukpik Team</b><br/> </div></td></tr><tr> <td></td></tr><tr><td style='font-size: 0; line-height: 0;' height='20'>&nbsp;</td></tr><tr> <td bgcolor='#485465'> <table border='0' cellpadding='0' cellspacing='0' width='100%'> <tr><td style='font-size: 0; line-height: 0;' height='15'>&nbsp;</td></tr><tr> <td style='padding: 0 10px; color: #FFFFFF;'> <table border='0' width='100%'> <tr> <td style='color:white;' width='20%'> <a href='http://www.lukpik.com' target='_blank' style='font-size:15px;color:white;'>Lukpik</a> </td><td style='color:white;' width='80%' align='right'> <div style='text-align:right;'> <a href='mailto:support@lukpik.com' style='text-decoration:none;'> <img src='http://www.lukpik.com/img/emailicons/email.png' width='35' style='margin-right:5px;'/> </a> <a href='http://www.facebook.com/Itslukpik' target='_blank' style='text-decoration:none;'> <img src='http://www.lukpik.com/img/emailicons/facbk.png' width='35' style='margin-right:5px;'/> </a> <a href='https://twitter.com/Its_lukpik' target='_blank' style='text-decoration:none;'> <img src='http://www.lukpik.com/img/emailicons/twitter.png' width='35' style='margin-right:5px;'/> </a> <a href='http://www.lukpik.com' target='_blank' style='text-decoration:none;'> <img src='http://www.lukpik.com/img/emailicons/website.png' width='35' style='margin-right:5px;'/> </a> </div></td></tr></table> </td></tr><tr><td style='font-size: 0; line-height: 0;' height='15'>&nbsp;</td></tr></table> </td></tr></table> </td></tr></table></body></html>";

                string subject = storename + "- Welcome to Lukpik.";

                string userSMSText = "Welcome to Lukpik. Your credentials are as follows:\nUsername : " + phonenum + "\nPassword : " + generatedPassword;
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
            const string valid = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
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

        public void setStoreMasterPassword(string phone, string masterPwd, string clientID)
        {
            try
            {
                int storeID = GetStoreIDbyPhone(phone);
                MySQLBusinessLogic bl = new MySQLBusinessLogic();
                //masterPwd = Encrypt(masterPwd);//Encryption
                if (bl.SetStoreMasterPassword(phone, storeID, masterPwd))
                {
                    Clients.Client(clientID).setMasterAck("1");
                }
                else
                    Clients.Client(clientID).setMasterAck("0");
            }
            catch (Exception ex)
            {
                Clients.Client(clientID).setMasterAck("0");
            }
        }

        public void getStoreMasterPassword(string phone, string clientID)
        {
            try
            {
                MySQLBusinessLogic bl = new MySQLBusinessLogic();
                int storeID = GetStoreIDbyPhone(phone);
                string storeMasterPwd = bl.GetStoremasterPassword(storeID);
                if (storeMasterPwd != "" && storeMasterPwd != null)
                {
                    //storeMasterPwd = Decrypt(storeMasterPwd);//Decryption
                    Clients.Client(clientID).setMasterAck("1", storeMasterPwd);
                }
                else
                    Clients.Client(clientID).setMasterAck("0");
            }
            catch (Exception ex)
            {
                Clients.Client(clientID).setMasterAck("0");
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
                string folderName = System.Configuration.ConfigurationManager.AppSettings.GetValues("RootPath").FirstOrDefault().ToString();

                bool exists = System.IO.Directory.Exists(folderName + storeID);
                string pathString = "";
                if (!exists)
                {
                    pathString = System.IO.Path.Combine(folderName, storeID.ToString());
                    System.IO.Directory.CreateDirectory(pathString);
                    pathString += "\\";
                }
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
                //string st = Decrypt(storeDetails.Rows[0].ItemArray[storeDetails.Columns.Count - 1].ToString());
                //storeDetails.Rows[0]["StoreMasterPassword"] = st;
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
                        string midText = "Dear " + name + ",<br> Your password was changed recently . The new password was sent to your phone number ending with XXXXXXX" + phone.Substring(7) + "<br>";
                        string body = AttachTexttoMailTemplate(midText, loginRedirect, "You Have Changed Your Password !", false);
                        string subject = "Lukpik - Password Changed ";
                        SendSMS("Hi " + name + ", Your password has been changed successfully. \nNew password : " + newpwd, phone);
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
                string username = System.Configuration.ConfigurationManager.AppSettings.GetValues("SMSTYPE_USERNAME").First().ToString();
                string password = System.Configuration.ConfigurationManager.AppSettings.GetValues("SMSTYPE_PASSWORD").First().ToString();
                string senderID = System.Configuration.ConfigurationManager.AppSettings.GetValues("SMSTYPE_SENDERID").First().ToString();
                //string username = "demo7788";
                //string password = "demo7788";

                string sUrl = "http://sms99.co.in/pushsms.php?username=" + username + "&password=" + password + "&sender=" + senderID + "&message=" + messageContent + "&numbers=" + mobilenumber + "";
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

                    string userText = "Dear " + fname + ",<br> Your password has been sent to your registered mobile number ending with XXXXXXX" + phone.Substring(7) + ".<br>";

                    string adminText = "Username of:  " + fname + ",<br>  have requested for password. Please find the password below<br>Username: " + phone + "<br>Password: " + pwd + "<br>";

                    string subject = "Lukpik - Forgot Password.";
                    Clients.Client(clientID).gotPassword("1");
                    //user body
                    string body = AttachTexttoMailTemplate(userText, loginRedirect, "Here is your password !", false);

                    //admin body
                    string adminBody = AttachTexttoMailTemplate(userText, loginRedirect, "Here is your password !", false);

                    if (email != "" || email != null)
                        SendEMail("support@lukpik.com", email, subject, body);
                    SendEMail("support@lukpik.com", "lukpik.store@gmail.com", "User :" + phone + ": Requested for password", adminBody);

                    //SMS section
                    string userSMSText = "Hi " + fname + ", Your password is  : " + pwd;
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
                string midText = "Dear " + fname + ",<br> Thank you for geting in touch. We will get back to you shortly.";
                string subject = "Lukpik - Thank You For Contacting Us ";
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

        public void addProduct(string productname, string gender, string productFamilyId, string productdescription, string price, string quantity, string size, string color, string visibility, string productCat_Sub_ID, string brandID, string collection, string images, string email, string clientID, string fileNames1, string ecommecelink, string productSKU, string masterPwd)
        {
            try
            {
                MySQLBusinessLogic bl = new MySQLBusinessLogic();
                DataTable dt = bl.GetStoreID(email);
                int storeID = Convert.ToInt32(dt.Rows[0].ItemArray[0]);
                int productCategoryID = Convert.ToInt32(productCat_Sub_ID.Split('_')[1]);
                int productSubCategoryID = Convert.ToInt32(productCat_Sub_ID.Split('_')[0]);
                List<string> lstImgID = new List<string>();
                List<string> lstImgPath = new List<string>();
                List<string> lstImgOrgPath = new List<string>();
                var fileNames = fileNames1.Split(',');
                var fileimages = images.Split(',');
                byte[] PrvImg = null;
                List<byte[]> lstByte = new List<byte[]>();
                string rootPath = System.Configuration.ConfigurationManager.AppSettings.GetValues("RootPath").First().ToString();
                string rp = rootPath;
                rootPath += storeID.ToString() + "\\";

                for (int i = 0; i < fileNames.Length; i++)
                {

                    string fileName = fileNames[i];
                    string encodedFileName = fileimages[i];
                    if (fileName != encodedFileName)
                    {
                        fileName = Regex.Replace(fileName.Substring(0, fileName.LastIndexOf('.')), "[.;]", "_") + fileName.Substring(fileName.LastIndexOf('.'), (fileName.Length - fileName.LastIndexOf('.')));
                        if (File.Exists(rp + fileName))
                            File.Delete(rp + fileName);
                        System.IO.File.Move(rp + encodedFileName, rootPath + fileName);
                        //if (i == 0)
                        //{
                           // Image image = Image.FromFile(rootPath + fileName);
                           // Image thumb = image.GetThumbnailImage(100, 100, () => false, IntPtr.Zero);
                           // thumb.Save(Path.ChangeExtension(rootPath + fileName.Split('.')[0], "thumb"));
                           // System.IO.File.Move(rootPath + fileName.Split('.')[0] + ".thumb", rootPath + "Thumb" + fileName);
                           // FileStream fs1 = new FileStream(rootPath + "Thumb" + fileName, FileMode.Open, FileAccess.Read);
                           // BinaryReader br1 = new BinaryReader(fs1);
                           // PrvImg = br1.ReadBytes((int)fs1.Length);
                           // br1.Close();
                           // fs1.Close();
                           //// File.Delete(rootPath + "Thumb" + fileName);
                        //}

                    }
                    //test image save code
                    string FileName = rootPath + fileName;
                    byte[] ImageData;
                    FileStream fs = new FileStream(rootPath + fileName, FileMode.Open, FileAccess.Read);
                    using (var image1 = Image.FromStream(fs))
                    {
                        var newWidth = (int)(image1.Width * 0.9);
                        var newHeight = (int)(image1.Height * 0.9);
                        var thumbnailImg = new Bitmap(newWidth, newHeight);
                        var thumbGraph = Graphics.FromImage(thumbnailImg);
                        thumbGraph.CompositingQuality = CompositingQuality.HighQuality;
                        thumbGraph.SmoothingMode = SmoothingMode.HighQuality;
                        thumbGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        var imageRectangle = new Rectangle(0, 0, newWidth, newHeight);
                        thumbGraph.DrawImage(image1, imageRectangle);
                        thumbnailImg.Save(rootPath + "New" + fileName, image1.RawFormat);
                        FileStream fs2 = new FileStream(rootPath + "New" + fileName, FileMode.Open, FileAccess.Read);
                        BinaryReader br = new BinaryReader(fs2);
                        ImageData = br.ReadBytes((int)fs2.Length);
                        br.Close();
                        fs2.Close();
                    }
                    fs.Close();
                    //Image image1 = Image.FromFile(rootPath + fileName);
                    //Image thumb1 = image1.GetThumbnailImage(image1.Width, image1.Height, () => false, IntPtr.Zero);
                    //thumb1.Save(Path.ChangeExtension(rootPath + fileName.Split('.')[0], "thumb"));
                    //System.IO.File.Move(rootPath + fileName.Split('.')[0] + ".thumb", rootPath + "New" + fileName);
                    //FileStream fs = new FileStream(rootPath + "New" + fileName, FileMode.Open, FileAccess.Read);
                    //BinaryReader br = new BinaryReader(fs);
                    //ImageData = br.ReadBytes((int)fs.Length);
                    //br.Close();
                    //fs.Close();
                    //File.Delete(rootPath + "New" + fileName);
                    lstImgPath.Add(rootPath + "New" + fileName);
                    lstImgOrgPath.Add(rootPath + fileName);
                    lstImgID.Add("image" + (i + 1));
                    //File.Delete(rootPath + fileName);
                    if (ImageData.Length < 16777216)
                        lstByte.Add(ImageData);

                }

                
                //int brandID = 0;
                //brandID = bl.GetBrandNameID(brand);
                //if (brandID == 0)
                //{
                //    brandID = bl.InsertBrandandGetID(brand);
                //}

                
                int retVal = bl.AddProduct(productname, gender, Convert.ToInt32(productFamilyId), productdescription, Convert.ToDouble(price), quantity, size, color, Convert.ToInt32(visibility), productCategoryID, productSubCategoryID, Convert.ToInt32(brandID), collection, images, storeID, DateTime.Now, email, lstByte, PrvImg, ecommecelink, productSKU, masterPwd,lstImgID,lstImgPath,lstImgOrgPath);
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
        public List<byte[]> ImageUpload(string images, string fileNames1)
        {
            List<byte[]> lstByte = new List<byte[]>();
            try
            {
                var fileNames = fileNames1.Split(',');
                var fileimages = images.Split(',');
                byte[] PrvImg = null;

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
            }
            catch (Exception ex)
            {
            }
            return lstByte;
        }
        public void updateProduct(string productID, string productname, string gender, string productFamilyId, string productdescription, string price, string quantity, string size, string color, string visibility, string productCat_Sub_ID, string brandID, string collection, string images, string email, string clientID, string fileNames1, string ecommecelink, string imgIDs, string productSKU, string masterPwd)
        {
            try
            {
                int productCategoryID = Convert.ToInt32(productCat_Sub_ID.Split('_')[1]);
                int productSubCategoryID = Convert.ToInt32(productCat_Sub_ID.Split('_')[0]);
                int pro_ID = 0;
                if (productID != "")
                    pro_ID = Convert.ToInt32(productID);
                MySQLBusinessLogic bl = new MySQLBusinessLogic();
                List<string> lstImgID = new List<string>();
                List<string> lstImgPath = new List<string>();
                List<string> lstImgOrgPath = new List<string>();
                DataTable dt = bl.GetStoreID(email);
                int storeID = Convert.ToInt32(dt.Rows[0].ItemArray[0]);
                int result = bl.UpdateProduct(pro_ID, productname, gender, Convert.ToInt32(productFamilyId), productdescription, Convert.ToDouble(price), quantity, size, color, Convert.ToInt32(visibility), productCategoryID, productSubCategoryID, Convert.ToInt32(brandID), collection, storeID, DateTime.Now, email, ecommecelink, productSKU, masterPwd);
                if (fileNames1 != "")
                {
                    //List<byte[]> lstb = new List<byte[]>();
                    //lstb = ImageUpload(images, fileNames1);

                    var fileNames = fileNames1.Split(',');
                    var fileimages = images.Split(',');
                    byte[] PrvImg = null;
                    List<byte[]> lstByte = new List<byte[]>();
                    string rootPath = System.Configuration.ConfigurationManager.AppSettings.GetValues("RootPath").First().ToString();
                    string rp = rootPath;
                    rootPath += storeID.ToString() + "\\";
                    var imageIDs = imgIDs.Split(',');
                    for (int i = 0; i < fileNames.Length; i++)
                    {

                        string fileName = fileNames[i];
                        string encodedFileName = fileimages[i];
                        if (fileName != encodedFileName)
                        {
                            fileName = Regex.Replace(fileName.Substring(0, fileName.LastIndexOf('.')), "[.;]", "_") + fileName.Substring(fileName.LastIndexOf('.'), (fileName.Length - fileName.LastIndexOf('.')));
                            //if (File.Exists(rootPath + fileName))
                            //    File.Delete(rootPath + fileName);
                            //System.IO.File.Move(rootPath + encodedFileName, rootPath + fileName);
                            if (File.Exists(rp + fileName))
                                File.Delete(rp + fileName);
                            System.IO.File.Move(rp + encodedFileName, rootPath + fileName);

                            //if (imageIDs.Contains("image1"))
                            //{

                            //    Image image = Image.FromFile(rootPath + fileName);
                            //    Image thumb = image.GetThumbnailImage(100, 100, () => false, IntPtr.Zero);
                            //    thumb.Save(Path.ChangeExtension(rootPath + fileName.Split('.')[0], "thumb"));
                            //    System.IO.File.Move(rootPath + fileName.Split('.')[0] + ".thumb", rootPath + "New" + fileName);
                            //    FileStream fs1 = new FileStream(rootPath + "New" + fileName, FileMode.Open, FileAccess.Read);
                            //    BinaryReader br1 = new BinaryReader(fs1);
                            //    PrvImg = br1.ReadBytes((int)fs1.Length);
                            //    br1.Close();
                            //    fs1.Close();
                            //    //File.Delete(rootPath + "New" + fileName);
                            //}

                        }
                        //test image save code
                        string FileName = rootPath + fileName;
                        byte[] ImageData;
                        FileStream fs = new FileStream(rootPath + fileName, FileMode.Open, FileAccess.Read);
                        using (var image1 = Image.FromStream(fs))
                        {
                            var newWidth = (int)(image1.Width * 0.9);
                            var newHeight = (int)(image1.Height * 0.9);
                            var thumbnailImg = new Bitmap(newWidth, newHeight);
                            var thumbGraph = Graphics.FromImage(thumbnailImg);
                            thumbGraph.CompositingQuality = CompositingQuality.HighQuality;
                            thumbGraph.SmoothingMode = SmoothingMode.HighQuality;
                            thumbGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            var imageRectangle = new Rectangle(0, 0, newWidth, newHeight);
                            thumbGraph.DrawImage(image1, imageRectangle);
                            thumbnailImg.Save(rootPath + "New" + fileName, image1.RawFormat);
                            FileStream fs2 = new FileStream(rootPath + "New" + fileName, FileMode.Open, FileAccess.Read);
                            BinaryReader br = new BinaryReader(fs2);
                            ImageData = br.ReadBytes((int)fs2.Length);
                            br.Close();
                            fs2.Close();
                        }
                        fs.Close();
                        //Image image1 = Image.FromFile(rootPath + fileName);
                        //Image thumb1 = image1.GetThumbnailImage(image1.Width, image1.Height, () => false, IntPtr.Zero);
                        //thumb1.Save(Path.ChangeExtension(rootPath + fileName.Split('.')[0], "thumb"));
                        //System.IO.File.Move(rootPath + fileName.Split('.')[0] + ".thumb", rootPath + "New" + fileName);
                        //FileStream fs = new FileStream(rootPath + "New" + fileName, FileMode.Open, FileAccess.Read);
                        //BinaryReader br = new BinaryReader(fs);
                        //ImageData = br.ReadBytes((int)fs.Length);
                        //br.Close();
                        //fs.Close();
                        lstImgPath.Add(rootPath + "New" + fileName);
                        lstImgOrgPath.Add(rootPath + fileName);
                        lstImgID.Add(imageIDs[i]);
                        //File.Delete(rootPath + "New" + fileName);
                        //File.Delete(rootPath + fileName);
                        if (ImageData.Length < 16777216)
                            lstByte.Add(ImageData);
                    }

                    if (lstByte.Count > 0)
                    {
                        DataTable dtImg = new DataTable();
                        dtImg = bl.GetProductImagePaths(pro_ID,storeID);
                        for (int i = 0; i < lstByte.Count; i++)
                        {
                            bool IsUpdate = false;
                            for (int k = 0; k < dtImg.Rows.Count; k++)
                            {
                                if (imageIDs[i] == dtImg.Rows[k].ItemArray[0].ToString()) {
                                    if (File.Exists(dtImg.Rows[k].ItemArray[2].ToString()))
                                        File.Delete(dtImg.Rows[k].ItemArray[2].ToString());
                                    if (File.Exists(dtImg.Rows[k].ItemArray[1].ToString()))
                                        File.Delete(dtImg.Rows[k].ItemArray[1].ToString());
                                    IsUpdate = true;
                                }
                            }
                            bl.UpdateProducts_Image(lstByte[i], pro_ID, imageIDs[i], PrvImg,lstImgPath[i],lstImgOrgPath[i],IsUpdate);
                        }
                    }
                }
                if (size != "" || color != "" || collection != "")
                {
                    if (bl.DeleteProductSpectication(pro_ID))
                    {
                        bl.AddSpecification(email, color, size, collection);
                    }
                }
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
                        DataTable dt = new DataTable();
                        dt = bl.GetProductImagePaths(pro_id, 0);
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            if (File.Exists(dt.Rows[i].ItemArray[2].ToString()))
                                File.Delete(dt.Rows[i].ItemArray[2].ToString());
                            if (File.Exists(dt.Rows[i].ItemArray[1].ToString()))
                                File.Delete(dt.Rows[i].ItemArray[1].ToString());
                        }
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
        public void getProductDetails(string email, string productID, string isDuplicate, string clientID)
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
                DataTable dtImgPaths = new DataTable();
                //if (prod_ID != 0)
                string json2 = "";
                string ImageData = "";
                if (isDuplicate == "false")
                {
                    //dtImage = bl.GetProductImageDetails(storeID, prod_ID);
                    dtImgPaths = bl.GetProductImagePaths(prod_ID,storeID);
                }
                string js3 = "";
                if (dt.Rows.Count > 0)
                {
                    string json = ConvertDataTabletoString(dt);

                    //
                    if (prod_ID != 0)
                    {
                        for (int k = 0; k < dtImgPaths.Rows.Count; k++)
                        {
                            js3 += ",\"" + dtImgPaths.Rows[k].ItemArray[1].ToString().Split('\\')[dtImgPaths.Rows[k].ItemArray[1].ToString().Split('\\').Length - 2] + "/" + dtImgPaths.Rows[k].ItemArray[1].ToString().Split('\\').Last() + "\"";
                            //js3 += ",\"" + dtImgPaths.Rows[k].ItemArray[1].ToString().Replace('\\', '/') + "\"";
                        }
                    }
                    else
                    {
                        for (int k = 0; k < dtImgPaths.Rows.Count; k++)
                        {
                            json2 += ",\"" + dtImgPaths.Rows[k].ItemArray[1].ToString().Split('\\')[dtImgPaths.Rows[k].ItemArray[1].ToString().Split('\\').Length - 2] + "/" + dtImgPaths.Rows[k].ItemArray[1].ToString().Split('\\').Last() + "\"";
                            //json2 += ",\"" + dtImgPaths.Rows[k].ItemArray[1].ToString().Replace('\\','/')+ "\"";
                        }
                    }
                        

                    //
                    if (dtImage.Rows.Count > 0)
                    {
                        if (prod_ID != 0)
                        {
                            //for (int i = 0; i < dtImage.Columns.Count; i++)
                            //{
                            //    if (dtImage.Rows[0].ItemArray[i].GetType().Name != "DBNull")
                            //    {
                            //        byte[] bytes = (Byte[])dtImage.Rows[0].ItemArray[i];
                            //        if (bytes.Length < 16777216)
                            //        {
                            //            string base64String = Convert.ToBase64String(bytes, 0, bytes.Length);
                            //            string ImageUrl = "data:image/png;base64," + base64String;
                            //            json2 += ", \"" + ImageUrl + "\"";

                            //        }
                            //        else
                            //            json2 += ",\"NoImage\"";
                            //    }
                            //    else
                            //    {
                            //        json2 += ",\"NoImage\"";
                            //    }
                            //}
                        }
                        else
                        {
                            //for (int i = 0; i < dtImage.Rows.Count; i++)
                            //{
                            //    if (dtImage.Rows[i].ItemArray[0].GetType().Name != "DBNull")
                            //    {
                            //        byte[] bytes = (Byte[])dtImage.Rows[i].ItemArray[0];
                            //        if (bytes.Length < 16777216)
                            //        {
                            //            string base64String = Convert.ToBase64String(bytes, 0, bytes.Length);
                            //            string ImageUrl = "data:image/png;base64," + base64String;
                            //            json2 += ", \"" + ImageUrl + "\"";
                            //        }
                            //        else
                            //            json2 += ",\"NoImage\"";
                            //    }
                            //    else
                            //    {
                            //        json2 += ",\"NoImage\"";
                            //    }
                            //}
                        }
                    }
                    if (isDuplicate == "false"&&json2!="")
                    {
                        ImageData = "{\"ImageUrl\":[" + json2.Remove(0, 1) + "]}";
                    }
                    else
                    {
                        ImageData = "";
                    }
                    string ImgDataPath = "";
                    if(js3!=null&&js3!="")
                        ImgDataPath = "{\"ImageUrl\":[" + js3.Remove(0, 1) + "]}";

                    Clients.Client(clientID).productDetails(json, ImageData, ImgDataPath);
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

        public void getCollectionColorSizes(string productID, string phone, string clientID)
        {
            try
            {
                // 1 - color
                // 2 - size
                // 3 - tag
                // 4 - brand
                if (productID != "")
                {
                    MySQLBusinessLogic bl = new MySQLBusinessLogic();
                    List<string> lstColor = bl.GetProductrelatedSpecifications(Convert.ToInt32(productID), 1);
                    List<string> lstSize = bl.GetProductrelatedSpecifications(Convert.ToInt32(productID), 2);
                    List<string> lstTag = bl.GetProductrelatedSpecifications(Convert.ToInt32(productID), 3);
                    string colors = string.Join(",", lstColor);
                    string tags = string.Join(",", lstTag);
                    string sizes = string.Join(",", lstSize);
                    var jsonSerialiser = new JavaScriptSerializer();



                    //var jsonSerialiser = new JavaScriptSerializer();
                    //json2 = jsonSerialiser.Serialize(lstSelected);
                    Clients.Client(clientID).fillCollectionColorSizes("1", colors, tags, sizes);
                }
            }
            catch (Exception ex)
            {
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

        public void isLimitReached(string phone, string clientID)
        {
            try
            {
                MySQLBusinessLogic bl = new MySQLBusinessLogic();

                DataTable dt = bl.GetStoreID(phone);
                int storeID = Convert.ToInt32(dt.Rows[0].ItemArray[0]);



                int result = bl.ProductLimitReached(retailerProductLimit, phone, storeID);

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
                //LogFile(ex.Message, ex.ToString(), email);
            }
        }

        public void LogFile(string sExceptionName, string sEventName, int nErrorLineNo, string username, string uploadedfile, string file)
        {
            StreamWriter log;
            try
            {
                string path = System.Configuration.ConfigurationManager.AppSettings.GetValues("RootPath").FirstOrDefault().ToString();
                if (!File.Exists(path + "\\logfile.csv"))
                {
                    log = new StreamWriter(path + "\\logfile.csv");
                }
                else
                {
                    log = File.AppendText(path + "\\logfile.csv");
                }

                // Write to the file:
                log.WriteLine("\"DataTime:" + DateTime.Now + "\",\"UserID:" + username + "\",\"UploadedFile:" + uploadedfile.Split('\\').Last() + "\",\"ExceptionName:" + sExceptionName + "\",\"FileName:" + file + "\",\"Error Line No.:" + nErrorLineNo + "\"");//"|Event Name:" + sEventName +

                log.Close();
            }
            catch (Exception ex)
            { }
        }

        public void getBrandImagesfromFolder(string clientID)
        {
            try
            {
                string brandImagespath = System.Configuration.ConfigurationManager.AppSettings.GetValues("BrandImages").First().ToString();
                String searchFolder = @brandImagespath;
                var filters = new String[] { "jpg", "jpeg", "png", "gif", "tiff", "bmp" };
                var files = GetFilesFrom(searchFolder, filters, false);
                string html = "";
                for (int i = 0; i < files.Length; i++)
                {
                    string imgSrc = files[i].ToString();
                    imgSrc = imgSrc.Replace("\\", "/");

                    string path = "../img/" + imgSrc.Split(new string[] { "img/" }, StringSplitOptions.None)[1];
                    html = "<div class='col-md-2 imgPrevHover'><img src='" + path + "' class='imgHeight' /></div>";
                    Clients.Client(clientID).gotBrands("1", html);
                }
                html += "";
                
            }
            catch (Exception ex)
            {
                Clients.Client(clientID).gotBrands("0", "");
            }
        }

        public static String[] GetFilesFrom(String searchFolder, String[] filters, bool isRecursive)
        {
            List<String> filesFound = new List<String>();
            var searchOption = isRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            foreach (var filter in filters)
            {
                filesFound.AddRange(Directory.GetFiles(searchFolder, String.Format("*.{0}", filter), searchOption));
            }
            return filesFound.ToArray();
        }

        #region USER SECTION

        public void getAllStoreDetails(string clientID, string categoryIDs,string storeType)
        {
            try
            {
                MySQLBusinessLogic bl = new MySQLBusinessLogic();
                DataTable dt = new DataTable();
                if (categoryIDs == "" && storeType=="")
                    dt = bl.GetAllStores();
                else
                {
                    //get list of Store ID's from category id
                    List<string> lst = new List<string>();
                    string storeIDS = "";
                    if (categoryIDs != "")
                    {
                        lst = bl.GetStoresbyCategoryID(categoryIDs);
                        storeIDS = String.Join(",", lst);
                    }
                    //return all stores 
                    //if (lst.Count > 0)
                        dt = bl.GetStoresByStoreID(storeIDS, storeType);
                    //else
                    //    Clients.Client(clientID).gotStores("0");

                }
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataTable temp = new DataTable();
                        for (int c = 0; c < dt.Columns.Count; c++)
                        {
                            DataColumn dc = new DataColumn();
                            dc.ColumnName = dt.Columns[c].ToString();
                            if (dc.ColumnName == "StoreImage")
                            {
                                dc.DataType = typeof(string);
                            }
                            else
                                dc.DataType = dt.Columns[c].DataType;
                            temp.Columns.Add(dc);
                        }
                        //temp.Rows[i].ItemArray = dt.Rows[i].ItemArray;
                        temp.ImportRow(dt.Rows[i]);
                        string ImageUrl = "NoImage";
                        if (dt.Rows[i]["StoreImage"].GetType().Name != "DBNull")
                        {
                            byte[] bytes = (Byte[])dt.Rows[i]["StoreImage"];
                            string base64String = Convert.ToBase64String(bytes, 0, bytes.Length);
                            ImageUrl = "data:image/png;base64," + base64String;
                        }
                        temp.Rows[0]["StoreImage"] = ImageUrl;
                        string json = ConvertDataTabletoString(temp);
                        Clients.Client(clientID).gotStores("1", json);
                    }
                }
                else
                {
                    Clients.Client(clientID).gotStores("0");
                }

            }
            catch (Exception ex)
            {
            }
        }

        
        public void getProductsbyStore(string storeID,string clientID,string frm)
        {
            try
            {
                //0 - exception
                //1 - success
                //2 - if no storeID detected - PageNotFound
                //3- no stores detected
                MySQLBusinessLogic bl = new MySQLBusinessLogic();
                DataTable dt = new DataTable();
                int sID;
                if (storeID == null || storeID == "")
                    Clients.Client(clientID).storeProducts("2");
                else
                {


                    if (frm == "1")
                    {
                        sID = Convert.ToInt32(storeID);
                        dt = bl.GetProductsbyStores(sID);
                    }
                    else if (frm == "2")
                    {
                        dt = bl.GetAllProducts();
                    }
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            DataTable temp = new DataTable();
                            for (int c = 0; c < dt.Columns.Count; c++)
                            {
                                DataColumn dc = new DataColumn();
                                dc.ColumnName = dt.Columns[c].ToString();
                                if (dc.ColumnName == "ProductImage")
                                {
                                    dc.DataType = typeof(string);
                                }
                                else
                                    dc.DataType = dt.Columns[c].DataType;
                                temp.Columns.Add(dc);
                            }
                            //temp.Rows[i].ItemArray = dt.Rows[i].ItemArray;
                            temp.ImportRow(dt.Rows[i]);
                            string ImageUrl = "NoImage";
                            if (dt.Rows[i]["ProductImage"].GetType().Name != "DBNull")
                            {
                                byte[] bytes = (Byte[])dt.Rows[i]["ProductImage"];
                                string base64String = Convert.ToBase64String(bytes, 0, bytes.Length);
                                ImageUrl = "data:image/png;base64," + base64String;
                            }
                            temp.Rows[0]["ProductImage"] = ImageUrl;
                            string json = ConvertDataTabletoString(temp);
                            Clients.Client(clientID).storeProducts("1", json);
                        }
                    }
                    else
                    {
                        Clients.Client(clientID).storeProducts("3");
                    }



                }

            }
            catch (Exception ex)
            {
                Clients.Client(clientID).storeProducts("0");
            }
        }

        public void getAllCategories(string clientID,string storetype)
        {
            try
            {
                string json1 = "";//list of all categories
                
                DataTable dtAllCategories = new DataTable();
                MySQLBusinessLogic bl = new MySQLBusinessLogic();
                dtAllCategories = bl.GetOtherCategorieswithCount(storetype);
                if (dtAllCategories.Rows.Count > 0)
                    json1 = ConvertDataTabletoString(dtAllCategories);

                Clients.Client(clientID).gotAllCategories(json1);
            }
            catch (Exception ex)
            {
            }
        }

        #region FILTER SECTION
        public void filterbyCategories(string clientID,string categoryID,string storeType,string distance)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
            }
        }
        #endregion

        #endregion

    }
}