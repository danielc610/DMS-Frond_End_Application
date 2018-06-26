using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace DMS_FEA.ATAT_Lib
{
    public class ATAT_Lib
    {
        public string PasswordHashing(string password)
        {
            //Password hashing

            string salt = "Asi@nTaTLimitEd";
            SHA256 sha256 = new SHA256CryptoServiceProvider();
            string salted = salt + password;
            byte[] pass = Encoding.Default.GetBytes(salted);
            byte[] hashPass = sha256.ComputeHash(pass);
            string result = Convert.ToBase64String(hashPass);

            return result;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="sp">
        /// Stored Procedure Name
        /// </param>
        /// <param name="dict">
        /// Stored Procedure Parameter 
        /// </param>
        /// <returns>
        /// DataTable with results
        /// </returns>
        public static DataTable GetData(string sp, Dictionary<string, string> dict)
        {
            var sda = new SqlDataAdapter(sp, ConfigurationManager.ConnectionStrings["DMSDBContext"].ConnectionString);

            foreach(KeyValuePair<string, string> pair in dict)
            {
                sda.SelectCommand.Parameters.AddWithValue(pair.Key, pair.Value);
            }
            sda.SelectCommand.CommandType = CommandType.StoredProcedure;
            var dt = new DataTable();
            sda.Fill(dt);
            return dt;
        }



    }
}