using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
namespace WritersToolbox.models
{
    public class MyImage
    {
                
        private Picture picture;
        private string name;
        private BitmapImage vorschaubild;
        private BitmapImage bild;

        public string Name { get { return name; } }
        public BitmapImage Vorschaubild { get { return vorschaubild; } }
 
        public MyImage(Picture picture)
        {
            this.picture = picture;
            this.name = picture.Name;

            this.vorschaubild = new BitmapImage();
            vorschaubild.SetSource(picture.GetThumbnail());

        }
        public MyImage(BitmapImage bitmap, string name)
        {
            this.vorschaubild = bitmap;
            this.name = name;
        }

        public BitmapImage GetBild()
        {
            if (bild == null) // Wenn das Bild noch nie eingelesen wurde
            {
                bild = new BitmapImage();
                bild.SetSource(picture.GetThumbnail());
            }
            return bild;
        }
    }
}
