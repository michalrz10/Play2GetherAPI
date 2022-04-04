using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Play2GetherAPI.Models
{
    [DataContract]
    public class Login
    {
        [ForeignKey("UserLogin")]
        [DataMember]
        public long LoginId { get; set; }
        [DataMember]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [DataMember]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public virtual User UserLogin { get; set; }

    }
}
