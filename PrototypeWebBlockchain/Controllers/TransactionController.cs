using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Npgsql;
using PrototypeWebBlockchain.Repository;
using PrototypeWebBlockchain.Models;
using System.IO;
using System.Security.Cryptography;
using System.Configuration;
using Newtonsoft.Json;
using PrototypeWebBlockchain.Functions.Filters;
using System.Diagnostics;
using PrototypeWebBlockchain.Functions.Services;

namespace PrototypeWebBlockchain.Controllers
{
    [AuthorizeUser]
    public class TransactionController : Controller
    {
        private readonly TransactionRepository transactionRepository;
        private readonly FileFunction fileupload;
           
        public TransactionController()
        {
            transactionRepository = new TransactionRepository();
            fileupload = new FileFunction();
        }

        public ActionResult Transactionlist()
        {

            return View();
        }

        public ActionResult UploadFile()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UploadFile(FileClass _file)
        {
            if (HttpContext.Request.Files[0].ContentLength == 0)
            {
                ModelState.AddModelError("file", "File is null , Please set Image");
                return View();
            }

            _file.image = HttpContext.Request.Files[0];

            if (_file.image != null)
            {
                fileupload.UploadFile(_file, transactionRepository, Session["ID"].ToString());
            }

            return RedirectToRoute(new { Controller = "Transaction", Action = "Transactionlist" });
        }

        [HttpPost]
        public JsonResult ValidateImages(string data)
        {
            var transaction = fileupload.GetValidatedFiles(transactionRepository);

            return new JsonResult() { Data = new { JTransaction = transaction } };
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Transactionlist(string id)
        {
            GoogleDriveFilesRepository.RestoreFile(id);
            return RedirectToRoute(new { Controller = "Transaction", Action = "Transactionlist" });
        } 

    }
}