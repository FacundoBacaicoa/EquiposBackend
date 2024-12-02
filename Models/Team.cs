using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AppEquiposBackend.Models
{
    public class Team
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string Country { get; set; }

        [StringLength(50)]
        public string City { get; set; }

        [StringLength(50)]
        public string Stadium { get; set; }

        [StringLength(50)]
        public string Category { get; set; }

        [StringLength(50)]
        public string Coach { get; set; }

        // Collection of players
        [JsonIgnore]
        public virtual ICollection<Player> Players { get; set; } = new List<Player>();
    }
}
