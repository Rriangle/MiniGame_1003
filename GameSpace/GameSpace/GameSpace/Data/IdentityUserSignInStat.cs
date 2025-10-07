using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameSpace.Data
{
    [Table("UserSignInStat")]
    public class IdentityUserSignInStat
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LogId { get; set; }

        public DateTime SignTime { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        public int PointsGained { get; set; }

        public DateTime PointsGainedTime { get; set; }

        public int ExpGained { get; set; }

        public DateTime ExpGainedTime { get; set; }

        [MaxLength(200)]
        public string CouponGained { get; set; } = string.Empty;

        public DateTime CouponGainedTime { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; } = null!;
    }
}
