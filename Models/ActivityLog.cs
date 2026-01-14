namespace EmployeeManagementSystem.Models
{
    public class ActivityLog
    {
        public int Id { get; set; }
        public required string UserId { get; set; }
        public required string Action { get; set; }
        public required string EntityName { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
