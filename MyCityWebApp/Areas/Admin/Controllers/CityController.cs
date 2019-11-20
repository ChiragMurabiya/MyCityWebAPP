using MyCityWebApp.Areas.Admin.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Mvc;

namespace MyCityWebApp.Areas.Admin.Controllers
{
    public class CityController : Controller
    {
        string WebAPIUrl = ConfigurationManager.AppSettings["MyCityWebAPIUrl"];
        HttpClient client = new HttpClient();
        HttpResponseMessage response = new HttpResponseMessage();

        // GET: Admin/City
        public ActionResult Index()
        {
            IEnumerable<CityModel> city = null;
            List<CityModel> cityInfo = new List<CityModel>();
            var readTask = "";

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(WebAPIUrl);
                //HTTP GET
                var responseTask = client.GetAsync("/api/Cities");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    readTask = result.Content.ReadAsStringAsync().Result;
                    cityInfo = JsonConvert.DeserializeObject<List<CityModel>>(readTask);

                    if (cityInfo.Count == 0)
                    {
                        ViewBag.Message = "No data found";
                        return View(city);
                    }
                }
                else //web api sent error response 
                {
                    city = Enumerable.Empty<CityModel>();
                    ViewBag.Message = "No data found";
                    return View(city);
                }
            }
            return View(cityInfo);
        }

        private SelectList getStates()           //fetch the tournament type details from the table
        {

            List<StateModel> stateInfo = new List<StateModel>();
            var readTask = "";

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(WebAPIUrl);
                //HTTP GET
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

        public ActionResult Create(int ID)
        {
            //ViewBag.stateNameList = getStates();            
            CityModel model = new CityModel();
            model.stateList = new SelectList(getStates(), "Value", "Text");

            if (ID != 0)
            {
                var response = client.GetAsync(string.Format("{0}/api/Cities/{1}", WebAPIUrl, ID)).Result;
                if (response.IsSuccessStatusCode)
                {
                    var responseString = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                    string code = responseString.SelectToken("code").ToString();
                    if (code == "0")
                    {
                        model.ID = (int)responseString["data"]["ID"];
                        model.Name = (string)responseString["data"]["Name"];
                        
                        model.stateID = (int)responseString["data"]["StateID"];

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
        public ActionResult Create(CityModel model)
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
                        model.stateID = model.stateID;

                        string data = Newtonsoft.Json.JsonConvert.SerializeObject(model);
                        var content = new StringContent(data, Encoding.UTF8, "application/json");
                        var response = client.PutAsync(string.Format("{0}/api/Cities", WebAPIUrl), content).Result;
                        //var response = client.PutAsync("http://localhost:49254/api/Cities/", content).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            var responseString = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                            string code = responseString.SelectToken("code").ToString();
                            if (code == "0")
                            {
                                return RedirectToAction("Index", "City");
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
                        model.stateID = model.stateID;

                        string data = Newtonsoft.Json.JsonConvert.SerializeObject(model);
                        var content = new StringContent(data, Encoding.UTF8, "application/json");
                        var response = client.PostAsync(string.Format("{0}/api/Cities", WebAPIUrl), content).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            var responseString = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                            string code = responseString.SelectToken("code").ToString();
                            if (code == "0")
                            {
                                return RedirectToAction("Index", "City");
                            }
                            else
                            {
                                ViewBag.Message = "State already exists.";
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

        public int DeleteCity(int CityID)
        {
            response = client.DeleteAsync(string.Format("{0}/api/Cities/{1}", WebAPIUrl, CityID)).Result;
            if (response.IsSuccessStatusCode)
            {
                var result = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                string code = result.SelectToken("code").ToString();
                if (code == "0")
                {
                    return 0;
                }
                else if (code == "1")
                {
                    return 1;
                }
                else if (code == "2")
                {
                    return 2;
                }

            }
            return -1;
        }
    }
}