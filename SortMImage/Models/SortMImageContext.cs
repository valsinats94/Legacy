using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortMImage.Models
{
    public class SortMImageContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<ImageModel> Images { get; set; }
    }
}
