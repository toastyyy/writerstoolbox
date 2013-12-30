using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WritersToolbox.models;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Data.Linq;
using System.Windows.Media.Imaging;
using Microsoft.Phone.BackgroundAudio;
using System.IO;
using Microsoft.Xna.Framework.Media;
namespace WritersToolbox.viewmodels
{
    /* *
     * 
     * Die AddNoteViewModel Klasse bzw. Präsentations-Logik ist eine aggregierte Datenquelle,
     * die verschiedene Daten von MemoryNote und ihre entsprechende Eigenschaften bereitstellt.
     * 
     * */
    class AddNoteViewModel 
    {
        private WritersToolboxDatebase db;
        private Table<MemoryNote> memoryNote;
        private MemoryNote obj_memoryNote;
        private ObservableCollection<Image> _imageList;
        private const char SEPARATOR = '|';


        /// <summary>
        /// A log of a starting process
        /// </summary>
        public ObservableCollection<Image> imageList
        {
            get { return _imageList; }

            set
            {
                _imageList = value;
            }
        }
        /* *
         * 
         * Im Defaultkonstruktor wird die Verbindung zur Datenbank erstellt und
         * Model MemoryNote instanziiert.
         * 
         * */
        public AddNoteViewModel () 
        {
            try
            {
                db = WritersToolboxDatebase.getInstance();
                memoryNote = db.GetTable<MemoryNote>();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }

        /* *
         * 
         * Die save-Methode() speichert und ändert Notize, die nicht zugeordnet sind.
         * 
         * */
        public void save(int memoryNoteID,DateTime addedDate, string title, string contentText, List<Image> contentImages,
            List<AudioTrack> contentAudios, string tags, DateTime updatedDate)
        {
            try
            {
                if (memoryNoteID == -1) //neue MemoryNote speichern.
                {
                    obj_memoryNote = new MemoryNote();
                    obj_memoryNote.addedDate = addedDate;
                    obj_memoryNote.title = title;
                    obj_memoryNote.contentText = contentText;

                    string contentImagesPath = "";
                    foreach (Image img in contentImages)
                    {
                        contentImagesPath += ((BitmapImage)img.Source).UriSource.ToString() + "|";
                    }
                    obj_memoryNote.ContentImageString = contentImagesPath;

                    string contentAudiosPath = "";
                    foreach (AudioTrack track in contentAudios)
                    {
                        contentAudiosPath += track.Source.AbsoluteUri.ToString() + "|";
                    }
                    obj_memoryNote.contentAudioString = contentAudiosPath;

                    obj_memoryNote.tags = tags;

                    obj_memoryNote.updatedDate = updatedDate;
                    obj_memoryNote.associated = false;

                    //obj_memoryNote in DataContext hinzufügen.
                    db.GetTable<MemoryNote>().InsertOnSubmit(obj_memoryNote);
                }
                else //Änderungen an einer MemoryNote speichern.
                {
                    obj_memoryNote = db.GetTable<MemoryNote>().Single(memoryNote => memoryNote.memoryNoteID == memoryNoteID);

                    obj_memoryNote.addedDate = addedDate;
                    obj_memoryNote.title = title;
                    obj_memoryNote.contentText = contentText;

                    string contentImagesPath = "";
                    foreach (Image img in contentImages)
                    {
                        contentImagesPath += ((BitmapImage)img.Source).UriSource.ToString() + "|";
                    }
                    obj_memoryNote.ContentImageString = contentImagesPath;

                    string contentAudiosPath = "";
                    foreach (AudioTrack track in contentAudios)
                    {
                        contentAudiosPath += track.Source.AbsoluteUri.ToString() + "|";
                    }
                    obj_memoryNote.contentAudioString = contentAudiosPath;

                    obj_memoryNote.tags = tags;

                    obj_memoryNote.updatedDate = updatedDate;
                    obj_memoryNote.associated = false;
                }


                //Änderung in der Datenbank übertragen.
                db.SubmitChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        /* *
         * 
         * Die saveAsTypeObject()-Methode speichert und ändert eine Notiz, die zu einem TypObjekt zugeordnet ist.
         * 
         * */
        public void saveAsTypeObject(int memoryNoteID, DateTime addedDate, string title, string contentText, List<Image> contentImages,
            List<AudioTrack> contentAudios, string tags, DateTime updatedDate, int typeObjectID)
        {
            try
            {
                if (memoryNoteID == -1) //neue MemoryNote speichern.
                {
                    obj_memoryNote = new MemoryNote();
                    obj_memoryNote.addedDate = addedDate;
                    obj_memoryNote.title = title;
                    obj_memoryNote.contentText = contentText;

                    string contentImagesPath = "";
                    foreach (Image img in contentImages)
                    {
                        contentImagesPath += ((BitmapImage)img.Source).UriSource.ToString() + "|";
                    }
                    obj_memoryNote.ContentImageString = contentImagesPath;

                    string contentAudiosPath = "";
                    foreach (AudioTrack track in contentAudios)
                    {
                        contentAudiosPath += track.Source.AbsoluteUri.ToString() + "|";
                    }
                    obj_memoryNote.contentAudioString = contentAudiosPath;

                    obj_memoryNote.tags = tags;

                    obj_memoryNote.updatedDate = updatedDate;
                    obj_memoryNote.associated = true;

                    //Foreign key speichern
                    TypeObject temp_typeobject = db.GetTable<TypeObject>().Single(typeObject => typeObject.typeObjectID == typeObjectID);
                    obj_memoryNote.obj_TypeObject = temp_typeobject;

                    //obj_memoryNote in DataContext hinzufügen.
                    db.GetTable<MemoryNote>().InsertOnSubmit(obj_memoryNote);
                }
                else //Änderungen an einer MemoryNote speichern.
                {
                    obj_memoryNote = db.GetTable<MemoryNote>().Single(memoryNote => memoryNote.memoryNoteID == memoryNoteID);

                    obj_memoryNote.addedDate = addedDate;
                    obj_memoryNote.title = title;
                    obj_memoryNote.contentText = contentText;

                    string contentImagesPath = "";
                    foreach (Image img in contentImages)
                    {
                        contentImagesPath += ((BitmapImage)img.Source).UriSource.ToString() + "|";
                    }
                    obj_memoryNote.ContentImageString = contentImagesPath;

                    string contentAudiosPath = "";
                    foreach (AudioTrack track in contentAudios)
                    {
                        contentAudiosPath += track.Source.AbsoluteUri.ToString() + "|";
                    }
                    obj_memoryNote.contentAudioString = contentAudiosPath;

                    obj_memoryNote.tags = tags;

                    obj_memoryNote.updatedDate = updatedDate;
                    obj_memoryNote.associated = true;

                    //Foreign key speichern
                    TypeObject temp_typeobject = db.GetTable<TypeObject>().Single(typeObject => typeObject.typeObjectID == typeObjectID);
                    obj_memoryNote.obj_TypeObject = temp_typeobject;
                }


                //Änderung in der Datenbank übertragen.
                db.SubmitChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /* *
         * 
         * Die saveAsEvent()-Methode speichert und ändert eine Notiz, die zu einem Event zugeordnet ist.
         * 
         * */
        public void saveAsEvent(int memoryNoteID, DateTime addedDate, string title, string contentText, List<Image> contentImages,
             List<AudioTrack> contentAudios, string tags, DateTime updatedDate, int eventID)
        {
            try
            {
                if (memoryNoteID == -1) //neue MemoryNote speichern.
                {
                    obj_memoryNote = new MemoryNote();
                    obj_memoryNote.addedDate = addedDate;
                    obj_memoryNote.title = title;
                    obj_memoryNote.contentText = contentText;

                    string contentImagesPath = "";
                    foreach (Image img in contentImages)
                    {
                        contentImagesPath += ((BitmapImage)img.Source).UriSource.ToString() + "|";
                    }
                    obj_memoryNote.ContentImageString = contentImagesPath;

                    string contentAudiosPath = "";
                    foreach (AudioTrack track in contentAudios)
                    {
                        contentAudiosPath += track.Source.AbsoluteUri.ToString() + "|";
                    }
                    obj_memoryNote.contentAudioString = contentAudiosPath;

                    obj_memoryNote.tags = tags;

                    obj_memoryNote.updatedDate = updatedDate;
                    obj_memoryNote.associated = true;

                    //Foreign key speichern
                    Event temp_event = db.GetTable<Event>().Single(_event => _event.eventID == eventID);
                    obj_memoryNote.obj_Event = temp_event;

                    //obj_memoryNote in DataContext hinzufügen.
                    db.GetTable<MemoryNote>().InsertOnSubmit(obj_memoryNote);
                }
                else //Änderungen an einer MemoryNote speichern.
                {
                    obj_memoryNote = db.GetTable<MemoryNote>().Single(memoryNote => memoryNote.memoryNoteID == memoryNoteID);

                    obj_memoryNote.addedDate = addedDate;
                    obj_memoryNote.title = title;
                    obj_memoryNote.contentText = contentText;

                    string contentImagesPath = "";
                    foreach (Image img in contentImages)
                    {
                        contentImagesPath += ((BitmapImage)img.Source).UriSource.ToString() + "|";
                    }
                    obj_memoryNote.ContentImageString = contentImagesPath;

                    string contentAudiosPath = "";
                    foreach (AudioTrack track in contentAudios)
                    {
                        contentAudiosPath += track.Source.AbsoluteUri.ToString() + "|";
                    }
                    obj_memoryNote.contentAudioString = contentAudiosPath;

                    obj_memoryNote.tags = tags;

                    obj_memoryNote.updatedDate = updatedDate;
                    obj_memoryNote.associated = true;

                    //Foreign key speichern
                    Event temp_event = db.GetTable<Event>().Single(_event => _event.eventID == eventID);
                    obj_memoryNote.obj_Event = temp_event;
                }


                //Änderung in der Datenbank übertragen.
                db.SubmitChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /* *
         * 
         * Die getImages()-Methode, liefert eine Liste von Image-Objekten, die zu einer Notiz gehören.
         * 
         * */
        public ObservableCollection<MyImage> getImages(int memoryNoteID)
        {

            string imagePath = (from note in memoryNote
                                where note.memoryNoteID == memoryNoteID
                                select note.ContentImageString).FirstOrDefault(); 

            
            string[] str_result = imagePath.Split(SEPARATOR);

            ObservableCollection<MyImage> imageList = new ObservableCollection<MyImage>();

            for (int i = 0; i < str_result.Length; i++)
            {
                MediaLibrary medianbibliothek = new MediaLibrary();

                foreach (Picture picture in medianbibliothek.Pictures)
                {
                    if (picture.Name.Equals(Path.GetFileName(str_result[i])))
                    {
                        MyImage foto = new MyImage(picture);
                        imageList.Add(foto);
                    }
                }
           }
            return imageList;          
        }

        /* *
 * 
 * Die getImages()-Methode, liefert eine Liste von Image-Objekten, die zu einer Notiz gehören.
 * 
 * */
        public ObservableCollection<MyImage> getImages(int memoryNoteID, string deletedImages, string addImages)
        {

            string imagePath = (from note in memoryNote
                                where note.memoryNoteID == memoryNoteID
                                select note.ContentImageString).FirstOrDefault();


            List<String> str_result = imagePath.Split(SEPARATOR).ToList<String>();
            List<String> str_result_deletedImages = deletedImages.Split(SEPARATOR).ToList<String>();
            List<String> str_result_addImages = addImages.Split(SEPARATOR).ToList<String>();

            foreach (string str_item in str_result_addImages)
            {
                str_result.Add(str_item);
            }
            foreach (string str_item in str_result_deletedImages)
            {
                str_result.Remove(str_item);
            }
            ObservableCollection<MyImage> imageList = new ObservableCollection<MyImage>();

            for (int i = 0; i < str_result.Count; i++)
            {
                MediaLibrary medianbibliothek = new MediaLibrary();
                
                foreach (Picture picture in medianbibliothek.Pictures)
                {
                    if (picture.Name.Equals(Path.GetFileName(str_result[i])))
                    {
                        MyImage foto = new MyImage(picture);
                        imageList.Add(foto);
                    }
                }
            }

            return imageList;
        }

        /* *
         * 
         * Die getAudios()-Methode, liefert eine Liste von Audio-Objekten, die zu einer Notiz gehören.
         * 
         * */
        public List<AudioTrack> getAudios(int memoryNoteID)
        {

            string audioPath = (from note in memoryNote
                                where note.memoryNoteID == memoryNoteID
                                select note.ContentImageString).FirstOrDefault();

            string[] str_result = audioPath.Split(SEPARATOR);

            List<AudioTrack> audioList = new List<AudioTrack>();

            for (int i = 0; i < str_result.Length; i++)
            {
                audioList.Add(new AudioTrack(new Uri(str_result[i]), null, null, null, null));
            }
            return audioList;
        }
    }
}
