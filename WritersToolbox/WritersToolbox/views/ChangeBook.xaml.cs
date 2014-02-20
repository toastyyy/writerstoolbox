﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Coding4Fun.Toolkit.Controls;
using System.Windows.Media;
using WritersToolbox.models;

namespace WritersToolbox.views
{
    public partial class ChangeBook : PhoneApplicationPage
    {
        private Book book;

        public ChangeBook()
        {
            DataContext = Books.Books_VM;
            InitializeComponent();
        }
        /// <summary>
        /// Ein Buch wird geändert.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateBook(object sender, EventArgs e)
        {
            String name = bName.Text;

            try
            {
                Books.Books_VM.updateBook(book.bookID, name, book.obj_bookType.bookTypeID);
                NavigationService.GoBack();
            }
            catch (ArgumentException ae)
            {
                MessageBox.Show(ae.Message, "Fehler", MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// Cancelt die Erstellungen eines Buchs und geht eine Seite zurück.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelBook(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (NavigationContext.QueryString.ContainsKey("item"))
            {
                var item = NavigationContext.QueryString["item"];
                var indexParsed = int.Parse(item);
                book = Books.Books_VM.getBookByID(indexParsed);
                bName.Text = book.name;
                booktype.Text = book.obj_bookType.name;
            }


        }

        private void changeBookType(object sender, System.Windows.Input.GestureEventArgs e)
        {
            booktype_popup.IsOpen = true;
        }

        private void bookTypeCancel(object sender, System.Windows.Input.GestureEventArgs e)
        {
            booktype_popup.IsOpen = false;
        }
    }
}