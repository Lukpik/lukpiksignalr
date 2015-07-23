using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data;
using System.Net.Mail;

namespace Lukpik.Cl
{
    public class MySQLBusinessLogic
    {
        private MySqlConnection con;
        private MySqlCommand cmd;
        public MySQLBusinessLogic()
        {
            string connStr = ConfigurationManager.ConnectionStrings["ERETAILDB"].ToString();
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

        public bool CheckStoreExistence(string storename, string email)
        {
            // 0 - Transaction failed
            // 1 - Transaction succeeded

            bool retVal = true;
            DataTable dt = new DataTable();
            try
            {
                string cmdText = "select 1 from `store` where `Email`='" + email + "'";
                cmd = new MySqlCommand(cmdText, con);
                con.Open();
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                da.Fill(dt);
                con.Close();
                if (dt.Rows.Count > 0)
                    retVal = true;
                else
                    retVal = false;
            }
            catch (Exception ex)
            {
                retVal = true;
            }
            return retVal;
        }
        public int AddStore(string storename, string storecity, string storephone, DateTime firstOpeneddate, DateTime remodeldate, string email, string fname, string lname, int isVerified, int activationFlag, string passsword, int cardsAccepted, int homedeliveryflag, int trailflag)
        {
            // 0 - Transaction failed
            // 1 - Transaction succeeded
            // 2 - Username already existed
            int retVal = 0;
            try
            {
                if (!CheckStoreExistence(storename, email))
                {
                    //string cmdText = "insert into test_table(test_column1,test_column2) values(@col1,@col2)";
                    string cmdText = "insert into `store` (`store_name`,`store_city`,`store_phone`,`first_opened_date`,`last_remodel_date`,`Email`,`StoreOwnerFirstName`,`StoreOwnerLastName`,`IsVerified`,`ActivationFlag`,`password`,`Cardsaccepted`,`homedeliveryflag`,`trialroomflag`) values(@storename,@storecity,@storephone,@firstOpeneddate,@remodeldate,@email,@fname, @lname, @isVerified,@activationFlag,@passsword,@cardsAccepted,@homedeliveryflag,@trailflag);";
                    cmd = new MySqlCommand(cmdText, con);
                    cmd.Parameters.AddWithValue("@storename", storename);
                    cmd.Parameters.AddWithValue("@storecity", storecity);
                    cmd.Parameters.AddWithValue("@storephone", storephone);
                    cmd.Parameters.AddWithValue("@firstOpeneddate", firstOpeneddate);
                    cmd.Parameters.AddWithValue("@remodeldate", remodeldate);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@fname", fname);
                    cmd.Parameters.AddWithValue("@lname", lname);
                    cmd.Parameters.AddWithValue("@isVerified", isVerified);
                    cmd.Parameters.AddWithValue("@activationFlag", activationFlag);
                    cmd.Parameters.AddWithValue("@passsword", passsword);
                    cmd.Parameters.AddWithValue("@cardsAccepted", cardsAccepted);
                    cmd.Parameters.AddWithValue("@homedeliveryflag", homedeliveryflag);
                    cmd.Parameters.AddWithValue("@trailflag", trailflag);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    retVal = 1;
                }
                else
                {
                    retVal = 2;
                }
            }
            catch (Exception ex)
            {
                retVal = 0;
            }
            return retVal;
        }

        public bool UpdateStoreDetails(string storename, string storetype, string shortdesc, string storedesc, string brands, string othercategories, int iscreditcard, int istrail, int ishomedelivery, string ownerfirstname, string ownerlastname, string phone, string email, string addline1, string addline2, string city, string state, string country, string pincode, double latitude, double longitude, string websiteurl, string fburl, string twitterurl, string googleurl, DateTime datemodified)
        {
            bool retVal = false;
            try
            {
                //StoreTagLine - fpor short description
                string cmdText = "SET SQL_SAFE_UPDATES = 0; update `store` set `store_name`=@storename,`store_type`=@storetype,`StoreTagLine`=@shortdesc,`StoreDescription`=@storedesc,`Cardsaccepted`=@iscreditcard,`trialroomflag`=@istrail,`homedeliveryflag`=@ishomedelivery, `StoreOwnerFirstName`=@ownerfirstname, `StoreOwnerLastName`=@ownerlastname, `store_phone`=@phone, `store_street_addressline1`=@addline1, `store_street_addressline2`=@addline2, `store_city`=@city,`store_state`=@state, `store_country`=@country, `store_postal_code`=@pincode, `Latitude`=@latitude, `Longitude`=@longitude, `StoreWebsite`=@websiteurl, `StoreFacebookPage`=@fburl, `StoreTwitterPage`=@twitterurl, `StoreGooglePage`=@googleurl,`last_remodel_date`=@datemodified where `Email`='" + email + "' ";
                cmd = new MySqlCommand(cmdText, con);
                cmd.Parameters.AddWithValue("@storename", storename);
                cmd.Parameters.AddWithValue("@storetype", storetype);
                cmd.Parameters.AddWithValue("@shortdesc", shortdesc);
                cmd.Parameters.AddWithValue("@storedesc", storedesc);
                cmd.Parameters.AddWithValue("@iscreditcard", iscreditcard);
                cmd.Parameters.AddWithValue("@istrail", istrail);
                cmd.Parameters.AddWithValue("@ishomedelivery", ishomedelivery);
                cmd.Parameters.AddWithValue("@ownerfirstname", ownerfirstname);
                cmd.Parameters.AddWithValue("@ownerlastname", ownerlastname);
                cmd.Parameters.AddWithValue("@phone", phone);
                cmd.Parameters.AddWithValue("@addline1", addline1);
                cmd.Parameters.AddWithValue("@addline2", addline2);
                cmd.Parameters.AddWithValue("@city", city);
                cmd.Parameters.AddWithValue("@state", state);
                cmd.Parameters.AddWithValue("@country", country);
                cmd.Parameters.AddWithValue("@pincode", pincode);
                cmd.Parameters.AddWithValue("@latitude", latitude);
                cmd.Parameters.AddWithValue("@longitude", longitude);
                cmd.Parameters.AddWithValue("@websiteurl", websiteurl);
                cmd.Parameters.AddWithValue("@fburl", fburl);
                cmd.Parameters.AddWithValue("@twitterurl", twitterurl);
                cmd.Parameters.AddWithValue("@googleurl", googleurl);
                cmd.Parameters.AddWithValue("@datemodified", datemodified);
                
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                retVal = true;
            }
            catch (Exception)
            {
                retVal = false;
            }
            return retVal;
        }


        # endregion

        #region CHANGE PASSWORD

        public int ChangePassword(string email, string oldpwd, string newpwd)
        {
            int retVal = 0;
            try
            {
                if (CheckAuthenticatedUser(email, oldpwd))
                {
                    string cmdText = "SET SQL_SAFE_UPDATES = 0; update `store` set `password`=@newpwd where `Email`=@email";
                    cmd = new MySqlCommand(cmdText, con);
                    cmd.Parameters.AddWithValue("@newpwd", newpwd);
                    cmd.Parameters.AddWithValue("@email", email);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    retVal = 1;
                }
                else
                {
                    retVal = 2;
                }
            }
            catch (Exception ex)
            {
                //0 - if something went wrong(exception)
                //1 - if passord changed successfully
                //2 - if authentication fails
                retVal = 0;
            }
            return retVal;
        }

        public bool CheckAuthenticatedUser(string email, string pwd)
        {
            bool retVal = false;
            try
            {
                DataTable dt = new DataTable();
                string cmdText = "select 1 from `store` where `Email`='" + email + "' and `password`='" + pwd + "'";
                cmd = new MySqlCommand(cmdText, con);
                con.Open();
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                da.Fill(dt);
                con.Close();
                if (dt.Rows.Count == 1)
                    retVal = true;
                else
                    retVal = false;
            }
            catch (Exception ex)
            {
                retVal = false;
            }
            return retVal;
        }
        #endregion

        public DataTable GetStoreOwnerName(string email)
        {
            DataTable dt = new DataTable();
            try
            {
                string cmdText = "select `StoreOwnerFirstName`,`StoreOwnerLastName` from `store` where `Email`='" + email + "'";
                cmd = new MySqlCommand(cmdText, con);
                con.Open();
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                da.Fill(dt);
                con.Close();
                if (dt.Rows.Count != 1)
                {
                    //If no.of rows greater than 1 return send and empty DataTable
                    dt = new DataTable();
                }
            }
            catch (Exception ex)
            {
                dt = new DataTable();
            }
            return dt;
        }

        # region USER LOGIN
        public int LoginUser(string username, string pwd)
        {
            int retVal = 0;
            try
            {
                DataTable dt = new DataTable();
                string cmdText = "select 1 from `store` where `Email`='" + username + "' and `password`='" + pwd + "'";
                cmd = new MySqlCommand(cmdText, con);
                con.Open();
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                da.Fill(dt);
                con.Close();
                if (dt.Rows.Count == 1)
                {
                    retVal = 1;
                    //AddtoLoginHistory(username, DateTime.Now);
                }
                else
                    retVal = 2;
            }
            catch (Exception ex)
            {
                retVal = 0;
            }
            return retVal;
        }

        public bool AddtoLoginHistory(string username, DateTime dt)
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

        public DataTable GetStoreRetailerDetails(string username)
        {
            DataTable dt = new DataTable();
            try
            {
                string cmdText = "select * from `store` where `Email`='" + username + "'";
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

        #endregion


   
    }
}