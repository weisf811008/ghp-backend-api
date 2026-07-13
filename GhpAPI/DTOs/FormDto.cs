namespace GhpAPI.DTOs
{
    /// <summary>表單資料</summary>
    public class FormDto
    {
        /// <summary>表單 ID</summary>
        public int Id { get; set; }
        /// <summary>表單標題</summary>
        public string Title { get; set; } = null!;
        /// <summary>備註</summary>
        public string? Remarks { get; set; }
        /// <summary>學校 ID</summary>
        public int SchoolId { get; set; }
        /// <summary>表單細項列表</summary>
        public List<FormDetailDto> Details { get; set; } = new List<FormDetailDto>();
        /// <summary>建立時間</summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>更新時間</summary>
        public DateTime UpdatedAt { get; set; }
        /// <summary>刪除時間（軟刪除）</summary>
        public DateTime? DeletedAt { get; set; }
    }

    /// <summary>新增/修改表單</summary>
    public class SaveFormDto
    {
        /// <summary>表單標題</summary>
        public string Title { get; set; } = null!;
        /// <summary>備註</summary>
        public string? Remarks { get; set; }
        /// <summary>細項 ID 列表</summary>
        public List<int> ItemIds { get; set; } = new List<int>();
    }

    /// <summary>表單細項</summary>
    public class FormDetailDto
    {
        /// <summary>細項 ID</summary>
        public int ItemId { get; set; }
        /// <summary>細項編號</summary>
        public string No { get; set; } = null!;
        /// <summary>細項名稱</summary>
        public string Item { get; set; } = null!;
        /// <summary>是否需要填寫數值</summary>
        public bool NeedCheckValue { get; set; }
        /// <summary>大項 ID</summary>
        public int CategoryId { get; set; }
        /// <summary>大項名稱</summary>
        public string Category { get; set; } = null!;
    }
}
