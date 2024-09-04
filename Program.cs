using System.Text;

namespace BasicLibrary
{
    internal class Program
    {
        static List<(string BName, string BAuthor, int ID, int Qty)> Books = new List<(string BName, string BAuthor, int ID, int Qty)>();
        static string filePath = "C:\\Users\\Lenovo\\Desktop\\Ibrahim_Projects\\LibrarySystemFiles\\LibraryBooks.txt";

        static void Main(string[] args)
        {
            LoadBooksFromFile();
            int AccessLevel;
            bool StopApp = false;
            do
            {
                Console.Clear();
                Console.WriteLine("Enter (1) for admin access | (2) for user access | (0) to Exit:");
                AccessLevel = int.Parse(Console.ReadLine());

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

                int choice = int.Parse(Console.ReadLine());

                switch (choice)
                {
                    case 1:
                        AddnNewBook();
                        break;

                    case 2:
                        ViewAllBooks();
                        break;

                    case 3:
                        SearchForBook();
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

                int choice = int.Parse(Console.ReadLine());

                switch (choice)
                {
                    case 1:
                        SearchForBook();
                        break;

                    case 2:
                        //BorrowBook();
                        break;

                    case 3:
                        //ReturnBook();
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
            string name = Console.ReadLine();   

            Console.WriteLine($"Enter Author Name of \"{name}\": ");
            string author= Console.ReadLine();  

            Console.WriteLine($"Enter ID of \"{name}\": ");
            int ID = int.Parse(Console.ReadLine());

            Console.WriteLine($"Enter available quantity of \"{name}\": ");
            int Qty = int.Parse(Console.ReadLine());

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
                sb.Append("Book ").Append(BookNumber).Append(" Author : ").Append(Books[i].BAuthor);
                sb.AppendLine();
                sb.Append("Book ").Append(BookNumber).Append(" ID : ").Append(Books[i].ID);
                sb.AppendLine().AppendLine();
                Console.WriteLine(sb.ToString());
                sb.Clear();

            }
        }

        static void SearchForBook()
        {
            Console.WriteLine("Enter the book or author name to search");
            string name = Console.ReadLine();  
            bool flag=false;

            for(int i = 0; i< Books.Count;i++)
            {
                if (Books[i].BName == name)
                {
                    Console.WriteLine($"Book details:" +
                        $"\nName: {Books[i].BName} | Author: {Books[i].BAuthor} | Qty: {Books[i].Qty}");
                    flag = true;
                    break;
                }

            }

            if (flag != true)
            { Console.WriteLine("Book not found"); }
        }

        static void BorrowBook(int BookIndex = -1)
        {
            if (BookIndex == -1)
            {
                Console.WriteLine("Choose a method:" +
                    "\n1. Search by book / author name." +
                    "\n2. Browse available books." +
                    "\n\n0. Exit");

                int Choice = int.Parse(Console.ReadLine());
                switch (Choice)
                {
                    case 1:
                        SearchForBook();
                        break;
                    case 2:

                        break;
                }
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
                        writer.WriteLine($"{book.BName}|{book.BAuthor}|{book.ID}");
                    }
                }
                Console.WriteLine("Books saved to file successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving to file: {ex.Message}");
            }
        }

    }
}
