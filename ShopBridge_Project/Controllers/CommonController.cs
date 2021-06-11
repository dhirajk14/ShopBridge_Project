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
            string pageNo = request.Form["PageNumber"];
            string pageSize = request.Form["PageSize"];
            string output = "";
            string fileAsString = "";

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
                        if (HttpContext.Current.Request.Files.Count > 0)
                        {
                            for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                            {
                                HttpFileCollection files = request.Files;
                                HttpPostedFile httpfile = files[i];

                                try
                                {
                                    if (httpfile.ContentLength > 0)
                                    {

                                        byte[] fileInBytes = new byte[httpfile.ContentLength];
                                        using (BinaryReader theReader = new BinaryReader(httpfile.InputStream))
                                        {
                                            fileInBytes = theReader.ReadBytes(httpfile.ContentLength);
                                        }
                                         fileAsString = Convert.ToBase64String(fileInBytes);

                                    }
                                    else
                                    {
                                        jresponse = "{\"status\":false,\"message\":\"Image file not found\"}";
                                        return new RawJsonActionResult(jresponse);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    jresponse = "{\"status\":false,\"message\":\"Some error occurred\"}";
                                    return new RawJsonActionResult(jresponse);
                                }
                            }
                        }
                        else
                        {
                            jresponse = "{\"status\":false,\"message\":\"Image file not found\"}";
                            return new RawJsonActionResult(jresponse);
                        }
                        SqlConnection sqlConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
                        SqlCommand sqlComm = new SqlCommand();
                        sqlComm = sqlConn.CreateCommand();
                        sqlComm.CommandText = @"INSERT INTO ShopBridgeProduct " + "(PRODUCT_NAME,PRODUCT_DESCRIPTION,PRICE,IMAGE_FILE) " + "VALUES(@paramName,@descr,@price,@image)";
                        sqlComm.Parameters.Add("@paramName", SqlDbType.NVarChar);
                        sqlComm.Parameters["@paramName"].Value = itemName;
                        sqlComm.Parameters.Add("@descr", SqlDbType.NVarChar);
                        sqlComm.Parameters["@descr"].Value = description;
                        sqlComm.Parameters.Add("@price", SqlDbType.NVarChar);
                        sqlComm.Parameters["@price"].Value = price;
                        if (!string.IsNullOrEmpty(fileAsString))
                        {
                            sqlComm.Parameters.Add("@image", SqlDbType.NVarChar);
                            sqlComm.Parameters["@image"].Value = fileAsString;
                        }                      
                        sqlConn.Open();
                        sqlComm.ExecuteNonQuery();
                        sqlConn.Close();
                        jresponse = "{\"status\":true,\"message\":\"Data Inserted Successfully\"}";
                        return new RawJsonActionResult(jresponse);
                    }
                    else if (action.ToUpper() == "MODIFY")
                    {
                        fileAsString = "";
                        if (string.IsNullOrEmpty(itemName))
                        {
                            jresponse = "{\"status\":false,\"message\":\"Name is empty\"}";
                            return new RawJsonActionResult(jresponse);
                        }
                        if (HttpContext.Current.Request.Files.Count > 0)
                        {
                            for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                            {
                                HttpFileCollection files = request.Files;
                                HttpPostedFile httpfile = files[i];

                                try
                                {
                                    if (httpfile.ContentLength > 0)
                                    {

                                        byte[] fileInBytes = new byte[httpfile.ContentLength];
                                        using (BinaryReader theReader = new BinaryReader(httpfile.InputStream))
                                        {
                                            fileInBytes = theReader.ReadBytes(httpfile.ContentLength);
                                        }
                                        fileAsString = Convert.ToBase64String(fileInBytes);

                                    }
                                    else
                                    {
                                        jresponse = "{\"status\":false,\"message\":\"Image file not found\"}";
                                        return new RawJsonActionResult(jresponse);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    jresponse = "{\"status\":false,\"message\":\"Some error occurred\"}";
                                    return new RawJsonActionResult(jresponse);
                                }
                            }
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
                        else if(ModifyItem.ToUpper() == "IMAGEFILE" && !string.IsNullOrEmpty(fileAsString) )
                        {
                            sqlComm.CommandText = @"UPDATE ShopBridgeProduct SET IMAGE_FILE=" + "'" + fileAsString + "'" + " WHERE PRODUCT_NAME=" + "'" + itemName + "'";
                        }
                        else
                        {
                            jresponse = "{\"status\":true,\"message\":\"Invalid Request.Select field to modify\"}";
                            return new RawJsonActionResult(jresponse);
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
                        if (string.IsNullOrEmpty(pageNo))
                        {
                            jresponse = "{\"status\":false,\"message\":\"PageNumber is empty\"}";
                            return new RawJsonActionResult(jresponse);
                        }
                        else if (string.IsNullOrEmpty(pageSize))
                        {
                            jresponse = "{\"status\":false,\"message\":\"PageSize is empty\"}";
                            return new RawJsonActionResult(jresponse);
                        }
                        SqlConnection sqlConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
                        String SQL = "SELECT ID,PRODUCT_NAME,PRODUCT_DESCRIPTION,PRICE FROM ShopBridgeProduct";
                        String SQLOrderBy = "ORDER BY ID ASC "; //GetOrderByClause(Object someInputParams);
                        String limitedSQL = GetPaginatedSQL((Convert.ToInt32(pageNo)-1)* Convert.ToInt32(pageSize), Convert.ToInt32(pageSize), SQL, SQLOrderBy);
                        SqlDataAdapter sqlAdapt = new SqlDataAdapter(limitedSQL, sqlConn);
                        SqlCommandBuilder sqlCmdBuilder = new SqlCommandBuilder(sqlAdapt);
                        DataSet sqlSet = new DataSet();
                        sqlAdapt.Fill(sqlSet, "dataSetTableName");
                        sqlConn.Close();
                        string js = ConvertDataSetToJSONString(sqlSet,pageNo,pageSize);
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

        public static string GetPaginatedSQL(int startRow, int numberOfRows, string sql, string orderingClause)
        {
            if (numberOfRows <= 0)
            {
                return String.Format("{0} {1}", sql, orderingClause);
            }
            String partialSQL = sql.Remove(0, "SELECT ".Length);

            return String.Format(
                "SELECT * FROM ( SELECT ROW_NUMBER() OVER ({0}) AS ROW_NO, {1} ) AS SUB WHERE ROW_NO > {2} AND ROW_NO <= {3}",
                orderingClause,
                partialSQL,
                startRow.ToString(),
                (startRow + numberOfRows).ToString()
            );
        }

        public static string ConvertDataSetToJSONString(DataSet ds,string pageno,string pagesize)
        {
            string lst = "{"+ "\"PageNo\":\"" + pageno + "\"," + "\"PageSize\":\"" + pagesize + "\",";
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
                    lst += "\"ProductList" + count + "\"" + ":" + serializer.Serialize(lst1) + ",";
                }
                else
                {
                    lst += "\"ProductList\"" + ":" + serializer.Serialize(lst1) + ",";
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
