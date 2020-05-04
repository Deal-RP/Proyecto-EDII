using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Visual.Models;
using Newtonsoft.Json;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "Userrr", "Pass" };//CHINITO AQUI PODEMOS MANDAR A CIFRADO ESTO PERO NO SE QUE PENSAS
        }
        [HttpGet("{lista}")]
        public ActionResult<IEnumerable<object>> Get(string lista)
        {
            if (lista == "sucursales")
            {
                var sucursales = new List<Sucursal>();
                return sucursales;
            }
            else if (lista == "productos")
            {
                var productos = new List<Producto>();
                return productos;
            }
            else if (lista == "sucursalprecio")
            {
                var sucursalprecio = new List<SucursalPrecio>();
                return sucursalprecio;
            }
            else return null;
        }
        [Route("producto!")]
        public ActionResult Post(Producto item)
        {
            try
            {
                //insertar producto en arbol
            }
            catch (Exception)
            {
            }
            return Ok();
        }
        [Route("sucursal!")]
        public ActionResult Post(Sucursal item)
        {
            try
            {
                //insertar sucursal en arbol
            }
            catch (Exception)
            {
            }
            return Ok();
        }
        [Route("sucursalprecio!")]
        public ActionResult Post(SucursalPrecio item)
        {
            try
            {
                //insertar sucursal-precio en arbol
            }
            catch (Exception)
            {
            }
            return Ok();
        }
    }
}
