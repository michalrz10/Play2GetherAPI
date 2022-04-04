using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Play2GetherAPI.Models
{
    [DataContract]
    public class UserEvent
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DataMember]
        public long UserEventId { get; set; }
        [DataMember]
        [ForeignKey("UserE")]
        public long UserId { get; set; }
        [DataMember]
        [ForeignKey("EventU")]
        public long EventId { get; set; }
        public virtual User UserE { get; set; }
        public virtual Event EventU { get; set; }

    }
}
