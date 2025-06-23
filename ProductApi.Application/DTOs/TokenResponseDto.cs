namespace ProductApi.Application.DTOs;

public record TokenResponseDto(string AccessToken, DateTime ExpiresAtUtc);
