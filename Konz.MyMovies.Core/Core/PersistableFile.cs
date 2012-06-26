using System;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using System.Xml;
using System.Xml.Serialization;

namespace Konz.MyMovies.Core
{
    public class PersistableFile<T>
    {
        public string FileName { get; set; }
        public T Data { get; set; }

        public void Delete(Action<bool, Exception> OnDeleted)
        {
            var worker = new BackgroundWorker();

            worker.DoWork += delegate(object sender, DoWorkEventArgs e)
            {
                e.Result = Delete();
            };

            worker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e)
            {
                if (e.Error == null)
                    OnDeleted((bool)e.Result, e.Error);
                else
                    OnDeleted(false, e.Error);
            };

            worker.RunWorkerAsync();
        }
        
        public void Save(Action<Exception> OnSaved)
        {
            var worker = new BackgroundWorker();
            worker.DoWork += delegate(object sender, DoWorkEventArgs e)
            {
                Save();
            };

            worker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e)
            {
                OnSaved(e.Error);
            };

            worker.RunWorkerAsync();
        }

        public static void Load(string FileName, Action<PersistableFile<T>, Exception> OnLoaded)
        {
            var worker = new BackgroundWorker();

            worker.DoWork += delegate(object sender, DoWorkEventArgs e)
            {
                e.Result = Load(FileName);
            };

            worker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e)
            {
                if (e.Error == null)
                    OnLoaded(e.Result as PersistableFile<T>, e.Error);
                else
                    OnLoaded(null, e.Error);
            };

            worker.RunWorkerAsync();
        }

        private bool Delete()
        {
            using (var myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (myIsolatedStorage.FileExists(FileName))
                {
                    myIsolatedStorage.DeleteFile(FileName);
                    return true;
                }
            }
            return false;
        }

        public void Save()
        {
            var xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.Indent = true;
            using (var myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (myIsolatedStorage.FileExists(FileName))
                    myIsolatedStorage.DeleteFile(FileName);

                using (var stream = myIsolatedStorage.CreateFile(FileName))
                {
                    try
                    {
                        var serializer = new XmlSerializer(typeof(PersistableFile<T>));
                        using (XmlWriter xmlWriter = XmlWriter.Create(stream, xmlWriterSettings))
                            serializer.Serialize(xmlWriter, this);
                    }
                    finally
                    {
                        stream.Close();
                    }
                }
            }
        }

        public static PersistableFile<T> Load(string FileName)
        {
            PersistableFile<T> result = null;
            using (var myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!myIsolatedStorage.FileExists(FileName))
                    throw new System.IO.FileNotFoundException(string.Format("The file {0} was not found", FileName));

                using (var stream = myIsolatedStorage.OpenFile(FileName, System.IO.FileMode.Open))
                {
                    try
                    {
                        var serializer = new XmlSerializer(typeof(PersistableFile<T>));
                        using (XmlReader xmlReader = XmlReader.Create(stream))
                            result = serializer.Deserialize(xmlReader) as PersistableFile<T>;
                    }
                    finally
                    {
                        stream.Close();
                    }
                }
            }
            return result;
        }

    }
}
