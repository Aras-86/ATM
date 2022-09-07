namespace ATM;

// MARK: enums
enum Menu
{
    MainMenu,
    AdminLogin,
    UserMainMenu,
}

enum MainMenuChoice
{
    AdminMenu = 1,
    UserMenu,
    Exit,
}

enum AdminChoice
{
    OpenAccount = 1,
    EditUser,
    DeleteUser,
    ShowAllUsers,
    SearchUser,
    BackToMainMenu,
}

enum EditUserChoice
{
    ChangeUsername = 1,
    ChangePin,
    IncreaseBalance,
    BackToMainMenu,
}

enum UserChoice
{
    AccountBalance = 1,
    CashTransfer,
    CashWithdrawal,
    ChangePIN,
    LastTenTransactions,
    BackToMainMenu,
}
