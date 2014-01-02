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
        public string id { get; set; }
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
            this.id = DateTime.Now.ToString("yyyyMMddHHmmssfff");
        }
        public MyImage(BitmapImage bitmap, string name)
        {
            this.vorschaubild = bitmap;
            this.name = name;
            this.id = DateTime.Now.ToString("yyyyMMddHHmmssfff");
        }

        public override bool Equals(object obj)
        {
            return id.Equals(((MyImage)obj).id);
        }
    }
}
