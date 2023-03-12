using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;
using SoftgenTask.Enum;

namespace SoftgenTask
{
    public static class AcademicDatabase
    {
        private static readonly string _connectionString = ConfigurationManager.ConnectionStrings["SoftgenTask.Properties.Settings.MainDatabaseConnectionString"].ConnectionString;

        public static Academic GetUser(AcademicTable table, int id)
        {
            string query = $"SELECT * FROM { AcademicTableString(table) } WHERE Id = { id }";

            Academic academic = new Academic();

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        academic.Id = reader.GetInt32(0);
                        academic.FirstName = reader.GetString(1);
                        academic.LastName = reader.GetString(2);
                        academic.EmailAddress = reader.GetString(3);
                        academic.BirthDate = reader.GetDateTime(4);
                    }
                }
            }
            return academic;
        }


        public static void AddUser(AcademicTable table, string firstName, string lastName, string emailAddress, DateTime birthDate)
        {
            string query = $"INSERT INTO { AcademicTableString(table) } VALUES (@FirstName, @LastName, @EmailAddress, @BirthDate)";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                connection.Open();

                command.Parameters.AddWithValue("@FirstName", firstName);
                command.Parameters.AddWithValue("@LastName", lastName);
                command.Parameters.AddWithValue("@EmailAddress", emailAddress);
                command.Parameters.AddWithValue("@BirthDate", birthDate);

                command.ExecuteScalar();
            }
        }

        public static List<int> SearchUser(AcademicTable table, string firstName = "", string lastName = "", string emailAddress = "", DateTime? birthDate = null)
        {
            string query = $"SELECT Id FROM { AcademicTableString(table) } "
                + $"WHERE FirstName LIKE '%{firstName}%' "
                + $"AND LastName LIKE '%{lastName}%' ";

            if (emailAddress != "")
                query += $"AND Email = '{emailAddress}' ";

            if (birthDate != null)
                query += $"AND BirthDate = '{birthDate}' ";

            var idList = new List<int>();

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            idList.Add(reader.GetInt32(i));
                        }
                    }
                }
            }

            return idList;
        }

        public static bool EditUser(AcademicTable table, int id, string firstName = "", string lastName = "", string emailAddress = "", DateTime? birthDate = null)
        {
            bool isValid = false;

            string query = $"UPDATE { AcademicTableString(table) } SET";

            if (firstName != "")
            {
                query += $" FirstName = '{ firstName }',";
                isValid = true;
            }
            if (lastName != "")
            {
                query += $" LastName = '{ lastName }',";
                isValid = true;
            }
            if (emailAddress != "")
            {
                query += $" Email = '{ emailAddress }',";
                isValid = true;
            }
            if (birthDate != null)
            {
                query += $" BirthDate = '{ birthDate }',";
                isValid = true;
            }

            query = query.Remove(query.Length - 1);
            query += $" WHERE Id = { id }";

            if (isValid)
            {
                using (var connection = new SqlConnection(_connectionString))
                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();

                    command.ExecuteScalar();
                }
            }
            return isValid;
        }

        public static void DeleteUser(AcademicTable table, int id)
        {
            string query = $"DELETE FROM { AcademicTableString(table) } WHERE id = { id }";

            DeleteUsersFromGroups(table, id);

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                connection.Open();

                command.ExecuteScalar();
            }
        }

        private static void DeleteUsersFromGroups(AcademicTable table, int id)
        {
            string query = $"DELETE FROM { AcademicGroupTableString(table) } WHERE AcademicId = { id }";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                connection.Open();

                command.ExecuteScalar();
            }
        }

        private static string AcademicTableString(AcademicTable table)
        {
            switch(table)
            {
                case AcademicTable.STUDENTS:
                    return "Students";
                case AcademicTable.TEACHERS:
                    return "Teachers";
                default:
                    throw new NotImplementedException($"Code not implemented for academic Table: { table }");
            }
        }

        private static string AcademicGroupTableString(AcademicTable table)
        {
            switch (table)
            {
                case AcademicTable.STUDENTS:
                    return "StudentGroup";
                case AcademicTable.TEACHERS:
                    return "TeacherGroup";
                default:
                    throw new NotImplementedException($"Code not implemented for academic Table: { table }");
            }
        }
    }
}
