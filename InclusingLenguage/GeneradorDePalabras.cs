using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InclusingLenguage
{
    public  class GeneradorDePalabras
    {
        

        public static string GenerarOracionAleatoria()
        {
            string[] sujetos = { "El niño", "La mujer", "Mi amigo", "Un gato", "La maestra" };
            string[] verbos = { "come", "lee", "juega", "escribe", "observa" };
            string[] complementos = { "una manzana", "un libro", "en el parque", "una carta", "la televisión" };


            Random rnd = new Random();
            string sujeto = sujetos[rnd.Next(sujetos.Length)];
            string verbo = verbos[rnd.Next(verbos.Length)];
            string complemento = complementos[rnd.Next(complementos.Length)];

            return $"{sujeto} {verbo} {complemento}.";
        }

    }
}
