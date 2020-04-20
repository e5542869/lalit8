using System.Web.Services;
using System.Data;
using System.Data.SqlClient;
using AccountsDB;
using Microsoft.ApplicationBlocks.Data;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Script.Services;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using Newtonsoft.Json;

/// <summary>
/// Summary description for AccountsAppAPI
/// </summary>
[WebService(Namespace = "http://kccsasr.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[System.Web.Script.Services.ScriptService]
public class AccountsAppAPI : DataBaseConnection
{
    EntitiesModel db = new EntitiesModel();
    DataTable dt = new DataTable();
    DataSet ds = new DataSet();
    SqlParameter[] param;

    public AccountsAppAPI()
    {


        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }


    [WebMethod]
    public DataSet MenuBind(string SKey)
    {
        try
        {
            //param = new SqlParameter[1];
            //param[0] = new SqlParameter("@MI_ParentMenuItemId", MI_ParentMenuItemId);
            ds = SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.MenuBind", param);
            SKey = "";
        }
        catch (Exception ex)
        {
            SKey = ex.ToString();
        }
        return ds;
    }

    [WebMethod]
    public DataSet LoadUser(string SKey, int UserAccountID)
    {
        try
        {

            param = new SqlParameter[1];
            param[0] = new SqlParameter("@UserAccountID", UserAccountID);
            ds = SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "LoginGetDefaultValuesForCurrentUser", param);
            SKey = "";

        }
        catch (Exception ex)
        {

            SKey = ex.ToString();
        }
        return ds;
    }

    [WebMethod]
    public DataSet GetLoginFinancialID(string SKey, int UserAccountID)
    {
        try
        {

            param = new SqlParameter[1];
            param[0] = new SqlParameter("@UserAccountID", UserAccountID);
            ds = SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.GetLoginFinancialID", param);
            SKey = "";
            return ds;
        }
        catch (Exception ex)
        {

            SKey = ex.ToString();
        }
        return ds;

    }

    [WebMethod]
    public DataSet LoginCollege(string UserName, string Password, string Error, DateTime Extra)
    {
        var qury = (from u in db.UserAccounts
                    where u.UA_AccountName == UserName && u.UA_MyAppPassword == Encrypt(Password)
                    select new { u.UA_AccountName, u.UA_AccountPassword, u.UserAccountId, u.UA_IsActive, u.UA_UserType }).SingleOrDefault();





        if (qury != null)
        {
            //if (qury.UA_AccountPassword != Password)
            //{
            //    TelerikUtilz.MBoxInfo("Please check your password.", "");
            //    return;
            //}
            if (qury.UA_IsActive == true)
            {
                int UserAccountId = Convert.ToInt32(qury.UserAccountId);

                ds = LoadUser(Error, UserAccountId);
            }
            else
            {
                ds = new DataSet();
            }
        }
        return ds;

    }

    /// <summary>
    /// Function to get VoucherDetails 
    /// </summary>
    /// <returns></returns>
    [WebMethod]
    public DataSet GetVoucherDetails(string SKey, string userName)
    {
        try
        {
            int InstId = 0;
            param = new SqlParameter[1];
            param[0] = new SqlParameter("@InstId", InstId);
            if(userName.Equals("300010"))
            ds = SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "DisplayVoucherDetails", param);
            SKey = "";
        }
        catch (Exception ex)
        {
            SKey = ex.ToString();
        }
        return ds;
    }

    [WebMethod]
    public int PersonalLogin(string UserName, string Password, string Error)
    {
        var qury = (from u in db.UserAccounts
                    where u.UA_AccountName == UserName && u.UA_AccountPassword == Encrypt(Password)
                    && u.UA_UserType != "I"
                    select new { u.UA_AccountName, u.UA_AccountPassword, u.UserAccountId, u.UA_IsActive, u.UA_UserType }).SingleOrDefault();
        if (qury != null)
        {
            return qury.UserAccountId;
        }
        return 0;
    }

    private static byte[] key = { };
    private static byte[] IV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
    private static string EncryptionKey = "!5623a#de";
    private static string EncryptionKeyCode = "gurpreetd";

    public string Decrypt(string Input)
    {
        Byte[] inputByteArray = new Byte[Input.Length];
        try
        {
            key = System.Text.Encoding.UTF8.GetBytes(EncryptionKey.Substring(0, 8));
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            inputByteArray = Convert.FromBase64String(Input);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(key, IV), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            Encoding encoding = Encoding.UTF8;
            return encoding.GetString(ms.ToArray());
        }
        catch (Exception ex)
        {
            return "";
        }
    }


    // i copied this code
    public string Encrypt(string Input)
    {
        try
        {
            key = System.Text.Encoding.UTF8.GetBytes(EncryptionKey.Substring(0, 8));
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            Byte[] inputByteArray = Encoding.UTF8.GetBytes(Input);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(key, IV), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return Convert.ToBase64String(ms.ToArray());
        }
        catch (Exception ex)
        {
            return "";
        }
    }


    // Ledger Summaries ////
    [WebMethod]
    public DataSet CreateLedgerVoucherMonthlySummary(string SKey, int InstId, int FinancialYearId, int DepartmentId, int GroupId, string LedgerIdz, int ForInstId)
    {
        try
        {
            param = new SqlParameter[6];
            param[0] = new SqlParameter("@InstId", InstId);
            param[1] = new SqlParameter("@FinancialYearId", FinancialYearId);
            param[2] = new SqlParameter("@GroupId", GroupId);
            param[3] = new SqlParameter("@DepartmentId", DepartmentId);
            param[4] = new SqlParameter("@LedgerIdz", LedgerIdz);
            param[5] = new SqlParameter("@ForInstId", ForInstId);
            ds = SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.CreateLedgerVoucherMonthlySummary", param);
            SKey = "";
        }
        catch (Exception ex)
        {
            SKey = ex.ToString();
        }
        return ds;
    }

    [WebMethod]
    public DataSet CreateLedgerGroupSummary(string SKey, int InstId, int FinancialYearId, int DepartmentId, int GroupId, int AccountGroupId, int ForInstId)
    {
        try
        {
            param = new SqlParameter[6];
            param[0] = new SqlParameter("@InstId", InstId);
            param[1] = new SqlParameter("@FinancialYearId", FinancialYearId);
            param[2] = new SqlParameter("@GroupId", GroupId);
            param[3] = new SqlParameter("@DepartmentId", DepartmentId);
            param[4] = new SqlParameter("@AccountGroupId", AccountGroupId);
            param[5] = new SqlParameter("@ForInstId", ForInstId);
            ds = SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.CreateLedgerGroupSummary", param);
            SKey = "";
        }
        catch (Exception ex)
        {
            SKey = ex.ToString();
        }
        return ds;
    }

    [WebMethod]
    public DataSet CreateLedgerStatementOfAccounts(string SKey, string InstIdz, int FinancialYearId, int DepartmentId, int GroupId, bool IsShowZeroBalance, bool ShowOnlyOpeningBalaces)
    {
        try
        {
            param = new SqlParameter[6];
            param[0] = new SqlParameter("@InstIdz", InstIdz);
            param[1] = new SqlParameter("@FinancialYearId", FinancialYearId);
            param[2] = new SqlParameter("@GroupId", GroupId);
            param[3] = new SqlParameter("@DepartmentId", DepartmentId);
            param[4] = new SqlParameter("@IsShowZeroBalance", IsShowZeroBalance);
            param[5] = new SqlParameter("@ShowOnlyOpeningBalaces", ShowOnlyOpeningBalaces);
            ds = SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.CreateLedgerStatementOfAccounts", param);
            SKey = "";
        }
        catch (Exception ex)
        {
            SKey = ex.ToString();
        }
        return ds;
    }



    [WebMethod]
    public DataSet FinancialReportsTrialBalanceSheet(string SKey, string InstIdz, int FinancialYearId, int GroupId)
    {
        try
        {
            param = new SqlParameter[3];
            param[0] = new SqlParameter("@InstIdz", InstIdz);
            param[1] = new SqlParameter("@FinancialYearId", FinancialYearId);
            param[2] = new SqlParameter("@GroupId", GroupId);

            ds = SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.FinancialReportsTrialBalanceSheet", param);
            SKey = "";
        }
        catch (Exception ex)
        {
            SKey = ex.ToString();
        }
        return ds;
    }

    [WebMethod]
    public DataSet CreateLedgerVoucher(string SKey, DateTime FromDate, DateTime ToDate, int LedgerId, int InstId, int DepartmentId, int ForInstId)
    {
        try
        {
            param = new SqlParameter[6];
            param[0] = new SqlParameter("@FromDate", FromDate);
            param[1] = new SqlParameter("@ToDate", ToDate);
            param[2] = new SqlParameter("@LedgerId", LedgerId);
            param[3] = new SqlParameter("@InstId", InstId);
            param[4] = new SqlParameter("@DepartmentId", DepartmentId);
            param[5] = new SqlParameter("@ForInstId", ForInstId);

            ds = SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.CreateLedgerVoucher", param);
            SKey = "";
        }
        catch (Exception ex)
        {
            SKey = ex.ToString();
        }
        return ds;
    }

    [WebMethod]
    public DataSet AccountGenreteLedgerTranscationListAll(string SKey, DateTime FromDate, DateTime ToDate, int LedgerId, int InstId, int DepartmentId, int FinancialYearId, int ForInstId, int AccountGroupId)
    {
        try
        {
            //param = new SqlParameter[8];
            //param[0] = new SqlParameter("@FromDate", FromDate);
            //param[1] = new SqlParameter("@ToDate", ToDate);
            //param[2] = new SqlParameter("@LedgerId", LedgerId);
            //param[3] = new SqlParameter("@InstId", InstId);
            //param[4] = new SqlParameter("@DepartmentId", DepartmentId);
            //param[5] = new SqlParameter("@ForInstId", ForInstId);
            //param[6] = new SqlParameter("@FinancialYearId", FinancialYearId);
            //param[7] = new SqlParameter("@AccountGroupId", AccountGroupId);
            //ds = SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.AccountGenreteLedgerTranscationListAll", param);
            //SKey = "";




            SqlCommand cm = new SqlCommand("Accounts.AccountGenreteLedgerTranscationListAll", con);
            cm.CommandTimeout = 0;
            cm.CommandType = CommandType.StoredProcedure;
            cm.Parameters.AddWithValue("@InstId", InstId);
            cm.Parameters.AddWithValue("@DepartmentId", DepartmentId);
            cm.Parameters.AddWithValue("@FinancialYearId", FinancialYearId);
            cm.Parameters.AddWithValue("@FromDate", FromDate);
            cm.Parameters.AddWithValue("@ToDate", ToDate);
            cm.Parameters.AddWithValue("@ForInstId", ForInstId);
            cm.Parameters.AddWithValue("@LedgerId", LedgerId);
            cm.Parameters.AddWithValue("@AccountGroupId", AccountGroupId);



            SqlDataAdapter da = new SqlDataAdapter(cm);

            //DataSet ds = new DataSet();
            ds = new DataSet();
            da.Fill(ds);
            da.Dispose();
            cm.Dispose();


        }

        catch (Exception ex)
        {
            SKey = ex.ToString();
        }
        return ds;
    }

    [WebMethod]
    public DataSet FinancialReportsBalanceSheet(string SKey, int GroupId, int InstId, int DepartmentId, int FinancialYearId, bool IsShowZeroBalance, bool ShowOnlyOpeningBalaces
        , DateTime FromDate, DateTime ToDate, bool IsIncomeExpenditure, int ForInstId, bool IsTrialBalanceSheet, string TGuid, bool IsTrialWithDetailBalance)
    {
        try
        {
            //param = new SqlParameter[11];
            //param[0] = new SqlParameter("@GroupId", GroupId);
            //param[1] = new SqlParameter("@InstId", InstId);
            //param[2] = new SqlParameter("@DepartmentId", DepartmentId);
            //param[3] = new SqlParameter("@FinancialYearId", FinancialYearId);
            //param[4] = new SqlParameter("@IsShowZeroBalance", IsShowZeroBalance);
            //param[5] = new SqlParameter("@ShowOnlyOpeningBalaces", ShowOnlyOpeningBalaces);
            //param[6] = new SqlParameter("@FromDate", FromDate);
            //param[7] = new SqlParameter("@ToDate", ToDate);
            //param[8] = new SqlParameter("@IsIncomeExpenditure", IsIncomeExpenditure);
            //param[9] = new SqlParameter("@ForInstId", ForInstId);
            //param[10] = new SqlParameter("@IsTrialBalanceSheet", IsTrialBalanceSheet);

            //ds = SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.FinancialReportsBalanceSheet", param);


            SqlCommand cm = new SqlCommand("Accounts.FinancialReportsBalanceSheet", con);
            cm.CommandTimeout = 0;
            cm.CommandType = CommandType.StoredProcedure;
            cm.Parameters.AddWithValue("@GroupId", GroupId);
            cm.Parameters.AddWithValue("@InstId", InstId);
            cm.Parameters.AddWithValue("@DepartmentId", DepartmentId);
            cm.Parameters.AddWithValue("@FinancialYearId", FinancialYearId);
            cm.Parameters.AddWithValue("@IsShowZeroBalance", IsShowZeroBalance);
            cm.Parameters.AddWithValue("@ShowOnlyOpeningBalaces", ShowOnlyOpeningBalaces);
            cm.Parameters.AddWithValue("@FromDate", FromDate);
            cm.Parameters.AddWithValue("@ToDate", ToDate);
            cm.Parameters.AddWithValue("@IsIncomeExpenditure", IsIncomeExpenditure);
            cm.Parameters.AddWithValue("@ForInstId", ForInstId);
            cm.Parameters.AddWithValue("@IsTrialBalanceSheet", IsTrialBalanceSheet);
            cm.Parameters.AddWithValue("@IsTrialWithDetailBalance", IsTrialWithDetailBalance);
            cm.Parameters.AddWithValue("@Guid", TGuid);



            SqlDataAdapter da = new SqlDataAdapter(cm);

            //DataSet ds = new DataSet();
            ds = new DataSet();
            da.Fill(ds);
            da.Dispose();
            cm.Dispose();



            SKey = "";
        }
        catch (Exception ex)
        {
            SKey = ex.ToString();
        }
        return ds;
    }

    [WebMethod]
    public DataSet AccountGenreteBalanceSheet(string SKey, int GroupId, int InstId, int DepartmentId, int FinancialYearId, bool IsShowZeroBalance, bool ShowOnlyOpeningBalaces
        , DateTime FromDate, DateTime ToDate, int ForInstId, string TGuid, decimal SurPlusDeficitAmount)
    {
        try
        {
            using (SqlConnection constr = new SqlConnection(con.ConnectionString))
            {
                using (SqlCommand cm = new SqlCommand())
                {
                    cm.Connection = constr;
                    cm.CommandTimeout = 0;
                    cm.CommandText = "Accounts.AccountGenreteBalanceSheet";
                    cm.CommandType = CommandType.StoredProcedure;
                    cm.Parameters.AddWithValue("@GroupId", GroupId);
                    cm.Parameters.AddWithValue("@InstId", InstId);
                    cm.Parameters.AddWithValue("@DepartmentId", DepartmentId);
                    cm.Parameters.AddWithValue("@FinancialYearId", FinancialYearId);
                    cm.Parameters.AddWithValue("@IsShowZeroBalance", IsShowZeroBalance);
                    cm.Parameters.AddWithValue("@ShowOnlyOpeningBalaces", ShowOnlyOpeningBalaces);
                    cm.Parameters.AddWithValue("@FromDate", FromDate);
                    cm.Parameters.AddWithValue("@ToDate", ToDate);
                    cm.Parameters.AddWithValue("@ForInstId", ForInstId);
                    cm.Parameters.AddWithValue("@Guid", TGuid);
                    cm.Parameters.AddWithValue("@tSD", SurPlusDeficitAmount);
                    SqlDataAdapter da1 = new SqlDataAdapter(cm);
                    da1.Fill(ds);
                    da1.Dispose();
                    cm.Dispose();
                }
            }
            SKey = "";
        }
        catch (Exception ex)
        {
            SKey = ex.ToString();
        }
        return ds;
    }


    [WebMethod]
    public DataSet BalanceSheetMenntal(string SKey, int GroupId, int InstId, int DepartmentId, int FinancialYearId, bool IsShowZeroBalance, bool ShowOnlyOpeningBalaces
       , DateTime FromDate, DateTime ToDate, int ForInstId, string TGuid, decimal SurPlusDeficitAmount)
    {
        try
        {
            using (SqlConnection constr = new SqlConnection(con.ConnectionString))
            {
                using (SqlCommand cm = new SqlCommand())
                {
                    cm.Connection = constr;
                    cm.CommandTimeout = 0;
                    cm.CommandText = "Accounts.BalanceSheetMenntal";
                    cm.CommandType = CommandType.StoredProcedure;
                    cm.Parameters.AddWithValue("@GroupId", GroupId);
                    cm.Parameters.AddWithValue("@InstId", InstId);
                    cm.Parameters.AddWithValue("@DepartmentId", DepartmentId);
                    cm.Parameters.AddWithValue("@FinancialYearId", FinancialYearId);
                    cm.Parameters.AddWithValue("@IsShowZeroBalance", IsShowZeroBalance);
                    cm.Parameters.AddWithValue("@ShowOnlyOpeningBalaces", ShowOnlyOpeningBalaces);
                    cm.Parameters.AddWithValue("@FromDate", FromDate);
                    cm.Parameters.AddWithValue("@ToDate", ToDate);
                    cm.Parameters.AddWithValue("@ForInstId", ForInstId);
                    cm.Parameters.AddWithValue("@Guid", TGuid);
                    cm.Parameters.AddWithValue("@tSD", SurPlusDeficitAmount);
                    SqlDataAdapter da1 = new SqlDataAdapter(cm);
                    da1.Fill(ds);
                    da1.Dispose();
                    cm.Dispose();
                }
            }
            SKey = "";
        }
        catch (Exception ex)
        {
            SKey = ex.ToString();
        }
        return ds;
    }


    [WebMethod]
    public DataSet AccountTrialBalanceDisplayWiseCollegeNameId(string SKey, int GroupId, int InstId, int DepartmentId, int FinancialYearId
        , DateTime FromDate, DateTime ToDate, int ForInstId, int AccountGroupId, string tguid, bool IsShowOnlyOpeningBalacesJas)
    {
        try
        {
            param = new SqlParameter[10];
            param[0] = new SqlParameter("@GroupId", GroupId);
            param[1] = new SqlParameter("@InstId", InstId);
            param[2] = new SqlParameter("@DepartmentId", DepartmentId);
            param[3] = new SqlParameter("@FinancialYearId", FinancialYearId);
            param[4] = new SqlParameter("@FromDate", FromDate);
            param[5] = new SqlParameter("@ToDate", ToDate);
            param[6] = new SqlParameter("@ForInstId", ForInstId);
            param[7] = new SqlParameter("@AccountGroupId", AccountGroupId);
            param[8] = new SqlParameter("@IsShowOnlyOpeningBalacesJas", IsShowOnlyOpeningBalacesJas);
            param[9] = new SqlParameter("@Guid", tguid);
            ds = SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.AccountTrialBalanceDisplayWiseCollegeNameId", param);
            SKey = "";
        }
        catch (Exception ex)
        {
            SKey = ex.ToString();
        }
        return ds;
    }


    [WebMethod]
    public DataSet GetAccountGroup(string SKey, int InstId, int showInLedger)
    {
        param = new SqlParameter[2];
        param[0] = new SqlParameter("@InstId", InstId);
        param[1] = new SqlParameter("@showInLedger", showInLedger);
        ds = SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.GetAccountGroup", param);

        return ds;
    }

    [WebMethod]
    public DataSet AccountGroupDisplay(string SKey, int AccountGroupId)
    {
        try
        {
            param = new SqlParameter[1];
            param[0] = new SqlParameter("@AccountGroupId", AccountGroupId);

            ds = SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.AccountGroupDisplay", param);
            SKey = "";
        }
        catch (Exception ex)
        {
            SKey = ex.ToString();
        }
        return ds;
    }

    [WebMethod]
    public DataSet GetAccountLedger(string SKey, int InstId)
    {
        param = new SqlParameter[1];
        param[0] = new SqlParameter("@InstId", InstId);
        ds = SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.GetAccountLedger", param);

        return ds;
    }

    [WebMethod]
    public DataSet AccountLedgerDisplay(string SKey, int LedgerId)
    {
        try
        {
            param = new SqlParameter[1];
            param[0] = new SqlParameter("@LedgerId", LedgerId);

            ds = SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.AccountLedgerDisplay", param);
            SKey = "";
        }
        catch (Exception ex)
        {
            SKey = ex.ToString();
            SKey = ex.ToString();
        }
        return ds;
    }


    [WebMethod]
    public DataSet UniversalClassesvVoucherType()
    {

        try
        {
            ds = SqlHelper.ExecuteDataset(con, CommandType.Text, "select VoucherTypeId,VoucherTypeName from  Accounts.VVoucherType");
        }
        catch (Exception ex)
        {

        }
        return ds;
    }





    /// <summary>
    /// Function to fill all cash or bank ledgers for combobox
    /// </summary>
    /// <param name="cmbCashOrBank"></param>
    /// <param name="isAll"></param>
    [WebMethod]
    public DataSet CashOrBankComboFill(bool isAll, string SKey, int InstId, int FinancialYearId, int DepartmentId, int GroupId)
    {
        try
        {
            param = new SqlParameter[4];
            param[0] = new SqlParameter("@InstId", InstId);
            param[1] = new SqlParameter("@FinancialYearId", FinancialYearId);
            param[2] = new SqlParameter("@GroupId", GroupId);
            param[3] = new SqlParameter("@DepartmentId", DepartmentId);
            ds = SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.CashOrBankComboFill", param);
            SKey = "";
        }
        catch (Exception ex)
        {
            SKey = ex.ToString();
        }
        return ds;
    }


    /// <summary>
    /// Function to fill AccountLedgers for combobox
    /// </summary>
    /// <returns></returns>
    [WebMethod]
    public DataSet AccountLedger(string SKey, int InstId, int FinancialYearId, int DepartmentId, int GroupId)
    {
        try
        {
            param = new SqlParameter[4];
            param[0] = new SqlParameter("@InstId", InstId);
            param[1] = new SqlParameter("@FinancialYearId", FinancialYearId);
            param[2] = new SqlParameter("@GroupId", GroupId);
            param[3] = new SqlParameter("@DepartmentId", DepartmentId);
            ds = SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.AccountLedgerViewAll", param);
            SKey = "";
        }
        catch (Exception ex)
        {
            SKey = ex.ToString();
        }
        return ds;
    }



    /// <summary>
    /// Function to fill AccountLedgers for combobox
    /// </summary>
    /// <returns></returns>
    [WebMethod]
    public DataSet AccountLedgerForTransaction(string SKey, int InstId, int FinancialYearId, int DepartmentId, int GroupId,int ShowInTransactionPage)
    {
        try
        {
            param = new SqlParameter[5];
            param[0] = new SqlParameter("@InstId", InstId);
            param[1] = new SqlParameter("@FinancialYearId", FinancialYearId);
            param[2] = new SqlParameter("@GroupId", GroupId);
            param[3] = new SqlParameter("@DepartmentId", DepartmentId);
            param[4] = new SqlParameter("@ShowInTransactionPage", ShowInTransactionPage);
            ds = SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.AccountLedgerForTransaction", param);
            SKey = "";
        }
        catch (Exception ex)
        {
            SKey = ex.ToString();
        }
        return ds;
    }


    /// <summary>
    /// Function to fill AccountLedgers for combobox
    /// </summary>
    /// <returns></returns>
    [WebMethod]
    public DataSet TransactionMasterAndDetailById(string SKey, int InstId, int TransactionMasterId)
    {
        try
        {
            param = new SqlParameter[2];
            param[0] = new SqlParameter("@InstId", InstId);
            param[1] = new SqlParameter("@TransactionMasterId", TransactionMasterId);
            ds = SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.TransactionMasterAndDetailById", param);
            SKey = "";
        }
        catch (Exception ex)
        {
            SKey = ex.ToString();
        }
        return ds;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="transactionMaster"></param>
    /// <param name="transactionDetail"></param>
    /// <param name="OldTransactionMasterId">In case of Edit, It will delete complete old entry and insert new entry</param>
    /// <returns></returns>
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public int TransactionMasterInsert(int InstId, TransactionMaster transactionMaster, List<TransactionDetail> transactionDetail, int OldTransactionMasterId)
    {
        try
        {
            if (OldTransactionMasterId > 0)
            {
                TransactionMasterDelete(OldTransactionMasterId);
            }
            List<TransactionDetail> transaction = new List<TransactionDetail>();//Added for backup changes
            string skey = string.Empty;//Added for backup changes
            // Set or Reset Default values in MasterTable
            transactionMaster.AdminInsertDate = null;
            transactionMaster.AdminInsertUserAccountId = null;
            transactionMaster.InsertDate = DateTime.Now;
            transactionMaster.UpdateDate = DateTime.Now;

            db.Add(transactionMaster);
            db.SaveChanges();
            int TransactionMasterId = transactionMaster.TransactionMasterId; // Get MaximumId (like Scope_Indentity in SQL)
            Guid gid = Guid.NewGuid();

            foreach (TransactionDetail LoopTD in transactionDetail)
            {
                LoopTD.TransactionMasterId = TransactionMasterId; //MasterId as maximum
                LoopTD.UniqueID = gid.ToString();
                db.Add(LoopTD);
                transaction.Add(LoopTD);
            }
            string jsonData = JsonConvert.SerializeObject(transaction);//Added for backup changes
            int count= TransactionDetailsBackup(skey, jsonData);//Added for backup changes
            db.SaveChanges();
            TransactionNotificationInsert(InstId, TransactionMasterId);
            if (transactionDetail.Count == count)
                return TransactionMasterId;
            else
                return 0;
        }

        catch (Exception ex)
        {
            // Delete entries from Both tables against TransactionMasterId
            // it will be implemented in future. when started in proct or finalzation time //Menntal
            return 0;
        }
    }


    [WebMethod]
    public DataSet TransactionNotificationInsert(int InstId, int TMId)
    {
        try
        {
            param = new SqlParameter[2];
            param[0] = new SqlParameter("@TMId", TMId);
            param[1] = new SqlParameter("@InstId", InstId);
            ds = SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.TransactionNotificationInsert", param);
        }
        catch (Exception ex)
        {

        }
        return ds;
    }

    [WebMethod]
    public DataSet SelectMaximumNewVoucherNoAutoGenerate(int InstId, int FinacialYearId, int VoucherTypeId)
    {
        try
        {
            param = new SqlParameter[3];
            param[0] = new SqlParameter("@InstId", InstId);
            param[1] = new SqlParameter("@FinacialYearId", FinacialYearId);
            param[2] = new SqlParameter("@VoucherTypeId", VoucherTypeId);

            ds = SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.SelectMaximumNewVoucherNoAutoGenerate", param);
        }
        catch (Exception ex)
        {

        }
        return ds;
    }


    [WebMethod]
    public bool TransactionMasterDelete(int TransactionMasterId)
    {
        try
        {
            var obj = db.TransactionMasters.Where(ab => ab.TransactionMasterId == TransactionMasterId);
            db.Delete(obj);
            db.SaveChanges();

            var obj1 = db.TransactionDetails.Where(ab => ab.TransactionMasterId == TransactionMasterId);
            db.Delete(obj1);
            db.SaveChanges();
            return true;
        }

        catch (Exception ex)
        {
            return false;
        }
    }
    [WebMethod]
    public DataSet vAccountGroupsByAll(string SKey, int InstId)
    {
        try
        {
            param = new SqlParameter[1];
            param[0] = new SqlParameter("@InstId", InstId);
            ds = SqlHelper.ExecuteDataset(con, CommandType.Text, "select AccountGroupId,AccountGroupName,Nature from Accounts.VAccountGroup where InstId=@InstId", param);
        }
        catch (Exception ex)
        {

        }
        return ds;
    }

    [WebMethod]
    public DataSet AccountGroupWhosNotAChild(string SKey, int InstId)
    {
        try
        {
            param = new SqlParameter[1];
            param[0] = new SqlParameter("@InstId", InstId);
            ds = SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.AccountGroupWhosNotAChild", param);
        }
        catch (Exception ex)
        {

        }
        return ds;
    }


    [WebMethod]
    public bool InsertAccountGroup(string SKey, AccountGroupMaster Ac, int InstId, bool IsModify,bool IsCommonGroup)
    {
        try
        {
            if (IsModify)
            {
                var al = (from obj1 in db.AccountGroupMasters
                          where obj1.AccountGroupId == Ac.AccountGroupId
                          select obj1).FirstOrDefault();

                al.AccountGroupName = Ac.AccountGroupName;
                al.AccountGroupNameAlias = Ac.AccountGroupNameAlias;
                al.AffectGrossProfit = Ac.AffectGrossProfit;
                al.GroupUnder = Ac.GroupUnder;
                al.Narration = Ac.Narration;
                al.Nature = Ac.Nature;
                al.UpdateDate = DateTime.Now;
                al.UpdateUserAccountId = Ac.UpdateUserAccountId;
                db.SaveChanges();
                UpdateAccountGroupById(SKey, Ac.AccountGroupId,IsCommonGroup);
                return true;
            }

            else
            {

                Ac.InsertDate = System.DateTime.Now;
                Ac.UpdateDate = System.DateTime.Now;
                db.Add(Ac);
                db.SaveChanges();

                //AccountGroup ag = new AccountGroup();
                //ag.AccountGroupId = Ac.AccountGroupId;
                //ag.InstId = InstId;
                //db.Add(ag);
                //db.SaveChanges();
                InsertAccountGroupById(SKey,Ac.AccountGroupId,InstId,IsCommonGroup);


                ///// Resetting All Child Group/////
                SqlHelper.ExecuteNonQuery(con, CommandType.StoredProcedure, "Accounts.Ideas_AllChildAccountUpdator");
                return true;
            }
        }
        catch { return false; }

    }

    [WebMethod]
    public DataTable InsertAccountGroupById(string SKey,int AccountGroupId, int InstId, bool IsCommonGroup)
    {
        try
        {
            int IsEnable = 0;
            param = new SqlParameter[4];
            param[0] = new SqlParameter("@AccountGroupId", AccountGroupId);
            param[1] = new SqlParameter("@InstId", InstId);
            param[2] = new SqlParameter("@IsEnable", IsEnable);
            param[3] = new SqlParameter("@IsCommonGroup", IsCommonGroup);
            ds = SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.InsertAccountGroupById", param);
            return ds.Tables[0];
        }
        catch(Exception ex)
        {
            return dt; }
    }

    [WebMethod]
    public DataSet UpdateAccountGroupById(string SKey, int AccountGroupId, bool IsCommonGroup)
    {
        try
        {
            param = new SqlParameter[2];
            param[0] = new SqlParameter("@AccountGroupId", AccountGroupId);
            param[1] = new SqlParameter("@IsCommonGroup", IsCommonGroup);
            ds = SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.UpdateAccountGroupById", param);
            return ds;
        }
        catch { return ds; }
    }

    [WebMethod]
    public DataSet UpdateAccountLedgerById(string SKey, int LedgerId)
    {
        try
        {
            param = new SqlParameter[1];
            param[0] = new SqlParameter("@LedgerId", LedgerId);
            ds = SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.UpdateAccountLedgerById", param);
            return ds;
        }
        catch(Exception ex)
        {
return ds; }
    }

    [WebMethod]
    public DataSet AccountGroupById(string SKey, int InstId)
    {

        try
        {
            param = new SqlParameter[1];
            param[0] = new SqlParameter("@InstId", InstId);
            ds = SqlHelper.ExecuteDataset(con, CommandType.Text, "select AG1.AccountGroupId,AG1.AccountGroupName,AG1.Nature,AG2.AccountGroupName AS Under "
                + "  from Accounts.vAccountGroup AS AG1 LEFT JOIN  Accounts.AccountGroupMaster as AG2 on Ag1.GroupUnder =AG2.AccountGroupId "
                + " where AG1.InstId=@InstId ORDER BY ag1.AccountGroupName", param);
        }
        catch (Exception ex)
        {

        }
        return ds;
    }

    [WebMethod]
    public DataSet AccountGroupNature(string SKey)
    {
        try
        {
            ds = SqlHelper.ExecuteDataset(con, CommandType.Text, "select NatureId,NatureShortTitle from Accounts.AccountGroupNature");
        }
        catch (Exception ex)
        {

        }
        return ds;
    }

    [WebMethod]
    public string NatureById(string SKey, int NatuerId)
    {
        string Nature = (from i in db.AccountGroupMasters where i.AccountGroupId == NatuerId select i.Nature).SingleOrDefault().ToString();
        return Nature;
    }


    [WebMethod]
    public DataSet AccountLedgerByAll(string SKey, int InstId, int FinancialYearId)
    {
        try
        {
            param = new SqlParameter[2];
            param[0] = new SqlParameter("@InstId", InstId);
            param[1] = new SqlParameter("@FinancialYearId", FinancialYearId);
            ds = SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.AccountLedgerByAll", param);
        }
        catch (Exception ex)
        {

        }
        return ds;
    }


    [WebMethod]
    public bool InsertAccountLedger(string SKey, AccountLedgerMaster alm, AccountLedger AL, AccountLedgerOpeningBalance ob, bool IsModify, int OldForInstId)
    {
        try
        {
            if (IsModify)
            {
                var al = (from obj1 in db.AccountLedgerMasters
                          where obj1.LedgerId == alm.LedgerId
                          select obj1).FirstOrDefault();

                al.AccountGroupId = alm.AccountGroupId;
                al.Address = alm.Address;
                al.CrOrDr = alm.CrOrDr;
                al.CST = alm.CST;
                al.Email = alm.Email;
                al.LedgerName = alm.LedgerName;
                al.LedgerNameAlias = alm.LedgerNameAlias;
                al.LedgerNamePrint = alm.LedgerNamePrint;
                al.Mobile = alm.Mobile;
                al.Narration = alm.Narration;
                al.PAN = alm.PAN;
                al.Phone = alm.Phone;
                al.TIN = alm.TIN;
                al.UpdateDate = DateTime.Now;
                al.UpdateUserAccountId = alm.UpdateUserAccountId;
                al.IsUnderSecretary = alm.IsUnderSecretary;
                al.IsShowMainCollege = alm.IsShowMainCollege;

                db.SaveChanges();

                UpdateAccountLedgerOpeningBalance(SKey, alm.LedgerId, ob.OB_OpeningBalance, ob.OB_CrOrDr, ob.OB_FinancialYearId, ob.OB_InstId, Convert.ToInt32(ob.OB_ForInstId), OldForInstId);
                UpdateAccountLedgerById(SKey, alm.LedgerId);
                //var obj = (from ac in db.AccountLedgerOpeningBalances
                //           where ac.OB_InstId == ob.OB_InstId && ac.OB_LedgerId == ob.OB_LedgerId && ac.OB_FinancialYearId == ob.OB_FinancialYearId
                //           select ac).FirstOrDefault();
                //if (obj == null)
                //{
                //    AccountLedgerOpeningBalance abc = new AccountLedgerOpeningBalance();
                //    abc.OB_CrOrDr = ob.OB_CrOrDr;
                //    abc.OB_DepartmentId = ob.OB_DepartmentId;
                //    abc.OB_FinancialYearId = ob.OB_FinancialYearId;
                //    abc.OB_InsertDate = DateTime.Now;
                //    abc.OB_InstId = ob.OB_InstId;
                //    abc.OB_LedgerId = ob.OB_InstId;
                //    abc.OB_OpeningBalance = ob.OB_OpeningBalance;
                //    db.Add(abc);
                //    db.SaveChanges();
                //}
                //else
                //{
                //    obj.OB_OpeningBalance = ob.OB_OpeningBalance;
                //    obj.OB_CrOrDr = ob.OB_CrOrDr;
                //    db.SaveChanges();
                //}
            }
            else
            {
                int Id = (from i in db.AccountLedgerMasters select i.LedgerId).Max(); // Getting Max LedgerId from Master Table
                alm.LedgerId = Id + 1;
                alm.InsertDate = System.DateTime.Now;
                alm.UpdateDate = System.DateTime.Now;

                db.Add(alm);
                db.SaveChanges();

                //AccountLedger AL1 = new AccountsDB.AccountLedger();
                //AL1.DepartmentId = 0;
                //AL1.ForInstId = AL.ForInstId;
                //AL1.LedgerId = alm.LedgerId; 
                //AL1.InstId = AL.InstId;
                //db.Add(AL1);
                //db.SaveChanges();

                InsertLedgerDataForAllInst(SKey, AL.InstId, AL.ForInstId, alm.LedgerId);

                ob.OB_InsertDate = DateTime.Now;
                ob.OB_LedgerId = alm.LedgerId;
                ob.OB_ForInstId = AL.ForInstId;
                db.Add(ob);
                db.SaveChanges();
                UpdateAccountLedgerById(SKey, alm.LedgerId);
            }
            return true;
        }
        catch (Exception ex) { return false; }

    }


    [WebMethod]
    public bool UpdateAccountLedgerOpeningBalance(string SKey, int LedgerId, Decimal OpeningBalance, string CrDr, int FinancialYearId, int InstId, int ForInstId, int OldForInstId)
    {
        try
        {
            var obj = (from ob in db.AccountLedgerOpeningBalances
                       where ob.OB_LedgerId == LedgerId && ob.OB_InstId == InstId && ob.OB_FinancialYearId == FinancialYearId && ob.OB_ForInstId == OldForInstId
                       select ob).FirstOrDefault();

            if (obj == null)
            {
                AccountLedgerOpeningBalance avc = new AccountLedgerOpeningBalance();
                avc.OB_OpeningBalance = OpeningBalance;
                avc.OB_CrOrDr = CrDr;
                avc.OB_DepartmentId = 0;
                avc.OB_FinancialYearId = FinancialYearId;
                avc.OB_InsertDate = DateTime.Now;
                avc.OB_InstId = InstId;
                avc.OB_LedgerId = LedgerId;
                avc.OB_ForInstId = ForInstId;
                db.Add(avc);


                db.SaveChanges();
            }
            else
            {

                obj.OB_OpeningBalance = OpeningBalance;
                obj.OB_CrOrDr = CrDr;
                obj.OB_InsertDate = DateTime.Now;
                obj.OB_ForInstId = ForInstId;
                db.SaveChanges();

            }

            return true;
        }
        catch { return false; }

    }

    [WebMethod]
    public DataTable UpdateAccountLedger(string SKey, int InstId, int OldForInstId, int NewForInstId, int LedgerId, decimal OpeningBalance, int FinancialYearId)
    {
        try
        {
            param = new SqlParameter[6];
            param[0] = new SqlParameter("@InstId", InstId);
            param[1] = new SqlParameter("@OldForInstId", OldForInstId);
            param[2] = new SqlParameter("@NewForInstId", NewForInstId);
            param[3] = new SqlParameter("@LedgerId", LedgerId);
            param[4] = new SqlParameter("@OpeningBalance", OpeningBalance);
            param[5] = new SqlParameter("@FinancialYearId", FinancialYearId);
            ds = SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.UpdateAccountLedger", param);
            return ds.Tables[0];
        }
        catch { return dt; }
    }

    [WebMethod]
    public DataTable InsertLedgerDataForAllInst(string SKey, int InstId, int ForInstId, int LedgerId)
    {
        try
        {
            param = new SqlParameter[3];
            param[0] = new SqlParameter("@InstId", InstId);
            param[1] = new SqlParameter("@ForInstId", ForInstId);
            param[2] = new SqlParameter("@LedgerId", LedgerId);
            ds = SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.InsertLedgerDataForAllInst", param);
            return ds.Tables[0];
        }
        catch { return dt; }
    }



    //[WebMethod]
    //public DataSet ReportingTrialBalanceSheet(string SKey, int GroupId, string InstIdz, int FinancialYearId, int DepartmentId, bool IsShowZeroBalance, string ReportName, bool ShowOnlyOpeningBalaces)
    //{
    //    string FileName = "";
    //    try
    //    {
    //        ds = CreateLedgerStatementOfAccounts(SKey, InstIdz, FinancialYearId, DepartmentId, GroupId, IsShowZeroBalance, ShowOnlyOpeningBalaces);


    //    }
    //    catch (Exception ex)
    //    {

    //    }
    //    return FileName;
    //}


    //[WebMethod]
    //public string ReportingGroupSummary(string SKey, int GroupId, int InstId, int DepartmentId, int FinancialYearId, int AccountGroupId)
    //{
    //    string FileName = "";
    //    try
    //    {
    //        ds = CreateLedgerGroupSummary(SKey, InstId, FinancialYearId, DepartmentId, GroupId, AccountGroupId);

    //    }
    //    catch (Exception ex)
    //    {

    //    }
    //    return FileName;
    //}

    //[WebMethod]
    //public string ReportingMonthlySummary(string SKey, int GroupId, int InstId, int DepartmentId, int FinancialYearId, string LedgerIdz, String LedgerName)
    //{
    //    string FileName = "";
    //    try
    //    {
    //       //ds = CreateLedgerVoucherMonthlySummary(SKey, InstId, FinancialYearId, DepartmentId, GroupId, LedgerIdz);

    //    }
    //    catch (Exception ex)
    //    {

    //    }
    //    return FileName;
    //}


    //[WebMethod]
    //public string ReportingLedgerVoucher(string SKey, string DateString, string LedgerName, DataSet dataset)
    //{
    //    string FileName = "";
    //    try
    //    {

    //    }
    //    catch (Exception ex)
    //    {
    //        FileName = ex.Message;

    //    }
    //    return FileName;
    //}






    [WebMethod]
    public bool IsAccountLedgerIsAlreadyExist(string SKey, int InstId, string LedgerName, int LedgerId, int ForInstId)
    {
        var obj = (from ab in db.AccountLedgerMasters
                   join al in db.AccountLedgers on ab.LedgerId equals al.LedgerId
                   where al.InstId == InstId && ab.LedgerName == LedgerName
                    && ab.LedgerId != LedgerId && al.ForInstId == ForInstId
                   select ab).FirstOrDefault();

        if (obj == null) return false;
        return true;
    }

    [WebMethod]
    public bool IsAccountGroupIsAlreadyExist(string SKey, int InstId, string AccountGroupName, int AccountGroupId)
    {
        var obj = (from ab in db.AccountGroupMasters
                   join al in db.AccountGroups on ab.AccountGroupId equals al.AccountGroupId
                   where al.InstId == InstId && ab.AccountGroupName == AccountGroupName
                   && ab.AccountGroupId != AccountGroupId
                   select ab).FirstOrDefault();

        if (obj == null) return false;
        return true;
    }

    [WebMethod]
    public bool IsVoucherNoAlreadyExist(string SKey, int InstId, string VoucherNo, int TransactionMasterId)
    {
        var obj = (from ab in db.TransactionMasters
                   where ab.InstId == InstId && ab.VoucherNo == VoucherNo
                   && ab.TransactionMasterId != TransactionMasterId
                   select ab).FirstOrDefault();

        if (obj == null) return false;
        return true;
    }

    [WebMethod]
    public DataTable AccountLedgerDeleteById(string SKey, int LedgerId, int InstId)
    {
        try
        {
            param = new SqlParameter[2];
            param[0] = new SqlParameter("@LedgerId", LedgerId);
            param[1] = new SqlParameter("@InstId", InstId);
            ds = SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.AccountLedgerDeleteById", param);
            return ds.Tables[0];
        }
        catch { return dt; }
    }

    [WebMethod]
    public DataSet TransactionMasterAndDetailByVoucherNo(string SKey, int InstId, string VoucherNo, int VoucherTypeId)
    {
        try
        {
            param = new SqlParameter[3];
            param[0] = new SqlParameter("@InstId", InstId);
            param[1] = new SqlParameter("@VoucherNo", VoucherNo);
            param[2] = new SqlParameter("@VoucherTypeId", VoucherTypeId);
            ds = SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.TransactionMasterAndDetailByVoucherNo", param);
            SKey = "";
        }
        catch (Exception ex)
        {
            SKey = ex.ToString();
        }
        return ds;
    }


    [WebMethod]
    public DataSet CreateLedgerVoucherList(string SKey, DateTime FromDate, DateTime ToDate, int InstId, int VoucherTypeId, int DepartmentId, int ForInstId)
    {
        try
        {
            param = new SqlParameter[6];
            param[0] = new SqlParameter("@FromDate", FromDate);
            param[1] = new SqlParameter("@ToDate", ToDate);
            param[2] = new SqlParameter("@InstId", InstId);
            param[3] = new SqlParameter("@VoucherTypeId", VoucherTypeId);
            param[4] = new SqlParameter("@DepartmentId", DepartmentId);
            param[5] = new SqlParameter("@ForInstId", ForInstId);

            ds = SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.CreateLedgerVoucherList", param);
            SKey = "";
        }
        catch (Exception ex)
        {
            SKey = ex.ToString();
        }
        return ds;
    }

    [WebMethod]
    public DataTable AccountGroupDeleteById(string SKey, int AccountGroupId, int InstId)
    {
        try
        {
            param = new SqlParameter[2];
            param[0] = new SqlParameter("@AccountGroupId", AccountGroupId);
            param[1] = new SqlParameter("@InstId", InstId);
            ds = SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.AccountGroupDeleteById", param);
            return ds.Tables[0];
        }
        catch { return dt; }
    }

    [WebMethod]
    public DataTable AccountGroupEnableById(string SKey, int AccountGroupId,int IsEnable)
    {
        try
        {
            param = new SqlParameter[2];
            param[0] = new SqlParameter("@AccountGroupId", AccountGroupId);
            param[1] = new SqlParameter("@IsEnable", IsEnable);
            ds = SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.AccountGroupEnableById", param);
            return ds.Tables[0];
        }
        catch { return dt; }
    }

    [WebMethod]
    public DataTable AccountLedgerEnableById(string SKey, int LedgerId, int IsEnable)
    {
        try
        {
            param = new SqlParameter[2];
            param[0] = new SqlParameter("@LedgerId", LedgerId);
            param[1] = new SqlParameter("@IsEnable", IsEnable);
            ds = SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.AccountLedgerEnableById", param);
            return ds.Tables[0];
        }
        catch { return dt; }
    }


    [WebMethod]
    public DataSet CreateLedgerIncomeExpenditute(string SKey, int GroupId, bool IsIncomeExpenditure, int InstId, int DepartmentId,
        bool IsShowZeroBalance, int FinancialYearId, bool ShowOnlyOpeningBalaces, DateTime FromDate, DateTime ToDate, int ForInstId, int AccountGroupId, string TGuid, bool IsShowOnlyOpeningBalacesJas)
    {
        try
        {
            param = new SqlParameter[13];
            param[0] = new SqlParameter("@GroupId", GroupId);
            param[1] = new SqlParameter("@IsIncomeExpenditure", IsIncomeExpenditure);
            param[2] = new SqlParameter("@InstId", InstId);
            param[3] = new SqlParameter("@DepartmentId", DepartmentId);
            param[4] = new SqlParameter("@IsShowZeroBalance", IsShowZeroBalance);
            param[5] = new SqlParameter("@FinancialYearId", FinancialYearId);
            param[6] = new SqlParameter("@ShowOnlyOpeningBalaces", ShowOnlyOpeningBalaces);
            param[7] = new SqlParameter("@FromDate", FromDate);
            param[8] = new SqlParameter("@ToDate", ToDate);
            param[9] = new SqlParameter("@ForInstId", ForInstId);
            param[10] = new SqlParameter("@AccountGroupId", AccountGroupId);
            param[11] = new SqlParameter("@Guid", TGuid);
            param[12] = new SqlParameter("@IsShowOnlyOpeningBalacesJas", IsShowOnlyOpeningBalacesJas);
            ds = SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.CreateLedgerIncomeExpenditute", param);
            SKey = "";
        }
        catch (Exception ex)
        {
            SKey = ex.ToString();
        }
        return ds;
    }

    [WebMethod]
    public DataSet CreateLedgerCommonIncomeExpenditute(string SKey, int GroupId, bool IsIncomeExpenditure, int InstId, int DepartmentId,
        bool IsShowZeroBalance, int FinancialYearId, bool ShowOnlyOpeningBalaces, DateTime FromDate, DateTime ToDate, int ForInstId, int AccountGroupId, string TGuid, bool IsShowOnlyOpeningBalacesJas)
    {
        try
        {
            param = new SqlParameter[13];
            param[0] = new SqlParameter("@GroupId", GroupId);
            param[1] = new SqlParameter("@IsIncomeExpenditure", IsIncomeExpenditure);
            param[2] = new SqlParameter("@InstId", InstId);
            param[3] = new SqlParameter("@DepartmentId", DepartmentId);
            param[4] = new SqlParameter("@IsShowZeroBalance", IsShowZeroBalance);
            param[5] = new SqlParameter("@FinancialYearId", FinancialYearId);
            param[6] = new SqlParameter("@ShowOnlyOpeningBalaces", ShowOnlyOpeningBalaces);
            param[7] = new SqlParameter("@FromDate", FromDate);
            param[8] = new SqlParameter("@ToDate", ToDate);
            param[9] = new SqlParameter("@ForInstId", ForInstId);
            param[10] = new SqlParameter("@AccountGroupId", AccountGroupId);
            param[11] = new SqlParameter("@Guid", TGuid);
            param[12] = new SqlParameter("@IsShowOnlyOpeningBalacesJas", IsShowOnlyOpeningBalacesJas);
            ds = SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.CreateLedgerCommonIncomeExpenditute", param);
            SKey = "";
        }
        catch (Exception ex)
        {
            SKey = ex.ToString();
        }
        return ds;
    }


    [WebMethod]
    public DataSet CreateLedgerIncomeExpendituteCollegeGroup(string SKey, int GroupId, bool IsIncomeExpenditure, int InstId, int DepartmentId,
        bool IsShowZeroBalance, int FinancialYearId, bool ShowOnlyOpeningBalaces, DateTime FromDate, DateTime ToDate, int ForInstId, int AccountGroupId, string TGuId, bool IsShowOnlyOpeningBalacesJas)
    {
        try
        {

            //SqlCommand cm = new SqlCommand("Accounts.FinancialReportsBalanceSheet", con);
            //cm.CommandTimeout = 0;
            //cm.CommandType = CommandType.StoredProcedure;
            //cm.Parameters.AddWithValue("@GroupId", GroupId);
            //cm.Parameters.AddWithValue("@InstId", InstId);
            //cm.Parameters.AddWithValue("@DepartmentId", DepartmentId);
            //cm.Parameters.AddWithValue("@FinancialYearId", FinancialYearId);
            //cm.Parameters.AddWithValue("@IsShowZeroBalance", IsShowZeroBalance);
            //cm.Parameters.AddWithValue("@ShowOnlyOpeningBalaces", ShowOnlyOpeningBalaces);
            //cm.Parameters.AddWithValue("@FromDate", FromDate);
            //cm.Parameters.AddWithValue("@ToDate", ToDate);
            //cm.Parameters.AddWithValue("@IsIncomeExpenditure", IsIncomeExpenditure);
            //cm.Parameters.AddWithValue("@ForInstId", ForInstId);
            //cm.Parameters.AddWithValue("@IsTrialBalanceSheet", IsTrialBalanceSheet);
            //cm.Parameters.AddWithValue("@Guid", TGuid);



            //SqlDataAdapter da = new SqlDataAdapter(cm);

            ////DataSet ds = new DataSet();
            //ds = new DataSet();
            //da.Fill(ds);
            //da.Dispose();
            //cm.Dispose();


            param = new SqlParameter[13];
            param[0] = new SqlParameter("@GroupId", GroupId);
            param[1] = new SqlParameter("@IsIncomeExpenditure", IsIncomeExpenditure);
            param[2] = new SqlParameter("@InstId", InstId);
            param[3] = new SqlParameter("@DepartmentId", DepartmentId);
            param[4] = new SqlParameter("@IsShowZeroBalance", IsShowZeroBalance);
            param[5] = new SqlParameter("@FinancialYearId", FinancialYearId);
            param[6] = new SqlParameter("@ShowOnlyOpeningBalaces", ShowOnlyOpeningBalaces);
            param[7] = new SqlParameter("@FromDate", FromDate);
            param[8] = new SqlParameter("@ToDate", ToDate);
            param[9] = new SqlParameter("@ForInstId", ForInstId);
            param[10] = new SqlParameter("@AccountGroupId", AccountGroupId);
            param[11] = new SqlParameter("@GuId", TGuId);
            param[12] = new SqlParameter("@IsShowOnlyOpeningBalacesJas", IsShowOnlyOpeningBalacesJas);
            ds = SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.CreateLedgerIncomeExpendituteCollegeGroup", param);
            SKey = "";
        }
        catch (Exception ex)
        {
            SKey = ex.ToString();
        }
        return ds;
    }

    [WebMethod]
    public DataSet CreateLedgerCommonIncomeExpendituteCollegeGroup(string SKey, int GroupId, bool IsIncomeExpenditure, int InstId, int DepartmentId,
        bool IsShowZeroBalance, int FinancialYearId, bool ShowOnlyOpeningBalaces, DateTime FromDate, DateTime ToDate, int ForInstId, int AccountGroupId, string TGuId, bool IsShowOnlyOpeningBalacesJas)
    {
        try
        {

            //SqlCommand cm = new SqlCommand("Accounts.FinancialReportsBalanceSheet", con);
            //cm.CommandTimeout = 0;
            //cm.CommandType = CommandType.StoredProcedure;
            //cm.Parameters.AddWithValue("@GroupId", GroupId);
            //cm.Parameters.AddWithValue("@InstId", InstId);
            //cm.Parameters.AddWithValue("@DepartmentId", DepartmentId);
            //cm.Parameters.AddWithValue("@FinancialYearId", FinancialYearId);
            //cm.Parameters.AddWithValue("@IsShowZeroBalance", IsShowZeroBalance);
            //cm.Parameters.AddWithValue("@ShowOnlyOpeningBalaces", ShowOnlyOpeningBalaces);
            //cm.Parameters.AddWithValue("@FromDate", FromDate);
            //cm.Parameters.AddWithValue("@ToDate", ToDate);
            //cm.Parameters.AddWithValue("@IsIncomeExpenditure", IsIncomeExpenditure);
            //cm.Parameters.AddWithValue("@ForInstId", ForInstId);
            //cm.Parameters.AddWithValue("@IsTrialBalanceSheet", IsTrialBalanceSheet);
            //cm.Parameters.AddWithValue("@Guid", TGuid);



            //SqlDataAdapter da = new SqlDataAdapter(cm);

            ////DataSet ds = new DataSet();
            //ds = new DataSet();
            //da.Fill(ds);
            //da.Dispose();
            //cm.Dispose();


            param = new SqlParameter[13];
            param[0] = new SqlParameter("@GroupId", GroupId);
            param[1] = new SqlParameter("@IsIncomeExpenditure", IsIncomeExpenditure);
            param[2] = new SqlParameter("@InstId", InstId);
            param[3] = new SqlParameter("@DepartmentId", DepartmentId);
            param[4] = new SqlParameter("@IsShowZeroBalance", IsShowZeroBalance);
            param[5] = new SqlParameter("@FinancialYearId", FinancialYearId);
            param[6] = new SqlParameter("@ShowOnlyOpeningBalaces", ShowOnlyOpeningBalaces);
            param[7] = new SqlParameter("@FromDate", FromDate);
            param[8] = new SqlParameter("@ToDate", ToDate);
            param[9] = new SqlParameter("@ForInstId", ForInstId);
            param[10] = new SqlParameter("@AccountGroupId", AccountGroupId);
            param[11] = new SqlParameter("@GuId", TGuId);
            param[12] = new SqlParameter("@IsShowOnlyOpeningBalacesJas", IsShowOnlyOpeningBalacesJas);
            ds = SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.CreateLedgerCommonIncomeExpendituteCollegeGroup", param);
            SKey = "";
        }
        catch (Exception ex)
        {
            SKey = ex.ToString();
        }
        return ds;
    }


    [WebMethod]
    public DataSet AccountGenreteDayBook_Select(string SKey, int GroupId, int LedgerId, int InstId, int DepartmentId
        , int FinancialYearId, DateTime FromDate, DateTime ToDate, int ForInstId, int AccountGroupId)
    {
        try
        {
            //param = new SqlParameter[9];
            //param[0] = new SqlParameter("@GroupId", GroupId);
            //param[1] = new SqlParameter("@DepartmentId", DepartmentId);
            //param[2] = new SqlParameter("@InstId", InstId);
            //param[3] = new SqlParameter("@LedgerId", LedgerId);
            //param[4] = new SqlParameter("@FinancialYearId", FinancialYearId);
            //param[5] = new SqlParameter("@FromDate", FromDate);
            //param[6] = new SqlParameter("@ToDate", ToDate);
            //param[7] = new SqlParameter("@ForInstId", ForInstId);
            //param[8] = new SqlParameter("@AccountGroupId", AccountGroupId);

            //ds = SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.AccountGenreteDayBook_Select", param);



            SqlCommand cm = new SqlCommand("Accounts.AccountGenreteDayBook_Select", con);
            cm.CommandTimeout = 600;
            cm.CommandType = CommandType.StoredProcedure;
            cm.Parameters.AddWithValue("@GroupId", GroupId);
            cm.Parameters.AddWithValue("@DepartmentId", DepartmentId);
            cm.Parameters.AddWithValue("@InstId", InstId);
            cm.Parameters.AddWithValue("@LedgerId", LedgerId);
            cm.Parameters.AddWithValue("@FinancialYearId", FinancialYearId);
            cm.Parameters.AddWithValue("@FromDate", FromDate);
            cm.Parameters.AddWithValue("@ToDate", ToDate);
            cm.Parameters.AddWithValue("@ForInstId", ForInstId);
            cm.Parameters.AddWithValue("@AccountGroupId", AccountGroupId);

            SqlDataAdapter da = new SqlDataAdapter(cm);
            ds = new DataSet();
            da.Fill(ds);
            da.Dispose();
            cm.Dispose();

            SKey = "";
        }
        catch (Exception ex)
        {
            SKey = ex.ToString();
        }
        return ds;
    }


    [WebMethod]
    public DataSet TransactionNotificationSelect(string SKey, int ToInstId, string SelectType, int IsCompTransaction, int FinancialYearId)
    {
        //try
        //{
        param = new SqlParameter[4];
        param[0] = new SqlParameter("@ToInstId", ToInstId);
        param[1] = new SqlParameter("@SelectType", SelectType);
        param[2] = new SqlParameter("@IsCompTransaction", IsCompTransaction);
        param[3] = new SqlParameter("@FinancialYearId", FinancialYearId);
        ds = SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.TransactionNotificationSelect", param);
        //    SKey = "";
        //}
        //catch (Exception ex)
        //{
        //    SKey = ex.ToString();
        //}
        return ds;
    }



    [WebMethod]
    public DataSet TransactionNotificationUpdate(string SKey, int UniqueId, int UserAccountId)
    {
        try
        {
            param = new SqlParameter[2];
            param[0] = new SqlParameter("@UniqueId", UniqueId);
            param[1] = new SqlParameter("@UserAccountId", UserAccountId);
            ds = SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.TransactionNotificationUpdate", param);
            SKey = "";
        }
        catch (Exception ex)
        {
            SKey = ex.ToString();
        }
        return ds;
    }

    [WebMethod]
    public DataSet InstDepartmentList(string SKey)
    {
        try
        {
            string mysql = "select Inst_Id,AccountInstTitle AS Inst_Title,AccountShortTitle AS Inst_ShortTitle from vInst_Info WHERE (Inst_Id<300001 OR Inst_Id=300010) ";
            ds = SqlHelper.ExecuteDataset(con, CommandType.Text, mysql, param);

            //UNION ALL SELECT 0,'All','All'

        }
        catch (Exception ex)
        {

        }
        return ds;
    }

    [WebMethod]
    public DataSet GetCollegeNameByForInstId(string SKey, int ForInstId)
    {
        try
        {
            param = new SqlParameter[1];
            param[0] = new SqlParameter("@ForInstId", ForInstId);

            string mysql = "select Inst_Id,Inst_Title,Inst_ShortTitle from vInst_Info WHERE Inst_Id=@ForInstId And (Inst_Id<300001 OR Inst_Id=300010) ";
            ds = SqlHelper.ExecuteDataset(con, CommandType.Text, mysql, param);

        }
        catch (Exception ex)
        {

        }
        return ds;
    }

    [WebMethod]
    public DataSet GetVersion(string SKey)
    {
        try
        {
            string mysql = "SELECT Version FROM Accounts.Version";
            ds = SqlHelper.ExecuteDataset(con, CommandType.Text, mysql);

        }
        catch (Exception ex)
        {

        }
        return ds;
    }

    [WebMethod]
    public DataSet GetCommonLedger(string SKey, int InstId)
    {
        try
        {
            param = new SqlParameter[1];
            param[0] = new SqlParameter("@InstId", InstId);
            ds = SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.CommonLedger_Select", param);

        }
        catch (Exception ex)
        {

        }
        return ds;
    }


    //[WebMethod]
    //public string ReportingIncomeExpenditure(string SKey, DataSet dataset, string ReportHeader, string ReportTitle)
    //{
    //    string FileName = "";
    //    try
    //    {

    //    }
    //    catch (Exception ex)
    //    {
    //        FileName = ex.Message;

    //    }
    //    return FileName;
    //}

    [WebMethod]
    public bool ChangePassword(string SKey, string UserName, string CurrentPassword, string NewPassword)
    {
        CurrentPassword = Encrypt(CurrentPassword);

        var qury = db.UserAccounts.FirstOrDefault(p => p.UA_AccountName == UserName && p.UA_MyAppPassword == CurrentPassword);


        if (qury != null)
        {
            qury.UA_MyAppPassword = Encrypt(NewPassword);
            db.SaveChanges();
            return true;
        }
        else
        {
            return false;
        }

    }

    [WebMethod]
    public DataSet GetPasswordByAccountName(string SKey, string UA_AccountName)
    {
        try
        {
            param = new SqlParameter[1];
            param[0] = new SqlParameter("@UA_AccountName", UA_AccountName);
            ds = SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.GetPassByUserAccount", param);

            ds.Tables[0].Rows[0]["Password"] = Decrypt(ds.Tables[0].Rows[0]["Password"].ToString());
            ds.Tables[0].AcceptChanges();
        }
        catch (Exception ex)
        {

        }
        return ds;
    }

    [WebMethod]
    public DataSet OpeningBalance(string SKey, int ForInstId, int FinancialYearId, int InstId, DateTime FromDate, int LedgerId, int Extra)
    {
        try
        {
            param = new SqlParameter[5];
            param[0] = new SqlParameter("@ForInstId", ForInstId);
            param[1] = new SqlParameter("@FinancialYearId", FinancialYearId);
            param[2] = new SqlParameter("@InstId", InstId);
            param[3] = new SqlParameter("@FromDate", FromDate);
            param[4] = new SqlParameter("@LedgerId", LedgerId);
            ds = SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.OpeningBalance", param);
        }
        catch (Exception ex)
        {

        }
        return ds;
    }


    [WebMethod]
    public DataSet ResolvedIssues(string SKey)
    {
        try
        {
            param = new SqlParameter[1];
            ds = SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.usp_IssuesResolved", param);
        }
        catch (Exception ex)
        {

        }
        return ds;
    }

    [WebMethod]
    public DataSet UpdateTransactionMasterByMasterID(string SKey, int InstId, DateTime FromDate, int VoucherTypeId, int FinancialYearId)
    {
        try
        {
            param = new SqlParameter[4];
            param[0] = new SqlParameter("@InstId", InstId);
            param[1] = new SqlParameter("@FromDate", FromDate);
            param[2] = new SqlParameter("@VoucherTypeId", VoucherTypeId);
            param[3] = new SqlParameter("@FinancialYearId", FinancialYearId);
            ds = SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.UpdateTransactionMasterByMasterID", param);
        }
        catch (Exception ex)
        {

        }
        return ds;
    }


    [WebMethod]
    public bool UpdateTransactionMasterWithTable(string SKey, int InstId, int VoucherTypeId, int FinancialYearId, DataTable dt)
    {
        try
        {
            param = new SqlParameter[4];
            param[0] = new SqlParameter("@InstId", InstId);
            param[1] = new SqlParameter("@dt", dt);
            param[2] = new SqlParameter("@VoucherTypeId", VoucherTypeId);
            param[3] = new SqlParameter("@FinancialYearId", FinancialYearId);
            return Convert.ToBoolean(SqlHelper.ExecuteScalar(con, CommandType.StoredProcedure, "Accounts.UpdateTransactionMasterWithTable", param));
        }
        catch (Exception ex)
        {

        }
        return false;
    }


    [WebMethod]
    public DataSet NotificationPendingLedger(string SKey, int InstId)
    {
        try
        {
            param = new SqlParameter[1];
            param[0] = new SqlParameter("@InstId", InstId);
            return SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.usp_NotificationPendingLedger", param);
        }
        catch (Exception ex)
        {

        }
        return ds;
    }


    [WebMethod]
    public DataSet NotificationConfirmedLedger(string SKey, int InstId)
    {
        try
        {
            param = new SqlParameter[1];
            param[0] = new SqlParameter("@InstId", InstId);
            return SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.usp_NotificationConfirmedLedger", param);
        }
        catch (Exception ex)
        {

        }
        return ds;
    }



    [WebMethod]
    public DataSet NotificationConfirmedUpdate(string SKey, int NotificationToInstId, int Id)
    {
        try
        {
            param = new SqlParameter[2];
            param[0] = new SqlParameter("@NotificationToInstId", NotificationToInstId);
            param[1] = new SqlParameter("@Id", Id);
            return SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.usp_NotificationConfirmedUpdate", param);
        }
        catch (Exception ex)
        {

        }
        return ds;
    }

    [WebMethod]
    public DataSet NotificationConfirmedDelete(string SKey, int Id)
    {
        try
        {
            param = new SqlParameter[1];
            param[0] = new SqlParameter("@Id", Id);
            return SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.usp_NotificationConfirmedDelete", param);
        }
        catch (Exception ex)
        {

        }
        return ds;
    }

    [WebMethod]
    public DataSet ReconciliationLedgerList(string SKey, int ToInstId)
    {
        try
        {
            param = new SqlParameter[1];
            param[0] = new SqlParameter("@ToInstId", ToInstId);
            return SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.usp_ReconciliationLedgerList", param);
        }
        catch (Exception ex)
        {

        }
        return ds;

    }


    [WebMethod]
    public DataSet ReconciliationLedgerConfirmedDelete(string SKey, int Id)
    {
        try
        {
            param = new SqlParameter[1];
            param[0] = new SqlParameter("@Id", Id);
            return SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.usp_ReconciliationLedgerConfirmedDelete", param);
        }
        catch (Exception ex)
        {

        }
        return ds;
    }

    [WebMethod]
    public DataSet ReconciliationLedgerConfirmedUpdate(string SKey, int NotificationToLedgerId, int Id)
    {
        try
        {
            param = new SqlParameter[2];
            param[0] = new SqlParameter("@NotificationToLedgerId", NotificationToLedgerId);
            param[1] = new SqlParameter("@Id", Id);
            return SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.usp_ReconciliationLedgerConfirmedUpdate", param);
        }
        catch (Exception ex)
        {

        }
        return ds;
    }

    [WebMethod]
    public DataSet ReconciliationSelectAllLedgers(string SKey, int InstId, int ToInstId, int FinancialYearId, DateTime ToDate)
    {
        try
        {
            param = new SqlParameter[4];
            param[0] = new SqlParameter("@InstId", InstId);
            param[1] = new SqlParameter("@ToInstId", ToInstId);
            param[2] = new SqlParameter("@FinancialYearId", FinancialYearId);
            param[3] = new SqlParameter("@ToDate", ToDate);
            return SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.usp_ReconciliationSelectAllLedgers", param);
        }
        catch (Exception ex)
        {

        }
        return ds;
    }

    [WebMethod]
    public DataSet ReconciliationLedgersByInstId(string SKey, int InstId, int LedgerId, int ToInstId, int ToLedgerId, int FinancialYearId,
        DateTime ToDate)
    {
        try
        {
            param = new SqlParameter[6];
            param[0] = new SqlParameter("@InstId", InstId);
            param[1] = new SqlParameter("@ToInstId", ToInstId);
            param[2] = new SqlParameter("@FinancialYearId", FinancialYearId);
            param[3] = new SqlParameter("@ToLedgerId", ToLedgerId);
            param[4] = new SqlParameter("@LedgerId", LedgerId);
            param[5] = new SqlParameter("@ToDate", ToDate);
            return SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.usp_ReconciliationLedgersByInstId", param);
        }
        catch (Exception ex)
        {

        }
        return ds;
    }


    [WebMethod]
    public DataSet OpeningTrialDetailed(string SKey, int InstId, int FinancialYearId, int ForInstId, int AccountGroupId, DateTime FromDate, DateTime ToDate)
    {
        try
        {
            param = new SqlParameter[6];
            param[0] = new SqlParameter("@InstId", InstId);
            param[1] = new SqlParameter("@ForInstId", ForInstId);
            param[2] = new SqlParameter("@FinancialYearId", FinancialYearId);
            param[3] = new SqlParameter("@AccountGroupId", AccountGroupId);
            param[4] = new SqlParameter("@FromDate", FromDate);
            param[5] = new SqlParameter("@ToDate", ToDate);
            return SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.OpeningTrialDetailed", param);
        }
        catch (Exception ex)
        {

        }
        return ds;
    }


    [WebMethod]
    public DataSet OpeningBalanceByAccountGroupId(string SKey, int InstId, int FinancialYearId,
        int ForInstId, int AccountGroupId, DateTime ToDate, bool IsIncludeChildAccountGroup, string MyGuid)
    {
        try
        {
            param = new SqlParameter[7];
            param[0] = new SqlParameter("@InstId", InstId);
            param[1] = new SqlParameter("@ForInstId", ForInstId);
            param[2] = new SqlParameter("@FinancialYearId", FinancialYearId);
            param[3] = new SqlParameter("@AccountGroupId", AccountGroupId);
            param[4] = new SqlParameter("@ToDate", ToDate);
            param[5] = new SqlParameter("@IsIncludeChildAccountGroup", IsIncludeChildAccountGroup);
            param[6] = new SqlParameter("@Guid", MyGuid);
            return SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.OpeningBalanceByAccountGroupId", param);
        }
        catch (Exception ex)
        {

        }
        return ds;
    }

    // Methods for KDL Lab Data Entry
    [WebMethod]
    public int KDL_FetchMaxId(string SKey, string val)
    {

        param = new SqlParameter[1];
        param[0] = new SqlParameter("@String", val);
        return Convert.ToInt32(SqlHelper.ExecuteScalar(con, CommandType.StoredProcedure, "KDL_FetchMaximumSrNo", param));
    }

    [WebMethod]
    public bool KDL_InsertContactInfo(string SKey, DataTable dt)
    {
        try
        {
            param = new SqlParameter[1];
            param[0] = new SqlParameter("@dt", dt);
            SqlHelper.ExecuteNonQuery(con, CommandType.StoredProcedure, "KDL_InsertContactInfo", param);
            return true;
        }
        catch
        { }
        return false;
    }


    [WebMethod]
    public bool KDL_InsertMakeBill(string SKey, DataTable dt)
    {
        try
        {
            param = new SqlParameter[1];
            param[0] = new SqlParameter("@dt", dt);
            SqlHelper.ExecuteNonQuery(con, CommandType.StoredProcedure, "KDL_InsertMakeBill", param);
            return true;
        }
        catch
        { }
        return false;
    }


    [WebMethod]
    public bool KDL_InsertBillTransactions(string SKey, DataTable dt)
    {
        try
        {
            param = new SqlParameter[1];
            param[0] = new SqlParameter("@dt", dt);
            SqlHelper.ExecuteNonQuery(con, CommandType.StoredProcedure, "KDL_InsertBillTransactions", param);
            return true;
        }
        catch
        { }
        return false;
    }

    [WebMethod]
    public bool BalanceTransferEndYear(string SKey, DateTime FromDate, DateTime ToDate, int FinancialYearId, int InstId)
    {
        try
        {
            param = new SqlParameter[4];
            param[0] = new SqlParameter("@FromDate", FromDate);
            param[1] = new SqlParameter("@ToDate", ToDate);
            param[2] = new SqlParameter("@FinancialYearId", FinancialYearId);
            param[3] = new SqlParameter("@InstId", InstId);
            SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.BalanceTransferEndYear", param);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }


    [WebMethod]
    public DataSet CreateLedgerVoucherBykeyword(string SKey, int InstId, int FinancialYearId,
       string SearchValue)
    {
        try
        {
            param = new SqlParameter[3];
            param[0] = new SqlParameter("@InstId", InstId);
            param[1] = new SqlParameter("@FinancialYearId", FinancialYearId);
            param[2] = new SqlParameter("@SearchValue", SearchValue);
            return SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.CreateLedgerVoucherBykeyword", param);
        }
        catch (Exception ex)
        {

        }
        return ds;
    }


    [WebMethod]
    public DataSet NegativeCashBalance(string SKey, int InstId, int FinancialYearId)
    {
        try
        {
            SqlCommand cm = new SqlCommand("Accounts.NegativeCashBalance", con);
            cm.CommandTimeout = 0;
            cm.CommandType = CommandType.StoredProcedure;
            cm.Parameters.AddWithValue("@InstId", InstId);
            cm.Parameters.AddWithValue("@FinancialYearId", FinancialYearId);

            SqlDataAdapter da = new SqlDataAdapter(cm);
            ds = new DataSet();
            da.Fill(ds);
            da.Dispose();
            cm.Dispose();

            //param = new SqlParameter[2];
            //param[0] = new SqlParameter("@InstId", InstId);
            //param[1] = new SqlParameter("@FinancialYearId", FinancialYearId);
            //return SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.NegativeCashBalance", param);
        }
        catch (Exception ex)
        {

        }
        return ds;
    }


    [WebMethod]
    public bool MoveTransactiontoOtherLedtger(string SKey, int InstId, int FinancialYearId,
      int FromLedgerId, int ToLedgerId)
    {
        try
        {
            param = new SqlParameter[4];
            param[0] = new SqlParameter("@InstId", InstId);
            param[1] = new SqlParameter("@FinancialYearId", FinancialYearId);
            param[2] = new SqlParameter("@FromLedgerId", FromLedgerId);
            param[3] = new SqlParameter("@ToLedgerId", ToLedgerId);
            SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.MoveTransactiontoOtherLedtger", param);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }


    [WebMethod]
    public bool LedgerTableConrent(string SKey, int InstId, int FinancialYearId,
     DataTable MyTb)
    {
        try
        {
            param = new SqlParameter[3];
            param[0] = new SqlParameter("@InstId", InstId);
            param[1] = new SqlParameter("@FinancialYearId", FinancialYearId);
            param[2] = new SqlParameter("@MyTb", MyTb);
            SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.LedgerTableConrent", param);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }




    [WebMethod]
    public DataSet LedgerTableConrentLoadData(string SKey, int InstId, int FinancialYearId)
    {
        try
        {
            param = new SqlParameter[2];
            param[0] = new SqlParameter("@InstId", InstId);
            param[1] = new SqlParameter("@FinancialYearId", FinancialYearId);
            return ds = SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.LedgerTableConrentLoadData", param);
        }
        catch (Exception ex)
        {
            return ds;
        }
    }

    [WebMethod]
    public DataSet TransactionPermissionSelect(string SKey, int FinancialYearId)
    {
        try
        {
            param = new SqlParameter[1];
            param[0] = new SqlParameter("@FinancialYearId", FinancialYearId);
            return ds = SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.TransactionPermissionSelect", param);
        }
        catch (Exception ex)
        {
            return ds;
        }
    }

    [WebMethod]
    public bool TransactionPermissionUpdate(string SKey, int FinancialYearId, int InstitutionId, bool IsTransactionAddAllow,
        bool IsTransactionEditAllow, bool IsOpeningBalanceEditAllow, bool IsNewLedgerAddAllow)
    {
        try
        {
            param = new SqlParameter[6];
            param[0] = new SqlParameter("@FinancialYearId", FinancialYearId);
            param[1] = new SqlParameter("@InstitutionId", InstitutionId);
            param[2] = new SqlParameter("@IsTransactionAddAllow", IsTransactionAddAllow);
            param[3] = new SqlParameter("@IsTransactionEditAllow", IsTransactionEditAllow);
            param[4] = new SqlParameter("@IsOpeningBalanceEditAllow", IsOpeningBalanceEditAllow);
            param[5] = new SqlParameter("@IsNewLedgerAddAllow", IsNewLedgerAddAllow);
            SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.TransactionPermissionUpdate", param);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    [WebMethod]
    public DataSet PrintTransactionVoucherSlip(string SKey, int TransactionMasterId)
    {
        try
        {
            param = new SqlParameter[1];
            param[0] = new SqlParameter("@TransactionMasterId", TransactionMasterId);
            return ds = SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.PrintTransactionVoucherSlip", param);
        }
        catch (Exception ex)
        {
            return ds;
        }
    }

    [WebMethod]
    public DateTime GetLastDateForDataEntry()
    {
        return DateTime.Now;
        //return DateTime.Now.AddDays(-4);
    }

    [WebMethod]
    public List<DateTime> AllowTransactionInTheseDates()
    {
        List<DateTime> list = new List<DateTime>();
        DateTime timeToUse = new DateTime(2018, 3, 31, 10, 15, 30);
        list.Add(timeToUse);
        return list;
    }

    /// <summary>
    /// Function to get AccountLedgerIndex 
    /// </summary>
    /// <returns></returns>
    [WebMethod]
    public DataSet AccountLedgerIndex(string SKey, string Jsondata, int InstId)
    {
        try
        {
            param = new SqlParameter[2];
            param[0] = new SqlParameter("@Jsondata", Jsondata);
            param[1] = new SqlParameter("@InstId", InstId);
            ds = SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.LedgerIndex", param);
            SKey = "";
        }
        catch (Exception ex)
        {
            SKey = ex.ToString();
        }
        return ds;
    }

    [WebMethod]
    // Added for backup changes
    public int TransactionDetailsBackup(string SKey, string Jsondata)
    {
        int count = 0;
        try
        {
            param = new SqlParameter[2];
            param[0] = new SqlParameter("@Jsondata", Jsondata);
            param[1] = new SqlParameter("@RowCount", SqlDbType.Int);
            param[1].Direction = ParameterDirection.Output;
            ds = SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Accounts.TransactionDetailsBackupData", param);
            count = Convert.ToInt32(param[1].Value);
            SKey = "";
        }
        catch (Exception ex)
        {
            SKey = ex.ToString();
        }
        return count;
    }

    [WebMethod]
    public DataSet AccountGenreteLedgerTranscationListAllIndex(string SKey, DateTime FromDate, DateTime ToDate, int LedgerId, int InstId, int DepartmentId, int FinancialYearId, int ForInstId, int AccountGroupId)
    {
        try
        {
            SqlCommand cm = new SqlCommand("Accounts.AccountGenreteLedgerTranscationListAllIndex", con);
            //cm.CommandTimeout = 0;
            cm.CommandType = CommandType.StoredProcedure;
            cm.Parameters.AddWithValue("@InstId", InstId);
            //cm.Parameters.AddWithValue("@DepartmentId", DepartmentId);
            //cm.Parameters.AddWithValue("@FinancialYearId", FinancialYearId);
            //cm.Parameters.AddWithValue("@FromDate", FromDate);
            //cm.Parameters.AddWithValue("@ToDate", ToDate);
            //cm.Parameters.AddWithValue("@ForInstId", ForInstId);
            //cm.Parameters.AddWithValue("@LedgerId", LedgerId);
            //cm.Parameters.AddWithValue("@AccountGroupId", AccountGroupId);



            SqlDataAdapter da = new SqlDataAdapter(cm);

            //DataSet ds = new DataSet();
            ds = new DataSet();
            da.Fill(ds);
            da.Dispose();
            cm.Dispose();


        }

        catch (Exception ex)
        {
            SKey = ex.ToString();
        }
        return ds;
    }
}

    