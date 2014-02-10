using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WritersToolbox.datawrapper;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Data.Linq;
using System.Windows.Media.Imaging;
using Microsoft.Phone.BackgroundAudio;
using System.IO;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Media.PhoneExtensions;
using System.Diagnostics;
namespace WritersToolbox.viewmodels
{
    /// <summary>
    /// Die AddNoteViewModel Klasse bzw. Präsentations-Logik ist eine aggregierte Datenquelle,
    /// die verschiedene Daten von MemoryNote und ihre entsprechende Eigenschaften bereitstellt.
    /// </summary>
    class AddNoteViewModel 
    {
        //Separator zwischen den Paths der Bilder oder Memos einer Notiz.
        private const char SEPARATOR = '|';
        //Datenbank variable.
        private models.WritersToolboxDatebase db;
        //Notizen Tabelle.
        private Table<models.MemoryNote> memoryNote;
        //Notiz, als Entity object.
        private models.MemoryNote obj_memoryNote;
        

        /// <summary>
        /// Im Defaultkonstruktor wird die Verbindung zur Datenbank erstellt und
        /// Model MemoryNote instanziiert.
        /// </summary>
        public AddNoteViewModel () 
        {
            try
            {
                db = models.WritersToolboxDatebase.getInstance();
                memoryNote = db.GetTable<models.MemoryNote>();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }

        /// <summary>
        /// Title eine Notiz zurückgeben.
        /// </summary>
        /// <param name="memoryNoteID">Übergegebene NotizID</param>
        /// <returns>Liefert Title als String</returns>
        public string getTitle(int memoryNoteID)
        {
            string title = (from note in memoryNote
                            where note.memoryNoteID == memoryNoteID
                            select note.title).FirstOrDefault();

            return title == null ? "" : title;
        }
        
        /// <summary>
        /// Details einer Notiz zurücklieferen.
        /// </summary>
        /// <param name="memoryNoteID">Übergegebene Notiz</param>
        /// <returns>liefert Details einer Notiz als String zurück</returns>
        public string getDetails(int memoryNoteID)
        {
            string details = (from note in memoryNote
                              where note.memoryNoteID == memoryNoteID
                              select note.contentText).FirstOrDefault();

            return details == null ? "" : details;
        }

        /// <summary>
        /// liefert die Schlagwörter einer Notiz zurück.
        /// </summary>
        /// <param name="memoryNoteID">Übergegebene Notiz</param>
        /// <returns>liefert die Schlagwörter einer Notiz als String zurück.</returns>
        public string getTags(int memoryNoteID)
        {
            string tags = (from note in memoryNote
                           where note.memoryNoteID == memoryNoteID
                           select note.tags).FirstOrDefault();

            return tags == null ? "" : tags;
        }

        /// <summary>
        /// Die getImages()-Methode, liefert eine Liste von Image-Objekten, die zu einer Notiz gehören.
        /// </summary>
        /// <param name="memoryNoteID">Primäteschlußel der Notiz</param>
        /// <returns>Lifert eine Collection von Images, die zu der Notiz gehören</returns>
        public ObservableCollection<MyImage> getImages(int memoryNoteID)
        {
            //ImagesPath der Notiz von Table memoryNote abfragen.
            string imagePath = (from note in memoryNote
                                where note.memoryNoteID == memoryNoteID
                                select note.ContentImageString).FirstOrDefault();

            ObservableCollection<MyImage> imageList = new ObservableCollection<MyImage>();

            if (imagePath != null)
            {
                //Text splitern.
                string[] str_result = imagePath.Split(SEPARATOR);

                for (int i = 0; i < str_result.Length; i++)
                {

                    try
                    {
                        //Images von Medialibrary holen und in der Collection speichern.
                        MediaLibrary medianbibliothek = new MediaLibrary();

                        Picture picture = medianbibliothek.Pictures.Where(p =>
                            p.GetPath().Equals(str_result[i])).Single();

                        MyImage foto = new MyImage(picture);
                        imageList.Add(foto);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }


                }
            }

            return imageList;
        }

        /// <summary>
        /// Erweiterung der getImages()-Methode, liefert eine Liste von Image-Objekten, die zu einer Notiz gehören.
        /// Wegen Navigation zwischen Screnns bei einer Notiz werden, die Bilder die hinzugefügt oder gelöscht sind, verloren,
        /// deswegen wurde diese überladene Methode geschrieben, um diese temporale Änderung zu speichern.
        /// </summary>
        /// <param name="memoryNoteID">Primäreschlußel der Notiz</param>
        /// <param name="deletedImages">Die Images die von Notiz gelöscht wurden</param>
        /// <param name="addImages">Die Images die zur Notiz hinzugefügt wurden</param>
        /// <returns></returns>
        public ObservableCollection<MyImage> getImages(int memoryNoteID, string deletedImages, string addImages)
        {
            //ImagesPath der Notiz von Table memoryNote abfragen.
            string imagePath = (from note in memoryNote
                                where note.memoryNoteID == memoryNoteID
                                select note.ContentImageString).FirstOrDefault();

            ObservableCollection<MyImage> imageList = new ObservableCollection<MyImage>();

            if (imagePath != null)
            {

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

                for (int i = 0; i < str_result.Count; i++)
                {
                    try
                    {
                        MediaLibrary medianbibliothek = new MediaLibrary();
                        Picture picture = medianbibliothek.Pictures.Where(p =>
                            p.GetPath().Equals(str_result[i])).Single();
                        MyImage foto = new MyImage(picture);
                        imageList.Add(foto);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }
            }

            if (imagePath == null && addImages.Length > 0)
            {
                List<String> str_result_addImages = addImages.Split(SEPARATOR).ToList<String>();
                if (deletedImages.Length > 0)
                {
                    List<String> str_result_deletedImages = deletedImages.Split(SEPARATOR).ToList<String>();
                    foreach (string str_item in str_result_deletedImages)
                    {
                        str_result_addImages.Remove(str_item);
                    }
                }
                for (int i = 0; i < str_result_addImages.Count; i++)
                {
                    try
                    {
                        MediaLibrary medianbibliothek = new MediaLibrary();
                        Picture picture = medianbibliothek.Pictures.Where(p =>
                            p.GetPath().Equals(str_result_addImages[i])).Single();
                        MyImage foto = new MyImage(picture);
                        imageList.Add(foto);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }
            }


            return imageList;
        }

        /// <summary>
        /// Die getAudios()-Methode, liefert eine Liste von Audio-Objekten, die zu einer Notiz gehören.
        /// </summary>
        /// <param name="memoryNoteID"></param>
        /// <returns></returns>
        public ObservableCollection<SoundData> getAudios(int memoryNoteID)
        {

            string audioPath = (from note in memoryNote
                                where note.memoryNoteID == memoryNoteID
                                select note.contentAudioString).FirstOrDefault();

            ObservableCollection<SoundData> audioList = new ObservableCollection<SoundData>();
            if (audioPath != null)
            {
                string[] str_result = audioPath.Split(SEPARATOR);
                for (int i = 0; i < str_result.Length; i++)
                {
                    if (!str_result[i].Trim().Equals(""))
                    {
                        audioList.Add(new SoundData() { filePath = str_result[i] });
                    }

                }
            }
            return audioList;
        }

        /// <summary>
        /// Die save-Methode() speichert und ändert Notize, die nicht zugeordnet sind.
        /// </summary>
        /// <param name="memoryNoteID">Primäreschlußel der Notiz wenn es zu verfügung ist, sonst 0</param>
        /// <param name="addedDate">Erstelldatum</param>
        /// <param name="title">Title</param>
        /// <param name="contentText">Details</param>
        /// <param name="contentImages">Collection von Images</param>
        /// <param name="contentAudios">Collection von Memos</param>
        /// <param name="tags">Schlagwörter</param>
        /// <param name="updatedDate">Änderungsdatum</param>
        public void save(int memoryNoteID,DateTime addedDate, string title, string contentText, ObservableCollection<MyImage> contentImages,
            ObservableCollection<SoundData> contentAudios, string tags, DateTime updatedDate)
        {
            try
            {
                if (memoryNoteID == 0) //neue MemoryNote speichern.
                {
                    obj_memoryNote = new models.MemoryNote();
                    obj_memoryNote.addedDate = new DateTime(addedDate.Year, addedDate.Month, addedDate.Day);
                    obj_memoryNote.title = title;
                    obj_memoryNote.contentText = contentText;

                    string contentImagesPath = "";
                    foreach (MyImage img in contentImages)
                    {
                        contentImagesPath += img.path + "|";
                    }
                    obj_memoryNote.ContentImageString = contentImagesPath;

                    string contentAudiosPath = "";
                    foreach (SoundData track in contentAudios)
                    {
                        contentAudiosPath += track.filePath + "|";
                    }
                    obj_memoryNote.contentAudioString = contentAudiosPath;

                    obj_memoryNote.tags = tags;

                    obj_memoryNote.updatedDate = new DateTime(updatedDate.Year, updatedDate.Month, updatedDate.Day); ;
                    obj_memoryNote.associated = false;

                    //obj_memoryNote in DataContext hinzufügen.
                    db.GetTable<models.MemoryNote>().InsertOnSubmit(obj_memoryNote);
                }
                else //Änderungen an einer MemoryNote speichern.
                {
                    obj_memoryNote = db.GetTable<models.MemoryNote>().Single(memoryNote => memoryNote.memoryNoteID == memoryNoteID);

                    obj_memoryNote.title = title;
                    obj_memoryNote.contentText = contentText;

                    string contentImagesPath = "";
                    foreach (MyImage img in contentImages)
                    {
                        contentImagesPath += img.path + "|";
                    }
                    obj_memoryNote.ContentImageString = contentImagesPath;

                    string contentAudiosPath = "";
                    foreach (SoundData track in contentAudios)
                    {
                        contentAudiosPath += track.filePath + "|";
                    }
                    obj_memoryNote.contentAudioString = contentAudiosPath;

                    obj_memoryNote.tags = tags;

                    obj_memoryNote.updatedDate = new DateTime(updatedDate.Year, updatedDate.Month, updatedDate.Day); ;
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

        /// <summary>
        /// Die saveAsTypeObject()-Methode speichert und ändert eine Notiz, die zu einem TypObjekt zugeordnet ist.
        /// </summary>
        /// <param name="memoryNoteID">Primäreschlußel der Notiz wenn es zu verfügung ist, sonst 0</param>
        /// <param name="addedDate">Erstelldatum</param>
        /// <param name="title">Title</param>
        /// <param name="contentText">Details</param>
        /// <param name="contentImages">Collection von Images</param>
        /// <param name="contentAudios">Collection von Memos</param>
        /// <param name="tags">Schlagwörter</param>
        /// <param name="updatedDate">Änderungsdatum</param>
        /// <param name="typeObjectID">Schlüßel des TypObjects</param>
        public void saveAsTypeObject(int memoryNoteID, DateTime addedDate, string title, string contentText, ObservableCollection<MyImage> contentImages,
            ObservableCollection<SoundData> contentAudios, string tags, DateTime updatedDate, int typeObjectID)
        {
            try
            {
                if (memoryNoteID == 0) //neue MemoryNote speichern.
                {
                    obj_memoryNote = new models.MemoryNote();
                    obj_memoryNote.addedDate = new DateTime(addedDate.Year, addedDate.Month, addedDate.Day);
                    obj_memoryNote.title = title;
                    obj_memoryNote.contentText = contentText;

                    string contentImagesPath = "";
                    foreach (MyImage img in contentImages)
                    {
                        contentImagesPath += img.path + "|";
                    }
                    obj_memoryNote.ContentImageString = contentImagesPath;

                    string contentAudiosPath = "";
                    foreach (SoundData track in contentAudios)
                    {
                        contentAudiosPath += track.filePath + "|";
                    }
                    obj_memoryNote.contentAudioString = contentAudiosPath;

                    obj_memoryNote.tags = tags;

                    obj_memoryNote.updatedDate = new DateTime(updatedDate.Year, updatedDate.Month, updatedDate.Day); ;
                    obj_memoryNote.associated = true;

                    //Foreign key speichern
                    models.TypeObject temp_typeobject = db.GetTable<models.TypeObject>().Single(typeObject => typeObject.typeObjectID == typeObjectID);
                    obj_memoryNote.obj_TypeObject = temp_typeobject;

                    //obj_memoryNote in DataContext hinzufügen.
                    db.GetTable<models.MemoryNote>().InsertOnSubmit(obj_memoryNote);
                }
                else //Änderungen an einer MemoryNote speichern.
                {
                    obj_memoryNote = db.GetTable<models.MemoryNote>().Single(memoryNote => memoryNote.memoryNoteID == memoryNoteID);

                    obj_memoryNote.title = title;
                    obj_memoryNote.contentText = contentText;

                    string contentImagesPath = "";
                    foreach (MyImage img in contentImages)
                    {
                        contentImagesPath += img.path + "|";
                    }
                    obj_memoryNote.ContentImageString = contentImagesPath;

                    string contentAudiosPath = "";
                    foreach (SoundData track in contentAudios)
                    {
                        contentAudiosPath += track.filePath + "|";
                    }
                    obj_memoryNote.contentAudioString = contentAudiosPath;

                    obj_memoryNote.tags = tags;

                    obj_memoryNote.updatedDate = new DateTime(updatedDate.Year, updatedDate.Month, updatedDate.Day); ;
                    obj_memoryNote.associated = true;

                    //Foreign key speichern
                    models.TypeObject temp_typeobject = db.GetTable<models.TypeObject>().Single(typeObject => typeObject.typeObjectID == typeObjectID);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="memoryNoteID"></param>
        /// <param name="addedDate"></param>
        /// <param name="title"></param>
        /// <param name="contentText"></param>
        /// <param name="contentImages"></param>
        /// <param name="contentAudios"></param>
        /// <param name="tags"></param>
        /// <param name="updatedDate"></param>
        /// <param name="eventID"></param>
        public void saveAsEvent(int memoryNoteID, DateTime addedDate, string title, string contentText, List<Image> contentImages,
             List<AudioTrack> contentAudios, string tags, DateTime updatedDate, int eventID)
        {
            try
            {
                if (memoryNoteID == -1) //neue MemoryNote speichern.
                {
                    obj_memoryNote = new models.MemoryNote();
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
                    models.Event temp_event = db.GetTable<models.Event>().Single(_event => _event.eventID == eventID);
                    obj_memoryNote.obj_Event = temp_event;

                    //obj_memoryNote in DataContext hinzufügen.
                    db.GetTable<models.MemoryNote>().InsertOnSubmit(obj_memoryNote);
                }
                else //Änderungen an einer MemoryNote speichern.
                {
                    obj_memoryNote = db.GetTable<models.MemoryNote>().Single(memoryNote => memoryNote.memoryNoteID == memoryNoteID);

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
                    models.Event temp_event = db.GetTable<models.Event>().Single(_event => _event.eventID == eventID);
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

    }
}
