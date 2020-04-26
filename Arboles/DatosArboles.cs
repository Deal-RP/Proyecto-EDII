using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arboles
{
    public class DatosArboles
    {
        #region INSTANCIA
        private static DatosArboles _instance = null;
        public static DatosArboles Instance
        {
            get
            {
                if (_instance == null) _instance = new DatosArboles();
                return _instance;
            }
        }
        #endregion

        public int Grado;
        public Delegate ObtenerNodo;
        public Delegate ObtenerString;
        public string path;
        public int key;
    }
}
