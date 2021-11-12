using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Archive
{
    public static class Formatter
    {
        // the filestream used in this class
        static FileStream fileStream;

        // the formatter used in this class
        static BinaryFormatter formatter = new BinaryFormatter();

        /// <summary>
        /// Method to save a player's information into the disk
        /// </summary>
        /// <param name="player">player's original data</param>
        /// <param name="archiveId">index of the archive</param>
        public static void Save(Player player, int archiveId)
        {
            // create a file in the data path
            fileStream = new FileStream(GetDataPath(archiveId), FileMode.Create);

            // serialize player's data and write into the file stream
            formatter.Serialize(fileStream, new Data(player));

            // close the stream
            fileStream.Close();
        }

        /// <summary>
        /// Method to load a player's data from the disk
        /// </summary>
        /// <param name="archiveId">index of the archive</param>
        /// <returns></returns>
        public static Data Load(int archiveId)
        {
            // get the archive file path
            var path = GetDataPath(archiveId);

            // return null if the archive file does not exist
            if (!File.Exists(path))
                return null;

            // otherwise open the file
            fileStream = new FileStream(path, FileMode.Open);

            // create a data variable to collect data from the file stream
            Data data = formatter.Deserialize(fileStream) as Data;

            // close the stream
            fileStream.Close();

            return data;
        }

        /// <summary>
        /// Method to remove a archive file based on the given index
        /// </summary>
        /// <param name="archiveId">index of the archive</param>
        public static void Remove(int archiveId)
        {
            // get the archive file path
            var path = GetDataPath(archiveId);

            // delete the archive file if it exists
            if (File.Exists(path))
                File.Delete(path);
        }

        public static string GetDataPath(int archiveId) => Application.persistentDataPath + $"/archive{archiveId}.arc";
    }
}


