using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace  Lukpik.retailers.FileUploader
{
    /// <summary>
    /// Summary description for FileUploader
    /// </summary>
    public class FileUploader : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string fileName = DateTime.Now.Ticks.ToString();
            try
            {
                //fileName = HttpContext.Current.Request.QueryString.ToString().Split(new string[] { "FileName=" }, StringSplitOptions.None)[1];
                //fileName = HttpContext.Current.Request.QueryString["FileName"].ToString().Replace("([@-@-@])", "&");
               // fileName = DateTime.Now.Ticks.ToString() + "_" + HttpContext.Current.Request.QueryString["UserID"].ToString() + "_" + HttpContext.Current.Request.QueryString["SessionID"].ToString();
                string path = System.Configuration.ConfigurationManager.AppSettings.GetValues("RootPath").First().ToString();
                
                //if (HttpContext.Current.Request.QueryString["UserID"].ToString().Trim() == "0")
                //    path = System.Configuration.ConfigurationManager.AppSettings.GetValues("RootPath").First().ToString() + "Users\\" + HttpContext.Current.Request.QueryString["SessionID"].ToString().Trim() + "\\";
                fileName = fileName.Length > 64 ? (fileName.Substring(0, 50) + "." + fileName.Split('.').Last()) : fileName;
                //var encoding = System.Text.Encoding.GetEncoding("windows - 1252");
                using (FileStream fs = File.Create(path + fileName))
                {
                    Byte[] buffer = new Byte[32 * 1024];
                    int read = context.Request.GetBufferlessInputStream().Read(buffer, 0, buffer.Length);
                    using (StreamWriter w = new StreamWriter(fs, System.Text.Encoding.GetEncoding("iso-8859-1")))
                    {
                    while (read > 0)
                    {
                        
                        //fs.Write(buffer, 0, read);                       


                        char[] chars = System.Text.Encoding.GetEncoding("iso-8859-1").GetChars(buffer);
                            w.Write(chars, 0, read);
                        
                        read = context.Request.GetBufferlessInputStream().Read(buffer, 0, buffer.Length);
                    }
                    } 
                    fs.Close();  
                    
                }
                
                
                context.Response.Write(fileName);
               
            }
            catch (Exception ex)
            {
            }
           // return fileName;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}