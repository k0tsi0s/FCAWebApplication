using FCAWebApplication.Models;
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
            return uploadedFile;

        }

        [HttpGet]
        public ActionResult UploadFile()
        {
            //return View();
            return View("Index");
        }
        
        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase file)
        {
            try
            {
                if (file == null || file.ContentLength <= 0)
                {
                    ViewBag.Result = "File upload failed!!";
                    uploadedFile = false;
                    return View("Index");
                }
                else
                {

                    var fileName = file.FileName.ToLower();
                    if (!fileName.EndsWith(".csv") && !(fileName.EndsWith(".cgif")) && !(fileName.EndsWith(".cxt")))
                    {

                        ViewBag.Result = "Invalid file type.";
                        uploadedFile = false;
                        return View("Index");
                    }
                    DirectoryInfo di = new DirectoryInfo(Path.Combine(Server.MapPath("~/Uploads")));

                    foreach (FileInfo files in di.GetFiles())
                    {
                        files.Delete();
                    }
                    foreach (DirectoryInfo dir in di.GetDirectories())
                    {
                        dir.Delete(true);
                    }

                    string _FileName = Path.GetFileName(file.FileName);
                    Random rnd = new Random();
                    ////_FileName = _FileName + rnd.Next(1000, 9999).ToString();
                    string wfName = Path.GetFileNameWithoutExtension(file.FileName);
                    wfName += rnd.Next(100000, 999999).ToString();
                    WorkingFileName = wfName;

                    string wfExtension = Path.GetExtension(file.FileName);

                    string _path = Path.Combine(Server.MapPath("~/Uploads"), wfName + wfExtension);

                    file.SaveAs(_path);

                    ViewBag.Result = "File Upload Succeed!";

                    wfServerPath = Server.MapPath("~/Uploads/");

                    string strCmdText;
                    if (String.Equals(wfExtension, ".csv", StringComparison.OrdinalIgnoreCase) || String.Equals(wfExtension, ".cgif", StringComparison.OrdinalIgnoreCase))
                    {

                        strCmdText = "-windowstyle hidden cd " + wfServerPath + "; ../Content/Executables/CG-FCA-v7.exe " + wfName + wfExtension;

                        var proccess = System.Diagnostics.Process.Start("powershell.exe", strCmdText);
                        proccess.WaitForExit();

                    }


                    strCmdText = "-windowstyle hidden cd " + wfServerPath + "; ../Content/Executables/In-Close4_oneLinerEdition.exe " + wfName + ".cxt; mv concepts.json " + wfName + ".json";
                    var proccess2 = System.Diagnostics.Process.Start("powershell.exe", strCmdText);
                    proccess2.WaitForExit();
                    uploadedFile = true;
                    ViewBag.Message = true;
                }
                return View("Index");
            }
            catch
            {
                ViewBag.Result = "File upload failed!!";
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

        public ActionResult PreviewFile(string fileName)
        {
            string filePath = Server.MapPath("~/Uploads/") + fileName;
            string[] texts = System.IO.File.ReadAllLines(filePath);
            ViewBag.Data = texts;
            return View();
        }
    }
}