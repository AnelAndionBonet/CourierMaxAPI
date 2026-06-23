using CourierMax.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourierMax.Data.Configurations
{
    public class TipoDetalleConfiguration : IEntityTypeConfiguration<TipoDetalle>
    {
        public void Configure(EntityTypeBuilder<TipoDetalle> builder)
        {
            builder.ToTable("TipoDetalles", "gral");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Nombre).ColumnaTexto(150).IsRequired();
            builder.Property(e => e.Nomenclatura).ColumnaTexto(50).IsRequired();

            builder.ConfigurarAuditoria();
        }
    }
}
