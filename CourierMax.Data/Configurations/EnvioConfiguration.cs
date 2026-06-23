using CourierMax.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourierMax.Data.Configurations
{
    public class EnvioConfiguration : IEntityTypeConfiguration<Envio>
    {
        public void Configure(EntityTypeBuilder<Envio> builder)
        {
            builder.ToTable("Envios", "core");
            builder.HasKey(e => e.Id);

            // Guid secuencial generado por la base de datos (clustered, sin fragmentación).
            builder.Property(e => e.Id)
                .HasDefaultValueSql("NEWSEQUENTIALID()")
                .ValueGeneratedOnAdd();

            builder.Property(e => e.CodigoRastreo).ColumnaTexto(11).IsRequired();
            builder.Property(e => e.Peso).HasColumnType("decimal(18,3)");
            builder.Property(e => e.Largo).HasColumnType("decimal(9,2)");
            builder.Property(e => e.Ancho).HasColumnType("decimal(9,2)");
            builder.Property(e => e.Alto).HasColumnType("decimal(9,2)");
            builder.Property(e => e.Costo).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(e => e.FechaAsignacion).HasColumnType("datetime2");
            builder.Property(e => e.FechaEntrega).HasColumnType("datetime2");

            builder.HasIndex(e => e.CodigoRastreo).IsUnique();

            builder.HasOne(e => e.Remitente)
                .WithMany()
                .HasForeignKey(e => e.IdRemitente)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Destinatario)
                .WithMany()
                .HasForeignKey(e => e.IdDestinatario)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.UnidadPeso)
                .WithMany()
                .HasForeignKey(e => e.IdUnidadPeso)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.UnidadVolumen)
                .WithMany()
                .HasForeignKey(e => e.IdUnidadVolumen)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.TipoServicio)
                .WithMany()
                .HasForeignKey(e => e.IdTipoServicio)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Estado)
                .WithMany(es => es.Envios)
                .HasForeignKey(e => e.IdEstado)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.TipoPaquete)
                .WithMany()
                .HasForeignKey(e => e.IdTipoPaquete)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Conductor)
                .WithMany(c => c.Envios)
                .HasForeignKey(e => e.IdConductor)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            builder.ConfigurarAuditoria();
        }
    }
}
