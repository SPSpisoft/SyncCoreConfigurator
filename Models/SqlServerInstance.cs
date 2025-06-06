using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.Sql;
using System.Linq;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Management;
using System.Threading.Tasks;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;

namespace SyncCore.Models
{
    /// <summary>
    /// Represents a SQL Server instance and its connection details
    /// </summary>
    /// <remarks>
    /// کلاس نمایش‌دهنده یک نمونه SQL Server و جزئیات اتصال به آن
    /// </remarks>
    public class SqlServerInstance
    {
        /// <summary>
        /// Server name or instance name
        /// </summary>
        /// <remarks>
        /// نام سرور یا نمونه
        /// </remarks>
        public string ServerName { get; set; }

        /// <summary>
        /// Instance name
        /// </summary>
        /// <remarks>
        /// نام نمونه
        /// </remarks>
        public string InstanceName { get; set; }

        /// <summary>
        /// SQL Server version
        /// </summary>
        /// <remarks>
        /// نسخه SQL Server
        /// </remarks>
        public string Version { get; set; }

        /// <summary>
        /// Indicates if the server is local
        /// </summary>
        /// <remarks>
        /// نشان‌دهنده اینکه آیا سرور محلی است
        /// </remarks>
        public bool IsLocal { get; set; }

        /// <summary>
        /// Full name of the SQL Server instance
        /// </summary>
        /// <remarks>
        /// نام کامل نمونه SQL Server
        /// </remarks>
        public string FullName => string.IsNullOrEmpty(InstanceName) ? ServerName : $"{ServerName}\\{InstanceName}";

        /// <summary>
        /// Username for SQL authentication
        /// </summary>
        /// <remarks>
        /// نام کاربری برای احراز هویت SQL
        /// </remarks>
        public string Username { get; set; }

        /// <summary>
        /// Password for SQL authentication
        /// </summary>
        /// <remarks>
        /// رمز عبور برای احراز هویت SQL
        /// </remarks>
        public string Password { get; set; }

        /// <summary>
        /// Indicates if Windows authentication is used
        /// </summary>
        /// <remarks>
        /// نشان‌دهنده استفاده از احراز هویت ویندوز
        /// </remarks>
        public bool UseWindowsAuth { get; set; }

        /// <summary>
        /// Database name to connect to
        /// </summary>
        /// <remarks>
        /// نام پایگاه داده برای اتصال
        /// </remarks>
        public string DatabaseName { get; set; }

        /// <summary>
        /// List of available SQL Server instances
        /// </summary>
        /// <remarks>
        /// لیست نمونه‌های SQL Server موجود
        /// </remarks>
        public List<string> AvailableServers { get; private set; }

        /// <summary>
        /// List of available databases
        /// </summary>
        /// <remarks>
        /// لیست پایگاه‌های داده موجود
        /// </remarks>
        public List<string> AvailableDatabases { get; private set; }

        public SqlServerInstance()
        {
            LoadAvailableServers();
            AvailableDatabases = new List<string>();
        }

        /// <summary>
        /// Loads available SQL Server instances
        /// </summary>
        /// <remarks>
        /// بارگذاری نمونه‌های SQL Server موجود
        /// </remarks>
        private void LoadAvailableServers()
        {
            try
            {
                var servers = new List<string>();
                // Add local server
                servers.Add(Environment.MachineName);

                // تلاش با SqlDataSourceEnumerator
                try
                {
                    var enumerator = SqlDataSourceEnumerator.Instance;
                    var dataTable = enumerator.GetDataSources();
                    foreach (DataRow row in dataTable.Rows)
                    {
                        string serverName = row["ServerName"].ToString();
                        string instanceName = row["InstanceName"].ToString();
                        string fullName;
                        if (string.IsNullOrEmpty(instanceName) || instanceName.ToUpper() == "MSSQLSERVER")
                        {
                            fullName = serverName;
                        }
                        else
                        {
                            fullName = $"{serverName}\\{instanceName}";
                        }
                        if (!servers.Contains(fullName))
                        {
                            servers.Add(fullName);
                        }
                    }
                }
                catch
                {
                    // اگر SqlDataSourceEnumerator جواب نداد، از WMI استفاده کن
                    try
                    {
                        using (var searcher = new ManagementObjectSearcher(
                            "SELECT * FROM Win32_Service WHERE Name LIKE 'MSSQL$%' OR Name = 'MSSQLSERVER'"))
                        {
                            foreach (ManagementObject service in searcher.Get())
                            {
                                string serviceName = service["Name"].ToString();
                                string instanceName;
                                if (serviceName == "MSSQLSERVER")
                                {
                                    instanceName = Environment.MachineName;
                                }
                                else
                                {
                                    string actualInstanceName = serviceName.Replace("MSSQL$", "");
                                    instanceName = $"{Environment.MachineName}\\{actualInstanceName}";
                                }
                                if (!servers.Contains(instanceName))
                                {
                                    servers.Add(instanceName);
                                }
                            }
                        }
                    }
                    catch
                    {
                        // اگر هیچ روشی جواب نداد، فقط سرور لوکال رو نمایش بده
                    }
                }
                AvailableServers = servers.Distinct().ToList();
            }
            catch
            {
                AvailableServers = new List<string>();
            }
        }

        /// <summary>
        /// Loads available databases for the current server
        /// </summary>
        /// <remarks>
        /// بارگذاری پایگاه‌های داده موجود برای سرور فعلی
        /// </remarks>
        public void LoadAvailableDatabases()
        {
            try
            {
                var databases = new List<string>();
                using (var connection = new SqlConnection(BuildConnectionString()))
                {
                    connection.Open();
                    var command = new SqlCommand(@"
                        SELECT name 
                        FROM sys.databases 
                        WHERE database_id > 4 
                        AND state_desc = 'ONLINE'
                        ORDER BY name", connection);
                    
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string dbName = reader.GetString(0);
                            if (!string.IsNullOrEmpty(dbName))
                            {
                                databases.Add(dbName);
                            }
                        }
                    }
                }
                AvailableDatabases = databases;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading databases: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                AvailableDatabases = new List<string>();
            }
        }

        /// <summary>
        /// Builds the connection string for this instance
        /// </summary>
        /// <returns>Connection string</returns>
        /// <remarks>
        /// ساخت رشته اتصال برای این نمونه
        /// </remarks>
        public string BuildConnectionString()
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = ServerName,
                IntegratedSecurity = UseWindowsAuth
            };

            if (!UseWindowsAuth)
            {
                builder.UserID = Username;
                builder.Password = Password;
            }

            if (!string.IsNullOrEmpty(DatabaseName))
            {
                builder.InitialCatalog = DatabaseName;
            }

            return builder.ToString();
        }

        /// <summary>
        /// Tests the connection to the SQL Server instance
        /// </summary>
        /// <returns>True if connection is successful</returns>
        /// <remarks>
        /// تست اتصال به نمونه SQL Server
        /// </remarks>
        public bool TestConnection()
        {
            try
            {
                using (var connection = new SqlConnection(BuildConnectionString()))
                {
                    connection.Open();
                    LoadAvailableDatabases();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public override string ToString()
        {
            return FullName;
        }

        public static async Task<List<SqlServerInstance>> GetAvailableServers()
        {
            return await Task.Run(() =>
            {
                var servers = new List<SqlServerInstance>();
                try
                {
                    // Add local server first
                    servers.Add(new SqlServerInstance
                    {
                        ServerName = Environment.MachineName,
                        IsLocal = true
                    });
                    // تلاش با SqlDataSourceEnumerator
                    try
                    {
                        var enumerator = SqlDataSourceEnumerator.Instance;
                        var dataTable = enumerator.GetDataSources();
                        foreach (DataRow row in dataTable.Rows)
                        {
                            string serverName = row["ServerName"].ToString();
                            string instanceName = row["InstanceName"].ToString();
                            string fullName;
                            if (string.IsNullOrEmpty(instanceName) || instanceName.ToUpper() == "MSSQLSERVER")
                            {
                                fullName = serverName;
                            }
                            else
                            {
                                fullName = $"{serverName}\\{instanceName}";
                            }
                            if (!servers.Any(s => s.ServerName == fullName))
                            {
                                servers.Add(new SqlServerInstance
                                {
                                    ServerName = fullName,
                                    InstanceName = (string.IsNullOrEmpty(instanceName) || instanceName.ToUpper() == "MSSQLSERVER") ? null : instanceName,
                                    IsLocal = serverName == Environment.MachineName
                                });
                            }
                        }
                    }
                    catch
                    {
                        // اگر SqlDataSourceEnumerator جواب نداد، از WMI استفاده کن
                        try
                        {
                            using (var searcher = new ManagementObjectSearcher(
                                "SELECT * FROM Win32_Service WHERE Name LIKE 'MSSQL$%' OR Name = 'MSSQLSERVER'"))
                            {
                                foreach (ManagementObject service in searcher.Get())
                                {
                                    string serviceName = service["Name"].ToString();
                                    string instanceName;
                                    if (serviceName == "MSSQLSERVER")
                                    {
                                        instanceName = Environment.MachineName;
                                    }
                                    else
                                    {
                                        string actualInstanceName = serviceName.Replace("MSSQL$", "");
                                        instanceName = $"{Environment.MachineName}\\{actualInstanceName}";
                                    }
                                    if (!servers.Any(s => s.ServerName == instanceName))
                                    {
                                        servers.Add(new SqlServerInstance
                                        {
                                            ServerName = instanceName,
                                            InstanceName = serviceName == "MSSQLSERVER" ? null : serviceName.Replace("MSSQL$", ""),
                                            IsLocal = true
                                        });
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error finding SQL Server instances: {ex.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error finding SQL Server instances: {ex.Message}");
                }
                return servers;
            });
        }
    }
} 