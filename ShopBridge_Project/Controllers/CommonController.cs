using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;

namespace ShopBridge_Project.Controllers
{
    public class CommonController : ApiController
    {

        [Route("api/home/inventory")]
        [HttpPost]
        public RawJsonActionResult Inventory()
        {
            dynamic jresponse = new JObject();
            var request = HttpContext.Current.Request;
            string action = request.Form["Action"];
            string itemName = request.Form["Name"];
            string description = request.Form["Description"];
            string price = request.Form["Price"];
            string ModifyItem = request.Form["ModifyItem"];
            string output = "";

            try
            {
                if (string.IsNullOrEmpty(action))
                {
                    jresponse = "{\"status\":false,\"message\":\"Action is empty\"}";
                    return new RawJsonActionResult(jresponse);
                }
                else
                {
                    if (action.ToUpper() == "ADD")
                    {
                        if (string.IsNullOrEmpty(itemName))
                        {
                            jresponse = "{\"status\":false,\"message\":\"Name is empty\"}";
                            return new RawJsonActionResult(jresponse);
                        }
                        if (string.IsNullOrEmpty(description))
                        {
                            jresponse = "{\"status\":false,\"message\":\"Description is empty\"}";
                            return new RawJsonActionResult(jresponse);
                        }
                        if (string.IsNullOrEmpty(price))
                        {
                            jresponse = "{\"status\":false,\"message\":\"Price is empty\"}";
                            return new RawJsonActionResult(jresponse);
                        }
                        SqlConnection sqlConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
                        SqlCommand sqlComm = new SqlCommand();
                        sqlComm = sqlConn.CreateCommand();
                        sqlComm.CommandText = @"INSERT INTO ShopBridgeProduct " + "(PRODUCT_NAME,PRODUCT_DESCRIPTION,PRICE) " + "VALUES(@paramName,@descr,@price)";
                        sqlComm.Parameters.Add("@paramName", SqlDbType.NVarChar);
                        sqlComm.Parameters["@paramName"].Value = itemName;
                        sqlComm.Parameters.Add("@descr", SqlDbType.NVarChar);
                        sqlComm.Parameters["@descr"].Value = description;
                        sqlComm.Parameters.Add("@price", SqlDbType.NVarChar);
                        sqlComm.Parameters["@price"].Value = price;
                        sqlConn.Open();
                        sqlComm.ExecuteNonQuery();
                        sqlConn.Close();
                        jresponse = "{\"status\":true,\"message\":\"Data Inserted Successfully\"}";
                        return new RawJsonActionResult(jresponse);
                    }
                    else if (action.ToUpper() == "MODIFY")
                    {
                        if (string.IsNullOrEmpty(itemName))
                        {
                            jresponse = "{\"status\":false,\"message\":\"Name is empty\"}";
                            return new RawJsonActionResult(jresponse);
                        }
                        SqlConnection sqlConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
                        SqlCommand sqlComm = new SqlCommand();
                        sqlComm = sqlConn.CreateCommand();
                        if (ModifyItem.ToUpper() == "PRICE")
                        {
                            sqlComm.CommandText = @"UPDATE ShopBridgeProduct SET " + ModifyItem + "=" + "'" + price + "'" + " WHERE PRODUCT_NAME=" + "'" + itemName + "'";
                        }
                        else if(ModifyItem.ToUpper() == "DESCRIPTION")
                        {
                            sqlComm.CommandText = @"UPDATE ShopBridgeProduct SET PRODUCT_DESCRIPTION=" + "'" + description + "'" + " WHERE PRODUCT_NAME=" + "'" + itemName + "'";
                        }                     
                        sqlConn.Open();
                        sqlComm.ExecuteNonQuery();
                        output = Convert.ToInt32(sqlComm.ExecuteScalar()).ToString();
                        sqlConn.Close();
                        jresponse = "{\"status\":true,\"message\":\"Data Updated Successfully\"}";
                        return new RawJsonActionResult(jresponse);
                    }
                    else if (action.ToUpper() == "DELETE")
                    {
                        if (string.IsNullOrEmpty(itemName))
                        {
                            jresponse = "{\"status\":false,\"message\":\"Name is empty\"}";
                            return new RawJsonActionResult(jresponse);
                        }
                        SqlConnection sqlConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
                        SqlCommand sqlComm = new SqlCommand();
                        sqlComm = sqlConn.CreateCommand();
                        sqlComm.CommandText = @"DELETE FROM ShopBridgeProduct WHERE PRODUCT_NAME="+ "'" + itemName + "'";
                        sqlConn.Open();
                        sqlComm.ExecuteNonQuery();
                        sqlConn.Close();
                        jresponse = "{\"status\":true,\"message\":\"Data Deleted Successfully\"}";
                        return new RawJsonActionResult(jresponse);
                    }
                    else if (action.ToUpper() == "LISTS")
                    {
                        SqlConnection sqlConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
                        SqlDataAdapter sqlAdapt = new SqlDataAdapter(@"SELECT PRODUCT_NAME FROM ShopBridgeProduct", sqlConn);
                        SqlCommandBuilder sqlCmdBuilder = new SqlCommandBuilder(sqlAdapt);
                        DataSet sqlSet = new DataSet();
                        sqlAdapt.Fill(sqlSet, "dataSetTableName");
                        sqlConn.Close();
                        string js = ConvertDataSetToJSONString(sqlSet);
                        jresponse = "{\"status\":true,\"message\":\"Data Updated Successfully\"}";
                        return new RawJsonActionResult(js);
                    }
                }
            }
            catch (Exception ex)
            {
                jresponse = "{\"status\":false,\"message\":\"Some error occurred\"}";
                return new RawJsonActionResult(jresponse);
            }
            return new RawJsonActionResult(jresponse);
        }

        public static string ConvertDataSetToJSONString(DataSet ds)
        {
            string lst = "{";
            var count = 0;
            foreach (DataTable dt in ds.Tables)
            {
                var lst1 = dt.AsEnumerable()
                .Select(r => r.Table.Columns.Cast<DataColumn>()
                 .Select(c => new KeyValuePair<string, object>(c.ColumnName, r[c.Ordinal])
                ).ToDictionary(z => z.Key, z => z.Value != null ? GetSafeString(z.Value.ToString(), true) : null)
                ).ToList();

                //now serialize it
                var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;
                if (count > 0)
                {
                    lst += "\"Table" + count + "\"" + ":" + serializer.Serialize(lst1) + ",";
                }
                else
                {
                    lst += "\"Table\"" + ":" + serializer.Serialize(lst1) + ",";
                }
                count++;
            }
            lst = lst.TrimEnd(',') + "}";
            //lst = HtmlDecodedSafeValue(lst); Coomneted by rana on 26/11/2020
            return lst;
        }

        public static string GetSafeString(string str, bool safe)
        {
            if (safe)
            {
                return HttpUtility.HtmlEncode(str);
            }
            return str;
        }

        public class RawJsonActionResult : IHttpActionResult
        {
            private readonly string _jsonString;

            public RawJsonActionResult(string jsonString)
            {
                _jsonString = jsonString;
            }

            public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
            {
                var content = new StringContent(_jsonString);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = content };
                return Task.FromResult(response);
            }
        }

    }
}