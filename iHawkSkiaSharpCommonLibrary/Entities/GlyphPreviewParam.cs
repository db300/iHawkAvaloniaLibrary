using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iHawkSkiaSharpCommonLibrary.Entities
{
    public class GlyphPreviewParam
    {
        public bool YMinMaxVisible { get; set; }
        public bool HheaAscDescVisible { get; set; }
        public bool TypoAscDescVisible { get; set; }
        public bool WinAscDescVisible { get; set; }
        /// <summary>
        /// 行间距标识(YMinMax/AscDesc)
        /// </summary>
        [DefaultValue("AscDesc")]
        public string LineGapTag { get; set; } = "AscDesc";
    }
}
