using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Proyecto_EDII.Models;
using Arboles;
using System.IO;
using System.Net.Mime;
using Microsoft.AspNetCore.Http;

namespace Proyecto_EDII.Controllers
{
    delegate string ObjectToString(object o);
    delegate object StringToObject(string s);
    delegate object Modify(object o, string[] s);

    [ApiController]
    [Route("[controller]")]
    public class WareHouseController : ControllerBase
    {
        #region SHOW METHODS
        //Todos los datos
        [HttpGet, Route("product")]
        public List<ProductData> AllProduct()
        {
            ArbolB<ProductData>.IniciarArbol("Product", new StringToObject(ProductData.StringToObject), new ObjectToString(ProductData.ObjectToString));
            SDES.obtainKey();
            return ArbolB<ProductData>.Recorrido(null);
        }

        [HttpGet, Route("office")]
        public List<OfficeData> AllOffice()
        {
            ArbolB<OfficeData>.IniciarArbol("Office", new StringToObject(OfficeData.StringToObject), new ObjectToString(OfficeData.ObjectToString));
            SDES.obtainKey();
            return ArbolB<OfficeData>.Recorrido(null);
        }

        [HttpGet, Route("officeProduct")]
        public List<OfficeProduct> AllOfficeProduct()
        {
            ArbolB<OfficeProduct>.IniciarArbol("OfficeProduct", new StringToObject(OfficeProduct.StringToObject), new ObjectToString(OfficeProduct.ObjectToString));
            SDES.obtainKey();
            return ArbolB<OfficeProduct>.Recorrido(null);
        }

        //Solo un dato
        [HttpGet, Route("productOne")]
        public List<ProductData> ProductShow([FromForm]int id)
        {
            ArbolB<ProductData>.IniciarArbol("Product", new StringToObject(ProductData.StringToObject), new ObjectToString(ProductData.ObjectToString));
            SDES.obtainKey();
            return ArbolB<ProductData>.Recorrido(new ProductData { ID = id}, 1);
        }

        [HttpGet, Route("officeOne")]
        public List<OfficeData> OfficeShow([FromForm] int id)
        {
            ArbolB<OfficeData>.IniciarArbol("Office", new StringToObject(OfficeData.StringToObject), new ObjectToString(OfficeData.ObjectToString));
            SDES.obtainKey();
            return ArbolB<OfficeData>.Recorrido(new OfficeData { ID = id}, 1);
        }

        [HttpGet, Route("officeProductOne")]
        public List<OfficeProduct> OfficeProductShow([FromForm] int idOffice, [FromForm]int idProduct)
        {
            ArbolB<OfficeProduct>.IniciarArbol("OfficeProduct", new StringToObject(OfficeProduct.StringToObject), new ObjectToString(OfficeProduct.ObjectToString));
            SDES.obtainKey();
            return ArbolB<OfficeProduct>.Recorrido(new OfficeProduct { IdOffice = idOffice, IdProduct = idProduct }, 1);
        }
        #endregion

        #region ADD METHODS
        [HttpPost, Route("ADD/office")]
        public void Add([FromForm]OfficeData info)
        {
            ArbolB<OfficeData>.IniciarArbol("Office", new StringToObject(OfficeData.StringToObject), new ObjectToString(OfficeData.ObjectToString));
            SDES.obtainKey();
            for (int i = 0; i < 500; i++)
            {
                ArbolB<OfficeData>.InsertarArbol(new OfficeData { ID = ArbolB<OfficeData>.newID(), Name = info.Name, Address = info.Address });
            }
        }

        [HttpPost, Route("ADD/product")]
        public void Add([FromForm]ProductData info)
        {
            ArbolB<ProductData>.IniciarArbol("Product", new StringToObject(ProductData.StringToObject), new ObjectToString(ProductData.ObjectToString));
            SDES.obtainKey();
            for (int i = 0; i < 500; i++)
            {
                ArbolB<ProductData>.InsertarArbol(new ProductData { ID = ArbolB<ProductData>.newID(), Name = "prueba", Price = 5.5 });
            }
        }

        [HttpPost, Route("ADD/productCSV")]
        public void Add([FromForm]IFormFile info)
        {
            ArbolB<ProductData>.IniciarArbol("Product", new StringToObject(ProductData.StringToObject), new ObjectToString(ProductData.ObjectToString));
            SDES.obtainKey();
            ProductData.InsertCSV(info.OpenReadStream());
        }

        [HttpPost, Route("ADD/officeProduct")]
        public void Add([FromForm]OfficeProduct info)
        {
            ArbolB<OfficeProduct>.IniciarArbol("OfficeProduct", new StringToObject(OfficeProduct.StringToObject), new ObjectToString(OfficeProduct.ObjectToString));
            SDES.obtainKey();
            for (int i = 0; i < 500; i++)
            {
                ArbolB<OfficeProduct>.InsertarArbol(new OfficeProduct { IdOffice = i, IdProduct = i, Inventory = i });
            }
        }
        #endregion

        #region COMPRESS METHODS
        [HttpGet, Route("OBTAIN/{name}")]
        public async Task<FileStreamResult> compressProduct(string name)
        {
            LZW.Comprimir(name);
            return await Download($"temp\\{name}.txt");
        }

        async Task<FileStreamResult> Download(string path)
        {
            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            Directory.Delete("temp", true);
            return File(memory, MediaTypeNames.Application.Octet, Path.GetFileName(path));
        }
        #endregion

        #region LOAD METHODS
        [HttpPost, Route("LOAD")]
        public void decompressProduct([FromForm]IFormFile archivo)
        {
            LZW.Descomprimir(archivo);
        }
        #endregion

        #region CHANGE KEY
        [HttpPost, Route("ALTER/{key}")]
        public void Alter(int key)
        {
            if (key < 1024 && key >= 0)
            {
                ArbolB<OfficeData>.IniciarArbol("Office", new StringToObject(OfficeData.StringToObject), new ObjectToString(OfficeData.ObjectToString));
                ArbolB<OfficeData>.ModificarArbol(key);

                ArbolB<OfficeProduct>.IniciarArbol("OfficeProduct", new StringToObject(OfficeProduct.StringToObject), new ObjectToString(OfficeProduct.ObjectToString));
                ArbolB<OfficeProduct>.ModificarArbol(key);

                ArbolB<ProductData>.IniciarArbol("Product", new StringToObject(ProductData.StringToObject), new ObjectToString(ProductData.ObjectToString));
                ArbolB<ProductData>.ModificarArbol(key);
                SDES.manageKey(key);
            }
        }
        #endregion

        #region ALTER METHODS
        [HttpPost, Route("ALTER/office")]
        public void Alter([FromForm]OfficeData info)
        {
            SDES.obtainKey();
            ArbolB<OfficeData>.IniciarArbol("Office", new StringToObject(OfficeData.StringToObject), new ObjectToString(OfficeData.ObjectToString));
            ArbolB<OfficeData>.Modificar(info,
                new string[2] { info.Name, info.Address },
                new Modify(OfficeData.Alter));
        }

        [HttpPost, Route("ALTER/product1")]
        public void Alter([FromForm]ProductData info)
        {
            SDES.obtainKey();
            ArbolB<ProductData>.IniciarArbol("Product", new StringToObject(ProductData.StringToObject), new ObjectToString(ProductData.ObjectToString));
            ArbolB<ProductData>.Modificar(info, new string[2] { info.Name, info.Price.ToString() }, new Modify(ProductData.Alter));
        }

        [HttpPost, Route("ALTER/product2")]
        public void Alter([FromForm]string Name, [FromForm]int id)
        {
            SDES.obtainKey();
            ArbolB<ProductData>.IniciarArbol("Product", new StringToObject(ProductData.StringToObject), new ObjectToString(ProductData.ObjectToString));
            ArbolB<ProductData>.Modificar(new ProductData { ID = id }, new string[2] { Name, null }, new Modify(ProductData.Alter));
        }

        [HttpPost, Route("ALTER/officeProduct")]
        public void Alter([FromForm]OfficeProduct info)
        {
            SDES.obtainKey();
            ArbolB<OfficeProduct>.IniciarArbol("OfficeProduct", new StringToObject(OfficeProduct.StringToObject), new ObjectToString(OfficeProduct.ObjectToString));
            ArbolB<OfficeProduct>.Modificar(info, new string[2] { info.Inventory.ToString(), null }, new Modify(OfficeProduct.Alter));
        }
        #endregion

        #region TRANSFER METHOD
        [HttpPost, Route("TRANSFER")]
        public void transfer([FromForm]int idProduct, [FromForm]int idO1, [FromForm]int idO2, [FromForm]int cant)
        {
            SDES.obtainKey();
            ArbolB<OfficeProduct>.IniciarArbol("OfficeProduct", new StringToObject(OfficeProduct.StringToObject), new ObjectToString(OfficeProduct.ObjectToString));
            var data1 = ArbolB<OfficeProduct>.Recorrido(new OfficeProduct { IdOffice = idO1, IdProduct = idProduct }, 1);
            var data2 = ArbolB<OfficeProduct>.Recorrido(new OfficeProduct { IdOffice = idO2, IdProduct = idProduct }, 1);

            if (data1.Count != 0 && data1[0].Inventory - cant >= 0)
            {
                if (data2.Count == 0)
                {
                    ArbolB<OfficeProduct>.InsertarArbol(new OfficeProduct { IdOffice = idO2, IdProduct = idProduct, Inventory = cant });
                }
                else
                {
                    data2[0].Inventory = data2[0].Inventory + cant;
                    ArbolB<OfficeProduct>.Modificar(data2[0], new string[2] { data2[0].Inventory.ToString(), string.Empty }, new Modify(OfficeProduct.Alter));
                }
                data1[0].Inventory = data1[0].Inventory - cant;
                ArbolB<OfficeProduct>.Modificar(data1[0], new string[2] { data1[0].Inventory.ToString(), string.Empty }, new Modify(OfficeProduct.Alter));
            }
        }
        #endregion
    }
}
