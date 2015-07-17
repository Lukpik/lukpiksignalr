using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data;
using System.Net.Mail;

/// <summary>
/// Summary description for MySqlBL
/// </summary>
public class MySqlBL
{
    private MySqlConnection con;
    private MySqlCommand cmd;
    public MySqlBL()
    {
        string connStr = ConfigurationManager.ConnectionStrings["DAILYEXPENSES_MYSQL"].ToString();
        con = new MySqlConnection(connStr);


    }

    # region TEST QUERIES
    public bool InsertValues(string col1, string col2)
    {
        bool retVal = false;
        try
        {
            string cmdText = "insert into test_table(test_column1,test_column2) values(@col1,@col2)";
            cmd = new MySqlCommand(cmdText, con);
            cmd.Parameters.AddWithValue("@col1", col1);
            cmd.Parameters.AddWithValue("@col2", col2);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
            retVal = true;
        }
        catch (Exception ex)
        {
        }
        return retVal;
    }
    public DataTable GetValues()
    {
        DataTable dt = new DataTable();
        try
        {
            string cmdText = "select * from test_table";
            cmd = new MySqlCommand(cmdText, con);
            con.Open();
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            da.Fill(dt);
            con.Close();
        }
        catch (Exception ex)
        {
        }
        return dt;
    }
    # endregion

    # region USER REGISTRATION
    public bool CheckUsername(string username)
    {
        bool retVal = false;
        DataTable dt = new DataTable();
        try
        {
            string cmdText = "select * from `user_registration` where `usr_uname`='" + username + "'";
            cmd = new MySqlCommand(cmdText, con);
            con.Open();
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            da.Fill(dt);
            con.Close();
            if (dt.Rows.Count > 0)
                retVal = false;
            else
                retVal = true;
        }
        catch (Exception ex)
        {
        }
        return retVal;
    }
    public bool CheckEmail(string email)
    {
        bool retVal = false;
        DataTable dt = new DataTable();
        try
        {
            string cmdText = "select * from `user_registration` where `usr_email`='" + email + "'";
            cmd = new MySqlCommand(cmdText, con);
            con.Open();
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            da.Fill(dt);
            con.Close();
            if (dt.Rows.Count > 0)
                retVal = false;
            else
                retVal = true;
        }
        catch (Exception ex)
        {
        }
        return retVal;
    }
    public bool AddUser(string username, string pwd, string email, string type, int isActive, DateTime dt)
    {
        bool retVal = false;
        try
        {
            //string cmdText = "insert into test_table(test_column1,test_column2) values(@col1,@col2)";
            string cmdText = "insert into `user_registration` (`usr_uname`,`usr_password`,`usr_email`,`usr_type`,`usr_active`,`datetimereg`) values(@username,@pwd,@email,@type,@isActive,@dt);";
            cmd = new MySqlCommand(cmdText, con);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@pwd", pwd);
            cmd.Parameters.AddWithValue("@email", email);
            cmd.Parameters.AddWithValue("@type", type);
            cmd.Parameters.AddWithValue("@isActive", isActive);
            cmd.Parameters.AddWithValue("@dt", dt);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
            retVal = true;
        }
        catch (Exception ex)
        {
        }
        return retVal;
    }

    # endregion

    # region USER LOGIN
    public bool LoginUser(string username, string pwd)
    {
        bool retVal = false;
        try
        {
            DataTable dt = new DataTable();
            string cmdText = "select * from `user_registration` where `usr_uname`='" + username + "' and `usr_password`='" + pwd + "'";
            cmd = new MySqlCommand(cmdText, con);
            con.Open();
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            da.Fill(dt);
            con.Close();
            if (dt.Rows.Count == 1 )
            {
                retVal = true;
                AddtoLoginHistory(username, DateTime.Now);
            }
            else
                retVal = false;
        }
        catch (Exception ex)
        {
        }
        return retVal;
    }

    public bool AddtoLoginHistory(string username,DateTime dt)
    {
        bool retVal = false;
        try
        {
            string cmdText = "insert into `userlogin_history` (`log_uname`,`log_date`) values(@username,@dt);";
            cmd = new MySqlCommand(cmdText, con);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@dt", dt);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
            retVal = true;
        }
        catch (Exception ex)
        {
            retVal = true;
        }
        return retVal;
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

    #endregion


}