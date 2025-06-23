using FluentValidation;
using ProductApi.Application.DTOs;

namespace ProductApi.Application.Validators;

public class CreateProductValidator : AbstractValidator<CreateProductDto>
{
	public CreateProductValidator()
	{
		RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
	}
}
