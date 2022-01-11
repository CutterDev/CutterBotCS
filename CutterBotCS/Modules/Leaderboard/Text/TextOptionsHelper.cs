﻿using SixLabors.Fonts;
using SixLabors.ImageSharp.Drawing.Processing;

namespace CutterBotCS.Modules.Leaderboard
{
    /// <summary>
    /// Text Options Helper
    /// </summary>
    public class TextOptionsHelper
    {
        public DrawingOptions AlignLeft;
        public DrawingOptions AlignRight;
        public DrawingOptions AlignCenter;

        /// <summary>
        /// Constructor
        /// </summary>
        public TextOptionsHelper()
        {
            AlignLeft = new DrawingOptions()
            {
                TextOptions = new TextOptions()
                {
                    HorizontalAlignment = HorizontalAlignment.Left
                }
            };

            AlignRight = new DrawingOptions()
            {
                TextOptions = new TextOptions()
                {
                    HorizontalAlignment = HorizontalAlignment.Right,
                }
            };

            AlignCenter = new DrawingOptions()
            {
                TextOptions = new TextOptions()
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                }
            };
        }

    }
    /// <summary>
    /// Text Alignment Enum
    /// </summary>
    public enum TextAlignment
    {
        Left,
        Right,
        Center
    }
}
