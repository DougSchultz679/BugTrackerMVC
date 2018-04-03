using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mime;
using System.IO;

namespace BugTracker.Controllers
{
    public class DownloadController:Controller
    {
        [Authorize]
        public FileResult Download(string filePath)
        {
            return File(filePath, MediaTypeNames.Application.Octet, Path.GetFileName(filePath));
        }
    }
}