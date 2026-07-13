namespace GhpAPI.DTOs
{
    /// <summary>學校簡要資料</summary>
    public class SchoolDto
    {
        /// <summary>學校代碼</summary>
        public string Code { get; set; } = null!;
        /// <summary>學校名稱</summary>
        public string Name { get; set; } = null!;
    }

    /// <summary>使用者角色</summary>
    public class UserRoleDto
    {
        /// <summary>角色名稱</summary>
        public string Role { get; set; } = null!;
        /// <summary>是否為保留角色</summary>
        public bool Reserved { get; set; }
    }

    /// <summary>使用者資料</summary>
    public class UserDto
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
    }

    /// <summary>新增使用者</summary>
    public class CreateUserDto
    {
        /// <summary>帳號</summary>
        public string Username { get; set; } = null!;
        /// <summary>密碼</summary>
        public string Password { get; set; } = null!;
        /// <summary>電子郵件</summary>
        public string? Email { get; set; }
        /// <summary>姓名</summary>
        public string? Name { get; set; }
        /// <summary>電話</summary>
        public string? Phone { get; set; }
        /// <summary>角色列表</summary>
        public List<string> Roles { get; set; } = new List<string>();
    }

    /// <summary>修改使用者</summary>
    public class UpdateUserDto
    {
        /// <summary>電子郵件</summary>
        public string? Email { get; set; }
        /// <summary>姓名</summary>
        public string? Name { get; set; }
        /// <summary>電話</summary>
        public string? Phone { get; set; }
        /// <summary>學校 ID</summary>
        public int SchoolId { get; set; }
        /// <summary>角色列表</summary>
        public List<string> Roles { get; set; } = new List<string>();
    }
}
