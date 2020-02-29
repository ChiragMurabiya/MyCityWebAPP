using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace MyCityWebApp.Areas.Admin.Models
{
    public class BannerModel
    {
        public int ID { get; set; }        
        public string ImageName { get; set; }
        public string ImagePath { get; set; }
        public Nullable<bool> Active { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> Updated { get; set; }
        public Nullable<int> UpdatedBy { get; set; }

        [ReadOnly(true)]
        [DisplayName("Upload Banner")]
        public string UploadBanner { get; set; }

        [ReadOnly(true)]
        [DisplayName("Image")]
        public string Image { get; set; }
    }
}