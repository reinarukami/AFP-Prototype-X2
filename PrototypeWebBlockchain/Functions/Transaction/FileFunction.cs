using System.Collections.Generic;
using System.Linq;
using Dapper;
using System.Data;
using Npgsql;
using PrototypeWebBlockchain.Models;
using System.IO;
using System.Security.Cryptography;
using System;
using System.Web;
using System.Configuration;
using Nethereum.Contracts;
using Nethereum.Web3.Accounts.Managed;
using Nethereum.Web3;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.ABI.Decoders;
using PrototypeWebBlockchain.Functions.Default;
using PrototypeWebBlockchain.Functions.Services;

namespace PrototypeWebBlockchain.Repository
{
    public class FileFunction : DefaultWeb3
    {

        Contract InitContract()
        {
            
            var web3 = InitializeWeb3(HttpContext.Current.Session["Address"].ToString(), HttpContext.Current.Session["Password"].ToString());

            string abi = ConfigurationManager.AppSettings["ContractABI"];

            var FileContract = web3.Eth.GetContract(abi, ConfigurationManager.AppSettings["ContractAddress"]);

            web3.Eth.TransactionManager.DefaultGas = 999999;

            return FileContract;

        }

        string ConvertSavedFileToSha(string fileSavePath)
        {

            using (SHA256 sha256 = SHA256.Create())
            {
                using (FileStream input = File.Open(fileSavePath, FileMode.Open))
                {
                    var shaValue = BitConverter.ToString(sha256.ComputeHash(input)).Replace("-", "");

                    return shaValue;
                }
            }
        }

        int CountFiles(Contract _filecontract)
        {
            var TaskCountFile = _filecontract.GetFunction("GetCount").CallDeserializingToObjectAsync<FileCountDTO>(HttpContext.Current.Session["Address"].ToString(), null, null);
            TaskCountFile.Wait();

            return TaskCountFile.Result.count;
        }

        TransactionJson SetTransaction(int _id, string _filename, string _status, string _date, string _backuphash)
        {
            TransactionJson transaction = new TransactionJson();
            transaction.id = _id;
            transaction.filename = _filename;
            transaction.status = _status;
            transaction.date = _date;
            transaction.backuphash = _backuphash;
            return transaction;
        }

        public void UploadFile(FileClass _file, TransactionRepository _transactionRepository, string _memberID)
        {
            var FileContract = InitContract();

            // Validate the uploaded image(optional)
            int imageId = _transactionRepository.NextID().FirstOrDefault();
            string imagepath = ConfigurationManager.AppSettings["FileImagePath"];
            string imageName = imageId.ToString() + '_' + _memberID + '_' + _file.image.FileName.ToString();

            // Get the complete file path
            string fileSavePath = Path.Combine(imagepath + imageName);

            _file.image.SaveAs(fileSavePath);

            //BackUp File Google Drive
            string GoogleItemID = GoogleDriveFilesRepository.UploadImage(fileSavePath, imageName, _file.image.ContentType);

            //Blockchain
            var TaskAddFile = FileContract.GetFunction("AddFiles").SendTransactionAsync(HttpContext.Current.Session["Address"].ToString(), imageId, ConvertSavedFileToSha(fileSavePath), GoogleItemID, DateTime.Now.ToString());
            TaskAddFile.Wait();

            //Database
            var transaction = new TransactionData()
            {
                id = imageId,
                member_id = Int32.Parse(_memberID.ToString()),
                filename = imageName,
                filepath = imagepath,
                date = DateTime.Now.ToString()
            };
            
            _transactionRepository.Add(transaction);

        }

        public List<FileDTO> GetFiles()
        {
            var FileContract = InitContract();
            var count = CountFiles(FileContract);
            var result = new List<FileDTO>();

            for (int i = 1; i <= count; i++)
            {
                var TaskGetFile = FileContract.GetFunction("GetFiles").CallDeserializingToObjectAsync<FileDTO>(HttpContext.Current.Session["Address"].ToString(),null,null,i);
                TaskGetFile.Wait();
                result.Add(TaskGetFile.Result);
            }

            return result;
        }

        public List<TransactionJson> GetValidatedFiles(TransactionRepository _transactionRepository)
        {
            var blockresult = GetFiles();
            var dataresult = _transactionRepository.FindByID(Int32.Parse(HttpContext.Current.Session["ID"].ToString()));
            var transaction = new List<TransactionJson>();
            var filepath = ConfigurationManager.AppSettings["FileImagePath"];


            foreach (var item in blockresult)
            {
                var filedata = dataresult.Where(r => r.id == item.id).FirstOrDefault();

                if(filedata == null)
                {
                    transaction.Add(SetTransaction(item.id, item.filehash + "(File was Deleted or Updated in the database)", "Status/cross.png", item.date, item.backuphash));
                }

                else
                { 
                    try
                    {
                        string file = Path.Combine(filepath, filedata.filename);
                        string shavalue = ConvertSavedFileToSha(file);
                        if(shavalue == item.filehash)
                        {
                            transaction.Add(SetTransaction(item.id, filedata.filename, "Status/check.png", item.date, item.backuphash));
                        }
                        else
                        {
                            transaction.Add(SetTransaction(item.id, filedata.filename + "(File was Tampered)", "Status/cross.png", item.date, item.backuphash));
                        }
                    }
                    catch
                    {
                        transaction.Add(SetTransaction(item.id, item.filehash + "(File was Deleted )", "Status/cross.png", item.date, item.backuphash));
                    }
                }
            }

            return transaction;
        }

    }
}