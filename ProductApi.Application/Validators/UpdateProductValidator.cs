using FluentValidation;
using ProductApi.Application.DTOs;

namespace ProductApi.Application.Validators;

public class UpdateProductValidator : AbstractValidator<UpdateProductDto>
{
	public UpdateProductValidator()
	{
		RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
	}
}
