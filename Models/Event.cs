using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Play2GetherAPI.Models
{
    [DataContract]
    public class Event
    {
        public Event()
        {
            UsersEvents = new List<UserEvent>();
            Messages = new List<Message>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DataMember]
        public long EventId { get; set; }
        [DataMember]
        [Column(TypeName = "Date")]
        public DateTime TimeDate { get; set; }
        [DataMember]
        public uint Vacancies { get; set; }
        [DataMember]
        [ForeignKey("Organizer")]
        public long UserId { get; set; }
        [DataMember]
        [ForeignKey("Spot")]
        public long PlaceId { get; set; }
        [DataMember]
        [ForeignKey("ActivitieEvent")]
        public long ActivitieId { get; set; }
        public virtual User Organizer { get; set; }
        public virtual ICollection<UserEvent> UsersEvents { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        public virtual Place Spot { get; set; }
        public virtual Activitie ActivitieEvent { get; set; }
    }
}
