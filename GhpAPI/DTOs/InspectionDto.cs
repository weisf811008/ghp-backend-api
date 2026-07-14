using System.ComponentModel.DataAnnotations;

namespace GhpAPI.DTOs
{
    /// <summary>巡檢執行人</summary>
    public class InspectedByDto
    {
        /// <summary>帳號</summary>
        public string Username { get; set; } = null!;
        /// <summary>姓名</summary>
        public string? Name { get; set; }
    }

    /// <summary>巡檢附件</summary>
    public class InspectionFileDto
    {
        /// <summary>附件 ID</summary>
        public int Id { get; set; }
        /// <summary>細項 ID</summary>
        public int ItemId { get; set; }
        /// <summary>儲存的檔案名稱</summary>
        public string Filename { get; set; } = null!;
        /// <summary>原始檔案名稱</summary>
        public string Originalname { get; set; } = null!;
        /// <summary>編碼方式</summary>
        public string Encoding { get; set; } = null!;
        /// <summary>檔案類型</summary>
        public string Mimetype { get; set; } = null!;
    }

    /// <summary>巡檢細項結果</summary>
    public class InspectionDetailDto
    {
        /// <summary>細項 ID</summary>
        public int ItemId { get; set; }
        /// <summary>細項編號</summary>
        public string No { get; set; } = null!;
        /// <summary>大項名稱</summary>
        public string Category { get; set; } = null!;
        /// <summary>細項名稱</summary>
        public string Item { get; set; } = null!;
        /// <summary>是否需要填寫數值</summary>
        public bool NeedCheckValue { get; set; }
        /// <summary>檢查狀態（pass/fail/others）</summary>
        public string Status { get; set; } = null!;
        /// <summary>備註</summary>
        public string? Remarks { get; set; }
        /// <summary>檢查數值</summary>
        public string? CheckValue { get; set; }
        /// <summary>附件列表</summary>
        public List<InspectionFileDto> Files { get; set; } = new List<InspectionFileDto>();
    }

    /// <summary>巡檢紀錄</summary>
    public class InspectionDto
    {
        /// <summary>巡檢 ID</summary>
        public int Id { get; set; }
        /// <summary>巡檢日期</summary>
        public DateTime Date { get; set; }
        /// <summary>改善期限</summary>
        public DateTime? DueDate { get; set; }
        /// <summary>備註</summary>
        public string? Remarks { get; set; }
        /// <summary>版本號</summary>
        public int Version { get; set; }
        /// <summary>結案時間</summary>
        public DateTime? ClosedAt { get; set; }
        /// <summary>上一次巡檢 ID（複查用）</summary>
        public int? ParentId { get; set; }
        /// <summary>表單 ID</summary>
        public int FormId { get; set; }
        /// <summary>表單標題</summary>
        public string? Title { get; set; }
        /// <summary>建立時間</summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>巡檢執行人</summary>
        public InspectedByDto InspectedBy { get; set; } = null!;
    }

    /// <summary>巡檢紀錄（含細項）</summary>
    public class InspectionWithDetailsDto : InspectionDto
    {
        /// <summary>細項結果列表</summary>
        public List<InspectionDetailDto> Details { get; set; } = new List<InspectionDetailDto>();
    }

    /// <summary>上傳附件資訊</summary>
    public class SaveInspectionFileDto
    {
        /// <summary>儲存的檔案名稱</summary>
        [Required(ErrorMessage = "檔案名稱為必填")]
        public string Filename { get; set; } = null!;
        /// <summary>原始檔案名稱</summary>
        [Required(ErrorMessage = "原始檔案名稱為必填")]
        public string Originalname { get; set; } = null!;
        /// <summary>編碼方式</summary>
        [Required(ErrorMessage = "編碼方式為必填")]
        public string Encoding { get; set; } = null!;
        /// <summary>檔案類型</summary>
        [Required(ErrorMessage = "檔案類型為必填")]
        public string Mimetype { get; set; } = null!;
    }

    /// <summary>新增巡檢細項</summary>
    public class SaveInspectionDetailDto
    {
        /// <summary>細項 ID</summary>
        [Required(ErrorMessage = "細項為必填")]
        public int ItemId { get; set; }
        /// <summary>檢查狀態（pass/fail/others）</summary>
        [Required(ErrorMessage = "檢查狀態為必填")]
        public string Status { get; set; } = null!;
        /// <summary>備註</summary>
        public string? Remarks { get; set; }
        /// <summary>檢查數值</summary>
        public string? CheckValue { get; set; }
        /// <summary>附件列表</summary>
        public List<SaveInspectionFileDto> Files { get; set; } = new List<SaveInspectionFileDto>();
    }

    /// <summary>新增巡檢紀錄</summary>
    public class SaveInspectionDto
    {
        /// <summary>巡檢日期</summary>
        [Required(ErrorMessage = "巡檢日期為必填")]
        public DateTime Date { get; set; }
        /// <summary>改善期限</summary>
        public DateTime? DueDate { get; set; }
        /// <summary>備註</summary>
        public string? Remarks { get; set; }
        /// <summary>表單 ID</summary>
        [Required(ErrorMessage = "表單為必填")]
        public int FormId { get; set; }
        /// <summary>細項結果列表</summary>
        public List<SaveInspectionDetailDto> Details { get; set; } = new List<SaveInspectionDetailDto>();
    }
}
