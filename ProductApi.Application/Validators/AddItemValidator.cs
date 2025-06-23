using FluentValidation;
using ProductApi.Application.DTOs;

namespace ProductApi.Application.Validators;

public class AddItemValidator : AbstractValidator<AddItemDto>
{
	public AddItemValidator()
	{
		RuleFor(x => x.Quantity).GreaterThan(0);
	}
}
