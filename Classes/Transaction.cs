using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace esMT940.Classes
{
    [Serializable]
    public partial class Transaction
    {
        public DateTime TrnValueDate {  get; set; }
        public DateTime? TrnEntryDate { get; set; }
        public string TrnDebitCredit { get; set; } = string.Empty;
        public string TrnFundsCode { get; set; } = string.Empty;
        public double TrnAmmount { get; set; }
        public string TrnTransactionType { get; set; } = string.Empty;
        public string TrnCustReference { get; set; } = string.Empty;
        public string TrnBankReference { get; set; } = string.Empty;
        public string TrnSuppDetails { get; set; } = string.Empty;
        public string TrnDescription { get; set; } = string.Empty;
    }
}
