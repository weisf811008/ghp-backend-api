using System.ComponentModel.DataAnnotations;

namespace GhpAPI.DTOs
{
    /// <summary>登入資料</summary>
    public class LoginDto
    {
        /// <summary>帳號</summary>
        [StringLength(100, MinimumLength = 2, ErrorMessage = "長度需在 2-100 字元之間")]
        [Required(ErrorMessage = "帳號為必填")]
        public string Username { get; set; } = null!;
        /// <summary>密碼</summary>
        [StringLength(100, MinimumLength = 6, ErrorMessage = "密碼長度需在 6-100 字元之間")]
        [Required(ErrorMessage = "密碼為必填")]
        public string Password { get; set; } = null!;
    }
}
