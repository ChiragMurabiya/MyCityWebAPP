using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyCityWebApp.Areas.Admin.Models
{
    public class ShopModel
    {
        public int ID { get; set; }
        public Nullable<int> UserID { get; set; }
        public Nullable<int> CategoryID { get; set; }
        public Nullable<int> CityID { get; set; }
        public Nullable<int> StateID { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Shop Name", Prompt = "Shop Name")]
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public Nullable<bool> Active { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> Updated { get; set; }
        public Nullable<int> UpdatedBy { get; set; }

        [NotMapped]
        public SelectList stateList { get; set; }
        [NotMapped]
        public SelectList CategoryList { get; set; }
    }

    public class ShopImageModel
    {
        public int ID { get; set; }
        public Nullable<int> ShopID { get; set; }
        public string ImageName { get; set; }
        public string ImagePath { get; set; }
        public Nullable<bool> Active { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> Updated { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
    }
}