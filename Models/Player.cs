using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AppEquiposBackend.Models
{
    public class Player
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        [Range(16, 50)]
        public int Age { get; set; }

        [Required]
        [StringLength(50)]
        public string Country { get; set; }

        [Required]
        [StringLength(50)]
        public string City { get; set; }

        [Range(0, double.MaxValue)]
        public double Salary { get; set; }

        // Foreign key for Team
        [ForeignKey("Team")]
        public int TeamId { get; set; }

        [JsonIgnore]
        public virtual Team? Team { get; set; }

        [JsonIgnore]
        [NotMapped]
        public string TeamName => Team?.Name ?? "No team";
    }
}
