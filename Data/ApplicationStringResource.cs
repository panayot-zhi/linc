using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace linc.Data;

[Index(nameof(Key))]
public class ApplicationStringResource
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string Key { get; set; }

    [Required]
    public string Value { get; set; }

    [Required]
    [ForeignKey(nameof(Language))]
    public int LanguageId { get; set; }

    public ApplicationLanguage Language { get; set; }
}