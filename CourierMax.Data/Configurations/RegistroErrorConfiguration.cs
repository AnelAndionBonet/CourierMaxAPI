using CourierMax.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourierMax.Data.Configurations
{
    public class RegistroErrorConfiguration : IEntityTypeConfiguration<RegistroError>
    {
        public void Configure(EntityTypeBuilder<RegistroError> builder)
        {
            builder.ToTable("RegistroErrores", "gral");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.FechaUtc).HasColumnType("datetime2").IsRequired();
            builder.Property(e => e.Ruta).ColumnaTexto(500).IsRequired();
            builder.Property(e => e.Metodo).ColumnaTexto(10).IsRequired();
            builder.Property(e => e.Mensaje).ColumnaTexto(2000).IsRequired();
            builder.Property(e => e.Detalle).HasColumnType("varchar(max)").UseCollation(AuditableConfiguration.Colacion);
            builder.Property(e => e.TraceId).ColumnaTexto(100);

            builder.HasIndex(e => e.FechaUtc);
        }
    }
}
