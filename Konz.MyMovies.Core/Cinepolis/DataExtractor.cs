using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml;
using Konz.MyMovies.Model;

namespace Konz.MyMovies.Core.Cinepolis
{
    public class DataExtractor
    {
        #region Fields

        const string ShowsRootPath = @"http://www.cinepolis.com.mx/Cartelera/XMLCinepolis/{0}_{1}.xml";
        const string CitiesRootPath = @"http://www.cinepolis.com.mx/widget/aspx/ciudades.aspx";
        const string ImagesRootPath = @"http://www.cinepolis.com.mx/imagenes/peliculas/{0}";
        
        City _city;
        DateTime _date;
        
        Action<City> OnCompleteShows;
        Action<List<City>> OnCompleteCities;

        #endregion

        #region Public Methods

        public void GetCities(Action<List<City>> OnComplete)
        {
            this.OnCompleteCities = OnComplete;
            var wc = new WebClient();
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(GetCinepolisCitiesComplete);            
            wc.DownloadStringAsync(new Uri(CitiesRootPath));
        }

        public void GetCityData(City city, DateTime date, Action<City> OnComplete)
        {
            this._city = city;
            this._date = date;            
            this.OnCompleteShows = OnComplete;

            var wc = new WebClient();
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(GetCinepolisShowsComplete);
            wc.DownloadStringAsync(new Uri(string.Format(ShowsRootPath, city.Code, date.ToString("yyyyMMdd"))));
        }

        #endregion

        #region Read Cities

        private void GetCinepolisCitiesComplete(object sender, DownloadStringCompletedEventArgs e)
        {
            var reader = XmlReader.Create(new StringReader(e.Result));
            var cities = new List<City>();
            while (reader.Read() && reader.Name != "ciudades" || reader.NodeType != XmlNodeType.EndElement)
            {
                if (reader.Name.Equals("ciudad"))
                {
                    var c = ReadCity(reader);
                    cities.Add(c);
                }
            }
            OnCompleteCities(cities);
        }

        private City ReadCity(XmlReader reader)
        {
            var c = new City();
            string val;
            if (GetAttribute(reader, "idPais", out val))
                c.CountryCode = val;
            while (reader.Read() && reader.Name != "ciudad" || reader.NodeType != XmlNodeType.EndElement)
            {
                if (GetValue(reader, "nombre", out val))
                    c.Name = val;
                if (GetValue(reader, "id", out val))
                    c.Code = val;
            }
            return c;
        }

        #endregion

        #region Read Showtimes

        private void GetCinepolisShowsComplete(object sender, DownloadStringCompletedEventArgs e)
        {
            var reader = XmlReader.Create(new StringReader(e.Result));
            var data = new ShowtimeData()
            {
                Date = _date,
                City = _city
            };

            while (reader.Read() && (reader.Name != "cinepolis" || reader.NodeType != XmlNodeType.EndElement))
            {
                if (reader.Name.Equals("vigencia"))
                {
                    data.Expiration = ReadExpiration(reader);
                }

                if (reader.Name.Equals("pelicula"))
                {                    
                    data.Movies.Add(ReadMovie(reader));
                }
                    
                if (reader.Name.Equals("complejo"))
                {
                    data.Theaters.Add(ReadTheater(reader));
                }

                if (reader.Name.Equals("cartelera"))
                {
                    data.Carteleras.Add(ReadShowtimeScheadule(reader));
                }
            }
            if (data.Carteleras.Count == 0)
                OnCompleteShows(null);
            else
                OnCompleteShows(FillCityWithData(_city, data));
        }

        private ShowtimeScheadule ReadShowtimeScheadule(XmlReader reader)
        {
            var c = new ShowtimeScheadule();
            while (reader.Read() && reader.Name != "cartelera" || reader.NodeType != XmlNodeType.EndElement)
            {
                string val;
                if (reader.Name == "cartelera" && reader.NodeType != XmlNodeType.EndElement && GetAttribute(reader, "nombre", out val))
                    c.CityName = val;                    
                if (GetValue(reader, "ciudad", out val))
                    c.CityCode = val;
                if (GetValue(reader, "complejoid", out val))
                    c.TheaterCode = val;
                if (GetValue(reader, "pelicula", out val))
                    c.MovieCode = val;
                if (GetValue(reader, "estreno", out val))
                    c.IsNew = val;
                if (reader.Name == "sala" && reader.NodeType != XmlNodeType.EndElement)
                    c.ShowtimeRooms.Add(ReadShowtimeRoom(reader, c.MovieCode));
            }
            return c;
        }

        private ShowtimeRoom ReadShowtimeRoom(XmlReader reader, string movieCode)
        {
            var s = new ShowtimeRoom();
            string val;
            if (GetAttribute(reader, "numero", out val))
                s.Code = val;
            while (reader.Read() && reader.Name != "sala" || reader.NodeType != XmlNodeType.EndElement)
            {
                if (reader.Name == "horario" && reader.NodeType != XmlNodeType.EndElement)
                {
                    string id, hr;
                    if (GetAttribute(reader, "idshowtime", out id) && GetAttribute(reader, "hr", out hr))
                        s.Showtimes.Add(new Showtime(movieCode, _date, hr));
                }
            }
            return s;
        }

        private Theater ReadTheater(XmlReader reader)
        {
            var c = new Theater();
            string val;
            while (reader.Read() && reader.Name != "complejo" || reader.NodeType != XmlNodeType.EndElement)
            {
                if (GetValue(reader, "complejoid", out val))
                    c.Code = val;
                //if (GetValue(reader, "ciudad", out val))
                //    c.CiudadCode = val;
                if (GetValue(reader, "nombre", out val))
                    c.Name = val;
                if (GetValue(reader, "orden", out val))
                    c.Order = val;
                if (reader.Name == "sala" && reader.NodeType != XmlNodeType.EndElement)
                    c.TheaterRooms.Add(ReadTheaterRoom(reader));
            }
            return c;
        }

        private TheaterRoom ReadTheaterRoom(XmlReader reader)
        {
            var s = new TheaterRoom();
            string val;
            while (reader.Read() && reader.Name != "sala" || reader.NodeType != XmlNodeType.EndElement)
            {
                if (GetValue(reader, "numerosala", out val))
                    s.Code = val;
                if (GetValue(reader, "orden", out val))
                    s.Orden = val;
                if (GetValue(reader, "sonido", out val))
                    s.Features.Add(val);
            }
            return s;
        }

        private Movie ReadMovie(XmlReader reader)
        {
            var p = new Movie();
            string val;
            //if (GetAttribute(reader, "peliculaid", out val))
            //    p.CodeAlt = val;
            if (GetAttribute(reader, "peliculaidVista", out val))
                p.Code = val;
            if (GetAttribute(reader, "nombre", out val))
                p.Title = val;
            while (reader.Read() && reader.Name != "pelicula" || reader.NodeType != XmlNodeType.EndElement)
            {
                if (GetValue(reader, "clasificacion", out val))
                    p.Classification = val;
                if (GetValue(reader, "imagencartel", out val))
                    p.PosterURI = string.Format(ImagesRootPath, val);
                if (GetValue(reader, "sinopsis", out val))
                    p.Sinopsis = val;
                if (GetValue(reader, "actores", out val))
                    p.Actors = val;
                if (GetValue(reader, "calificacion", out val))
                    p.Rating = val;
            }
            return p;
        }

        private Expiration ReadExpiration(XmlReader reader)
        {
            var v = new Expiration();
            while (reader.Read() && reader.Name != "vigencia" || reader.NodeType != XmlNodeType.EndElement)
            {
                var culture = new CultureInfo("en-US");
                if (reader.Name == "de" && reader.NodeType != XmlNodeType.EndElement)
                    v.From = DateTime.ParseExact(reader.GetAttribute("fecha").Trim(), "MM/dd/yyyy", culture);
                if (reader.Name == "hasta" && reader.NodeType != XmlNodeType.EndElement)
                    v.Until = DateTime.ParseExact(reader.GetAttribute("fecha").Trim(), "MM/dd/yyyy", culture);
            }
            return v;
        }

        #endregion

        #region XML Reading Methods

        private bool GetAttribute(XmlReader reader, string attributeName, out string result)
        {
            result = reader.GetAttribute(attributeName).Trim();
            return !string.IsNullOrWhiteSpace(result);
        }

        private bool GetValue(XmlReader reader, string elementName, out string result)
        {
            result = string.Empty;
            if (reader.Name == elementName && reader.NodeType != XmlNodeType.EndElement)
                result = reader.ReadElementContentAsString().Trim();
            return !string.IsNullOrWhiteSpace(result);
        }

        #endregion

        private City FillCityWithData(City city, ShowtimeData data)
        {
            city.Movies = data.Movies;
            city.Theaters = data.Theaters;

            foreach (var cartelera in data.Carteleras)
            {
                var theater = city.Theaters.Where(x => x.Code == cartelera.TheaterCode).SingleOrDefault();

                //cinepolis bug
                if (theater == null)
                    continue;

                foreach (var sala in cartelera.ShowtimeRooms)
                    theater.Showtimes.AddRange(sala.Showtimes);
            }
            return city;
        }

    }
}
