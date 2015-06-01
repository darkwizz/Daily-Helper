using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Entities;

namespace Server.DataLayer
{
    class MsSqlDAL: IDAL
    {
        private static string _connectionString = @"Data Source=ARTUR-PC\SQLSERVER; Database=dbDailyHelper; " +
                                                    @"Integrated Security=SSPI"; // SSPI <=> true
        private static SqlConnection _connection = null;
        private static DataSet _database = null;

        private static SqlDataAdapter _userAdapter = null;
        private static SqlDataAdapter _todoAdapter = null;
        private static SqlDataAdapter _noteAdapter = null;
        private static SqlDataAdapter _socialNetAccountsAdapter = null;

        private static SqlCommandBuilder _userCommandBuilder = null;
        private static SqlCommandBuilder _todoCommandBuilder = null;
        private static SqlCommandBuilder _noteCommandBuilder = null;
        private static SqlCommandBuilder _socialNetCommandBuilder = null;

        private static Dictionary<SocialNetworkAccounts, int> _accountsIds = null;

        static MsSqlDAL()
        {
            _connection = new SqlConnection(_connectionString);

            _userAdapter = new SqlDataAdapter(@"SELECT * FROM tbUser", _connectionString);
            _todoAdapter = new SqlDataAdapter(@"SELECT * FROM tbTodo", _connectionString);
            _noteAdapter = new SqlDataAdapter(@"SELECT * FROM tbNote", _connectionString);
            _socialNetAccountsAdapter = new SqlDataAdapter(@"SELECT * FROM tbAccountsInfo", _connectionString);

            _userCommandBuilder = new SqlCommandBuilder(_userAdapter);
            _todoCommandBuilder = new SqlCommandBuilder(_todoAdapter);
            _noteCommandBuilder = new SqlCommandBuilder(_noteAdapter);
            _socialNetCommandBuilder = new SqlCommandBuilder(_socialNetAccountsAdapter);

            //User table updates
            _userAdapter.InsertCommand = _userCommandBuilder.GetInsertCommand(true);
            _userAdapter.UpdateCommand = _userCommandBuilder.GetUpdateCommand(true);
            _userAdapter.DeleteCommand = _userCommandBuilder.GetDeleteCommand(true);

            //Note table updates
            _noteAdapter.InsertCommand = _noteCommandBuilder.GetInsertCommand(true);
            _noteAdapter.UpdateCommand = _noteCommandBuilder.GetUpdateCommand(true);
            _noteAdapter.DeleteCommand = _noteCommandBuilder.GetDeleteCommand(true);

            //Todo table updates
            _todoAdapter.InsertCommand = _todoCommandBuilder.GetInsertCommand(true);
            _todoAdapter.UpdateCommand = _todoCommandBuilder.GetUpdateCommand(true);
            _todoAdapter.DeleteCommand = _todoCommandBuilder.GetDeleteCommand(true);

            //Social network accounts table updates
            _socialNetAccountsAdapter.InsertCommand = _socialNetCommandBuilder.GetInsertCommand(true);
            _socialNetAccountsAdapter.UpdateCommand = _socialNetCommandBuilder.GetUpdateCommand(true);
            _socialNetAccountsAdapter.DeleteCommand = _socialNetCommandBuilder.GetDeleteCommand(true);

            _userAdapter.Fill(_database, "tbUser");
            _todoAdapter.Fill(_database, "tbTodo");
            _noteAdapter.Fill(_database, "tbNote");
            _socialNetAccountsAdapter.Fill(_database, "tbAccountsInfo");

            FillAccountsMappings();
        }

        private static void FillAccountsMappings()
        {
            _accountsIds = new Dictionary<SocialNetworkAccounts, int>();
            SqlDataAdapter tempAdapter = new SqlDataAdapter(@"SELECT * FROM tbAccount", _connectionString);
            DataTable tempTable = new DataTable("tbAccount");
            tempAdapter.Fill(tempTable);
            foreach (DataRow row in tempTable.Rows)
            {
                int id = (int)row["Id"];
                string accountName = (string)row["Account"];
                SocialNetworkAccounts account = (SocialNetworkAccounts)Enum.Parse(typeof(SocialNetworkAccounts), accountName);
                _accountsIds.Add(account, id);
            }
        }

        public User GetUser(string email)
        {
            User user = new User();
            DataRow[] rows = _database.Tables["tbUser"].Select("Email=");
            if (rows == null || rows.Length == 0)
            {
                return null;
            }
            string id = (string)rows[0]["Id"];
            string password = (string)rows[0]["Password"];

            user.Id = Guid.Parse(id);
            user.Email = email;
            user.Password = password;

            return user;
        }

        /// <summary>
        /// Save new user into database. If such user already exists, then throw new <code>SqlException</code>
        /// </summary>
        /// <param name="user">User to save in DB</param>
        public void SaveUser(User user)
        {
            try
            {
                DataRow row = _database.Tables["tbUser"].NewRow();
                row["Id"] = user.Id;
                row["Email"] = user.Email;
                row["Password"] = user.Password;
                _database.Tables["tbUser"].Rows.Add(row);
                _database.AcceptChanges();
            }
            catch (SqlException ex)
            {
                _database.RejectChanges();
                throw ex;
            }
        }
    }
}
