using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicEventManagementSystem.Models
{
    public class RegistrationData
    {
        [Key]
        public long DataID { get; set; }

        [Required]
        [Column(TypeName = "TEXT")]
        public string FieldValue { get; set; } = string.Empty;

        // --- Foreign Keys ---

        [Required]
        public int RegistrationID { get; set; }
        [ForeignKey("RegistrationID")]
        public virtual EventRegistration? EventRegistration { get; set; }

        [Required]
        public int FieldID { get; set; }
        [ForeignKey("FieldID")]
        public virtual RequiredField? RequiredField { get; set; }
    }
}