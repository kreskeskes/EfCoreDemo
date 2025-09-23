namespace Shared.DTOs
{
    public record UserDto(string Name, string Email, List<int> BlogIds, List<int> PostIds);

}
