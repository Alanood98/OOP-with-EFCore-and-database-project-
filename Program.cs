using System;
using System.Linq;
using LibraryProject.Models;
using LibraryProject.Reposi;
using static LibraryProject.Models.User;


namespace LibraryProject
{
    class Program
    {
        static void Main(string[] args)
        {
            AppDB context = new AppDB(); // Assuming App is your DbContext
            AdminReposi adminrep = new AdminReposi(context);
            UserReposi userRepo = new UserReposi(context);
            BookReposi bookRepo = new BookReposi(context);
            CatgReposi catgRepo = new CatgReposi(context);
            BorrowingReposi borrowingRepo = new BorrowingReposi(context);

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Welcome to the Library System!");
                Console.WriteLine("1. Admin Login");
                Console.WriteLine("2. User Login");
                Console.WriteLine("3. Exit");
                Console.Write("Choose an option: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AdminMode(adminrep, bookRepo, catgRepo, userRepo, borrowingRepo);
                        break;
                    case "2":
                        UserMode(userRepo, bookRepo, borrowingRepo, catgRepo);
                        break;
                    case "3":
                        Console.WriteLine("Exiting... Goodbye!");
                        return;
                    default:
                        Console.WriteLine("Invalid option! Press any key to try again.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        // Admin Mode
        static void AdminMode(AdminReposi adminRepo, BookReposi bookRepo, CatgReposi catgRepo, UserReposi userRepo, BorrowingReposi borrowingRepo)
        {
            Console.Clear();
            Console.WriteLine("Admin Login or Register");
            Console.WriteLine("1. Login");
            Console.WriteLine("2. Register");
            Console.Write("Choose an option: ");
            string choice = Console.ReadLine();

            if (choice == "2")
            {
                Console.Write("Enter Your Name: ");
                string name = Console.ReadLine();

                Console.Write("Enter Your Email: ");
                string email = Console.ReadLine();

                Console.WriteLine("Enter Your Password (8 digits only, or type 'exit' to cancel): ");
                int password;
                while (true)
                {
                    string input = Console.ReadLine();

                    if (input.ToLower() == "exit")
                    {
                        Console.WriteLine("Registration canceled. Press any key to return to the main menu.");
                        Console.ReadKey(); //When this method is called, the program stops and waits for the user to press any key on the keyboard
                        return;
                    }

                    if (int.TryParse(input, out password) && password.ToString().Length == 8)
                    {
                        break; // Valid password provided
                    }

                    Console.Write("Invalid password. Enter exactly 8 digits, or type 'exit' to cancel: ");
                }

                string result = adminRepo.RegisterAdmin(name, email, password);
                Console.WriteLine(result);
                Console.WriteLine("Press any key to return to the main menu.");
                Console.ReadKey();
                return;
            }

            Console.Write("Enter Email: ");
            string loginEmail = Console.ReadLine();
            Console.Write("Enter Password: ");
            if (!int.TryParse(Console.ReadLine(), out int loginPassword))
            {
                Console.WriteLine("Invalid password format. Press any key to return to the main menu.");
                Console.ReadKey();
                return;
            }

            var admin = adminRepo.verifyAdmin(loginEmail, loginPassword).FirstOrDefault();
            if (admin == null)
            {
                Console.WriteLine("Invalid credentials. Press any key to return to the main menu.");
                Console.ReadKey();
                return;
            }

            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Welcome, {admin.AName}");
                Console.WriteLine("1. Add Book");
                Console.WriteLine("2. Update Book");
                Console.WriteLine("3. Delete Book");
                Console.WriteLine("4. View Books");
                Console.WriteLine("5. Manage Categories");
                Console.WriteLine("6. View Users");
                Console.WriteLine("7. Logout");
                Console.Write("Choose an option: ");
                string adminChoice = Console.ReadLine();

                switch (adminChoice)
                {
                    case "1":
                        AddBook(bookRepo, catgRepo);
                        break;
                    case "2":
                        UpdateBook(bookRepo, catgRepo);
                        break;
                    case "3":
                        DeleteBook(bookRepo, catgRepo);
                        break;
                    case "4":
                        ViewBooks(bookRepo);
                        break;
                    case "5":
                        ManageCategories(bookRepo, catgRepo);
                        break;
                    case "6":
                        ViewUsers(userRepo, borrowingRepo, bookRepo);
                        break;
                    case "7":
                        if (ConfirmLogout()) return;
                        break;
                    default:
                        Console.WriteLine("Invalid option! Press any key to try again.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        // User Mode
        static void UserMode(UserReposi userRepo, BookReposi bookRepo, BorrowingReposi borrowingRepo, CatgReposi catgRepo)
        {
            try
            {
                Console.Clear();
                Console.WriteLine("User Login or Register");
                Console.WriteLine("1. Login");
                Console.WriteLine("2. Register");
                Console.Write("Choose an option: ");
                string choice = Console.ReadLine();

                if (choice == "2")
                {
                    Console.WriteLine("Registration");

                    // Ask for the user's name
                    Console.Write("Enter Your Name: ");
                    string name = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(name))
                    {
                        Console.WriteLine("Name cannot be empty. Press any key to return.");
                        Console.ReadKey();
                        return;
                    }

                    // Ask for the user's password
                    Console.Write("Enter Your Password (8 digits only, or type 'exit' to cancel): ");
                    int password;
                    while (true)
                    {
                        string input = Console.ReadLine();

                        if (input.ToLower() == "exit")
                        {
                            Console.WriteLine("Registration canceled. Press any key to return to the main menu.");
                            Console.ReadKey();
                            return;
                        }

                        if (int.TryParse(input, out password) && password.ToString().Length == 8)
                        {
                            break;
                        }
                        Console.WriteLine("Invalid password. Enter exactly 8 digits, or type 'exit' to cancel:");
                    }

                    // Ask for the user's gender
                    Console.Write("Enter Your Gender (M = Male/ F = Female): ");
                    string genderInput;
                    Gender gender;
                    while (true)
                    {
                        genderInput = Console.ReadLine();
                        if (Enum.TryParse(genderInput, true, out gender) &&
                            (gender == Gender.M || gender == Gender.F))
                        {
                            break;
                        }
                        Console.WriteLine("Invalid input. Please enter M  for = Male, F for = Female: ");
                    }

                    // Register the user
                    string result = userRepo.RegisterUser(password, name, gender);
                    Console.WriteLine(result);
                    Console.WriteLine("Press any key to return to the main menu.");
                    Console.ReadKey();
                    return;
                }

                // Login functionality remains unchanged
                Console.Write("Enter Passcode: ");
                int loginPassword;
                if (!int.TryParse(Console.ReadLine(), out loginPassword))
                {
                    Console.WriteLine("Invalid passcode. Press any key to return.");
                    Console.ReadKey();
                    return;
                }

                var user = userRepo.verifyUser(loginPassword).FirstOrDefault();
                if (user == null)
                {
                    Console.WriteLine("Invalid passcode. Press any key to return to the main menu.");
                    Console.ReadKey();
                    return;
                }

                while (true)
                {
                    Console.Clear();
                    Console.WriteLine($"Welcome, {user.UName}");
                    Console.WriteLine("1. View Books");
                    Console.WriteLine("2. Borrow Book");
                    Console.WriteLine("3. Return Book");
                    Console.WriteLine("4. Logout");
                    Console.Write("Choose an option: ");
                    string userChoice = Console.ReadLine();

                    switch (userChoice)
                    {
                        case "1":
                            ViewBooks(bookRepo);
                            break;
                        case "2":
                            BorrowBook(bookRepo, borrowingRepo, user);
                            break;
                        case "3":
                            ReturnBook( borrowingRepo,  bookRepo,  user);
                            break;
                        case "4":
                            if (ConfirmLogout()) return;
                            break;
                        default:
                            Console.WriteLine("Invalid option! Press any key to try again.");
                            Console.ReadKey();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                Console.WriteLine("Press any key to return to the main menu.");
                Console.ReadKey();
            }
        }



        static void AddBook(BookReposi bookRepo, CatgReposi catgRepo)
        {
            Console.Clear();
            Console.WriteLine("Add New Book");

            // Display all categories
            Console.WriteLine("Available Categories:");
            var categories = catgRepo.GetAllCategory();
            foreach (var category in categories)
            {
                Console.WriteLine($"ID: {category.CID}, Name: {category.CName}, Number of Books: {category.NumberOfBooks}");
            }

            // Get the Category ID
            int categoryId = GetValidatedInt("Enter the Category ID to which the book belongs (or type 'exit' to cancel): ", allowExit: true);
            if (categoryId == -1) return; // Exit if user chooses to cancel

            // Validate category
            var selectedCategory = categories.FirstOrDefault(c => c.CID == categoryId);
            if (selectedCategory == null)
            {
                Console.WriteLine("Invalid Category ID. Press any key to return to the menu.");
                Console.ReadKey();
                return;
            }

            // Get Book Details
            Console.Write("Enter Book Name: ");
            string name = Console.ReadLine();

            Console.Write("Enter Author: ");
            string author = Console.ReadLine();

            double price = GetValidatedDouble("Enter Price (greater than 0, or type 'exit' to cancel): ", allowExit: true);
            if (price == -1) return;

            int copies = GetValidatedInt("Enter Total Copies (greater than 0, or type 'exit' to cancel): ", allowExit: true);
            if (copies == -1) return;

            int period = GetValidatedInt("Enter Borrow Period (days, greater than 0, or type 'exit' to cancel): ", allowExit: true);
            if (period == -1) return;

            // Add the book to the repository
            bookRepo.InsertBook(new Book
            {
                BName = name,
                BAuthor = author,
                Price = price,
                TotalCopies = copies,
                Period = period,
                CID = categoryId,
                BorrowedCopies = 0
            });

            // Update the category's book count
            bookRepo.updateCategoryCopyWhenAddingBook(categoryId);

            Console.WriteLine($"Book '{name}' added successfully to Category '{selectedCategory.CName}'. Press any key to return to the menu.");
            Console.ReadKey();
        }

        static int GetValidatedInt(string prompt, bool allowExit = false)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine()?.Trim();

                if (allowExit && input?.ToLower() == "exit") return -1;

                if (int.TryParse(input, out int result) && result > 0)
                {
                    return result;
                }
                Console.WriteLine("Invalid input. Please enter a valid number greater than 0.");
            }
        }

        static double GetValidatedDouble(string prompt, bool allowExit = false)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine()?.Trim();

                if (allowExit && input?.ToLower() == "exit") return -1;

                if (double.TryParse(input, out double result) && result > 0)
                {
                    return result;
                }
                Console.WriteLine("Invalid input. Please enter a valid number greater than 0.");
            }
        }



        static void UpdateBook(BookReposi bookRepo, CatgReposi catgRepo)
        {
            Console.Clear();
            Console.WriteLine("Update Book");

            // Display all categories and their books
            Console.WriteLine("\nCategories and their Books:");
            var categories = catgRepo.GetAllCategory();
            foreach (var category in categories)
            {
                Console.WriteLine($"\nCategory ID: {category.CID}, Name: {category.CName}");
                var booksInCategory = bookRepo.GetAllBooks().Where(b => b.CID == category.CID).ToList();
                if (booksInCategory.Any())
                {
                    foreach (var Book in booksInCategory)
                    {
                        Console.WriteLine($"\tBook ID: {Book.BID}, Name: {Book.BName}, Author: {Book.BAuthor}");
                    }
                }
                else
                {
                    Console.WriteLine("\tNo books in this category.");
                }
            }

            Console.Write("\nEnter the Book Name to update (or press Enter to return): ");
            string bookName = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(bookName))
            {
                Console.WriteLine("No book selected. Returning to the menu...");
                Console.ReadKey();
                return;
            }

            var book = bookRepo.GetBookByName(bookName).FirstOrDefault();
            if (book == null)
            {
                Console.WriteLine("Book not found. Press any key to return.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"\nUpdating Book: {book.BName}, Author: {book.BAuthor}");

            Console.Write("Enter new Book Name (leave blank to keep current): ");
            string newName = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newName))
            {
                book.BName = newName;
            }

            Console.Write("Enter New Author (leave blank to keep current): ");
            string newAuthor = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newAuthor))
            {
                book.BAuthor = newAuthor;
            }

            Console.Write("Enter New Price (leave blank to keep current): ");
            string newPriceInput = Console.ReadLine();
            if (double.TryParse(newPriceInput, out double newPrice) && newPrice > 0)
            {
                book.Price = newPrice;
            }

            Console.Write("Enter New Borrow Period (days, leave blank to keep current): ");
            string newPeriodInput = Console.ReadLine();
            if (int.TryParse(newPeriodInput, out int newPeriod) && newPeriod > 0)
            {
                book.Period = newPeriod;
            }

            Console.Write("Enter New Total Copies (leave blank to keep current): ");
            string newCopiesInput = Console.ReadLine();
            if (int.TryParse(newCopiesInput, out int newCopies) && newCopies > 0)
            {
                book.TotalCopies = newCopies;
            }

            // Save changes using the repository
            bookRepo.UpdateBookByName(book.BName);
            Console.WriteLine("\nBook updated successfully. Press any key to return.");
            Console.ReadKey();
        }




        static void DeleteBook(BookReposi bookRepo, CatgReposi catgRepo)
        {
            Console.Clear();
            Console.WriteLine("Delete Book");

            // Display all categories and their books
            Console.WriteLine("Categories and their Books:");
            var categories = catgRepo.GetAllCategory();
            foreach (var category in categories)
            {
                Console.WriteLine($"\nCategory ID: {category.CID}, Name: {category.CName}");
                var booksInCategory = bookRepo.GetAllBooks().Where(b => b.CID == category.CID).ToList();
                if (booksInCategory.Any())
                {
                    foreach (var Book in booksInCategory)
                    {
                        Console.WriteLine($"\tBook ID: {Book.BID}, Name: {Book.BName}, Author: {Book.BAuthor}");
                    }
                }
                else
                {
                    Console.WriteLine("\tNo books in this category.");
                }
            }

            Console.Write("\nEnter the Book ID to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int bookId))
            {
                Console.WriteLine("Invalid input. Please enter a valid Book ID. Press any key to return.");
                Console.ReadKey();
                return;
            }

            var book = bookRepo.GetAllBooks().FirstOrDefault(b => b.BID == bookId);
            if (book == null)
            {
                Console.WriteLine("Book not found. Press any key to return.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"Are you sure you want to delete the book '{book.BName}' by '{book.BAuthor}'? (y/n): ");
            string confirm = Console.ReadLine();
            if (confirm?.ToLower() != "y")
            {
                Console.WriteLine("Operation canceled. Press any key to return.");
                Console.ReadKey();
                return;
            }

            // Delete the book
            bookRepo.DeleteBookById(bookId);

            // Update the category's book count
            bookRepo.updateCategoryCopyWhenDeleting(book.CID);

            Console.WriteLine($"Book '{book.BName}' deleted successfully. Press any key to return.");
            Console.ReadKey();
        }



        static void ViewBooks(BookReposi bookRepo)
        {
            Console.Clear();
            Console.WriteLine("View All Books");

            var books = bookRepo.GetAllBooks();
            foreach (var book in books)
            {
                Console.WriteLine($"ID: {book.BID}, Name: {book.BName}, Author: {book.BAuthor}, Price: {book.Price}, Total Copies: {book.TotalCopies}, Borrowed: {book.BorrowedCopies}");
            }

            Console.WriteLine("Press any key to return.");
            Console.ReadKey();
        }

        static void ManageCategories(BookReposi bookRepo, CatgReposi catgRepo)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Manage Categories");
                Console.WriteLine("1. View All Categories");
                Console.WriteLine("2. Add Category");
                Console.WriteLine("3. Update Category");
                Console.WriteLine("4. Delete Category");
                Console.WriteLine("5. Return to Admin Menu");
                Console.Write("Choose an option: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ViewAllCategories(bookRepo, catgRepo);
                        break;
                    case "2":
                        AddCategory(catgRepo);
                        break;
                    case "3":
                        UpdateCategory(catgRepo);
                        break;
                    case "4":
                        DeleteCategory(catgRepo);
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Invalid option. Press any key to try again.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void ViewUsers(UserReposi userRepo, BorrowingReposi borrowingRepo, BookReposi bookRepo)
        {
            Console.Clear();
            Console.WriteLine("View All Users");

            var users = userRepo.GetAllUsers();
            foreach (var user in users)
            {
                Console.WriteLine($"\nID: {user.UID}, Name: {user.UName}");

                // Fetch the books borrowed by the user
                var borrowings = borrowingRepo.GetAll().Where(b => b.UID == user.UID).ToList();

                if (borrowings.Any())
                {
                    Console.WriteLine("Borrowed Books:");
                    foreach (var borrowing in borrowings)
                    {
                        var book = bookRepo.GetAllBooks().FirstOrDefault(b => b.BID == borrowing.BID);
                        if (book != null)
                        {
                            Console.WriteLine($"\tBook ID: {book.BID}, Name: {book.BName}, Borrow Date: {borrowing.BrDate:d}, Due Date: {borrowing.ReturenDate:d}");

                            if (borrowing.IsReturned)
                            {
                                Console.WriteLine($"\t\tReturned on: {borrowing.ActualDate:d}, Rating: {borrowing.Rating}/5");
                            }
                            else
                            {
                                Console.WriteLine("\t\tNot yet returned.");
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("\tNo borrowing history.");
                }
            }

            Console.WriteLine("\nPress any key to return.");
            Console.ReadKey();
        }


        static void ViewAllCategories(BookReposi bookRepo, CatgReposi catgRepo)
        {
            Console.Clear();
            Console.WriteLine("View All Categories");

            var categories = catgRepo.GetAllCategory();
            foreach (var category in categories)
            {
                // Get all books in the current category
                var booksInCategory = bookRepo.GetAllBooks().Where(b => b.CID == category.CID).ToList();

                // Calculate the number of books in the category
                int numberOfBooks = booksInCategory.Count;

                Console.WriteLine($"ID: {category.CID}, Name: {category.CName}, Number of Books: {numberOfBooks}");

                // Display books in the category
                if (booksInCategory.Any())
                {
                    foreach (var book in booksInCategory)
                    {
                        Console.WriteLine($"\tBook ID: {book.BID}, Name: {book.BName}, Author: {book.BAuthor}, Total Copies: {book.TotalCopies}, Borrowed Copies: {book.BorrowedCopies}");
                    }
                }
                else
                {
                    Console.WriteLine("\tNo books in this category.");
                }
            }

            Console.WriteLine("Press any key to return.");
            Console.ReadKey();
        }


        static void AddCategory(CatgReposi catgRepo)
        {
            Console.Clear();
            Console.WriteLine("Add New Category");

            Console.Write("Enter Category Name (or press Enter to return): ");
            string name = Console.ReadLine()?.Trim(); // Use Trim to remove extra spaces

            // Check if the input is null, empty, or whitespace
            if (string.IsNullOrEmpty(name))
            {
                Console.WriteLine("No category name provided. Returning to the category management menu...");
                Console.WriteLine("Press any key to return.");
                Console.ReadKey();
                return; // Return to the menu without adding a category
            }

            // Add the new category
            catgRepo.InsertCategory(new Category { CName = name, NumberOfBooks = 0 });
            Console.WriteLine($"Category '{name}' added successfully. Press any key to return.");
            Console.ReadKey();
        }


      static void UpdateCategory(CatgReposi catgRepo)
{
    Console.Clear();
    Console.WriteLine("Update Category");

    // Display all categories
    var categories = catgRepo.GetAllCategory();
    if (!categories.Any())
    {
        Console.WriteLine("No categories available to update. Press any key to return.");
        Console.ReadKey();
        return;
    }

    Console.WriteLine("Available Categories:");
    foreach (var category in categories)
    {
        Console.WriteLine($"ID: {category.CID}, Name: {category.CName}");
    }

    Console.Write("\nEnter the Category Name to update (or press Enter to cancel): ");
    string currentName = Console.ReadLine()?.Trim();

    if (string.IsNullOrWhiteSpace(currentName))
    {
        Console.WriteLine("No Category Name entered. Returning to the menu...");
        Console.ReadKey();
        return;
    }

    var selectedCategory = categories.FirstOrDefault(c => c.CName.Equals(currentName, StringComparison.OrdinalIgnoreCase));
    if (selectedCategory == null)
    {
        Console.WriteLine("Category not found. Press any key to return.");
        Console.ReadKey();
        return;
    }

    Console.Write($"Enter new name for the category '{selectedCategory.CName}' (or press Enter to keep the current name): ");
    string newName = Console.ReadLine()?.Trim();

    if (string.IsNullOrEmpty(newName))
    {
        Console.WriteLine("No changes made. Returning to the menu...");
        Console.ReadKey();
        return;
    }

    try
    {
        catgRepo.UpdateCategoryByName( newName); // Pass current and new names
        Console.WriteLine($"Category updated successfully to '{newName}'. Press any key to return.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error updating category: {ex.Message}. Press any key to return.");
    }

    Console.ReadKey();
}




        static void DeleteCategory(CatgReposi catgRepo)
        {
            Console.Clear();
            Console.WriteLine("Delete Category");

            var categories = catgRepo.GetAllCategory();
            if (!categories.Any())
            {
                Console.WriteLine("No categories available to delete. Press any key to return.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Available Categories:");
            foreach (var category in categories)
            {
                Console.WriteLine($"ID: {category.CID}, Name: {category.CName}, Number of Books: {category.NumberOfBooks}");
            }

            Console.Write("\nEnter the Category ID to delete (or press Enter to cancel): ");
            string input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input) || !int.TryParse(input, out int categoryId))
            {
                Console.WriteLine("Invalid input. Press any key to return.");
                Console.ReadKey();
                return;
            }

            var selectedCategory = categories.FirstOrDefault(c => c.CID == categoryId);
            if (selectedCategory == null)
            {
                Console.WriteLine("Category not found. Press any key to return.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"Are you sure you want to delete the category '{selectedCategory.CName}' with ID {categoryId}? (y/n)");
            if (Console.ReadLine()?.ToLower() != "y")
            {
                Console.WriteLine("Category deletion canceled. Press any key to return.");
                Console.ReadKey();
                return;
            }

            catgRepo.DeleteCategoryById(categoryId);
            Console.WriteLine($"Category '{selectedCategory.CName}' deleted successfully. Press any key to return.");
            Console.ReadKey();
        }




        static bool ConfirmLogout()
        {
            Console.Write("Are you sure you want to logout? (y/n): ");
            string choice = Console.ReadLine();
            return choice?.ToLower() == "y"; //The ?. is the null-conditional operator
        }

        static void BorrowBook(BookReposi bookRepo, BorrowingReposi borrowingRepo, User user)
        {
            Console.Clear();
            Console.WriteLine("Borrow Book");

            // Display all available books
            var books = bookRepo.GetAllBooks().Where(b => b.TotalCopies > b.BorrowedCopies).ToList();
            if (!books.Any()) //Returns true if at least one book exists in the list, and false if the list is empty
            {
                Console.WriteLine("No books available for borrowing. Press any key to return to the menu.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Available Books:");
            foreach (var book in books)
            {
                Console.WriteLine($"ID: {book.BID}, Name: {book.BName}, Author: {book.BAuthor}, Available Copies: {book.TotalCopies - book.BorrowedCopies}");
            }

            Console.Write("Enter the Book ID to borrow: ");
            if (!int.TryParse(Console.ReadLine(), out int bookId))
            {
                Console.WriteLine("Invalid input. Please enter a valid Book ID. Press any key to return.");
                Console.ReadKey();
                return;
            }

            var selectedBook = books.FirstOrDefault(b => b.BID == bookId);
            if (selectedBook == null)
            {
                Console.WriteLine("Invalid Book ID or book is unavailable. Press any key to return.");
                Console.ReadKey();
                return;
            }

            // Check if the user has already borrowed this book
            var borrowings = borrowingRepo.GetAll().Where(b => b.UID == user.UID && b.BID == bookId && !b.IsReturned).ToList();
            if (borrowings.Any())
            {
                Console.WriteLine("You have already borrowed this book and need to return it before borrowing again. Press any key to return.");
                Console.ReadKey();
                return;
            }

            // Borrow the book
            borrowingRepo.Insert(new Borrowing
            {
                UID = user.UID,
                BID = bookId,
                BrDate = DateTime.Now,
                ReturenDate = DateTime.Now.AddDays(selectedBook.Period),
                IsReturned = false
            });

            // Update the borrowed copies count
            selectedBook.BorrowedCopies++;
            bookRepo.UpdateBookByName(selectedBook.BName);

            Console.WriteLine($"You have successfully borrowed '{selectedBook.BName}'. Please return it by {DateTime.Now.AddDays(selectedBook.Period):d}. Press any key to return.");
            Console.ReadKey();
        }

        static void ReturnBook(BorrowingReposi borrowingRepo, BookReposi bookRepo, User user)
        {
            Console.Clear();
            Console.WriteLine("Return Book");

            // Fetch borrowed books for the user
            var borrowings = borrowingRepo.GetAll().Where(b => b.UID == user.UID && !b.IsReturned).ToList();
            if (!borrowings.Any())
            {
                Console.WriteLine("You have no borrowed books to return. Press any key to return to the menu.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Borrowed Books:");
            foreach (var borrowing in borrowings)
            {
                var book = bookRepo.GetAllBooks().FirstOrDefault(b => b.BID == borrowing.BID);
                if (book != null)
                {
                    Console.WriteLine($"Book ID: {borrowing.BID}, Name: {book.BName}, Borrow Date: {borrowing.BrDate:d}, Return Due: {borrowing.ReturenDate:d}");
                }
            }

            Console.Write("Enter the Book ID to return: ");
            if (!int.TryParse(Console.ReadLine(), out int bookId))
            {
                Console.WriteLine("Invalid input. Please enter a valid Book ID. Press any key to return.");
                Console.ReadKey();
                return;
            }

            var borrowingToReturn = borrowings.FirstOrDefault(b => b.BID == bookId);
            if (borrowingToReturn == null)
            {
                Console.WriteLine("Invalid Book ID or you have not borrowed this book. Press any key to return.");
                Console.ReadKey();
                return;
            }

            // Mark the book as returned
            borrowingToReturn.IsReturned = true;
            borrowingToReturn.ActualDate = DateTime.Now;
            borrowingRepo.UpdateBorrowingByBookName(bookRepo.GetAllBooks().First(b => b.BID == bookId).BName);

            // Update the book's borrowed copies count
            var bookToUpdate = bookRepo.GetAllBooks().FirstOrDefault(b => b.BID == bookId);
            if (bookToUpdate != null)
            {
                bookToUpdate.BorrowedCopies--;
                bookRepo.UpdateBookByName(bookToUpdate.BName);
            }

            Console.WriteLine($"You have successfully returned '{bookToUpdate.BName}'. Thank you! Press any key to return.");
            Console.ReadKey();
        }


    }
}