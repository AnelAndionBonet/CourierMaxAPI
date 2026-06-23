namespace CourierMax.UseCases;

public record TarifaInput(
    string ServicioNomenclatura,
    decimal PesoKg,
    decimal TarifaDistancia,
    string TipoPaqueteNomenclatura);

public static class TariffCalculator
{
    private static decimal Base(string servicio) => servicio switch
    {
        "STD" => 8000m,
        "EXP" => 15000m,
        "MSD" => 25000m,
        _ => throw new ArgumentException($"Servicio inválido: {servicio}"),
    };

    private static decimal Recargo(string tipoPaquete) => tipoPaquete switch
    {
        "FRA" => 0.30m,
        "PER" => 0.25m,
        "DOC" or "PAQ" => 0m,
        _ => throw new ArgumentException($"Tipo de paquete inválido: {tipoPaquete}"),
    };

    public static decimal Calcular(TarifaInput input)
    {
        var baseServicio = Base(input.ServicioNomenclatura);
        var pesoExtra = Math.Max(0, input.PesoKg - 2m) * 1500m;
        var subtotal = baseServicio + pesoExtra + input.TarifaDistancia;
        var total = subtotal * (1 + Recargo(input.TipoPaqueteNomenclatura));
        return Math.Round(total, 0, MidpointRounding.AwayFromZero);
    }
}
