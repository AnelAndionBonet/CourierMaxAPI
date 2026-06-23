namespace CourierMax.Dtos.Driver
{
	/// <summary>Reporte de métricas de eficiencia por conductor (RF-06).</summary>
	public record DriverMetricsDto(
		int IdConductor,
		string Conductor,
		int TotalAsignados,
		int Entregados,
		int Cancelados,
		int EnTransito,
		double TiempoPromedioEntregaDias,
		double PorcentajeDentroSla,
		decimal PesoTotalKg);
}
