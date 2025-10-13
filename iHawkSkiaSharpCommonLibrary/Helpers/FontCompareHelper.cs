using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iHawkSkiaSharpCommonLibrary.Helpers
{
    public static class FontCompareHelper
    {
        /// <summary>
        /// 加载两个字体文件并比对Unicode差异及字形路径差异
        /// </summary>
        /// <param name="fontFile1"></param>
        /// <param name="fontFile2"></param>
        /// <returns></returns>
        public static CompareResult CompareFonts(string fontFile1, string fontFile2)
        {
            var font1 = new SkiaSharpHelper(fontFile1);
            var font2 = new SkiaSharpHelper(fontFile2);

            var unicode1 = font1.GetAllUnicode() ?? [];
            var unicode2 = font2.GetAllUnicode() ?? [];

            var set1 = new HashSet<int>(unicode1);
            var set2 = new HashSet<int>(unicode2);

            var onlyIn1 = set1.Except(set2).ToList();
            var onlyIn2 = set2.Except(set1).ToList();
            var common = set1.Intersect(set2);

            var gidMap1 = font1.GetUnicodeGidMap() ?? [];
            var gidMap2 = font2.GetUnicodeGidMap() ?? [];

            var diffGlyph = new List<int>();
            using var typeface1 = SKTypeface.FromFile(fontFile1);
            using var typeface2 = SKTypeface.FromFile(fontFile2);
            using var fontObj1 = new SKFont(typeface1, typeface1.UnitsPerEm);
            using var fontObj2 = new SKFont(typeface2, typeface2.UnitsPerEm);
            foreach (var code in common)
            {
                if (!gidMap1.TryGetValue(code, out var gid1) || !gidMap2.TryGetValue(code, out var gid2))
                    continue;
                if (gid1 == 0 || gid2 == 0)
                    continue;

                // 获取路径数据
                using var path1 = fontObj1.GetGlyphPath((ushort)gid1);
                using var path2 = fontObj2.GetGlyphPath((ushort)gid2);

                // 路径数据序列化为字符串进行快速比较
                var str1 = path1?.ToSvgPathData();
                var str2 = path2?.ToSvgPathData();

                if (!string.Equals(str1, str2, StringComparison.Ordinal))
                {
                    diffGlyph.Add(code);
                }
            }

            return new CompareResult
            {
                OnlyInFont1 = onlyIn1,
                OnlyInFont2 = onlyIn2,
                SameUnicodeDifferentGlyph = diffGlyph
            };
        }

        #region custom class
        public class CompareResult
        {
            public List<int> OnlyInFont1 { get; set; } = [];
            public List<int> OnlyInFont2 { get; set; } = [];
            public List<int> SameUnicodeDifferentGlyph { get; set; } = [];
        }
        #endregion
    }
}
