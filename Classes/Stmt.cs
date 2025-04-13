using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace esMT940.Classes
{
    [Serializable]
    public class Stmt
    {
        public string StmtSequence { get; set; }
        public string StmtReference { get; set; }
        public string StmtAccount { get; set; }
        public string StmtCurrency { get; set; }
        public double StmtOpeningBalance { get; set; }
        public double StmtClosingBalance { get; set; }
        public ICollection<Transaction>? StmtTransactions { get; set; }

        public Stmt() 
        { 
            StmtTransactions = new List<Transaction>();
        }

        public Stmt(Stmt stmt)
        {
            if (stmt == null)
            {
                throw new ArgumentNullException(nameof(stmt));
            }
            
            StmtSequence = stmt.StmtSequence;
            StmtReference = stmt.StmtReference;
            StmtAccount = stmt.StmtAccount;
            StmtCurrency = stmt.StmtCurrency;
            StmtOpeningBalance = stmt.StmtOpeningBalance;
            StmtClosingBalance = stmt.StmtClosingBalance;
            StmtTransactions = stmt.StmtTransactions;
        }
    }
}
