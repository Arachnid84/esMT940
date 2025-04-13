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
        public DateTime trnValueDate {  get; set; }
        public DateTime? trnEntryDate { get; set; }
        public string trnDebitCredit { get; set; }
        public string trnFundsCode { get; set; }
        public double trnAmmount { get; set; }
        public string trnIDCode { get; set; }
        public string trnCustReference { get; set; }
        public string trnBankReference { get; set; }
        public string trnSuppDetails { get; set; }
        public string trnDescription { get; set; }
    }
}
