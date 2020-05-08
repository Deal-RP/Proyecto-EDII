using System;
using System.Collections.Generic;
using Arboles;
using System.IO;

namespace Proyecto_EDII.Models
{
    #region OFFICE
    public class OfficeData : IComparable
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

        public int CompareTo(object obj)
        {
            return this.ID.CompareTo(((OfficeData)obj).ID);
        }

        public static string ObjectToString(object Nuevo)
        {
            var Actual = (OfficeData)Nuevo;
            Actual.Name = Actual.Name == null ? string.Empty : Actual.Name;
            Actual.Address = Actual.Address == null ? string.Empty : Actual.Address;
            return $"{string.Format("{0,-100}", Actual.ID.ToString())}{SDES.CifradoDecifrado(string.Format("{0,-100}", Actual.Name), true)}{SDES.CifradoDecifrado(string.Format("{0,-100}", Actual.Address), true)}";
        }

        public static OfficeData StringToObject(string info)
        {
            var infoSeparada = new List<string>();
            for (int i = 0; i < 3; i++)
            {
                infoSeparada.Add(info.Substring(0, 100));
                info = info.Substring(100);
            }

            return new OfficeData()
            {
                ID = Convert.ToInt32(infoSeparada[0].Trim()),
                Name = SDES.CifradoDecifrado(infoSeparada[1], false).Trim(),
                Address = SDES.CifradoDecifrado(infoSeparada[2], false).Trim()
            };
        }

        public static OfficeData Alter(object info, string[] freshInfo)
        {
            var originInfo = (OfficeData)info;
            originInfo.Name = freshInfo[0] == null ? originInfo.Name : freshInfo[0];
            originInfo.Address = freshInfo[1] == null ? originInfo.Address : freshInfo[1];
            return originInfo;
        }
    }
    #endregion

    #region PRODUCT
    public class ProductData : IComparable
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }

        public int CompareTo(object obj)
        {
            return this.ID.CompareTo(((ProductData)obj).ID);
        }

        public static string ObjectToString(object Nuevo)
        {
            var Actual = (ProductData)Nuevo;
            Actual.Name = Actual.Name == null ? string.Empty : Actual.Name;
            return $"{string.Format("{0,-100}", Actual.ID.ToString())}{SDES.CifradoDecifrado(string.Format("{0,-100}", Actual.Name), true)}{SDES.CifradoDecifrado(string.Format("{0,-100}", Actual.Price.ToString()), true)}";
        }

        public static ProductData StringToObject(string info)
        {
            var infoSeparada = new List<string>();
            for (int i = 0; i < 3; i++)
            {
                infoSeparada.Add(info.Substring(0, 100));
                info = info.Substring(100);
            }

            var auxPrice = 0.00;
            double.TryParse(SDES.CifradoDecifrado(infoSeparada[2], false).Trim(), out auxPrice);

            return new ProductData()
            {
                ID = Convert.ToInt32(infoSeparada[0].Trim()),
                Name = SDES.CifradoDecifrado(infoSeparada[1], false).Trim(),
                Price = auxPrice
            };
        }

        public static ProductData Alter(object info, string[] freshInfo)
        {
            var originInfo = (ProductData)info;
            originInfo.Name = freshInfo[0] == null ? originInfo.Name : freshInfo[0];
            originInfo.Price = freshInfo[1] == null ? originInfo.Price : Convert.ToDouble(freshInfo[1]);
            return originInfo;
        }

        public static void InsertCSV(Stream info)
        {
            using (var archive = new StreamReader(info))
            {
                var line = string.Empty;
                while ((line = archive.ReadLine()) != null)
                {
                    var entreComillas = false;
                    var partes = new List<string>();
                    var aux = string.Empty;
                    foreach (var caracter in line)
                    {
                        if (entreComillas)
                        {
                            if (caracter == '\"')
                            {
                                entreComillas = false;
                            }
                            else
                            {
                                aux += caracter;
                            }
                        }
                        else
                        {
                            if (caracter == ',')
                            {
                                partes.Add(aux);
                                aux = string.Empty;
                            }
                            else if (caracter == '\"')
                            {
                                entreComillas = true;
                            }
                            else
                            {
                                aux += caracter;
                            }
                        }
                    }
                    if (aux != string.Empty)
                    {
                        partes.Add(aux);
                    }

                    ArbolB<ProductData>.InsertarArbol(new ProductData { ID = ArbolB<ProductData>.newID(), Name = partes[0], Price = Convert.ToDouble(partes[1]) });
                }
            }
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

        public static OfficeProduct Alter(object info, string[] freshInfo)
        {
            var originInfo = (OfficeProduct)info;
            originInfo.Inventory = Convert.ToInt32(freshInfo[0]);
            return originInfo;
        }
    }
    #endregion
}