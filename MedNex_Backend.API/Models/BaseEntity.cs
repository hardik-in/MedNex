namespace MedNex_Backend.API.Models.Entities
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public Guid PublicId { get; set; }

        [System.ComponentModel.DataAnnotations.MaxLength(20)]
        public string? ReferenceId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}