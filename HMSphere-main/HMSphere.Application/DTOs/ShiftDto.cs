using HMSphere.Domain.Enums;

namespace HMSphere.Application.DTOs
{
    public class ShiftDto
    {
        public int Id { get; set; }
        public ShiftType Type { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}