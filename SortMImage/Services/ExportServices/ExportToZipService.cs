using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;
using SortMImage.Models;

namespace SortMImage.Services.ExportServices
{
    public class ExportToZipService : ExportService
    {
        public override void Export(IEnumerable<ImageModel> images, string imagesCollectionName)
        {
            StringBuilder saveDirectory = new StringBuilder();
            saveDirectory.Append(Path.GetPathRoot(Environment.SystemDirectory));
            saveDirectory.Append(@"SortedImages\");
            saveDirectory.Append(@"ZippedSortedImages\");
            if (!Directory.Exists(saveDirectory.ToString()))
                Directory.CreateDirectory(saveDirectory.ToString());

            string zipName = saveDirectory.ToString() + imagesCollectionName + ".zip";

            ZipFile zipFile;
            if (!File.Exists(zipName))
                zipFile = ZipFile.Create(zipName);
            else
                zipFile = new ZipFile(zipName);

            zipFile.BeginUpdate();
            foreach (ImageModel image in images)
            {
                zipFile.Add(image.ImagePath, image.Name);
            }

            zipFile.CommitUpdate();
            zipFile.Close();
        }
    }
}
