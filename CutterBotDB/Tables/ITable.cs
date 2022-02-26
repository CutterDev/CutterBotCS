using MySql.Data.MySqlClient;

namespace CutterDB.Tables
{
    /// <summary>
    /// Table Interfaces
    /// </summary>
    public interface ITable<T>
    {
        /// <summary>
        /// Open Database Connection
        /// </summary>
        void OpenConnection(string connstring, out string message);

        /// <summary>
        /// Close Database Connection
        /// </summary>
        void CloseConnection(out string message);
    }
}
