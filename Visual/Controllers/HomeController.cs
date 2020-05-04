using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Visual.Models;

namespace Visual.Controllers
{
    public class HomeController : Controller
    {
        // direccion .https://localhost:44347/.WareHouse
        public ActionResult Home()
        {
            return View();
        }

        public ActionResult Main()
        {
            return View();
        }
        
        public ActionResult Login()
        {
            try
            {
                string apiUser = string.Empty;
				string apiPass = string.Empty;
                string nombre = Request.Form["Usuario"].ToString();
                string password = Request.Form["Password"].ToString();
                using (var client = new HttpClient())
                {//https://localhost:44347/api/values
                    client.BaseAddress = new Uri("https://localhost:44347/");//DIRECCION API
                    var responseTask = client.GetAsync("api/values");// CONTROLLER/ACTION
                    responseTask.Wait();
                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsStringAsync();
                        readTask.Wait();
                        var obtenidoApi = JsonConvert.DeserializeObject<string[]>(readTask.Result);
                        apiUser = obtenidoApi[0];//AQUI LO DESCIFRARIAMOS
                        apiPass = obtenidoApi[1];
                    }
                    else
                    {
                        apiUser = null;
                        apiPass = null;
                    }
                }
                if (apiUser == null)
                {
                    ModelState.AddModelError(string.Empty, "Usuario no creado");
                }
                if (apiUser == nombre)
                {
                    if (apiPass == password)
                    {
                        return RedirectToAction("Main", "Home");
                    }
                }
                ModelState.AddModelError(string.Empty, "Datos incorrectos");
                return View();
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Productos()
        {
            try
            {
                using (var client = new HttpClient())
                {//https://localhost:44347//api/values
                    client.BaseAddress = new Uri("https://localhost:44347/");//DIRECCION API
                    var responseTask = client.GetAsync("api/values/productos");// CONTROLLER/ACTION
                    responseTask.Wait();
                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsStringAsync();
                        readTask.Wait();
                        var obtenidoApi = JsonConvert.DeserializeObject<IEnumerable<Producto>>(readTask.Result);
                        return View(obtenidoApi);
                    }
                }
            }
            catch (Exception)
            {
                return View("Main");
            }
            return View("Main");
        }

        public ActionResult Sucursales()
        {
            try
            {
                using (var client = new HttpClient())
                {// https://localhost:44347//api/values
                    client.BaseAddress = new Uri("https://localhost:44347/");//DIRECCION API
                    var responseTask = client.GetAsync("api/values/sucursales");// CONTROLLER/ACTION
                    responseTask.Wait();
                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsStringAsync();
                        readTask.Wait();
                        var obtenidoApi = JsonConvert.DeserializeObject<IEnumerable<Sucursal>>(readTask.Result);
                        return View(obtenidoApi);
                    }
                }
            }
            catch (Exception)
            {
                return View("Main");
            }
            return View("Main");
        }

        public ActionResult SucursalPrecio()
        {
            try
            {
                using (var client = new HttpClient())
                {//https://localhost:44347//api/values
                    client.BaseAddress = new Uri("https://localhost:44347/");//DIRECCION API
                    var responseTask = client.GetAsync("api/values/sucursalprecio");// CONTROLLER/ACTION
                    responseTask.Wait();
                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsStringAsync();
                        readTask.Wait();
                        var obtenidoApi = JsonConvert.DeserializeObject<IEnumerable<SucursalPrecio>>(readTask.Result);
                        return View(obtenidoApi);
                    }
                }
            }
            catch (Exception)
            {
                return View("Main");
            }
            return View("Main");
        }

        public ActionResult NuevoProducto()
        {
            return View();
        }

        public ActionResult NuevoSucursal()
        {
            return View();
        }

        public ActionResult NuevoSucursalPrecio()
        {
            return View();
        }

        [HttpPost]
        public ActionResult NuevoProducto(Producto item)
        {
            var producto = new Producto();
            var id = int.Parse(Request.Form["id"].ToString());
            var nombre = Request.Form["nom"].ToString();
            float precio = float.Parse(Request.Form["pre"].ToString());
            //https://localhost:44347//api/values
            try
            {
                producto.id = id;
                producto.nombre = nombre;
                producto.precio = precio;
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:44347/");
                    var responseTask = client.PostAsJsonAsync<Producto>("api/values/producto!", producto);
                    responseTask.Wait();
                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {

                    }
                }
            }
            catch (Exception)
            {

            }
            //
            return RedirectToAction("Productos");
        }

        [HttpPost]
        public ActionResult NuevoSucursal(Sucursal item)
        {
            var id = int.Parse(Request.Form["id"].ToString());
            var nombre = Request.Form["nom"].ToString();
            var direccion = Request.Form["dir"].ToString();
            var sucursal = new Sucursal();
            //
            try
            {
                sucursal.id = id;
                sucursal.nombre = nombre;
                sucursal.direccion = direccion;
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:44347/");
                    var responseTask = client.PostAsJsonAsync<Sucursal>("api/values/sucursal!",sucursal);
                    responseTask.Wait();
                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {

                    }
                }
            }
            catch (Exception)
            {

            }
            //
            return RedirectToAction("Sucursales");
        }

        [HttpPost]
        public ActionResult NuevoSucursalPrecio(SucursalPrecio item)
        {
            var id_sucursal = int.Parse(Request.Form["id_s"].ToString());
            var id_producto = int.Parse(Request.Form["id_p"].ToString());
            var cantidad = int.Parse(Request.Form["cant"].ToString());
            var sucupre = new SucursalPrecio();
            //
            try
            {
                sucupre.id_s = id_sucursal;
                sucupre.id_p = id_producto;
                sucupre.cantidad = cantidad;
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:44347/");
                    var responseTask = client.PostAsJsonAsync<SucursalPrecio>("api/values/sucursalprecio!", sucupre);
                    responseTask.Wait();
                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {

                    }
                }
            }
            catch (Exception)
            {

            }
            //
            return RedirectToAction("SucursalPrecio");
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
