using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SortMImage.Models;

namespace SortMImage.Services.ExportServices
{
    public abstract class ExportService
    {
        public abstract void Export(IEnumerable<ImageModel> images, string imagesCollectionName);
    }
}
