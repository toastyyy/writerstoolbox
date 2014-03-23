using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Diagnostics;
using DropNet;
using DropNet.Models;
using System.IO;
using Microsoft.Phone.Tasks;
using System.IO.IsolatedStorage;
using WritersToolbox.viewmodels;
using System.Collections;
using System.Xml.Linq;
using System.Xml;

namespace WritersToolbox.views
{
    public partial class ExportImportBackup : PhoneApplicationPage
    {
        UserLogin token = null;
        DropNetClient _client;
        public ExportImportBackup()
        {
            InitializeComponent();
            String credentials = this.loadUserCredentials();
            if (credentials.Equals(""))
            {
                _client = new DropNetClient("6uvenkdtbc0antp", "dxb48bxwgem3ziz");
            }
            else {
                String secret = credentials.Split('|')[0];
                String usertoken = credentials.Split('|')[1].Replace("\r\n", "");
                _client = new DropNetClient("6uvenkdtbc0antp", "dxb48bxwgem3ziz", usertoken, secret);
                this.btnConnectDropbox.Visibility = Visibility.Collapsed;
                this.exportBackupButton.Visibility = Visibility.Visible;
                this.importBackupButton.Visibility = Visibility.Visible;
                this.displayAccountInformation();
            }
            
            _client.UseSandbox = true;
        }

        private Stream generateStringStream(String s) {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        private void displayAccountInformation() {
            _client.AccountInfoAsync((accountInfo) =>
            {
                tInfo.Text = "Eingeloggt als " + accountInfo.display_name;
            },
            (error) =>
            {
                tInfo.Text = "Fehler beim Abfragen von Accountinformationen";
            });
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            // prueft ob eine authentifizierung mit dropbox stattgefunden hat.
            // wenn ja kann eine dropbox client instanz ordnungsgemaess aufgebaut werden
            // der entsprechende state wird entfernt
            if (PhoneApplicationService.Current.State.ContainsKey("dropboxAuth")) {
                PhoneApplicationService.Current.State.Remove("dropboxAuth");
                this.btnConnectDropbox.Visibility = Visibility.Collapsed;

                _client.GetAccessTokenAsync(
                        (accessToken) =>
                        {
                            this.token = accessToken;
                            this.saveUserCredentials(this.token.Secret, this.token.Token);
                            this._client.UserLogin = accessToken;
                            this.exportBackupButton.Visibility = Visibility.Visible;
                            this.importBackupButton.Visibility = Visibility.Visible;
                            this.displayAccountInformation();
                        },
                        (error) =>
                        {
                            Debug.WriteLine(error.Message);
                        }
                    );
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

        private void exportBackup(object sender, System.Windows.Input.GestureEventArgs e)
        {
            MemoryStream ms = new MemoryStream();
            ms.Position = 0;
            Dictionary<String, IEnumerator> data = this.getExportData();


                XDocument doc = new XDocument();
                var root = new XElement("data");

                var elementBookTypes = new XElement("bookTypes");
                IEnumerator bookTypes = data["bookTypes"];
                while (bookTypes.MoveNext()) {
                    datawrapper.BookType bt = (datawrapper.BookType) bookTypes.Current;
                    var bookType = new XElement("BookType");
                    bookType.SetAttributeValue("bookTypeID", bt.bookTypeID);
                    bookType.SetAttributeValue("addedDate", bt.addedDate.ToString());
                    bookType.SetAttributeValue("name", bt.name);
                    bookType.SetAttributeValue("numberOfChapter", bt.numberOfChapter);
                    bookType.SetAttributeValue("updatedDate", bt.updatedDate.ToString());
                    elementBookTypes.Add(bookType);
                }
                root.Add(elementBookTypes);

                var elementBooks = new XElement("books");
                IEnumerator books = data["books"];
                while (books.MoveNext())
                {
                    datawrapper.Book b = (datawrapper.Book)books.Current;
                    var book = new XElement("Book");
                    book.SetAttributeValue("bookID", b.bookID);
                    book.SetAttributeValue("addedDate", b.addedDate);
                    book.SetAttributeValue("bookTypeID", b.bookType.bookTypeID);
                    book.SetAttributeValue("name", b.name);
                    book.SetAttributeValue("updatedDate", b.updatedDate);
                    elementBooks.Add(book);
                }
                root.Add(elementBooks);

                var elementTomes = new XElement("tomes");
                IEnumerator tomes = data["tomes"];
                while (tomes.MoveNext())
                {
                    datawrapper.Tome t = (datawrapper.Tome)tomes.Current;
                    var tome = new XElement("Tome");
                    tome.SetAttributeValue("addedDate", t.addedDate.ToString());
                    tome.SetAttributeValue("bookID", t.book.bookID);
                    tome.SetAttributeValue("deleted", t.deleted);
                    tome.SetAttributeValue("information", t.information);
                    tome.SetAttributeValue("title", t.title);
                    tome.SetAttributeValue("tomeID", t.tomeID);
                    tome.SetAttributeValue("tomeNumber", t.tomeNumber);
                    tome.SetAttributeValue("updatedDate", t.updatedDate);
                    elementTomes.Add(tome);
                }
                root.Add(elementTomes);

                var elementChapters = new XElement("chapters");
                IEnumerator chapters = data["chapters"];
                while (chapters.MoveNext())
                {
                    datawrapper.Chapter c = (datawrapper.Chapter)chapters.Current;
                    var chapter = new XElement("Chapter");
                    chapter.SetAttributeValue("addedDate", c.addedDate.ToString());
                    chapter.SetAttributeValue("chapterID", c.chapterID);
                    chapter.SetAttributeValue("chapterNumber", c.chapterNumber);
                    chapter.SetAttributeValue("deleted", c.deleted);
                    chapter.SetAttributeValue("title", c.title);
                    chapter.SetAttributeValue("tomeID", c.tome.tomeID);
                    chapter.SetAttributeValue("updatedDate", c.updatedDate.ToString());
                    elementChapters.Add(chapter);
                }
                root.Add(elementChapters);

                var elementEvents = new XElement("events");
                IEnumerator events = data["events"];
                while (events.MoveNext())
                {
                    datawrapper.Event eve = (datawrapper.Event)events.Current;
                    var ev = new XElement("Event");
                    ev.SetAttributeValue("chapterID", eve.chapter.chapterID);
                    ev.SetAttributeValue("deleted", eve.deleted);
                    ev.SetAttributeValue("eventID", eve.eventID);
                    ev.SetAttributeValue("finaltext", eve.finaltext);
                    ev.SetAttributeValue("orderInChapter", eve.orderInChapter);
                    ev.SetAttributeValue("title", eve.title);
                    elementChapters.Add(ev);
                }
                root.Add(elementEvents);

                var elementNotes = new XElement("notes");
                IEnumerator notes = data["notes"];
                while (notes.MoveNext())
                {
                    datawrapper.MemoryNote mn = (datawrapper.MemoryNote)notes.Current;
                    var note = new XElement("Note");
                    note.SetAttributeValue("addedDate", mn.addedDate);
                    note.SetAttributeValue("associated", mn.associated);
                    note.SetAttributeValue("contentAudioString", mn.contentAudioString);
                    note.SetAttributeValue("contentImageString", mn.contentImageString);
                    note.SetAttributeValue("contentText", mn.contentText);
                    note.SetAttributeValue("deleted", mn.deleted);
                    note.SetAttributeValue("eventID", mn.fk_eventID);
                    note.SetAttributeValue("typeObjectID", mn.fk_typeObjectID);
                    note.SetAttributeValue("location", mn.location);
                    note.SetAttributeValue("tags", mn.tags);
                    note.SetAttributeValue("title", mn.title);
                    note.SetAttributeValue("updatedDate", mn.updatedDate);
                    note.SetAttributeValue("memoryNoteID", mn.memoryNoteID);

                    elementNotes.Add(note);
                }
                root.Add(elementNotes);


                var elementTypes = new XElement("types");
                IEnumerator types = data["types"];
                while (types.MoveNext())
                {
                    datawrapper.Type t = (datawrapper.Type)types.Current;
                    var type = new XElement("Type");
                    type.SetAttributeValue("color", t.color);
                    type.SetAttributeValue("imageString", t.imageString);
                    type.SetAttributeValue("title", t.title);
                    type.SetAttributeValue("typeID", t.typeID);

                    elementTypes.Add(type);
                }
                root.Add(elementTypes);

                var elementTypeObjects = new XElement("typeObjects");
                IEnumerator typeObjects = data["typeObjects"];
                while (typeObjects.MoveNext())
                {
                    datawrapper.TypeObject to = (datawrapper.TypeObject)typeObjects.Current;
                    var typeObject = new XElement("TypeObject");
                    typeObject.SetAttributeValue("color", to.color);
                    typeObject.SetAttributeValue("imageString", to.imageString);
                    typeObject.SetAttributeValue("name", to.name);
                    typeObject.SetAttributeValue("typeID", to.type.typeID);
                    typeObject.SetAttributeValue("typeObjectID", to.typeObjectID);
                    typeObject.SetAttributeValue("used", to.used);
                    elementTypeObjects.Add(typeObject);
                }
                root.Add(elementTypeObjects);

                doc.Add(root);
                doc.Save(ms);
                ms.Position = 0;
            

            
            _client.UploadFileAsync("/", "backup.xml", ms,
                        (response) =>
                        {
                            MessageBox.Show("Backup fertiggestellt!");
                        },
                        (error) =>
                        {
                            MessageBox.Show("Beim Erstellen des Backups ist ein Fehler aufgetreten. Möglicherweise ist die Verbindung abgebrochen oder die Dropbox ist voll.");
                        }
                        );
        }

        /// <summary>
        /// Speichert UserToken, damit der User die App nicht bei jedem Neustart authorisieren muss.
        /// </summary>
        /// <param name="secret"></param>
        /// <param name="token"></param>
        private void saveUserCredentials(String secret, String token) {
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

        private String loadUserCredentials() {
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

        private Dictionary<String, IEnumerator> getExportData() {
            ExportViewModel evm = new ExportViewModel();
            return evm.exportData;
        }
    }
}