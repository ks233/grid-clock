using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace grid_clock
{
    internal class ColorPalette
    {
        public static bool dark = false;

        public static Color BackgroundColor
        {
            get
            {
                if(!dark) return Color.White;
                else return Color.FromArgb(50,50,50); 
            }
        }

        public static Color TextColor
        {
            get
            {
                if (!dark) return Color.Black;
                else return Color.White;
            }
        }
        public static Color ButtonColor
        {
            get
            {
                if (!dark) return Color.White;
                else return Color.FromArgb(128, 128, 128);
            }
        }
        public static Color TextBoxColor
        {
            get
            {
                if (!dark) return Color.White;
                else return Color.FromArgb(70, 70, 70);
            }
        }

        public static Color morning = Color.FromArgb(71, 222, 111);
        public static Color afternoon = Color.FromArgb(91, 232, 245);
        public static Color evening =  Color.FromArgb(255, 178, 101);
        public static Color night = Color.FromArgb(186, 122, 255);


        public static Color AlarmLine = Color.Red;
    }                 
}
