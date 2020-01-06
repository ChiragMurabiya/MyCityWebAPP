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

namespace MyCityWebApp.Areas.Admin.Controllers
{
    public class ShopController : Controller
    {
        string WebAPIUrl = ConfigurationManager.AppSettings["MyCityWebAPIUrl"];
        HttpClient client = new HttpClient();
        HttpResponseMessage response = new HttpResponseMessage();

        // GET: Admin/Shop
        public ActionResult Index()
        {
            IEnumerable<ShopAndImageModel> Shop = null;
            List<ShopAndImageModel> ShopInfo = new List<ShopAndImageModel>();
            var readTask = "";

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(WebAPIUrl);
                //HTTP GET
                var responseTask = client.GetAsync("/api/GetShopAndImage");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    readTask = result.Content.ReadAsStringAsync().Result;
                    ShopInfo = JsonConvert.DeserializeObject<List<ShopAndImageModel>>(readTask);

                    if (ShopInfo.Count == 0)
                    {
                        ViewBag.Message = "No data found";
                        return View(Shop);
                    }
                }
                else //web api sent error response 
                {
                    Shop = Enumerable.Empty<ShopAndImageModel>();
                    ViewBag.Message = "No data found";
                    return View(Shop);
                }
            }
            return View(ShopInfo);
        }

        private SelectList getStates()
        {
            List<StateModel> stateInfo = new List<StateModel>();
            var readTask = "";

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(WebAPIUrl);
                var responseTask = client.GetAsync("/api/States");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    readTask = result.Content.ReadAsStringAsync().Result;
                    stateInfo = JsonConvert.DeserializeObject<List<StateModel>>(readTask);
                }
                return new SelectList(stateInfo, "ID", "Name");
            }
        }

        public JsonResult GetCityList(int StateID)
        {
            List<CityModel> cityInfo = new List<CityModel>();
            var readTask = "";

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(WebAPIUrl);
                var responseTask = client.GetAsync("/api/Cities?StateID=" + StateID);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    readTask = result.Content.ReadAsStringAsync().Result;
                    cityInfo = JsonConvert.DeserializeObject<List<CityModel>>(readTask);
                }
                return Json(cityInfo, JsonRequestBehavior.AllowGet);
            }
        }

        private SelectList GetCategories()
        {
            List<CategoryModel> CategoryInfo = new List<CategoryModel>();
            var readTask = "";

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(WebAPIUrl);
                var responseTask = client.GetAsync("/api/Categories");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    readTask = result.Content.ReadAsStringAsync().Result;
                    CategoryInfo = JsonConvert.DeserializeObject<List<CategoryModel>>(readTask);
                }
                return new SelectList(CategoryInfo, "ID", "Name");
            }
        }

        [HttpGet]
        public ActionResult Create(int ID)
        {
            ShopAndImageModel model = new ShopAndImageModel();
            model.stateList = new SelectList(getStates(), "Value", "Text");
            model.CategoryList = new SelectList(GetCategories(), "Value", "Text");

            if (ID != 0)
            {
                //var response = client.GetAsync(string.Format("{0}/api/Shops/{1}", WebAPIUrl, ID)).Result;
                var response = client.GetAsync(string.Format("{0}/api/GetShopAndImageByShopID?ShopID={1}", WebAPIUrl, ID)).Result;

                if (response.IsSuccessStatusCode)
                {
                    var responseString = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                    string code = responseString.SelectToken("code").ToString();
                    if (code == "0")
                    {
                        model.ID = (int)responseString["data"][0]["ID"];
                        model.UserID = (int)responseString["data"][0]["UserID"];
                        model.CategoryID = (int)responseString["data"][0]["CategoryID"];
                        model.CityID = (int)responseString["data"][0]["CityID"];
                        model.StateID = (int)responseString["data"][0]["StateID"];
                        model.Name = (string)responseString["data"][0]["Name"];
                        model.Address = (string)responseString["data"][0]["Address"];
                        model.Phone = (string)responseString["data"][0]["Phone"];
                        model.Mobile = (string)responseString["data"][0]["Mobile"];
                        model.ShopImageID = (int)responseString["data"][0]["ShopImageID"];
                        model.ImageName = (string)responseString["data"][0]["ImageName"];
                        model.ImagePath = (string)responseString["data"][0]["ImagePath"];
                        model.Active = Convert.ToBoolean(responseString["data"][0]["Active"]);
                    }
                    else
                    {

                    }
                }
                return View(model);
            }
            else
            {
                return View(model);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ShopAndImageModel model, HttpPostedFileBase postedFile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.ID > 0)
                    {
                        model.Active = model.Active;
                        model.Created = System.DateTime.Now;
                        model.CreatedBy = 1;
                        model.Updated = System.DateTime.Now;
                        model.UpdatedBy = 1;
                        model.StateID = model.StateID;
                        model.CityID = model.CityID;
                        model.CategoryID = model.CategoryID;
                        model.UserID = 1;
                        model.ShopImageID = model.ShopImageID;

                        string data = Newtonsoft.Json.JsonConvert.SerializeObject(model);
                        var content = new StringContent(data, Encoding.UTF8, "application/json");
                        var response = client.PutAsync(string.Format("{0}/api/Shops", WebAPIUrl), content).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            var responseString = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                            string code = responseString.SelectToken("code").ToString();
                            int ShopID = Convert.ToInt32(responseString.SelectToken("id"));
                            if (code == "0")
                            {
                                //Image Upload Code Start
                                if (postedFile != null)
                                {
                                    //Extract Image File Name.
                                    string ImageName = System.IO.Path.GetFileName(postedFile.FileName);

                                    //Set the Image File Path.
                                    string ImagePath = "~/Areas/Images/" + ImageName;

                                    //Save the Image File in Folder.
                                    postedFile.SaveAs(Server.MapPath(ImagePath));

                                    ShopImageModel ShopImageModel = new ShopImageModel();
                                    ShopImageModel.ID = (int)model.ShopImageID;
                                    ShopImageModel.ImageName = ImageName;
                                    ShopImageModel.ImagePath = ImagePath;
                                    ShopImageModel.ShopID = ShopID;
                                    ShopImageModel.Active = true;
                                    ShopImageModel.Created = System.DateTime.Now;
                                    ShopImageModel.CreatedBy = 1;
                                    ShopImageModel.Updated = System.DateTime.Now;
                                    ShopImageModel.UpdatedBy = 1;

                                    string dataImage = Newtonsoft.Json.JsonConvert.SerializeObject(ShopImageModel);
                                    var contentImage = new StringContent(dataImage, Encoding.UTF8, "application/json");
                                    var responseImage = client.PutAsync(string.Format("{0}/api/ShopImages", WebAPIUrl), contentImage).Result;
                                    if (responseImage.IsSuccessStatusCode)
                                    {
                                        var responseStringImage = JObject.Parse(responseImage.Content.ReadAsStringAsync().Result);
                                        string codeImage = responseStringImage.SelectToken("code").ToString();
                                        if (codeImage == "0")
                                        {
                                            return RedirectToAction("Index", "Shop");
                                        }
                                    }
                                    else
                                    {
                                        ViewBag.Message = "Shop Image Insertion failed. " + responseImage.ToString();
                                        return View(model);
                                    }
                                }
                                //Image Upload Code End

                                return RedirectToAction("Index", "Shop");
                            }
                            else
                            {
                                ViewBag.Message = "State not found";
                                return View(model);
                            }
                        }
                        else
                        {
                            ViewBag.Message = "Update failed.";
                            return View(model);
                        }
                    }
                    else
                    {
                        model.Active = true;
                        model.Created = System.DateTime.Now;
                        model.CreatedBy = 1;
                        model.Updated = System.DateTime.Now;
                        model.UpdatedBy = 1;
                        model.StateID = model.StateID;
                        model.CityID = model.CityID;
                        model.UserID = 1;

                        string data = Newtonsoft.Json.JsonConvert.SerializeObject(model);
                        var content = new StringContent(data, Encoding.UTF8, "application/json");
                        var response = client.PostAsync(string.Format("{0}/api/Shops", WebAPIUrl), content).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            var responseString = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                            string code = responseString.SelectToken("code").ToString();
                            int ShopID = Convert.ToInt32(responseString.SelectToken("id"));
                            if (code == "0")
                            {
                                //Image Upload Code Start
                                if (postedFile != null)
                                {
                                    //Extract Image File Name.
                                    string ImageName = System.IO.Path.GetFileName(postedFile.FileName);

                                    //Set the Image File Path.
                                    string ImagePath = "~/Areas/Images/" + ImageName;

                                    //Save the Image File in Folder.
                                    postedFile.SaveAs(Server.MapPath(ImagePath));

                                    ShopImageModel ShopImageModel = new ShopImageModel();
                                    ShopImageModel.ImageName = ImageName;
                                    ShopImageModel.ImagePath = ImagePath;
                                    ShopImageModel.ShopID = ShopID;
                                    ShopImageModel.Active = true;
                                    ShopImageModel.Created = System.DateTime.Now;
                                    ShopImageModel.CreatedBy = 1;
                                    ShopImageModel.Updated = System.DateTime.Now;
                                    ShopImageModel.UpdatedBy = 1;

                                    string dataImage = Newtonsoft.Json.JsonConvert.SerializeObject(ShopImageModel);
                                    var contentImage = new StringContent(dataImage, Encoding.UTF8, "application/json");
                                    var responseImage = client.PostAsync(string.Format("{0}/api/ShopImages", WebAPIUrl), contentImage).Result;
                                    if (responseImage.IsSuccessStatusCode)
                                    {
                                        var responseStringImage = JObject.Parse(responseImage.Content.ReadAsStringAsync().Result);
                                        string codeImage = responseStringImage.SelectToken("code").ToString();
                                        if (codeImage == "0")
                                        {
                                            return RedirectToAction("Index", "Shop");
                                        }
                                    }
                                    else
                                    {
                                        ViewBag.Message = "Shop Image Insertion failed. " + responseImage.ToString();
                                        return View(model);
                                    }
                                }
                                //Image Upload Code End

                                return RedirectToAction("Index", "Shop");
                            }
                            else
                            {
                                ViewBag.Message = "Shop already exists.";
                                return View(model);
                            }
                        }
                        else
                        {
                            ViewBag.Message = "Insert failed. " + response.ToString();
                            return View(model);
                        }
                    }
                }
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int DeleteShop(int ShopID, int ShopImageID)
        {
            ShopImageModel model = new ShopImageModel();
            if (ShopImageID != 0)
            {
                var response = client.GetAsync(string.Format("{0}/api/ShopImages/{1}", WebAPIUrl, ShopImageID)).Result;
                if (response.IsSuccessStatusCode)
                {
                    var responseString = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                    string code = responseString.SelectToken("code").ToString();
                    if (code == "0")
                    {
                        model.ImageName = (string)responseString["data"]["ImageName"];
                        model.ImagePath = (string)responseString["data"]["ImagePath"];
                    }
                }
            }
            string fullPath = Request.MapPath("~/Areas/Images/" + model.ImageName);
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }

            response = client.DeleteAsync(string.Format("{0}/api/ShopImages/{1}", WebAPIUrl, ShopImageID)).Result;
            if (response.IsSuccessStatusCode)
            {
                var result = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                string code = result.SelectToken("code").ToString();
                if (code == "0")
                {
                    response = client.DeleteAsync(string.Format("{0}/api/Shops/{1}", WebAPIUrl, ShopID)).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        result = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                        code = result.SelectToken("code").ToString();
                        if (code == "0")
                        {
                            return 0;
                        }
                    }
                    return 0;
                }
            }
            return -1;
        }
    }
}