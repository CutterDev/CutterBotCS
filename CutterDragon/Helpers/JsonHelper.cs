using System;
using System.IO;
using System.Text.Json;

namespace CutterDragon.Helpers
{
    /// <summary>
    /// Json Helper
    /// </summary>
    public static class JsonHelper
    {
        /// <summary>
        /// Try Serialize To File
        /// </summary>    
        public static bool TrySerializeToFile<T>(T obj, string path)
        {
            bool result = false;

            string json;
            if (TrySerialize<T>(obj, out json))
            {
                try
                {
                    File.WriteAllText(path, json);
                    result = true;
                }
                catch (Exception e)
                {

                }
            }


            return result;
        }

        /// <summary>
        /// Serialize
        /// </summary>
        public static bool TrySerialize<T>(T obj, out string json)
        {
            bool result = false;
            json = string.Empty;

            try
            {
                json = JsonSerializer.Serialize(obj);
                result = true;
            }
            catch(Exception e)
            {

            }

            return result;
        }

        /// <summary>
        /// Try Deserialize
        /// </summary>
        public static bool TryDeserialize<T>(string json, out T obj)
        {
            bool result = false;
            obj = default(T);

            try
            {
                obj = JsonSerializer.Deserialize<T>(json);
                result = true;
            } 
            catch(Exception e)
            {

            }

            return result;
        }

        /// <summary>
        /// Serialize To File
        /// </summary>
        public static bool SerializeToFile<T>(T obj, string path)
        {
            bool result = false;
            string json;
            if (TrySerialize(obj, out json))
            {
                try
                {
                    File.WriteAllText(path, json);
                    result = true;
                }
                catch(Exception e)
                {

                }
            }

            return result;
        }

        /// <summary>
        /// Deserialize From File
        /// </summary>
        public static bool DeserializeFromFile<T>(string path, out T obj)
        {
            bool result = false;
            string jsontxt = null;
            obj = default(T);

            try
            {
                jsontxt = File.ReadAllText(path);

                // Check text existed in file and we can deserialize
                result = !string.IsNullOrWhiteSpace(jsontxt) &&
                             TryDeserialize<T>(jsontxt, out obj);
            }
            catch(Exception e)
            {

            }

            return result;
        }
    }
}
