using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Play2GetherAPI.Models
{

    [DataContract]
    public class Premium
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DataMember]
        public long PremiumId { get; set; }
        [DataMember]
        public DateTime StartDate { get; set; }
        [DataMember]
        public int Days { get; set; }
        [ForeignKey("UserPremium")]
        public long UserId { get; set; }
        public virtual User UserPremium { get; set; }

    }
}
