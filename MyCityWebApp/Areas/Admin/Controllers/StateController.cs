using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyCityWebApp.Areas.Admin.Models;
using MyCityWebApp.Areas.Admin.Controllers;
using System.Configuration;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

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
                    //readTask.Wait();

                    //state = readTask;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    stateInfo = JsonConvert.DeserializeObject<List<StateModel>>(readTask);
                }
                else //web api sent error response 
                {
                    //log response status here..

                    state = Enumerable.Empty<StateModel>();

                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            return View(stateInfo);
        }

        public ActionResult Create(int ID)
        {
            StateModel model = new StateModel();

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

                        string data = Newtonsoft.Json.JsonConvert.SerializeObject(model);
                        var content = new StringContent(data, Encoding.UTF8, "application/json");
                        var response = client.PutAsync("http://localhost:49254/api/States", content).Result;
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
                                ViewBag.Message = "This email is already being used by another login.  If you want to use that login, please press cancel below and login with that email.   You may request a password change if you don't remember the password. OK";
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
                        var response = client.PostAsync("/api/States", content).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            var responseString = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                            string code = responseString.SelectToken("code").ToString();
                            if (code == "0")
                            {
                                return View("Index");
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
                return RedirectToAction("Index", "State");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}