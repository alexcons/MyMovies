﻿using System;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using System.Xml;
using System.Xml.Serialization;

namespace Konz.MyMovies.Core
{
    public class PersistableFile<T>
    {
        #region Properties

        public string FileName { get; set; }

        public T Data { get; set; }

        #endregion

        #region Public Methods

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
                OnLoaded(e.Result as PersistableFile<T>, e.Error);
            };

            worker.RunWorkerAsync();
        }

        public bool Delete()
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
                        XmlWriter xmlWriter = XmlWriter.Create(stream, xmlWriterSettings);
                        serializer.Serialize(xmlWriter, this);
                    }
                    catch
                    {
                        return;
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
                    return null;

                using (var stream = myIsolatedStorage.OpenFile(FileName, System.IO.FileMode.Open))
                {
                    try
                    {
                        var serializer = new XmlSerializer(typeof(PersistableFile<T>));
                        XmlReader xmlReader = XmlReader.Create(stream);
                        result = serializer.Deserialize(xmlReader) as PersistableFile<T>;
                    }
                    catch
                    {
                        return null;
                    }
                }
            }
            return result;
        }

        #endregion
    }
}
