using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CourseScheduleService.Domain.Entities
{
    public abstract class BaseModel
    {
        [Column("id")]
        [Key]
        public int Id { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [Column("is_deleted", TypeName = "bit")]
        public bool IsDeleted { get; set; } = false;
    }
}