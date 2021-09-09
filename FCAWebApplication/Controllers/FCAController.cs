using FCAWebApplication.Models;
//using FCAWebApplication.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace FCAWebApplication.Controllers
{
    public class FCAController : Controller
    {
        static string WorkingFileName;
        static string wfServerPath;
        static Boolean uploadedFile = false;
        // GET: FCA
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public String GetFile()
        {
            var webClient = new WebClient();
            var path = wfServerPath + WorkingFileName + ".json";
            if (path != ".json")
            {
                var json = webClient.DownloadString(path);
                return json;
            }
            return null;
            
        }

        [HttpGet]
        public Boolean ShowFile()
        {
            //Console.WriteLine(uploadedFile);
            return uploadedFile;

        }

        [HttpGet]
        public ActionResult UploadFile()
        {
            //return View();
            return View();
        }

        //[HttpGet]
        //public String Logger()
        //{
        //    return //MyLogger.GetLogs();
        //}

        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase file)
        {
            
            try
            {
                if (file == null || file.ContentLength <= 0)
                {
                    //MyLogger.Record( "File is null or empty");
                    ViewBag.Result = "File upload failed!!";
                    uploadedFile = false;
                    return View("Index");
                }
                else
                {

                    //MyLogger.Record(  "File NOT null or empty...");
                    var fileName = file.FileName.ToLower();
                    //MyLogger.Record(  "Filename  = ..." + fileName);
                    if (!fileName.EndsWith(".csv") && !(fileName.EndsWith(".cgif")) && !(fileName.EndsWith(".cxt")))
                    {

                        ViewBag.Result = "Invalid file type.";
                        uploadedFile = false;
                        return View("Index");
                    }


                    //MyLogger.Record(  "attempt to fetching directory : " );
                    DirectoryInfo di = new DirectoryInfo(Path.Combine(Server.MapPath("~/Uploads")));

                    //MyLogger.Record(  "fetching directory : " + di.Name);

                    //MyLogger.Record(  "deleting files");
                    foreach (FileInfo files in di.GetFiles())
                    {
                        files.Delete();
                    }
                    foreach (DirectoryInfo dir in di.GetDirectories())
                    {
                        dir.Delete(true);
                    }
                    //MyLogger.Record(  "files deleted");
                    string _FileName = Path.GetFileName(file.FileName);
                    Random rnd = new Random();
                    ////_FileName = _FileName + rnd.Next(1000, 9999).ToString();
                    string wfName = Path.GetFileNameWithoutExtension(file.FileName);
                    wfName += rnd.Next(100000, 999999).ToString();
                    WorkingFileName = wfName;

                    string wfExtension = Path.GetExtension(file.FileName);

                    string _path = Path.Combine(Server.MapPath("~/Uploads"), wfName + wfExtension);
                    //MyLogger.Record("path : " + _path);
                    file.SaveAs(_path);
                    //MyLogger.Record("saved");
                    ViewBag.Result = "File Upload Succeed!";

                    wfServerPath = Server.MapPath("~/Uploads/");
                    
                    string removeQuotes;
                    string strCmdText;

                    removeQuotes = "-windowstyle hidden cd " + wfServerPath + "; (Get-Content " + wfName + wfExtension + ").Replace([char]34, ' ') | Set-Content " + wfName + wfExtension;
                    var proccess = System.Diagnostics.Process.Start("powershell.exe", removeQuotes);
                    proccess.WaitForExit();

                    if (String.Equals(wfExtension, ".csv", StringComparison.OrdinalIgnoreCase) || String.Equals(wfExtension, ".cgif", StringComparison.OrdinalIgnoreCase))
                    {
                        
                        //-noexit
                        strCmdText = "-windowstyle hidden cd " + wfServerPath + "; ../Content/Executables/CG-FCA-v7.exe " + wfName + wfExtension;
                        Console.WriteLine(strCmdText);

                        var proccessCXT = System.Diagnostics.Process.Start("powershell.exe", strCmdText);
                        proccessCXT.WaitForExit();
                    }


                    strCmdText = "-windowstyle hidden cd " + wfServerPath + "; ../Content/Executables/In-Close4_oneLinerEdition.exe " + wfName + ".cxt; mv concepts.json " + wfName + ".json";
                    var proccessJson = System.Diagnostics.Process.Start("powershell.exe", strCmdText);
                    proccessJson.WaitForExit();
                    uploadedFile = true;
                    ViewBag.Message = true;
                }
                return View("Index");
            }
            catch(Exception e)
            {

                //MyLogger.Record(  "exception + "  + e.Message);
                //MyLogger.Record(  "exception + " + e.StackTrace.ToString());
                ViewBag.Result = "File upload failed!!" + e.Message;
                uploadedFile = false;
                return View("Index");

            }
        }

        public ActionResult ExportFile(string file)
        {


            try
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(wfServerPath + WorkingFileName + file);
                string fileName = WorkingFileName + file;
                ViewBag.Result = "File export succeed!";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            catch
            {
                ViewBag.Message = "File export failed!";
                return View("Index");
            }
        }

        public ActionResult GeneratedFiles()
        {
            //Fetch all files in the Folder (Directory).
            string[] filePaths = Directory.GetFiles(Server.MapPath("~/Uploads/"));

            //Copy File names to Model collection.
            List<FileModel> files = new List<FileModel>();
            foreach (string filePath in filePaths)
            {
                files.Add(new FileModel { FileName = Path.GetFileName(filePath) });
            }

            return View(files);
        }

        public FileResult DownloadFile(string fileName)
        {
            //Build the File Path.
            string path = Server.MapPath("~/Uploads/") + fileName;

            //Read the File data into Byte Array.
            byte[] bytes = System.IO.File.ReadAllBytes(path);

            //Send the File to Download.
            return File(bytes, "application/octet-stream", fileName);
        }

        [HttpGet]
        public ActionResult PreviewFile(string id)
        {
            string withQuotes = "\"" + id + "\"";
            string filePath = Server.MapPath("~/Uploads/") + id;
            string texts = System.IO.File.ReadAllText(filePath);
            ViewBag.Data = texts;
            //ViewBag.Data = MvcHtmlString.Create(texts);
            //return View();
            return PartialView("_previewFile");
        }



    }
}