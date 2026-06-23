namespace CourierMax.UseCases;

/// <summary>
/// Política de SLA por tipo de servicio (RN-02).
/// Devuelve el número de días hábiles máximo para la entrega.
/// </summary>
public static class SlaPolicy
{
    public static int DiasHabiles(string servicioNomenclatura) => servicioNomenclatura switch
    {
        "STD" => 5,
        "EXP" => 2,
        "MSD" => 0,
        _ => int.MaxValue,
    };
}
