using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace linc.Data;

[Index(nameof(Culture), IsUnique = true)]
public class ApplicationLanguage
{
    [Key]
    public int Id { get; set; }

    [MaxLength(12)]
    public string Culture { get; set; }

    public ICollection<ApplicationStringResource> StringResources { get; set; }
}