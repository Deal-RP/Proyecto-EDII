using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Arboles
{
    public class ArbolB<T> where T : IComparable
    {
        static List<T> TemporalRecorrido;

        #region MANEJO METADATA
        static int[] ManejarMeta(int[] meta = null)
        {
            var buffer = new byte[14];
            using (var fs = new FileStream(DatosArboles.Instance.path, FileMode.OpenOrCreate))
            {
                if (meta == null)
                {
                    fs.Read(buffer, 0, 14);
                    var values = Encoding.UTF8.GetString(buffer).Split('|');

                    return new int[3] { Convert.ToInt32(values[0]), Convert.ToInt32(values[1]), Convert.ToInt32(values[2]) };
                }

                var linea = string.Empty;
                foreach (var item in meta)
                {
                    linea = $"{linea}{item.ToString("0000;-0000")}|";
                }
                fs.Write(Encoding.UTF8.GetBytes(linea.ToCharArray()), 0, 15);
            }
            return null;
        }
        #endregion

        #region MANEJO INICIAL
        public static void IniciarArbol(string nombre, Delegate ONodo, Delegate OString)
        {
            var grado = 7;
            DatosArboles.Instance.path = $"Datos\\{nombre}.txt";

            if (!Directory.Exists("Datos"))
            {
                Directory.CreateDirectory("Datos");
            }
            if (!File.Exists(DatosArboles.Instance.path))
            {
                string text = $"{grado.ToString("0000;-0000")}|0000|0001|";
                File.WriteAllText(DatosArboles.Instance.path, text);
            }
            DatosArboles.Instance.ObtenerNodo = ONodo;
            DatosArboles.Instance.ObtenerString = OString;
        }
        #endregion

        #region INSERTAR
        public static void InsertarArbol(T info)
        {
            /*METADATA
             * [0] = Grado
             * [1] = Raiz
             * [2] = Posicion siguiente
            */
            var metaData = ManejarMeta();
            DatosArboles.Instance.Grado = metaData[0];

            if (metaData[1] == 0)
            {
                metaData[1]++;
                metaData[2]++;

                var nodo = new Nodo<T>(0) { indice = 1, Valores = new List<T> { info }, Hijos = new List<int>() };
                nodo.NodoToString();

                ManejarMeta(metaData);
            }
            else
            {
                var carry = false;
                var carryHijo = false;
                var IsFirst = true;
                var Hijo = 0;
                var posHijo = 0;
                Insertar(metaData[1], ref info, ref Hijo, ref posHijo, ref carry, ref carryHijo, ref IsFirst);
            }
        }

        static void Insertar(int indiceActual, ref T info, ref int hijo, ref int posHijo, ref bool TieneCarry, ref bool TieneHijo, ref bool first)
        {
            var metaData = ManejarMeta();
            var Actual = Nodo<T>.StringToNodo(indiceActual);

            var pos = 0;
            if (Actual.Hijos.Count == 0)
            {
                while (pos < Actual.Valores.Count && Actual.Valores[pos].CompareTo(info) == -1)
                {
                    pos++;
                }
                Actual.Valores.Insert(pos, info);
            }
            else
            {
                while (pos < Actual.Valores.Count && Actual.Valores[pos].CompareTo(info) == -1)
                {
                    pos++;
                }
                Insertar(Actual.Hijos[pos], ref info, ref hijo, ref posHijo, ref TieneCarry, ref TieneHijo, ref first);
            }

            if (TieneCarry)
            {
                pos = 0;
                Actual = Nodo<T>.StringToNodo(Actual.indice);
                while (pos < Actual.Valores.Count && Actual.Valores[pos].CompareTo(info) == -1)
                {
                    pos++;
                }
                Actual.Valores.Insert(pos, info);
                if (TieneHijo)
                {
                    Actual.Hijos.Insert(posHijo, hijo);
                    TieneHijo = false;
                }
                TieneCarry = false;
            }

            if (Actual.Valores.Count == Actual.cantV + 1)
            {
                if (Actual.Hijos.Count == 0 && Actual.Padre != 0)
                {
                    var padre = Nodo<T>.StringToNodo(Actual.Padre);
                    var indiceAct = padre.Hijos.IndexOf(Actual.indice);
                    var indiceHer = BuscarRotar(padre, indiceAct, metaData);
                    //var indiceDato = indiceAct == 0 ? 0 : indiceAct - 1;
                    T datoTemporal = Actual.Valores[0];
                    var LlevaCarry = false;

                    if (indiceHer < indiceAct)
                    {
                        var indiceDato = indiceAct == 0 ? 0 : indiceAct - 1;
                        for (int i = indiceAct; i >= indiceHer; i--)
                        {
                            if (padre.Hijos[i] != Actual.indice)
                            {
                                Actual = Nodo<T>.StringToNodo(padre.Hijos[i]);
                            }
                            if (LlevaCarry)
                            {
                                padre.Valores.Insert(indiceDato, datoTemporal);
                                datoTemporal = padre.Valores[indiceDato + 1];
                                padre.Valores.RemoveAt(indiceDato + 1);
                                padre.NodoToString();
                                Actual.Valores.Add(datoTemporal);
                                datoTemporal = Actual.Valores[0];
                                if (i != indiceHer)
                                {
                                    Actual.Valores.RemoveAt(0);
                                }
                                indiceDato--;
                            }
                            else
                            {
                                datoTemporal = Actual.Valores[0];
                                Actual.Valores.RemoveAt(0);
                            }

                            Actual.NodoToString();
                            LlevaCarry = true;
                        }
                    }
                    else if (indiceHer > indiceAct)
                    {
                        var indiceDato = indiceAct == padre.Hijos.Count() - 1 ? indiceAct - 1 : indiceAct;
                        for (int i = indiceAct; i <= indiceHer; i++)
                        {
                            if (padre.Hijos[i] != Actual.indice)
                            {
                                Actual = Nodo<T>.StringToNodo(padre.Hijos[i]);
                            }
                            if (LlevaCarry)
                            {
                                padre.Valores.Insert(indiceDato, datoTemporal);
                                datoTemporal = padre.Valores[indiceDato + 1];
                                padre.Valores.RemoveAt(indiceDato + 1);
                                padre.NodoToString();
                                Actual.Valores.Insert(0, datoTemporal);
                                datoTemporal = Actual.Valores[Actual.Valores.Count - 1];
                                if (i != indiceHer)
                                {
                                    Actual.Valores.RemoveAt(Actual.Valores.Count - 1);
                                }
                                indiceDato++;
                            }
                            else
                            {
                                datoTemporal = Actual.Valores[Actual.Valores.Count - 1];
                                Actual.Valores.RemoveAt(Actual.Valores.Count - 1);
                            }

                            Actual.NodoToString();
                            LlevaCarry = true;
                        }
                    }
                    else
                    {
                        var indiceDato = indiceAct == 0 ? 0 : indiceAct - 1;
                        indiceHer = indiceAct - 1 >= 0 ? indiceAct - 1 : indiceAct + 1;
                        var hermano = Nodo<T>.StringToNodo(padre.Hijos[indiceHer]);
                        var listaTemporal = new List<T>();
                        var temporal = new Nodo<T>(padre.indice) { indice = metaData[2], Valores = new List<T>(), Hijos = new List<int>() };

                        if (indiceAct - 1 == indiceHer)
                        {
                            foreach (var item in hermano.Valores)
                            {
                                listaTemporal.Add(item);
                            }
                            listaTemporal.Add(padre.Valores[indiceDato]);
                            foreach (var item in Actual.Valores)
                            {
                                listaTemporal.Add(item);
                            }
                            var cantDividida = (listaTemporal.Count - 2) / 3;

                            temporal.Valores.AddRange(listaTemporal.GetRange(0, cantDividida));
                            listaTemporal.RemoveRange(0, cantDividida);

                            padre.Valores.RemoveAt(indiceDato);
                            padre.Valores.Insert(indiceDato, listaTemporal[0]);
                            listaTemporal.RemoveAt(0);

                            hermano.Valores.Clear();
                            hermano.Valores.AddRange(listaTemporal.GetRange(0, cantDividida));
                            listaTemporal.RemoveRange(0, cantDividida);

                            padre.Valores.Insert(indiceDato + 1, listaTemporal[0]);
                            listaTemporal.RemoveAt(0);

                            Actual.Valores.Clear();
                            Actual.Valores.AddRange(listaTemporal.GetRange(0, cantDividida));

                            indiceHer = indiceAct - 1 >= 0 ? indiceAct - 1 : indiceAct + 2;
                            padre.Hijos.Insert(indiceHer, temporal.indice);
                        }
                        else
                        {
                            foreach (var item in Actual.Valores)
                            {
                                listaTemporal.Add(item);
                            }
                            listaTemporal.Add(padre.Valores[indiceDato]);
                            foreach (var item in hermano.Valores)
                            {
                                listaTemporal.Add(item);
                            }
                            var cantDividida = (listaTemporal.Count - 2) / 3;

                            Actual.Valores.Clear();
                            Actual.Valores.AddRange(listaTemporal.GetRange(0, cantDividida));
                            listaTemporal.RemoveRange(0, cantDividida);

                            padre.Valores.RemoveAt(indiceDato);
                            padre.Valores.Insert(indiceDato, listaTemporal[0]);
                            listaTemporal.RemoveAt(0);

                            hermano.Valores.Clear();
                            hermano.Valores.AddRange(listaTemporal.GetRange(0, cantDividida));
                            listaTemporal.RemoveRange(0, cantDividida);

                            padre.Valores.Insert(indiceDato + 1, listaTemporal[0]);
                            listaTemporal.RemoveAt(0);

                            temporal.Valores.AddRange(listaTemporal.GetRange(0, cantDividida));

                            padre.Hijos.Insert(indiceHer + 1, temporal.indice);
                        }

                        if (padre.Valores.Count > padre.cantV)
                        {
                            info = padre.Valores[0];
                            padre.Valores.RemoveAt(0);
                            posHijo = padre.Hijos.IndexOf(temporal.indice);
                            padre.Hijos.RemoveAt(posHijo);
                            hijo = temporal.indice;
                            TieneCarry = true;
                            TieneHijo = true;
                        }

                        temporal.NodoToString();
                        padre.NodoToString();
                        hermano.NodoToString();
                        metaData[2]++;
                        ManejarMeta(metaData);
                    }
                }
                else
                {
                    metaData = ManejarMeta();
                    var posMedia = Actual.Valores.Count % 2 == 0 ? (Actual.Valores.Count - 1) / 2 : Actual.Valores.Count / 2;
                    var padreHermano = Actual.Padre == 0 ? metaData[2] + 1 : Actual.Padre;
                    var Hermano = new Nodo<T>(padreHermano) { indice = metaData[2], Valores = Actual.Valores.GetRange(0, posMedia) };

                    metaData[2]++;

                    if (Actual.Hijos.Count != 0)
                    {
                        Hermano.Hijos = Actual.Hijos.GetRange(0, posMedia + 1);
                        Actual.Hijos.RemoveRange(0, posMedia + 1);

                        foreach (var indiceHijo in Hermano.Hijos)
                        {
                            var Hijo = Nodo<T>.StringToNodo(indiceHijo);

                            Hijo.Padre = Hermano.indice;
                            Hijo.NodoToString();
                        }
                    }

                    if (Actual.Padre == 0)
                    {
                        var padre = new Nodo<T>(0) { Valores = new List<T> { Actual.Valores[posMedia] }, Hijos = new List<int> { Hermano.indice, Actual.indice }, indice = metaData[2] };
                        metaData[1] = metaData[2];
                        metaData[2]++;
                        padre.NodoToString();
                        Hermano.Padre = padre.indice;
                        Actual.Padre = padre.indice;
                    }
                    else
                    {
                        var padre = Nodo<T>.StringToNodo(Actual.Padre);

                        info = Actual.Valores[posMedia];
                        pos = 0;
                        while (pos < padre.Valores.Count && padre.Valores[pos].CompareTo(info) == -1)
                        {
                            pos++;
                        }
                        padre.Valores.Insert(pos, info);
                        padre.Hijos.Insert(padre.Hijos.IndexOf(Actual.indice), Hermano.indice);

                        if (padre.Valores.Count > padre.cantV)
                        {
                            info = padre.Valores[0];
                            padre.Valores.RemoveAt(0);
                            posHijo = padre.Hijos.IndexOf(Hermano.indice);
                            padre.Hijos.RemoveAt(posHijo);
                            hijo = Hermano.indice;
                            TieneCarry = true;
                            TieneHijo = true;
                        }

                        padre.NodoToString();

                        //Primer metodo

                        //while (pos < padre.Valores.Count && padre.Valores[pos].CompareTo(info) == -1)
                        //{
                        //    pos++;
                        //}
                        //padre.Valores.Insert(pos, info);

                        //if (padre.Valores.Count <= padre.cantV)
                        //{
                        //    padre.Hijos.Insert(padre.Hijos.IndexOf(Actual.indice), Hermano.indice);
                        //    padre.NodoToString();
                        //}
                        //else
                        //{
                        //    TieneCarry = true;
                        //}
                    }

                    Actual.Valores.RemoveRange(0, posMedia + 1);
                    Actual.NodoToString();
                    Hermano.NodoToString();
                    ManejarMeta(metaData);
                }
            }
            if (first)
            {
                Actual.NodoToString();
                first = false;
            }
        }

        static int BuscarRotar(Nodo<T> Padre, int indiceLista, int[] metadata)
        {
            var temporal = new Nodo<T>(1);

            if (indiceLista - 1 >= 0)
            {
                temporal = Nodo<T>.StringToNodo(Padre.Hijos[indiceLista - 1]);
                if (temporal.Valores.Count < temporal.cantV)
                {
                    return indiceLista - 1;
                }
            }
            if (indiceLista + 1 < Padre.Hijos.Count)
            {
                temporal = Nodo<T>.StringToNodo(Padre.Hijos[indiceLista + 1]);
                if (temporal.Valores.Count < temporal.cantV)
                {
                    return indiceLista + 1;
                }
            }
            if (indiceLista - 2 >= 0)
            {
                for (int i = indiceLista - 2; i >= 0; i--)
                {
                    temporal = Nodo<T>.StringToNodo(Padre.Hijos[i]);
                    if (temporal.Valores.Count < temporal.cantV)
                    {
                        return i;
                    }
                }
            }
            if (indiceLista + 2 < Padre.Hijos.Count)
            {
                for (int i = indiceLista + 2; i < Padre.Hijos.Count; i++)
                {
                    temporal = Nodo<T>.StringToNodo(Padre.Hijos[i]);
                    if (temporal.Valores.Count < temporal.cantV)
                    {
                        return i;
                    }
                }
            }
            return indiceLista;
        }
        #endregion

        #region BUSQUEDA
        public static List<T> Recorrido(T info1, T info2, int tipo = 0)
        {
            TemporalRecorrido = new List<T>();
            var metaData = ManejarMeta();
            DatosArboles.Instance.Grado = metaData[0];
            if (metaData[1] != 0)
            {
                var Raiz = Nodo<T>.StringToNodo(metaData[1]);
                var continuar = true;

                switch (tipo)
                {
                    case 0:
                        InOrden(Raiz);
                        break;
                    case 1:
                        Busqueda(Raiz, info1, ref continuar);
                        break;
                    case 2:
                        Busqueda(Raiz, info1, ref continuar);
                        continuar = true;
                        Busqueda(Raiz, info2, ref continuar);
                        break;
                }
            }

            return TemporalRecorrido;
        }

        static void InOrden(Nodo<T> Actual)
        {
            if (Actual.Hijos.Count == 0)
            {
                foreach (var dato in Actual.Valores)
                {
                    TemporalRecorrido.Add(dato);
                }
            }
            else
            {
                var posDato = 1;
                foreach (var hijo in Actual.Hijos)
                { 
                    InOrden(Nodo<T>.StringToNodo(hijo));
                    if (posDato < Actual.Hijos.Count)
                    {
                        TemporalRecorrido.Add(Actual.Valores[posDato - 1]);
                        posDato++;
                    }
                }
            }
        }

        static void Busqueda(Nodo<T> Actual, T info, ref bool continuar)
        {
            var pos = 0;
            while (continuar && pos < Actual.Valores.Count && (Actual.Valores[pos].CompareTo(info) == -1 || Actual.Valores[pos].CompareTo(info) == 0))
            {
                if (Actual.Valores[pos].CompareTo(info) == 0)
                {
                    continuar = false;
                    TemporalRecorrido.Add(Actual.Valores[pos]);
                }
                pos++;
            }
            if (continuar && Actual.Hijos.Count != 0)
            {
                Busqueda(Nodo<T>.StringToNodo(Actual.Hijos[pos]), info, ref continuar);
            }
        }
        #endregion
    }
}
