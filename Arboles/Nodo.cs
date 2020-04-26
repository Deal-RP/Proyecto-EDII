using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Arboles
{
    public class Nodo<T>
    {
        #region CONSTRUCTOR
        public Nodo( int padre)
        {
            if (padre == 0)
            {
                cantV = (4 * (DatosArboles.Instance.Grado - 1)) / 3;
            }
            else
            {
                cantV = DatosArboles.Instance.Grado - 1;
            }

            this.Padre = padre;
        }
        #endregion

        #region ATRIBUTOS
        public int indice;

        public int Padre;

        public int cantV;

        public List<int> Hijos = new List<int>();

        public List<T> Valores = new List<T>();

        static int lenght = 300;
        #endregion

        #region CONVERT
        static string bufferToString(byte[] linea)
        {
            var aux = string.Empty;

            foreach (var caracter in linea)
            {
                aux += (char)caracter;
            }
            return aux;
        }

        static byte[] stringToBuffer(string linea)
        {
            var aux = new List<byte>();
            foreach (var caracter in linea)
            {
                aux.Add((byte)caracter);
            }
            return aux.ToArray();
        }
        #endregion

        #region STRING -> NODO
        public static Nodo<T> StringToNodo(int posicion)
        {
            var cantHijos = ((4 * (DatosArboles.Instance.Grado - 1)) / 3) + 1;
            var cantCaracteres = 8 + (4 * cantHijos) + (lenght * (cantHijos - 1));
            //Lee la linea de archivo de texto que contiene el nodo
            var buffer = new byte[cantCaracteres];
            using (var fs = new FileStream(DatosArboles.Instance.path, FileMode.OpenOrCreate))
            {
                fs.Seek((posicion - 1) * cantCaracteres + 15, SeekOrigin.Begin);
                fs.Read(buffer, 0, cantCaracteres);
            }

            var nodeString = bufferToString(buffer);
            //Divide los valores para llenar el nodo que se va a utilizar
            var values = new List<string>();
            for (int i = 0; i < cantHijos + 2; i++)
            {
                values.Add(nodeString.Substring(0, 4));
                nodeString = nodeString.Substring(4);
            }

            for (int i = 0; i < cantHijos - 1; i++)
            {
                values.Add(nodeString.Substring(0, lenght));
                nodeString = nodeString.Substring(lenght);
            }

            var NodoSalida = new Nodo<T>(Convert.ToInt32(values[1]));

            NodoSalida.indice = Convert.ToInt32(values[0]);
            for (int i = 2; i < (2 + cantHijos); i++)
            {
                if (values[i].Trim() != "-")
                {
                    NodoSalida.Hijos.Add(Convert.ToInt32(values[i]));
                }
            }

            for (int i = (2 + cantHijos); i < (1 + (2 * cantHijos)); i++)
            {
                if (values[i].Trim() != "-")
                {
                    NodoSalida.Valores.Add((T)DatosArboles.Instance.ObtenerNodo.DynamicInvoke(values[i]));
                }
            }

            return NodoSalida;
        }
        #endregion

        #region NODO -> STRING
        public void NodoToString()
        {
            string hijos = string.Empty;
            string datos = string.Empty;

            var cantHijos = ((4 * (DatosArboles.Instance.Grado - 1)) / 3) + 1;

            foreach (var item in Hijos)
            {
                hijos += item.ToString("0000;-0000");
            }
            for (int i = Hijos.Count; i < cantHijos; i++)
            {
                hijos += string.Format("{0,-4}", "-");
            }

            foreach (var item in Valores)
            {
                datos += Convert.ToString(DatosArboles.Instance.ObtenerString.DynamicInvoke(item));
            }

            for (int i = Valores.Count; i < (cantHijos - 1); i++)
            {
                datos += string.Format("{0,-300}", "-");
            }
            var p = datos.Length;
            var NodoChar = ($"{indice.ToString("0000;-0000")}{Padre.ToString("0000;-0000")}{hijos}{datos}");
            var cantCaracteres = 8 + (4 * cantHijos) + (lenght * (cantHijos - 1));
            using (var fs = new FileStream(DatosArboles.Instance.path, FileMode.OpenOrCreate))
            {
                fs.Seek((indice - 1) * cantCaracteres + 15, SeekOrigin.Begin);
                fs.Write(stringToBuffer(NodoChar), 0, cantCaracteres);
            }
        }
        #endregion
    }
}
