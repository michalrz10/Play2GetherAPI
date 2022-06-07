using Play2GetherAPI.ControllerModels;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Play2GetherAPI.Models
{
    [DataContract]
    public class Place
    {
        public Place()
        {
            Events = new List<Event>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DataMember]
        public long PlaceId { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public float Latitude { get; set; }
        [DataMember]
        public float Longitude { get; set; }
        [DataMember]
        [NotMapped]
        public Point geometry 
        { 
            get 
            { 
                return (new Point(Longitude, Latitude)); 
            }
            set { } 
        }

        [DataMember]
        public string ImageUrl { get; set; }
        public virtual ICollection<Event> Events { get; set; }
        public virtual ICollection<PlaceActivitie> PlaceActivities { get; set; }
    }
}
