using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Training_Quest3.Model;

namespace Training_Quest3.DataBase
{
    class DBContext : DbContext
    {
        public DBContext() : base("DbConnection")
        {}

        public DbSet<UserInfoModel> UserInfoRecord { get; set; }
    }
}
