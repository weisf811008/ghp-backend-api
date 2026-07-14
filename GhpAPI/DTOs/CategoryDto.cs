using System.ComponentModel.DataAnnotations;

namespace GhpAPI.DTOs
{
    /// <summary>大項資料</summary>
    public class CategoryDTo
    {
        /// <summary>大項 ID</summary>
        public int Id { get; set; }
        /// <summary>大項名稱</summary>
        public string Category { get; set; } = null!;
        /// <summary>建立時間</summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>更新時間</summary>
        public DateTime UpdatedAt { get; set; }
        /// <summary>刪除時間（軟刪除）</summary>
        public DateTime? DeletedAt { get; set; }
    }

    /// <summary>新增/修改大項</summary>
    public class SaveCategoryDto
    {
        /// <summary>大項名稱</summary>
        [Required(ErrorMessage = "大項名稱為必填")]
        public string Category { get; set; } = null!;
    }
}
