using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iHawkSkiaSharpCommonLibrary.Helpers
{
    public class FontHelper
    {
        #region constructor
        public FontHelper(string fileName)
        {
            var typeface = SKTypeface.FromFile(fileName);
            _font = new SKFont(typeface, typeface.UnitsPerEm);
#if DEBUG
            System.Diagnostics.Debug.WriteLine(_font.Metrics);
#endif
            int codepoint = char.ConvertToUtf32("你", 0);
            ushort glyph = _font.GetGlyph(codepoint);
            var path = _font.GetGlyphPath(glyph);
            var contours = GetContours(path);
        }
        #endregion

        #region property
        private readonly SKFont _font;
        #endregion

        #region method
        public static List<List<SKPoint>> GetContours(SKPath path)
        {
            var contours = new List<List<SKPoint>>();
            var currentContour = new List<SKPoint>();

            using (var iterator = path.CreateRawIterator())
            {
                SKPoint[] points = new SKPoint[4];
                SKPathVerb verb;
                while ((verb = iterator.Next(points)) != SKPathVerb.Done)
                {
                    switch (verb)
                    {
                        case SKPathVerb.Move:
                            if (currentContour.Count > 0)
                            {
                                contours.Add(new List<SKPoint>(currentContour));
                                currentContour.Clear();
                            }
                            currentContour.Add(points[0]);
                            break;
                        case SKPathVerb.Line:
                            currentContour.Add(points[1]);
                            break;
                        case SKPathVerb.Quad:
                            currentContour.Add(points[1]);
                            currentContour.Add(points[2]);
                            break;
                        case SKPathVerb.Cubic:
                            currentContour.Add(points[1]);
                            currentContour.Add(points[2]);
                            currentContour.Add(points[3]);
                            break;
                        case SKPathVerb.Close:
                            if (currentContour.Count > 0)
                            {
                                contours.Add(new List<SKPoint>(currentContour));
                                currentContour.Clear();
                            }
                            break;
                    }
                }
                if (currentContour.Count > 0)
                {
                    contours.Add(currentContour);
                }
            }
            return contours;
        }
        #endregion
    }
}
