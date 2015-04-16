namespace Earthworm.Serialization
{
    /// <summary>
    /// Represents the KML style.
    /// </summary>
    public class KmlStyle
    {
        /// <summary>
        /// The url of the icon.
        /// </summary>
        public string IconUrl { get; private set; }

        /// <summary>
        /// The colour of icons.  The format conforms to the KML specification (i.e. 7fff0000 where alpha=0x7f, blue=0xff, green=0x00, and red=0x00).
        /// </summary>
        public string IconColour { get; private set; }

        /// <summary>
        /// The size of icons.
        /// </summary>
        public double IconScale { get; private set; }

        /// <summary>
        /// The colour of lines.  The format conforms to the KML specification (i.e. 7fff0000 where alpha=0x7f, blue=0xff, green=0x00, and red=0x00).
        /// </summary>
        public string LineColour { get; private set; }

        /// <summary>
        /// The width of lines.  This applies to polygons, too.
        /// </summary>
        public double LineWidth { get; private set; }

        /// <summary>
        /// The colour of polygons.  The format conforms to the KML specification (i.e. 7fff0000 where alpha=0x7f, blue=0xff, green=0x00, and red=0x00).
        /// </summary>
        public string PolygonColour { get; private set; }

        /// <summary>
        /// Initializes a new instance of the KmlStyle class.
        /// </summary>
        /// <param name="iconUrl">The url of the icon.</param>
        /// <param name="iconColour">The colour of icons.  The format conforms to the KML specification (i.e. 7fff0000 where alpha=0x7f, blue=0xff, green=0x00, and red=0x00).</param>
        /// <param name="iconScale">The size of icons.</param>
        /// <param name="lineColour">The colour of lines.  The format conforms to the KML specification (i.e. 7fff0000 where alpha=0x7f, blue=0xff, green=0x00, and red=0x00).</param>
        /// <param name="lineWidth">The width of lines.  This applies to polygons, too.</param>
        /// <param name="polygonColour">The colour of polygons.  The format conforms to the KML specification (i.e. 7fff0000 where alpha=0x7f, blue=0xff, green=0x00, and red=0x00).</param>
        public KmlStyle(string iconUrl = null, string iconColour = null, double iconScale = 1.1, string lineColour = null, double lineWidth = 1.2, string polygonColour = null)
        {
            IconUrl = iconUrl ?? "http://maps.google.com/mapfiles/kml/pushpin/ylw-pushpin.png";
            IconColour = iconColour ?? "ffffffff";
            IconScale = iconScale;
            LineColour = lineColour ?? "ffffffff";
            LineWidth = lineWidth;
            PolygonColour = polygonColour ?? "ffffffff";
        }

        /// <summary>
        /// Overridden to return the value equality.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

            var s = obj as KmlStyle;

            if (s == null)
                return false;

            return s.IconUrl == IconUrl
                && s.IconColour == IconColour
                && s.IconScale == IconScale
                && s.LineColour == LineColour
                && s.LineWidth == LineWidth
                && s.PolygonColour == PolygonColour;
        }

        /// <summary>
        /// Serves as a hash function for equality comparison.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var n = 23;
                var hash = 17;

                hash = hash * n + IconUrl.GetHashCode();
                hash = hash * n + IconColour.GetHashCode();
                hash = hash * n + IconScale.GetHashCode();
                hash = hash * n + LineColour.GetHashCode();
                hash = hash * n + LineWidth.GetHashCode();
                hash = hash * n + PolygonColour.GetHashCode();

                return hash;
            }
        }
    }
}
