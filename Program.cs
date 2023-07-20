using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Text.RegularExpressions;

class UsernamePassword
{
    private string us;
    private string ps;

    public UsernamePassword()
    {
        this.us = "admin";
        this.ps = "1";
    }

    public string Us
    {
        get
        {
            return this.us;
        }
    }

    public string Ps
    {

        get
        {
            return this.ps;
        }
    }
}

class Student
{
    public string StudentName { get; set; }
    public int StudentRollNo { get; set; }
    public int StudentClass { get; set; }
    public string StudentDOB { get; set; }
    public string StudentEmail { get; set; }
    public string ParentNo { get; set; }

    public bool IsDeleted;

    public Student()
    {

    }

    public Student(string StudentName, int StudentRollNo, int StudentClass, string StudentDOB, string StudentEmail, string ParentNo)
    {
        this.StudentName = StudentName;
        this.StudentRollNo = StudentRollNo;
        this.StudentClass = StudentClass;
        this.StudentDOB = StudentDOB;
        this.StudentEmail = StudentEmail;
        this.ParentNo = ParentNo;
        this.IsDeleted = false;
    }

    public Student(string studentName, int studentRollNo, int studentClass, string studentDOB, string studentEmail, string parentNo, bool IsDeleted)
    {
        this.StudentName = studentName;
        this.StudentRollNo = studentRollNo;
        this.StudentClass = studentClass;
        StudentDOB = studentDOB;
        StudentEmail = studentEmail;
        ParentNo = parentNo;
        this.IsDeleted = IsDeleted;
    }

}

class StudentManagementSystem
{
    List<Student> studentsList = new List<Student>();

    public void AddStudent()
    {
        Student studentObject = new Student();

        System.Console.WriteLine("1. Add student info");
        System.Console.WriteLine();

        System.Console.Write("Enter student name: ");
        string StudentName = System.Console.ReadLine();

        if (NameValidationU(StudentName))
        {
            studentObject.StudentName = StudentName;
        }
        else
        {
            System.Console.WriteLine("Invalid input");
            return;
        }

        System.Console.Write("Enter student roll no: ");
        int StudentRollNo;
        bool b = int.TryParse(Console.ReadLine(), out StudentRollNo);

        studentObject.StudentRollNo = StudentRollNo;

        string conString = ConfigurationManager.ConnectionStrings["ConnectionStrings"].ConnectionString;

        using (SqlConnection con = new SqlConnection(conString))
        {
            con.Open();

            int idToCheck = StudentRollNo;

            string query = "select count(*) from Student where StudentRollNo = @ID";

            using (SqlCommand command = new SqlCommand(query, con))
            {
                command.Parameters.AddWithValue("@ID", idToCheck);

                int rowCount = (int)command.ExecuteScalar();

                if (rowCount > 0)
                {
                    Console.WriteLine("Row with Student Roll No {0} already exists", idToCheck);
                    return;
                }
            }

            con.Close();
        }

        System.Console.Write("Enter student class: ");

        int _class;
        if (int.TryParse(Console.ReadLine(), out _class))
        {
            studentObject.StudentClass = _class;
        }
        else
        {
            System.Console.WriteLine("Invalid input");
            return;
        }

        System.Console.Write("Enter student date of birth: ");
        string d = System.Console.ReadLine();

        studentObject.StudentDOB = d;

        if (DOBValidationU(d))
        {
            d = d;
        }
        else
        {
            System.Console.WriteLine("Invalid input");
            return;
        }

        System.Console.Write("Enter student email: ");
        string email = System.Console.ReadLine();

        if (MailValidationU(email))
        {
            studentObject.StudentEmail = email;
        }
        else
        {
            Console.WriteLine("Invalid input");
            return;
        }

        System.Console.Write("Enter parent's phone no: ");
        string phone = System.Console.ReadLine();


        if (PhoneValidationU(phone))
        {
            studentObject.ParentNo = phone;
        }
        else
        {
            System.Console.WriteLine("Invalid input");
            return;
        }

        studentObject.IsDeleted = false;

        Program p = new Program();

        using (SqlConnection connection = new SqlConnection(conString))
        {
            SqlCommand command1 = new SqlCommand("StudentSP", connection);

            command1.CommandType = CommandType.StoredProcedure;

            command1.Parameters.AddWithValue("@StudentName", SqlDbType.NVarChar).Value = studentObject.StudentName;
            command1.Parameters.AddWithValue("@StudentRollNo", SqlDbType.Int).Value = studentObject.StudentRollNo;
            command1.Parameters.AddWithValue("@StudentClass", SqlDbType.Int).Value = studentObject.StudentClass;
            command1.Parameters.AddWithValue("@StudentDOB", SqlDbType.NVarChar).Value = studentObject.StudentDOB;
            command1.Parameters.AddWithValue("@StudentEmail", SqlDbType.NVarChar).Value = studentObject.StudentEmail;
            command1.Parameters.AddWithValue("@ParentNo", SqlDbType.NVarChar).Value = studentObject.ParentNo;
            command1.Parameters.AddWithValue("@CreatedBy", SqlDbType.NVarChar).Value = Program.CreatedBy;
            command1.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
            command1.Parameters.AddWithValue("@Query", SqlDbType.NVarChar).Value = 1;

            connection.Open();

            command1.ExecuteNonQuery();

            connection.Close();
        }
    }


    public void SearchStudent()
    {
        Student studentObject = new Student();

        System.Console.WriteLine("2. Search student info");
        System.Console.WriteLine();

        System.Console.Write("Enter student name first letter: ");
        string name = System.Console.ReadLine();

        string conString = ConfigurationManager.ConnectionStrings["ConnectionStrings"].ConnectionString;

        using (SqlConnection connection = new SqlConnection(conString))
        {
            SqlCommand command = new SqlCommand("select * from Student where StudentName like '"+name+"%' and IsDeleted = 0;", connection); // -> select query

            System.Console.WriteLine();

            string s1 = String.Format("Student Name \t\t Roll No \t\t Class \t\t Date of birth \t\t Email \t\t Phone no");
            System.Console.WriteLine(s1);
            string s2 = String.Format("------------ \t\t ------- \t\t ----- \t\t ------------- \t\t ----- \t\t --------");
            System.Console.WriteLine(s2);

            connection.Open();

            SqlDataReader dr = command.ExecuteReader(); // only for select queries

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    System.Console.WriteLine("{0}\t\t\t   {1}\t\t\t  {2}\t\t  {3}\t   {4}   {5}", dr.GetValue(1).ToString(), dr.GetValue(2).ToString(), dr.GetValue(3).ToString(), dr.GetValue(4).ToString(), dr.GetValue(5).ToString(), dr.GetValue(6).ToString());
                }
            }
            else
            {
                System.Console.WriteLine("No result found");
                return;
            }
        }
    }

    public void ViewAllStudents()
    {

        System.Console.WriteLine("3. View all student info");
        System.Console.WriteLine();

        string conString = ConfigurationManager.ConnectionStrings["ConnectionStrings"].ConnectionString;

        using (SqlConnection connection = new SqlConnection(conString))
        {
            SqlCommand command = new SqlCommand("StudentSP", connection); // -> select query
            command.Parameters.AddWithValue("@Query", SqlDbType.NVarChar).Value = 4;

            command.CommandType = CommandType.StoredProcedure;

            string s1 = String.Format("Student Name \t\t Roll No \t\t Class \t\t Date of birth \t\t Email \t\t Phone no");
            System.Console.WriteLine(s1);
            string s2 = String.Format("------------ \t\t ------- \t\t ----- \t\t ------------- \t\t ----- \t\t --------");
            System.Console.WriteLine(s2);

            connection.Open();

            SqlDataReader dr = command.ExecuteReader();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    System.Console.WriteLine("{0}\t\t\t   {1}\t\t\t  {2}\t\t  {3}\t   {4}   {5}", dr.GetValue(1).ToString(), dr.GetValue(2).ToString(), dr.GetValue(3).ToString(), dr.GetValue(4).ToString(), dr.GetValue(5).ToString(), dr.GetValue(6).ToString());
                }
            }
            else
            {
                System.Console.WriteLine("No result found");
                return;
            }
        }
    }

    public void UpdateStudent()
    {
        Student studentObject = new Student();

        System.Console.WriteLine("4. Update student info");
        System.Console.WriteLine();

        System.Console.Write("Enter student roll no: ");

        int StudentRollNo;
        bool b = int.TryParse(Console.ReadLine(), out StudentRollNo);

        studentObject.StudentRollNo = StudentRollNo;

        string conString = ConfigurationManager.ConnectionStrings["ConnectionStrings"].ConnectionString;

        using (SqlConnection con = new SqlConnection(conString))
        {
            con.Open();

            int idToCheck = StudentRollNo;

            string query = "select count(*) from Student where StudentRollNo = @ID";

            using (SqlCommand command = new SqlCommand(query, con))
            {
                command.Parameters.AddWithValue("@ID", idToCheck);

                int rowCount = (int)command.ExecuteScalar();

                if (rowCount > 0)
                {
                    System.Console.WriteLine("1. Add student info");
                    System.Console.WriteLine();

                    System.Console.Write("Enter student name: ");
                    string StudentName = System.Console.ReadLine();

                    if (NameValidationU(StudentName))
                    {
                        studentObject.StudentName = StudentName;
                    }
                    else
                    {
                        System.Console.WriteLine("Invalid input");
                        return;
                    }
                    System.Console.Write("Enter student class: ");

                    int _class;
                    if (int.TryParse(Console.ReadLine(), out _class))
                    {
                        studentObject.StudentClass = _class;
                    }
                    else
                    {
                        System.Console.WriteLine("Invalid input");
                        return;
                    }

                    System.Console.Write("Enter student date of birth: ");
                    string d = System.Console.ReadLine();


                    if (DOBValidationU(d))
                    {
                        studentObject.StudentDOB = d;
                    }
                    else
                    {
                        System.Console.WriteLine("Invalid input");
                        return;
                    }

                    System.Console.Write("Enter student email: ");
                    string email = System.Console.ReadLine();

                    if (MailValidationU(email))
                    {
                        studentObject.StudentEmail = email;
                    }
                    else
                    {
                        Console.WriteLine("Invalid input");
                        return;
                    }

                    System.Console.Write("Enter parent's phone no: ");
                    string phone = System.Console.ReadLine();


                    if (PhoneValidationU(phone))
                    {
                        studentObject.ParentNo = phone;
                    }
                    else
                    {
                        System.Console.WriteLine("Invalid input");
                        return;
                    }
                    Program p = new Program();

                    using (SqlConnection connection = new SqlConnection(conString))
                    {
                        SqlCommand command1 = new SqlCommand("UpdateStudent", connection);
                        //SqlCommand command1 = new SqlCommand("StudentSP", connection);

                        command1.CommandType = CommandType.StoredProcedure;


                        //command.Parameters.AddWithValue("@Query", 2);

                        command1.Parameters.AddWithValue("@StudentName", SqlDbType.NVarChar).Value = studentObject.StudentName;
                        command1.Parameters.AddWithValue("@StudentRollNo", SqlDbType.Int).Value = studentObject.StudentRollNo;
                        command1.Parameters.AddWithValue("@StudentClass", SqlDbType.Int).Value = studentObject.StudentClass;
                        command1.Parameters.AddWithValue("@StudentDOB", SqlDbType.NVarChar).Value = studentObject.StudentDOB;
                        command1.Parameters.AddWithValue("@StudentEmail", SqlDbType.NVarChar).Value = studentObject.StudentEmail;
                        command1.Parameters.AddWithValue("@ParentNo", SqlDbType.NVarChar).Value = studentObject.ParentNo;
                        command1.Parameters.AddWithValue("@UpdatedBy", SqlDbType.NVarChar).Value = Program.CreatedBy;
                        command1.Parameters.AddWithValue("@UpdatedOn", DateTime.Now);

                        connection.Open();

                        command1.ExecuteNonQuery();

                        connection.Close();
                    }
                }
                else
                {
                    System.Console.WriteLine("No record exists!");
                    return;
                }
            }

            con.Close();
        }


    }

    public void DeleteStudent()
    {
        Student studentObject = new Student();

        System.Console.WriteLine("5. Delete student info");
        System.Console.WriteLine();

        System.Console.Write("Enter student roll no: ");
        int rollNo = int.Parse(System.Console.ReadLine());

        string conString = ConfigurationManager.ConnectionStrings["ConnectionStrings"].ConnectionString;

        using (SqlConnection connection = new SqlConnection(conString))
        {
            SqlCommand command = new SqlCommand("StudentSP", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@StudentRollNo", SqlDbType.Int).Value = rollNo;
            command.Parameters.AddWithValue("@Query", SqlDbType.NVarChar).Value = 3;

            connection.Open();

            int n = command.ExecuteNonQuery();

            connection.Close();

            if (n == 0)
            {
                System.Console.WriteLine("No result found");
                return;
            }
            else
            {
                System.Console.WriteLine("Deleted");
            }
        }
    }

    public bool NameValidationU(string name)
    {
        string pattern = @"^[a-zA-Z\s'-]+$";
        Regex regex = new Regex(pattern);
        if (regex.IsMatch(name))
            return (true);
        else
            return (false);
    }

    public bool DOBValidationU(string dateOfBirth)
    {
        string pattern = @"\b(0[1-9]|1\d|2\d|3[01])-(0[1-9]|1[0-2])-\d{4}\b";
        Regex regex = new Regex(pattern);

        if (regex.IsMatch(dateOfBirth))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool PhoneValidationU(string Phone)
    {
        string pattern = @"(^[0-9]{10}$)";
        Regex regex = new Regex(pattern);
        if (regex.IsMatch(Phone))
            return (true);
        else
            return (false);
    }
    public bool MailValidationU(string Mail)
    {
        try
        {
            MailAddress m = new MailAddress(Mail);

            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}

class Program
{
    public static string CreatedBy = null;
    static void Main()
    {

        try
        {
            UsernamePassword up = new UsernamePassword();

            StudentManagementSystem sm = new StudentManagementSystem();

            System.Console.WriteLine(" Student Management System Application");

            System.Console.WriteLine();

            System.Console.WriteLine("Login page");

            System.Console.WriteLine();

            System.Console.WriteLine("Enter Username: ");
            string us = System.Console.ReadLine();

            Program.CreatedBy = us;

            System.Console.WriteLine();

            System.Console.WriteLine("Enter password: ");
            string ps = System.Console.ReadLine();

            System.Console.WriteLine();

            if (us.Equals(up.Us) && ps.Equals(up.Ps))
            {
                bool flag = true;

                while (flag)
                {
                    int choice;
                    bool validInput = false;

                    do
                    {
                        System.Console.WriteLine(" 1. Add student info");
                        System.Console.WriteLine(" 2. Search student info");
                        System.Console.WriteLine(" 3. View all student info");
                        System.Console.WriteLine(" 4. Update student info");
                        System.Console.WriteLine(" 5. Delete student info");
                        System.Console.WriteLine(" 6. Exit");
                        System.Console.WriteLine();

                        System.Console.Write("Enter your choice: ");
                        string input = Console.ReadLine();

                        validInput = int.TryParse(input, out choice);

                        if (!validInput)
                        {
                            System.Console.WriteLine("Invalid input");
                            System.Console.WriteLine();
                        }

                    } while (!validInput);

                    System.Console.WriteLine("");

                    switch (choice)
                    {
                        case 1:
                            sm.AddStudent();
                            break;
                        case 2:
                            sm.SearchStudent();
                            break;
                        case 3:
                            sm.ViewAllStudents();
                            break;
                        case 4:
                            sm.UpdateStudent();
                            break;
                        case 5:
                            sm.DeleteStudent();
                            break;
                        case 6:
                            flag = false;
                            break;

                    }
                    System.Console.WriteLine();
                }
            }
            else
            {
                System.Console.WriteLine("Invalid login");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        System.Console.ReadKey();
    }
}

