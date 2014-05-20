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
using DropNet;
using DropNet.Models;
using System.Diagnostics;

namespace WritersToolbox.views
{
    /// <summary>
    /// GUI für den Export im Buchformat.
    /// </summary>
    public partial class ExportText : PhoneApplicationPage
    {
        private BooksViewModel bvm = null;
        private int bookID = -1;
        UserLogin token = null;
        DropNetClient _client;

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
        /// <summary>
        /// Erzeugt eine neue Instanz der GUI und führt den Login in der Dropbox aus.
        /// </summary>
        public ExportText()
        {
            InitializeComponent();
            String credentials = this.loadUserCredentials();
            if (credentials.Equals(""))
            {
                _client = new DropNetClient("6uvenkdtbc0antp", "dxb48bxwgem3ziz");
            }
            else
            {
                String secret = credentials.Split('|')[0];
                String usertoken = credentials.Split('|')[1].Replace("\r\n", "");
                _client = new DropNetClient("6uvenkdtbc0antp", "dxb48bxwgem3ziz", usertoken, secret);
                this.btnConnectDropbox.Visibility = Visibility.Collapsed;
                this.displayAccountInformation();
            }

            _client.UseSandbox = true;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.bvm = new BooksViewModel();
            PhoneApplicationService.Current.State["assignNote"] = true;
            this.bvm.loadData();
            PhoneApplicationService.Current.State.Remove("assignNote");
            this.DataContext = this.bvm;

            if (PhoneApplicationService.Current.State.ContainsKey("dropboxAuth"))
            {
                PhoneApplicationService.Current.State.Remove("dropboxAuth");
                this.btnConnectDropbox.Visibility = Visibility.Collapsed;

                _client.GetAccessTokenAsync(
                        (accessToken) =>
                        {
                            this.token = accessToken;
                            this.saveUserCredentials(this.token.Secret, this.token.Token);
                            this._client.UserLogin = accessToken;
                            this.displayAccountInformation();
                        },
                        (error) =>
                        {
                            Debug.WriteLine(error.Message);
                        }
                    );
            }
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
                    _client.UploadFileAsync("/", filename, this.generateStringStream(html),
                        (response) =>
                        {
                            MessageBox.Show("Das Buch wurde erfolgreich exportiert", "Buch exportiert", MessageBoxButton.OK);

                        },
                        (error) =>
                        {
                            MessageBox.Show("Beim Erstellen des Buches ist ein Fehler aufgetreten. Möglicherweise ist die Verbindung abgebrochen oder die Dropbox ist voll.");
                        }
                        );
                }


            }

            
        }

        private void connectToDropbox(object sender, System.Windows.Input.GestureEventArgs e)
        {
            _client.GetTokenAsync(
                (userLogin) =>
                {
                    // Rechte fuer die App einfordern
                    var url = _client.BuildAuthorizeUrl("http://dkdevelopment.net/BoxShotLogin.htm");
                    PhoneApplicationService.Current.State["authURL"] = url;
                    NavigationService.Navigate(new Uri("/views/DropboxAuthorize.xaml", UriKind.RelativeOrAbsolute));
                },
                (error) =>
                {
                    Debug.WriteLine(error.Message);
                }
);
        }

        /// <summary>
        /// Speichert UserToken, damit der User die App nicht bei jedem Neustart authorisieren muss.
        /// </summary>
        /// <param name="secret"></param>
        /// <param name="token"></param>
        private void saveUserCredentials(String secret, String token)
        {
            IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();
            using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream("dropbox_user", FileMode.OpenOrCreate, isoStore))
            {
                using (StreamWriter writer = new StreamWriter(isoStream))
                {
                    writer.WriteLine(secret + "|" + token);
                }
                isoStream.Close();
            }
        }

        private String loadUserCredentials()
        {
            String ret = "";
            IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();
            if (isoStore.FileExists("dropbox_user"))
            {
                using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream("dropbox_user", FileMode.Open, isoStore))
                {
                    using (StreamReader reader = new StreamReader(isoStream))
                    {
                        ret = reader.ReadToEnd();
                    }
                }
            }
            return ret;
        }

        private void displayAccountInformation()
        {
            _client.AccountInfoAsync((accountInfo) =>
            {
                tInfo.Text = "Eingeloggt als " + accountInfo.display_name;
            },
            (error) =>
            {
                tInfo.Text = "Fehler beim Abfragen von Accountinformationen";
            });
        }

        private Stream generateStringStream(String s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}