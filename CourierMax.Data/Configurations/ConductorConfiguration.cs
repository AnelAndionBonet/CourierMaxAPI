using CourierMax.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourierMax.Data.Configurations
{
    public class ConductorConfiguration : IEntityTypeConfiguration<Conductor>
    {
        public void Configure(EntityTypeBuilder<Conductor> builder)
        {
            builder.ToTable("Conductores", "core");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Nombre).ColumnaTexto(150).IsRequired();
            builder.Property(e => e.Activo).IsRequired();

            builder.HasOne(e => e.Vehiculo)
                .WithMany(v => v.Conductores)
                .HasForeignKey(e => e.IdVehiculo)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(e => e.IdVehiculo).IsUnique();

            builder.ConfigurarAuditoria();
        }
    }
}
