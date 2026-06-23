IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260622002939_InicialModeloDatos'
)
BEGIN
    IF SCHEMA_ID(N'gral') IS NULL EXEC(N'CREATE SCHEMA [gral];');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260622002939_InicialModeloDatos'
)
BEGIN
    IF SCHEMA_ID(N'core') IS NULL EXEC(N'CREATE SCHEMA [core];');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260622002939_InicialModeloDatos'
)
BEGIN
    CREATE TABLE [gral].[Ciudades] (
        [Id] int NOT NULL IDENTITY,
        [NombreCiudad] varchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
        [Codigo] varchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
        [FechaCreacion] datetime2 NOT NULL,
        [CreadoPor] varchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
        [FechaModificacion] datetime2 NULL,
        [ModificadoPor] varchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
        [FechaAnulacion] datetime2 NULL,
        [AnuladoPor] varchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
        [ObservacionEstado] varchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
        CONSTRAINT [PK_Ciudades] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260622002939_InicialModeloDatos'
)
BEGIN
    CREATE TABLE [gral].[Estados] (
        [Id] int NOT NULL IDENTITY,
        [Nombre] varchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
        [Nomenclatura] varchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
        [FechaCreacion] datetime2 NOT NULL,
        [CreadoPor] varchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
        [FechaModificacion] datetime2 NULL,
        [ModificadoPor] varchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
        [FechaAnulacion] datetime2 NULL,
        [AnuladoPor] varchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
        [ObservacionEstado] varchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
        CONSTRAINT [PK_Estados] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260622002939_InicialModeloDatos'
)
BEGIN
    CREATE TABLE [gral].[TipoDetalles] (
        [Id] int NOT NULL IDENTITY,
        [Nombre] varchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
        [Nomenclatura] varchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
        [FechaCreacion] datetime2 NOT NULL,
        [CreadoPor] varchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
        [FechaModificacion] datetime2 NULL,
        [ModificadoPor] varchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
        [FechaAnulacion] datetime2 NULL,
        [AnuladoPor] varchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
        [ObservacionEstado] varchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
        CONSTRAINT [PK_TipoDetalles] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260622002939_InicialModeloDatos'
)
BEGIN
    CREATE TABLE [core].[Clientes] (
        [Id] int NOT NULL IDENTITY,
        [Nombre] varchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
        [Telefono] varchar(30) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
        [Direccion] varchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
        [IdTipoIdentificacion] int NOT NULL,
        [Identificacion] varchar(30) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
        [IdCiudad] int NOT NULL,
        [FechaCreacion] datetime2 NOT NULL,
        [CreadoPor] varchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
        [FechaModificacion] datetime2 NULL,
        [ModificadoPor] varchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
        [FechaAnulacion] datetime2 NULL,
        [AnuladoPor] varchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
        [ObservacionEstado] varchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
        CONSTRAINT [PK_Clientes] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Clientes_Ciudades_IdCiudad] FOREIGN KEY ([IdCiudad]) REFERENCES [gral].[Ciudades] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Clientes_TipoDetalles_IdTipoIdentificacion] FOREIGN KEY ([IdTipoIdentificacion]) REFERENCES [gral].[TipoDetalles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260622002939_InicialModeloDatos'
)
BEGIN
    CREATE TABLE [core].[Envios] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
        [IdRemitente] int NOT NULL,
        [IdDestinatario] int NOT NULL,
        [Peso] decimal(18,3) NOT NULL,
        [IdUnidadPeso] int NOT NULL,
        [Dimension] decimal(18,3) NOT NULL,
        [IdUnidadVolumen] int NOT NULL,
        [IdTipoServicio] int NOT NULL,
        [IdEstado] int NOT NULL,
        [FechaCreacion] datetime2 NOT NULL,
        [CreadoPor] varchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
        [FechaModificacion] datetime2 NULL,
        [ModificadoPor] varchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
        [FechaAnulacion] datetime2 NULL,
        [AnuladoPor] varchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
        [ObservacionEstado] varchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
        CONSTRAINT [PK_Envios] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Envios_Clientes_IdDestinatario] FOREIGN KEY ([IdDestinatario]) REFERENCES [core].[Clientes] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Envios_Clientes_IdRemitente] FOREIGN KEY ([IdRemitente]) REFERENCES [core].[Clientes] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Envios_Estados_IdEstado] FOREIGN KEY ([IdEstado]) REFERENCES [gral].[Estados] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Envios_TipoDetalles_IdTipoServicio] FOREIGN KEY ([IdTipoServicio]) REFERENCES [gral].[TipoDetalles] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Envios_TipoDetalles_IdUnidadPeso] FOREIGN KEY ([IdUnidadPeso]) REFERENCES [gral].[TipoDetalles] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Envios_TipoDetalles_IdUnidadVolumen] FOREIGN KEY ([IdUnidadVolumen]) REFERENCES [gral].[TipoDetalles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260622002939_InicialModeloDatos'
)
BEGIN
    CREATE INDEX [IX_Clientes_IdCiudad] ON [core].[Clientes] ([IdCiudad]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260622002939_InicialModeloDatos'
)
BEGIN
    CREATE INDEX [IX_Clientes_IdTipoIdentificacion] ON [core].[Clientes] ([IdTipoIdentificacion]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260622002939_InicialModeloDatos'
)
BEGIN
    CREATE INDEX [IX_Envios_IdDestinatario] ON [core].[Envios] ([IdDestinatario]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260622002939_InicialModeloDatos'
)
BEGIN
    CREATE INDEX [IX_Envios_IdEstado] ON [core].[Envios] ([IdEstado]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260622002939_InicialModeloDatos'
)
BEGIN
    CREATE INDEX [IX_Envios_IdRemitente] ON [core].[Envios] ([IdRemitente]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260622002939_InicialModeloDatos'
)
BEGIN
    CREATE INDEX [IX_Envios_IdTipoServicio] ON [core].[Envios] ([IdTipoServicio]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260622002939_InicialModeloDatos'
)
BEGIN
    CREATE INDEX [IX_Envios_IdUnidadPeso] ON [core].[Envios] ([IdUnidadPeso]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260622002939_InicialModeloDatos'
)
BEGIN
    CREATE INDEX [IX_Envios_IdUnidadVolumen] ON [core].[Envios] ([IdUnidadVolumen]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260622002939_InicialModeloDatos'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260622002939_InicialModeloDatos', N'9.0.17');
END;

COMMIT;
GO

