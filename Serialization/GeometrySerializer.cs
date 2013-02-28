using System;
using System.Linq;
using System.Xml.Linq;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;

namespace Earthworm.Serialization
{
    /// <summary>
    /// Provides extension methods for converting geometries into JSON-serializable objects.
    /// </summary>
    public static class GeometrySerializer
    {
        private static readonly XNamespace Ns = "http://www.esri.com/schemas/ArcGIS/9.3";
        private static readonly XNamespace Xsi = "http://www.w3.org/2001/XMLSchema-instance";

        #region Private

        private static XDocument Serialize(object o)
        {
            IXMLStream xmlStream = new XMLStream();

            IXMLWriter xmlWriter = new XMLWriter();
            xmlWriter.WriteTo((IStream)xmlStream);

            IXMLSerializer xmlSerializer = new XMLSerializer();
            xmlSerializer.WriteObject(xmlWriter, null, null, "esri", Ns.NamespaceName, o);

            return XDocument.Parse(xmlStream.SaveToString());
        }

        private static T Deserialize<T>(XDocument xml)
        {
            IXMLStream xmlStream = new XMLStream();
            xmlStream.LoadFromString(xml.ToString());

            IXMLReader xmlReader = new XMLReader();
            xmlReader.ReadFrom(xmlStream as IStream);

            IXMLSerializer xmlSerializer = new XMLSerializer();

            return (T)xmlSerializer.ReadObject(xmlReader, null, null);
        }

        private static XDocument GetEmptyXmlDocument(string geometryType)
        {
            return XDocument.Parse(@"<?xml version=""1.0"" encoding=""utf-8""?>
                                     <ns:esri xsi:type=""typens:" + geometryType + @"""
                                         xmlns:ns=""" + Ns.NamespaceName + @"""
                                         xmlns:xsi=""" + Xsi.NamespaceName + @"""
                                         xmlns:typens=""" + Ns.NamespaceName + @""">
                                     </ns:esri>");
        }

        private static double[] GetCoordinatesArray(XElement point)
        {
            Func<string, double> getNumber = name => double.Parse(point.Element(name).Value);

            return new[] { getNumber("X"), getNumber("Y") };
        }

        private static JsonPoint ToPoint(this IPoint shape)
        {
            XDocument xml = Serialize(shape);

            JsonPoint point = new JsonPoint();
            point.x = double.Parse(xml.Root.Element("X").Value);
            point.y = double.Parse(xml.Root.Element("Y").Value);

            return point;
        }

        private static JsonMultipoint ToMultipoint(this IMultipoint shape)
        {
            XDocument xml = Serialize(shape);

            JsonMultipoint multipoint = new JsonMultipoint();
            multipoint.points = (from point in xml.Element(Ns + "esri").Element(Ns + "PointArray").Elements(Ns + "Point")
                                 select GetCoordinatesArray(point)).ToArray();

            return multipoint;
        }

        private static JsonPolyline ToPolyline(this IPolyline shape)
        {
            XDocument xml = Serialize(shape);

            JsonPolyline polyline = new JsonPolyline();
            polyline.paths = (from path in xml.Element(Ns + "esri").Element(Ns + "PathArray").Elements(Ns + "Path")
                              let fromPoints = path.Descendants(Ns + "FromPoint")
                              let points = fromPoints.Count() == 0
                                ? path.Element(Ns + "PointArray").Elements(Ns + "Point")
                                : fromPoints.Concat(fromPoints.Take(1))
                              select (from point in points
                                      select GetCoordinatesArray(point)).ToArray()).ToArray();

            return polyline;
        }

        private static JsonPolygon ToPolygon(this IPolygon shape)
        {
            XDocument xml = Serialize(shape);

            JsonPolygon polygon = new JsonPolygon();
            polygon.rings = (from ring in xml.Element(Ns + "esri").Element(Ns + "RingArray").Elements(Ns + "Ring")
                             let fromPoints = ring.Descendants(Ns + "FromPoint")
                             let points = fromPoints.Count() == 0
                               ? ring.Element(Ns + "PointArray").Elements(Ns + "Point")
                               : fromPoints.Concat(fromPoints.Take(1))
                             select (from point in points
                                     select GetCoordinatesArray(point)).ToArray()).ToArray();

            return polygon;
        }

        private static IPoint ToEsriPoint(this JsonPoint shape)
        {
            XDocument xml = GetEmptyXmlDocument("PointN");
            xml.Root.Add(new XElement("X", shape.x));
            xml.Root.Add(new XElement("Y", shape.y));

            return Deserialize<IPoint>(xml);
        }

        private static IMultipoint ToEsriMultipoint(this JsonMultipoint shape)
        {
            XDocument xml = GetEmptyXmlDocument("MultipointN");

            XElement pointArray =
                new XElement(Ns + "PointArray", new XAttribute(Xsi + "type", "typens:ArrayOfPoint"),
                    from point in shape.points
                    select new XElement(Ns + "Point", new XAttribute(Xsi + "type", "typens:PointN"),
                        new XElement("X", point[0]),
                        new XElement("Y", point[1])));

            xml.Root.Add(pointArray);

            return Deserialize<IMultipoint>(xml);
        }

        private static IPolyline ToEsriPolyline(this JsonPolyline shape)
        {
            XDocument xml = GetEmptyXmlDocument("PolylineN");

            XElement pathArray =
                new XElement(Ns + "PathArray", new XAttribute(Xsi + "type", "typens:ArrayOfPath"),
                    from path in shape.paths
                    select new XElement(Ns + "Path", new XAttribute(Xsi + "type", "typens:Path"),
                        new XElement(Ns + "PointArray", new XAttribute(Xsi + "type", "typens:ArrayOfPoint"),
                            from point in path
                            select new XElement(Ns + "Point", new XAttribute(Xsi + "type", "typens:PointN"),
                                new XElement("X", point[0]),
                                new XElement("Y", point[1])))));

            xml.Root.Add(pathArray);

            return Deserialize<IPolyline>(xml);
        }

        private static IPolygon ToEsriPolygon(this JsonPolygon shape)
        {
            XDocument xml = GetEmptyXmlDocument("PolygonN");

            XElement ringArray =
                new XElement(Ns + "RingArray", new XAttribute(Xsi + "type", "typens:ArrayOfRing"),
                    from ring in shape.rings
                    select new XElement(Ns + "Ring", new XAttribute(Xsi + "type", "typens:Ring"),
                        new XElement(Ns + "PointArray", new XAttribute(Xsi + "type", "typens:ArrayOfPoint"),
                            from point in ring
                            select new XElement(Ns + "Point", new XAttribute(Xsi + "type", "typens:PointN"),
                                new XElement("X", point[0]),
                                new XElement("Y", point[1])))));

            xml.Root.Add(ringArray);

            return Deserialize<IPolygon>(xml);
        }

        #endregion

        /// <summary>
        /// Converts an Esri IGeometry object into an IJsonGeometry object.
        /// </summary>
        /// <param name="shape"></param>
        /// <returns></returns>
        public static IJsonGeometry ToJsonGeometry(this IGeometry shape)
        {
            if (shape is IPoint)
                return ((IPoint)shape).ToPoint();

            if (shape is IMultipoint)
                return ((IMultipoint)shape).ToMultipoint();

            if (shape is IPolyline)
                return ((IPolyline)shape).ToPolyline();

            if (shape is IPolygon)
                return ((IPolygon)shape).ToPolygon();

            throw new Exception("This geometry type is not supported.");
        }

        /// <summary>
        /// Converts an IJsonGeometry object into an Esri IGeometry object.
        /// </summary>
        /// <param name="shape"></param>
        /// <returns></returns>
        public static IGeometry ToEsriGeometry(this IJsonGeometry shape)
        {
            if (shape is JsonPoint)
                return ((JsonPoint)shape).ToEsriPoint();

            if (shape is JsonMultipoint)
                return ((JsonMultipoint)shape).ToEsriMultipoint();

            if (shape is JsonPolyline)
                return ((JsonPolyline)shape).ToEsriPolyline();

            if (shape is JsonPolygon)
                return ((JsonPolygon)shape).ToEsriPolygon();

            throw new Exception("This geometry type is not supported.");
        }
    }
}
