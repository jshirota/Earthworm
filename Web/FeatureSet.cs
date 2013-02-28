using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Script.Serialization;
using ESRI.ArcGIS.Geometry;
using Earthworm.AO;
using Earthworm.Serialization;

namespace Earthworm.Web
{
    /// <summary>
    /// Provides functionality for downloading features from ArcGIS Server REST services.  Automatically deserializes JSON features into objects of the specified type.
    /// </summary>
    public class FeatureSet
    {
        private readonly JavaScriptSerializer _serializer = new JavaScriptSerializer { MaxJsonLength = int.MaxValue };
        private readonly string _url;
        private readonly string _baseUrl;
        private readonly RestApi.LayerDefinition _layerDefinition;
        private readonly RestApi.Field _idField;

        /// <summary>
        /// The spatial reference of the source data.
        /// </summary>
        public ISpatialReference SpatialReference { get; private set; }

        /// <summary>
        /// The geometry type of the source data.
        /// </summary>
        public esriGeometryType GeometryType { get; private set; }

        /// <summary>
        /// The total number of features that can be downloaded based on the current query strings.  Returns null if the feature count cannot be determined (due to older versions of ArcGIS Server).
        /// </summary>
        public int? FeatureCount { get; private set; }

        /// <summary>
        /// Initializes a new instance of the FeatureSet class.  Performs initial queries to obtain the service metadata.
        /// </summary>
        /// <param name="url">The REST endpoint URL (i.e. "http://localhost:6080/arcgis/rest/services/SampleWorldCities/MapServer/0").  Also accepts query strings such as "where".</param>
        public FeatureSet(string url)
        {
            _url = Regex.IsMatch(url, @"/MapServer/\d+/?$", RegexOptions.IgnoreCase) ? url + "/query" : url;
            _baseUrl = Regex.Match(_url, @"^.*?/MapServer/\d+", RegexOptions.IgnoreCase).Value;

            _layerDefinition = Download<RestApi.LayerDefinition>(_baseUrl + "?f=json");

            if (_layerDefinition.Type != "Feature Layer" && _layerDefinition.Type != "Table")
                throw new Exception(string.Format("'{0}' is not supported.", _layerDefinition.Type));

            _idField = _layerDefinition.Fields.SingleOrDefault(f => f.Type == "esriFieldTypeOID");

            RestApi.SpatialReference spatialReference = _layerDefinition.Extent == null ? null : _layerDefinition.Extent.SpatialReference;

            if (spatialReference != null)
            {
                SpatialReference = spatialReference.Wkt == null
                    ? TopologicalOpExt.GetSpatialReference(spatialReference.Wkid)
                    : TopologicalOpExt.GetSpatialReference(spatialReference.Wkt);
            }

            GeometryType = _layerDefinition.GeometryType == null
                ? esriGeometryType.esriGeometryNull
                : (esriGeometryType)Enum.Parse(typeof(esriGeometryType), _layerDefinition.GeometryType);

            try
            {
                string u = ReplaceQueryString(_url, "where", s => s == "" ? "1%3E0" : s);
                u = ReplaceQueryString(u, "returnCountOnly", s => "true");
                u = ReplaceQueryString(u, "f", s => "json");

                FeatureCount = Download<RestApi.FeatureCount>(u).Count;
            }
            catch { }
        }

        #region Private

        private string ReplaceQueryString(string url, string queryString, Func<string, string> filter)
        {
            string pattern = @"(?<=((\?|&)" + queryString + @"=)).*?(?=((&|$)))";

            if (Regex.IsMatch(url, pattern))
                return Regex.Replace(url, pattern, m => filter(m.Value), RegexOptions.IgnoreCase);

            return url + (url.Contains("?") ? "&" : "?") + queryString + "=" + filter("");
        }

        private T Download<T>(string url) where T : RestApi.Response
        {
            string json = null;

            for (int i = 0; i < 10; i++)
            {
                try
                {
                    using (GZipWebClient client = new GZipWebClient())
                    {
                        client.Encoding = Encoding.UTF8;
                        client.Headers.Add("Accept-Encoding", "gzip,deflate");
                        json = client.DownloadString(url);
                    }

                    break;
                }
                catch
                {
                    Thread.Sleep(100);
                }
            }

            if (json == null)
                throw new Exception("Download failed.");

            T response = _serializer.Deserialize<T>(json);

            if (response.Error != null)
                throw new Exception(response.Error.Description ?? response.Error.Message);

            return response;
        }

        private class GZipWebClient : WebClient
        {
            protected override WebRequest GetWebRequest(Uri address)
            {
                HttpWebRequest request = (HttpWebRequest)base.GetWebRequest(address);
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                return request;
            }
        }

        private Dictionary<string, object>[] Download(int min, int max, bool returnAll = false)
        {
            if (_idField == null)
                throw new Exception("The layer must have one and only one OID field.");

            string u = ReplaceQueryString(_url, "where", s => string.Format("({0})+AND+{1}%3E%3D{2}+AND+{1}%3C{3}", s == "" ? "1%3E0" : s, _idField.Name, min, max));
            u = ReplaceQueryString(u, "outFields", s => returnAll ? "*" : "");
            u = ReplaceQueryString(u, "returnGeometry", s => returnAll ? "true" : "false");
            u = ReplaceQueryString(u, "f", s => "json");

            return Download<RestApi.FeatureSet>(u).Features;
        }

        private int FindNext(int id)
        {
            int min = id;
            int max = int.MaxValue / 2;

            while (true)
            {
                int mid = (min + max) / 2;

                if (Download(min, mid).Length == 0)
                    min = mid;
                else
                    max = mid;

                if (max - min < 1000)
                    return min;
            }
        }

        private IEnumerable<Dictionary<string, object>> Download(int batchSize)
        {
            int id = 0;
            int count = 0;

            while (true)
            {
                if (count > 10)
                {
                    if (Download(id, int.MaxValue).Length == 0)
                        yield break;

                    id = FindNext(id);
                    count = 0;
                }

                Dictionary<string, object>[] features = Download(id, id + batchSize, true);

                id += batchSize;

                if (features.Length == 0)
                    count++;
                else
                    count = 0;

                foreach (Dictionary<string, object> f in features)
                    yield return f;
            }
        }

        #endregion

        /// <summary>
        /// Downloads JSON features, deserializes them as objects of the specified type and returns them as a (lazily evaluated) sequence.  This repetitively queries the server until all features meeting the current filter criteria have been downloaded.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> Map<T>() where T : MappableFeature, new()
        {
            return Download(50)
                .Take(FeatureCount ?? int.MaxValue)
                .Select(d =>
                {
                    T f = Json.Deserialize<T>(_serializer.Serialize(d));

                    if (f.Shape != null && SpatialReference != null)
                        f.Shape.SpatialReference = SpatialReference;

                    return f;
                });
        }
    }
}
