using System.ComponentModel.DataAnnotations;

namespace GhpAPI.DTOs
{
    /// <summary>登入資料</summary>
    public class LoginDto
    {
        /// <summary>帳號</summary>
        [Required(ErrorMessage = "帳號為必填")]
        public string Username { get; set; } = null!;
        /// <summary>密碼</summary>
        [Required(ErrorMessage = "密碼為必填")]
        public string Password { get; set; } = null!;
    }
}
