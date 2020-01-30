using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyCityWebApp.Areas.Admin.Models
{
    public class ShopProductAndProductImageModel
    {
        public int ID { get; set; }
        public Nullable<int> ShopID { get; set; }
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
    }
}