using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Proyecto_EDII.Models;
using Arboles;

namespace Proyecto_EDII.Controllers
{
    delegate string ObjectToString(object o);
    delegate object StringToObject(string s);

    [ApiController]
    [Route("[controller]")]
    public class WareHouseController : ControllerBase
    {
        #region ADD METHODS
        [HttpPost, Route("ADD1")]
        public void AddOffice([FromForm]OfficeData info)
        {
            DatosArboles.Instance.key = 15;
            ArbolB<OfficeData>.IniciarArbol("Office", new StringToObject(OfficeData.StringToObject), new ObjectToString(OfficeData.ObjectToString));
            for (int i = 0; i < 500; i++)
            {
                ArbolB<OfficeData>.InsertarArbol(new OfficeData { ID = i, Name = "prueba", Price = 5.5});
            }
        }

        public List<OfficeProduct> prueba()
        {
            ArbolB<OfficeProduct>.IniciarArbol("OfficeProduct", new StringToObject(OfficeProduct.StringToObject), new ObjectToString(OfficeProduct.ObjectToString));
            DatosArboles.Instance.key = 15;
            return ArbolB<OfficeProduct>.Recorrido(new OfficeProduct { IdOffice = 5, IdProduct = 5}, null, 1);
        }

        [HttpPost, Route("ADD2")]
        public void AddProduct([FromForm]ProductData info)
        {
            DatosArboles.Instance.key = 15;
            ArbolB<ProductData>.IniciarArbol("Product", new StringToObject(ProductData.StringToObject), new ObjectToString(ProductData.ObjectToString));
            for (int i = 0; i < 500; i++)
            {
                ArbolB<ProductData>.InsertarArbol(new ProductData { ID = i});
            }
            
        }

        [HttpPost, Route("ADD3")]
        public void AddProductOffice([FromForm]OfficeProduct info)
        {
            DatosArboles.Instance.key = 15;
            ArbolB<OfficeProduct>.IniciarArbol("OfficeProduct", new StringToObject(OfficeProduct.StringToObject), new ObjectToString(OfficeProduct.ObjectToString));
            for (int i = 0; i < 500; i++)
            {
                ArbolB<OfficeProduct>.InsertarArbol(new OfficeProduct { IdOffice = i, IdProduct = i, Inventory = i});
            }
        }

        [HttpPost, Route("ALTER")]
        public void AlterOffice([FromForm]OfficeData info)
        {
            DatosArboles.Instance.key = 15;
            ArbolB<OfficeData>.IniciarArbol("Office", new StringToObject(OfficeData.StringToObject), new ObjectToString(OfficeData.ObjectToString));
            ArbolB<OfficeData>.Recorrido(info, null);
        }
        #endregion
    }
}
