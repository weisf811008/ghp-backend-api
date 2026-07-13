namespace GhpAPI.DTOs
{
    /// <summary>操作紀錄</summary>
    public class HistoryDto
    {
        /// <summary>紀錄 ID</summary>
        public int Id { get; set; }
        /// <summary>操作時間</summary>
        public DateTime Timestamp { get; set; }
        /// <summary>紀錄等級（info、warn、error）</summary>
        public string Level { get; set; } = null!;
        /// <summary>操作描述</summary>
        public string Message { get; set; } = null!;
        /// <summary>類型（auth、system，一般操作為 null）</summary>
        public string? Type { get; set; }
        /// <summary>來源功能</summary>
        public string? Controller { get; set; }
        /// <summary>操作的資料 ID</summary>
        public string? InstanceKey { get; set; }
        /// <summary>操作者帳號</summary>
        public string? Username { get; set; }
        /// <summary>操作者姓名</summary>
        public string? Name { get; set; }
        /// <summary>操作者學校 ID</summary>
        public int? SchoolId { get; set; }
    }
}
