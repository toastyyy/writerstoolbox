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
using System.Collections;
using System.IO.IsolatedStorage;
using System.IO;

namespace WritersToolbox.views
{
    public partial class ExportText : PhoneApplicationPage
    {
        private BooksViewModel bvm = null;
        private int bookID = -1;

        private static String exportCSSString = @"
                body {
                    background-color: #eeeeee;
                    margin-top: 20px;
                    margin-bottom: 15px;
                }
                div {
                    width: 70%;
                    background-color: #ffffff;
                    margin: 0 auto;
                    padding: 20px 15px 20px 15px;
                }
                p {
                    text-align: justify;
                }
            ";
        public ExportText()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.bvm = new BooksViewModel();
            PhoneApplicationService.Current.State["assignNote"] = true;
            this.bvm.loadData();
            PhoneApplicationService.Current.State.Remove("assignNote");
            this.DataContext = this.bvm;
        }

        private void BookChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                bookID = ((datawrapper.Book)e.AddedItems[0]).bookID;
            }
            else {
                this.bookID = ((datawrapper.Book)this.bookPicker.SelectedItem).bookID;
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (bookID >= 0)
            {


                // zunaechst das passende buch finden
                IEnumerator enumerator = this.bvm.Books.GetEnumerator();
                datawrapper.Book book = null;

                while (enumerator.MoveNext() && book == null) {
                    if (((datawrapper.Book)enumerator.Current).bookID == this.bookID) {
                        book = (datawrapper.Book)enumerator.Current;
                    }
                }

                // wenn ein passendes buch zur id gefunden wurde koennen alle tomes einzeln
                // in eine HTML exportiert werden.
                
                if (book != null) {
                    IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();

                    String html = "<!DOCTYPE html><html><head>"
                                + "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">"
                                + "<style>"
                                + exportCSSString
                                + "</style>"
                                + "</head><body><div>";

                    html += "<h1>" + book.name + "</h1><br>";
                    enumerator = book.tomes.GetEnumerator();

                    // ueber alle baende des buches iterieren
                    while (enumerator.MoveNext()) {
                        datawrapper.Tome curTome = (datawrapper.Tome)enumerator.Current;
                        TomeDetailsViewModel tdvm = new TomeDetailsViewModel(curTome.tomeID);
                        tdvm.removeNewChapterEntry();
                        tdvm.removeNewEventEntry();
                        IEnumerator chapterEnumerator = tdvm.structur.GetEnumerator();
                        html += "<hr><h2>" + curTome.title + "</h2><br>";
                        // ueber alle kapitel des aktuellen bandes iterieren
                        while (chapterEnumerator.MoveNext()) {
                            datawrapper.Chapter curChapter = (datawrapper.Chapter)chapterEnumerator.Current;
                            html += "<h3>" + curChapter.title + "</h3>";
                            // ueber alle events des aktuellen kapitels iterieren
                            IEnumerator eventEnumerator = curChapter.events.GetEnumerator();
                            while (eventEnumerator.MoveNext()) {
                                datawrapper.Event curEvent = (datawrapper.Event)eventEnumerator.Current;
                                EventDetailViewModel edvm = new EventDetailViewModel(curEvent.eventID);
                                edvm.LoadData();
                                html += "<i>" + curEvent.title + "</i><br>";
                                html += "<p>" + edvm.Event.finaltext + "</p>";
                            }
                        }
                    }
                    html += "</div></body></html>";    
                    // html minimieren
                    html = html.Replace("\r\n", " ").Replace("  ", " ");

                    String filename = (book.name.Replace(" ", "_")) + ".html";
                    using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(filename, FileMode.OpenOrCreate, isoStore))
                    {    
                        using (StreamWriter writer = new StreamWriter(isoStream))
                        {
                            writer.WriteLine(html);
                        }
                        isoStream.Close();
                    }
                    MessageBox.Show("Das Buch wurde erfolgreich exportiert", "Buch exportiert", MessageBoxButton.OK);
                }


            }

            
        }
    }
}