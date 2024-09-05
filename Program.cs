using System.Security.Cryptography;
using System.Text;

namespace BasicLibrary
{
    internal class Program
    {
        static int CurrentUser = -1;
        static List<(int UserID, string UserEmail, string UserPass)> Users = new List<(int UserID, string UserEmail, string UserPass)>();
        static List<(string AdminEmail, string AdminPass)> Admins = new List<(string AdminEmail, string AdminPass)>() ;
        static List<(string BName, string BAuthor, int ID, int Qty)> Books = new List<(string BName, string BAuthor, int ID, int Qty)>();
        static string filePath = "C:\\Users\\Lenovo\\Desktop\\Ibrahim_Projects\\LibrarySystemFiles\\LibraryBooks.txt";
        static string adminsPath = "C:\\Users\\Lenovo\\Desktop\\Ibrahim_Projects\\LibrarySystemFiles\\LibraryAdmins.txt";
        static string UsersPath = "C:\\Users\\Lenovo\\Desktop\\Ibrahim_Projects\\LibrarySystemFiles\\LibraryUsers.txt";

        static void Main(string[] args)
        {
            Admins.Add(("Admin@BusaidiStore.com", "admin"));
            LoadBooksFromFile();
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
                        AdminMenu();
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

        static void AdminMenu()
        {
            bool ExitFlag = false;
            do
            {
                Console.Clear();
                Console.WriteLine("Welcome!\nAdmin Authorized:");
                Console.WriteLine("\nEnter the number of the operation to perform:");
                Console.WriteLine("\n1. Add New Book");
                Console.WriteLine("\n2. Display All Books");
                Console.WriteLine("\n3. Search for Book");
                Console.WriteLine("\n\n0. Exit");

                int choice;
                while (!int.TryParse(Console.ReadLine(), out choice))
                {
                    Console.WriteLine("Invalid input, please try again: ");
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

                    case 0:
                        SaveBooksToFile();
                        ExitFlag = true;
                        break;

                    default:
                        Console.WriteLine("Invalid choice, please try again.");
                        break;



                }

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();

            } while (ExitFlag != true);
        }

        static void UserMenu()
        {
            bool ExitFlag = false;
            do
            {
                Console.Clear();
                Console.WriteLine("Welcome to the Lirary!");
                Console.WriteLine("\nEnter the number of the service required:");
                Console.WriteLine("\n1. Search book");
                Console.WriteLine("\n2. Borrow book");
                Console.WriteLine("\n3. Return book");
                Console.WriteLine("\n0. Exit");

                int choice;
                while (!int.TryParse(Console.ReadLine(), out choice))
                {
                    Console.WriteLine("Invalid input, please try again: ");
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
                        ExitFlag = true;
                        break;

                    default:
                        Console.WriteLine("Invalid choice, please try again...");
                        break;
                }
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey ();
            } while (ExitFlag != true);
        }

        static void AddnNewBook()
        { 
            Console.WriteLine("Enter Book Name");
            string name;
            while (string.IsNullOrEmpty(name = Console.ReadLine()))
            {
                Console.WriteLine("Invalid Input, please try again: ");
            }

            Console.WriteLine($"Enter Author Name of \"{name}\": ");
            string author;
            while (string.IsNullOrEmpty(author = Console.ReadLine()))
            {
                Console.WriteLine("Invalid Input, please try again: ");
            }

            Console.WriteLine($"Enter ID of \"{name}\": ");
            int ID;
            while ((!int.TryParse(Console.ReadLine(), out ID)) || (ID < 0))
            {
                Console.WriteLine("Invalid input, please try again:");
            }

            Console.WriteLine($"Enter available quantity of \"{name}\": ");
            int Qty;
            while ((!int.TryParse(Console.ReadLine(), out Qty)) || (Qty < 1))
            {
                Console.WriteLine("Invalid input, please try again:");
            }
            Books.Add(( name, author, ID,  Qty));
            Console.WriteLine($"Book \"{name}\" Added Succefully");

        }

        static void ViewAllBooks()
        {
            StringBuilder sb = new StringBuilder();

            int BookNumber = 0;

            for (int i = 0; i < Books.Count; i++)
            {             
                BookNumber = i + 1;
                sb.Append("Book ").Append(BookNumber).Append(" name : ").Append(Books[i].BName);
                sb.AppendLine();
                sb.Append(" Author : ").Append(Books[i].BAuthor);
                sb.AppendLine();
                sb.Append(" ID : ").Append(Books[i].ID);
                sb.AppendLine();
                sb.Append(" Qty : ").Append (Books[i].Qty);
                sb.AppendLine().AppendLine();
                Console.WriteLine(sb.ToString());
                sb.Clear();

            }
        }

        static void SearchForBook(bool AdmnOrUsr)
        {
            Console.WriteLine("Enter the book or author name to search");
            string name;
            while (string.IsNullOrEmpty(name = Console.ReadLine()))
            {
                Console.WriteLine("Invalid Input, please try again: ");
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
                if (Books[i].BName == name)
                {
                    Console.WriteLine($"Book details:" +
                        $"\nName: {Books[i].BName} | Author: {Books[i].BAuthor} | Qty: {Books[i].Qty}");
                    BookIndex = i;
                    flag = true;
                    break;
                }
                if (Books[i].BAuthor == name)
                {
                    sb.AppendLine($"{count}. Book Name: {Books[i].BName} | Quantity: {Books[i].Qty}");
                    count++;
                    AuthBooks = true;
                    BookIds.Add( i );
                }

            }
            if (AuthBooks && AdmnOrUsr)
            {
                Console.WriteLine("Choose a book to borrow:");
                Console.WriteLine(sb.ToString());
                int BookChoice;
                while((!int.TryParse(Console.ReadLine(), out BookChoice))||(BookChoice < 1) ||(BookChoice > BookIds.Count))
                {
                    Console.WriteLine("Invalid input, please try again:");
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

            if (!flag && !AuthBooks)
            { 
                Console.WriteLine("Book not found"); 
            }
            else if (!AdmnOrUsr && AuthBooks)
            {
                Console.WriteLine(sb.ToString());
            }
            else
            {
                if (Books[BookIndex].Qty <= 0)
                {
                    Console.WriteLine("Sorry, book is out of stock.");
                }
                else
                {
                    Console.WriteLine("Borrow book? (1)Yes / (2)No:");
                    int BorrowConf;
                    while((!int.TryParse(Console.ReadLine(), out BorrowConf))||(BorrowConf>2) ||(BorrowConf<1))
                    {
                        Console.WriteLine("Invalid input, please try again:");
                    }
                    if (BorrowConf != 1)
                    {
                        Console.WriteLine("Returning to menu...");
                    }
                    else
                    {
                        BorrowBook(BookIndex);
                    }
                }
            }
        }

        static void BorrowBook(int BookIndex = -1)
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
                        Console.WriteLine("Invalid input, please try again: ");
                    }
                    Console.Clear();
                    switch (Choice)
                    {
                        case 1:
                            SearchForBook(true);
                            break;
                        case 2:
                            ViewAllBooks();
                            Console.WriteLine("Enter the number from the list of book to borrow:");
                            int BookChoice;
                            while((!int.TryParse(Console.ReadLine(), out BookChoice))||(BookChoice < 1) ||(BookChoice > Books.Count))
                            {
                                Console.WriteLine("Invalid input, please try again:");
                            }
                            BorrowBook(BookChoice - 1);
                            break;
                        case 0:
                            SaveBooksToFile();
                            Console.WriteLine("Returning to menu...");
                            ExitBorrow = true;
                            break;

                        default:
                            Console.WriteLine("Invalid input, please try again:");
                            break;
                    }
                }while (!ExitBorrow);
            }
            else
            {
                Console.WriteLine("Enter the quantity to borrow:");
                int BorrowQty;
                while ((!int.TryParse(Console.ReadLine(), out BorrowQty)) || (BorrowQty < 1) || (BorrowQty > 5) || (BorrowQty > Books[BookIndex].Qty))
                {
                    Console.WriteLine("Invalid input or exceeds limit, please try again:");
                }
                Books[BookIndex] = (Books[BookIndex].BName, Books[BookIndex].BAuthor, Books[BookIndex].ID, (Books[BookIndex].Qty - BorrowQty));
                Console.WriteLine($"{BorrowQty} x {Books[BookIndex].BName} borrowed successfully!");
                SaveBooksToFile();
            }
        }

        static void ReturnBook()
        {
            int BookChoice;
            ViewAllBooks();
            Console.WriteLine("Enter the number from the list of book to return:");
            while ((!int.TryParse(Console.ReadLine(), out BookChoice)) || (BookChoice < 1) || (BookChoice > Books.Count))
            {
                Console.WriteLine("Invalid input, please try again: ");
            }
            Console.WriteLine("Enter the quantity to return:");
            int ReturnQty;
            while ((!int.TryParse(Console.ReadLine(), out ReturnQty)) || (ReturnQty < 1) || (ReturnQty > 5))
            {
                Console.WriteLine("Invalid input, please try again: ");
            }

            Books[BookChoice - 1] = (Books[BookChoice - 1].BName, Books[BookChoice - 1].BAuthor, Books[BookChoice - 1].ID, (Books[BookChoice - 1].Qty + ReturnQty));
            Console.WriteLine($"{ReturnQty} x {Books[BookChoice - 1].BName} returned successfully!");
            SaveBooksToFile();
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
                    Console.WriteLine("Books loaded from file successfully.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading from file: {ex.Message}");
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
                Console.WriteLine("Books saved to file successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving to file: {ex.Message}");
            }
        }

        static void SaveAdminsToFile()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(adminsPath))
                {
                    foreach (var admin in Admins)
                    {
                        writer.WriteLine($"{admin.AdminEmail}|{admin.AdminPass}");
                    }
                }
                Console.WriteLine("Admins info saved to file successfully.");
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
                            if (parts.Length == 2)
                            {
                                Admins.Add((parts[0], parts[1]));
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

        static void SaveUsersToFile()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(UsersPath))
                {
                    foreach (var user in Users)
                    {
                        writer.WriteLine($"{user.UserID}|{user.UserEmail}|{user.UserPass}");
                    }
                }
                Console.WriteLine("Users info saved to file successfully.");
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
                            if (parts.Length == 3)
                            {
                                Users.Add((int.Parse(parts[0]), parts[1], parts[3]));
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

        static void EditBook()
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
                Console.WriteLine("\nChoose an editing option:\n1. Edit Book Name.\n2. Edit Book Author.\n3. Add quantity.\n4. Remove Book.");
                int EditChoice;
                while ((!int.TryParse(Console.ReadLine(), out EditChoice)) || (EditChoice > 4) || (EditChoice < 1))
                {
                    Console.WriteLine("\nInvalid option, please try again.");
                }

                switch (EditChoice)
                {
                    case 0:
                        ExitFlag = true;
                        break;

                    case 1:
                        Console.WriteLine($"\nEnter the new name for {Books[ChosenBook - 1].BName}: ");
                        string NewName;
                        while(string.IsNullOrEmpty(NewName = Console.ReadLine()))
                        {
                            Console.WriteLine("Invalid input, please try again:");
                        }
                        string OldBName = Books[ChosenBook - 1].BName;
                        Books[ChosenBook - 1] = (NewName, Books[ChosenBook - 1].BAuthor, Books[ChosenBook - 1].ID, Books[ChosenBook - 1].Qty);
                        Console.WriteLine($"Book \"{OldBName}\" name changed to: {NewName}.");
                        break;

                    case 2:
                        Console.WriteLine($"\nEnter the new author name for {Books[ChosenBook - 1].BName}: ");
                        string NewAuth;
                        while (string.IsNullOrEmpty(NewAuth = Console.ReadLine()))
                        {
                            Console.WriteLine("Invalid input, please try again:");
                        }
                        string OldAuth = Books[ChosenBook - 1].BAuthor;
                        Books[ChosenBook - 1] = (Books[ChosenBook - 1].BName, NewAuth, Books[ChosenBook - 1].ID, Books[ChosenBook - 1].Qty);
                        Console.WriteLine($"\"{Books[ChosenBook - 1].BName}\" Author changed from: {OldAuth} to: {NewAuth}.");
                        break;

                    case 3:
                        Console.WriteLine($"\nEnter the additional quantity for {Books[ChosenBook - 1].BName}: ");
                        int NewQty;
                        while ((!int.TryParse(Console.ReadLine(), out NewQty))||(NewQty < 1))
                        {
                            Console.WriteLine("Invalid input, please try again:");
                        }
                        Books[ChosenBook - 1] = (Books[ChosenBook - 1].BName, Books[ChosenBook - 1].BAuthor, Books[ChosenBook - 1].ID, (Books[ChosenBook - 1].Qty + NewQty));
                        Console.WriteLine($"{Books[ChosenBook - 1].BName} Quantity has been increased to {Books[ChosenBook - 1].Qty} successfully.");
                        break;

                    case 4:
                        string RemovedBook = Books[ChosenBook - 1].BName;
                        Books.RemoveAt(ChosenBook - 1);
                        Console.WriteLine($"Book \"{RemovedBook}\" has been removed from the library File.");
                        break;

                    default:
                        Console.WriteLine("Invalid input, please try again.");
                        break;
                }
            } while (!ExitFlag);
        }
    }
}
