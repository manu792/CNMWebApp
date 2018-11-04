USE [msdb]
GO

/****** Object:  Job [EscalarSolicitudVacaciones]    Script Date: 11/4/2018 11:22:24 AM ******/
BEGIN TRANSACTION
DECLARE @ReturnCode INT
SELECT @ReturnCode = 0
/****** Object:  JobCategory [[Uncategorized (Local)]]    Script Date: 11/4/2018 11:22:24 AM ******/
IF NOT EXISTS (SELECT name FROM msdb.dbo.syscategories WHERE name=N'[Uncategorized (Local)]' AND category_class=1)
BEGIN
EXEC @ReturnCode = msdb.dbo.sp_add_category @class=N'JOB', @type=N'LOCAL', @name=N'[Uncategorized (Local)]'
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback

END

DECLARE @jobId BINARY(16)
EXEC @ReturnCode =  msdb.dbo.sp_add_job @job_name=N'EscalarSolicitudVacaciones', 
		@enabled=1, 
		@notify_level_eventlog=0, 
		@notify_level_email=0, 
		@notify_level_netsend=0, 
		@notify_level_page=0, 
		@delete_level=0, 
		@description=N'Se encarga de escalar la solicitud de vacaciones, al director general, para aquellos empleados que hayan puesto una solicitud y hayan pasado 4 dias o mas, sin aprobacion o rechazo por parte del jefe directo del empleado', 
		@category_name=N'[Uncategorized (Local)]', 
		@owner_login_name=N'Manuel', @job_id = @jobId OUTPUT
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [EscalarSolicitudDirector]    Script Date: 11/4/2018 11:22:24 AM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'EscalarSolicitudDirector', 
		@step_id=1, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=3, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'UPDATE S
SET AprobadorId = (
					SELECT Cedula 
					FROM [CNMDB].[dbo].[Empleados]
					WHERE CategoriaId = (
										  SELECT CategoriaId 
										  FROM [CNMDB].[dbo].[Categorias] 
										  WHERE Nombre = ''Director General''
										)
				  )
FROM [CNMDB].[dbo].[SolicitudesVacaciones] S
INNER JOIN [CNMDB].[dbo].[Empleados] J
ON S.AprobadorId = J.Cedula
INNER JOIN [CNMDB].[dbo].[UsuarioRoles] R
ON J.Cedula = R.UserId
WHERE S.FechaSolicitud <= DATEADD(DAY, -4, GETDATE())
AND S.EstadoId = (SELECT EstadoId FROM [CNMDB].[dbo].[Estados] WHERE Nombre = ''Por revisar'')
AND R.RoleId = (SELECT Id FROM [CNMDB].[dbo].[Roles] WHERE NAME = ''Jefatura'')', 
		@database_name=N'CNMDB', 
		@flags=0
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [RechazarSolicitud]    Script Date: 11/4/2018 11:22:24 AM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'RechazarSolicitud', 
		@step_id=2, 
		@cmdexec_success_code=0, 
		@on_success_action=1, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'UPDATE S
SET EstadoId = (
					SELECT EstadoId 
					FROM [CNMDB].[dbo].[Estados]
					WHERE Nombre = ''Rechazado''
				  )
FROM [CNMDB].[dbo].[SolicitudesVacaciones] S
INNER JOIN [CNMDB].[dbo].[Empleados] J
ON S.AprobadorId = J.Cedula
INNER JOIN [CNMDB].[dbo].[UsuarioRoles] R
ON J.Cedula = R.UserId
WHERE S.FechaSolicitud <= DATEADD(DAY, -4, GETDATE())
AND S.EstadoId = (SELECT EstadoId FROM [CNMDB].[dbo].[Estados] WHERE Nombre = ''Por revisar'')
AND R.RoleId = (SELECT Id FROM [CNMDB].[dbo].[Roles] WHERE NAME = ''Director'')', 
		@database_name=N'CNMDB', 
		@flags=0
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
EXEC @ReturnCode = msdb.dbo.sp_update_job @job_id = @jobId, @start_step_id = 1
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
EXEC @ReturnCode = msdb.dbo.sp_add_jobschedule @job_id=@jobId, @name=N'EscalarSolicitudVacaciones', 
		@enabled=1, 
		@freq_type=4, 
		@freq_interval=1, 
		@freq_subday_type=1, 
		@freq_subday_interval=0, 
		@freq_relative_interval=0, 
		@freq_recurrence_factor=0, 
		@active_start_date=20181026, 
		@active_end_date=99991231, 
		@active_start_time=30000, 
		@active_end_time=235959, 
		@schedule_uid=N'b5073587-b883-4fe5-a278-46c232984b66'
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
EXEC @ReturnCode = msdb.dbo.sp_add_jobserver @job_id = @jobId, @server_name = N'(local)'
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
COMMIT TRANSACTION
GOTO EndSave
QuitWithRollback:
    IF (@@TRANCOUNT > 0) ROLLBACK TRANSACTION
EndSave:

GO


