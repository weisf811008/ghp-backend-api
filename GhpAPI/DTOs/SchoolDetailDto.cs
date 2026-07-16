using System.ComponentModel.DataAnnotations;

namespace GhpAPI.DTOs
{
    /// <summary>學校詳細資料</summary>
    public class SchoolDetailDto
    {
        /// <summary>學校 ID</summary>
        public int Id { get; set; }
        /// <summary>學校代碼</summary>
        public string Code { get; set; } = null!;
        /// <summary>學校名稱</summary>
        public string Name { get; set; } = null!;
        /// <summary>所在縣市</summary>
        public string? City { get; set; }
        /// <summary>地址</summary>
        public string? Address { get; set; }
        /// <summary>電話</summary>
        public string? Phone { get; set; }
        /// <summary>網址</summary>
        public string? Url { get; set; }
        /// <summary>建立時間</summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>更新時間</summary>
        public DateTime UpdatedAt { get; set; }
        /// <summary>刪除時間（軟刪除）</summary>
        public DateTime? DeletedAt { get; set; }
    }

    /// <summary>新增學校</summary>
    public class CreateSchoolDto
    {
        /// <summary>學校代碼</summary>
        [Required(ErrorMessage = "學校代碼為必填")]
        public string Code { get; set; } = null!;
        /// <summary>學校名稱</summary>
        [Required(ErrorMessage = "學校名稱為必填")]
        public string Name { get; set; } = null!;
        /// <summary>所在縣市</summary>
        public string? City { get; set; }
        /// <summary>地址</summary>
        public string? Address { get; set; }
        /// <summary>電話</summary>
        [Phone(ErrorMessage = "電話格式不正確")]
        public string? Phone { get; set; }
        /// <summary>網址</summary>
        [Url(ErrorMessage = "網址格式不正確")]
        public string? Url { get; set; }
    }

    /// <summary>修改學校</summary>
    public class UpdateSchoolDto
    {
        /// <summary>學校名稱</summary>
        [Required(ErrorMessage = "學校名稱為必填")]
        public string? Name { get; set; }
        /// <summary>所在縣市</summary>
        public string? City { get; set; }
        /// <summary>地址</summary>
        public string? Address { get; set; }
        /// <summary>電話</summary>
        public string? Phone { get; set; }
        /// <summary>網址</summary>
        public string? Url { get; set; }
    }
}
