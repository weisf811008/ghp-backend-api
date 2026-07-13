namespace GhpAPI.DTOs
{
    /// <summary>訪視表條文資料</summary>
    public class VisitingFormDto
    {
        /// <summary>訪視表 ID</summary>
        public int Id { get; set; }
        /// <summary>訪視表編號</summary>
        public string Code { get; set; } = null!;
        /// <summary>訪視表類別</summary>
        public string Class { get; set; } = null!;
        /// <summary>訪視表內容</summary>
        public string Description { get; set; } = null!;
        /// <summary>建立時間</summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>更新時間</summary>
        public DateTime UpdatedAt { get; set; }
        /// <summary>刪除時間（軟刪除）</summary>
        public DateTime? DeletedAt { get; set; }
    }

    /// <summary>新增/修改訪視表條文</summary>
    public class SaveVisitingFormDto
    {
        /// <summary>訪視表編號</summary>
        public string Code { get; set; } = null!;
        /// <summary>訪視表類別</summary>
        public string Class { get; set; } = null!;
        /// <summary>訪視表內容</summary>
        public string Description { get; set; } = null!;
    }
}
