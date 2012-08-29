using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Mvc;
using TestAnObject;

namespace TestMvc4.Controllers
{
    public class ValueClientController : Controller
    {
        private HttpClient client;

        //
        // GET: /ValueClient/

        public ActionResult Index()
        {
            var response = GetClient().GetAsync("api/values").Result;
            response.EnsureSuccessStatusCode(); // Throw on error code.
            var objects = response.Content.ReadAsAsync<IEnumerable<AnObject>>().Result;
            return View(objects);
        }

        private HttpClient GetClient()
        {
            if(client == null)
            {
                client = new HttpClient();
                var url = ControllerContext.HttpContext.Request.Url;
                client.BaseAddress = new Uri("http://127.0.0.1:81");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            }
            return client;
        }

        //
        // GET: /ValueClient/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /ValueClient/Create

        [HttpPost]
        public ActionResult Create(AnObject anObject)
        {
            try
            {
                var response = GetClient().PostAsJsonAsync("api/values", anObject).Result;
                response.EnsureSuccessStatusCode();
                return RedirectToAction("Index");
            }
            catch
            {
                return View(anObject);
            }
        }

        //
        // GET: /ValueClient/Edit/5

        public ActionResult Edit(int id)
        {
            var anObject = GetAnObject(id);
            return View(anObject);
        }

        private AnObject GetAnObject(int id)
        {
            var response = GetClient().GetAsync(string.Format("api/values/{0}", id)).Result;
            response.EnsureSuccessStatusCode(); // Throw on error code.
            var anObject = response.Content.ReadAsAsync<AnObject>().Result;
            return anObject;
        }

        //
        // POST: /ValueClient/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, AnObject anObject)
        {
            try
            {
                var response = GetClient().PutAsJsonAsync(string.Format("api/values/{0}", id), anObject).Result;
                response.EnsureSuccessStatusCode();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /ValueClient/Delete/5

        public ActionResult Delete(int id)
        {
            var anObject = GetAnObject(id);
            return View(anObject);
        }

        //
        // POST: /ValueClient/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                var response = GetClient().DeleteAsync(string.Format("api/values/{0}", id)).Result;
                response.EnsureSuccessStatusCode();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
