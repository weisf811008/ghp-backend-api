namespace GhpAPI.DTOs
{
    /// <summary>登入資料</summary>
    public class LoginDto
    {
        /// <summary>帳號</summary>
        public string Username { get; set; } = null!;
        /// <summary>密碼</summary>
        public string Password { get; set; } = null!;
    }
}
