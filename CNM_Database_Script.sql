-- CREATE DATABASE
CREATE DATABASE CNMDB;


-- ROLES
USE [CNMDB]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Roles](
	[Id] [nvarchar](128) NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_dbo.Roles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


-- CATEGORIAS
USE [CNMDB]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Categorias](
	[CategoriaId] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [nvarchar](max) NULL,
	[RolId] [nvarchar](128) NULL,
 CONSTRAINT [PK_dbo.Categorias] PRIMARY KEY CLUSTERED 
(
	[CategoriaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[Categorias]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Categorias_dbo.Roles_RolId] FOREIGN KEY([RolId])
REFERENCES [dbo].[Roles] ([Id])
GO

ALTER TABLE [dbo].[Categorias] CHECK CONSTRAINT [FK_dbo.Categorias_dbo.Roles_RolId]
GO


-- ESTADOS
USE [CNMDB]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Estados](
	[EstadoId] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.Estados] PRIMARY KEY CLUSTERED 
(
	[EstadoId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO


-- UNIDADES TECNICAS
USE [CNMDB]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[UnidadesTecnicas](
	[UnidadTecnicaId] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.UnidadesTecnicas] PRIMARY KEY CLUSTERED 
(
	[UnidadTecnicaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO


-- EMPLEADOS
USE [CNMDB]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Empleados](
	[Cedula] [nvarchar](128) NOT NULL,
	[Nombre] [nvarchar](max) NOT NULL,
	[PrimerApellido] [nvarchar](max) NOT NULL,
	[SegundoApellido] [nvarchar](max) NULL,
	[FechaIngreso] [datetime] NOT NULL,
	[FechaCreacion] [datetime] NOT NULL,
	[FotoRuta] [nvarchar](max) NULL,
	[CategoriaId] [int] NOT NULL,
	[UnidadTecnicaId] [int] NOT NULL,
	[EstaActivo] [bit] NOT NULL,
	[EsContrasenaTemporal] [bit] NOT NULL,
	[JefeId] [nvarchar](128) NULL,
	[Correo] [nvarchar](256) NOT NULL,
	[ContrasenaHash] [nvarchar](max) NULL,
	[SelloSeguridad] [nvarchar](max) NULL,
	[Telefono] [nvarchar](max) NULL,
	[NombreUsuario] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_dbo.Empleados] PRIMARY KEY CLUSTERED 
(
	[Cedula] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[Empleados]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Empleados_dbo.Categorias_CategoriaId] FOREIGN KEY([CategoriaId])
REFERENCES [dbo].[Categorias] ([CategoriaId])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Empleados] CHECK CONSTRAINT [FK_dbo.Empleados_dbo.Categorias_CategoriaId]
GO

ALTER TABLE [dbo].[Empleados]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Empleados_dbo.Empleados_JefeId] FOREIGN KEY([JefeId])
REFERENCES [dbo].[Empleados] ([Cedula])
GO

ALTER TABLE [dbo].[Empleados] CHECK CONSTRAINT [FK_dbo.Empleados_dbo.Empleados_JefeId]
GO

ALTER TABLE [dbo].[Empleados]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Empleados_dbo.UnidadesTecnicas_UnidadTecnicaId] FOREIGN KEY([UnidadTecnicaId])
REFERENCES [dbo].[UnidadesTecnicas] ([UnidadTecnicaId])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Empleados] CHECK CONSTRAINT [FK_dbo.Empleados_dbo.UnidadesTecnicas_UnidadTecnicaId]
GO

-- USUARIOROLES
USE [CNMDB]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[UsuarioRoles](
	[UserId] [nvarchar](128) NOT NULL,
	[RoleId] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_dbo.UsuarioRoles] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[UsuarioRoles]  WITH CHECK ADD  CONSTRAINT [FK_dbo.UsuarioRoles_dbo.Empleados_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Empleados] ([Cedula])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[UsuarioRoles] CHECK CONSTRAINT [FK_dbo.UsuarioRoles_dbo.Empleados_UserId]
GO

ALTER TABLE [dbo].[UsuarioRoles]  WITH CHECK ADD  CONSTRAINT [FK_dbo.UsuarioRoles_dbo.Roles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Roles] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[UsuarioRoles] CHECK CONSTRAINT [FK_dbo.UsuarioRoles_dbo.Roles_RoleId]
GO

-- SOLICITUDESVACACIONES
USE [CNMDB]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SolicitudesVacaciones](
	[SolicitudVacacionesId] [int] IDENTITY(1,1) NOT NULL,
	[Cedula] [nvarchar](128) NOT NULL,
	[CantidadDiasSolicitados] [int] NOT NULL,
	[Comentario] [nvarchar](500) NULL,
	[EstadoId] [int] NOT NULL,
	[AprobadorId] [nvarchar](max) NULL,
	[FechaSolicitud] [datetime] NOT NULL,
	[UltimaActualizacion] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.SolicitudesVacaciones] PRIMARY KEY CLUSTERED 
(
	[SolicitudVacacionesId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[SolicitudesVacaciones]  WITH CHECK ADD  CONSTRAINT [FK_dbo.SolicitudesVacaciones_dbo.Empleados_Cedula] FOREIGN KEY([Cedula])
REFERENCES [dbo].[Empleados] ([Cedula])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[SolicitudesVacaciones] CHECK CONSTRAINT [FK_dbo.SolicitudesVacaciones_dbo.Empleados_Cedula]
GO

ALTER TABLE [dbo].[SolicitudesVacaciones]  WITH CHECK ADD  CONSTRAINT [FK_dbo.SolicitudesVacaciones_dbo.Estados_EstadoId] FOREIGN KEY([EstadoId])
REFERENCES [dbo].[Estados] ([EstadoId])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[SolicitudesVacaciones] CHECK CONSTRAINT [FK_dbo.SolicitudesVacaciones_dbo.Estados_EstadoId]
GO

-- SALDODIASPOREMPLEADO
USE [CNMDB]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SaldoDiasPorEmpleado](
	[Cedula] [nvarchar](128) NOT NULL,
	[SaldoDiasDisponibles] [decimal](18, 2) NOT NULL,
	[UltimaActualizacion] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.SaldoDiasPorEmpleado] PRIMARY KEY CLUSTERED 
(
	[Cedula] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[SaldoDiasPorEmpleado]  WITH CHECK ADD  CONSTRAINT [FK_dbo.SaldoDiasPorEmpleado_dbo.Empleados_Cedula] FOREIGN KEY([Cedula])
REFERENCES [dbo].[Empleados] ([Cedula])
GO

ALTER TABLE [dbo].[SaldoDiasPorEmpleado] CHECK CONSTRAINT [FK_dbo.SaldoDiasPorEmpleado_dbo.Empleados_Cedula]
GO


-- DIASPORSOLICITUD
USE [CNMDB]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DiasPorSolicitud](
	[SolicitudVacacionesId] [int] NOT NULL,
	[UsuarioId] [nvarchar](128) NOT NULL,
	[Fecha] [datetime] NOT NULL,
	[Periodo] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_dbo.DiasPorSolicitud] PRIMARY KEY CLUSTERED 
(
	[SolicitudVacacionesId] ASC,
	[UsuarioId] ASC,
	[Fecha] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[DiasPorSolicitud]  WITH CHECK ADD  CONSTRAINT [FK_dbo.DiasPorSolicitud_dbo.SolicitudesVacaciones_SolicitudVacacionesId] FOREIGN KEY([SolicitudVacacionesId])
REFERENCES [dbo].[SolicitudesVacaciones] ([SolicitudVacacionesId])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[DiasPorSolicitud] CHECK CONSTRAINT [FK_dbo.DiasPorSolicitud_dbo.SolicitudesVacaciones_SolicitudVacacionesId]
GO


-- USUARIOCLAIMS
USE [CNMDB]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[UsuarioClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](128) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.UsuarioClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[UsuarioClaims]  WITH CHECK ADD  CONSTRAINT [FK_dbo.UsuarioClaims_dbo.Empleados_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Empleados] ([Cedula])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[UsuarioClaims] CHECK CONSTRAINT [FK_dbo.UsuarioClaims_dbo.Empleados_UserId]
GO


-- USUARIOLOGINS
USE [CNMDB]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[UsuarioLogins](
	[LoginProvider] [nvarchar](128) NOT NULL,
	[ProviderKey] [nvarchar](128) NOT NULL,
	[UserId] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_dbo.UsuarioLogins] PRIMARY KEY CLUSTERED 
(
	[LoginProvider] ASC,
	[ProviderKey] ASC,
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[UsuarioLogins]  WITH CHECK ADD  CONSTRAINT [FK_dbo.UsuarioLogins_dbo.Empleados_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Empleados] ([Cedula])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[UsuarioLogins] CHECK CONSTRAINT [FK_dbo.UsuarioLogins_dbo.Empleados_UserId]
GO


-- INSERT REQUIRED DATA
-- ROLES 
  INSERT INTO CNMDB.dbo.Roles
  VALUES('1', 'Funcionario')
  
  INSERT INTO CNMDB.dbo.Roles
  VALUES('2', 'Jefatura')
  
  INSERT INTO CNMDB.dbo.Roles
  VALUES('3', 'Manager')
  
  INSERT INTO CNMDB.dbo.Roles
  VALUES('4', 'Director')
  
  INSERT INTO CNMDB.dbo.Roles
  VALUES('5', 'Recursos Humanos')

	-- ESTADOS
  INSERT INTO CNMDB.dbo.Estados
  VALUES('Por Revisar')

  INSERT INTO CNMDB.dbo.Estados
  VALUES('Aprobado')

  INSERT INTO CNMDB.dbo.Estados
  VALUES('Rechazado')

	-- CATEGORIAS
  INSERT INTO CNMDB.dbo.Categorias
  VALUES('Administrativos', 1)

  INSERT INTO CNMDB.dbo.Categorias
  VALUES('Técnicos', 1)

  INSERT INTO CNMDB.dbo.Categorias
  VALUES('Profesional', 1)

  INSERT INTO CNMDB.dbo.Categorias
  VALUES('Jefatura', 2)

  INSERT INTO CNMDB.dbo.Categorias
  VALUES('Director Administrativo', 4)

  INSERT INTO CNMDB.dbo.Categorias
  VALUES('Director General', 4)

  INSERT INTO CNMDB.dbo.Categorias
  VALUES('Misceláneos', 1)

  INSERT INTO CNMDB.dbo.Categorias
  VALUES('Seguridad', 1)

  INSERT INTO CNMDB.dbo.Categorias
  VALUES('Manager', 3)

  INSERT INTO CNMDB.dbo.Categorias
  VALUES('RH', 5)

  -- UNIDADES TECNICAS
  INSERT INTO CNMDB.dbo.UnidadesTecnicas
  VALUES('Centro Nacional de la Música (CNM)')

  INSERT INTO CNMDB.dbo.UnidadesTecnicas
  VALUES('Instituto Nacional de la Música (INM)')

  INSERT INTO CNMDB.dbo.UnidadesTecnicas
  VALUES('Coro Sinfónico Nacional')

  INSERT INTO CNMDB.dbo.UnidadesTecnicas
  VALUES('Compañía de Lírica Nacional')

  INSERT INTO CNMDB.dbo.UnidadesTecnicas
  VALUES('Profesores INM')

  INSERT INTO CNMDB.dbo.UnidadesTecnicas
  VALUES('Todas las unidades técnicas')

  -- CREAR SUPER USUARIO POR DEFECTO
  INSERT INTO CNMDB.dbo.Empleados(Cedula, Nombre, PrimerApellido, SegundoApellido, FechaIngreso, FechaCreacion, FotoRuta, CategoriaId, UnidadTecnicaId, EstaActivo, EsContrasenaTemporal, JefeId, Correo, ContrasenaHash, SelloSeguridad, Telefono, NombreUsuario) 
  VALUES ('00000000', 'Super', 'User', NULL, GETDATE(), GETDATE(), NULL, 9, 6, 1, 0, NULL, 'manager@manager.com', 'AJoUptE4Guzz1Qz7DbAwkW6LuzLGzNj7YYxPqZqzhZbsRC483VJ+3sET9H/WGYNn0A==', '39347cd1-64f0-48c5-b18f-dd936da871d8', NULL, 'manager@manager.com')


  -- ASIGNO ROL DE MANAGER AL SUPER USUARIO POR DEFECTO
  INSERT INTO CNMDB.dbo.UsuarioRoles
  VALUES('00000000', (SELECT Id FROM CNMDB.dbo.Roles WHERE Name = 'Manager'))