using CourierMax.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourierMax.Data.Configurations
{
    public class CiudadConfiguration : IEntityTypeConfiguration<Ciudad>
    {
        public void Configure(EntityTypeBuilder<Ciudad> builder)
        {
            builder.ToTable("Ciudades", "gral");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.NombreCiudad).ColumnaTexto(150).IsRequired();
            builder.Property(e => e.Codigo).ColumnaTexto(50).IsRequired();

            builder.ConfigurarAuditoria();
        }
    }
}
