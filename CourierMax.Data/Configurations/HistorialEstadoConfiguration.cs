using CourierMax.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourierMax.Data.Configurations
{
    public class HistorialEstadoConfiguration : IEntityTypeConfiguration<HistorialEstado>
    {
        public void Configure(EntityTypeBuilder<HistorialEstado> builder)
        {
            builder.ToTable("HistorialEstados", "core");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.FechaCambio).HasColumnType("datetime2").IsRequired();
            builder.Property(e => e.Motivo).ColumnaTexto(250);
            builder.Property(e => e.UsuarioCambio).ColumnaTexto(150).IsRequired();

            builder.HasOne(e => e.Envio)
                .WithMany(en => en.HistorialEstados)
                .HasForeignKey(e => e.IdEnvio)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.EstadoAnterior)
                .WithMany()
                .HasForeignKey(e => e.IdEstadoAnterior)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            builder.HasOne(e => e.EstadoNuevo)
                .WithMany()
                .HasForeignKey(e => e.IdEstadoNuevo)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(e => e.IdEnvio);

            builder.ConfigurarAuditoria();
        }
    }
}
