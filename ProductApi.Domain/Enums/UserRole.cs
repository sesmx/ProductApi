using System.ComponentModel;

namespace ProductApi.Domain.Enums;

public enum UserRole
{
	[Description("Product Administrator")]
	Admin = 1,

	[Description("Product User")]
	ProductUser = 2,
}
