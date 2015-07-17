using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Data.SqlClient;
using System.Net.Mail;

/// <summary>
/// Summary description for BusinessLogicLayer
/// </summary>
public class BusinessLogicLayer
{
    private SqlConnection cn;
    private SqlCommand cmd;
    private SqlDataAdapter da;
    private DataSet ds;
    private DataView dv;
    //    private DataReader dr;
    public BusinessLogicLayer()
    {
        string cs;
        cs = ConfigurationManager.ConnectionStrings["ASPNETDBCS"].ConnectionString;
        cn = new SqlConnection(cs);
        cmd = new SqlCommand();
        cmd.Connection = cn;
        cmd.CommandType = CommandType.Text;
    }
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
    public string createProcedure(string query)
    {
        try
        {
            cmd.CommandText = query;
            if (cn.State == ConnectionState.Closed)
                cn.Open();
            cmd.ExecuteNonQuery();
            cn.Close();
            return "success";
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    public string addJobPostAdmin(string jobtext, string companyname, string updateddate)
    {
        try
        {
            cmd.CommandText = "insert into jobpost (jobtext,jobcompanyname,jobupdatedon) values('" + jobtext + "','" + companyname + "','" + updateddate + "')";
            if (cn.State == ConnectionState.Closed)
                cn.Open();
            cmd.ExecuteNonQuery();
            cn.Close();
            return "true";
        }
        catch (Exception ex)
        {

            return ex.Message;
        }
    }
    public string addNewsAdmin(string newstext, string category, string updateddate)
    {
        try
        {
            cmd.CommandText = "insert into newspost (newstext,newscategory,newsupdatedon) values('" + newstext + "','" + category + "','" + updateddate + "')";
            if (cn.State == ConnectionState.Closed)
                cn.Open();
            cmd.ExecuteNonQuery();
            cn.Close();
            return "true";
        }
        catch (Exception ex)
        {

            return ex.Message;
        }
    }
    public void addEmailtoContact(string emailid, string mobile, string primaryemail, string secondaryemail)
    {
        try
        {
            cmd.CommandText = "insert into jscontact (email,mobile,primaryemail,secondaryemail) values ('" + emailid + "','" + mobile + "','" + primaryemail + "','" + secondaryemail + "')";
            if (cn.State == ConnectionState.Closed)
                cn.Open();
            cmd.ExecuteNonQuery();
            cn.Close();
        }
        catch (Exception ex)
        {
            
            throw;
        }
    }
    public void addEmailtoPG(string emailid)
    {
        try
        {
            cmd.CommandText = "insert into jspostgraduation (email) values('" + emailid + "')";
            if (cn.State == ConnectionState.Closed)
                cn.Open();
            cmd.ExecuteNonQuery();
            cn.Close();

        }
        catch (Exception ex)
        {
            
            throw;
        }
    }
    public void addEmailtoGrad(string emailid)
    {
        try
        {
            cmd.CommandText = "insert into jsgraduation (email) values('" + emailid + "')";
            if (cn.State == ConnectionState.Closed)
                cn.Open();
            cmd.ExecuteNonQuery();
            cn.Close();

        }
        catch (Exception ex)
        {

            throw;
        }
    }
    public void addEmailtoInter(string emailid)
    {
        try
        {
            cmd.CommandText = "insert into jsinterdiplamo (email) values('" + emailid + "')";
            if (cn.State == ConnectionState.Closed)
                cn.Open();
            cmd.ExecuteNonQuery();
            cn.Close();

        }
        catch (Exception ex)
        {

            throw;
        }
    }
    public void addEmailtoSSC(string emailid)
    {
        try
        {
            cmd.CommandText = "insert into jsssc (email) values('" + emailid + "')";
            if (cn.State == ConnectionState.Closed)
                cn.Open();
            cmd.ExecuteNonQuery();
            cn.Close();

        }
        catch (Exception ex)
        {

            throw;
        }
    }
    public bool updateContactDetails(string email,string presentaddress,string presentcity,string presentstate,string permanentaddress,string permanentcity,string permanentstate,string mobile,string secemail)
    {
        try
        {
            cmd.CommandText = "update jscontact set presentaddress='" + presentaddress + "', presentcity='" + presentcity + "',presentstate='" + presentstate + "',permanentaddress='" + permanentaddress + "', permanentcity='" + permanentcity + "', permanentstate='" + permanentstate + "',secondaryemail='" + secemail + "' where email='" + email + "'";
            if (cn.State == ConnectionState.Closed)
                cn.Open();
            cmd.ExecuteNonQuery();
            cn.Close();
            return true;
        }
        catch (Exception ex)
        {
            return false;
            throw;
        }
    }
}
