using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Training_Quest3.Model
{

    [Table("UserRecord")]
    internal class UserInfoModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string BD_month { get; set; }
        public string BD_day { get; set; }
        public string BD_year { get; set; }
        public string Address { get; set; }
        public string FavAnimal { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        [NotMapped]
        public string FullName { get { return $"{FirstName} {MiddleName} {LastName}".Trim(); } }
    }
}
