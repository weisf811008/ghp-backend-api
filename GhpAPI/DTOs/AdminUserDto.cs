using System.ComponentModel.DataAnnotations;

namespace GhpAPI.DTOs
{
    /// <summary>系統管理員查看的使用者資料</summary>
    public class AdminUserDto
    {
        /// <summary>使用者 ID</summary>
        public int Id { get; set; }
        /// <summary>帳號</summary>
        public string Username { get; set; } = null!;
        /// <summary>電子郵件</summary>
        public string? Email { get; set; }
        /// <summary>姓名</summary>
        public string? Name { get; set; }
        /// <summary>電話</summary>
        public string? Phone { get; set; }
        /// <summary>是否需要變更密碼</summary>
        public bool NeedToChangePass { get; set; }
        /// <summary>是否為保留帳號</summary>
        public bool Reserved { get; set; }
        /// <summary>學校 ID</summary>
        public int SchoolId { get; set; }
        /// <summary>所屬學校</summary>
        public SchoolDto? School { get; set; }
        /// <summary>角色列表</summary>
        public List<UserRoleDto> Roles { get; set; } = new List<UserRoleDto>();
        /// <summary>建立時間</summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>更新時間</summary>
        public DateTime UpdatedAt { get; set; }
        /// <summary>刪除時間（軟刪除）</summary>
        public DateTime? DeletedAt { get; set; }
    }

    /// <summary>系統管理員新增使用者</summary>
    public class CreateAdminUserDto
    {
        /// <summary>帳號</summary>
        [Required(ErrorMessage = "帳號為必填")]
        public string Username { get; set; } = null!;
        /// <summary>密碼</summary>
        [StringLength(100, MinimumLength = 6, ErrorMessage = "密碼長度需在 6-100 字元之間")]
        [Required(ErrorMessage = "密碼為必填")]
        public string Password { get; set; } = null!;
        /// <summary>電子郵件</summary>
        [EmailAddress(ErrorMessage = "Email 格式不正確")]
        public string? Email { get; set; }
        /// <summary>姓名</summary>
        public string? Name { get; set; }
        /// <summary>電話</summary>
        [Phone(ErrorMessage = "電話格式不正確")]
        public string? Phone { get; set; }
        /// <summary>學校 ID</summary>
        [Required(ErrorMessage = "學校為必填")]
        public int SchoolId { get; set; }
        /// <summary>角色列表（例如：學校管理員、巡檢人員）</summary>
        public List<string> Roles { get; set; } = new List<string>();
    }

    /// <summary>系統管理員修改使用者</summary>
    public class UpdateAdminUserDto
    {
        /// <summary>電子郵件</summary>
        public string? Email { get; set; }
        /// <summary>姓名</summary>
        public string? Name { get; set; }
        /// <summary>電話</summary>
        public string? Phone { get; set; }
        /// <summary>學校 ID</summary>
        [Required(ErrorMessage = "學校為必填")]
        public int SchoolId { get; set; }
        /// <summary>角色列表</summary>
        public List<string> Roles { get; set; } = new List<string>();
    }

    /// <summary>重設密碼</summary>
    public class ResetPasswordDto
    {
        /// <summary>新密碼</summary>
        [StringLength(100, MinimumLength = 6, ErrorMessage = "密碼長度需在 6-100 字元之間")]
        [Required(ErrorMessage = "新密碼為必填")]
        public string NewPassword { get; set; } = null!;
    }
}
