using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Play2GetherAPI.Models
{
    [DataContract]
    public class Message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DataMember]
        public long MessageId { get; set; }
        [DataMember]
        public string Text { get; set; }
        [DataMember]
        [ForeignKey("UserMessage")]
        public long UserId { get; set; }
        [DataMember]
        [ForeignKey("EventMessage")]
        public long EventId { get; set; }
        public virtual User UserMessage { get; set; }
        public virtual Event EventMessage { get; set; }
    }
}
