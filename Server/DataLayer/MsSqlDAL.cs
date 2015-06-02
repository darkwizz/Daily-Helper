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

        private static Dictionary<SocialNetworkAccounts, Guid> _accountsIds = null;

        static MsSqlDAL()
        {
            _database = new DataSet("dbDailyHelper");
            _connection = new SqlConnection(_connectionString);

            _userAdapter = new SqlDataAdapter(@"SELECT * FROM tbUser", _connectionString);
            _todoAdapter = new SqlDataAdapter(@"SELECT * FROM tbTodo", _connectionString);
            _noteAdapter = new SqlDataAdapter(@"SELECT * FROM tbNote", _connectionString);
            _socialNetAccountsAdapter = new SqlDataAdapter(@"SELECT * FROM tbAccountInfo", _connectionString);

            SqlCommandBuilder _userCommandBuilder = new SqlCommandBuilder(_userAdapter);
            SqlCommandBuilder _todoCommandBuilder = new SqlCommandBuilder(_todoAdapter);
            SqlCommandBuilder _noteCommandBuilder = new SqlCommandBuilder(_noteAdapter);
            SqlCommandBuilder _socialNetCommandBuilder = new SqlCommandBuilder(_socialNetAccountsAdapter);

            _userAdapter.Fill(_database, "tbUser");
            _todoAdapter.Fill(_database, "tbTodo");
            _noteAdapter.Fill(_database, "tbNote");
            _socialNetAccountsAdapter.Fill(_database, "tbAccountInfo");

            FillAccountsMappings();
            FillDbRelations();
        }

        private static void FillDbRelations()
        {
            DataRelation todoUser = new DataRelation("TodoUser", _database.Tables["tbUser"].Columns["Id"],
                                                                 _database.Tables["tbTodo"].Columns["IdUser"]);
            DataRelation noteUser = new DataRelation("NoteUser", _database.Tables["tbUser"].Columns["Id"],
                                                                 _database.Tables["tbNote"].Columns["IdUser"]);
            DataRelation accountsInfoUser = new DataRelation("AccountInfoUser", _database.Tables["tbUser"].Columns["Id"],
                                                                                 _database.Tables["tbAccountInfo"].Columns["IdUser"]);
            _database.Relations.AddRange(new DataRelation[] { todoUser, noteUser, accountsInfoUser });
        }

        private static void FillAccountsMappings()
        {
            _accountsIds = new Dictionary<SocialNetworkAccounts, Guid>();
            SqlDataAdapter tempAdapter = new SqlDataAdapter(@"SELECT * FROM tbAccount", _connectionString);
            DataTable tempTable = new DataTable("Account");
            tempAdapter.Fill(tempTable);
            foreach (DataRow row in tempTable.Rows)
            {
                Guid id = (Guid)row["Id"];
                string accountName = (string)row["Account"];
                SocialNetworkAccounts account = (SocialNetworkAccounts)Enum.Parse(typeof(SocialNetworkAccounts), accountName);
                _accountsIds.Add(account, id);
            }
        }

        public User GetUser(string email)
        {
            User user = new User();
            DataRow[] rows = _database.Tables["tbUser"].Select(string.Format("Email='{0}'", email));
            if (rows == null || rows.Length == 0)
            {
                return null;
            }
            Guid id = (Guid)rows[0]["Id"];
            string password = (string)rows[0]["Pass"];

            user.Id = id;
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
                row["Pass"] = user.Password;
                _database.Tables["tbUser"].Rows.Add(row);
                _userAdapter.Update(_database.Tables["tbUser"]); //_database, "tbUser"
                //_database.AcceptChanges();
            }
            catch (SqlException ex) // if such email already exists
            {
                _database.RejectChanges();
                throw ex;
            }
        }

        /// <summary>
        /// Pushes all local changes to database
        /// </summary>
        private void Update()
        {
            
            _noteAdapter.Update(_database.Tables["tbNote"]); //_database, "tbNote"
            _todoAdapter.Update(_database.Tables["tbTodo"]); //_database, "tbTodo"
            _socialNetAccountsAdapter.Update(_database.Tables["tbAccountInfo"]); //_database, "tbAccountInfo"
        }


        public void SaveNote(User user, Note note)
        {
        }

        public void RemoveNote(User user, Note note)
        {
        }

        public void UpdateNote(User user, Note note)
        {
        }

        public void SaveTodoItem(User user, TodoItem item)
        {
        }

        public void RemoveTodoItem(User user, TodoItem item)
        {
        }

        public void SaveAccountInfo(User user, SocialNetworkAccountInfo info)
        {
        }
    }
}
