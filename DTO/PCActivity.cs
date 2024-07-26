using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivityLookUp.DTO
{
    [Table("pcActivity", Schema = "extension")]
    public class PCActivity
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Column("processname")]
        public string? ProcessName { get; set; }

        [Column("mainwindowtitle")]
        public string? MainWindowTitle { get; set; }

        [Column("duration")]
        public double? Duration { get; set; }

        [Column("starttimestamp")]
        public DateTime? StartTimestamp { get; set; }

        [Column("createdat")]
        public DateTime? CreatedAt { get; set; }
        [Column("updatedat")]
        public DateTime? UpdatedAt { get; set; }
    }
}
