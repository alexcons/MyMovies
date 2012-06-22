using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Konz.MyMovies.Core
{
    public class PersistableFile <T>
    {
        public string FileName { get; set; }
        public T Data { get; set; }

        public void Delete(Action<bool, Exception> OnDeleted)
        {
            var worker = new BackgroundWorker();
            
            worker.DoWork += delegate(object sender, DoWorkEventArgs e)
            {
                e.Result = false;
                using (var myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (myIsolatedStorage.FileExists(FileName))
                    {
                        myIsolatedStorage.DeleteFile(FileName);
                        e.Result = true;
                    }
                }                
            };

            worker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e)
            {
                OnDeleted((bool)e.Result, e.Error);
            };
                
            worker.RunWorkerAsync();
        }

        public void Save(Action<Exception> OnSaved)
        {
            var worker = new BackgroundWorker();
            worker.DoWork += delegate(object sender, DoWorkEventArgs e)
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
                e.Result = result;
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

    }
}
