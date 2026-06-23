using CourierMax.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourierMax.Data.Configurations
{
    public class DistanciaTarifaConfiguration : IEntityTypeConfiguration<DistanciaTarifa>
    {
        public void Configure(EntityTypeBuilder<DistanciaTarifa> builder)
        {
            builder.ToTable("DistanciasTarifas", "gral");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.DistanciaKm).IsRequired();
            builder.Property(e => e.TarifaDistancia).HasColumnType("decimal(18,2)").IsRequired();

            builder.HasOne(e => e.CiudadOrigen)
                .WithMany()
                .HasForeignKey(e => e.IdCiudadOrigen)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.CiudadDestino)
                .WithMany()
                .HasForeignKey(e => e.IdCiudadDestino)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(e => new { e.IdCiudadOrigen, e.IdCiudadDestino }).IsUnique();

            builder.ConfigurarAuditoria();
        }
    }
}
