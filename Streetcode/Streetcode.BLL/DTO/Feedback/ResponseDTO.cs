namespace Streetcode.BLL.DTO.Feedback;

public class ResponseDTO
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Description { get; set; }
}