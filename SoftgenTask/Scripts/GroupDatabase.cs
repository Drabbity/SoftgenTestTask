using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftgenTask
{
    public static class GroupDatabase
    {
        private const string _GROUP_TABLE_NAME = "Groups";
        private const string _GROUP_STUDENT_TABLE_NAME = "StudentGroup";
        private const string _GROUP_TEACHER_TABLE_NAME = "TeacherGroup";
        private static readonly string _connectionString = ConfigurationManager.ConnectionStrings["SoftgenTask.Properties.Settings.MainDatabaseConnectionString"].ConnectionString;

        public static void AddGroup(string groupName)
        {
            string query = $"INSERT INTO { _GROUP_TABLE_NAME } VALUES ('{ groupName }')";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                connection.Open();

                command.ExecuteScalar();
            }
        }

        public static void AddStudent(int groupId, int studentId)
            => AddAcademic(_GROUP_STUDENT_TABLE_NAME, groupId, studentId);

        public static void AddTeacher(int groupId, int teacherId)
            => AddAcademic(_GROUP_TEACHER_TABLE_NAME, groupId, teacherId);

        private static void AddAcademic(string table, int groupId, int academicId)
        {
            string query = $"INSERT INTO { table } VALUES ( @groupId, @academicId )";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                connection.Open();

                command.Parameters.AddWithValue("@groupId", groupId);
                command.Parameters.AddWithValue("@academicId", academicId);

                command.ExecuteScalar();
            }
        }

        public static void RemoveStudent(int groupId, int studentId)
            => RemoveAcademic(_GROUP_STUDENT_TABLE_NAME, groupId, studentId);

        public static void RemoveTeacher(int groupId, int studentId)
            => RemoveAcademic(_GROUP_TEACHER_TABLE_NAME, groupId, studentId);

        private static void RemoveAcademic(string table, int groupId, int academicId)
        {
            string query = $"DELETE FROM { table } WHERE GroupId = { groupId } AND AcademicId = { academicId }";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                connection.Open();

                command.ExecuteScalar();
            }
        }
        public static List<int> GetStudents(int groupId)
            => GetAcademics(_GROUP_STUDENT_TABLE_NAME, groupId);

        public static List<int> GetTeachers(int groupId)
            => GetAcademics(_GROUP_TEACHER_TABLE_NAME, groupId);

        private static List<int> GetAcademics(string table, int groupId)
        {
            string query = $"SELECT AcademicId FROM { table } WHERE GroupId = { groupId }";

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

        public static List<int> SearchGroup(string groupName)
        {
            string query = $"SELECT Id FROM { _GROUP_TABLE_NAME } WHERE Name LIKE '%{ groupName }%'";

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

        public static void EditName(int id, string newName)
        {

            string query = $"UPDATE { _GROUP_TABLE_NAME } SET Name = '{ newName }' WHERE Id = { id }";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                connection.Open();

                command.ExecuteScalar();
            }
        }

        public static void DeleteGroup(int id)
        {
            string query = $"DELETE FROM { _GROUP_TABLE_NAME } WHERE id = { id }";

            DeleteGroupsFromCombinedTables(_GROUP_STUDENT_TABLE_NAME, id);
            DeleteGroupsFromCombinedTables(_GROUP_TEACHER_TABLE_NAME, id);

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                connection.Open();

                command.ExecuteScalar();
            }
        }

        private static void DeleteGroupsFromCombinedTables(string tableName, int id)
        {
            string query = $"DELETE FROM { tableName } WHERE GroupId = { id }";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                connection.Open();

                command.ExecuteScalar();
            }
        }
    }
}
