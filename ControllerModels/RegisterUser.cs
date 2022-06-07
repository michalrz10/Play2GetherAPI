using Play2GetherAPI.Models;
using System.Runtime.Serialization;

namespace Play2GetherAPI.ControllerModels
{
    [DataContract]
    public class RegisterUser : Login
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string SurName { get; set; }
        [DataMember]
        public uint Age { get; set; }
    }
}
