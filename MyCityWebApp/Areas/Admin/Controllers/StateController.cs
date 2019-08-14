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
    public class StateController : Controller
    {
        string WebAPIUrl = ConfigurationManager.AppSettings["MyCityWebAPIUrl"];
        HttpClient client = new HttpClient();
        HttpResponseMessage response = new HttpResponseMessage();

        // GET: Admin/State
        public ActionResult Index()
        {
            IEnumerable<StateModel> state = null;
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

                    if (stateInfo.Count == 0)
                    {
                        ViewBag.Message = "No data found";
                        return View(state);
                    }
                }
                else //web api sent error response 
                {
                    state = Enumerable.Empty<StateModel>();                    
                    ViewBag.Message = "No data found";
                    return View(state);
                }
            }
            return View(stateInfo);
        }

        public ActionResult Create(int ID)
        {
            StateModel model = new StateModel();
            if (ID != 0)
            {
                var response = client.GetAsync(string.Format("{0}/api/States/{1}", WebAPIUrl, ID)).Result;
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
        public ActionResult Create(StateModel model)
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
                        //var response = client.PutAsync("/api/States", content).Result;
                        var response = client.PutAsync(string.Format("{0}/api/States", WebAPIUrl), content).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            var responseString = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                            string code = responseString.SelectToken("code").ToString();
                            if (code == "0")
                            {
                                return RedirectToAction("Index", "State");
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

                        string data = Newtonsoft.Json.JsonConvert.SerializeObject(model);
                        var content = new StringContent(data, Encoding.UTF8, "application/json");
                        //var response = client.PostAsync("api/States", content).Result;
                        var response = client.PostAsync(string.Format("{0}/api/States", WebAPIUrl), content).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            var responseString = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                            string code = responseString.SelectToken("code").ToString();
                            if (code == "0")
                            {
                                return RedirectToAction("Index", "State");
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
                    return RedirectToAction("Index", "State");
                }
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int DeleteState(int StateID)
        {
            response = client.DeleteAsync(string.Format("{0}/api/States/{1}", WebAPIUrl, StateID)).Result;
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