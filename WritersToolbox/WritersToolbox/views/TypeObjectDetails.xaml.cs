using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using WritersToolbox.viewmodels;

namespace WritersToolbox.views
{
    public partial class TypeObjectDetails : PhoneApplicationPage
    {
        private TypeDetailViewModel tdvm;
        public TypeObjectDetails()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (NavigationContext.QueryString.ContainsKey("item"))
            {
                int toID = int.Parse(NavigationContext.QueryString["item"]);
                tdvm = new TypeDetailViewModel(toID);
                this.DataContext = tdvm;
            }
        }
    }
}