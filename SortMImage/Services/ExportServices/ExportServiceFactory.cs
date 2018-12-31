using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SortMImage.Enums.Enums;

namespace SortMImage.Services.ExportServices
{
    public class ExportServiceFactory
    {
        public static ExportService GetExportService(ExportTypes exportType)
        {
            switch (exportType)
            {
                case ExportTypes.ExportToZip:
                    return new ExportToZipService();
                default:
                    return null;
            }
        }
    }
}
