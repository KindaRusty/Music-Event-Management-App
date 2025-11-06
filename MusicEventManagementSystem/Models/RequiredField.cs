using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicEventManagementSystem.Models
{
    public class RequiredField
    {
        [Key]
        public int FieldID { get; set; }

        [Required]
        [StringLength(100)]
        public string FieldName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string FieldType { get; set; } = "Text";

        public bool IsRequired { get; set; } = true;
        public int DisplayOrder { get; set; } = 0; 

        public int EventID { get; set; }
        [ForeignKey("EventID")]
        public virtual MusicEvent? MusicEvent { get; set; }

        // Navigation properties
        public virtual ICollection<RegistrationData> RegistrationData { get; set; } = new List<RegistrationData>();
    }
}