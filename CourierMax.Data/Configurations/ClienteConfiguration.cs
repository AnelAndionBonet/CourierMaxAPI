using CourierMax.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourierMax.Data.Configurations
{
    public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
    {
        public void Configure(EntityTypeBuilder<Cliente> builder)
        {
            builder.ToTable("Clientes", "core");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Nombre).ColumnaTexto(150).IsRequired();
            builder.Property(e => e.Telefono).ColumnaTexto(30).IsRequired();
            builder.Property(e => e.Direccion).ColumnaTexto(250).IsRequired();
            builder.Property(e => e.Identificacion).ColumnaTexto(30).IsRequired();

            builder.HasOne(e => e.TipoIdentificacion)
                .WithMany()
                .HasForeignKey(e => e.IdTipoIdentificacion)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Ciudad)
                .WithMany(c => c.Clientes)
                .HasForeignKey(e => e.IdCiudad)
                .OnDelete(DeleteBehavior.Restrict);

            builder.ConfigurarAuditoria();
        }
    }
}
