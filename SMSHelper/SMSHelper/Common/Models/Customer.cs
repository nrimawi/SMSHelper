
using System.ComponentModel.DataAnnotations;


namespace SMSHelper.Common.DataModels
{
    public class Customer
    {
        [Key]

        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public int FingerprintId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public string StartDate { get; set; }
        [Required]
        public string EndDate { get; set; }
        [Required]
        public string LastSeen { get; set; }
        public string LastNotification { get; set; }
        [Required]
        public int AccessDoor { get; set; }
        [Required]
        public int Active { get; set; }
        public string CreatedDate { get; set; }

        public override string ToString() { return $"User id={this.UserId}, Name={this.Name}, FingerprintId= {this.FingerprintId}, AccessDoor= {this.AccessDoor}, EndDate= {this.EndDate} "; }

    }
}
