using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml;
using System.Collections.Generic;
using Konz.MyMovies.Core;

namespace Konz.MyMovies.Core.Cinepolis
{
    public class DataExtractor
    {
        public string FilePath { get; set; }
        Action<CinepolisData> OnComplete;
        public DataExtractor()
        {
            FilePath = @"http://www.cinepolis.com.mx/Cartelera/XMLCinepolis/{0}_{1}.xml";
        }

        public void GetData(string cityCode, DateTime date, Action<CinepolisData> OnComplete)
        {
            this.OnComplete = OnComplete;
            var wc = new WebClient();
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(GetCinepolisFileComplete);
            wc.DownloadStringAsync(new Uri(string.Format(FilePath, cityCode, date.ToString("yyyyMMdd"))), date);
        }

        void GetCinepolisFileComplete(object sender, DownloadStringCompletedEventArgs e)
        {
            var reader = XmlReader.Create(new StringReader(e.Result));
            var data = new CinepolisData();
            data.Fecha = (DateTime)e.UserState;

            while (reader.Read() && reader.Name != "cinepolis" || reader.NodeType != XmlNodeType.EndElement)
            {
                if (reader.Name.Equals("vigencia"))
                {
                    data.Vigencia = ReadVigencia(reader);
                }

                if (reader.Name.Equals("pelicula"))
                {                    
                    data.Peliculas.Add(ReadPelicula(reader));
                }
                    
                if (reader.Name.Equals("complejo"))
                {
                    data.Complejos.Add(ReadComplejo(reader));
                }

                if (reader.Name.Equals("cartelera"))
                {
                    data.Carteleras.Add(ReadCartelera(reader));
                }
            }
            
            OnComplete(data);

        }

        private Cartelera ReadCartelera(XmlReader reader)
        {
            var c = new Cartelera();
            while (reader.Read() && reader.Name != "cartelera" || reader.NodeType != XmlNodeType.EndElement)
            {
                string val;
                if (reader.Name == "cartelera" && reader.NodeType != XmlNodeType.EndElement && GetAttribute(reader, "nombre", out val))
                    c.CiudadNombre = val;                    
                if (GetValue(reader, "ciudad", out val))
                    c.CiudadCode = val;
                if (GetValue(reader, "complejoid", out val))
                    c.ComplejoCode = val;
                if (GetValue(reader, "pelicula", out val))
                    c.PeliculaCode = val;
                if (GetValue(reader, "estreno", out val))
                    c.EsEstreno = val;
                if (reader.Name == "sala" && reader.NodeType != XmlNodeType.EndElement)
                    c.Salas.Add(ReadSalaCartelera(reader));
            }
            return c;
        }

        private SalaCartelera ReadSalaCartelera(XmlReader reader)
        {
            var s = new SalaCartelera();
            string val;
            if (GetAttribute(reader, "numero", out val))
                s.Code = val;
            while (reader.Read() && reader.Name != "sala" || reader.NodeType != XmlNodeType.EndElement)
            {
                if (reader.Name == "horario" && reader.NodeType != XmlNodeType.EndElement)
                {
                    string id, hr;
                    if (GetAttribute(reader, "idshowtime", out id) && GetAttribute(reader, "hr", out hr))
                        s.Horarios.Add(id, hr);
                }
            }
            return s;
        }

        private Complejo ReadComplejo(XmlReader reader)
        {
            var c = new Complejo();
            string val;
            while (reader.Read() && reader.Name != "complejo" || reader.NodeType != XmlNodeType.EndElement)
            {
                if (GetValue(reader, "complejoid", out val))
                    c.Code = val;
                if (GetValue(reader, "ciudad", out val))
                    c.CiudadCode = val;
                if (GetValue(reader, "nombre", out val))
                    c.Nombre = val;
                if (GetValue(reader, "orden", out val))
                    c.Orden = val;
                if (reader.Name == "sala" && reader.NodeType != XmlNodeType.EndElement)
                    c.Salas.Add(ReadSalaComplejo(reader));
            }
            return c;
        }

        private SalaComplejo ReadSalaComplejo(XmlReader reader)
        {
            var s = new SalaComplejo();
            string val;
            while (reader.Read() && reader.Name != "sala" || reader.NodeType != XmlNodeType.EndElement)
            {
                if (GetValue(reader, "numerosala", out val))
                    s.Code = val;
                if (GetValue(reader, "orden", out val))
                    s.Orden = val;
                if (GetValue(reader, "sonido", out val))
                    s.Sonidos.Add(val);
            }
            return s;
        }

        private Pelicula ReadPelicula(XmlReader reader)
        {
            var p = new Pelicula();
            string val;
            if (GetAttribute(reader, "peliculaid", out val))
                p.Code = val;
            if (GetAttribute(reader, "nombre", out val))
                p.Nombre = val;
            while (reader.Read() && reader.Name != "pelicula" || reader.NodeType != XmlNodeType.EndElement)
            {
                if (GetValue(reader, "clasificacion", out val))
                    p.Clasificacion = val;
                if (GetValue(reader, "imagencartel", out val))
                    p.Cartel = val;
                if (GetValue(reader, "sinopsis", out val))
                    p.Sinopsis = val;
                if (GetValue(reader, "actores", out val))
                    p.Actores = val;
                if (GetValue(reader, "calificacion", out val))
                    p.Calificacion = val;
            }
            return p;
        }

        private Vigencia ReadVigencia(XmlReader reader)
        {
            var v = new Vigencia();
            while (reader.Read() && reader.Name != "vigencia" || reader.NodeType != XmlNodeType.EndElement)
            {
                if (reader.Name == "de" && reader.NodeType != XmlNodeType.EndElement)
                    v.de = DateTime.Parse(reader.GetAttribute("fecha"));
                if (reader.Name == "hasta" && reader.NodeType != XmlNodeType.EndElement)
                    v.hasta = DateTime.Parse(reader.GetAttribute("fecha"));
            }
            return v;
        }

        private bool GetAttribute(XmlReader reader, string attributeName, out string result)
        {
            result = reader.GetAttribute(attributeName);
            return !string.IsNullOrWhiteSpace(result);
        }

        private bool GetValue(XmlReader reader, string elementName, out string result)
        {
            result = string.Empty;
            if (reader.Name == elementName && reader.NodeType != XmlNodeType.EndElement)
                result = reader.ReadElementContentAsString();
            return !string.IsNullOrWhiteSpace(result);
        }

    }
}
