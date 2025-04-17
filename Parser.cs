using esMT940.Classes;
using System.Security.Cryptography;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using System.Text.RegularExpressions;
using System.Reflection;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Data;
using System.Globalization;

namespace esMT940
{
    public class Parser
    {
        public static async Task<ICollection<Stmt>> ParseAsync(string fileName)
        {
            string data;
            StreamReader streamReader;
            using (streamReader = new StreamReader(fileName))
            {
                data = streamReader.ReadToEnd();
            }
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(data));

            return await ParseAsync(stream);
        }

        private static readonly string[] separator = new[] { "\r\n", "\r", "\n" };

        public static async Task<ICollection<Stmt>> ParseAsync(Stream stream)
        {
            using StreamReader sr = new StreamReader(stream);
            
            List<Stmt> stmt = new List<Stmt>();

            string? lines = await sr.ReadToEndAsync();
            lines = lines.TrimEnd('\r', '\n');
            string[] trnLines = lines.Split(separator, StringSplitOptions.None);

            var stmtLine = default(Stmt);
            var transactionLine = default(Transaction);

            for (int lPoint = 0; lPoint < trnLines.Count(); lPoint++)
            {
                
                bool whiteSpace = string.IsNullOrWhiteSpace(trnLines[lPoint]);
                if ((!whiteSpace) && (trnLines[lPoint][0] == ':'))
                {
                    var tag = trnLines[lPoint].Substring(trnLines[lPoint].IndexOf(':'), trnLines[lPoint].IndexOf(':', 1) + 1);
                    var transactionData = trnLines[lPoint].Substring(tag.Length);

                    switch (tag)
                    {
                        case ":20:":
                            stmtLine = new Stmt
                            {
                                StmtReference = transactionData
                            };
                            break;
                        case ":25:":
                            if (stmtLine != null)
                            {
                                stmtLine.StmtAccount = transactionData;
                            }
                            break;
                        case ":28:":
                        case ":28C:":
                            if (stmtLine != null)
                            {
                                stmtLine.StmtSequence = transactionData;
                            }
                            break;
                        case ":60m:":
                        case ":60M:":
                        case ":60F:":
                            var regexOB = new Regex(@"([C|D]{1})([0-9]{2})([0-9]{2})([0-9]{2})([A-Z]{3})(\d.*)");
                            var matchOB = regexOB.Match(transactionData);

                            double balanceOB = 0;
                            if (matchOB.Groups[1].Value == "D")
                            {
                                balanceOB = balanceOB - Convert.ToDouble(matchOB.Groups[6].Value);
                            }
                            else
                            {
                                balanceOB = balanceOB + Convert.ToDouble(matchOB.Groups[6].Value);
                            }
                            if (stmtLine != null)
                            {
                                stmtLine.StmtOpeningBalance = balanceOB;
                                stmtLine.StmtCurrency = matchOB.Groups[5].Value;
                            }
                            break;
                        case ":61:":
                            transactionLine = new Transaction();

                            var trnRegex = new Regex(@"^(?<valuedate>(?<year>\d{2})(?<month>\d{2})(?<day>\d{2}))(?<entrydate>(?<entrymonth>\d{2})(?<entryday>\d{2}))?(?<creditdebit>C|D|RC|RD)(?<fundscode>[A-z]{0,1}?)(?<ammount>\d*[,.]\d{0,2})(?<transactiontype>[\w\s]{4})(?<reference>[\s\w]{0,16})(?:(?<servicingreference>//[\s\w]{0,16}))*(?<supplementary>\r\n[\s\w]{0,34})*");


                            var trnMatch = trnRegex.Match(transactionData);

                            if (!trnMatch.Success)
                            {
                                throw new InvalidExpressionException(transactionData);
                            }

                            transactionLine.TrnValueDate = Utility.ParseDate(trnMatch.Groups["year"].Value, trnMatch.Groups["month"].Value, trnMatch.Groups["day"].Value, Utility.CInfo);
                            if (trnMatch.Groups["entrydate"].Length > 0)
                            {
                                transactionLine.TrnEntryDate = Utility.ParseDate(trnMatch.Groups["year"].Value, trnMatch.Groups["entrymonth"].Value, trnMatch.Groups["entryday"].Value, Utility.CInfo);
                            }
                            else
                            {
                                transactionLine.TrnEntryDate = transactionLine.TrnValueDate;
                            }
                            transactionLine.TrnDebitCredit = trnMatch.Groups["creditdebit"].Value;
                            transactionLine.TrnFundsCode = trnMatch.Groups["fundscode"].Value;
                            transactionLine.TrnAmmount = Convert.ToDouble(trnMatch.Groups["ammount"].Value);
                            transactionLine.TrnTransactionType = trnMatch.Groups["transactiontype"].Value;
                            transactionLine.TrnCustReference = trnMatch.Groups["reference"].Value; ;
                            transactionLine.TrnBankReference = trnMatch.Groups["servicingreference"].Value;
                            transactionLine.TrnSuppDetails = trnMatch.Groups["supplementary"].Value; ;

                            break;
                        case ":86:":
                            if ((stmtLine != null) && (transactionLine != null))
                            {
                                transactionLine.TrnDescription = transactionData;
                                for (int nextLine = lPoint + 1; (string.IsNullOrWhiteSpace(trnLines[nextLine]) == false) && (trnLines[nextLine][0] != ':'); nextLine++)
                                { 
                                    transactionLine.TrnDescription = transactionLine.TrnDescription + trnLines[nextLine];
                                }
                            }
                            break;
                        case ":62m:":
                        case ":62M:":
                        case ":62F:":
                            if (((stmt != null) && (stmtLine != null)) && (transactionLine != null))
                            {
                                stmtLine.StmtTransactions.Add(transactionLine);
                                transactionLine = null;
                            }

                            var regex = new Regex(@"([C|D]{1})([0-9]{2})([0-9]{2})([0-9]{2})([A-Z]{3})(\d.*)");
                            var match = regex.Match(transactionData);

                            double balance = 0;
                            if (match.Groups[1].Value == "D")
                            {
                                balance = balance - Convert.ToDouble(match.Groups[6].Value);
                            }
                            else
                            {
                                balance = balance + Convert.ToDouble(match.Groups[6].Value);
                            }
                            if (stmtLine != null)
                            {
                                stmtLine.StmtClosingBalance = balance;
                            }

                            if ((stmt !=null && stmtLine != null) && (transactionLine == null))
                            {
                                stmt.Add(stmtLine);
                                stmtLine = null;
                            }

                            break;
                    }
                }  
            }
            if (stmt != null)
            {
                return stmt;
            }
            else
            { return new List<Stmt>(); }
        }
    }
}
