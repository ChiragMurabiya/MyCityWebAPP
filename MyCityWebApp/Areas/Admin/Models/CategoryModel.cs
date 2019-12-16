using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyCityWebApp.Areas.Admin.Models
{
    public class CategoryModel
    {
        public int ID { get; set; }
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Category Name", Prompt = "Category Name")]
        public string Name { get; set; }
        public Nullable<bool> Active { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> Updated { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
    }
}