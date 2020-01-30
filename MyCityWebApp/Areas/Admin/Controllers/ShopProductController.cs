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
    public class ShopProductController : Controller
    {
        string WebAPIUrl = ConfigurationManager.AppSettings["MyCityWebAPIUrl"];
        HttpClient client = new HttpClient();
        HttpResponseMessage response = new HttpResponseMessage();

        // GET: Admin/ShopProduct
        public ActionResult Index(int ShopID)
        {
            IEnumerable<ShopProductAndProductImageModel> ShopProduct = null;
            List<ShopProductAndProductImageModel> ShopProductInfo = new List<ShopProductAndProductImageModel>();
            var readTask = "";

            ViewBag.ShopID = ShopID;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(WebAPIUrl);
                //HTTP GET
                var responseTask = client.GetAsync("/api/GetShopProduct?ShopId=" + ShopID);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    readTask = result.Content.ReadAsStringAsync().Result;
                    ShopProductInfo = JsonConvert.DeserializeObject<List<ShopProductAndProductImageModel>>(readTask);

                    if (ShopProductInfo.Count == 0)
                    {
                        ViewBag.Message = "No data found";
                        return View(ShopProduct);
                    }
                }
                else //web api sent error response 
                {
                    ShopProduct = Enumerable.Empty<ShopProductAndProductImageModel>();
                    ViewBag.Message = "No data found";
                    return View(ShopProduct);
                }
            }
            return View(ShopProductInfo);
        }

        public ActionResult Create(int ID, int ShopID)
        {
            ShopProductAndProductImageModel model = new ShopProductAndProductImageModel();
            if (ID != 0)
            {
                var response = client.GetAsync(string.Format("{0}/api/ShopProducts/{1}", WebAPIUrl, ID)).Result;
                if (response.IsSuccessStatusCode)
                {
                    var responseString = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                    string code = responseString.SelectToken("code").ToString();
                    if (code == "0")
                    {
                        model.ID = (int)responseString["data"]["ID"];
                        model.Name = (string)responseString["data"]["Name"];
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
        public ActionResult Create(ShopProductAndProductImageModel model)
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
                        var response = client.PutAsync(string.Format("{0}/api/ShopProducts", WebAPIUrl), content).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            var responseString = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                            string code = responseString.SelectToken("code").ToString();
                            if (code == "0")
                            {
                                return RedirectToAction("Index", "ShopProduct", new { ShopID = model.ShopID });
                            }
                            else
                            {
                                ViewBag.Message = "Product not found";
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

                        string data = Newtonsoft.Json.JsonConvert.SerializeObject(model);
                        var content = new StringContent(data, Encoding.UTF8, "application/json");
                        var response = client.PostAsync(string.Format("{0}/api/ShopProducts", WebAPIUrl), content).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            var responseString = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                            string code = responseString.SelectToken("code").ToString();
                            if (code == "0")
                            {
                                return RedirectToAction("Index", "ShopProduct", new { ShopID = model.ShopID });
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
    }
}