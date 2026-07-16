using System.ComponentModel.DataAnnotations;

namespace GhpAPI.DTOs
{
    /// <summary>細項資料</summary>
    public class ItemDto
    {
        /// <summary>細項 ID</summary>
        public int Id { get; set; }
        /// <summary>細項編號</summary>
        public string No { get; set; } = null!;
        /// <summary>細項名稱</summary>
        public string Item { get; set; } = null!;
        /// <summary>檢查週期（半年(開學前)/日/週）</summary>
        public string? Period { get; set; }
        /// <summary>檢查區域（人員衛生/作業區/烹調區/配膳區/庫房）</summary>
        public string? Area { get; set; }
        /// <summary>是否需要填寫數值</summary>
        public bool NeedCheckValue { get; set; } = false;
        /// <summary>是否需要每日檢查</summary>
        public bool NeedDaily { get; set; } = false;
        /// <summary>大項 ID</summary>
        public int CategoryId { get; set; }
        /// <summary>關聯的 GHP 條文編號列表</summary>
        public List<string> Regulations { get; set; } = new List<string>();
        /// <summary>關聯的訪視表編號列表</summary>
        public List<string> VisitingForms { get; set; } = new List<string>();
        /// <summary>建立時間</summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>更新時間</summary>
        public DateTime UpdatedAt { get; set; }
        /// <summary>刪除時間（軟刪除）</summary>
        public DateTime? DeletedAt { get; set; }
    }

    /// <summary>新增/修改細項</summary>
    public class SaveItemDto
    {
        /// <summary>細項編號</summary>
        [Required(ErrorMessage = "細項編號為必填")]
        [MaxLength(10, ErrorMessage = "不可超過 10 字元")]
        public string No { get; set; } = null!;
        /// <summary>細項名稱</summary>
        [Required(ErrorMessage = "細項名稱為必填")]
        public string Item { get; set; } = null!;
        /// <summary>檢查週期</summary>
        public string? Period { get; set; }
        /// <summary>檢查區域</summary>
        public string? Area { get; set; }
        /// <summary>是否需要填寫數值</summary>
        public bool NeedCheckValue { get; set; } = false;
        /// <summary>是否需要每日檢查</summary>
        public bool NeedDaily { get; set; } = false;
        /// <summary>大項 ID</summary>
        [Required(ErrorMessage = "大項為必填")]
        public int CategoryId { get; set; }
        /// <summary>關聯的 GHP 條文編號列表</summary>
        public List<string> Regulations { get; set; } = new List<string>();
        /// <summary>關聯的訪視表編號列表</summary>
        public List<string> VisitingForms { get; set; } = new List<string>();
    }
}
