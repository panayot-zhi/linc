﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace linc.Data
{
    public class ApplicationIssue
    {
        public int Id { get; set; }


        [Required]
        public int IssueNumber { get; set; }

        [Required]
        public int ReleaseYear { get; set; }

        [MaxLength(1024)]
        public string Description { get; set; }


        [ForeignKey(nameof(PdfFile))]
        public int? PdfFileId { get; set; }

        public ApplicationDocument PdfFile { get; set; }


        #region Automatic

        public DateTime LastUpdated { get; set; }

        public DateTime DateCreated { get; set; }

        #endregion
    }
}