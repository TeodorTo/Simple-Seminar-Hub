using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace SeminarHub.Data
{
    public class SeminarParticipant
    {
        public int SeminarId { get; set; }

        [ForeignKey(nameof(SeminarId))] 
        public Seminar Seminar { get; set; } = null!;

        public string ParticipantId { get; set; } = string.Empty;

        [ForeignKey(nameof(ParticipantId))] 
        public IdentityUser Participant { get; set; } = null!;


    }
}
