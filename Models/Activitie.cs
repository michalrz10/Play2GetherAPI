using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Play2GetherAPI.Models
{
    [DataContract]
    public class Activitie
    {
        public Activitie()
        {
            Events = new List<Event>();
        }

        [DataMember]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ActivitieId { get; set; }
        [DataMember]
        public string Name { get; set; }
        public virtual ICollection<Event> Events { get; set; }
        public virtual ICollection<PlaceActivitie> PlaceActivities { get; set; }
    }
}
