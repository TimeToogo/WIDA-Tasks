using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Xml.Serialization;
namespace WIDA.Utes
{
    //The serializer is used to store parameters for each definition in a file
    public class ObjectSerializer
    {
        public string SerializeObject(object Object)
        {
            MemoryStream Stream = new MemoryStream();
            BinaryFormatter Formatter = new BinaryFormatter();
            Formatter.Serialize(Stream, Object);
            string ReturnVal = Convert.ToBase64String(Stream.ToArray());
            Stream.Close();
            Stream.Dispose();

            return ReturnVal;
        }

        public object DeserializeString(string String)
        {
            MemoryStream Stream = new MemoryStream(Convert.FromBase64String(String));
            BinaryFormatter Formatter = new BinaryFormatter();
            object ReturnVal = Formatter.Deserialize(Stream);
            Stream.Close();
            Stream.Dispose();

            return ReturnVal;
        }
    }
}
