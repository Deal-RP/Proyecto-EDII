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
            return $"{string.Format("{0,-100}", Actual.ID.ToString())}{SDES.CifradoDecifrado(string.Format("{0,-100}", Actual.Name), true)}{SDES.CifradoDecifrado(string.Format("{0,-100}",Actual.Address), true)}";
        }

        public static ProductData StringToObject(string info)
        {
            var infoSeparada = new List<string>();
            for (int i = 0; i < 3; i++)
            {
                infoSeparada.Add(info.Substring(0, 100));
                info = info.Substring(100);
            }

            return new ProductData() { 
                ID = Convert.ToInt32(infoSeparada[0].Trim()), 
                Name = SDES.CifradoDecifrado(infoSeparada[1], false).Trim(), 
                Address = SDES.CifradoDecifrado(infoSeparada[2], false).Trim()
            };
        }

        public static ProductData Alter (ProductData originInfo , ProductData freshInfo)
        {
            originInfo.Name = freshInfo.Name == null ? originInfo.Name : freshInfo.Name;
            originInfo.Address = freshInfo.Address == null ? originInfo.Address : freshInfo.Address;
            return originInfo;
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
            return $"{string.Format("{0,-100}", Actual.ID.ToString())}{SDES.CifradoDecifrado(string.Format("{0,-100}", Actual.Name), true)}{SDES.CifradoDecifrado(string.Format("{0,-100}", Actual.Price.ToString()), true)}";
        }

        public static OfficeData StringToObject(string info)
        {
            var infoSeparada = new List<string>();
            for (int i = 0; i < 3; i++)
            {
                infoSeparada.Add(info.Substring(0, 100));
                info = info.Substring(100);
            }

            var auxPrice = 0.00;
            double.TryParse(SDES.CifradoDecifrado(infoSeparada[2], false).Trim(), out auxPrice);

            return new OfficeData() {
                ID = Convert.ToInt32(infoSeparada[0].Trim()),
                Name = SDES.CifradoDecifrado(infoSeparada[1], false).Trim(),
                Price = auxPrice
            };
        }

        public static OfficeData Alter(OfficeData originInfo, OfficeData freshInfo)
        {
            originInfo.Name = freshInfo.Name == null ? originInfo.Name : freshInfo.Name;
            originInfo.Price = freshInfo.Price == null ? originInfo.Price : freshInfo.Price;
            return originInfo;
        }
    }
    #endregion

    #region OFFICE-PRODUCT
    public class OfficeProduct : IComparable
    {
        //ID = IdOffice-IdProduct 
        public int IdOffice { get; set; }
        public int IdProduct { get; set; }
        public int Inventory { get; set; }

        public int CompareTo(object obj)
        {
            return $"{this.IdOffice}-{this.IdProduct}".CompareTo($"{((OfficeProduct)obj).IdOffice}-{((OfficeProduct)obj).IdProduct}");
        }

        public static string ObjectToString(object Nuevo)
        {
            var Actual = (OfficeProduct)Nuevo;
            return $"{string.Format("{0,-100}", Actual.IdOffice.ToString())}{string.Format("{0,-100}", Actual.IdProduct.ToString())}{SDES.CifradoDecifrado(string.Format("{0,-100}", Actual.Inventory.ToString()), true)}";
        }

        public static OfficeProduct StringToObject(string info)
        {
            var infoSeparada = new List<string>();
            for (int i = 0; i < 3; i++)
            {
                infoSeparada.Add(info.Substring(0, 100));
                info = info.Substring(100);
            }

            var auxInvent = 0;
            int.TryParse(SDES.CifradoDecifrado(infoSeparada[2], false), out auxInvent);

            return new OfficeProduct()
            {
                IdOffice = Convert.ToInt32(infoSeparada[0].Trim()),
                IdProduct = Convert.ToInt32(infoSeparada[1].Trim()),
                Inventory = auxInvent
            };
        }

        public static OfficeProduct Alter(OfficeProduct originInfo, OfficeProduct freshInfo)
        {
            originInfo.Inventory = freshInfo.Inventory == null ? originInfo.Inventory : freshInfo.Inventory;
            return originInfo;
        }
    }
    #endregion
}