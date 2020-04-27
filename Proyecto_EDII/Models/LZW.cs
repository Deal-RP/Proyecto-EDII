using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arboles;

namespace Proyecto_EDII.Models
{
    public class LZW
    {
        public static void Comprimir(string nombre)
        {
            Directory.CreateDirectory("temp");
            ArbolB<ProductData>.IniciarArbol(nombre, null, null);
            using (var streamReader = new FileStream($"Datos\\{nombre}.txt", FileMode.Open))
            {
                using (var reader = new BinaryReader(streamReader))
                {
                    using (var streamWriter = new FileStream($"temp\\{nombre}.txt", FileMode.OpenOrCreate))
                    {
                        using (var writer = new BinaryWriter(streamWriter))
                        {
                            var DiccionarioLetras = new Dictionary<string, string>();
                            var bufferLength = 10000;
                            var bytebuffer = new byte[bufferLength];
                            var stringLetra = string.Empty;

                            while (reader.BaseStream.Position != reader.BaseStream.Length)
                            {
                                bytebuffer = reader.ReadBytes(bufferLength);

                                for (int i = 0; i < bytebuffer.Count(); i++)
                                {
                                    stringLetra = Convert.ToString(Convert.ToChar(bytebuffer[i]));

                                    if (!DiccionarioLetras.ContainsKey(stringLetra))
                                    {
                                        var stringnum = Convert.ToString(DiccionarioLetras.Count() + 1, 2);
                                        DiccionarioLetras.Add(stringLetra, stringnum);
                                        stringLetra = string.Empty;
                                    }
                                }
                            }

                            writer.Write(Encoding.UTF8.GetBytes(Convert.ToString(DiccionarioLetras.Count).PadLeft(8, '0').ToCharArray()));

                            foreach (var fila in DiccionarioLetras)
                            {
                                writer.Write(Convert.ToByte(Convert.ToChar(fila.Key[0])));
                            }

                            reader.BaseStream.Position = 0;
                            stringLetra = string.Empty;
                            var anterior = string.Empty;

                            var ListaCaracteres = new List<string>();

                            while (reader.BaseStream.Position != reader.BaseStream.Length)
                            {
                                bytebuffer = reader.ReadBytes(bufferLength);

                                for (int i = 0; i < bytebuffer.Count(); i++)
                                {
                                    stringLetra += Convert.ToString(Convert.ToChar(bytebuffer[i]));

                                    if (!DiccionarioLetras.ContainsKey(stringLetra))
                                    {
                                        var stringnum = Convert.ToString(DiccionarioLetras.Count() + 1, 2);
                                        DiccionarioLetras.Add(stringLetra, stringnum);
                                        ListaCaracteres.Add(DiccionarioLetras[anterior]);
                                        anterior = string.Empty;
                                        anterior += stringLetra.Last();
                                        stringLetra = anterior;
                                    }
                                    else
                                    {
                                        anterior = stringLetra;
                                    }
                                }
                            }

                            ListaCaracteres.Add(DiccionarioLetras[stringLetra]);

                            var cantMaxBits = Math.Log2((float)DiccionarioLetras.Count);
                            cantMaxBits = cantMaxBits % 1 >= 0.5 ? Convert.ToInt32(cantMaxBits) : Convert.ToInt32(cantMaxBits) + 1;

                            writer.Write(Convert.ToByte(cantMaxBits));

                            for (int i = 0; i < ListaCaracteres.Count; i++)
                            {
                                ListaCaracteres[i] = ListaCaracteres[i].PadLeft(Convert.ToInt32(cantMaxBits), '0');
                            }

                            var bufferEscritura = new List<byte>();

                            var aux = string.Empty;

                            foreach (var item in ListaCaracteres)
                            {
                                aux += item;
                                if (aux.Length >= 8)
                                {
                                    var max = aux.Length / 8;
                                    for (int i = 0; i < max; i++)
                                    {
                                        bufferEscritura.Add(Convert.ToByte(Convert.ToInt32(aux.Substring(0, 8), 2)));
                                        aux = aux.Substring(8);
                                    }
                                }
                            }

                            if (aux.Length != 0)
                            {
                                bufferEscritura.Add(Convert.ToByte(Convert.ToInt32(aux.PadRight(8, '0'), 2)));
                            }

                            writer.Write(bufferEscritura.ToArray());

                        }
                    }
                }
            }
        }

        public static void Descomprimir(IFormFile archivo)
        {
            if (File.Exists($"Datos\\{archivo.FileName}"))
            {
                File.Delete($"Datos\\{archivo.FileName}");
            }
            using (var reader = new BinaryReader(archivo.OpenReadStream()))
            {
                using (var streamWriter = new FileStream($"Datos\\{archivo.FileName}", FileMode.OpenOrCreate))
                {
                    using (var writer = new BinaryWriter(streamWriter))
                    {
                        var DiccionarioLetras = new Dictionary<int, string>();
                        var bufferLength = 10000;
                        var bytebuffer = new byte[bufferLength];

                        bytebuffer = reader.ReadBytes(8);
                        var CantidadDiccionario = Convert.ToInt32(Encoding.UTF8.GetString(bytebuffer));

                        for (int i = 0; i < CantidadDiccionario; i++)
                        {
                            bytebuffer = reader.ReadBytes(1);
                            var Letra = Convert.ToChar(bytebuffer[0]).ToString();
                            DiccionarioLetras.Add(DiccionarioLetras.Count() + 1, Letra);
                        }

                        bytebuffer = reader.ReadBytes(1);
                        var cantidadBits = Convert.ToInt32(bytebuffer[0]);

                        var auxActual = string.Empty;
                        var auxPrevio = string.Empty;
                        var aux = string.Empty;
                        var first = true;

                        var bufferEscritura = new List<byte>();

                        while (reader.BaseStream.Position != reader.BaseStream.Length)
                        {
                            bytebuffer = reader.ReadBytes(bufferLength);
                            foreach (var item in bytebuffer)
                            {
                                aux += Convert.ToString(item, 2).PadLeft(8, '0');
                                while (aux.Length >= cantidadBits)
                                {
                                    var nuevoNum = Convert.ToInt32(aux.Substring(0, cantidadBits), 2);
                                    if (nuevoNum != 0)
                                    {
                                        if (first)
                                        {
                                            first = false;
                                            auxPrevio = DiccionarioLetras[nuevoNum];
                                            bufferEscritura.Add(Convert.ToByte(Convert.ToChar(auxPrevio)));
                                        }
                                        else
                                        {
                                            if (nuevoNum > DiccionarioLetras.Count)
                                            {
                                                auxActual = auxPrevio + auxPrevio.First();
                                                DiccionarioLetras.Add(DiccionarioLetras.Count + 1, auxActual);
                                            }
                                            else
                                            {
                                                auxActual = DiccionarioLetras[nuevoNum];
                                                DiccionarioLetras.Add(DiccionarioLetras.Count + 1, $"{auxPrevio}{auxActual.First()}");
                                            }

                                            foreach (var letra in auxActual)
                                            {
                                                bufferEscritura.Add(Convert.ToByte(letra));
                                            }
                                            auxPrevio = auxActual;
                                        }
                                    }
                                    aux = aux.Substring(cantidadBits);
                                }
                            }
                        }
                        writer.Write(bufferEscritura.ToArray());
                    }
                }
            }
        }
    }
}
