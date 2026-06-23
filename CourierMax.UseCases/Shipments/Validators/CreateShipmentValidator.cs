using CourierMax.Dtos.Shipments;
using FluentValidation;

namespace CourierMax.UseCases;

/// <summary>
/// Validaciones de datos (RN-04) aplicables a la creación del envío.
/// El teléfono del remitente/destinatario se valida al crear el Cliente
/// (modelo vía Clientes-FK), no en esta petición.
/// </summary>
public class CreateShipmentValidator : AbstractValidator<CreateShipmentRequest>
{
	public CreateShipmentValidator()
	{
		RuleFor(x => x.IdRemitente)
			.GreaterThan(0);

		RuleFor(x => x.IdDestinatario)
			.GreaterThan(0);

		RuleFor(x => x.Peso)
			.InclusiveBetween(0.1m, 100m)
			.WithMessage("El peso debe estar entre 0.1 y 100 kg.");

		RuleFor(x => x.Largo).InclusiveBetween(1m, 200m)
			.WithMessage("El largo debe estar entre 1 y 200 cm.");

		RuleFor(x => x.Ancho).InclusiveBetween(1m, 200m)
			.WithMessage("El ancho debe estar entre 1 y 200 cm.");

		RuleFor(x => x.Alto).InclusiveBetween(1m, 200m)
			.WithMessage("El alto debe estar entre 1 y 200 cm.");

		RuleFor(x => x.IdTipoPaquete)
			.GreaterThan(0);

		RuleFor(x => x.IdTipoServicio)
			.GreaterThan(0);

		RuleFor(x => x.IdUnidadPeso)
			.GreaterThan(0);

		RuleFor(x => x.IdUnidadVolumen)
			.GreaterThan(0);


		RuleFor(x => x.Usuario)
			.NotEmpty();
	}
}
