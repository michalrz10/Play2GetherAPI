using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Play2GetherAPI.Models
{

    public class EmailToken
    {
        [DataMember]
        public long EmailTokenId { get; set; }
        [DataMember]
        [ForeignKey("UserToken")]
        public long UserId { get; set; }
        [DataMember]
        public string Url { get; set; }
        public virtual User UserToken { get; set; }
    }
}
