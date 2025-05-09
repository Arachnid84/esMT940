# About Extreme Solutions MT940 Parser
Developed as basic need for a different project which required an easy and simple to use MT940 parser which should work well with multiple banks in the SEPA zone. The package has been compiled and tested for use within 
Microsoft Visual Studio 2022 as well as tested in a near production ready internal application for CRM, Accounting and Sales pipeline.

[Source code on GitHub](https://github.com/Arachnid84/esMT940) | [NuGet](https://www.nuget.org/packages/esMT940)

## Getting started
### Install the package
Install esMT940 trough NuGet
`dotnet add package esMT940`

## How to use
To parse an MT940 file you can use the following basic example
```
using esMT940;

public function ImportMT940(string mt940File)
{
    var mt940 = Parser.ParseAsync(mt940File);

    foreach (var entry in mt940.Result)
    {
        //do mapping of fields
        foreach (var transaction in entry.StmtTransactions)
        {
            //do mapping of individual transactions contained in a statement
        }
    }
}
```

`Parser.ParseAsync()` supports either a full file input (e.g. file path) or a stream by preceding with `var stream = mt940File.OpenReadStream();`

## Class exposure
The MT940 parser casts the content of a MT940 file into a single object containing one or more transactions as subclass on a statement:

### Statement
The statement class returns the following values:
- String: StmtReference: Tag 20 - Transaction Reference Number. Returns empty if not provided
- String: StmtAccount: Tag 25 - Account number for statement. Returns empty if not provided
- String: StmtSequence: Tag 28(c) - Statement or statement sequence number. Returns empty if not provided
- String: StmtCurrency: Returns the statements primary currency if provided. Returns empty if not provided
- Double: StmtOpeningBalance: Tag 60 - The openening balance of the statement
- Double: StmtClosingBalance: Tag 62 - The closing balance of the statement
- ICollenction Transaction: StmtTransactions: Tag 61 - Collection of transactions contained in the current statement

### Transaction
The transactions contain the following values as collection within the statement:
- DateTime: TrnValueDate: The date on which the value was entered
- DateTime: TrnEntryDate: The date on which the value was entered, this one can be ambigous and will be filled with TrnValueDate if not provided in the transaction
- String: TrnDebitCredit: Returns the type of transaction D for Debit C for Credit. Returns empty if not provided
- String: TrnFundsCode: Indicate the kind of currency on the 3rd character for ISO currency code. Returns empty if not provided. Returns empty if not provided
- Double: TrnAmount: The actual amount of money involved with this transaction
- String: TrnTransactionType: Transaction Type Identification Code. Returns empty if not provided
- String: TrnCustReference: Customer reference. Returns empty if not provided
- String: TrnBankReference: Bank reference. Returns empty if not provided
- String: TrnSuppDetails: Supplementary details on transaction if provided. Returns empty if not provided
- String: TrnDescription: Description of the transaction as printed on the actual statement. Returns empty if not provided