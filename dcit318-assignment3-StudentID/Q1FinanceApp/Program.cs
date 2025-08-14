
using System;
using System.Collections.Generic;

namespace Q1FinanceApp
{
    // a) Record type
    public readonly record struct Transaction(int Id, DateTime Date, decimal Amount, string Category);

    // b) Interface
    public interface ITransactionProcessor
    {
        void Process(Transaction transaction);
    }

    // c) Concrete processors
    public sealed class BankTransferProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[BankTransfer] Processing {transaction.Amount:C} for {transaction.Category} on {transaction.Date:d}.");
        }
    }

    public sealed class MobileMoneyProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[MobileMoney] Processing {transaction.Amount:C} for {transaction.Category} on {transaction.Date:d}.");
        }
    }

    public sealed class CryptoWalletProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[CryptoWallet] Sending {transaction.Amount:C} tagged '{transaction.Category}' on {transaction.Date:d}.");
        }
    }

    // d) Base Account
    public class Account
    {
        public string AccountNumber { get; }
        public decimal Balance { get; protected set; }

        public Account(string accountNumber, decimal initialBalance)
        {
            AccountNumber = accountNumber ?? throw new ArgumentNullException(nameof(accountNumber));
            Balance = initialBalance;
        }

        public virtual void ApplyTransaction(Transaction transaction)
        {
            Balance -= transaction.Amount;
            Console.WriteLine($"[Account] {transaction.Amount:C} deducted. New balance: {Balance:C}");
        }
    }

    // e) Sealed SavingsAccount
    public sealed class SavingsAccount : Account
    {
        public SavingsAccount(string accountNumber, decimal initialBalance) : base(accountNumber, initialBalance) {}

        public override void ApplyTransaction(Transaction transaction)
        {
            if (transaction.Amount > Balance)
            {
                Console.WriteLine("Insufficient funds");
                return;
            }
            Balance -= transaction.Amount;
            Console.WriteLine($"[SavingsAccount] Deducted {transaction.Amount:C}. Updated balance: {Balance:C}");
        }
    }

    // f) FinanceApp
    public class FinanceApp
    {
        private readonly List<Transaction> _transactions = new();

        public void Run()
        {
            // i. Instantiate SavingsAccount
            var account = new SavingsAccount("SA-001", 1000m);

            // ii. Create Transactions
            var t1 = new Transaction(1, DateTime.Today, 120.50m, "Groceries");
            var t2 = new Transaction(2, DateTime.Today, 300m, "Utilities");
            var t3 = new Transaction(3, DateTime.Today, 200m, "Entertainment");

            // iii. Processors
            ITransactionProcessor p1 = new MobileMoneyProcessor();
            ITransactionProcessor p2 = new BankTransferProcessor();
            ITransactionProcessor p3 = new CryptoWalletProcessor();

            p1.Process(t1);
            p2.Process(t2);
            p3.Process(t3);

            // iv. Apply to account
            account.ApplyTransaction(t1);
            account.ApplyTransaction(t2);
            account.ApplyTransaction(t3);

            // v. Add to _transactions
            _transactions.AddRange(new[] { t1, t2, t3 });

            Console.WriteLine("\nAll transactions recorded:");
            foreach (var t in _transactions)
            {
                Console.WriteLine($"  #{t.Id}: {t.Category} - {t.Amount:C} on {t.Date:d}");
            }
        }
    }

    public static class Program
    {
        public static void Main()
        {
            var app = new FinanceApp();
            app.Run();
        }
    }
}
