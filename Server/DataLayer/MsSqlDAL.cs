using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Server.Entities;
using Server.Faults;

namespace Server.DataLayer
{
    class MsSqlDAL: IDAL
    {
        #region FIELDS
        private static string _connectionString = @"Data Source=ARTUR-PC\SQLSERVER; Database=dbDailyHelper; " +
                                                    @"Integrated Security=SSPI"; // SSPI <=> true
        private static SqlConnection _connection = null;
        private static DataSet _database = null;

        private static SqlDataAdapter _userAdapter = null;
        private static SqlDataAdapter _todoAdapter = null;
        private static SqlDataAdapter _noteAdapter = null;
        private static SqlDataAdapter _socialNetAccountsAdapter = null;
        private static SqlDataAdapter _scheduleItemAdapter = null;

        private static Dictionary<SocialNetworkAccounts, Guid> _accountsIds = null;
        private static Dictionary<Guid, SocialNetworkAccounts> _idsAccounts = null;

        private static bool _areTablesFilled = false;
        #endregion

        static MsSqlDAL()
        {
            _database = new DataSet("dbDailyHelper");
            _connection = new SqlConnection(_connectionString);

            _userAdapter = new SqlDataAdapter(@"SELECT * FROM tbUser", _connectionString);
            _todoAdapter = new SqlDataAdapter(@"SELECT * FROM tbTodo", _connectionString);
            _noteAdapter = new SqlDataAdapter(@"SELECT * FROM tbNote", _connectionString);
            _socialNetAccountsAdapter = new SqlDataAdapter(@"SELECT * FROM tbAccountInfo", _connectionString);
            _scheduleItemAdapter = new SqlDataAdapter(@"SELECT * FROM tbScheduleItem", _connectionString);

            SqlCommandBuilder _userCommandBuilder = new SqlCommandBuilder(_userAdapter);
            SqlCommandBuilder _todoCommandBuilder = new SqlCommandBuilder(_todoAdapter);
            SqlCommandBuilder _noteCommandBuilder = new SqlCommandBuilder(_noteAdapter);
            SqlCommandBuilder _socialNetCommandBuilder = new SqlCommandBuilder(_socialNetAccountsAdapter);
            SqlCommandBuilder _scheduleItemCommandBuilder = new SqlCommandBuilder(_scheduleItemAdapter);

            try
            {
                FillTables();
            }
            catch (SqlException ex)
            {                
                if (!ConnectionFault(ex)) // For debugging
                {
                    throw ex;
                }
                Console.WriteLine("Connection timeout error. Server is unavailable");
            }
        }

        private static bool ConnectionFault(SqlException ex)
        {
            for (int i = 0; i < ex.Errors.Count; i++)
            {
                if (ex.Errors[i].Number == -2) // if connection timeout error
                {
                    return true;
                }
            }
            return false;
        }

        private static void FillTables()
        {
            _database.Clear();
            _database.Tables.Clear();

            _userAdapter.Fill(_database, "tbUser");
            _todoAdapter.Fill(_database, "tbTodo");
            _noteAdapter.Fill(_database, "tbNote");
            _socialNetAccountsAdapter.Fill(_database, "tbAccountInfo");
            _scheduleItemAdapter.Fill(_database, "tbScheduleItem");

            FillAccountsMappings();
            FillDbRelations();

            _areTablesFilled = true;
        }

        private static void FillDbRelations()
        {
            DataRelation todoUser = new DataRelation("TodoUser", _database.Tables["tbUser"].Columns["Id"],
                                                                 _database.Tables["tbTodo"].Columns["IdUser"]);
            DataRelation noteUser = new DataRelation("NoteUser", _database.Tables["tbUser"].Columns["Id"],
                                                                 _database.Tables["tbNote"].Columns["IdUser"]);
            DataRelation accountsInfoUser = new DataRelation("AccountInfoUser", _database.Tables["tbUser"].Columns["Id"],
                                                                                 _database.Tables["tbAccountInfo"].Columns["IdUser"]);
            DataRelation scheduleItemUser = new DataRelation("ScheduleItemUser", _database.Tables["tbUser"].Columns["Id"],
                                                                                 _database.Tables["tbScheduleItem"].Columns["IdUser"]);
            _database.Relations.AddRange(new DataRelation[] { todoUser, noteUser, accountsInfoUser, scheduleItemUser });
        }

        private static void FillAccountsMappings()
        {
            _accountsIds = new Dictionary<SocialNetworkAccounts, Guid>();
            _idsAccounts = new Dictionary<Guid, SocialNetworkAccounts>();
            SqlDataAdapter tempAdapter = new SqlDataAdapter(@"SELECT * FROM tbAccount", _connectionString);
            DataTable tempTable = new DataTable("Account");
            tempAdapter.Fill(tempTable);
            foreach (DataRow row in tempTable.Rows)
            {
                Guid id = (Guid)row["Id"];
                string accountName = (string)row["Account"];
                SocialNetworkAccounts account = (SocialNetworkAccounts)Enum.Parse(typeof(SocialNetworkAccounts), accountName);
                _accountsIds.Add(account, id);
                _idsAccounts.Add(id, account);
            }
        }

        #region USER
        public User GetUser(string email)
        {
            try
            {
                if (!_areTablesFilled)
                {
                    FillTables();
                }

                User user = new User();
                DataRow[] rows = _database.Tables["tbUser"].Select(string.Format("Email = '{0}'", email));
                if (rows == null || rows.Length == 0)
                {
                    return null;
                }
                Guid id = (Guid)rows[0]["Id"];
                string password = (string)rows[0]["Pass"];

                user.Id = id;
                user.Email = email;
                user.Password = password;
                // Here all dictionaries will be null to make prevent reading unnecessary fields;
                // when they will be needed, the Client call appropriate method explicitly
                user.Notes = new Dictionary<Guid,Note>();
                user.TodoItems = new Dictionary<Guid,TodoItem>();
                user.Accounts = new Dictionary<Guid,SocialNetworkAccountInfo>();
                user.ScheduleItems = new Dictionary<Guid,OnceRunningScheduleItem>();

                return user;
            }
            catch (SqlException ex)
            {
                if (!ConnectionFault(ex))
                {
                    Console.WriteLine(ex.Message); // logging
                    throw; // <=> throw ex
                }
                throw new FaultException<DatabaseConnectionFault>(new DatabaseConnectionFault()
                    {
                        ErrorMessage = "Database connection timeout is ended",
                        FullDescription = ex.Message,
                        InnerException = ex
                    });
            }
        }

        /// <summary>
        /// Save new user into database. If such user already exists, then throw new <code>SqlException</code>
        /// </summary>
        /// <param name="user">User to save in DB</param>
        public void SaveUser(User user)
        {
            try
            {
                if (!_areTablesFilled)
                {
                    FillTables();
                }
                DataRow[] existingRows = _database.Tables["tbUser"].Select(string.Format("Email = '{0}'", user.Email));
                if (existingRows.Length != 0)
                {
                    throw new FaultException<DataAlreadyExistsFault>(new DataAlreadyExistsFault()
                    {
                        ErrorMessage = "Such user already exists",
                        FullDescription = "User with email " + user.Email + " has already registered. " +
                            "Please, choose another email",
                        InnerException = null
                    });
                }

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
                if (!ConnectionFault(ex))
                {
                    Console.WriteLine(ex.Message); // logging
                    throw; // <=> throw ex
                }
                throw new FaultException<DatabaseConnectionFault>(new DatabaseConnectionFault()
                {
                    ErrorMessage = "Database connection timeout is ended",
                    FullDescription = ex.Message,
                    InnerException = ex
                });
            }
        }
        #endregion

        #region READ_DICTIONARIES
        // Read all needed dictionaries
        private static Dictionary<Guid, Note> GetUserNotes(DataRow row)
        {
            Dictionary<Guid, Note> notes = new Dictionary<Guid, Note>();
            foreach (DataRow child in row.GetChildRows("NoteUser"))
            {
                Note note = new Note();
                note.Id = (Guid)child["Id"];
                note.NoteText = (string)child["NoteText"];
                notes.Add(note.Id, note);
            }
            return notes;
        }

        private static Dictionary<Guid, TodoItem> GetUserTodo(DataRow row)
        {
            Dictionary<Guid, TodoItem> todoItems = new Dictionary<Guid, TodoItem>();
            foreach (DataRow child in row.GetChildRows("TodoUser"))
            {
                TodoItem item = new TodoItem();
                item.Id = (Guid)child["Id"];
                item.TodoText = (string)child["TodoText"];
                todoItems.Add(item.Id, item);
            }
            return todoItems;
        }

        private static Dictionary<Guid, SocialNetworkAccountInfo> GetAccounts(DataRow row)
        {
            Dictionary<Guid, SocialNetworkAccountInfo> accounts = new Dictionary<Guid, SocialNetworkAccountInfo>();
            foreach (DataRow child in row.GetChildRows("AccountInfoUser"))
            {
                SocialNetworkAccountInfo info = new SocialNetworkAccountInfo();
                info.Id = (Guid)child["Id"];
                info.IsActive = (bool)child["IsActive"];
                info.Login = (string)child["AccountLogin"];
                info.Password = (string)child["AccountPass"];
                info.Account = _idsAccounts[(Guid)child["IdAccount"]];
                accounts.Add(info.Id, info);
            }
            return accounts;
        }

        private static Dictionary<Guid, OnceRunningScheduleItem> GetScheduleItems(DataRow row, string machineName)
        {
            Dictionary<Guid, OnceRunningScheduleItem> scheduleItems = new Dictionary<Guid, OnceRunningScheduleItem>();
            foreach (DataRow child in row.GetChildRows("ScheduleItemUser"))
            {
                string scheduleItemMachineName = (string)child["MachineName"];
                if (!machineName.Equals(scheduleItemMachineName))
                {
                    continue;
                }

                OnceRunningScheduleItem item;
                Guid scheduleItemId = (Guid)child["Id"];
                string executablePath = (string)child["ExecutablePath"];
                DateTime triggeringTime = (DateTime)child["TriggeringTime"];
                string message = (string)child["TriggeringMessage"];
                bool isRegular = (bool)child["IsRegular"];

                if (!isRegular)
                {
                    item = new OnceRunningScheduleItem();
                }
                else
                {
                    item = GetRegularlyScheduleItem(scheduleItemId);
                }
                item.Id = scheduleItemId;
                item.ExecutablePath = executablePath;
                item.TriggeringTime = triggeringTime;
                item.Message = message;

                scheduleItems.Add(item.Id, item);
            }
            return scheduleItems;
        }

        private static RegularlyRunningScheduleItem GetRegularlyScheduleItem(Guid id)
        {
            RegularlyRunningScheduleItem item = new RegularlyRunningScheduleItem();
            try
            {
                _connection.Open();
                SqlCommand command = _connection.CreateCommand();
                command.CommandText = @"SELECT DayNumber FROM tbSchedulingDay WHERE ScheduleItemId = @Id";
                SqlParameter idParameter = new SqlParameter("@Id", id);
                command.Parameters.Add(idParameter);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int dayNumber = (int)reader["DayNumber"];
                        item.RunningDays[dayNumber] = true;
                    }
                }
            }
            finally
            {
                _connection.Close();
            }
            return item;
        }
        #endregion

        #region NOTES
        public void SaveNote(User user, Note note)
        {
            try
            {
                if (!_areTablesFilled)
                {
                    FillTables();
                }

                DataRow row = _database.Tables["tbNote"].NewRow();
                row["Id"] = note.Id;
                row["NoteText"] = note.NoteText;
                row["IdUser"] = user.Id;
                _database.Tables["tbNote"].Rows.Add(row);
                _noteAdapter.Update(_database.Tables["tbNote"]); //_database, "tbNote"
            }
            catch (SqlException ex)
            {
                if (!ConnectionFault(ex))
                {
                    Console.WriteLine(ex.Message); // logging
                    throw;
                }
                throw new FaultException<DatabaseConnectionFault>(new DatabaseConnectionFault()
                {
                    ErrorMessage = "Database connection timeout is ended",
                    FullDescription = ex.Message,
                    InnerException = ex
                });
            }
        }

        public void RemoveNote(Note note)
        {
            try
            {
                if (!_areTablesFilled)
                {
                    FillTables();
                }

                DataRow[] rows = _database.Tables["tbNote"].Select("Id = '" + note.Id + "'");
                DataRow deletedRow = rows[0];
                deletedRow.Delete();
                _noteAdapter.Update(_database.Tables["tbNote"]); //_database, "tbNote"
            }
            catch (SqlException ex)
            {
                if (!ConnectionFault(ex))
                {
                    Console.WriteLine(ex.Message); // loggingS
                    throw;
                }
                throw new FaultException<DatabaseConnectionFault>(new DatabaseConnectionFault()
                {
                    ErrorMessage = "Database connection timeout is ended",
                    FullDescription = ex.Message,
                    InnerException = ex
                });
            }
        }

        public void UpdateNote(Note note)
        {
            try
            {
                if (!_areTablesFilled)
                {
                    FillTables();
                }

                DataRow[] rows = _database.Tables["tbNote"].Select("Id = '" + note.Id + "'");
                DataRow editedRow = rows[0];
                editedRow["NoteText"] = note.NoteText;
                _noteAdapter.Update(_database.Tables["tbNote"]); //_database, "tbNote"
            }
            catch (SqlException ex)
            {
                if (!ConnectionFault(ex))
                {
                    Console.WriteLine(ex.Message); // loggingS
                    throw;
                }
                throw new FaultException<DatabaseConnectionFault>(new DatabaseConnectionFault()
                {
                    ErrorMessage = "Database connection timeout is ended",
                    FullDescription = ex.Message,
                    InnerException = ex
                });
            }
        }

        public Dictionary<Guid, Note> GetNotes(User user)
        {
            try
            {
                if (!_areTablesFilled)
                {
                    FillTables();
                }

                DataRow[] rows = _database.Tables["tbUser"].Select(string.Format("Id = {0}", user.Id));
                if (rows == null || rows.Length == 0)
                {
                    return null;
                }
                return GetUserNotes(rows[0]);
            }
            catch (SqlException ex)
            {
                if (!ConnectionFault(ex))
                {
                    Console.WriteLine(ex.Message); // logging
                    throw; // <=> throw ex
                }
                throw new FaultException<DatabaseConnectionFault>(new DatabaseConnectionFault()
                {
                    ErrorMessage = "Database connection timeout is ended",
                    FullDescription = ex.Message,
                    InnerException = ex
                });
            }
        }
        #endregion

        #region TODO
        public void SaveTodoItem(User user, TodoItem item)
        {
            try
            {
                if (!_areTablesFilled)
                {
                    FillTables();
                }

                DataRow row = _database.Tables["tbTodo"].NewRow();
                row["Id"] = item.Id;
                row["TodoText"] = item.TodoText;
                row["IdUser"] = user.Id;
                _database.Tables["tbTodo"].Rows.Add(row);
                _todoAdapter.Update(_database.Tables["tbTodo"]); //_database, "tbTodo"
            }
            catch (SqlException ex)
            {
                if (!ConnectionFault(ex))
                {
                    Console.WriteLine(ex.Message); // loggingS
                    throw;
                }
                throw new FaultException<DatabaseConnectionFault>(new DatabaseConnectionFault()
                {
                    ErrorMessage = "Database connection timeout is ended",
                    FullDescription = ex.Message,
                    InnerException = ex
                });
            }
        }

        public void RemoveTodoItem(TodoItem item)
        {
            try
            {
                if (!_areTablesFilled)
                {
                    FillTables();
                }

                DataRow[] rows = _database.Tables["tbTodo"].Select("Id = '" + item.Id + "'");
                DataRow deletedRow = rows[0];
                deletedRow.Delete();
                _todoAdapter.Update(_database.Tables["tbTodo"]); //_database, "tbTodo"
            }
            catch (SqlException ex)
            {
                if (!ConnectionFault(ex))
                {
                    Console.WriteLine(ex.Message); // loggingS
                    throw;
                }
                throw new FaultException<DatabaseConnectionFault>(new DatabaseConnectionFault()
                {
                    ErrorMessage = "Database connection timeout is ended",
                    FullDescription = ex.Message,
                    InnerException = ex
                });
            }
        }

        public Dictionary<Guid, TodoItem> GetTodoItems(User user)
        {
            try
            {
                if (!_areTablesFilled)
                {
                    FillTables();
                }

                DataRow[] rows = _database.Tables["tbUser"].Select(string.Format("Id = {0}", user.Id));
                if (rows == null || rows.Length == 0)
                {
                    return null;
                }
                return GetUserTodo(rows[0]);
            }
            catch (SqlException ex)
            {
                if (!ConnectionFault(ex))
                {
                    Console.WriteLine(ex.Message); // logging
                    throw; // <=> throw ex
                }
                throw new FaultException<DatabaseConnectionFault>(new DatabaseConnectionFault()
                {
                    ErrorMessage = "Database connection timeout is ended",
                    FullDescription = ex.Message,
                    InnerException = ex
                });
            }
        }
        #endregion

        #region SOCIAL_NETWORKS
        public void SaveAccountInfo(User user, SocialNetworkAccountInfo info)
        {
            try
            {
                if (!_areTablesFilled)
                {
                    FillTables();
                }

                DataRow[] rows = _database.Tables["tbAccountInfo"].Select("Id = ", info.Id.ToString());
                DataRow row = rows.Length == 0 ? _database.Tables["tbAccountInfo"].NewRow() : rows[0];

                row["Id"] = info.Id;
                row["IdUser"] = user.Id;
                row["IdAccount"] = _accountsIds[info.Account];
                row["AccountLogin"] = info.Login;
                row["AccountPass"] = info.Password;
                row["IsActive"] = info.IsActive;

                if (rows.Length == 0)
                {
                    _database.Tables["tbAccountInfo"].Rows.Add(row);
                }

                _socialNetAccountsAdapter.Update(_database.Tables["tbAccountInfo"]); //_database, "tbAccountInfo"
            }
            catch (SqlException ex)
            {
                if (!ConnectionFault(ex))
                {
                    Console.WriteLine(ex.Message); // loggingS
                    throw;
                }
                throw new FaultException<DatabaseConnectionFault>(new DatabaseConnectionFault()
                {
                    ErrorMessage = "Database connection timeout is ended",
                    FullDescription = ex.Message,
                    InnerException = ex
                });
            }
        }

        public Dictionary<Guid, SocialNetworkAccountInfo> GetAccounts(User user)
        {
            try
            {
                if (!_areTablesFilled)
                {
                    FillTables();
                }

                DataRow[] rows = _database.Tables["tbUser"].Select(string.Format("Id = {0}", user.Id));
                if (rows == null || rows.Length == 0)
                {
                    return null;
                }
                return GetAccounts(rows[0]);
            }
            catch (SqlException ex)
            {
                if (!ConnectionFault(ex))
                {
                    Console.WriteLine(ex.Message); // logging
                    throw; // <=> throw ex
                }
                throw new FaultException<DatabaseConnectionFault>(new DatabaseConnectionFault()
                {
                    ErrorMessage = "Database connection timeout is ended",
                    FullDescription = ex.Message,
                    InnerException = ex
                });
            }
        }
        #endregion

        #region SCHEDULER
        public void SaveScheduleItem(User user, OnceRunningScheduleItem item, string machineName)
        {
            try
            {
                if (!_areTablesFilled)
                {
                    FillTables();
                }

                DataRow row = _database.Tables["tbScheduleItem"].NewRow();
                row["Id"] = item.Id;
                Console.WriteLine(item.ExecutablePath);
                row["ExecutablePath"] = item.ExecutablePath;
                row["TriggeringTime"] = item.TriggeringTime;
                row["TriggeringMessage"] = item.Message;
                row["IdUser"] = user.Id;
                row["MachineName"] = machineName;
                row["IsRegular"] = item is RegularlyRunningScheduleItem ? true : false;
                _database.Tables["tbScheduleItem"].Rows.Add(row);
                _scheduleItemAdapter.Update(_database.Tables["tbScheduleItem"]);

                if (item is RegularlyRunningScheduleItem)
                {
                    InsertRegularItem(item as RegularlyRunningScheduleItem);
                }
            }
            catch (SqlException ex)
            {
                if (!ConnectionFault(ex))
                {
                    Console.WriteLine(ex.Message); // loggingS
                    throw;
                }
                throw new FaultException<DatabaseConnectionFault>(new DatabaseConnectionFault()
                {
                    ErrorMessage = "Database connection timeout is ended",
                    FullDescription = ex.Message,
                    InnerException = ex
                });
            }
        }

        private void InsertRegularItem(RegularlyRunningScheduleItem item)
        {
            try
            {
                _connection.Open();
                SqlCommand command = _connection.CreateCommand();
                for (int i = 0; i < item.RunningDays.Length; i++)
                {
                    if (item.RunningDays[i])
                    {
                        command.CommandText = @"INSERT INTO tbSchedulingDay(Id, IdScheduleItem, DayId)
                                                VALUES(@Id, @ScheduleItemId, @DayId)";
                        SqlParameter id = new SqlParameter("@Id", Guid.NewGuid());
                        SqlParameter scheduleItemId = new SqlParameter("@ScheduleItemId", item.Id);
                        SqlParameter dayId = new SqlParameter("@DayId", i);

                        command.Parameters.Add(id);
                        command.Parameters.Add(scheduleItemId);
                        command.Parameters.Add(dayId);
                        
                        command.ExecuteNonQuery();
                    }
                }
            }
            finally
            {
                if (_connection.State != ConnectionState.Closed)
                {
                    _connection.Close();
                }
            }
        }

        public void RemoveScheduleItem(OnceRunningScheduleItem item)
        {
            try
            {
                if (!_areTablesFilled)
                {
                    FillTables();
                }

                if (item is RegularlyRunningScheduleItem)
                {
                    DeleteRegularScheduleItem(item as RegularlyRunningScheduleItem);
                }
                DataRow[] rows = _database.Tables["tbScheduleItem"].Select("Id = '" + item.Id + "'");
                DataRow deletedRow = rows[0];
                deletedRow.Delete();
                _scheduleItemAdapter.Update(_database.Tables["tbScheduleItem"]);
            }
            catch (SqlException ex)
            {
                if (!ConnectionFault(ex))
                {
                    Console.WriteLine(ex.Message); // loggingS
                    throw;
                }
                throw new FaultException<DatabaseConnectionFault>(new DatabaseConnectionFault()
                {
                    ErrorMessage = "Database connection timeout is ended",
                    FullDescription = ex.Message,
                    InnerException = ex
                });
            }
        }

        private void DeleteRegularScheduleItem(RegularlyRunningScheduleItem item)
        {
            try
            {
                _connection.Open();
                SqlCommand command = _connection.CreateCommand();
                command.CommandText = @"DELETE FROM tbSchedulingDay WHERE IdScheduleItem = @Id";
                SqlParameter id = new SqlParameter("@Id", item.Id);
                command.Parameters.Add(id);
                command.ExecuteNonQuery();
            }
            finally
            {
                if (_connection.State != ConnectionState.Closed)
                {
                    _connection.Close();
                }
            }
        }

        public Dictionary<Guid, OnceRunningScheduleItem> GetScheduleItems(User user, string machineName)
        {
            try
            {
                if (!_areTablesFilled)
                {
                    FillTables();
                }

                DataRow[] rows = _database.Tables["tbUser"].Select(string.Format("Id = {0}", user.Id));
                if (rows == null || rows.Length == 0)
                {
                    return null;
                }
                return GetScheduleItems(rows[0], machineName);
            }
            catch (SqlException ex)
            {
                if (!ConnectionFault(ex))
                {
                    Console.WriteLine(ex.Message); // logging
                    throw; // <=> throw ex
                }
                throw new FaultException<DatabaseConnectionFault>(new DatabaseConnectionFault()
                {
                    ErrorMessage = "Database connection timeout is ended",
                    FullDescription = ex.Message,
                    InnerException = ex
                });
            }
        }
        #endregion
    }
}
