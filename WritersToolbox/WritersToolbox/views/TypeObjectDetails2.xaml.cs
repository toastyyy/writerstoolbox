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
using System.Windows.Media;
namespace WritersToolbox.views
{
    public partial class TypeObjectDetails2 : PhoneApplicationPage
    {
        TypeDetailViewModel tdvm = null;
        public TypeObjectDetails2()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Beim Navigieren zu dieser Seite wird das ausgewählte Objekt aus
        /// dem Navigationskontext herausgefiltert und die Details dazu mit dem
        /// Viewmodel geladen.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (NavigationContext.QueryString.ContainsKey("typeObjectID"))
            {
                int toID = int.Parse(NavigationContext.QueryString["typeObjectID"]);
                tdvm = new TypeDetailViewModel(toID);
                this.DataContext = tdvm;
            }
        }

        /// <summary>
        /// Eine zum TypObjekt zugehörige Notiz wurde ausgewählt und wird zum bearbeiten geöffnet.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void noteSelected(object sender, SelectionChangedEventArgs e)
        {
            LongListSelector selector = sender as LongListSelector;
            if (selector == null)
                return;
            datawrapper.MemoryNote n = selector.SelectedItem as datawrapper.MemoryNote;
            if (n == null)
                return;
            PhoneApplicationService.Current.State["memoryNoteID"] = n.memoryNoteID.ToString();
            PhoneApplicationService.Current.State["typeObjectID"] = n.fk_typeObjectID;
            PhoneApplicationService.Current.State["edit"] = "true";
            PhoneApplicationService.Current.State["assignedNote"] = "true";
            NavigationService.Navigate(new Uri("/views/AddNote.xaml", UriKind.Relative));
        }


        private void editTypeObjectClick(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/views/TypeObjectEdit.xaml?typeObjectID=" + this.tdvm.TypeObject.typeObjectID, UriKind.Relative));
        }

        private void deleteTypeObject_Click(object sender, EventArgs e)
        {
            MessageBoxResult mbr = MessageBox.Show("Möchtest du dieses Objekt wirklich löschen? Alle angehängten Notizen werden mitgelöscht. Du kannst das Objekt über den Papierkorb wiederherstellen.", "Löschen bestätigen", MessageBoxButton.OKCancel);
            if(mbr.Equals(MessageBoxResult.OK)) {
                TypesViewModel tvm = new TypesViewModel();
                tvm.deleteTypeObject(this.tdvm.TypeObject.typeObjectID);
                NavigationService.Navigate(new Uri("/views/Types.xaml", UriKind.Relative));
            }
        }

        private void TryDeleteTypeObject(object sender, EventArgs e)
        {
            TypeObjectDeleteQuestion.Text = "Wollen Sie das Typobjekt \"" + tdvm.TypeObject.name.ToString() + "\" löschen?";
            deleteTypeObjectPopup.IsOpen = true;
        }

        private void DeleteTypeObject(object sender, EventArgs e)
        {
            tdvm.deleteTypeObject(tdvm.TypeObject.typeObjectID);
            deleteTypeObjectPopup.IsOpen = false;
            NavigationService.GoBack();
        }

        private void DoNotDeleteTypeObject(object sender, EventArgs e)
        {
            deleteTypeObjectPopup.IsOpen = false;
        }
    }
}