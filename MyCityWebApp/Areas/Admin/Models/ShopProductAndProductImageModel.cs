using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyCityWebApp.Areas.Admin.Models
{
    public class ShopProductAndProductImageModel
    {
        public int ID { get; set; }
        public Nullable<int> ShopID { get; set; }
        public Nullable<int> ShopProductID { get; set; }
        public string Name { get; set; }
        public Nullable<bool> Active { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> Updated { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public string Description { get; set; }
        public Nullable<decimal> Discount { get; set; }
        public Nullable<decimal> Price { get; set; }
        public bool IsDisplayOnHomePage { get; set; }
        public Nullable<int> DisplayOrder { get; set; }
        public string ImageName { get; set; }
        public string ImagePath { get; set; }

        [ReadOnly(true)]
        [DisplayName("Add Product Images")]
        public string AddProductImages { get; set; }

        [ReadOnly(true)]
        [DisplayName("Upload Images")]
        public string UploadImage { get; set; }

        [NotMapped]
        public List<Images> ImageList { get; set; }
    }

    public class Images
    {
        public string Name { get; set; }
    }
}