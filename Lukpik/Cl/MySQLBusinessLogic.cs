using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data;
using System.Net.Mail;
using System.IO;

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
      
        public bool CheckPhoneExistance(string storename, string storephone)
        {
            // 0 - Transaction failed
            // 1 - Transaction succeeded

            bool retVal = true;
            DataTable dt = new DataTable();
            try
            {
                string cmdText = "select count(1) from `store` where `store_phone`=@storephone";
                cmd = new MySqlCommand(cmdText, con);
                cmd.Parameters.AddWithValue("@storephone", storephone);
                con.Open();
                int value = Convert.ToInt32(cmd.ExecuteScalar().ToString());
                //MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                //da.Fill(dt);
                con.Close();
                if (value > 0)
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

        public bool CheckEmailExistance(string email)
        {
            bool retVal = false;
            try
            {
                DataTable dt = new DataTable();
                string cmdText = "select count(1) from `store` where `Email`=@email";
                cmd = new MySqlCommand(cmdText, con);
                cmd.Parameters.AddWithValue("@email", email);
                con.Open();
                int value = Convert.ToInt32(cmd.ExecuteScalar().ToString());
                //MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                //da.Fill(dt);
                con.Close();
                if (value > 0)
                    retVal = true;
                else
                    retVal = false;
            }
            catch (Exception ex)
            {
            }
            return retVal;
        }
        public int AddStore(string storename, string storecity, string storephone, string storephone2, DateTime firstOpeneddate, DateTime remodeldate, string email, string fname, string lname, int isVerified, int activationFlag, string passsword, int cardsAccepted, int homedeliveryflag, int trailflag)
        {
            // 0 - Transaction failed
            // 1 - Transaction succeeded
            // 2 - Username already existed
            int retVal = 0;
            try
            {
                // && (email == "" || !CheckEmailExistance(email))
                if (!CheckPhoneExistance(storename, storephone) && (email == "" || !CheckEmailExistance(email)))
                {
                    //string cmdText = "insert into test_table(test_column1,test_column2) values(@col1,@col2)";
                    string cmdText = "insert into `store` (`store_name`,`store_city`,`store_phone`,`first_opened_date`,`last_remodel_date`,`Email`,`StoreOwnerFirstName`,`StoreOwnerLastName`,`IsVerified`,`ActivationFlag`,`password`,`Cardsaccepted`,`homedeliveryflag`,`trialroomflag`,`StoreAlternativeNumber`) values(@storename,@storecity,@storephone,@firstOpeneddate,@remodeldate,@email,@fname, @lname, @isVerified,@activationFlag,@passsword,@cardsAccepted,@homedeliveryflag,@trailflag,@storephone2);";
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
                    cmd.Parameters.AddWithValue("@storephone2", storephone2);

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

        public bool UpdateStoreDetails(string storename, string storetype, string shortdesc, string storedesc, string brands, string othercategories, int iscreditcard, int istrail, int ishomedelivery, string ownerfirstname, string ownerlastname, string phone, string email, string addline1, string addline2, string city, string state, string country, string pincode, double latitude, double longitude, string websiteurl, string fburl, string twitterurl, string googleurl, DateTime datemodified,int storeID)
        {
            bool retVal = false;
            try
            {
                //StoreTagLine - fpor short description
                string cmdText = "SET SQL_SAFE_UPDATES = 0; update `store` set `Email`=@email, `store_name`=@storename,`store_type`=@storetype,`StoreTagLine`=@shortdesc,`StoreDescription`=@storedesc,`Cardsaccepted`=@iscreditcard,`trialroomflag`=@istrail,`homedeliveryflag`=@ishomedelivery, `StoreOwnerFirstName`=@ownerfirstname, `StoreOwnerLastName`=@ownerlastname, `store_phone`=@phone, `store_street_addressline1`=@addline1, `store_street_addressline2`=@addline2, `store_city`=@city,`store_state`=@state, `store_country`=@country, `store_postal_code`=@pincode, `Latitude`=@latitude, `Longitude`=@longitude, `StoreWebsite`=@websiteurl, `StoreFacebookPage`=@fburl, `StoreTwitterPage`=@twitterurl, `StoreGooglePage`=@googleurl,`last_remodel_date`=@datemodified where `store_id`=@storeID";
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
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@storeID", storeID);
                
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

        public bool UpdateOtherCategories(int storeID, string othercategories)
        {
            bool retVal = false;
            try
            {
                //Divide othercategories in to array
                // Delete from the table name where yu find store id
                //Insert all the operations again
                
                //string[] cat= othercategories.Split(new string[] { "," }, StringSplitOptions.None);
                string[] cat = othercategories.Split(',');
                if(DeleteStoreIDfromOtherCat(storeID))
                {
                    for(int i = 0; i < cat.Length; i++)
                    {
                        //Insertion
                        int categoryID=Convert.ToInt32(cat[i]);
                        string cmdText = "insert into `storecategories`(`StoreID`,`CategoryID`) values (" + storeID + "," + categoryID + ")";
                        cmd = new MySqlCommand(cmdText, con);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                

            }
            catch (Exception ex)
            {
            }
            return retVal;
        }

        public bool UpdateBrands(int storeID, string brands)
        {
            bool retVal = false;
            try
            {
                //Divide othercategories in to array
                // Delete from the table name where yu find store id
                //Insert all the operations again

                //string[] cat= othercategories.Split(new string[] { "," }, StringSplitOptions.None);
                string[] cat = brands.Split(',');
                if (DeleteStoreIDfromBrands(storeID))
                {
                    for (int i = 0; i < cat.Length; i++)
                    {
                        //Insertion
                        //int brandID = Convert.ToInt32(cat[i]);
                        string brandName = cat[i];
                        if (brandName != "")
                        {
                            int brandID = InsertBrandandGetID(brandName);
                            string cmdText = "insert into `storebrands`(`StoreID`,`BrandID`) values (" + storeID + "," + brandID + ")";
                            cmd = new MySqlCommand(cmdText, con);
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                }


            }
            catch (Exception ex)
            {
            }
            return retVal;
        }

        public int InsertBrandandGetID(string brandName)
        {
            try
            {
                List<string> lstOfBrandName = GetAllBrands();
                string val = lstOfBrandName.FirstOrDefault(x => x == brandName.ToLower());
                if (val == "" || val == null)
                {
                    string cmdText = "insert into `brands`(`brandname`) values ('" + brandName + "')";
                    cmd = new MySqlCommand(cmdText, con);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }

            }
            catch (Exception ex)
            { }
            return GetBrandNameID(brandName);
        }

        public int GetBrandNameID(string brandName)
        {
            int bID = 0;
            try
            {
                DataTable dt = new DataTable();
                string cmdQry = "select `BrandID` from `brands` where `brandname`='" + brandName + "'";
                cmd = new MySqlCommand(cmdQry, con);
                con.Open();
                cmd.ExecuteNonQuery();
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                da.Fill(dt);
                con.Close();
                bID = Convert.ToInt32(dt.Rows[0].ItemArray[0]);
            }
            catch (Exception ex)
            {
                bID = 0;
            }
            return bID;
        }

        public List<string> GetAllBrands()
        {
            List<string> lstBrandNames = new List<string>();
            try
            {
                DataTable dt = new DataTable();
                string cmdQry = "select `brandname` from `brands` ";
                cmd = new MySqlCommand(cmdQry, con);
                con.Open();
                cmd.ExecuteNonQuery();
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                da.Fill(dt);
                con.Close();
                lstBrandNames = (from row in dt.AsEnumerable() select Convert.ToString(row[dt.Columns[0].ColumnName]).ToLower()).Distinct().ToList();
            }
            catch (Exception ex)
            { }
            return lstBrandNames;
        }

        public DataTable GetAllBrandsDataTable()
        {
            DataTable dt = new DataTable();
            try
            {
                
                string cmdQry = "select * from `brands` ";
                cmd = new MySqlCommand(cmdQry, con);
                con.Open();
                cmd.ExecuteNonQuery();
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                da.Fill(dt);
                con.Close();
                
            }
            catch (Exception ex)
            { }
            return dt;
        }

        private string GetBrandNamefromID(int brandID)
        {
            string brandName = "";
            try
            {
                DataTable dt = new DataTable();
                string cmdQry = "select `brandname` from `brands` where `BrandID`=" + brandID + "";
                cmd = new MySqlCommand(cmdQry, con);
                con.Open();
                cmd.ExecuteNonQuery();
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                da.Fill(dt);
                con.Close();
                brandName = dt.Rows[0].ItemArray[0].ToString();
            }
            catch (Exception ex)
            {
                brandName = "";
            }
            return brandName;
        }

        public bool DeleteStoreIDfromBrands(int storeID)
        {
            bool retVal = false;
            try
            {
                string cmdText = "delete from `storebrands` where `StoreID`=@storeID;";
                cmd = new MySqlCommand(cmdText, con);
                cmd.Parameters.AddWithValue("@storeID", storeID);

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
        public bool DeleteStoreIDfromOtherCat(int storeID)
        {
            bool retVal = false;
            try
            {
                string cmdText = "delete from `storecategories` where `StoreID`=@storeID;";
                cmd = new MySqlCommand(cmdText, con);
                cmd.Parameters.AddWithValue("@storeID", storeID);

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

        #region CHANGE PASSWORD

        public int ChangePassword(int storeID,string phone, string oldpwd, string newpwd)
        {
            int retVal = 0;
            try
            {
                if (CheckAuthenticatedUser(phone, oldpwd))
                {
                    string cmdText = "SET SQL_SAFE_UPDATES = 0; update `store` set `password`=@newpwd where `store_id`=@storeID";
                    cmd = new MySqlCommand(cmdText, con);
                    cmd.Parameters.AddWithValue("@newpwd", newpwd);
                    cmd.Parameters.AddWithValue("@storeID", storeID);
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

        public bool CheckAuthenticatedUser(string phone, string pwd)
        {
            bool retVal = false;
            try
            {
                DataTable dt = new DataTable();
                string cmdText = "select 1 from `store` where `store_phone`=@phone and `password`=@pwd";
                cmd = new MySqlCommand(cmdText, con);
                cmd.Parameters.AddWithValue("@phone", phone);
                cmd.Parameters.AddWithValue("@pwd", pwd);
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

        public DataTable GetStoreOwnerName(int storeID)
        {
            DataTable dt = new DataTable();
            try
            {
                string cmdText = "select `StoreOwnerFirstName`,`StoreOwnerLastName`,`Email` from `store` where `store_id`=@storeID";
                cmd = new MySqlCommand(cmdText, con);
                cmd.Parameters.AddWithValue("@storeID", storeID);
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
        public int LoginUser(string phone, string pwd)
        {
            int retVal = 0;
            try
            {
                //Authentication of user by phone number
                DataTable dt = new DataTable();
                string cmdText = "select 1 from `store` where `store_phone`=@phone and `password`=@pwd";
                cmd = new MySqlCommand(cmdText, con);
                cmd.Parameters.AddWithValue("@phone", phone);
                cmd.Parameters.AddWithValue("@pwd", pwd);
                con.Open();
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                da.Fill(dt);
                con.Close();
                if (dt.Rows.Count == 1)
                {
                    retVal = 1;
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

        public int RetailerLoginCount(int storeID)
        {
            int retVal = 0;
            try
            {

                string cmdText = "select count(1) from `retailersessions` where `StoreID`=@storeID";
                cmd = new MySqlCommand(cmdText, con);
                cmd.Parameters.AddWithValue("@storeID", storeID);
                con.Open();
                retVal = Convert.ToInt32(cmd.ExecuteScalar().ToString());
                con.Close();
            }
            catch (Exception ex)
            {
            }
            return retVal;
        }
        public bool AddtoRetailerLoginHistory(int storeID,DateTime date)
        {
            bool retVal = false;
            try
            {
                string cmdText = "insert into `retailersessions` (`StoreID`,`logintime`) values(@storeID,@date);";
                cmd = new MySqlCommand(cmdText, con);
                cmd.Parameters.AddWithValue("@storeID", storeID);
                cmd.Parameters.AddWithValue("@date", date);
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
        //public bool AddtoLoginHistory(string username, DateTime dt)
        //{
        //    bool retVal = false;
        //    try
        //    {
        //        string cmdText = "insert into `userlogin_history` (`log_uname`,`log_date`) values(@username,@dt);";
        //        cmd = new MySqlCommand(cmdText, con);
        //        cmd.Parameters.AddWithValue("@username", username);
        //        cmd.Parameters.AddWithValue("@dt", dt);
        //        con.Open();
        //        cmd.ExecuteNonQuery();
        //        con.Close();
        //        retVal = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        retVal = true;
        //    }
        //    return retVal;
        //}

        public DataTable GetStoreRetailerDetails(int storeID)
        {
            DataTable dt = new DataTable();
            try
            {
                string cmdText = "select `store_id`,`store_type`,`store_name`,`store_number`,`store_street_addressline1`,`store_city`,`store_state`,`store_postal_code`,`store_country`,`store_manager`,`store_phone`,`store_fax`,`first_opened_date`,`last_remodel_date`,`Email`,`StoreOwnerFirstName`,`StoreOwnerLastName`,`IsVerified`,`CurrencyFormat`,`StoreDescription`,`StoreWebsite`,`StoreTwitterPage`,`StoreFacebookPage`,`StoreGooglePage`,`StoreAlternativePhoneNumber`,`IsStoreActive`,`StoreImage`,`StoreTagLine`,`Latitude`,`Longitude`,`StoreLukpikUrl`,`ActivationFlag`,`Cardsaccepted`,`homedeliveryflag`,`trialroomflag`,`store_street_addressline2` from `store` where `store_id`=@storeID";
                cmd = new MySqlCommand(cmdText, con);
                cmd.Parameters.AddWithValue("@storeID", storeID);
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

        public DataTable GetOtherCategories()
        {
            DataTable dt = new DataTable();
            try
            {
                string cmdText = "select * from `storetypecategories`";
                cmd = new MySqlCommand(cmdText, con);
                con.Open();
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                da.Fill(dt);
                con.Close();
            }
            catch (Exception ex)
            {
                dt = new DataTable();
            }
            return dt;
        }

        public List<string> GetOtherCategoriesbyStoreID(int storeID)
        {
            List<string> lst = new List<string>();
            try
            {
                DataTable dt = new DataTable();
                string cmdText = "select distinct `CategoryID` from `storecategories` where `StoreID`=@storeID";
                cmd = new MySqlCommand(cmdText, con);
                cmd.Parameters.AddWithValue("storeID", storeID);
                con.Open();
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                da.Fill(dt);
                con.Close();
                if (dt.Rows.Count > 0)
                {
                    int i = 0;
                    foreach (DataRow item in dt.Rows)
                    {
                        lst.Add(item.ItemArray[0].ToString());
                        i++;

                    }
                }
                else
                {
                    lst = new List<string>();
                }
            }
            catch (Exception ex)
            {
            }
            return lst;
        }

        public List<string> GetBrandsbyStoreID(int storeID)
        {
            List<string> lst = new List<string>();
            try
            {
                DataTable dt = new DataTable();
                string cmdText = "select `BrandID` from `storebrands` where `StoreID`=@storeID";
                cmd = new MySqlCommand(cmdText, con);
                cmd.Parameters.AddWithValue("storeID", storeID);
                con.Open();
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                da.Fill(dt);
                con.Close();
                if (dt.Rows.Count > 0)
                {
                    int i = 0;
                    foreach (DataRow item in dt.Rows)
                    {

                        string brandName = GetBrandNamefromID(Convert.ToInt32(item.ItemArray[0]));
                        if (brandName != "")
                            lst.Add(brandName);
                        i++;

                    }
                }
                else
                {
                    lst = new List<string>();
                }
            }
            catch (Exception ex)
            {
            }
            return lst;
        }

        public DataTable GetStoreID(string phone)
        {
            DataTable dt = new DataTable();
            try
            {
                string cmdText = "select `store_id` from `store` where `store_phone`='" + phone + "'";
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

        #region FORGOT PASSWORD
        public DataTable GetPassword(string phonenum)
        {
            DataTable dt = new DataTable();
            try
            {
                string cmdText = "select `password`,`StoreOwnerFirstName`,`Email` from `store` where `store_phone`='" + phonenum + "'";
                cmd = new MySqlCommand(cmdText, con);
                con.Open();
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                da.Fill(dt);
                con.Close();
            }
            catch (Exception ex)
            {
                dt = new DataTable();
                return dt;
            }
            return dt;
        }
        #endregion

        #region PRODUCT
        public int AddProduct(string productname, string gender, int productFamilyID, string productdescription, double price, string quantity, string size, string color, int visibility, int productCategoryID,int productSubCategoryID, int brandID, string collection, string images, int storeID, DateTime dtCreated, string email, List<byte[]> lstByt,byte[] thumbnail,string ecommercelink)
        {
            int retVal = 0;
            try
            {

                string cmdText = "insert into `products` (`ProductName`,`ProductLong Description`,`ProductFamilyID`,`ProductCategoryID`,`ProductSubCategoryID`,`Price`,`StoreID`,`ProductImage`,`IsVisible`,`CreatedDate`,`CreatedUser`,`ModifiedDate`,`ModifiedUser`,`Gender`,`BrandID`,`image1`,`image2`,`image3`,`image4`,`image5`,`image6`,`ECommerceLink`) values(@productname,@productdescription,@productFamilyID,@productCategoryID,@productSubCategoryID,@price,@storeID,@thumbnail,@visibility,@dtCreated,@email,@mofifiedDate,@mofifiedUser,@gender,@brandID,@image1,@image2,@image3,@image4,@image5,@image6,@ecommercelink);";
                cmd = new MySqlCommand(cmdText, con);
                cmd.Parameters.AddWithValue("@productname", productname);
                cmd.Parameters.AddWithValue("@productdescription", productdescription);
                cmd.Parameters.AddWithValue("@productFamilyID", productFamilyID);
                cmd.Parameters.AddWithValue("@productCategoryID", productCategoryID);
                cmd.Parameters.AddWithValue("@productSubCategoryID", productSubCategoryID);
                cmd.Parameters.AddWithValue("@price", price);
                cmd.Parameters.AddWithValue("@storeID", storeID);
                cmd.Parameters.AddWithValue("@thumbnail", thumbnail);
                cmd.Parameters.AddWithValue("@visibility", visibility);
                cmd.Parameters.AddWithValue("@dtCreated", dtCreated);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@mofifiedDate", dtCreated);
                cmd.Parameters.AddWithValue("@mofifiedUser", email);
                cmd.Parameters.AddWithValue("@gender", gender);
                cmd.Parameters.AddWithValue("@brandID", brandID);
                cmd.Parameters.AddWithValue("@image1", lstByt.Count > 0 ? lstByt[0] : null);
                cmd.Parameters.AddWithValue("@image2", lstByt.Count > 1 ? lstByt[1] : null);
                cmd.Parameters.AddWithValue("@image3", lstByt.Count > 2 ? lstByt[2] : null);
                cmd.Parameters.AddWithValue("@image4", lstByt.Count > 3 ? lstByt[3] : null);
                cmd.Parameters.AddWithValue("@image5", lstByt.Count > 4 ? lstByt[4] : null);
                cmd.Parameters.AddWithValue("@image6", lstByt.Count > 5 ? lstByt[5] : null);
                cmd.Parameters.AddWithValue("@ecommercelink", ecommercelink);


                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                retVal = 1;
            }
            catch (Exception)
            {
                retVal = 0;
            }
            return retVal;
        }

        public int UpdateProduct(int productID,string productname, string gender, int productFamilyID, string productdescription, double price, string quantity, string size, string color, int visibility, int productCategoryID, int productSubCategoryID, int brandID, string collection, int storeID, DateTime dtModified, string email, string ecommercelink)
        {
            int retVal = 0;
            try
            {

                string cmdText = "SET SQL_SAFE_UPDATES = 0; update `products` set `ProductName`=@productname,`ProductLong Description`=@productdescription,`ProductFamilyID`=@productFamilyID,`ProductCategoryID`=@productCategoryID,`ProductSubCategoryID`=@productSubCategoryID,`Price`=@price,`StoreID`=@storeID,`IsVisible`=@visibility,`ModifiedDate`=@modifieddate,`ModifiedUser`=@modifieduser,`Gender`=@gender,`BrandID`=@brandID,`ECommerceLink`=@ecommercelink where `ProductID`=@productID;";
                // values(@productname,@productdescription,@productFamilyID,@productCategoryID,@productSubCategoryID,@price,@storeID,@thumbnail,@visibility,@dtCreated,@email,@mofifiedDate,@mofifiedUser,@gender,@brandID,@image1,@image2,@image3,@image4,@image5,@image6,@ecommercelink)
                cmd = new MySqlCommand(cmdText, con);
                cmd.Parameters.AddWithValue("@productname", productname);
                cmd.Parameters.AddWithValue("@productdescription", productdescription);
                cmd.Parameters.AddWithValue("@productFamilyID", productFamilyID);
                cmd.Parameters.AddWithValue("@productCategoryID", productCategoryID);
                cmd.Parameters.AddWithValue("@productSubCategoryID", productSubCategoryID);
                cmd.Parameters.AddWithValue("@price", price);
                cmd.Parameters.AddWithValue("@storeID", storeID);
                cmd.Parameters.AddWithValue("@visibility", visibility);
                cmd.Parameters.AddWithValue("@modifieddate", dtModified);
                cmd.Parameters.AddWithValue("@modifieduser", email);
                cmd.Parameters.AddWithValue("@gender", gender);
                cmd.Parameters.AddWithValue("@brandID", brandID);
                cmd.Parameters.AddWithValue("@ecommercelink", ecommercelink);
                cmd.Parameters.AddWithValue("@productID", productID);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                retVal = 1;
            }
            catch (Exception)
            {
                retVal = 0;
            }
            return retVal;
        }

        public bool RemoveProduct(int productID)
        {
            bool retVal = false;
            try
            {
                string cmdText = "SET SQL_SAFE_UPDATES = 0; delete from `products` where `ProductID`=@productID";
                cmd = new MySqlCommand(cmdText, con);
                cmd.Parameters.AddWithValue("@productID", productID);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                retVal = true;
            }
            catch (Exception ex)
            {
                retVal = false;
            }
            return retVal;
        }
        public DataTable GetProductFamily()
        {
            DataTable dt = new DataTable();
            try
            {
                
                string cmdText = "select * from `productfamily`";
                cmd = new MySqlCommand(cmdText, con);
                con.Open();
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                da.Fill(dt);
                con.Close();
            }
            catch (Exception ex)
            {
                dt = new DataTable();
            }
            return dt;
        }

        public DataTable GetProductType()
        {
            DataTable dt = new DataTable();
            try
            {

                string cmdText = "select * from `producttypes`";
                cmd = new MySqlCommand(cmdText, con);
                con.Open();
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                da.Fill(dt);
                con.Close();
            }
            catch (Exception ex)
            {
                dt = new DataTable();
            }
            return dt;
        }

        public DataTable GetProductImageDetails(int storeID, int productID)
        {
            DataTable dt = new DataTable();
            try
            {
                //string cmdText = " select `ProductName`,`ProductLong Description`,`ProductFamilyID`,`ProductTypeID`,`ProductCategoryID`,`ProductSubCategoryID`,`Price`,`Quantity`,`StoreID`,`ProductImage`,`IsVisible`,`IsVariant`,`CreatedDate`,`CreatedUser`,`ModifiedDate`,`ModifiedUser`,`DiscountPrice`,`IsOnSale`,`IsNewStock`,`image1` from `products` where `StoreID`=@storeID";
                //a.`image1` ,
               // string cmdText = "Select a.`ProductName`,a.`ProductLong Description`,a.`ProductCategoryID`,a.`ProductSubCategoryID`,a.`Price`,a.`Quantity`,a.`StoreID`,a.`ProductImage`,a.`IsVisible`, a.`IsVariant`,a.`CreatedDate`,a.`CreatedUser`,a.`ModifiedDate`,a.`ModifiedUser`,a.`DiscountPrice`,a.`IsOnSale`,a.`IsNewStock`,b.Name as  `ProductFamilyName`,c.Name as `ProductTypeName`,d.`brandname` as `BrandName` FROM `products` a, `productfamily` b,`producttypes` c ,`brands` d where b.`ProductFamilyID`=a.`ProductFamilyID` and c.`ProductTypeID`=a.`ProductTypeID` and d.`BrandID`=a.`BrandID` and  a.`StoreID`=@storeID";
                string columns = "`ProductImage`";
                string fromCondition = "";
                if (productID != 0)
                {
                    columns = "`image1`,`image2`,`image3`,`image4`,`image5`,`image6`";
                    fromCondition = "and `ProductID`=@productID";
                }
                string cmdText = "Select " + columns + " FROM `products` where `StoreID`=@storeID " + fromCondition + " order by `ModifiedDate` desc ";
                cmd = new MySqlCommand(cmdText, con);
                cmd.Parameters.AddWithValue("@storeID", storeID);
                if (productID != 0)
                {
                    cmd.Parameters.AddWithValue("@productID", productID);
                }
                con.Open();
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                da.Fill(dt);
                con.Close();
                return dt;

            }
            catch (Exception ex)
            {
            }
            return dt;
        }
        
        public DataTable GetProductDetails(int storeID,int productID)
        {
            DataTable dt = new DataTable();
            try
            {
                //string cmdText = " select `ProductName`,`ProductLong Description`,`ProductFamilyID`,`ProductTypeID`,`ProductCategoryID`,`ProductSubCategoryID`,`Price`,`Quantity`,`StoreID`,`ProductImage`,`IsVisible`,`IsVariant`,`CreatedDate`,`CreatedUser`,`ModifiedDate`,`ModifiedUser`,`DiscountPrice`,`IsOnSale`,`IsNewStock`,`image1` from `products` where `StoreID`=@storeID";
                //a.`image1` ,
                string fromCondition = "";
                if (productID != 0)
                {
                    fromCondition = "and a.`ProductID`=@productID";
                }
                //Select a.`ProductID`,a.`ProductName`,a.`ProductLong Description` as `ProductLongDescription`,a.`ProductCategoryID`,a.`ProductSubCategoryID`,a.`Price`,a.`Quantity`, a.`StoreID`,a.`ProductImage`,a.`IsVisible`, a.`IsVariant`,a.`CreatedDate`,a.`CreatedUser`,a.`ModifiedDate`,a.`ModifiedUser`,a.`DiscountPrice`,a.`IsOnSale`,a.`IsNewStock`, b.Name as  `ProductFamilyName`,c.Name as `ProductCategoryName`,e.`Name` as `ProductSubCategoryName`,d.`brandname` as `BrandName`,a.`ECommerceLink`,a.`Gender` FROM `products` a, `productfamily` b, `productcategory` c ,`brands` d,`productsubcategory` e where b.`ProductFamilyID`=a.`ProductFamilyID` and c.`ProductCategoryID`=a.`ProductCategoryID` and d.`BrandID`=a.`BrandID` and a.`ProductSubCategoryID`=e.`ProductSubCategoryID` and a.`StoreID`=14 order by a.`ModifiedDate` desc
                string cmdText = "Select a.`ProductID`,a.`ProductName`,a.`ProductLong Description` as `ProductLongDescription`,a.`ProductCategoryID`,a.`ProductSubCategoryID`,a.`Price`,a.`Quantity`, a.`StoreID`,a.`ProductImage`,a.`IsVisible`, a.`IsVariant`,a.`CreatedDate`,a.`CreatedUser`,a.`ModifiedDate`,a.`ModifiedUser`,a.`DiscountPrice`,a.`IsOnSale`,a.`IsNewStock`, b.Name as  `ProductFamilyName`,c.Name as `ProductCategoryName`,e.`Name` as `ProductSubCategoryName`,d.`brandname` as `BrandName`,a.`ECommerceLink`,a.`Gender` FROM `products` a, `productfamily` b, `productcategory` c ,`brands` d,`productsubcategory` e where b.`ProductFamilyID`=a.`ProductFamilyID` and c.`ProductCategoryID`=a.`ProductCategoryID` and d.`BrandID`=a.`BrandID` and a.`ProductSubCategoryID`=e.`ProductSubCategoryID` and a.`StoreID`=@storeID " + fromCondition + " order by a.`ModifiedDate` desc";

                cmd = new MySqlCommand(cmdText, con);
                cmd.Parameters.AddWithValue("@storeID", storeID);
                if (productID != 0)
                {
                    cmd.Parameters.AddWithValue("@productID", productID);
                }
                con.Open();
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                da.Fill(dt);
                con.Close();
                return dt;

            }
            catch (Exception ex)
            {
            }
            return dt;
        }

        public DataTable GetProductCategory_SubCategory(string gender, int productFamilyID)
        {
            DataTable dt = new DataTable();
            try
            {
                string cmdText = "select pc.Name as `Category`, CONCAT(psc.ProductSubCategoryID,'_',psc.Name,'_',pc.ProductCategoryID) as `SubCategory`  from `productsubcategory` psc inner join `productcategory` pc on psc.ProductCategoryID = pc.ProductCategoryID and pc.Gender = @gender and pc.ProductfamilyID = @productFamilyID order by pc.ProductCategoryID;";
                cmd = new MySqlCommand(cmdText, con);
                cmd.Parameters.AddWithValue("@gender", gender);
                cmd.Parameters.AddWithValue("@productFamilyID", productFamilyID);
                con.Open();
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                da.Fill(dt);
                con.Close();
            }
            catch (Exception ex)
            {
                dt = new DataTable();
            }
            return dt;
        }

        public bool AddSpecification(string email, string colorSpecificationValues, string sizeSpecificaionValues,string collectionValues)
        {
            bool retVal = false;
            try
            {
                //get product id
                string cmdText = "SELECT  `ProductID` from `products` where `CreatedUser`=@email order by `CreatedDate` desc limit 1";
                cmd = new MySqlCommand(cmdText, con);
                cmd.Parameters.AddWithValue("@email", email);
                con.Open();
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                con.Close();
                if (dt.Rows.Count > 0)
                {
                    int productID = Convert.ToInt32(dt.Rows[0].ItemArray[0]);

                    if (colorSpecificationValues != "")
                    {
                        //color specification
                        string[] colors = colorSpecificationValues.Split(',');
                        for (int i = 0; i < colors.Length; i++)
                        {
                            //1 - color
                            AddSpecificationDetails(colors[i], productID, 1);
                        }
                    }
                    if (sizeSpecificaionValues != "")
                    {
                        //size specification
                        string[] sizes = sizeSpecificaionValues.Split(',');
                        for (int i = 0; i < sizes.Length; i++)
                        {
                            //2 -size
                            AddSpecificationDetails(sizes[i], productID, 2);
                        }
                    }
                    if (collectionValues != "")
                    {
                        //size specification
                        string[] tags = collectionValues.Split(',');
                        for (int i = 0; i < tags.Length; i++)
                        {
                            //3 -Tag
                            AddSpecificationDetails(tags[i], productID, 3);
                        }
                    }
                    


                }

            }
            catch (Exception ex)
            {
            }
            return retVal;
        }

        public void AddSpecificationDetails(string specVal, int productID, int specID)
        {
            try
            {
                specVal = specVal.Trim();
                if (specVal != "")
                {
                    string cmdText = "insert into `productspecifications` (`ProductID`,`SpecificationID`,`Value`) values(@productID,@specID,@specVal)";
                    cmd = new MySqlCommand(cmdText, con);
                    cmd.Parameters.AddWithValue("@productID", productID);
                    cmd.Parameters.AddWithValue("@specID", specID);
                    cmd.Parameters.AddWithValue("@specVal", specVal);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            catch (Exception ex)
            {
            }
        }

        public int ProductLimitReached(int productLimit,string email,int storeID)
        {
            int retVal = 0;
            try
            {
                //If user level


                //If store level
                string cmdText = "select count(`StoreID`) from `products` where `StoreID`=@storeID";
                cmd = new MySqlCommand(cmdText, con);
                cmd.Parameters.AddWithValue("@storeID", storeID);
                con.Open();
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                con.Close();
                // 1- Limit reached
                // 2- about to reach - nearby
                // 3- not reached limit

                if (count == (productLimit - 1))
                {
                    //If limit is nearby
                    retVal = 2;
                }
                else if (count == productLimit)
                {
                    // if count reached the level
                    retVal = 1;
                }
                else if(count<productLimit)
                {
                    //not reached 
                    retVal = 3;
                }
            }
            catch (Exception ex)
            {
            }
            return retVal;
        }
        #endregion

        public bool UpdateStoreImage(byte[] ImageData, int storeID, DateTime datemodified)
        {
            bool retVal = false;
            try
            {
                string cmdText = "SET SQL_SAFE_UPDATES = 0; update `store` set `StoreImage`=@ImageData,`last_remodel_date`=@datemodified where `store_id`=@storeID";
                cmd = new MySqlCommand(cmdText, con);
                cmd.Parameters.AddWithValue("@ImageData", ImageData);
                cmd.Parameters.AddWithValue("@datemodified", datemodified);
                cmd.Parameters.AddWithValue("@storeID", storeID);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                retVal = true;
            }
            catch (Exception ex)
            { }
            return retVal;
        }

        public bool UpdateProductImage(byte[] ImageData, string email, DateTime datemodified, int val)
        {
            bool retVal = false;
            try
            {
                string cmdText = "";
                if (val == 0)
                    cmdText = "SET SQL_SAFE_UPDATES = 0; update `products` set `image1`=@ImageData where `Email`='" + email + "' ";
                cmd = new MySqlCommand(cmdText, con);
                cmd.Parameters.AddWithValue("@ImageData", ImageData);
                cmd.Parameters.AddWithValue("@datemodified", datemodified);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                retVal = true;
            }
            catch (Exception ex)
            { }
            return retVal;
        }


        public DataTable GetStoreImage(int storeID)
        {
            bool retVal = false;
            byte[] bytes;
            //var sevenItems = new byte[];
            DataTable dt = new DataTable();
            try
            {
                string cmdText = "select `StoreImage` from `store` where `store_id`=@storeID";
                cmd = new MySqlCommand(cmdText, con);
                cmd.Parameters.AddWithValue("@storeID", storeID);
                con.Open();
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                da.Fill(dt);
                con.Close();
                //bytes = dt.Rows[0].ItemArray[0];
                //byte[] binDate = (byte[])dt.Rows["StoreImage"];
                if (dt.Rows.Count > 0)
                    retVal = false;
                else
                    retVal = true;
            }
            catch (Exception ex)
            {
            }
            return dt;
        }

        #region SUBSCRIPTION

        public int SubscriptionEmail(string email,DateTime date,int subscription)
        {
            //return types 
            //0 - something went wrong
            //1 - subscribed
            //2 - already subscribed
            //3 - unsubscribed
            int retVal = 0;
            try
            {
                
                    DataTable dt = new DataTable();
                    string cmdText = "select `Email` from `subscriptionemails` where `email`=@email";
                    cmd = new MySqlCommand(cmdText, con);
                    cmd.Parameters.AddWithValue("@email", email);
                    con.Open();
                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    da.Fill(dt);
                    con.Close();
                    if (dt.Rows.Count < 1)
                    {
                        if (subscription == 1)
                        {
                            
                            //subscription == 1 is to subscribe
                            string cmdText1 = "insert into `subscriptionemails`(`Email`,`IsSubscribed`,`SubscribedDate`) values(@email,@subscription,@date)";
                            cmd = new MySqlCommand(cmdText1, con);
                            cmd.Parameters.AddWithValue("@email", email);
                            cmd.Parameters.AddWithValue("@date", date);
                            cmd.Parameters.AddWithValue("@subscription", subscription);
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                            retVal = 1;
                        }
                        
                    }
                    else
                    {
                        //Already registered
                        retVal = 2;
                        if (subscription == 2)
                        {
                            //unsubscription
                        }
                    }
                

            }
            catch (Exception ex)
            {
                retVal = 0;
            }
            return retVal;
        }
        #endregion

    }
}