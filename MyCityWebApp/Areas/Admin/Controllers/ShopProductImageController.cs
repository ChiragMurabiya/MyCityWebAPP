using MyCityWebApp.Areas.Admin.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Web.Script.Serialization;

namespace MyCityWebApp.Areas.Admin.Controllers
{
    public class ShopProductImageController : Controller
    {
        string WebAPIUrl = ConfigurationManager.AppSettings["MyCityWebAPIUrl"];
        HttpClient client = new HttpClient();
        HttpResponseMessage response = new HttpResponseMessage();

        // GET: Admin/ShopProductImage
        public ActionResult Index(int ShopProductID)
        {
            ViewBag.ShopProductID = ShopProductID;

            return View();
        }

        public ActionResult Create(int ShopProductID)
        {
            ShopProductAndProductImageModel model = new ShopProductAndProductImageModel();

            List<ShopProductAndProductImageModel> ShopProductInfo = new List<ShopProductAndProductImageModel>();
            List<Images> imgList = new List<Images>();
            var readTask = "";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(WebAPIUrl);
                var responseTask = client.GetAsync("/api/GetShopProductImageByShopProductID?ShopProductID=" + ShopProductID);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    readTask = result.Content.ReadAsStringAsync().Result;
                    ShopProductInfo = JsonConvert.DeserializeObject<List<ShopProductAndProductImageModel>>(readTask);
                }

                foreach (var images in ShopProductInfo)
                {
                    Images image = new Images();
                    image.Name = images.ImageName;
                    imgList.Add(image);
                }
                model.ImageList = imgList;
            }
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ShopProductAndProductImageModel model, List<HttpPostedFileBase> postedFiles)
        {
            List<Images> imgList = new List<Images>();

            foreach (HttpPostedFileBase postedFile in postedFiles)
            {
                Images image = new Images();
                if (postedFile != null)
                {
                    //Extract Image File Name.
                    string ImageName = System.IO.Path.GetFileName(postedFile.FileName);

                    //Set the Image File Path.
                    string ImagePath = "~/Areas/ProductImages/" + ImageName;

                    model.ShopProductID = model.ShopProductID;
                    model.ImageName = ImageName;
                    model.ImagePath = ImagePath;
                    model.Active = true;
                    model.Created = System.DateTime.Now;
                    model.CreatedBy = 1;
                    model.Updated = System.DateTime.Now;
                    model.UpdatedBy = 1;

                    string data = Newtonsoft.Json.JsonConvert.SerializeObject(model);
                    var content = new StringContent(data, Encoding.UTF8, "application/json");
                    var response = client.PostAsync(string.Format("{0}/api/ShopProductImages", WebAPIUrl), content).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var responseString = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                        string code = responseString.SelectToken("code").ToString();
                        if (code == "0")
                        {
                            //Save the Image File in Folder.
                            postedFile.SaveAs(Server.MapPath(ImagePath));
                        }
                    }

                    //string fileName = Path.GetFileName(postedFile.FileName);
                    //postedFile.SaveAs(path + fileName);

                    image.Name = ImageName;
                    imgList.Add(image);

                    model.ImageList = imgList;

                    ViewBag.Message += string.Format("<b>{0}</b> uploaded.<br />", ImageName);
                }
            }

            List<ShopProductAndProductImageModel> ShopProductInfo = new List<ShopProductAndProductImageModel>();
            var readTask = "";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(WebAPIUrl);
                var responseTask = client.GetAsync("/api/GetShopProductImageByShopProductID?ShopProductID=" + model.ShopProductID);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    readTask = result.Content.ReadAsStringAsync().Result;
                    ShopProductInfo = JsonConvert.DeserializeObject<List<ShopProductAndProductImageModel>>(readTask);
                }

                foreach (var images in ShopProductInfo)
                {
                    Images image = new Images();
                    image.Name = images.ImageName;
                    imgList.Add(image);
                }
                model.ImageList = imgList;
            }
            return View(model);
        }
    }
}