USE [master]
GO
/****** Object:  Database [CNMDB]    Script Date: 11/4/2018 11:19:46 AM ******/
CREATE DATABASE [CNMDB]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'CNMDB', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.MSSQLSERVER\MSSQL\DATA\CNMDB.mdf' , SIZE = 3264KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'CNMDB_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.MSSQLSERVER\MSSQL\DATA\CNMDB_log.ldf' , SIZE = 832KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [CNMDB] SET COMPATIBILITY_LEVEL = 120
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [CNMDB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [CNMDB] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [CNMDB] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [CNMDB] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [CNMDB] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [CNMDB] SET ARITHABORT OFF 
GO
ALTER DATABASE [CNMDB] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [CNMDB] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [CNMDB] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [CNMDB] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [CNMDB] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [CNMDB] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [CNMDB] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [CNMDB] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [CNMDB] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [CNMDB] SET  ENABLE_BROKER 
GO
ALTER DATABASE [CNMDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [CNMDB] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [CNMDB] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [CNMDB] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [CNMDB] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [CNMDB] SET READ_COMMITTED_SNAPSHOT ON 
GO
ALTER DATABASE [CNMDB] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [CNMDB] SET RECOVERY FULL 
GO
ALTER DATABASE [CNMDB] SET  MULTI_USER 
GO
ALTER DATABASE [CNMDB] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [CNMDB] SET DB_CHAINING OFF 
GO
ALTER DATABASE [CNMDB] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [CNMDB] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [CNMDB] SET DELAYED_DURABILITY = DISABLED 
GO
EXEC sys.sp_db_vardecimal_storage_format N'CNMDB', N'ON'
GO
USE [CNMDB]
GO
/****** Object:  Table [dbo].[__MigrationHistory]    Script Date: 11/4/2018 11:19:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[__MigrationHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ContextKey] [nvarchar](300) NOT NULL,
	[Model] [varbinary](max) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK_dbo.__MigrationHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC,
	[ContextKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Categorias]    Script Date: 11/4/2018 11:19:46 AM ******/
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
/****** Object:  Table [dbo].[DiasPorSolicitud]    Script Date: 11/4/2018 11:19:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DiasPorSolicitud](
	[SolicitudVacacionesId] [uniqueidentifier] NOT NULL,
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
/****** Object:  Table [dbo].[Empleados]    Script Date: 11/4/2018 11:19:46 AM ******/
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
	[FotoRuta] [nvarchar](max) NULL,
	[CategoriaId] [int] NOT NULL,
	[UnidadTecnicaId] [int] NOT NULL,
	[EstaActivo] [bit] NOT NULL,
	[EsContrasenaTemporal] [bit] NOT NULL,
	[Correo] [nvarchar](256) NOT NULL,
	[ContrasenaHash] [nvarchar](max) NULL,
	[SelloSeguridad] [nvarchar](max) NULL,
	[Telefono] [nvarchar](max) NULL,
	[NombreUsuario] [nvarchar](256) NOT NULL,
	[SaldoDiasEmpleado_SaldoDiasEmpleadoId] [nvarchar](128) NULL,
 CONSTRAINT [PK_dbo.Empleados] PRIMARY KEY CLUSTERED 
(
	[Cedula] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Estados]    Script Date: 11/4/2018 11:19:46 AM ******/
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
/****** Object:  Table [dbo].[Roles]    Script Date: 11/4/2018 11:19:46 AM ******/
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
/****** Object:  Table [dbo].[SaldoDiasPorEmpleado]    Script Date: 11/4/2018 11:19:46 AM ******/
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
/****** Object:  Table [dbo].[SolicitudesVacaciones]    Script Date: 11/4/2018 11:19:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SolicitudesVacaciones](
	[SolicitudVacacionesId] [uniqueidentifier] NOT NULL,
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
/****** Object:  Table [dbo].[UnidadesTecnicas]    Script Date: 11/4/2018 11:19:46 AM ******/
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
/****** Object:  Table [dbo].[UsuarioClaims]    Script Date: 11/4/2018 11:19:46 AM ******/
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
/****** Object:  Table [dbo].[UsuarioLogins]    Script Date: 11/4/2018 11:19:46 AM ******/
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
/****** Object:  Table [dbo].[UsuarioRoles]    Script Date: 11/4/2018 11:19:46 AM ******/
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
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_RolId]    Script Date: 11/4/2018 11:19:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_RolId] ON [dbo].[Categorias]
(
	[RolId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_SolicitudVacacionesId]    Script Date: 11/4/2018 11:19:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_SolicitudVacacionesId] ON [dbo].[DiasPorSolicitud]
(
	[SolicitudVacacionesId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_CategoriaId]    Script Date: 11/4/2018 11:19:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_CategoriaId] ON [dbo].[Empleados]
(
	[CategoriaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_SaldoDiasEmpleado_SaldoDiasEmpleadoId]    Script Date: 11/4/2018 11:19:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_SaldoDiasEmpleado_SaldoDiasEmpleadoId] ON [dbo].[Empleados]
(
	[SaldoDiasEmpleado_SaldoDiasEmpleadoId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_UnidadTecnicaId]    Script Date: 11/4/2018 11:19:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_UnidadTecnicaId] ON [dbo].[Empleados]
(
	[UnidadTecnicaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [UserNameIndex]    Script Date: 11/4/2018 11:19:46 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex] ON [dbo].[Empleados]
(
	[NombreUsuario] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [RoleNameIndex]    Script Date: 11/4/2018 11:19:46 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [RoleNameIndex] ON [dbo].[Roles]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Cedula]    Script Date: 11/4/2018 11:19:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_Cedula] ON [dbo].[SolicitudesVacaciones]
(
	[Cedula] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_EstadoId]    Script Date: 11/4/2018 11:19:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_EstadoId] ON [dbo].[SolicitudesVacaciones]
(
	[EstadoId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_UserId]    Script Date: 11/4/2018 11:19:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_UserId] ON [dbo].[UsuarioClaims]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_UserId]    Script Date: 11/4/2018 11:19:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_UserId] ON [dbo].[UsuarioLogins]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_RoleId]    Script Date: 11/4/2018 11:19:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_RoleId] ON [dbo].[UsuarioRoles]
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_UserId]    Script Date: 11/4/2018 11:19:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_UserId] ON [dbo].[UsuarioRoles]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Categorias]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Categorias_dbo.Roles_RolId] FOREIGN KEY([RolId])
REFERENCES [dbo].[Roles] ([Id])
GO
ALTER TABLE [dbo].[Categorias] CHECK CONSTRAINT [FK_dbo.Categorias_dbo.Roles_RolId]
GO
ALTER TABLE [dbo].[DiasPorSolicitud]  WITH CHECK ADD  CONSTRAINT [FK_dbo.DiasPorSolicitud_dbo.SolicitudesVacaciones_SolicitudVacacionesId] FOREIGN KEY([SolicitudVacacionesId])
REFERENCES [dbo].[SolicitudesVacaciones] ([SolicitudVacacionesId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[DiasPorSolicitud] CHECK CONSTRAINT [FK_dbo.DiasPorSolicitud_dbo.SolicitudesVacaciones_SolicitudVacacionesId]
GO
ALTER TABLE [dbo].[Empleados]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Empleados_dbo.Categorias_CategoriaId] FOREIGN KEY([CategoriaId])
REFERENCES [dbo].[Categorias] ([CategoriaId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Empleados] CHECK CONSTRAINT [FK_dbo.Empleados_dbo.Categorias_CategoriaId]
GO
ALTER TABLE [dbo].[Empleados]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Empleados_dbo.SaldoDiasPorEmpleado_SaldoDiasEmpleado_SaldoDiasEmpleadoId] FOREIGN KEY([SaldoDiasEmpleado_SaldoDiasEmpleadoId])
REFERENCES [dbo].[SaldoDiasPorEmpleado] ([Cedula])
GO
ALTER TABLE [dbo].[Empleados] CHECK CONSTRAINT [FK_dbo.Empleados_dbo.SaldoDiasPorEmpleado_SaldoDiasEmpleado_SaldoDiasEmpleadoId]
GO
ALTER TABLE [dbo].[Empleados]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Empleados_dbo.UnidadesTecnicas_UnidadTecnicaId] FOREIGN KEY([UnidadTecnicaId])
REFERENCES [dbo].[UnidadesTecnicas] ([UnidadTecnicaId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Empleados] CHECK CONSTRAINT [FK_dbo.Empleados_dbo.UnidadesTecnicas_UnidadTecnicaId]
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
ALTER TABLE [dbo].[UsuarioClaims]  WITH CHECK ADD  CONSTRAINT [FK_dbo.UsuarioClaims_dbo.Empleados_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Empleados] ([Cedula])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UsuarioClaims] CHECK CONSTRAINT [FK_dbo.UsuarioClaims_dbo.Empleados_UserId]
GO
ALTER TABLE [dbo].[UsuarioLogins]  WITH CHECK ADD  CONSTRAINT [FK_dbo.UsuarioLogins_dbo.Empleados_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Empleados] ([Cedula])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UsuarioLogins] CHECK CONSTRAINT [FK_dbo.UsuarioLogins_dbo.Empleados_UserId]
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
USE [master]
GO
ALTER DATABASE [CNMDB] SET  READ_WRITE 
GO
