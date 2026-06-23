using CourierMax.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourierMax.Data.Configurations
{
    public class VehiculoConfiguration : IEntityTypeConfiguration<Vehiculo>
    {
        public void Configure(EntityTypeBuilder<Vehiculo> builder)
        {
            builder.ToTable("Vehiculos", "core");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Placa).ColumnaTexto(10).IsRequired();
            builder.Property(e => e.CapacidadPesoKg).HasColumnType("decimal(9,2)").IsRequired();
            builder.Property(e => e.CapacidadVolumenM3).HasColumnType("decimal(9,2)").IsRequired();

            builder.HasIndex(e => e.Placa).IsUnique();

            builder.ConfigurarAuditoria();
        }
    }
}
