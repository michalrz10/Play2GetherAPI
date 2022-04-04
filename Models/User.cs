using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Play2GetherAPI.Models
{
    [DataContract]
    public class User
    {
        public User()
        {
            IsActive = false;
            Role = "User";
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DataMember]
        public long UserId { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string SurName { get; set; }
        [DataMember]
        public uint Age { get; set; }
        [DataMember]
        public string ImageUrl { get; set; }
        [DataMember]
        public string Role { get; set; }
        [DataMember]
        public bool IsActive { get; set; }
        public virtual Login Login { get; set; }
        public virtual ICollection<UserEvent> UserEvents { get; set; }
        public virtual ICollection<Event> Events { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
    }
}
