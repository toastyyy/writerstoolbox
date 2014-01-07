using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WritersToolbox.views
{
    abstract class GridManager
    {
        public static void addRow(ref Grid g, double height)
        {
            RowDefinition row = new RowDefinition();
            row.Height = new GridLength(height);
            g.RowDefinitions.Add(row);
        }

        public static void addCol(ref Grid g, double width)
        {
            ColumnDefinition col = new ColumnDefinition();
            
            col.Width = new GridLength(width);
            g.ColumnDefinitions.Add(col);
        }

        public static void addObjectInGrid(ref Grid g,ref Image obj, int i)
        {
            try
            {
                g.Children.Add(obj);
                Grid.SetRow(obj, (int)(i / 2));
                Grid.SetColumn(obj, i % 2); 
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.Message);
                Console.Out.WriteLine(ex.StackTrace);
            }

        }

        public static void addObjectInGrid(ref Grid g, ref Button obj, int i)
        {
            try
            {
                g.Children.Add(obj);
                Grid.SetRow(obj, (int)(i / 2));
                Grid.SetColumn(obj, i % 2);
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.Message);
                Console.Out.WriteLine(ex.StackTrace);
            }
        }

        public static void removeChild(ref Grid g, Button b)
        {
            g.Children.Remove(b);
        }
        public static void removeChild(ref Grid g, Image i)
        {
            g.Children.Remove(i);
        }
        public static void removeLast_ObjectFromGrid(ref Grid g)
        {
            g.ColumnDefinitions.RemoveAt(g.ColumnDefinitions.Count - 1);
        }

        public static void removeRow(Grid g, int index)
        {
            g.RowDefinitions.RemoveAt(index);
        }

        public static void removeCol(Grid g, int index)
        {
            g.ColumnDefinitions.RemoveAt(index);
        }

    }
}
