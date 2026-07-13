namespace GhpAPI.DTOs
{
    /// <summary>GHP 條文資料</summary>
    public class RegulationDto
    {
        /// <summary>條文 ID</summary>
        public int Id { get; set; }
        /// <summary>條文編號</summary>
        public string Code { get; set; } = null!;
        /// <summary>條文類別</summary>
        public string Class { get; set; } = null!;
        /// <summary>條文內容</summary>
        public string Description { get; set; } = null!;
        /// <summary>學校 ID</summary>
        public int SchoolId { get; set; }
        /// <summary>建立時間</summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>更新時間</summary>
        public DateTime UpdatedAt { get; set; }
        /// <summary>刪除時間（軟刪除）</summary>
        public DateTime? DeletedAt { get; set; }
    }

    /// <summary>新增/修改 GHP 條文</summary>
    public class SaveRegulationDto
    {
        /// <summary>條文編號</summary>
        public string Code { get; set; } = null!;
        /// <summary>條文類別</summary>
        public string Class { get; set; } = null!;
        /// <summary>條文內容</summary>
        public string Description { get; set; } = null!;
    }
}
