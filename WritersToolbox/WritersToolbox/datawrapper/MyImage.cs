using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Media.PhoneExtensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WritersToolbox.datawrapper
{
    public class MyImage
    {
        //Instanzvariable
        public string id { get; set; }
        public string name { get; set; }
        public BitmapImage vorschaubild { get; set; }
        public string path { get; set; }

        /// <summary>
        /// Konstruktor mit Picture Parameter.
        /// </summary>
        /// <param name="picture">Bild als Pictur</param>
        public MyImage(Picture picture)
        {
            this.id = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            this.name = picture.Name;
            this.path = picture.GetPath();
            this.vorschaubild = new BitmapImage();
            this.vorschaubild.SetSource(picture.GetThumbnail());

        }

        /// <summary>
        /// Um die Bilder zu vergleichen.
        /// </summary>
        /// <param name="obj">Bild</param>
        /// <returns>True wenn die Bilder gleich sind</returns>
        public override bool Equals(object obj)
        {
            return id.Equals(((MyImage)obj).id);
        }

        /// <summary>
        /// Um die Collections von Bilder zu vergelichen.
        /// </summary>
        /// <param name="c1">erste Collection</param>
        /// <param name="c2">zweite Collection</param>
        /// <returns>True wenn die beide Collection mit gleichen Inhat sind</returns>
        public static bool isImageCollectionEquals(ObservableCollection<MyImage> c1, ObservableCollection<MyImage> c2)
        {
            bool isequals = false;
            if (c1.Count == c2.Count)
            {
                if (c1.Count == 0 && c2.Count == 0)
                {
                    isequals = true;
                }
                for (int i = 0; i < c1.Count && !isequals; i++)
                {
                    isequals = c1[i].Equals(c2[i]);
                }
            }
            return isequals;
        }
    }
}
