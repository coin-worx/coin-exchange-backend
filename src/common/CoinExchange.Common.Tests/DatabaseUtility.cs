using System;
using System.IO;
using MySql.Data.MySqlClient;

namespace CoinExchange.Common.Tests
{
    /// <summary>
    /// Database utility class to run script
    /// </summary>
    public class DatabaseUtility
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
            (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private string _connectionString;
        private MySqlConnection _mySqlConnection;
        private string _filePath = Path.GetFullPath(@"~\..\..\..\..\..\Data\MySql\");
        public DatabaseUtility(string connectionString)
        {
            _connectionString = connectionString;
            _mySqlConnection=new MySqlConnection(_connectionString);
        }

        /// <summary>
        /// Run create sql script
        /// </summary>
        public void Create()
        {
            string script = File.ReadAllText(_filePath+"create.sql");
            ExecuteScript(script);
        }

        /// <summary>
        /// Run drop sql script
        /// </summary>
        public void Drop()
        {
            string script = File.ReadAllText(_filePath+"drop.sql");
            ExecuteScript(script);
        }

        /// <summary>
        /// Populate the database with master data.
        /// </summary>
        public void Populate()
        {
            string script = File.ReadAllText(_filePath+"populate.sql");
            ExecuteScript(script);
        }

        /// <summary>
        /// Execute sql script
        /// </summary>
        private void ExecuteScript(string script)
        {
            try
            {
                MySqlCommand command = new MySqlCommand(script, _mySqlConnection);
                _mySqlConnection.Open();
                command.ExecuteNonQuery();
                _mySqlConnection.Close();
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Execute Script Exception:",exception);
                }
            }
            finally
            {
                _mySqlConnection.Close();
            }
        }
    }
}
