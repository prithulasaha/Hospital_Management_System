using HMSphere.Domain.Enums;

namespace HMSphere.MVC.ViewModels
{
	public class ShiftViewModel
	{
        public int Id { get; set; }
		public ShiftType Type { get; set; }
		public TimeOnly StartTime { get; set; }
		public TimeOnly EndTime { get; set; }
		public string? Notes { get; set; }
		public bool IsActive { get; set; }
        public string Status
        {
            get
            {
                var currentTime = TimeOnly.FromDateTime(DateTime.Now);
                if (EndTime < StartTime)
                {
                    if (currentTime >= StartTime || currentTime <= EndTime)
                    {
                        return "Active Shift";
                    }
                }
                else
                {
                    if (currentTime >= StartTime && currentTime <= EndTime)
                    {
                        return "Active Shift";
                    }
                }
                return "Inactive Shift";
            }
        }
        public TimeSpan Duration
        {
            get
            {
                var duration = EndTime.ToTimeSpan() - StartTime.ToTimeSpan();
                if (duration < TimeSpan.Zero)
                {
                    duration += TimeSpan.FromHours(24);
                }
                return duration;
            }
        }
    }
}