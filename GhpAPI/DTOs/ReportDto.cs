namespace GhpAPI.DTOs
{
    /// <summary>異常紀錄列</summary>
    public class AbnormalRowDto
    {
        /// <summary>日期</summary>
        public string Date { get; set; } = null!;
        /// <summary>細項編號</summary>
        public string No { get; set; } = null!;
        /// <summary>大項名稱</summary>
        public string Category { get; set; } = null!;
        /// <summary>細項名稱</summary>
        public string Item { get; set; } = null!;
        /// <summary>狀態（pass/fail/others）</summary>
        public string Status { get; set; } = null!;
        /// <summary>備註</summary>
        public string? Remarks { get; set; }
    }

    /// <summary>每日報表</summary>
    public class DailyReportDto
    {
        /// <summary>每個細項的每日狀態（日期為動態欄位）</summary>
        public List<Dictionary<string, object?>> Rows { get; set; } = new();
        /// <summary>異常紀錄列表</summary>
        public List<AbnormalRowDto> AbnormalRows { get; set; } = new();
    }

    /// <summary>GHP/訪視表報表細項</summary>
    public class FormReportItemDto
    {
        /// <summary>日期</summary>
        public string Date { get; set; } = null!;
        /// <summary>巡檢 ID</summary>
        public int InspectionId { get; set; }
        /// <summary>表單 ID</summary>
        public int FormId { get; set; }
        /// <summary>細項編號</summary>
        public string ItemNo { get; set; } = null!;
        /// <summary>備註</summary>
        public string? Remarks { get; set; }
    }

    /// <summary>GHP/訪視表報表列</summary>
    public class FormReportRowDto
    {
        /// <summary>條文/訪視表編號</summary>
        public string Code { get; set; } = null!;
        /// <summary>類別</summary>
        public string Class { get; set; } = null!;
        /// <summary>內容</summary>
        public string Description { get; set; } = null!;
        /// <summary>合格紀錄</summary>
        public List<FormReportItemDto> Pass { get; set; } = new();
        /// <summary>不合格紀錄</summary>
        public List<FormReportItemDto> Fail { get; set; } = new();
        /// <summary>其他紀錄</summary>
        public List<FormReportItemDto> Others { get; set; } = new();
    }

    /// <summary>數值報表列</summary>
    public class CheckValueReportRowDto
    {
        /// <summary>日期</summary>
        public DateTime Date { get; set; }
    }

    /// <summary>數值報表（溫度/濕度/餐具）</summary>
    public class CheckValueReportDto
    {
        /// <summary>每日數值資料（欄位名稱為動態）</summary>
        public List<Dictionary<string, object?>> Rows { get; set; } = new();
        /// <summary>異常紀錄列表</summary>
        public List<AbnormalRowDto> AbnormalRows { get; set; } = new();
    }
}
