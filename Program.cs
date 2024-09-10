using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace BasicLibrary
{
    internal class Program
    {
        // GLOBAL VARIABLES.
        static int CurrentUser=-1; // saves the id of current user
        static List<(int UserID, string UserName, string UserEmail, string UserPass)> Users = new List<(int UserID, string UserName, string UserEmail, string UserPass)>(); // Users list
        static List<(int AdminID, string AdminName, string AdminEmail, string AdminPass)> Admins = new List<(int AdminID, string AdminName, string AdminEmail, string AdminPass)>() ; // Admins list
        static List<(string BName, string BAuthor, int ID, int Qty)> Books = new List<(string BName, string BAuthor, int ID, int Qty)>(); // Books list
        static List<(int UserID, int BookID, string BookName, int BorrowQty)> Borrows = new List<(int UserID, int BookID, string BookName, int BorrowQty)>(); // Current borrows list
        static List<(int UserID, int BookID, string BookName)> RecommendationSource = new List<(int UserID, int BookID, string BookName)>() ; // all borrows list


        // FILE PATHS.
        static string filePath = "C:\\Users\\Lenovo\\Desktop\\Ibrahim_Projects\\LibrarySystemFiles\\LibraryBooks.txt"; // Library books are saved here.
        static string adminsPath = "C:\\Users\\Lenovo\\Desktop\\Ibrahim_Projects\\LibrarySystemFiles\\LibraryAdmins.txt"; // Admins are saved here.
        static string UsersPath = "C:\\Users\\Lenovo\\Desktop\\Ibrahim_Projects\\LibrarySystemFiles\\LibraryUsers.txt"; // Users are saved here
        static string BorrowListPath = "C:\\Users\\Lenovo\\Desktop\\Ibrahim_Projects\\LibrarySystemFiles\\BorrowList.txt"; // Users currently borrowing books are saved here.
        static string RecommendationSourcePath = "C:\\Users\\Lenovo\\Desktop\\Ibrahim_Projects\\LibrarySystemFiles\\RecommendationSourceList.txt"; // The history of all borrowings is saved here.



        // MAIN FUNCTION.
        static void Main(string[] args)
        {
            //Loading the .txt files into the lists of tuples.
            LoadAdminsFromFile();
            LoadUsersFromFile();
            LoadBooksFromFile();
            LoadBorrowedListFromFile();
            LoadRecommendationSourceFromFile();
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
            bool AdminFlag = false;
            bool MasterAdminFlag = false;
            Console.WriteLine($"!!TOP SECRET!! DO NOT SHARE\n(Master Admin Email: {Admins[0].AdminEmail})");
            Console.WriteLine("\nEnter Admin ID:");
            while (string.IsNullOrEmpty(AdminID = Console.ReadLine().ToLower()))
            {
                Console.WriteLine("\nInvalid input, please try again:");
            }
            Console.WriteLine("\nEnter Admin Password (Hint: admin):");
            while (string.IsNullOrEmpty(AdminPass = Console.ReadLine().ToLower()))
            {
                Console.WriteLine("\nInvalid input, please try again:");
            }
            for (int i = 0; i < Admins.Count; i++)
            {
                if ((Admins[i].AdminEmail.ToLower() == AdminID) && (Admins[i].AdminPass.ToLower() == AdminPass)) //check if admin exist
                {
                    AdminFlag = true;
                }
                if (AdminFlag) // if admin id and pass are correct, check if it is Master admin (first admin)
                {
                    if (i == 0)
                    {
                        MasterAdminFlag = true;
                    }
                }
            }
            if (AdminFlag && !MasterAdminFlag)
            {
                AdminMenu();
            }
            else if (AdminFlag && MasterAdminFlag)
            {
                MasterAdmin();
            }
            else
            {
                Console.WriteLine("\nInvalid Admin Email or Password, please try again.");
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
                        ManageAdmins();
                        break;

                    case 6:
                        ManageUsers();
                        break;

                    case 7:
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
            List<string> ExistingAdmins = new List<string>();
            for (int i = 0; i < Admins.Count; i++)
            {
                ExistingAdmins.Add(Admins[i].AdminEmail.ToLower());
            } // Adding current admins Ids to a temporary list to prevent dublicate admin Ids

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
            string NewAdminName;
            while ((string.IsNullOrEmpty(NewAdminName = Console.ReadLine().ToLower())))
            {
                Console.WriteLine("\nInvalid name, please try again:");
            }
            Console.WriteLine("\nEnter new Admin Email:");
            string NewAdminEmail;
            while((string.IsNullOrEmpty(NewAdminEmail = Console.ReadLine().ToLower())) || (ExistingAdmins.Contains(NewAdminEmail.ToLower())))
            {
                Console.WriteLine("\nInvalid Email, please try again:");
            }
            Console.WriteLine($"\nEnter the password for {NewAdminEmail}:");
            string NewAdminPass;
            while (string.IsNullOrEmpty(NewAdminPass = Console.ReadLine().ToLower()))
            {
                Console.WriteLine("\nInvalid Password, please try again:");
            }

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
                        string NewName;
                        while ((string.IsNullOrEmpty(NewName = Console.ReadLine())))
                        {
                            Console.WriteLine("\nInvalid input, please try again:");
                        }
                        string OldName = Admins[ChosenAdmin - 1].AdminEmail;
                        Admins[ChosenAdmin - 1] = (Admins[ChosenAdmin - 1].AdminID, NewName, Admins[ChosenAdmin - 1].AdminEmail, Admins[ChosenAdmin - 1].AdminPass);
                        Console.WriteLine($"\nAdmin \"{OldName}\" Name changed to: \"{NewName}\".");
                        break;

                    case 2:
                        List<string> ExistingAdmins = new List<string>();
                        for (int i = 0; i < Admins.Count; i++)
                        {
                            ExistingAdmins.Add(Admins[i].AdminEmail.ToLower());
                        }
                        Console.WriteLine($"\nEnter the new Email for {Admins[ChosenAdmin - 1].AdminEmail}: ");
                        string NewEmail;
                        while ((string.IsNullOrEmpty(NewEmail = Console.ReadLine().ToLower())) || (ExistingAdmins.Contains(NewEmail)))
                        {
                            Console.WriteLine("\nInvalid input, please try again:");
                        }
                        string OldEmail = Admins[ChosenAdmin - 1].AdminEmail;
                        Admins[ChosenAdmin - 1] = (Admins[ChosenAdmin - 1].AdminID, Admins[ChosenAdmin - 1].AdminName, NewEmail, Admins[ChosenAdmin - 1].AdminPass);
                        Console.WriteLine($"\nAdmin \"{OldEmail}\" Email changed to: {NewEmail}.");
                        break;

                    case 3:
                        Console.WriteLine($"\nEnter the new Password for {Admins[ChosenAdmin - 1].AdminEmail}: ");
                        string NewPass;
                        while (string.IsNullOrEmpty(NewPass = Console.ReadLine().ToLower()))
                        {
                            Console.WriteLine("\nInvalid input, please try again:");
                        }
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
                Console.WriteLine("\n5. Manage Usera.");
                Console.WriteLine("\n6. View Library Report.");
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
                Console.WriteLine("\nChoose an option:\n1. Register new User.\n2. Edit existing User.\n\n0. Save & Exit.");
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
            List<string> ExistingUsers = new List<string>();
            for (int i = 0; i < Users.Count; i++)
            {
                ExistingUsers.Add(Users[i].UserEmail.ToLower());
            }
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
            string NewUserName;
            while ((string.IsNullOrEmpty(NewUserName = Console.ReadLine().ToLower())))
            {
                Console.WriteLine("\nInvalid Name, please try again:");
            }
            Console.WriteLine("\nEnter new User Email:");
            string NewUserEmail;
            while ((string.IsNullOrEmpty(NewUserEmail = Console.ReadLine().ToLower())) || (ExistingUsers.Contains(NewUserEmail)))
            {
                Console.WriteLine("\nInvalid Email, please try again:");
            }
            Console.WriteLine($"\nEnter the password for {NewUserEmail}:");
            string NewUserPass;
            while (string.IsNullOrEmpty(NewUserPass = Console.ReadLine().ToLower()))
            {
                Console.WriteLine("\nInvalid Password, please try again:");
            }
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
                        string NewName;
                        while (string.IsNullOrEmpty(NewName = Console.ReadLine()))
                        {
                            Console.WriteLine("\nInvalid input, please try again:");
                        }
                        string OldName = Users[ChosenUser - 1].UserName;
                        Users[ChosenUser - 1] = (Users[ChosenUser - 1].UserID, NewName, Users[ChosenUser - 1].UserEmail, Users[ChosenUser - 1].UserPass);
                        Console.WriteLine($"\n\"{Users[ChosenUser - 1].UserID}\" Password changed from: {OldName} to: {NewName}.");
                        break;

                    case 2:
                        List<string> ExistingUsers = new List<string>();
                        for (int i = 0; i < Users.Count; i++)
                        {
                            ExistingUsers.Add(Users[i].UserEmail.ToLower());
                        }
                        Console.WriteLine($"\nEnter the new Email for user {Users[ChosenUser - 1].UserID}: ");
                        string NewEmail;
                        while ((string.IsNullOrEmpty(NewEmail = Console.ReadLine().ToLower())) || (ExistingUsers.Contains(NewEmail)))
                        {
                            Console.WriteLine("\nInvalid input, please try again:");
                        }
                        Users[ChosenUser - 1] = (Users[ChosenUser - 1].UserID, Users[ChosenUser - 1].UserName, NewEmail, Users[ChosenUser - 1].UserPass);
                        Console.WriteLine($"\nUser \"{Users[ChosenUser - 1].UserID}\" Email changed to: {NewEmail}.");
                        break;

                    case 3:
                        Console.WriteLine($"\nEnter the new Password for user {Users[ChosenUser - 1].UserID}: ");
                        string NewPass;
                        while (string.IsNullOrEmpty(NewPass = Console.ReadLine().ToLower()))
                        {
                            Console.WriteLine("\nInvalid input, please try again:");
                        }
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
            while (string.IsNullOrEmpty(UsrPass = Console.ReadLine().ToLower()))
            {
                Console.WriteLine("\nInvalid input, please try again:");
            }
            for (int i = 0; i < Users.Count; i++)
            {
                if ((Users[i].UserEmail == UsrEmail) && (Users[i].UserPass == UsrPass))
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
                Console.Clear();
                Console.WriteLine("Welcome to the Library!");
                Console.WriteLine("\nEnter the number of the service required:");
                Console.WriteLine("\n1. Search book");
                Console.WriteLine("\n2. Borrow book");
                Console.WriteLine("\n3. Return book");
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

                    case 0:
                        SaveBooksToFile();
                        CurrentUser = -1;
                        ExitFlag = true;
                        break;

                    default:
                        Console.WriteLine("\nInvalid choice, please try again...");
                        break;
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



        // BOOKS RELATED FUNCTIONS.
        static void AddnNewBook()
        {
            List<string> ExistingBooks = new List<string>();
            for (int i = 0; i < Books.Count; i++)
            {
                ExistingBooks.Add(Books[i].BName.ToLower());
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
                ID = Books[Books.Count - 1].ID + 1;
            }
            else
            {
                ID = 1;
            }
            Console.WriteLine($"\nEnter available quantity of \"{name}\": ");
            int Qty;
            while ((!int.TryParse(Console.ReadLine(), out Qty)) || (Qty < 1))
            {
                Console.WriteLine("\nInvalid input, please try again:");
            }
            Books.Add(( name, author, ID,  Qty));
            Console.WriteLine($"\nBook \"{name}\" Added Succefully");
        }
        static void ViewAllBooks()
        {
            StringBuilder sb = new StringBuilder();

            int BookNumber = 0;

            for (int i = 0; i < Books.Count; i++)
            {             
                BookNumber = i + 1;
                sb.AppendLine($"{BookNumber}. Book: {Books[i].BName} | Author: {Books[i].BAuthor} | ID: {Books[i].ID} | Qty: {Books[i].Qty}\n");
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
            List<int> BookIds = new List<int>();
            StringBuilder sb = new StringBuilder();
            sb.Clear();
            for (int i = 0; i< Books.Count;i++)
            {
                if (Books[i].BName.ToLower() == name.ToLower()) // prints book if the searched name is a book name
                {
                    Console.WriteLine($"\nBook details:" +
                        $"\nName: {Books[i].BName} | Author: {Books[i].BAuthor} | Qty: {Books[i].Qty}");
                    BookIndex = i;
                    flag = true;
                    break;
                }
                if (Books[i].BAuthor.ToLower() == name.ToLower()) // prints a list of books made by the author if author name was searched.
                {
                    sb.AppendLine($"{count}. Book Name: {Books[i].BName} | Quantity: {Books[i].Qty}");
                    count++;
                    AuthBooks = true;
                    BookIds.Add( i );
                }

            }
            if (AuthBooks && AdmnOrUsr) // if the person searching is a user
            {
                Console.WriteLine("\nChoose a book to borrow:");
                Console.WriteLine(sb.ToString());
                int BookChoice;
                while((!int.TryParse(Console.ReadLine(), out BookChoice))||(BookChoice < 1) ||(BookChoice > BookIds.Count))
                {
                    Console.WriteLine("\nInvalid input, please try again:");
                }
                for (int i = 0; i<BookIds.Count;i++)
                {
                    if((BookChoice - 1) == BookIds[i])
                    {
                        BookIndex = BookIds[i];
                        break;
                    }
                }
            }

            if (!flag && !AuthBooks) // if book is not found
            { 
                Console.WriteLine("\nBook not found"); 
            }
            else if (!AdmnOrUsr && (AuthBooks || flag)) // if the person searching is an admin only print book(s) info and stop
            {
                Console.WriteLine(sb.ToString());
            }
            else
            {
                if (Books[BookIndex].Qty <= 0)
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
                Console.WriteLine("\nEnter the quantity to borrow:");
                int BorrowQty;
                while ((!int.TryParse(Console.ReadLine(), out BorrowQty)) || (BorrowQty < 1) || (BorrowQty > Books[BookIndex].Qty) || (BorrowQty > 5))
                {
                    Console.WriteLine("\nInvalid input or exceeds limit, please try again:");
                }

                bool BorrowedBefore = false;
                int BorrowedBeforeIndex = -1;
                Books[BookIndex] = (Books[BookIndex].BName, Books[BookIndex].BAuthor, Books[BookIndex].ID, (Books[BookIndex].Qty - BorrowQty));
                for (int i = 0; i < Borrows.Count; i++)
                {
                    if (Borrows[i].UserID == CurrentUser && Borrows[i].BookID == Books[BookIndex].ID)
                    {
                        BorrowedBefore = true;
                        BorrowedBeforeIndex = i;
                        break;
                    }
                }
                if (BorrowedBefore)
                {
                    Borrows[BorrowedBeforeIndex] = (Borrows[BorrowedBeforeIndex].UserID, Borrows[BorrowedBeforeIndex].BookID, Borrows[BorrowedBeforeIndex].BookName, (Borrows[BorrowedBeforeIndex].BorrowQty + BorrowQty));
                }
                else
                {
                    Borrows.Add((CurrentUser, Books[BookIndex].ID, Books[BookIndex].BName, BorrowQty));
                }
                
                RecommendationSource.Add((CurrentUser, Books[BookIndex].ID, Books[BookIndex].BName));
                Console.Clear();
                Console.WriteLine($"\n{BorrowQty} x {Books[BookIndex].BName} borrowed successfully!");
                SaveBorrowedListToFile();
                SaveBooksToFile();
                SaveRecommendationSourceToFile(CurrentUser, Books[BookIndex].ID, Books[BookIndex].BName);
                RecommendationBooks(Books[BookIndex].ID, Books[BookIndex].BName, CurrentUser);
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
                if (Borrows[i].UserID == CurrentUser)
                {
                    IDs.Add(Borrows[i].BookID);
                    UserBorrowed = true;
                    sb.AppendLine($"ID: {Borrows[i].BookID} | Name: {Borrows[i].BookName} | Qty: {Borrows[i].BorrowQty}");
                }
            }
            if (UserBorrowed)
            {
                Console.WriteLine(sb.ToString());
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
                        if ((Borrows[i].UserID == CurrentUser) && (Borrows[i].BookID == BookChoice))
                        {
                            BorrowedIndex = i;
                            break;
                        }
                    }
                    Console.WriteLine("\nEnter the quantity to return:");
                    int ReturnQty;
                    while ((!int.TryParse(Console.ReadLine(), out ReturnQty)) || (ReturnQty < 0) || (ReturnQty > Borrows[BorrowedIndex].BorrowQty))
                    {
                        Console.WriteLine("\nInvalid input or exceeds amount, please try again: ");
                    }
                    if (ReturnQty == 0)
                    {
                        Console.WriteLine("\nCancelling return process...");
                    }
                    else
                    {
                        try
                        {
                            if (Borrows[BorrowedIndex].BorrowQty == ReturnQty)
                            {
                                Borrows.RemoveAt(BorrowedIndex);
                            }
                            else
                            {
                                Borrows[BorrowedIndex] = (Borrows[BorrowedIndex].UserID, Borrows[BorrowedIndex].BookID, Borrows[BorrowedIndex].BookName, (Borrows[BorrowedIndex].BorrowQty - ReturnQty));
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"\nCould not update borrows list...{ex}");
                        }

                        for (int i = 0; i < Books.Count; i++)
                        {
                            if (Books[i].ID == BookChoice)
                            {
                                Books[i] = (Books[i].BName, Books[i].BAuthor, Books[i].ID, (Books[i].Qty + ReturnQty));
                                break;
                            }
                        }

                        Console.WriteLine($"\n{ReturnQty} x {Books[BookChoice - 1].BName} returned successfully!");
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
                            if (parts.Length == 4)
                            {
                                Books.Add((parts[0], parts[1], int.Parse(parts[2]), int.Parse(parts[3])));
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
                        writer.WriteLine($"{book.BName}|{book.BAuthor}|{book.ID}|{book.Qty}");
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
                Console.WriteLine("\nChoose an editing option:\n1. Edit Book Name.\n2. Edit Book Author.\n3. Add quantity.\n4. Remove Book.\n\n0. Exit.");
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
                            ExistingBooks.Add(Books[i].BName.ToLower());
                        }
                        Console.WriteLine($"\nEnter the new name for {Books[ChosenBook - 1].BName}: ");
                        string NewName;
                        while((string.IsNullOrEmpty(NewName = Console.ReadLine())) || (ExistingBooks.Contains(NewName.ToLower())))
                        {
                            Console.WriteLine("\nInvalid input, please try again:");
                        }
                        string OldBName = Books[ChosenBook - 1].BName;
                        Books[ChosenBook - 1] = (NewName, Books[ChosenBook - 1].BAuthor, Books[ChosenBook - 1].ID, Books[ChosenBook - 1].Qty);
                        Console.WriteLine($"\nBook \"{OldBName}\" name changed to: {NewName}.");
                        break;

                    case 2:
                        Console.WriteLine($"\nEnter the new author name for {Books[ChosenBook - 1].BName}: ");
                        string NewAuth;
                        while (string.IsNullOrEmpty(NewAuth = Console.ReadLine()))
                        {
                            Console.WriteLine("\nInvalid input, please try again:");
                        }
                        string OldAuth = Books[ChosenBook - 1].BAuthor;
                        Books[ChosenBook - 1] = (Books[ChosenBook - 1].BName, NewAuth, Books[ChosenBook - 1].ID, Books[ChosenBook - 1].Qty);
                        Console.WriteLine($"\n\"{Books[ChosenBook - 1].BName}\" Author changed from: {OldAuth} to: {NewAuth}.");
                        break;

                    case 3:
                        Console.WriteLine($"\nEnter the additional quantity for {Books[ChosenBook - 1].BName}: ");
                        int NewQty;
                        while ((!int.TryParse(Console.ReadLine(), out NewQty))||(NewQty < 1))
                        {
                            Console.WriteLine("\nInvalid input, please try again:");
                        }
                        Books[ChosenBook - 1] = (Books[ChosenBook - 1].BName, Books[ChosenBook - 1].BAuthor, Books[ChosenBook - 1].ID, (Books[ChosenBook - 1].Qty + NewQty));
                        Console.WriteLine($"\n{Books[ChosenBook - 1].BName} Quantity has been increased to {Books[ChosenBook - 1].Qty} successfully.");
                        break;

                    case 4:
                        string RemovedBook = Books[ChosenBook - 1].BName;
                        Books.RemoveAt(ChosenBook - 1);
                        Console.WriteLine($"\nBook \"{RemovedBook}\" has been removed from the library File.");
                        break;

                    default:
                        Console.WriteLine("\nInvalid input, please try again.");
                        break;
                }
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            } while (!ExitFlag);
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
                        writer.WriteLine($"{Borrow.UserID}|{Borrow.BookID}|{Borrow.BookName}|{Borrow.BorrowQty}");
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
                            if (parts.Length == 4)
                            {
                                Borrows.Add((int.Parse(parts[0]), int.Parse(parts[1]), parts[2], int.Parse(parts[3])));
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
        static void SaveRecommendationSourceToFile(int UID, int BID, string BName)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(RecommendationSourcePath, true))
                {
                    writer.WriteLine($"{UID}|{BID}|{BName}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving to file: {ex.Message}");
            }
        }
        static void LoadRecommendationSourceFromFile()
        {
            try
            {
                if (File.Exists(RecommendationSourcePath))
                {
                    using (StreamReader reader = new StreamReader(RecommendationSourcePath))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            var parts = line.Split('|');
                            if (parts.Length == 3)
                            {
                                RecommendationSource.Add((int.Parse(parts[0]), int.Parse(parts[1]), parts[2]));
                            }
                        }
                    }
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
            List<(int ID, string name)> OtherBooksUsersBorrowed = new List<(int ID, string name)>();
            
            
            for (int i = 0; i < RecommendationSource.Count; i++) // add users' Ids of other people who borrowed the same book.
            {
                if ((RecommendationSource[i].BookID == BorrowedBookID) && (RecommendationSource[i].UserID != UserID))
                {
                    UsersWhoBorrowedBook.Add(RecommendationSource[i].UserID);
                    FoundBorrowedBook = true;
                }
            }
            if (FoundBorrowedBook)
            {
                for (int i = 0; i < UsersWhoBorrowedBook.Count; i++) // add the other books borrowed by the people who borrowed the same book to a list
                {
                    for(int j = 0; j < RecommendationSource.Count; j++)
                    {
                        if ((RecommendationSource[j].UserID == UsersWhoBorrowedBook[i]) && (RecommendationSource[j].BookID != BorrowedBookID) && (!OtherBooksUsersBorrowed.Contains((RecommendationSource[j].BookID, RecommendationSource[j].BookName))))
                        {
                            OtherBooksUsersBorrowed.Add((RecommendationSource[j].BookID, RecommendationSource[j].BookName));
                        }
                    }
                }
                int count = 1;
                sb.Clear();
                for (int i = 0; i < OtherBooksUsersBorrowed.Count; i++)
                {
                    sb.AppendLine(count+". Book Name: "+OtherBooksUsersBorrowed[i].name + " | ID:"+ OtherBooksUsersBorrowed[i].ID);
                    count++;
                    if(count == 5)
                    {
                        break;
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
                            if (OtherBooksUsersBorrowed[Choice - 1].ID == Books[i].ID)
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
            string TopBookName = "";
            for (int i = 0;i < RecommendationSource.Count;i++)
            {
                int BookOccur = 0;
                int UserOccur = 0;
                for (int j = 0; j < RecommendationSource.Count; j++)
                {
                    if ((RecommendationSource[i].UserID == RecommendationSource[j].UserID) && (i != j))
                    {
                        UserOccur++;
                    }
                    if ((RecommendationSource[i].BookName == RecommendationSource[j].BookName) && (i != j))
                    {
                        BookOccur++;
                    }
                }
                if (UserOccur > TopUser)
                {
                    TopUser = UserOccur;
                    TopUserID = RecommendationSource[i].UserID;
                }
                if (BookOccur > TopBook)
                {
                    TopBook = BookOccur;
                    TopBookName = RecommendationSource[i].BookName;
                }
            }
            if ((TopUser != 0) && (TopBook != 0))
            {
                Console.WriteLine($"\n\nMost Common User: User with ID {TopUserID}");
                Console.WriteLine($"\nMost Common Book: {TopBookName}");
            }
        }
        static void ViewBorrowedBooksStats()
        {
            StringBuilder sb = new StringBuilder();
            List<(int ID, string Name)> TotalBorrowed = new List<(int ID, string Name)>();
            for (int i = 0; i < Books.Count;i++)
            {
                bool FoundBook = false;
                int BookSum = 0;
                for (int j = 0; j < Borrows.Count;j++)
                {
                    if (Borrows[j].BookID == Books[i].ID)
                    {
                        FoundBook = true;
                        BookSum += Borrows[j].BorrowQty;
                    }
                }
                if (FoundBook)
                {
                    sb.AppendLine($"\nBook ID: {Books[i].ID} | Name: {Books[i].BName} | Amount Borrowed: {BookSum} | Amount Available: {Books[i].Qty}");
                }
            }
            Console.WriteLine(sb.ToString());
        }
        static void ViewBorrowingUsersStats()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Borrows.Count; i++)
            {
                sb.AppendLine($"\nUser ID: {Borrows[i].UserID} | Book Borrowed: {Borrows[i].BookName} x {Borrows[i].BorrowQty}");
            }
            Console.WriteLine(sb.ToString());
        }
    }
}
