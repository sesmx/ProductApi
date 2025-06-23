namespace ProductApi.Infrastructure.Data.Configurations;

public record TokenResponseDto(string AccessToken, DateTime ExpiresAtUtc);
