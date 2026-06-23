//-----------------------------------------------------------------------
// <copyright file="CourierMaxContext.cs" company="None">
//     All rights reserved.
// </copyright>
// <author>aandion</author>
// <date>21/06/2026 17:36:23</date>
// <summary>Código fuente clase CourierMaxContext.</summary>
//-----------------------------------------------------------------------
using CourierMax.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CourierMax.Data
{
    /// <summary>
    /// Contexto de EF Core para CourierMax.
    /// </summary>
    public class CourierMaxContext : DbContext
    {
        public CourierMaxContext(DbContextOptions<CourierMaxContext> options)
            : base(options)
        {
        }

        public DbSet<Estado> Estados => Set<Estado>();
        public DbSet<Ciudad> Ciudades => Set<Ciudad>();
        public DbSet<TipoDetalle> TipoDetalles => Set<TipoDetalle>();
        public DbSet<Cliente> Clientes => Set<Cliente>();
        public DbSet<Envio> Envios => Set<Envio>();
        public DbSet<Vehiculo> Vehiculos => Set<Vehiculo>();
        public DbSet<Conductor> Conductores => Set<Conductor>();
        public DbSet<HistorialEstado> HistorialEstados => Set<HistorialEstado>();
        public DbSet<DistanciaTarifa> DistanciasTarifas => Set<DistanciaTarifa>();
        public DbSet<RegistroError> RegistroErrores => Set<RegistroError>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CourierMaxContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
