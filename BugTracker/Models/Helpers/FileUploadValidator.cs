using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Web;

namespace BugTracker.Models.Helpers
{
    public static class FileUploadValidator
    {
        public static bool IsAllowedFileType(this HttpPostedFileBase file)
        {
            if (file.HasFile() == false) return false;

            string mimeType = file.ContentType;
            if (mimeType.In("text/plain", "application/x-msaccess", "application/vnd.ms-excel", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "application/onenote", "application/vnd.ms-powerpoint", "application/vnd.openxmlformats-officedocument.presentationml.presentation", "application/msword", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "application/vnd.visio", "application/pdf", "application/vnd.visio2013","image/bmp", "image/gif", "image/jpeg", "image/png", "application/zip")) return true;
            else return false;
        }

        public static bool HasFile(this HttpPostedFileBase file)
        {
            return (file != null && file.ContentLength > 0) ? true : false;
        }

        public static bool IsWebFriendlyImage(HttpPostedFileBase file)
        {
            if (file.HasFile() == false)
                return false;

            if (file.ContentLength > 2 * 1024 * 1024 || file.ContentLength < 1024)
                return false;
            try
            {
                using (var img = Image.FromStream(file.InputStream))
                {
                    return ImageFormat.Jpeg.Equals(img.RawFormat) ||
                        ImageFormat.Png.Equals(img.RawFormat) ||
                        ImageFormat.Gif.Equals(img.RawFormat);
                }
            } catch { return false; }
        }
    }

}