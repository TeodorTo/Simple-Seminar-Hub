using Microsoft.AspNetCore.Identity;
using SeminarHub.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using static SeminarHub.Data.DataConstants;

namespace SeminarHub.Models
{
    public class SeminarEditViewModel
    {

        [Required]
        [StringLength(SeminarTopicMaxLength, MinimumLength = SeminarTopicMinLength)]
        public string Topic { get; set; } = string.Empty;

        [Required]
        [StringLength(SeminarLecturerMaxLength, MinimumLength = SeminarLecturerMinLength)]
        public string Lecturer { get; set; } = string.Empty;

        [Required]
        [StringLength(SeminarDetailsMaxLength, MinimumLength = SeminarDetailsMinLength)]
        public string Details { get; set; } = string.Empty;

        [Required]
        public string OrganizerId { get; set; } = string.Empty;

        [Required]
        public DateTime DateAndTime { get; set; }

        [Range(SeminarDurationMinValue, SeminarDurationMaxValue)]
        public int Duration { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public IEnumerable<CategoriesViewModel> Categories { get; set; } = new List<CategoriesViewModel>();
    }
}

