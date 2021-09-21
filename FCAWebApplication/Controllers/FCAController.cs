using FCAWebApplication.Models;
using FCAWebApplication.Services;
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
            return Redirect("FCA/Tool");
        }
        public ActionResult Tool()
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

        //[HttpGet]
        //public String Logger()
        //{
        //    return //MyLogger.GetLogs();
        //}

        [HttpGet]
        public ActionResult UploadFile()
        {
            //return View();
            return Redirect("Tool");
        }

        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase file)
        {
            
            try
            {
                if (file == null || file.ContentLength <= 0)
                {
                    ////MyLogger.Record( "File is null or empty");
                    ViewBag.Result = "File upload failed!!";
                    uploadedFile = false;
                    return Redirect("Tool");
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
                        return Redirect("Tool");
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
                    
                    //string removeQuotes;
                    //string strCmdText;


                    //****************************************************************
                    
                    //Change server path
                    string changePath = "cd " + wfServerPath + ";";
                    //Close remain powerShells
                    string closePSs = "Stop-Process -Name 'powershell.exe';";
                    //Remove quotes and replace the file
                    string removeQuotes = "(Get-Content " + wfName + wfExtension + ").Replace([char]34, ' ') | Set-Content " + wfName + wfExtension + ";";
                    //Execute CGFCA.exe with given file
                    string executeCGFCA = "../Content/Executables/CG-FCA-v7.exe " + wfName + wfExtension + ";";
                    //Execute In-Close4.exe with given file
                    string executeInClose = "../Content/Executables/In-Close4_oneLinerEdition.exe " + wfName + ".cxt" + ";";
                    //Rename file concepts.json to fileName.json
                    string renameJsonFile = "mv concepts.json " + wfName + ".json" + ";";

                    //close remain powershells
                    System.Diagnostics.Process.Start("powershell.exe", changePath + closePSs);

                    //change directory and remove quotes
                    var processCPRQ = System.Diagnostics.Process.Start("powershell.exe", changePath + removeQuotes);
                    //MyLogger.Record("Command for change directory and remover quote: " + changePath + removeQuotes);
                    processCPRQ.WaitForExit();

                    if (String.Equals(wfExtension, ".csv", StringComparison.OrdinalIgnoreCase) || String.Equals(wfExtension, ".cgif", StringComparison.OrdinalIgnoreCase))
                    {
                        //execute CCFCA with given file
                        var processCGFCA = System.Diagnostics.Process.Start("powershell.exe", changePath + executeCGFCA);
                        //MyLogger.Record("Command for execute CGFCA: " + changePath + executeCGFCA);
                        processCGFCA.WaitForExit();

                        //execute In-Close with given file
                        var processInClose = System.Diagnostics.Process.Start("powershell.exe", changePath + executeInClose);
                        //MyLogger.Record("Command for execute InClose: " + changePath + executeInClose);
                        processInClose.WaitForExit();

                        //rename generated json file to fileName.json
                        var processRename = System.Diagnostics.Process.Start("powershell.exe", changePath + renameJsonFile);
                        //MyLogger.Record("Command for renaming json file: " + changePath + renameJsonFile);
                        processRename.WaitForExit();

                    }
                    else
                    {
                        //execute In-Close with given file
                        var processInClose = System.Diagnostics.Process.Start("powershell.exe", changePath + executeInClose);
                        //MyLogger.Record("Command for execute InClose: " + changePath + executeInClose);
                        processInClose.WaitForExit();

                        //rename generated json file to fileName.json
                        var processRename = System.Diagnostics.Process.Start("powershell.exe", changePath + renameJsonFile);
                        //MyLogger.Record("Command for renaming json file: " + changePath + renameJsonFile);
                        processRename.WaitForExit();

                    }
                   
                    uploadedFile = true;
                    ViewBag.Message = true;
                }
                return Redirect("Tool");
            }
            catch(Exception e)
            {

                //MyLogger.Record(  "exception + "  + e.Message);
                //MyLogger.Record(  "exception + " + e.StackTrace.ToString());
                ViewBag.Result = "File upload failed!!" + e.Message;
                uploadedFile = false;
                return Redirect("Tool");

            }
        }

        //public ActionResult ExportFile(string file)
        //{
        //    try
        //    {
        //        byte[] fileBytes = System.IO.File.ReadAllBytes(wfServerPath + WorkingFileName + file);
        //        string fileName = WorkingFileName + file;
        //        ViewBag.Result = "File export succeed!";
        //        return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        //    }
        //    catch
        //    {
        //        ViewBag.Message = "File export failed!";
        //        return Redirect("Tool");
        //    }
        //}

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
        public FileResult DownloadExampleFile(string fileName)
        {
            //Build the File Path.
            string path = Server.MapPath("~/Content/Examples/") + fileName;

            //Read the File data into Byte Array.
            byte[] bytes = System.IO.File.ReadAllBytes(path);

            //Send the File to Download.
            return File(bytes, "application/octet-stream", fileName);
        }

        public FileResult DownloadReadMeFile(string fileName)
        {
            //Build the File Path.
            string path = Server.MapPath("~/Content/ReadMe/") + fileName;

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