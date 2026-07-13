namespace GhpAPI.DTOs
{
    /// <summary>角色資料</summary>
    public class RoleDto
    {
        /// <summary>角色 ID</summary>
        public int Id { get; set; }
        /// <summary>角色名稱</summary>
        public string Title { get; set; } = null!;
        /// <summary>是否為系統保留角色</summary>
        public bool Reserved { get; set; }
        /// <summary>建立時間</summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>更新時間</summary>
        public DateTime UpdatedAt { get; set; }
    }
}
