﻿using System.ComponentModel.DataAnnotations;
using linc.Models.Enumerations;

namespace linc.Data
{
    public class ApplicationDocument
    {
        public int Id { get; set; }


        [Required]
        [MaxLength(256)]
        public string Title { get; set; }


        [Required]
        [MaxLength(127)]
        public string FileName { get; set; }

        [Required]
        [MaxLength(32)]
        public string Extension { get; set; }

        [Required]
        [MaxLength(128)]
        public string MimeType { get; set; }


        [Required]
        public ApplicationDocumentType DocumentType { get; set; }


        [Required]
        [MaxLength(512)]
        public string RelativePath { get; set; }
    }
}