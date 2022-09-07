// ATM project
namespace ATM;


// MARK: class program
internal class Program
{
    // MARK: constants
    private static readonly string _adminUsername = "admin";
    private static readonly int _adminPassword = 1234;

    // MARK: variables
    private static bool _hasSaidWelcome = false;

    private static bool isAdmin = false;

    private static User? _currentUser = null;

    private static void Main(string[] args)
    {
        //LoadUsers();
        bool isRunning = true;


        ShowMainMenu();

        while (isRunning)
        {
            var choice = (MainMenuChoice)Convert.ToInt32(Console.ReadLine());

            if (choice == MainMenuChoice.AdminMenu)
            {
                Console.Clear();

                // Enter the admin menu
                Console.WriteLine("Please enter your username:");
                var tempUsername = Console.ReadLine();

                Console.WriteLine("Please enter your password:");
                int tempPassword = 0;

                try
                {
                    tempPassword = ToInt32NoException(Console.ReadLine());
                }
                catch (FormatException)
                {
                }
                if (string.Compare(tempUsername, _adminUsername) == 0 && tempPassword == _adminPassword)
                {
                    Console.Clear();
                    ShowAdminScreen();
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Username or PIN was incorrect!");
                    ShowMainMenu();
                }
            }
            else if (choice == MainMenuChoice.UserMenu)
            {
                Console.Clear();

                // Enter the user menu

                Console.WriteLine("Please enter the username:");
                string? username = Console.ReadLine();

                Console.WriteLine("Please enter your PIN: ");
                int pin = 0;
                try
                {
                    pin = ToInt32NoException(Console.ReadLine());
                }
                catch (FormatException) { }

                if (username is not null && !string.IsNullOrEmpty(username))
                {
                    username = username.Replace(' ', '_').Replace('@', '_');

                    if (File.Exists($"{username}.txt"))
                    {
                        User tempUser = new User();
                        tempUser.Username = username;
                        tempUser.Load();

                        if (pin == tempUser.Pin)
                        {
                            _currentUser = tempUser;
                            Console.Clear();
                            ShowUserOwnMenu();
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine("Username or PIN was incorrect!");
                            ShowMainMenu();
                        }
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("Username or PIN was incorrect!");
                        ShowMainMenu();
                    }
                }
            }
            else if (choice == MainMenuChoice.Exit)
            {
                if (AskForYesNo("Are you sure you wanna exit?"))
                {
                    // Stop the loop
                    isRunning = false;
                }
                else
                {
                    Console.Clear();
                    ShowMainMenu();
                }
            }
            else
            {
                Console.Clear();
                ShowMainMenu();
                Console.WriteLine();
                Console.WriteLine("Invalid choice!");
            }
        }
    }

    private static void ShowUserOwnMenu()
    {
        // When we enter the admin screen for the first time, say a welcome message.
        // We do this here because `_hasSaidWelcome` has been set to true in the main menu before.
        _hasSaidWelcome = false;

        while (_currentUser is not null)
        {
            Console.WriteLine("=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");

            if (!_hasSaidWelcome)
            {
                Console.WriteLine($"Welcome {_currentUser.Username}!");
                _hasSaidWelcome = true;
            }

            Console.Write("[1] Show balance\t");
            Console.WriteLine("[2] Wire transfer");
            Console.WriteLine("[3] Withdraw money");
            Console.WriteLine("[4] Change PIN");
            Console.Write("[5] Show 10 last transactions\t");
            Console.WriteLine("[6] Back to main menu");
            Console.WriteLine("=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");

            var choice = (UserChoice)Convert.ToInt32(Console.ReadLine());

            if (choice == UserChoice.AccountBalance)
            {
                Console.Clear();

                if (AskForYesNo("Showing your balance will reduce $1, are you sure?"))
                {
                    Console.Clear();
                    _currentUser.Balance -= 1;
                    _currentUser.AddLastTransaction(-1, "admin");
                    _currentUser.Save();
                    Console.WriteLine($"Your balance is {ShowMoney(_currentUser.Balance)}");
                }
            }
            else if (choice == UserChoice.CashTransfer)
            {
                Console.Clear();


                Console.WriteLine("Please enter the recipient's username: ");
                string? recipient = Console.ReadLine();

                if (recipient is not null && !string.IsNullOrEmpty(recipient))
                {
                    recipient = recipient.Replace(' ', '_').Replace('@', '_');

                    if (File.Exists($"{recipient}.txt"))
                    {
                        Console.WriteLine("Please enter the amount you wanna transfer: ");
                        var amount = ToInt32NoException(Console.ReadLine());

                        var recipientUser = new User()
                        {
                            Username = recipient
                        };

                        recipientUser.Load();

                        var answer = AskForYesNo($"You're about to send {ShowMoney(amount)} to {recipient} with bank account of {recipientUser.BankAccount}, are you sure?");
                        if (answer)
                        {
                            if (amount <= _currentUser.Balance)
                            {
                                _currentUser.Balance -= amount;
                                recipientUser.Balance += amount;

                                _currentUser.AddLastTransaction(-amount, recipientUser.Username);
                                recipientUser.AddLastTransaction(amount, _currentUser.Username);

                                _currentUser.Save();
                                recipientUser.Save();

                                Console.WriteLine($"Successfully transferred {ShowMoney(amount)}!");
                            }
                            else
                            {
                                Console.WriteLine("Insufficient balance!");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Recipient not found!!");
                    }

                }
            }
            else if (choice == UserChoice.CashWithdrawal)
            {
                Console.Clear();

                Console.WriteLine("Please enter how much do you want to withdraw: ");
                var withdraw = ToInt32NoException(Console.ReadLine());

                if (withdraw <= _currentUser.Balance)
                {
                    _currentUser.Balance -= Math.Abs(withdraw);
                    _currentUser.AddLastTransaction(-Math.Abs(withdraw), _currentUser.Username);
                    Console.WriteLine($"You withdrew {ShowMoney(withdraw)}");
                    _currentUser.Save();
                }
                else
                {
                    Console.WriteLine("Insufficent balance");
                }
            }
            else if (choice == UserChoice.ChangePIN)
            {
                Console.Clear();

                Console.WriteLine("Please enter your old pin:");
                var oldPin = ToInt32NoException(Console.ReadLine());

                if (_currentUser.Pin == oldPin)
                {
                    Console.WriteLine("Please enter your new pin:");
                    var newPin = ToInt32NoException(Console.ReadLine());

                    Console.WriteLine("Please repear your new pin:");
                    var newPinRepeat = ToInt32NoException(Console.ReadLine());

                    if (newPin == newPinRepeat)
                    {
                        _currentUser.Pin = newPin;
                        _currentUser.Save();
                        Console.WriteLine("Your pin was changed!");
                    }
                    else
                    {
                        Console.WriteLine("The pins don't match!");
                    }
                }
                else
                {
                    Console.WriteLine("Your pin was incorect!");
                }
            }
            else if (choice == UserChoice.LastTenTransactions)
            {
                Console.Clear();
                _currentUser.ShowLastTenTransactions();

            }
            else if (choice == UserChoice.BackToMainMenu)
            {
                _currentUser = null;
                Console.Clear();
                ShowMainMenu();
            }
        }
    }

    private static bool AskForYesNo(string message)
    {
        Console.WriteLine($"{message} (y/n)");
        var c = Console.ReadLine()?[0];
        if (c == 'y' || c == 'Y')
        {
            return true;
        }
        return false;
    }

    private static void ShowAdminScreen()
    {
        // When we enter the admin screen for the first time, say a welcome message.
        // We do this here because `_hasSaidWelcome` has been set to true in the main menu before.
        _hasSaidWelcome = false;
        isAdmin = true;

        // Main loop for admin page
        while (isAdmin)
        {
            Console.WriteLine("=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");

            if (!_hasSaidWelcome)
            {
                Console.WriteLine("Welcome admin!");
                _hasSaidWelcome = true;
            }

            Console.Write("[1] Open an account\t");
            Console.WriteLine("[2] Edit user");
            Console.WriteLine("[3] Delete user");
            Console.WriteLine("[4] Show all users");
            Console.Write("[5] Search in users\t");
            Console.WriteLine("[6] Back to main menu");
            Console.WriteLine("=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");

            var choice = (AdminChoice)Convert.ToInt32(Console.ReadLine());

            if (choice == AdminChoice.OpenAccount)
            {
                Console.Clear();
                Console.WriteLine("you are opening an account!");

                Console.WriteLine("Please enter the username:");
                string? username = Console.ReadLine();

                Console.WriteLine("Please enter the bank account number:");
                long bankAccount = ToInt64NoException(Console.ReadLine());

                Console.WriteLine("Please enter the password:");
                int pin = ToInt32NoException(Console.ReadLine());

                Console.WriteLine("Please enter the initial balance:");
                int initialBalance = ToInt32NoException(Console.ReadLine());

                var user = new User();

                if (username is not null)
                {
                    user.Username = username;
                }

                user.BankAccount = bankAccount;
                user.Pin = pin;
                user.Balance = initialBalance;

                user.AddLastTransaction(initialBalance, user.Username);
                user.Save();

                File.AppendAllText("master.txt", $"{user.Username}\n");

                Console.Clear();
                Console.WriteLine($"User {username} created successfully!");
            }
            else if (choice == AdminChoice.EditUser)
            {
                Console.Clear();
                Console.WriteLine("Please enter the username you wanna edit:");
                string? editUser = Console.ReadLine();

                if (editUser is not null && !string.IsNullOrEmpty(editUser))
                {
                    editUser = editUser.Replace(' ', '_').Replace('@', '_');

                    if (File.Exists($"{editUser}.txt"))
                    {
                        User tempUser = new User();
                        tempUser.Username = editUser;
                        tempUser.Load();
                        _currentUser = tempUser;
                        Console.Clear();
                        ShowAdminEditScreen();
                    }
                }
            }
            else if (choice == AdminChoice.DeleteUser)
            {
                Console.Clear();
                Console.WriteLine("Please enter the username you wanna delete:");
                string? name = Console.ReadLine();

                if (name is not null)
                {
                    name = name.Replace(' ', '_').Replace('@', '_');

                    if (File.Exists($"{name}.txt"))
                    {
                        File.Delete($"{name}.txt");

                        // Delete from master
                        var master = File.ReadAllText("master.txt");
                        if (master.Contains(name))
                        {
                            int index = master.IndexOf(name);
                            master = master.Remove(index, name.Length + 1);
                            File.WriteAllText("master.txt", master);
                        }
                        else
                        {
                            Console.WriteLine("User record not found in master");
                        }
                        Console.WriteLine("User deleted successfully");
                    }
                    else
                    {
                        Console.WriteLine("User file not found");
                    }
                }
                else
                {
                    Console.WriteLine("String entered was empty");
                }
            }
            else if (choice == AdminChoice.ShowAllUsers)
            {
                Console.Clear();
                if (File.Exists("master.txt"))
                {
                    var contents = File.ReadAllText("master.txt");
                    if (!string.IsNullOrEmpty(contents))
                    {
                        contents = contents.Remove(contents.Length - 1);
                        foreach (var item in contents.Split('\n'))
                        {
                            var tempuser = new User();
                            tempuser.Username = item;
                            tempuser.Load();

                            Console.WriteLine($"- {tempuser.Username} {tempuser.BankAccount} - {ShowMoney(tempuser.Balance)}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("master is empty");
                    }
                }
                else
                {
                    Console.WriteLine("Master is deleted.");
                }
            }
            else if (choice == AdminChoice.SearchUser)
            {
                Console.Clear();
                Console.WriteLine("Search user: ");
                string? search = Console.ReadLine();

                if (File.Exists("master.txt"))
                {
                    var master = File.ReadAllText("master.txt");
                    if (!string.IsNullOrEmpty(master))
                    {
                        // removes the extra \n at the end
                        master = master.Remove(master.Length - 1);

                        foreach (var item in master.Split('\n'))
                        {
                            if (search is not null)
                            {
                                if (item.Contains(search))
                                {
                                    var tempuser = new User();
                                    tempuser.Username = item;
                                    tempuser.Load();

                                    Console.WriteLine($"- {tempuser.Username} {tempuser.BankAccount} - {ShowMoney(tempuser.Balance)}");
                                }
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("master is empty");
                    }
                }
                else
                {
                    Console.WriteLine("Master is deleted.");
                }
            }
            else if (choice == AdminChoice.BackToMainMenu)
            {
                Console.Clear();
                ShowMainMenu();
                isAdmin = false;
            }
        }
    }

    private static void ShowAdminEditScreen()
    {
        while (isAdmin && _currentUser is not null)
        {
            Console.WriteLine("=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");
            Console.WriteLine($"You're about to edit this user {_currentUser.Username}!");
            Console.WriteLine($"His bank account number is {_currentUser.BankAccount} with a balance of {ShowMoney(_currentUser.Balance)}");
            Console.Write("[1] Change username\t");
            Console.WriteLine("[2] Change passowrd");
            Console.Write("[3] Increase balance\t");
            Console.WriteLine("[4] Back to main menu");
            Console.WriteLine("=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");

            var choice = (EditUserChoice)Convert.ToInt32(Console.ReadLine());

            if (choice == EditUserChoice.ChangeUsername)
            {
                Console.Clear();

                Console.WriteLine("Please enter the new username: ");
                string? newUsername = Console.ReadLine();

                var master = File.ReadAllText("master.txt");

                master = master.Replace(_currentUser.Username, newUsername);
                File.WriteAllText("master.txt", master);

                Console.Clear();
                Console.WriteLine($"Username of {_currentUser.Username} was changed to {newUsername}");

                File.Delete($"{_currentUser.Username}.txt");

                if (newUsername is not null)
                {
                    _currentUser.Username = newUsername;
                }

                _currentUser.Save();
            }
            else if (choice == EditUserChoice.ChangePin)
            {
                Console.Clear();
                Console.WriteLine("Please enter the new PIN: ");
                var newPin = ToInt32NoException(Console.ReadLine());

                _currentUser.Pin = newPin;
                _currentUser.Save();

                Console.Clear();
                Console.WriteLine("PIN was changed.");
            }
            else if (choice == EditUserChoice.IncreaseBalance)
            {
                Console.Clear();
                Console.WriteLine("How much do you want to increase: ");
                var balance = ToInt32NoException(Console.ReadLine());

                Console.Clear();
                Console.WriteLine($"Balance was increased from {ShowMoney(_currentUser.Balance)} to {ShowMoney(_currentUser.Balance + balance)}.");

                if (balance >= _currentUser.Balance)
                {
                    _currentUser.Balance += balance;
                    _currentUser.AddLastTransaction(balance, "admin");
                }
                else
                {
                    _currentUser.Balance -= Math.Abs(balance);
                    _currentUser.AddLastTransaction(-Math.Abs(balance), "admin");
                }

                _currentUser.Save();
            }
            else if (choice == EditUserChoice.BackToMainMenu)
            {
                // clear the console, save the changes, and set the current user to null
                // so that it exits this new menu
                Console.Clear();
                _currentUser.Save();
                _currentUser = null;
            }
        }
    }

    private static void ShowMainMenu()
    {
        Console.WriteLine("=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");

        if (!_hasSaidWelcome)
        {
            Console.WriteLine("Welcome!");
            _hasSaidWelcome = true;
        }

        Console.WriteLine("[1] Login as admin");
        Console.WriteLine("[2] Login as user");
        Console.WriteLine("[3] Exit");
        Console.WriteLine("=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");
    }

    private static string ShowMoney(int amount)
    {
        string amountStr = "";
        if (amount > 0)
        {
            amountStr = $"+{amount:C}";
        }
        else
        {
            amountStr = $"-${Math.Abs(amount):N}";
        }
        return amountStr;
    }

    private static int ToInt32NoException(string? str)
    {
        int a = 0;

        try
        {
            a = Convert.ToInt32(str);
        }
        catch (FormatException)
        {
            // Mohem nist
        }

        return a;
    }
    private static long ToInt64NoException(string? str)
    {
        long a = 0;

        try
        {
            a = Convert.ToInt64(str);
        }
        catch (FormatException)
        {
            // Mohem nist
        }

        return a;
    }
}
