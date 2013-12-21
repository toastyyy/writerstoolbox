using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace WritersToolbox.views
{
    public partial class PivotPage1 : PhoneApplicationPage
    {
        public PivotPage1()
        {
            InitializeComponent();
        }

        public partial class AnimatableScrollViewer : UserControl
        {
            public static readonly DependencyProperty AnimatablOffsetProperty = DependencyProperty.Register("AnimatableOffset",
                typeof(double), typeof(AnimatableScrollViewer), new PropertyMetadata(AnimatableOffsetPropertyChanged));

            public double AnimatableOffset
            {
                get { return (double)this.GetValue(AnimatablOffsetProperty); }
                set { this.SetValue(AnimatablOffsetProperty, value); }
            }

            public AnimatableScrollViewer()
            {
                AnimatableOffset = scrollViewer.VerticalOffset;
            }

            private static void AnimatableOffsetPropertyChanged(object sender, DependencyPropertyChangedEventArgs args)
            {
                AnimatableScrollViewer cThis = sender as AnimatableScrollViewer;
                cThis.scrollViewer.ScrollToVerticalOffset((double)args.NewValue);
            }
        }
    }


}