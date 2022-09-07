using System.Collections.Generic;

namespace ATM;

struct Transaction
{
    public int Amount { get; set; }

    public string Username { get; set; }

    public long TrackingNumber { get; set; }

    public DateTime TransactionDate { get; set; }

    public void Load(string transactionLine)
    {
        var transactionArr = transactionLine.Split(", ");
        Amount = Convert.ToInt32(transactionArr[0]);

        Username = transactionArr[1];

        TrackingNumber = Convert.ToInt64(transactionArr[2]);

        // split date and time by space ( )
        var datetime = transactionArr[3].Split(' ');

        // split date by hyphen (-)
        var date = datetime[0].Split('-');
        // split time by colon (:)
        var time = datetime[1].Split(':');
        TransactionDate = new DateTime(
            year: Convert.ToInt32(date[0]),
            month: Convert.ToInt32(date[1]),
            day: Convert.ToInt32(date[2]),
            hour: Convert.ToInt32(time[0]),
            minute: Convert.ToInt32(time[1]),
            second: Convert.ToInt32(time[2]));
    }

    public string Save()
    {
        return $"{Amount}, {Username}, {TrackingNumber}, {TransactionDate.ToString("yyyy-MM-dd hh:mm:ss")}";
    }

    public override string ToString()
    {
        string amount = "";
        if (Amount > 0)
        {
            amount = $"+{Amount:C}";
        }
        else
        {
            amount = $"-${Math.Abs(Amount):N}";
        }

        return $"Amount: {amount}\r\nUsername: {Username}\r\nTracking number: {TrackingNumber}\r\nDate and time: {TransactionDate.ToString("yyyy-MM-dd hh:mm:ss")}\r\n";
    }
}

class User
{
    private string _userName = "";
    public string Username { get => _userName; set => _userName = value.Replace(' ', '_').Replace('@', '_'); }
    public long BankAccount { get; set; }
    public int Pin { get; set; }
    public int Balance { get; set; }

    public List<Transaction> Transactions = new List<Transaction>();

    public void Save()
    {
        File.WriteAllText($"{Username}.txt", $"{Username} {BankAccount} {Pin} ${Balance}\n\n");

        foreach (var item in Transactions)
        {
            var temp = item.Save();
            File.AppendAllText($"{Username}.txt", $"{temp}\n");
        }
    }

    public void Load()
    {
        var fileContents = File.ReadAllText($"{Username}.txt");
        var contents = fileContents.Split("\n\n")[0].Split(' ');
        Username = contents[0];
        BankAccount = Convert.ToInt64(contents[1]);
        Pin = Convert.ToInt32(contents[2]);
        Balance = Convert.ToInt32(contents[3].Replace('$', ' '));

        var transactionsLines = fileContents.Split("\n\n")[1];
        transactionsLines = transactionsLines.Remove(transactionsLines.Length - 1);

        foreach (var item in transactionsLines.Split('\n'))
        {
            var transaction = new Transaction();
            transaction.Load(item);

            Transactions.Add(transaction);
        }
    }

    public void AddLastTransaction(int amount, string username)
    {
        var newTransaction = new Transaction()
        {
            Amount = amount,
            Username = username,
            TrackingNumber = new Random().NextInt64(),
            TransactionDate = DateTime.Now
        };

        Transactions.Add(newTransaction);
    }

    public void ShowLastTenTransactions()
    {
        Console.Clear();

        var enumerable = Transactions.AsEnumerable().Reverse<Transaction>();
        IEnumerable<Transaction> reverseTransaction;

        if (Transactions.Count > 10)
        {
            reverseTransaction = enumerable.Take(10);
        }
        else
        {
            reverseTransaction = enumerable.Take(Transactions.Count);
        }
        var i = 1;
        foreach (var item in reverseTransaction)
        {
            Console.WriteLine($"{i++}-> {item.ToString()}\n========\n\n");
        }
    }
}
