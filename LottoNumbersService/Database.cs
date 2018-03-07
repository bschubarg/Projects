// Lotto Numbers Service and Application.
// Open source.  This is a project to demonstrate 
// the various technologies to use when gathering data
// and publishing data by various means.  I created
// this project only for my personal use.  Any alterations
// by others is welcomed.
// 
// I do not pretend to be an expert on these technologies
// but rather a demonstration of my approach to satisfy
// certain requirements.
// 
// Acknowledgments: https://github.com/rubicon-oss/LicenseHeaderManager/wiki - License Header Snippet
//					https://www.codeproject.com/Articles/1041115/Webscraping-with-Csharp - ScrapySharp
// 
// Copyright (c) 2016 William Schubarg
using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Configuration;

namespace LottoNumbersService
{
    public static class Database
    {
        internal static string GetSqlConnectionString()
        {
            try
            {
                ConnectionStringSettings mySetting = ConfigurationManager.ConnectionStrings["LottoNumbers"];

                if (mySetting == null)
                    throw new Exception("Database connection settings have not been set in Connect.config file");

                return mySetting.ConnectionString;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        internal static bool TestSqlConnectionString()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection())
                {                    
                    conn.ConnectionString = GetSqlConnectionString();

                    conn.Open();                    
                }               

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// The Using statements will call the Dispose for me instead of have to directly call the IDisposal
        /// </summary>
        internal static int ExecuteNonQuery(string sp, SqlParameter[] parameters = null)
        {
            try
            {
                // Assuming that the Connection String is valid due to the TestSqlConnectionString method
                using (SqlConnection conn = new SqlConnection(GetSqlConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        if (parameters != null)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }

                        cmd.CommandText = sp;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = conn;

                        conn.Open();

                        // execute the command
                        int result = cmd.ExecuteNonQuery();

                        conn.Close();

                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        internal static DataTable ExecuteScalar(string sp, SqlParameter[] parameters = null)
        {
            DataTable dt = new DataTable();

            // Assuming that the Connection String is valid due to the TestSqlConnectionString method
            using (SqlConnection conn = new SqlConnection(GetSqlConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    cmd.CommandText = sp;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = conn;

                    conn.Open();

                    // execute the command
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {                        
                        dt.Load(rdr);
                    }

                    conn.Close();                   
                }                                
            }

            return dt;
        }
        /// <summary>
        /// The Using statements will call the Dispose for me instead of have to directly call the IDisposal
        /// sp: Stored Procedure
        /// tbl: Table name
        /// obj: List of entities to insert to tbl.
        /// return: Count of the rows in the dt(DataTable).
        /// </summary>
        internal static int InsertEntities<T>(string tbl, DataTable dt)
        {
            try
            {
                // Determine if there are any records to insert in the first place.
                if (dt == null || dt.Rows.Count == 0)
                    return 0;

                // Assuming that the Connection String is valid due to the TestSqlConnectionString method
                using (SqlConnection conn = new SqlConnection(GetSqlConnectionString()))
                {
                    conn.Open();                        

                    // Create the SqlBulkCopy object.                     
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
                    {
                        bulkCopy.DestinationTableName = tbl;

                        try
                        {
                            foreach (var column in dt.Columns)
                                bulkCopy.ColumnMappings.Add(column.ToString(), column.ToString());

                            // Write from the source to the destination.
                            // SQL Table constraint of 'CREATE UNIQUE INDEX UniqueIndexOnHashCode ON LottoNumber(HashCode) WITH IGNORE_DUP_KEY'
                            // to ignore duplicate entries
                            bulkCopy.WriteToServer(dt);

                            return dt.Rows.Count;
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message);
                        }
                    }                    
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// The Using statements will call the Dispose for me instead of have to directly call the IDisposal
        /// </summary>
        /// <param name="sp">Stored Proc to run</param>
        /// <param out name="obj">List of the retrieved rows from Stored Proc.</param>
        internal static int LoadEntities<T>(string sp, ref List<T> obj, List<Tuple<int, string, int>> parameters = null)
        {
            try
            {
                obj = null;
                var sqlParameters = new List<SqlParameter>();

                if (parameters != null)
                {
                    sqlParameters.AddRange(parameters.Select(p => new SqlParameter()
                    {
                        SqlDbType = (SqlDbType) p.Item1,
                        ParameterName = "@" + p.Item2.ToString(),
                        Value = p.Item3
                    }));
                }

                using (SqlConnection conn = new SqlConnection(GetSqlConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {

                        cmd.CommandText = sp;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = conn;

                        if (sqlParameters.Count != 0)
                            cmd.Parameters.AddRange(sqlParameters.ToArray());                        

                        conn.Open();

                        try
                        {
                            // execute the command
                            using (SqlDataReader rdr = cmd.ExecuteReader())
                            {
                                DataTable dt = new DataTable();
                                dt.Load(rdr);

                                obj = dt.ToListof<T>();

                                return obj.Count;
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message);
                        }
                    }
                }                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }            
        }
   
        internal static List<T> ToListof<T>(this DataTable dt)
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;

            var columnNames = dt.Columns.Cast<DataColumn>()
                .Select(c => c.ColumnName)
                .ToList();

            var objectProperties = typeof(T).GetProperties(flags);
            var targetList = dt.AsEnumerable().Select(dataRow =>
            {
                var instanceOfT = Activator.CreateInstance<T>();

                foreach (var properties in objectProperties.Where(properties => columnNames.Contains(properties.Name) && dataRow[properties.Name] != DBNull.Value))
                {
                    properties.SetValue(instanceOfT, dataRow[properties.Name], null);
                }
                
                return instanceOfT;

            }).ToList();

            return targetList;
        }
        /// <summary>
        /// Get the schema of the List that corresponds to the schema of the SQL table.
        /// Create Datatable for bulk insert.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        internal static DataTable ConvertToDataTable<T>(this IEnumerable<T> data)
        {
            PropertyDescriptorCollection props =
                 TypeDescriptor.GetProperties(typeof(T));

            DataTable table = new DataTable();

            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    table.Columns.Add(prop.Name, prop.PropertyType.GetGenericArguments()[0]);
                else
                    table.Columns.Add(prop.Name, prop.PropertyType);
            }

            var values = new object[props.Count];

            foreach (T item in data)
            {
                for (var i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                
                table.Rows.Add(values);
            }
            
            return table;              
        }
    }
}
