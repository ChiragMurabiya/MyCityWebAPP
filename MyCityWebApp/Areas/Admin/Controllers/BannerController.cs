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
    public class BannerController : Controller
    {
        string WebAPIUrl = ConfigurationManager.AppSettings["MyCityWebAPIUrl"];
        HttpClient client = new HttpClient();
        HttpResponseMessage response = new HttpResponseMessage();

        // GET: Admin/Banner
        public ActionResult Index()
        {
            IEnumerable<BannerModel> Banner = null;
            List<BannerModel> BannerInfo = new List<BannerModel>();
            var readTask = "";

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(WebAPIUrl);
                //HTTP GET
                var responseTask = client.GetAsync("/api/Banners");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    readTask = result.Content.ReadAsStringAsync().Result;
                    BannerInfo = JsonConvert.DeserializeObject<List<BannerModel>>(readTask);

                    if (BannerInfo.Count == 0)
                    {
                        ViewBag.Message = "No data found";
                        return View(Banner);
                    }
                }
                else //web api sent error response 
                {
                    Banner = Enumerable.Empty<BannerModel>();
                    ViewBag.Message = "No data found";
                    return View(Banner);
                }
            }
            return View(BannerInfo);
        }

        public ActionResult Create(int ID)
        {
            BannerModel model = new BannerModel();
            if (ID != 0)
            {
                var response = client.GetAsync(string.Format("{0}/api/Banners/{1}", WebAPIUrl, ID)).Result;
                if (response.IsSuccessStatusCode)
                {
                    var responseString = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                    string code = responseString.SelectToken("code").ToString();
                    if (code == "0")
                    {
                        model.ID = (int)responseString["data"]["ID"];
                        model.ImageName = (string)responseString["data"]["Name"];
                        model.Active = Convert.ToBoolean(responseString["data"]["Active"]);
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
        public ActionResult Create(BannerModel model, HttpPostedFileBase postedFile)
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

                        string data = Newtonsoft.Json.JsonConvert.SerializeObject(model);
                        var content = new StringContent(data, Encoding.UTF8, "application/json");
                        var response = client.PutAsync(string.Format("{0}/api/Categories", WebAPIUrl), content).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            var responseString = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                            string code = responseString.SelectToken("code").ToString();
                            if (code == "0")
                            {
                                return RedirectToAction("Index", "Category");
                            }
                            else
                            {
                                ViewBag.Message = "Category not found";
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
                        if (postedFile != null)
                        {
                            //Extract Image File Name.
                            string ImageName = System.IO.Path.GetFileName(postedFile.FileName);

                            //Set the Image File Path.
                            string ImagePath = "~/Areas/Images/" + ImageName;

                            //Save the Image File in Folder.
                            postedFile.SaveAs(Server.MapPath(ImagePath));

                            model.ImageName = ImageName;
                            model.ImagePath = ImagePath;
                            model.Active = true;
                            model.Created = System.DateTime.Now;
                            model.CreatedBy = 1;
                            model.Updated = System.DateTime.Now;
                            model.UpdatedBy = 1;

                            string data = Newtonsoft.Json.JsonConvert.SerializeObject(model);
                            var content = new StringContent(data, Encoding.UTF8, "application/json");
                            var response = client.PostAsync(string.Format("{0}/api/Banners", WebAPIUrl), content).Result;
                            if (response.IsSuccessStatusCode)
                            {
                                var responseString = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                                string code = responseString.SelectToken("code").ToString();
                                if (code == "0")
                                {
                                    return RedirectToAction("Index", "Banner");
                                }
                            }
                            else
                            {
                                ViewBag.Message = "Insert failed. " + response.ToString();
                                return View(model);
                            }
                        }
                        else
                        {
                            ViewBag.Message = "Please choose file to upload.";
                            return View(model);
                        }
                    }
                    //Image Upload Code End
                    return RedirectToAction("Index", "Banner");
                }
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int DeleteBanner(int BannerID)
        {
            response = client.DeleteAsync(string.Format("{0}/api/Banners/{1}", WebAPIUrl, BannerID)).Result;
            if (response.IsSuccessStatusCode)
            {
                var result = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                string code = result.SelectToken("code").ToString();
                if (code == "0")
                {
                    string fullPath = Request.MapPath("~/Areas/Images/" + model.ImageName);
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }

                    return 0;
                }
            }
            return -1;
        }
    }
}