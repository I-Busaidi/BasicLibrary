using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace BasicLibrary
{
    internal class Program
    {
        // GLOBAL VARIABLES.
        static int CurrentUser=-1; // saves the id of current user
        static List<(int UserID, string UserName, string UserEmail, string UserPass)> Users = new List<(int UserID, string UserName, string UserEmail, string UserPass)>(); // Users list
        static List<(int AdminID, string AdminName, string AdminEmail, string AdminPass)> Admins = new List<(int AdminID, string AdminName, string AdminEmail, string AdminPass)>() ; // Admins list
        static List<(int BookID, string BookName, string AuthName, int Cpy, int BorrowedCpy, double BookPrice, string Category, int BorrowPeriod)> Books = new List<(int BookID, string BookName, string AuthName, int Cpy, int BorrowedCpy, double BookPrice, string Category, int BorrowPeriod)>(); // Books list
        static List<(int UserID, int BookID, string BorrowDate, string DueDate, string ReturnDate, string BRating, bool IsReturned)> Borrows = new List<(int UserID, int BookID, string BorrowDate, string DueDate, string ReturnDate, string BRating, bool IsReturned)>(); // Current borrows list
        static List<(int CatID, string CatName, int CatBookCount)> Categories = new List<(int CatID, string CatName, int CatBookCount)> (); // Categories List.


        // REGEX FORMATS
        static string EmailFormat = @"^[^@\s]+@[^@\s]+\.(com|edu|om)$"; // allows only emails in the format: example@example.com(OR .edu / .om)
        static string PassFormat = @"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$"; // Accepts only 8 character password, atleast 1 cap letter, 1 small letter, one number and one special character.
        static string NameCheck = @"^[A-Za-z]+( [A-Za-z]+)*$"; // Allows only letters and only one space between each 2 words.


        // FILE PATHS.
        static string filePath = "C:\\Users\\Lenovo\\Desktop\\Ibrahim_Projects\\LibrarySystemFiles\\BooksFile.txt"; // Library books are saved here.
        static string adminsPath = "C:\\Users\\Lenovo\\Desktop\\Ibrahim_Projects\\LibrarySystemFiles\\AdminsFile.txt"; // Admins are saved here.
        static string UsersPath = "C:\\Users\\Lenovo\\Desktop\\Ibrahim_Projects\\LibrarySystemFiles\\UsersFile.txt"; // Users are saved here.
        static string BorrowListPath = "C:\\Users\\Lenovo\\Desktop\\Ibrahim_Projects\\LibrarySystemFiles\\BorrowingFile.txt"; // Users currently borrowing books are saved here.
        static string CategoriesPath = "C:\\Users\\Lenovo\\Desktop\\Ibrahim_Projects\\LibrarySystemFiles\\CategoriesFile.txt"; // The history of all borrowings is saved here.
        static string AdminRequestPath = "C:\\Users\\Lenovo\\Desktop\\Ibrahim_Projects\\LibrarySystemFiles\\AdminRequestFile.txt"; // stores admin requests if any.



        // MAIN FUNCTION.
        static void Main(string[] args)
        {
            //Loading the .txt files into the lists of tuples.
            LoadAdminsFromFile();
            LoadUsersFromFile();
            LoadBooksFromFile();
            LoadCategoryFromFile();
            LoadBorrowedListFromFile();
            if(Admins.Count < 1)
            {
                Admins.Add((1,"admin","admin", "admin")); //temporary admin id and pass in case there is no admin registered.
            }
            int AccessLevel;
            bool StopApp = false;
            do
            {
                Console.Clear();
                Console.WriteLine("Enter (1) for admin access | (2) for user access | (0) to Exit:");
                while (!int.TryParse(Console.ReadLine(), out AccessLevel))
                {
                    Console.WriteLine("Invalid input, please try again: ");
                }

                switch (AccessLevel)
                {
                    case 1:
                        AuthorityCheck();
                        break;

                    case 2:
                        UserMenu();
                        break;

                    case 0:
                        StopApp = true;
                        break;
                }
            } while (!StopApp);
        }



        // ADMINS RELATED FUNCTIONS.
        static void AuthorityCheck() // Function to check access level of admin (admin / Master admin)
        {
            string AdminID;
            string AdminPass;
            bool AdminEmailFlag = false;
            bool AdminPassFlag = false;
            bool MasterAdminFlag = false;
            Console.WriteLine($"!!TOP SECRET!! DO NOT SHARE\n(Master Admin Email: {Admins[0].AdminEmail})");
            Console.WriteLine("\nEnter Admin ID:");
            while (string.IsNullOrEmpty(AdminID = Console.ReadLine().ToLower().Trim()))
            {
                Console.WriteLine("\nInvalid input, please try again:");
            }
            Console.WriteLine($"\nEnter Admin Password (Hint: Master Admin Pass ({Admins[0].AdminPass})):");
            while (string.IsNullOrEmpty(AdminPass = Console.ReadLine().ToLower().Trim()))
            {
                Console.WriteLine("\nInvalid input, please try again:");
            }
            for (int i = 0; i < Admins.Count; i++)
            {
                if (Admins[i].AdminEmail.ToLower().Trim() == AdminID.Trim()) //check if admin exist
                {
                    AdminEmailFlag = true;
                }
                if (Admins[i].AdminPass.ToLower().Trim() == AdminPass.Trim())
                {
                    AdminPassFlag = true;
                }
                if (AdminEmailFlag && AdminPassFlag) // if admin id and pass are correct, check if it is Master admin (first admin)
                {
                    if (i == 0)
                    {
                        MasterAdminFlag = true;
                    }
                }
            }
            if (AdminEmailFlag && AdminPassFlag && !MasterAdminFlag)
            {
                AdminMenu();
            }
            else if (AdminEmailFlag && AdminPassFlag && MasterAdminFlag)
            {
                MasterAdmin();
            }
            else if (AdminEmailFlag && !AdminPassFlag)
            {
                Console.WriteLine("\nInvalid Admin Password, please try again.");
            }
            else
            {
                Console.WriteLine("Invalid admin email, register new email");
            }
        }
        static void MasterAdmin() // Master admin menu
        {
            bool ExitFlag = false;
            do
            {
                Console.Clear();
                Console.WriteLine("Welcome!\nMaster Admin Authorized:");
                Console.WriteLine("\nEnter the number of the operation to perform:");
                Console.WriteLine("\n1. Add New Book.");
                Console.WriteLine("\n2. Display All Books.");
                Console.WriteLine("\n3. Search for Book.");
                Console.WriteLine("\n4. Edit Book Info.");
                Console.WriteLine("\n5. Manage Library Admins.");
                Console.WriteLine("\n6. Manage Library Users.");
                Console.WriteLine("\n7. Manage Categories.");
                Console.WriteLine("\n8. View Library Report.");
                Console.WriteLine("\n\n0. Save & Exit.");

                int choice;
                while (!int.TryParse(Console.ReadLine(), out choice))
                {
                    Console.WriteLine("\nInvalid input, please try again: ");
                }

                switch (choice)
                {
                    case 1:
                        AddnNewBook();
                        break;

                    case 2:
                        ViewAllBooks();
                        break;

                    case 3:
                        SearchForBook(false);
                        break;

                    case 4:
                        EditBook();
                        break;

                    case 5:
                        ManageAdmins();
                        break;

                    case 6:
                        ManageUsers();
                        break;

                    case 7:
                        ManageCategories();
                        break;

                    case 8:
                        ReportLibStats();
                        break;

                    case 0:
                        SaveAdminsToFile();
                        SaveUsersToFile();
                        SaveBooksToFile();
                        ExitFlag = true;
                        break;

                    default:
                        Console.WriteLine("\nInvalid choice, please try again.");
                        break;
                }
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();

            } while (ExitFlag != true);
        }
        static void ManageAdmins() // Add admins OR edit admins credentials / remove admin
        {
            bool ExitFlag = false;
            do
            {
                Console.WriteLine("\nChoose an option:\n1. Register new admin.\n2. Edit existing admin.\n\n0. Save & Exit.");
                int Choice;
                while ((!int.TryParse(Console.ReadLine(), out Choice))||(Choice > 3)||(Choice < 0))
                {
                    Console.WriteLine("\nInvalid input, please try again:");
                }
                switch (Choice)
                {
                    case 1:
                        AddNewAdmin();
                        break;

                    case 2:
                        EditAdmin();
                        break;

                    case 0:
                        ExitFlag = true;
                        break;
                }
                Console.Clear();
            } while (!ExitFlag);
        }
        static void AddNewAdmin() // used in ManageAdmins() to add admins
        {
            int AdminID;
            if (Admins.Count < 1)
            {
                AdminID = 1;
            }
            else
            {
                AdminID = Admins[Admins.Count - 1].AdminID + 1;
            }

            Console.WriteLine("\nEnter new Admin Name:");
            var NameValidation = EntryValidation(Admins, Console.ReadLine(), 1);
            while (!NameValidation.Item1)
            {
                Console.Clear();
                Console.WriteLine(NameValidation.Item2);
                NameValidation = EntryValidation(Admins, Console.ReadLine(), 1);
            }
            string NewAdminName = NameValidation.Item2;

            Console.WriteLine("\nEnter new Admin Email:");
            var EmailValidation = EntryValidation(Admins, Console.ReadLine(), 2);
            while(!EmailValidation.Item1)
            {
                Console.Clear();
                Console.WriteLine(EmailValidation.Item2);
                EmailValidation = EntryValidation(Admins, Console.ReadLine(), 2);
            }
            string NewAdminEmail = EmailValidation.Item2;
            
            Console.WriteLine($"\nEnter the password for {NewAdminEmail}:");
            var PassValidation = EntryValidation(Admins, Console.ReadLine(), 3);
            while (!PassValidation.Item1)
            {
                Console.Clear();
                Console.WriteLine(PassValidation.Item2);
                PassValidation = EntryValidation(Admins, Console.ReadLine(), 3);
            }
            string NewAdminPass = PassValidation.Item2;

            Admins.Add((AdminID, NewAdminName, NewAdminEmail, NewAdminPass));
            Console.WriteLine($"\nAdmin {NewAdminEmail} added successfully.");
        }
        static void EditAdmin() // used in ManageAdmins() to edit / remove admins
        {
            bool ExitFlag = false;
            do
            {
                Console.Clear();
                ViewAllAdmins();
                Console.WriteLine("\n0. Exit");
                Console.WriteLine("\nChoose an admin to edit:");
                int ChosenAdmin;
                while ((!int.TryParse(Console.ReadLine(), out ChosenAdmin)) || (ChosenAdmin > Admins.Count) || (ChosenAdmin < 0))
                {
                    Console.WriteLine("\nInvalid input, please try again:");
                }
                if (ChosenAdmin == 0)
                {
                    return;
                }
                Console.WriteLine("\nChoose an editing option:\n1. Edit Admin Name.\n2. Edit Admin Email.\n3. Edit Admin Password.\n4. Remove Admin.\n\n0. Exit.");
                int EditChoice;
                while ((!int.TryParse(Console.ReadLine(), out EditChoice)) || (EditChoice > 3) || (EditChoice < 0))
                {
                    Console.WriteLine("\nInvalid option, please try again.");
                }

                switch (EditChoice)
                {
                    case 0:
                        ExitFlag = true;
                        break;

                    case 1:
                        Console.WriteLine($"\nEnter the new Name for {Admins[ChosenAdmin - 1].AdminEmail}: ");
                        var NameValidation = EntryValidation(Admins, Console.ReadLine(), 1);
                        while (!NameValidation.Item1)
                        {
                            Console.Clear();
                            Console.WriteLine(NameValidation.Item2);
                            NameValidation = EntryValidation(Admins, Console.ReadLine(), 1);
                        }
                        string NewName = NameValidation.Item2;
                        string OldName = Admins[ChosenAdmin - 1].AdminEmail;
                        Admins[ChosenAdmin - 1] = (Admins[ChosenAdmin - 1].AdminID, NewName, Admins[ChosenAdmin - 1].AdminEmail, Admins[ChosenAdmin - 1].AdminPass);
                        Console.WriteLine($"\nAdmin \"{OldName}\" Name changed to: \"{NewName}\".");
                        break;

                    case 2:
                        Console.WriteLine($"\nEnter the new Email for {Admins[ChosenAdmin - 1].AdminEmail}: ");
                        var EmailValidation = EntryValidation(Admins, Console.ReadLine(), 2);
                        while (!EmailValidation.Item1)
                        {
                            Console.Clear();
                            Console.WriteLine(EmailValidation.Item2);
                            EmailValidation = EntryValidation(Admins, Console.ReadLine(), 2);
                        }
                        string NewEmail = EmailValidation.Item2;
                        string OldEmail = Admins[ChosenAdmin - 1].AdminEmail;
                        Admins[ChosenAdmin - 1] = (Admins[ChosenAdmin - 1].AdminID, Admins[ChosenAdmin - 1].AdminName, NewEmail, Admins[ChosenAdmin - 1].AdminPass);
                        Console.WriteLine($"\nAdmin \"{OldEmail}\" Email changed to: {NewEmail}.");
                        break;

                    case 3:
                        Console.WriteLine($"\nEnter the new Password for {Admins[ChosenAdmin - 1].AdminEmail}: ");
                        var PassValidation = EntryValidation(Admins, Console.ReadLine(), 3);
                        while (!PassValidation.Item1)
                        {
                            Console.Clear();
                            Console.WriteLine(PassValidation.Item2);
                            PassValidation = EntryValidation(Admins, Console.ReadLine(), 3);
                        }
                        string NewPass = PassValidation.Item2;
                        string OldPass = Admins[ChosenAdmin - 1].AdminPass;
                        Admins[ChosenAdmin - 1] = (Admins[ChosenAdmin - 1].AdminID, Admins[ChosenAdmin - 1].AdminName, Admins[ChosenAdmin - 1].AdminEmail, NewPass);
                        Console.WriteLine($"\n\"{Admins[ChosenAdmin - 1].AdminEmail}\" Password changed from: {OldPass} to: {NewPass}.");
                        break;

                    case 4:
                        if (Admins.Count > 1)
                        {
                            string RemovedAdmin = Admins[ChosenAdmin - 1].AdminEmail;
                            Admins.RemoveAt(ChosenAdmin - 1);
                            Console.WriteLine($"\nAdmin \"{RemovedAdmin}\" has been removed from the Admins File.");
                        }
                        else
                        {
                            Console.WriteLine("\nCannot remove the last remaining admin.\nPlease add another admin account before deleting this account.");
                        }
                        break;

                    default:
                        Console.WriteLine("\nInvalid input, please try again.");
                        break;
                }
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            } while (!ExitFlag);
        }
        static void ViewAllAdmins()
        {
            StringBuilder sb = new StringBuilder();

            int AdmnNumber = 0;

            for (int i = 0; i < Admins.Count; i++)
            {
                AdmnNumber = i + 1;
                sb.Append("Admin ").Append(AdmnNumber).Append(" Email: ").Append(Admins[i].AdminEmail).Append(" Name: ").Append(Admins[i].AdminName);
                if(i == 0)
                {
                    sb.Append(" *ADMIN MASTER*");
                }
                sb.AppendLine();
                sb.Append(" Password: ").Append(Admins[i].AdminPass).Append(" ID: ").Append(Admins[i].AdminID);
                sb.AppendLine().AppendLine();
                Console.WriteLine(sb.ToString());
                sb.Clear();

            }
        }
        static void AdminMenu()
        {
            bool ExitFlag = false;
            do
            {
                Console.Clear();
                Console.WriteLine("Welcome!\nAdmin Authorized:");
                Console.WriteLine("\nEnter the number of the operation to perform:");
                Console.WriteLine("\n1. Add New Book.");
                Console.WriteLine("\n2. Display All Books.");
                Console.WriteLine("\n3. Search for Book.");
                Console.WriteLine("\n4. Edit Book Info.");
                Console.WriteLine("\n5. Manage Users.");
                Console.WriteLine("\n6. Manage Categories.");
                Console.WriteLine("\n7. View Library Report.");
                Console.WriteLine("\n\n0. Save & Exit.");

                int choice;
                while (!int.TryParse(Console.ReadLine(), out choice))
                {
                    Console.WriteLine("\nInvalid input, please try again: ");
                }

                switch (choice)
                {
                    case 1:
                        AddnNewBook();
                        break;

                    case 2:
                        ViewAllBooks();
                        break;

                    case 3:
                        SearchForBook(false);
                        break;

                    case 4:
                        EditBook();
                        break;

                    case 5:
                        ManageUsers();
                        break;

                    case 6:
                        ManageCategories();
                        break;

                    case 7:
                        ReportLibStats();
                        break;

                    case 0:
                        SaveBooksToFile();
                        ExitFlag = true;
                        break;

                    default:
                        Console.WriteLine("\nInvalid choice, please try again.");
                        break;
                }
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();

            } while (ExitFlag != true);
        }
        static void SaveAdminsToFile()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(adminsPath))
                {
                    foreach (var admin in Admins)
                    {
                        writer.WriteLine($"{admin.AdminID}|{admin.AdminName}|{admin.AdminEmail}|{admin.AdminPass}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving to file: {ex.Message}");
            }
        }
        static void LoadAdminsFromFile()
        {
            try
            {
                if (File.Exists(adminsPath))
                {
                    using (StreamReader reader = new StreamReader(adminsPath))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            var parts = line.Split('|');
                            if (parts.Length == 4)
                            {
                                Admins.Add((int.Parse(parts[0]), parts[1], parts[2], parts[3]));
                            }
                        }
                    }
                    Console.WriteLine("Admins loaded from file successfully.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading from file: {ex.Message}");
            }
        }



        // USERS RELATED FUNCTIONS.
        static void ManageUsers() // used by admins to edit / add / remove users
        {
            bool ExitFlag = false;
            do
            {
                Console.WriteLine("\nChoose an option:\n1. Register new User.\n2. Edit existing User.\n3. View a user's Profile.\n\n0. Save & Exit.");
                int Choice;
                while ((!int.TryParse(Console.ReadLine(), out Choice)) || (Choice > 3) || (Choice < 0))
                {
                    Console.WriteLine("\nInvalid input, please try again:");
                }
                switch (Choice)
                {
                    case 1:
                        AddNewUser();
                        break;

                    case 2:
                        EditUser();
                        break;

                    case 3:
                        Console.Clear();
                        StringBuilder sb = new StringBuilder();
                        for (int i = 0;i<Users.Count;i++)
                        {
                            sb.AppendLine($"{(i+1)}. User ID: {Users[i].UserID} | User Name: {Users[i].UserName}");
                        }
                        Console.WriteLine("Users List:\n"+sb.ToString());
                        Console.WriteLine("\nChoose a user from the list to view profile:");
                        int UIDtoSearch;
                        while ((!int.TryParse(Console.ReadLine(),out UIDtoSearch)) || (UIDtoSearch < 0) || (UIDtoSearch > Users.Count))
                        {
                            Console.WriteLine("Invalid input, please try again.");
                        }
                        UserIndividualReport(Users[UIDtoSearch-1].UserID);
                        break;

                    case 0:
                        ExitFlag = true;
                        break;
                }
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
                Console.Clear();
            } while (!ExitFlag);
        }
        static void AddNewUser()
        {
            int NewUserID;
            if (Users.Count > 0)
            {
                NewUserID = Users[Users.Count - 1].UserID + 1;
            }
            else
            {
                NewUserID = 1;
            }

            Console.WriteLine("\nEnter new User Name:");
            var NameValidation = EntryValidation(Users, Console.ReadLine(), 1);
            while (!NameValidation.Item1)
            {
                Console.Clear();
                Console.WriteLine(NameValidation.Item2);
                NameValidation = EntryValidation(Users, Console.ReadLine(), 1);
            }
            string NewUserName = NameValidation.Item2;

            Console.WriteLine("\nEnter new User Email:");
            var EmailValidation = EntryValidation(Users, Console.ReadLine(), 2);
            while (!EmailValidation.Item1)
            {
                Console.Clear();
                Console.WriteLine(EmailValidation.Item2);
                EmailValidation = EntryValidation(Users, Console.ReadLine(), 2);
            }
            string NewUserEmail = EmailValidation.Item2;

            Console.WriteLine($"\nEnter the password for {NewUserEmail}:");
            var PassValidation = EntryValidation(Users, Console.ReadLine(), 3);
            while (!PassValidation.Item1)
            {
                Console.Clear();
                Console.WriteLine(PassValidation.Item2);
                PassValidation = EntryValidation(Users, Console.ReadLine(), 3);
            }
            string NewUserPass = PassValidation.Item2;

            Users.Add((NewUserID, NewUserName, NewUserEmail, NewUserPass));
            Console.WriteLine($"\nUser {NewUserID} added successfully.");
        }
        static void EditUser() // edit user email and/or password or delete user
        {
            bool ExitFlag = false;
            do
            {
                Console.Clear();
                ViewAllUsers();
                Console.WriteLine("\n0. Exit");
                Console.WriteLine("\nChoose a user to edit:");
                int ChosenUser;
                while ((!int.TryParse(Console.ReadLine(), out ChosenUser)) || (ChosenUser > Users.Count) || (ChosenUser < 0))
                {
                    Console.WriteLine("\nInvalid input, please try again:");
                }
                if (ChosenUser == 0)
                {
                    return;
                }
                Console.WriteLine("\nChoose an editing option:\n1. Edit User Name.\n2. Edit User Email.\n3. Edit User Password.\n4. Remove User.\n\n0. Exit.");
                int EditChoice;
                while ((!int.TryParse(Console.ReadLine(), out EditChoice)) || (EditChoice > 4) || (EditChoice < 0))
                {
                    Console.WriteLine("\nInvalid option, please try again.");
                }

                switch (EditChoice)
                {
                    case 0:
                        ExitFlag = true;
                        break;

                    case 1:
                        Console.WriteLine($"\nEnter the new Name for user {Users[ChosenUser - 1].UserEmail}: ");
                        var NameValidation = EntryValidation(Users, Console.ReadLine(), 1);
                        while (!NameValidation.Item1)
                        {
                            Console.Clear();
                            Console.WriteLine(NameValidation.Item2);
                            NameValidation = EntryValidation(Users, Console.ReadLine(), 1);
                        }
                        string NewName = NameValidation.Item2;
                        string OldName = Users[ChosenUser - 1].UserName;
                        Users[ChosenUser - 1] = (Users[ChosenUser - 1].UserID, NewName, Users[ChosenUser - 1].UserEmail, Users[ChosenUser - 1].UserPass);
                        Console.WriteLine($"\n\"{Users[ChosenUser - 1].UserID}\" Password changed from: {OldName} to: {NewName}.");
                        break;

                    case 2:
                        Console.WriteLine($"\nEnter the new Email for user {Users[ChosenUser - 1].UserID}: ");
                        var EmailValidation = EntryValidation(Users, Console.ReadLine(), 2);
                        while (!EmailValidation.Item1)
                        {
                            Console.Clear();
                            Console.WriteLine(EmailValidation.Item2);
                            EmailValidation = EntryValidation(Users, Console.ReadLine(), 2);
                        }
                        string NewEmail = EmailValidation.Item2;
                        Users[ChosenUser - 1] = (Users[ChosenUser - 1].UserID, Users[ChosenUser - 1].UserName, NewEmail, Users[ChosenUser - 1].UserPass);
                        Console.WriteLine($"\nUser \"{Users[ChosenUser - 1].UserID}\" Email changed to: {NewEmail}.");
                        break;

                    case 3:
                        Console.WriteLine($"\nEnter the new Password for user {Users[ChosenUser - 1].UserID}: ");
                        var PassValidation = EntryValidation(Users, Console.ReadLine(), 3);
                        while (!PassValidation.Item1)
                        {
                            Console.Clear();
                            Console.WriteLine(PassValidation.Item2);
                            PassValidation = EntryValidation(Users, Console.ReadLine(), 3);
                        }
                        string NewPass = PassValidation.Item2;
                        string OldPass = Users[ChosenUser - 1].UserPass;
                        Users[ChosenUser - 1] = (Users[ChosenUser - 1].UserID, Users[ChosenUser - 1].UserName, Users[ChosenUser - 1].UserEmail, NewPass);
                        Console.WriteLine($"\n\"{Users[ChosenUser - 1].UserID}\" Password changed from: {OldPass} to: {NewPass}.");
                        break;

                    case 4:
                        int RemovedUser = Users[ChosenUser - 1].UserID;
                        Users.RemoveAt(ChosenUser - 1);
                        Console.WriteLine($"\nUser \"{RemovedUser}\" has been removed from the Users File.");
                        break;

                    default:
                        Console.WriteLine("\nInvalid input, please try again.");
                        break;
                }
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            } while (!ExitFlag);
        }
        static void ViewAllUsers()
        {
            StringBuilder sb = new StringBuilder();

            int UserNumber = 0;

            for (int i = 0; i < Users.Count; i++)
            {
                UserNumber = i + 1;
                sb.Append("User ").Append(UserNumber).Append(" Email: ").Append(Users[i].UserEmail);
                sb.AppendLine();
                sb.Append(" Password: ").Append(Users[i].UserPass);
                sb.AppendLine();
                sb.Append(" ID: ").Append(Users[i].UserID);
                sb.Append(" Name: ").Append(Users[i].UserName);
                sb.AppendLine().AppendLine();
                Console.WriteLine(sb.ToString());
                sb.Clear();

            }
        }
        static bool UserLogin() // returns true or false to be used in the login menu to determine the next action
        {

            string UsrEmail;
            string UsrPass;
            Console.WriteLine("\nEnter user Email:");
            while (string.IsNullOrEmpty(UsrEmail = Console.ReadLine().ToLower()))
            {
                Console.WriteLine("\nInvalid input, please try again:");
            }
            Console.WriteLine("\nEnter user Password:");
            while (string.IsNullOrEmpty(UsrPass = Console.ReadLine()))
            {
                Console.WriteLine("\nInvalid input, please try again:");
            }
            for (int i = 0; i < Users.Count; i++)
            {
                if ((Users[i].UserEmail.Trim() == UsrEmail.Trim()) && (Users[i].UserPass.Trim() == UsrPass.Trim()))
                {
                    CurrentUser = Users[i].UserID;
                    return true;
                }
            }
            return false;
        }
        static void UserMenu()
        {
            bool RegisterUser = false;
            do
            {
                Console.Clear();
                CurrentUser = -1;
                Console.WriteLine("\nWelcome to Busaidi Library!\n\nEnter (1) to login | (2) to sign up | (0) to Exit");
                int LoginSignUp;
                while((!int.TryParse(Console.ReadLine(), out LoginSignUp))||(LoginSignUp > 2) ||(LoginSignUp < 0))
                {
                    Console.WriteLine("\nInvalid input, please try again:");
                }
                if (LoginSignUp == 1)
                {
                    RegisterUser = UserLogin();
                }
                else if (LoginSignUp == 2)
                {
                    AddNewUser();
                }
                else
                {
                    return;
                }
                
            } while (!RegisterUser);


            bool ExitFlag = false;
            do
            {
                if (!OverDueCheck(CurrentUser))
                {
                    Console.Clear();
                    Console.WriteLine("Welcome to the Library!");
                    Console.WriteLine("\nEnter the number of the service required:");
                    Console.WriteLine("\n1. Search book");
                    Console.WriteLine("\n2. Borrow book");
                    Console.WriteLine("\n3. Return book");
                    Console.WriteLine("\n4. View Profile");
                    Console.WriteLine("\n0. Save & Exit");

                    int choice;
                    while (!int.TryParse(Console.ReadLine(), out choice))
                    {
                        Console.WriteLine("\nInvalid input, please try again: ");
                    }
                    Console.Clear();
                    switch (choice)
                    {
                        case 1:
                            SearchForBook(true);
                            break;

                        case 2:
                            BorrowBook();
                            break;

                        case 3:
                            ReturnBook();
                            break;

                        case 4:
                            UserIndividualReport(CurrentUser);
                            break;

                        case 0:
                            SaveBooksToFile();
                            CurrentUser = -1;
                            ExitFlag = true;
                            break;

                        default:
                            Console.WriteLine("\nInvalid choice, please try again...");
                            break;
                    }
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Welcome to the Library!");
                    Console.WriteLine("\n!You Must Return Books Overdue Before Using Other Services!");
                    Console.WriteLine("\n1. Return book");
                    Console.WriteLine("\n2. View Profile");
                    Console.WriteLine("\n0. Save & Exit");

                    int choice;
                    while (!int.TryParse(Console.ReadLine(), out choice))
                    {
                        Console.WriteLine("\nInvalid input, please try again: ");
                    }
                    Console.Clear();
                    switch (choice)
                    {
                        case 1:
                            ReturnBook();
                            break;

                        case 2:
                            UserIndividualReport(CurrentUser);
                            break;

                        case 0:
                            SaveBooksToFile();
                            CurrentUser = -1;
                            ExitFlag = true;
                            break;

                        default:
                            Console.WriteLine("\nInvalid choice, please try again...");
                            break;
                    }
                }
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            } while (ExitFlag != true);
        }
        static void SaveUsersToFile()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(UsersPath))
                {
                    foreach (var user in Users)
                    {
                        writer.WriteLine($"{user.UserID}|{user.UserName}|{user.UserEmail}|{user.UserPass}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving to file: {ex.Message}");
            }
        }
        static void LoadUsersFromFile()
        {
            try
            {
                if (File.Exists(UsersPath))
                {
                    using (StreamReader reader = new StreamReader(UsersPath))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            var parts = line.Split('|');
                            if (parts.Length == 4)
                            {
                                Users.Add((int.Parse(parts[0]), parts[1], parts[2], parts[3]));
                            }
                        }
                    }
                    Console.WriteLine("Admins loaded from file successfully.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading from file: {ex.Message}");
            }
        }
        static bool OverDueCheck(int UID)
        {
            for (int i = 0; i < Borrows.Count; i++)
            {
                if ((UID == Borrows[i].UserID) && (Borrows[i].IsReturned == false))
                {
                    if (DateTime.Parse(Borrows[i].DueDate) < DateTime.Now)
                    {
                        return true;
                    }
                }
            }
            return false;
        }



        // BOOKS RELATED FUNCTIONS.
        static void AddnNewBook()
        {
            List<string> ExistingBooks = new List<string>();
            for (int i = 0; i < Books.Count; i++)
            {
                ExistingBooks.Add(Books[i].BookName.ToLower());
            }
            Console.WriteLine("\nEnter Book Name");
            string name;
            while ((string.IsNullOrEmpty(name = Console.ReadLine())) || (ExistingBooks.Contains(name.ToLower())))
            {
                Console.WriteLine("\nInvalid Input, please try again: ");
            }

            Console.WriteLine($"\nEnter Author Name of \"{name}\": ");
            string author;
            while (string.IsNullOrEmpty(author = Console.ReadLine()))
            {
                Console.WriteLine("\nInvalid Input, please try again: ");
            }

            int ID;
            if (Books.Count > 0)
            {
                ID = Books[Books.Count - 1].BookID + 1;
            }
            else
            {
                ID = 1;
            }
            Console.WriteLine($"\nEnter available Copies of \"{name}\": ");
            int Qty;
            while ((!int.TryParse(Console.ReadLine(), out Qty)) || (Qty < 1))
            {
                Console.WriteLine("\nInvalid input, please try again:");
            }
            Console.WriteLine($"Enter the price of \"{name}\": ");
            double BPrice;
            while ((!double.TryParse(Console.ReadLine(),out BPrice)) || (BPrice <= 0))
            {
                Console.WriteLine("\nInvalid input, please try again.");
            }
            Console.WriteLine("\nCategories:\n");
            StringBuilder SB = new StringBuilder();
            for (int i = 0; i < Categories.Count; i++)
            {
                SB.Append((i+1) + ". " + Categories[i].CatName);
            }
            Console.WriteLine(SB.ToString());
            Console.WriteLine($"Enter the number of a category from the list for the book \"{name}\": ");
            int CatNo;
            while ((!int.TryParse(Console.ReadLine(), out CatNo)) || (CatNo <= 0) || (CatNo > Categories.Count))
            {
                Console.WriteLine("Invalid input, please try again.");
            }
            Console.WriteLine($"Enter the maximum borrowing period of \"{name}\" in days:");
            int BorrowPeriod;
            while ((!int.TryParse(Console.ReadLine(), out BorrowPeriod)) || (BorrowPeriod <= 0))
            {
                Console.WriteLine("Invalid input, please try again.");
            }
            Categories[CatNo - 1] = ((Categories[CatNo - 1].CatID, Categories[CatNo - 1].CatName, (Categories[CatNo - 1].CatBookCount + 1)));
            Books.Add((ID, name, author, Qty, 0, BPrice, Categories[CatNo - 1].CatName, BorrowPeriod));
            Console.WriteLine($"\nBook \"{name}\" Added Succefully");
        }
        static void ViewAllBooks()
        {
            StringBuilder sb = new StringBuilder();

            int BookNumber = 0;

            for (int i = 0; i < Books.Count; i++)
            {             
                BookNumber = i + 1;
                sb.AppendLine($"{BookNumber}. Book: {Books[i].BookName} | Author: {Books[i].AuthName} | ID: {Books[i].BookID} | Copies: {Books[i].Cpy}");
                sb.AppendLine($"Copies Available: {Books[i].Cpy - Books[i].BorrowedCpy} | Price: {Books[i].BookPrice} | Category: {Books[i].Category} | Borrow max period: {Books[i].BorrowPeriod}");
                sb.AppendLine("-----------------------");
            }
            Console.Clear();
            Console.WriteLine("Current available books:\n");
            Console.WriteLine(sb.ToString());
            sb.Clear();
        }
        static void SearchForBook(bool AdmnOrUsr) // function behaves differently depending on the access level
        {
            ViewAllBooks();
            Console.WriteLine("Enter the book or author name to search");
            string name;
            while (string.IsNullOrEmpty(name = Console.ReadLine()))
            {
                Console.WriteLine("\nInvalid Input, please try again: ");
            }
            bool flag=false;
            bool AuthBooks = false;
            int BookIndex = -1;
            int count = 1;
            List<int> BookIdsAuth = new List<int>();
            List<int> BookIds = new List<int>();
            StringBuilder AuthBookList = new StringBuilder();
            StringBuilder BooksList = new StringBuilder();
            AuthBookList.Clear();
            BooksList.Clear();
            for (int i = 0; i< Books.Count;i++)
            {
                if (Books[i].BookName.ToLower().Contains(name.ToLower())) // prints book if the searched name is a book name
                {
                    BooksList.AppendLine($"{count}. Book: {Books[i].BookName} | Author: {Books[i].AuthName} | ID: {Books[i].BookID} | Copies: {Books[i].Cpy}");
                    BooksList.AppendLine($"Copies Available: {Books[i].Cpy - Books[i].BorrowedCpy} | Price: {Books[i].BookPrice} | Category: {Books[i].Category} | Borrow max period: {Books[i].BorrowPeriod}");
                    BooksList.AppendLine("-----------------------");
                    count++;
                    flag = true;
                    BookIds.Add(i);
                }
                if (Books[i].AuthName.ToLower().Contains(name.ToLower())) // prints a list of books made by the author if author name was searched.
                {
                    AuthBookList.AppendLine($"{count}. Book: {Books[i].BookName} | Author: {Books[i].AuthName} | ID: {Books[i].BookID} | Copies: {Books[i].Cpy}");
                    AuthBookList.AppendLine($"Copies Available: {Books[i].Cpy - Books[i].BorrowedCpy} | Price: {Books[i].BookPrice} | Category: {Books[i].Category} | Borrow max period: {Books[i].BorrowPeriod}");
                    AuthBookList.AppendLine("-----------------------");
                    count++;
                    AuthBooks = true;
                    BookIdsAuth.Add( i );
                }

            }
            if (AuthBooks && AdmnOrUsr) // if the person searching is a user
            {
                Console.WriteLine("\nChoose a book to borrow:");
                Console.WriteLine(AuthBookList.ToString());
                int BookChoice;
                while((!int.TryParse(Console.ReadLine(), out BookChoice))||(BookChoice < 1) ||(BookChoice > BookIdsAuth.Count))
                {
                    Console.WriteLine("\nInvalid input, please try again:");
                }
                for (int i = 0; i<BookIdsAuth.Count;i++)
                {
                    if((BookChoice - 1) == BookIdsAuth[i])
                    {
                        BookIndex = BookIdsAuth[i];
                        break;
                    }
                }
            }
            if (flag && AdmnOrUsr) // if the person searching is a user
            {
                Console.WriteLine("\nChoose a book to borrow:");
                Console.WriteLine(BooksList.ToString());
                int BookChoice;
                while ((!int.TryParse(Console.ReadLine(), out BookChoice)) || (BookChoice < 1) || (BookChoice > BookIds.Count))
                {
                    Console.WriteLine("\nInvalid input, please try again:");
                }
                for (int i = 0; i < BookIds.Count; i++)
                {
                    if ((BookChoice - 1) == BookIds[i])
                    {
                        BookIndex = BookIds[i];
                        break;
                    }
                }
            }

            if (!flag && !AuthBooks) // if book is not found
            { 
                Console.WriteLine("\nBook or author not found"); 
            }
            else if (!AdmnOrUsr && (AuthBooks || flag)) // if the person searching is an admin only print book(s) info and stop
            {
                Console.WriteLine(AuthBookList.ToString());
            }
            else
            {
                if (Books[BookIndex].Cpy == Books[BookIndex].BorrowedCpy)
                {
                    Console.WriteLine("\nSorry, book is out of stock.");
                }
                else
                {
                    Console.WriteLine("\nBorrow book? (1)Yes / (2)No:");
                    int BorrowConf;
                    while((!int.TryParse(Console.ReadLine(), out BorrowConf))||(BorrowConf>2) ||(BorrowConf<1))
                    {
                        Console.WriteLine("\nInvalid input, please try again:");
                    }
                    if (BorrowConf != 1)
                    {
                        Console.WriteLine("\nReturning to menu...");
                    }
                    else
                    {
                        BorrowBook(BookIndex);
                    }
                }
            }
        }
        static void BorrowBook(int BookIndex = -1) // parameter is used to determine whether the function is used with a book index already decided or not
        {
            if (BookIndex == -1)
            {
                bool ExitBorrow = false;
                do
                {
                    Console.Clear();
                    Console.WriteLine("Choose a method:" +
                        "\n1. Search by book / author name." +
                        "\n2. Browse available books." +
                        "\n\n0. Exit");

                    int Choice;
                    while(!int.TryParse(Console.ReadLine(), out Choice))
                    {
                        Console.WriteLine("\nInvalid input, please try again: ");
                    }
                    Console.Clear();
                    switch (Choice)
                    {
                        case 1:
                            SearchForBook(true);
                            break;
                        case 2:
                            ViewAllBooks();
                            Console.WriteLine("\nEnter the number from the list of book to borrow:");
                            int BookChoice;
                            while((!int.TryParse(Console.ReadLine(), out BookChoice))||(BookChoice < 1) ||(BookChoice > Books.Count))
                            {
                                Console.WriteLine("\nInvalid input, please try again:");
                            }
                            BorrowBook(BookChoice - 1);
                            break;
                        case 0:
                            SaveBooksToFile();
                            Console.WriteLine("\nReturning to menu...");
                            ExitBorrow = true;
                            break;

                        default:
                            Console.WriteLine("\nInvalid input, please try again:");
                            break;
                    }
                } while (!ExitBorrow);
            }
            else
            {
                bool AlreadyBorrowed = false;
                for (int i = 0; i < Borrows.Count; i++)
                {
                    if ((Borrows[i].UserID == CurrentUser) && (Borrows[i].BookID == Books[BookIndex].BookID) && (Borrows[i].IsReturned == false))
                    {
                        AlreadyBorrowed = true;
                        break;
                    }
                }
                if (AlreadyBorrowed)
                {
                    Console.WriteLine("You have already borrowed this book before...");
                }
                else
                {
                    Borrows.Add((CurrentUser, Books[BookIndex].BookID, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTime.Today.AddDays(Books[BookIndex].BorrowPeriod).ToString("yyyy-MM-dd HH:mm:ss"), "N/A", "N/A", false));
                    Books[BookIndex] = (Books[BookIndex].BookID, Books[BookIndex].BookName, Books[BookIndex].AuthName, Books[BookIndex].Cpy, (Books[BookIndex].BorrowedCpy + 1), Books[BookIndex].BookPrice, Books[BookIndex].Category, Books[BookIndex].BorrowPeriod);
                    Console.Clear();
                    Console.WriteLine($"\n{Books[BookIndex].BookName} borrowed successfully!");
                    SaveBorrowedListToFile();
                    SaveBooksToFile();
                    RecommendationBooks(Books[BookIndex].BookID, Books[BookIndex].BookName, CurrentUser);
                }
            }
        }
        static void ReturnBook() // shows borrowing users their current books to be returned, and allows them to return
        {
            bool UserBorrowed = false;
            int BorrowedIndex = -1;
            List<int> IDs = new List<int>();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Borrowed Books:\n");
            for (int i = 0; i < Borrows.Count; i++)
            {
                if ((Borrows[i].UserID == CurrentUser) && (Borrows[i].IsReturned == false))
                {
                    IDs.Add(Borrows[i].BookID);
                    UserBorrowed = true;
                    sb.AppendLine($"ID: {Borrows[i].BookID} | Borrowed On: {Borrows[i].BorrowDate} | Due Date: {Borrows[i].DueDate} | Days Left: {(DateTime.Parse(Borrows[i].DueDate) - DateTime.Now).Days}");
                }
            }
            if (UserBorrowed)
            {
                Console.WriteLine(sb.ToString());
                IDs.Add(0);
                int BookChoice;
                Console.WriteLine("\n\n0. Exit");
                Console.WriteLine("\nEnter the ID of book to return:");
                while ((!int.TryParse(Console.ReadLine(), out BookChoice))||(BookChoice < 0) || (!IDs.Contains(BookChoice)))
                {
                    Console.WriteLine("\nInvalid input, please try again: ");
                }
                if (BookChoice == 0)
                {
                    Console.WriteLine("\nCancelling return process...");
                }
                else
                {
                    for (int i = 0; i < Borrows.Count; i++)
                    {
                        if ((Borrows[i].UserID == CurrentUser) && (Borrows[i].BookID == BookChoice) && (Borrows[i].IsReturned == false))
                        {
                            BorrowedIndex = i;
                            break;
                        }
                    }
                    Console.WriteLine("\nConfirm Book Return? (1) Yes / (2) No");
                    int ReturnConf;
                    while ((!int.TryParse(Console.ReadLine(), out ReturnConf)) || (ReturnConf < 1) || (ReturnConf > 2))
                    {
                        Console.WriteLine("\nInvalid input, please try again: ");
                    }
                    if (ReturnConf == 2)
                    {
                        Console.WriteLine("\nCancelling return process...");
                    }
                    else
                    {
                        int ReturnedIndex = -1;
                        for (int i = 0; i < Books.Count; i++)
                        {
                            if (Books[i].BookID == BookChoice)
                            {
                                ReturnedIndex = i;
                                Books[i] = (Books[i].BookID, Books[i].BookName, Books[i].AuthName, Books[i].Cpy, (Books[i].BorrowedCpy - 1), Books[i].BookPrice, Books[i].Category, Books[i].BorrowPeriod);
                                break;
                            }
                        }
                        Console.WriteLine($"Thank you for returning \"{Books[ReturnedIndex].BookName}\"!\n Please rate the book out of 5: ");
                        float BRating;
                        while((!float.TryParse(Console.ReadLine(), out BRating))||(BRating < 0) ||(BRating > 5))
                        {
                            Console.WriteLine("Invalid rating, please try again:");
                        }
                        try
                        {
                            Borrows[BorrowedIndex] = (Borrows[BorrowedIndex].UserID, Borrows[BorrowedIndex].BookID, Borrows[BorrowedIndex].BorrowDate, Borrows[BorrowedIndex].DueDate, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), BRating.ToString(), true);
                            SaveBorrowedListToFile();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"\nCould not update borrows list...{ex}");
                        }
                        Console.WriteLine($"\n{Books[ReturnedIndex].BookName} returned successfully!");
                        SaveBooksToFile();
                    }
                }
            }
            else
            {
                Console.WriteLine("\nCurrent User Has No Borrowed Books...");
            }
        }
        static void LoadBooksFromFile()
        {
            try
            {
                if (File.Exists(filePath))
                {
                    using (StreamReader reader = new StreamReader(filePath))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            var parts = line.Split('|');
                            if (parts.Length == 8)
                            {
                                Books.Add((int.Parse(parts[0]), parts[1], parts[2], int.Parse(parts[3]), int.Parse(parts[4]), double.Parse(parts[5]), parts[6], int.Parse(parts[7])));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError loading from file: {ex.Message}");
            }
        }
        static void SaveBooksToFile()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    foreach (var book in Books)
                    {
                        writer.WriteLine($"{book.BookID}|{book.BookName}|{book.AuthName}|{book.Cpy}|{book.BorrowedCpy}|{book.BookPrice}|{book.Category}|{book.BorrowPeriod}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving to file: {ex.Message}");
            }
        }
        static void EditBook() // used by admins to edit book info / add quantity / delete book
        {
            bool ExitFlag = false;
            do
            {
                Console.Clear();
                ViewAllBooks();
                Console.WriteLine("\n0. Exit");
                Console.WriteLine("\nChoose a book to edit:");
                int ChosenBook;
                while ((!int.TryParse(Console.ReadLine(), out ChosenBook)) || (ChosenBook > Books.Count) || (ChosenBook < 0))
                {
                    Console.WriteLine("\nInvalid input, please try again:");
                }
                if (ChosenBook == 0)
                {
                    return;
                }
                Console.WriteLine($"\nChoose an editing option for \"{Books[ChosenBook - 1].BookName}\":\n1. Edit Book Name.\n2. Edit Book Author.\n3. Add copies.\n4. Edit price.\n5. Change category.\n6. Remove Book.\n\n0. Exit.");
                int EditChoice;
                while ((!int.TryParse(Console.ReadLine(), out EditChoice)) || (EditChoice > 4) || (EditChoice < 0))
                {
                    Console.WriteLine("\nInvalid option, please try again.");
                }

                switch (EditChoice)
                {
                    case 0:
                        ExitFlag = true;
                        break;

                    case 1:
                        List<string> ExistingBooks = new List<string>();
                        for (int i = 0; i < Books.Count; i++)
                        {
                            ExistingBooks.Add(Books[i].BookName.ToLower());
                        }
                        Console.WriteLine($"\nEnter the new name for {Books[ChosenBook - 1].BookName}: ");
                        string NewName;
                        while((string.IsNullOrEmpty(NewName = Console.ReadLine())) || (ExistingBooks.Contains(NewName.ToLower())))
                        {
                            Console.WriteLine("\nInvalid input, please try again:");
                        }
                        string OldBName = Books[ChosenBook - 1].BookName;
                        Books[ChosenBook - 1] = (Books[ChosenBook - 1].BookID, NewName, Books[ChosenBook - 1].AuthName, Books[ChosenBook - 1].Cpy, Books[ChosenBook - 1].BorrowedCpy, Books[ChosenBook - 1].BookPrice, Books[ChosenBook - 1].Category, Books[ChosenBook - 1].BorrowPeriod);
                        Console.WriteLine($"\nBook \"{OldBName}\" name changed to: {NewName}.");
                        break;

                    case 2:
                        Console.WriteLine($"\nEnter the new author name for {Books[ChosenBook - 1].BookName}: ");
                        string NewAuth;
                        while (string.IsNullOrEmpty(NewAuth = Console.ReadLine()))
                        {
                            Console.WriteLine("\nInvalid input, please try again:");
                        }
                        string OldAuth = Books[ChosenBook - 1].AuthName;
                        Books[ChosenBook - 1] = (Books[ChosenBook - 1].BookID, Books[ChosenBook - 1].BookName, NewAuth, Books[ChosenBook - 1].Cpy, Books[ChosenBook - 1].BorrowedCpy, Books[ChosenBook - 1].BookPrice, Books[ChosenBook - 1].Category, Books[ChosenBook - 1].BorrowPeriod);
                        Console.WriteLine($"\n\"{Books[ChosenBook - 1].BookName}\" Author changed from: {OldAuth} to: {NewAuth}.");
                        break;

                    case 3:
                        Console.WriteLine($"\nEnter the additional copies for {Books[ChosenBook - 1].BookName}: ");
                        int NewQty;
                        while ((!int.TryParse(Console.ReadLine(), out NewQty))||(NewQty < 1))
                        {
                            Console.WriteLine("\nInvalid input, please try again:");
                        }
                        int ConfCpyeEdit;
                        while ((!int.TryParse(Console.ReadLine(), out ConfCpyeEdit)) || (ConfCpyeEdit > 2) || (ConfCpyeEdit < 1))
                        {
                            Console.WriteLine("Invalid input, please try again:");
                        }
                        if (ConfCpyeEdit != 2)
                        {
                            Books[ChosenBook - 1] = (Books[ChosenBook - 1].BookID, Books[ChosenBook - 1].BookName, Books[ChosenBook - 1].AuthName, (Books[ChosenBook - 1].Cpy + NewQty), Books[ChosenBook - 1].BorrowedCpy, Books[ChosenBook - 1].BookPrice, Books[ChosenBook - 1].Category, Books[ChosenBook - 1].BorrowPeriod);
                            Console.WriteLine($"\n{Books[ChosenBook - 1].BookName} Quantity has been increased to {Books[ChosenBook - 1].Cpy} successfully.");
                        }
                        else
                        {
                            Console.WriteLine("Copies addition cancelled...");
                        }
                        break;

                    case 4:
                        Console.WriteLine($"\nEnter the new price for {Books[ChosenBook - 1].BookName}: ");
                        double NewPrice;
                        while ((!double.TryParse(Console.ReadLine(), out NewPrice)) || (NewPrice <= 0))
                        {
                            Console.WriteLine("\nInvalid input, please try again:");
                        }
                        Console.WriteLine($"Price of \"{Books[ChosenBook - 1].BookName}\" will change from {Books[ChosenBook - 1].BookPrice} to {NewPrice}");
                        Console.WriteLine("\nConfirm? (1) Yes / (2) No");
                        int ConfPriceEdit;
                        while((!int.TryParse(Console.ReadLine(), out ConfPriceEdit))||(ConfPriceEdit > 2)||(ConfPriceEdit < 1))
                        {
                            Console.WriteLine("Invalid input, please try again:");
                        }
                        if (ConfPriceEdit != 2)
                        {
                            Books[ChosenBook - 1] = (Books[ChosenBook - 1].BookID, Books[ChosenBook - 1].BookName, Books[ChosenBook - 1].AuthName, Books[ChosenBook - 1].Cpy, Books[ChosenBook - 1].BorrowedCpy, NewPrice, Books[ChosenBook - 1].Category, Books[ChosenBook - 1].BorrowPeriod);
                            Console.WriteLine($"\n{Books[ChosenBook - 1].BookName} Quantity has been increased to {Books[ChosenBook - 1].Cpy} successfully.");
                        }
                        else
                        {
                            Console.WriteLine("Price Change cancelled...");
                        }
                        break;

                    case 5:
                        Console.WriteLine("\nCategories:\n");
                        StringBuilder SB = new StringBuilder();
                        int OldCatIndex = -1;
                        for (int i = 0; i < Categories.Count; i++)
                        {
                            SB.Append((i + 1) + ". " + Categories[i].CatName);
                            if (Categories[i].CatName == Books[ChosenBook - 1].Category)
                            {
                                OldCatIndex = i;
                            }
                        }
                        Console.WriteLine(SB.ToString());
                        Console.WriteLine($"\nCurrent category: {Books[ChosenBook - 1].Category}");
                        Console.WriteLine($"Enter the number of a category from the list for the book \"{Books[ChosenBook - 1].BookName}\": ");
                        int CatNo;
                        while ((!int.TryParse(Console.ReadLine(), out CatNo)) || (CatNo <= 0) || (CatNo > Categories.Count))
                        {
                            Console.WriteLine("Invalid input, please try again.");
                        }
                        Console.WriteLine($"\nCategory of \"{Books[ChosenBook - 1].BookName}\" will be changed from \"{Books[ChosenBook - 1].Category}\" to \"{Categories[CatNo - 1].CatName}\" ");
                        Console.WriteLine("Confirm? (1) Yes / (2) No");
                        int ConfCatChange;
                        while ((!int.TryParse(Console.ReadLine(), out ConfCatChange)) || (ConfCatChange > 2) || (ConfCatChange < 1))
                        {
                            Console.WriteLine("Invalid input, please try again.");
                        }
                        if (ConfCatChange == 1)
                        {
                            Categories[CatNo - 1] = (Categories[CatNo - 1].CatID, Categories[CatNo - 1].CatName, (Categories[CatNo - 1].CatBookCount + 1));
                            Categories[OldCatIndex] = (Categories[OldCatIndex].CatID, Categories[OldCatIndex].CatName, (Categories[OldCatIndex].CatBookCount - 1));
                            Books[ChosenBook - 1] = (Books[ChosenBook - 1].BookID, Books[ChosenBook - 1].BookName, Books[ChosenBook - 1].AuthName, Books[ChosenBook - 1].Cpy, Books[ChosenBook - 1].BorrowedCpy, Books[ChosenBook - 1].BookPrice, Categories[CatNo - 1].CatName, Books[ChosenBook - 1].BorrowPeriod);
                            Console.WriteLine($"\nCategory of \"{Books[ChosenBook - 1].BookName}\" changed from \"{Books[ChosenBook - 1].Category}\" to \"{Categories[CatNo - 1].CatName}\" successfully!");
                        }
                        else
                        {
                            Console.WriteLine("\nCancelling category edit...");
                        }
                        break;


                    case 6:
                        if (Books[ChosenBook - 1].BorrowedCpy > 0)
                        {
                            Console.WriteLine($"Book is currently borrowed by {Books[ChosenBook - 1].BorrowedCpy} user(s) and cannot be removed.");
                        }
                        else
                        {
                            Console.WriteLine($"Are you sure you want to remove {Books[ChosenBook - 1].BookName} from the library?");
                            Console.WriteLine("\n(1) Yes / (2) No");
                            int ConfChoice;
                            while ((!int.TryParse(Console.ReadLine(), out ConfChoice)) || (ConfChoice > 2) || (ConfChoice < 1))
                            {
                                Console.WriteLine("Invalid input, please try again.");
                            }
                            if (ConfChoice == 1)
                            {
                                int RemovedBookCat = -1;
                                for (int i = 0; i < Categories.Count; i++)
                                {
                                    if (Categories[i].CatName == Books[ChosenBook - 1].Category)
                                    {
                                        RemovedBookCat = i;
                                        break;
                                    }
                                }
                                string RemovedBook = Books[ChosenBook - 1].BookName;
                                Books.RemoveAt(ChosenBook - 1);
                                Categories[RemovedBookCat] = (Categories[RemovedBookCat].CatID, Categories[RemovedBookCat].CatName, (Categories[RemovedBookCat].CatBookCount - 1));
                                Console.WriteLine($"\nBook \"{RemovedBook}\" has been removed from the library File.");
                            }
                            else
                            {
                                Console.WriteLine("\nCancelling book removal...");
                            }
                        }
                        break;


                    default:
                        Console.WriteLine("\nInvalid input, please try again.");
                        break;
                }
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            } while (!ExitFlag);
        }
        static void ViewCategories()
        {
            StringBuilder sb = new StringBuilder();

            int CatNumber = 0;

            for (int i = 0; i < Categories.Count; i++)
            {
                CatNumber = i + 1;
                sb.AppendLine($"{CatNumber}. Category ID: {Categories[i].CatID} | Category Name: {Categories[i].CatName} | Books in {Categories[i].CatName}: {Categories[i].CatBookCount}");
            }
            Console.Clear();
            Console.WriteLine("Current available Categories:\n");
            Console.WriteLine(sb.ToString());
            sb.Clear();
        }
        static void ManageCategories()
        {
            bool MngCatCont = true;
            do
            {
                Console.Clear();
                ViewCategories();
                Console.WriteLine("Choose an option:\n1. Add New Category.\n2. Edit Category.\n\n0. Exit.");
                int CatMenu;
                while ((!int.TryParse(Console.ReadLine(), out CatMenu)) || (CatMenu > 2) || (CatMenu < 0))
                {
                    Console.WriteLine("Invalid option, please try again.");
                }
                switch (CatMenu)
                {
                    case 0:
                        MngCatCont = false;
                        break;

                    case 1:
                        AddCategory();
                        break;

                    case 2:
                        EditCategory();
                        break;
                }
            } while (MngCatCont);
        }
        static void AddCategory()
        {
            bool CatNotExist = false;
            bool CatValid = true;
            string NewCatName = "";
            int CatID;
            if (Categories.Count > 0 )
            {
                CatID = Categories[Categories.Count - 1].CatID + 1;
            }
            else
            {
                CatID = 1;
            }
            while (!CatNotExist)
            {
                Console.Clear();
                Console.WriteLine("Enter the New Category Name: ");
                NewCatName = Console.ReadLine();
                if (string.IsNullOrEmpty(NewCatName))
                {
                    Console.WriteLine("Invalid Input, please try again:");
                    CatNotExist = false;
                }
                else
                {
                    for (int i = 0;i < Categories.Count;i++)
                    {
                        if (Categories[i].CatName == NewCatName)
                        {
                            Console.WriteLine("Name Already Exists...");
                            CatNotExist = false;
                            CatValid = false;
                            break;
                        }
                    }
                    if (CatValid)
                    {
                        CatNotExist = true;
                    }
                }
            }
            Categories.Add((CatID, NewCatName, 0));
            SaveCategoriesToFile();
            Console.WriteLine($"New Category \"{NewCatName}\" Added Successfully!");
        }
        static void EditCategory()
        {
            bool ContCatEdit = true;
            do
            {
                Console.Clear ();
                ViewCategories();
                Console.WriteLine($"\n0. Exit.\n\nChoose a category from the list to edit (1 - {Categories.Count}):");
                int CatChoice;
                while ((!int.TryParse(Console.ReadLine(), out CatChoice)) || (CatChoice > Categories.Count) || (CatChoice < 0))
                {
                    Console.WriteLine("Invalid input, please try again:");
                }
                Console.Clear ();
                Console.WriteLine($"Choose an editing option for category \"{Categories[CatChoice-1].CatName}\":\n1. Edit Name.\n\n0. Cancel & Exit.");
                int EditChoice;
                while ((!int.TryParse(Console.ReadLine(), out EditChoice))||(EditChoice > 1) ||(EditChoice < 0))
                {
                    Console.WriteLine("Invalid option, please try again:");
                }
                switch (EditChoice)
                {
                    case 0:
                        ContCatEdit = false;
                        break;

                    case 1:
                        bool CatNotExist = false;
                        bool CatValid = true;
                        string NewCatName = "";
                        while (!CatNotExist)
                        {
                            Console.Clear();
                            Console.WriteLine($"Enter the New Category Name for {Categories[CatChoice - 1].CatName}: ");
                            NewCatName = Console.ReadLine();
                            if (string.IsNullOrEmpty(NewCatName))
                            {
                                Console.WriteLine("Invalid Input, please try again:");
                                CatNotExist = false;
                            }
                            else
                            {
                                for (int i = 0; i < Categories.Count; i++)
                                {
                                    if (Categories[i].CatName == NewCatName)
                                    {
                                        Console.WriteLine("Name Already Exists...");
                                        CatNotExist = false;
                                        CatValid = false;
                                        break;
                                    }
                                }
                                if (CatValid)
                                {
                                    CatNotExist = true;
                                }
                            }
                        }
                        Categories[CatChoice - 1] = (Categories[CatChoice - 1].CatID, NewCatName, Categories[CatChoice - 1].CatBookCount);
                        SaveCategoriesToFile();
                        break;
                }
            } while (ContCatEdit);
        }
        static void SaveCategoriesToFile()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(CategoriesPath))
                {
                    foreach (var Cat in Categories)
                    {
                        writer.WriteLine($"{Cat.CatID}|{Cat.CatName}|{Cat.CatBookCount}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving to file: {ex.Message}");
            }
        }
        static void LoadCategoryFromFile()
        {
            try
            {
                if (File.Exists(CategoriesPath))
                {
                    using (StreamReader reader = new StreamReader(CategoriesPath))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            var parts = line.Split('|');
                            if (parts.Length == 3)
                            {
                                Categories.Add((int.Parse(parts[0]), parts[1], int.Parse(parts[2])));
                            }
                        }
                    }
                    Console.WriteLine("Category info loaded from file successfully.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading from file: {ex.Message}");
            }
        }



        // UTILITY / REPORTS FUNCTIONS.
        static void SaveBorrowedListToFile()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(BorrowListPath))
                {
                    foreach (var Borrow in Borrows)
                    {
                        writer.WriteLine($"{Borrow.UserID}|{Borrow.BookID}|{Borrow.BorrowDate}|{Borrow.DueDate}|{Borrow.ReturnDate}|{Borrow.BRating}|{Borrow.IsReturned}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving to file: {ex.Message}");
            }
        }
        static void LoadBorrowedListFromFile()
        {
            try
            {
                if (File.Exists(BorrowListPath))
                {
                    using (StreamReader reader = new StreamReader(BorrowListPath))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            var parts = line.Split('|');
                            if (parts.Length == 7)
                            {
                                Borrows.Add((int.Parse(parts[0]), int.Parse(parts[1]), parts[2], parts[3], parts[4], parts[5], bool.Parse(parts[6])));
                            }
                        }
                    }
                    Console.WriteLine("Borrowing info loaded from file successfully.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading from file: {ex.Message}");
            }
        }
        static void RecommendationBooks(int BorrowedBookID, string BorrowedBookName ,int UserID) // used to recommend other books after borrowing based on people who borrowed the same book
        {
            StringBuilder sb = new StringBuilder();
            bool FoundBorrowedBook = false;
            List<int> UsersWhoBorrowedBook = new List<int>();
            List<int> OtherBooksUsersBorrowed = new List<int>();
            
            
            for (int i = 0; i < Borrows.Count; i++) // add users' Ids of other people who borrowed the same book.
            {
                if ((Borrows[i].BookID == BorrowedBookID) && (Borrows[i].UserID != UserID))
                {
                    UsersWhoBorrowedBook.Add(Borrows[i].UserID);
                    FoundBorrowedBook = true;
                }
            }
            if (FoundBorrowedBook)
            {
                for (int i = 0; i < UsersWhoBorrowedBook.Count; i++) // add the other books borrowed by the people who borrowed the same book to a list
                {
                    for(int j = 0; j < Borrows.Count; j++)
                    {
                        if ((Borrows[j].UserID == UsersWhoBorrowedBook[i]) && (Borrows[j].BookID != BorrowedBookID) && (!OtherBooksUsersBorrowed.Contains(Borrows[j].BookID)))
                        {
                            OtherBooksUsersBorrowed.Add(Borrows[j].BookID);
                        }
                    }
                }
                int count = 1;
                sb.Clear();
                for (int i = 0; i < OtherBooksUsersBorrowed.Count; i++)
                {
                    for (int j = 0; j < Books.Count; j++)
                    {
                        if (OtherBooksUsersBorrowed[i] == Books[i].BookID)
                        {
                            sb.AppendLine(count + ". Book Name: " + Books[i].BookName + " | ID: " + OtherBooksUsersBorrowed[i] + " | Copies Available: " + (Books[i].Cpy - Books[i].BorrowedCpy));
                            count++;
                            if (count == 5)
                            {
                                break;
                            }
                        }
                    }
                }
                if (OtherBooksUsersBorrowed.Count > 0) // print recommendation if the list is not empty
                {
                    Console.WriteLine($"\n\nPeople who borrowed \"{BorrowedBookName}\" also borrowed:\n");
                    Console.WriteLine(sb.ToString());
                    Console.WriteLine("\nEnter the number of the from the list if you want to borrow, or (0) to Exit:");
                    int Choice;
                    while ((!int.TryParse(Console.ReadLine(), out Choice)) || (Choice > count) || (Choice < 0))
                    {
                        Console.WriteLine("\nInvalid input, please try again:");
                    }
                    if (Choice == 0)
                    {
                        return;
                    }
                    else
                    {
                        int Index = -1;
                        for (int i = 0; i < Books.Count; i++)
                        {
                            if (OtherBooksUsersBorrowed[Choice - 1] == Books[i].BookID)
                            {
                                Index = i;
                                break;
                            }
                        }
                        BorrowBook(Index);
                    }
                }
                else
                {
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }
            }
        }
        static void ReportLibStats(int PageNo = 1) // shows the different stats of the Library / users
        {
                if (PageNo == 1)
                {
                    Console.Clear();
                    ViewAllBooks();
                    Console.WriteLine("\n\nPage 1 of 4");
                    Console.WriteLine("(Right Arrow Key ->) Page 2 | (Esc) to Exit");
                    var PressedKey = Console.ReadKey(true);
                    if (PressedKey.Key == ConsoleKey.Escape)
                    {
                        return;
                    }
                    else if (PressedKey.Key == ConsoleKey.RightArrow)
                    {
                        ReportLibStats(2);
                    }
                }
                else if (PageNo == 2)
                {
                    Console.Clear();
                    Console.WriteLine("Users Who Are Borrowing Books:\n");
                    ViewBorrowingUsersStats();
                    Console.WriteLine("\n\nPage 2 of 4");
                    Console.WriteLine("Page 1 (<- Left Arrow Key) | (Right Arrow Key ->) Page 3 | (Esc) to Exit");
                    var PressedKey = Console.ReadKey(true);
                    if (PressedKey.Key == ConsoleKey.Escape)
                    {
                        return;
                    }
                    else if (PressedKey.Key == ConsoleKey.RightArrow)
                    {
                        ReportLibStats(3);
                    }
                    else if (PressedKey.Key == ConsoleKey.LeftArrow)
                    {
                        ReportLibStats(1);
                    }
                }
                else if (PageNo == 3)
                {
                    Console.Clear();
                    Console.WriteLine("Books Currently Borrowed:\n");
                    ViewBorrowedBooksStats();
                    Console.WriteLine("\n\nPage 3 of 4");
                    Console.WriteLine("Page 2 (<- Left Arrow Key) | (Right Arrow Key ->) Page 4 | (Esc) to Exit");
                    var PressedKey = Console.ReadKey(true);
                    if (PressedKey.Key == ConsoleKey.Escape)
                    {
                        return;
                    }
                    else if (PressedKey.Key == ConsoleKey.RightArrow)
                    {
                        ReportLibStats(4);
                    }
                    else if (PressedKey.Key == ConsoleKey.LeftArrow)
                    {
                        ReportLibStats(2);
                    }
                }
                else if (PageNo == 4)
                {
                    Console.Clear();
                    Console.WriteLine("Most Common User / Book:\n");
                    MostCommonUserAndBook();
                    Console.WriteLine("\n\nPage 4 of 4");
                    Console.WriteLine("Page 3 (<- Left Arrow Key) | (Esc) to Exit");
                    var PressedKey = Console.ReadKey(true);
                    if (PressedKey.Key == ConsoleKey.Escape)
                    {
                        return;
                    }
                    else if (PressedKey.Key == ConsoleKey.LeftArrow)
                    {
                        ReportLibStats(3);
                    }
                }
        }
        static void MostCommonUserAndBook() // prints the most borrowed book / the most frequent user
        {
            int TopUser = 0;
            int TopBook = 0;
            int TopUserID = 0;
            int TopBookID = 0;
            for (int i = 0;i < Borrows.Count;i++)
            {
                int BookOccur = 0;
                int UserOccur = 0;
                for (int j = 0; j < Borrows.Count; j++)
                {
                    if ((Borrows[i].UserID == Borrows[j].UserID) && (i != j))
                    {
                        UserOccur++;
                    }
                    if ((Borrows[i].BookID == Borrows[j].BookID) && (i != j))
                    {
                        BookOccur++;
                    }
                }
                if (UserOccur > TopUser)
                {
                    TopUser = UserOccur;
                    TopUserID = Borrows[i].UserID;
                }
                if (BookOccur > TopBook)
                {
                    TopBook = BookOccur;
                    TopBookID = Borrows[i].BookID;
                }
            }
            if ((TopUser != 0) && (TopBook != 0))
            {
                int UserIndex = -1;
                int BookIndex = -1;
                for (int i = 0; i < Books.Count;i++)
                {
                    if (TopBookID == Books[i].BookID)
                    {
                        BookIndex = i;
                        break;
                    }
                }

                for (int i = 0; i < Users.Count;i++)
                {
                    if (TopUserID == Users[i].UserID)
                    {
                        UserIndex = i;
                        break;
                    }
                }
                Console.WriteLine($"\nNumber of unique books in the library: {Books.Count}");
                ViewCategories();
                Console.WriteLine($"\n\nMost Common User: ID: {TopUserID} | Name: {Users[UserIndex].UserName} | Email: {Users[UserIndex].UserEmail}");
                Console.WriteLine($"\n\nMost Common Book: ID: {TopBookID} | Name: {Books[BookIndex].BookName} | Author: {Books[BookIndex].AuthName} | Category: {Books[BookIndex].Category}");
            }
        }
        static void ViewBorrowedBooksStats()
        {
            StringBuilder sb = new StringBuilder();
            bool BorrowedBooks = false;
            for (int i = 0; i < Books.Count;i++)
            {
                if (Books[i].BorrowedCpy > 0)
                {
                    sb.AppendLine($"ID: {Books[i].BookID} | Name: {Books[i].BookName} | Total Copies: {Books[i].Cpy} | Copies Borrowed: {Books[i].BorrowedCpy} | Available Copies: {Books[i].Cpy - Books[i].BorrowedCpy}");
                }
            }
            Console.WriteLine(sb.ToString());
        }
        static void ViewBorrowingUsersStats()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Borrows.Count; i++)
            {
                if (!Borrows[i].IsReturned)
                {
                    sb.AppendLine($"\nUser ID: {Borrows[i].UserID} | Book ID: {Borrows[i].BookID} | Borrowed on: {Borrows[i].BorrowDate} | Due Date: {Borrows[i].DueDate} | Days Left: {(DateTime.Parse(Borrows[i].DueDate) - DateTime.Now).Days}");
                }
            }
            Console.WriteLine(sb.ToString());
        }
        static void UserIndividualReport(int UID)
        {
            StringBuilder BorrowedAndReturned = new StringBuilder();
            bool FoundReturned = false;
            StringBuilder BorrowedNOTReturned = new StringBuilder();
            bool FoundNotReturned = false;
            StringBuilder BooksOverDue = new StringBuilder();
            bool OverDue = false;
            StringBuilder UserInfo = new StringBuilder();
            for(int i = 0;i < Borrows.Count;++i)
            {
                if ((UID == Borrows[i].UserID) && (Borrows[i].IsReturned == true))
                {
                    BorrowedAndReturned.AppendLine($"Book ID: {Borrows[i].BookID} | Borrow Date: {Borrows[i].BorrowDate} | Returned On: {Borrows[i].ReturnDate} | Rating: {Borrows[i].BRating}");
                    FoundReturned = true;
                }
                if ((UID == Borrows[i].UserID) && (Borrows[i].IsReturned == false))
                {
                    BorrowedNOTReturned.AppendLine($"Book ID: {Borrows[i].BookID} | Borrow Date: {Borrows[i].BorrowDate} | Due Date: {Borrows[i].DueDate} | Days Left: {(DateTime.Parse(Borrows[i].DueDate) - DateTime.Now).Days}");
                    FoundNotReturned = true;

                    if (DateTime.Parse(Borrows[i].DueDate) < DateTime.Now)
                    {
                        BooksOverDue.AppendLine($"Book ID: {Borrows[i].BookID} | Book Borrow Date: {Borrows[i].BorrowDate}");
                        OverDue = true;
                    }
                }
            }

            for (int i = 0;i<Users.Count;++i)
            {
                if (Users[i].UserID == UID)
                {
                    UserInfo.AppendLine($"User ID: {Users[i].UserID} | User Name: {Users[i].UserName} | Email: {Users[i].UserEmail} | Password: {Users[i].UserPass}");
                    break;
                }
            }
            Console.Clear();
            Console.WriteLine("User Details: "+UserInfo.ToString());
            if (FoundReturned)
            {
                Console.WriteLine("\nBorrowed and Returned Books:\n" + BorrowedAndReturned.ToString());
            }
            if (FoundNotReturned)
            {
                Console.WriteLine("\nCurrently Borrowed Books: \n" + BorrowedNOTReturned.ToString());
            }
            if (OverDue)
            {
                Console.WriteLine("!BOOKS OVERDUE!\n" + BooksOverDue.ToString());
            }
        }
        //static (bool, List<(string, string, string)>) RequestAdminAccount(string AName, string AEmail, string APass)
        //{
        //    List<(string Name, string Email, string Pass)> AdminRequests = new List<(string Name, string Email, string Pass)>();
        //    bool AdminRequestFlag = false;
            
        //    return (AdminRequestFlag, AdminRequests);
        //}
        static (bool, string)EntryValidation(List<(int, string, string, string)> ListToCheck, string ItemToCheck, int CheckOperation)
        {
            if (CheckOperation == 1)
            {
                if (!Regex.IsMatch(ItemToCheck, NameCheck))
                {
                    return (false, "Invalid Name, Please Try Again.");
                }
                else
                {
                    for (int i = 0; i < ListToCheck.Count; i++)
                    {
                        if (ListToCheck[i].Item2.ToLower().Trim() == ItemToCheck.ToLower().Trim())
                        {
                            return (false, "Name Already Exists, Please Try Again.");
                        }
                    }
                }
            }
            else if (CheckOperation == 2)
            {
                if (!Regex.IsMatch(ItemToCheck, EmailFormat))
                {
                    return (false, "Invalid Email Pattern, Please Try Again.");
                }
                else
                {
                    for (int i = 0; i < ListToCheck.Count; i++)
                    {
                        if (ListToCheck[i].Item3.ToLower().Trim() == ItemToCheck.ToLower().Trim())
                        {
                            return (false, "Email Already Exists, Please Try Again.");
                        }
                    }
                }
            }
            else if (CheckOperation == 3)
            {
                if (!Regex.IsMatch(ItemToCheck, PassFormat))
                {
                    return (false, "Password does not comply with the standards, please try again.");
                }
            }
            return (true, ItemToCheck);
        }
    }
}
