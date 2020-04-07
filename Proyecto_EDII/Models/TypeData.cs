using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Arboles;

namespace Proyecto_EDII.Models
{
    #region PRODUCT
    public class ProductData : IComparable
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

        public int CompareTo(object obj)
        {
            return this.ID.CompareTo(((ProductData)obj).ID);
        }

        public static string ObjectToString(object Nuevo)
        {
            var Actual = (ProductData)Nuevo;
            Actual.Name = Actual.Name == null ? string.Empty : Actual.Name;
            Actual.Address = Actual.Address == null ? string.Empty : Actual.Address;
            var aux = $"{string.Format("{0,-100}", Actual.ID.ToString())}=/=/={string.Format("{0,-100}", Actual.Name)}=/=/={string.Format("{0,-100}", Actual.Address)}";

            return SDES.CifradoDecifrado(DatosArboles.Instance.key, aux, true);
        }

        public static ProductData StringToProducto(string info)
        {
            var aux = SDES.CifradoDecifrado(DatosArboles.Instance.key, info, false);
            var infoSeparada = aux.Split(new string[] { "=/=/=" }, StringSplitOptions.RemoveEmptyEntries);
            return new ProductData() { ID = Convert.ToInt32(infoSeparada[0].Trim()), Name = infoSeparada[1].Trim(), Address = infoSeparada[2].Trim()};
        }
    }
    #endregion

    #region OFFICE
    public class OfficeData : IComparable
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }

        public int CompareTo(object obj)
        {
            return this.ID.CompareTo(((OfficeData)obj).ID);
        }

        public static string ObjectToString(object Nuevo)
        {
            var Actual = (OfficeData)Nuevo;
            Actual.Name = Actual.Name == null ? string.Empty : Actual.Name;
            var aux = $"{string.Format("{0,-100}", Actual.ID.ToString())}=/=/={string.Format("{0,-100}", Actual.Name)}=/=/={string.Format("{0,-100}", Actual.Price.ToString())}";
            
            return SDES.CifradoDecifrado(DatosArboles.Instance.key, aux, true);
        }

        public static OfficeData StringToOffice(string info)
        {
            var aux = SDES.CifradoDecifrado(DatosArboles.Instance.key, info, false);
            var infoSeparada = aux.Split(new string[] { "=/=/=" }, StringSplitOptions.RemoveEmptyEntries);
            return new OfficeData() { ID = Convert.ToInt32(infoSeparada[0].Trim()), Name = infoSeparada[1].Trim(), Price = Convert.ToDouble(infoSeparada[2].Trim()) };
        }
    }
    #endregion

    #region OFFICE-PRODCUT
    public class OfficeProduct : IComparable
    {
        public int ID { get; set; }
        public int IdProduct { get; set; }
        public int Inventory { get; set; }

        public int CompareTo(object obj)
        {
            return this.ID.CompareTo(((OfficeProduct)obj).ID);
        }

        public static string ObjectToString(object Nuevo)
        {
            var Actual = (OfficeProduct)Nuevo;
            var aux = $"{string.Format("{0,-100}", Actual.ID.ToString())}=/=/={string.Format("{0,-100}", Actual.IdProduct.ToString())}=/=/={string.Format("{0,-100}", Actual.Inventory.ToString())}";
            
            return SDES.CifradoDecifrado(DatosArboles.Instance.key, aux, true);
        }

        public static OfficeProduct StringToOffice(string info)
        {
            var aux = SDES.CifradoDecifrado(DatosArboles.Instance.key, info, false);
            var infoSeparada = aux.Split(new string[] { "=/=/=" }, StringSplitOptions.RemoveEmptyEntries);
            return new OfficeProduct() { ID = Convert.ToInt32(infoSeparada[0].Trim()), IdProduct = Convert.ToInt32(infoSeparada[1].Trim()), Inventory = Convert.ToInt32(infoSeparada[2].Trim()) };
        }
    }
    #endregion
}