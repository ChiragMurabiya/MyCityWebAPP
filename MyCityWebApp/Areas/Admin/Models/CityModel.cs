using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyCityWebApp.Areas.Admin.Models
{
    public class CityModel
    {
        public int ID { get; set; }
        public int stateID { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "City Name", Prompt = "City Name")]
        public string Name { get; set; }
        public Nullable<bool> Active { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> Updated { get; set; }
        public Nullable<int> UpdatedBy { get; set; }

        [NotMapped]
        public SelectList stateList { get; set; }
    }
}