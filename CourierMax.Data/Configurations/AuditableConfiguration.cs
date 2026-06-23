using CourierMax.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourierMax.Data.Configurations
{
    /// <summary>
    /// Configuración Fluent reutilizable para los campos de auditoría y la
    /// colación estándar de las columnas de texto.
    /// </summary>
    internal static class AuditableConfiguration
    {
        public const string Colacion = "SQL_Latin1_General_CP1_CI_AS";

        /// <summary>
        /// Configura una columna de texto como varchar(n) con la colación estándar.
        /// </summary>
        public static PropertyBuilder ColumnaTexto(this PropertyBuilder property, int longitud)
            => property.HasColumnType($"varchar({longitud})").UseCollation(Colacion);

        /// <summary>
        /// Aplica la configuración de los 7 campos de auditoría comunes.
        /// </summary>
        public static void ConfigurarAuditoria<T>(this EntityTypeBuilder<T> builder)
            where T : class, IAuditable
        {
            builder.Property(e => e.FechaCreacion).HasColumnType("datetime2").IsRequired();
            builder.Property(e => e.CreadoPor).ColumnaTexto(150).IsRequired();
            builder.Property(e => e.FechaModificacion).HasColumnType("datetime2");
            builder.Property(e => e.ModificadoPor).ColumnaTexto(150);
            builder.Property(e => e.FechaAnulacion).HasColumnType("datetime2");
            builder.Property(e => e.AnuladoPor).ColumnaTexto(150);
            builder.Property(e => e.ObservacionEstado).ColumnaTexto(250);
        }
    }
}
