﻿namespace Streetcode.BLL.DTO.Users
{
    public class LoginResultDTO
    {
        public UserDTO User { get; set; } = new();
        public string Token { get; set; } = string.Empty;
        public DateTime ExpireAt { get; set; }
    }
}
