﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace linc.Data;

[Index(nameof(Key))]
public class ApplicationStringResource
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    [JsonPropertyName("key")]
    public string Key { get; set; }

    [Required]
    [JsonPropertyName("value")]
    public string Value { get; set; }

    [Required]
    [ForeignKey(nameof(Language))]
    public int LanguageId { get; set; }

    public ApplicationLanguage Language { get; set; }

    [Required]
    [ForeignKey(nameof(EditedBy))]
    [JsonPropertyName("editedById")]
    public string EditedById { get; set; }
    
    public ApplicationUser EditedBy { get; set; }
    
    public DateTime LastEdited { get; set; }
}