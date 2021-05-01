using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRchat.Models
{
    public class Question
    {
        [Key]
        public int QuestionId { get; set; }
        [Required]
        [Column(TypeName = "VARCHAR(100)")]
        public string Title { get; set; }
    }
}
