using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Play2GetherAPI.Models
{
    [DataContract]
    public class PlaceActivitie
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DataMember]
        public long PlaceActivitieId { get; set; }
        [DataMember]
        [ForeignKey("PlaceA")]
        public long ActivitieId { get; set; }
        [DataMember]
        [ForeignKey("ActivitieP")]
        public long PlaceId { get; set; }
        public virtual Place PlaceA { get; set; }
        public virtual Activitie ActivitieP { get; set; }
    }
}
