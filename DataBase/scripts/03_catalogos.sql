-- ============================================================================
-- BASE DE DATOS: BD_LIGAD (LigaD)
-- Script 03 · Catálogos (TLGD_TIP_VALOR)
-- Carga los códigos de tipos, roles, medallas, métodos y estados con su nombre.
-- Idempotente (MERGE): inserta los que faltan y actualiza nombre, descripción y orden.
-- ============================================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
USE BD_LIGAD;
GO

MERGE dbo.TLGD_TIP_VALOR AS destino
USING (VALUES
    -- Tipos de torneo
    ('TIPO_TORNEO', 'SAT', 'Satélite Cup', '4 equipos · S/ 15 · el campeón gana cupo al International', 1),
    ('TIPO_TORNEO', 'BAT', 'Battle Cup', '8 equipos · S/ 25 · campeón S/ 100 + cupo, subcampeón cupo', 2),
    ('TIPO_TORNEO', 'INT', 'International Cup', 'Hasta 32 equipos · gratis con cupo o S/ 50 de entrada directa', 3),
    -- Roles
    ('ROL_PLATAFORMA', 'JUGADOR', 'Jugador', 'Usuario de la plataforma', 1),
    ('ROL_PLATAFORMA', 'ADMIN', 'Administrador', 'Staff de Liga D', 2),
    ('ROL_EQUIPO', 'CAPITAN', 'Capitán', 'Edita logo y banner, invita, expulsa e inscribe al equipo', 1),
    ('ROL_EQUIPO', 'TITULAR', 'Titular', 'Uno de los 5 que compiten', 2),
    ('ROL_EQUIPO', 'SUPLENTE', 'Suplente', 'Hasta 2 por equipo', 3),
    -- Medallas de Dota 2
    ('MEDALLA_DOTA', 'HERALD', 'Herald', NULL, 1),
    ('MEDALLA_DOTA', 'GUARDIAN', 'Guardian', NULL, 2),
    ('MEDALLA_DOTA', 'CRUSADER', 'Crusader', NULL, 3),
    ('MEDALLA_DOTA', 'ARCHON', 'Archon', NULL, 4),
    ('MEDALLA_DOTA', 'LEGEND', 'Legend', NULL, 5),
    ('MEDALLA_DOTA', 'ANCIENT', 'Ancient', NULL, 6),
    ('MEDALLA_DOTA', 'DIVINE', 'Divine', NULL, 7),
    ('MEDALLA_DOTA', 'IMMORTAL', 'Immortal', NULL, 8),
    -- Métodos
    ('METODO_COBRO', 'YAPE', 'Yape', NULL, 1),
    ('METODO_COBRO', 'PLIN', 'Plin', NULL, 2),
    ('METODO_COBRO', 'BCP', 'Transferencia BCP', NULL, 3),
    ('METODO_INSCRIPCION', 'PAGO', 'Pago', 'Inscripción pagada con Culqi', 1),
    ('METODO_INSCRIPCION', 'CUPO', 'Cupo', 'Clasificado desde un Satélite o una Battle Cup', 2),
    -- Tiers de logros
    ('TIER_LOGRO', 'TANGO', 'Tango', NULL, 1),
    ('TIER_LOGRO', 'MEKANS', 'Mekans', NULL, 2),
    ('TIER_LOGRO', 'DAGON', 'Dagon', NULL, 3),
    ('TIER_LOGRO', 'BATTLE', 'Battle', NULL, 4),
    ('TIER_LOGRO', 'DIVINE', 'Divine', NULL, 5),
    ('TIER_LOGRO', 'INMORTAL', 'Inmortal', NULL, 6),
    -- Estados comunes a todas las tablas
    ('ESTADO_GENERAL', 'V', 'Vigente', NULL, 1),
    ('ESTADO_GENERAL', 'I', 'Inactivo', NULL, 2),
    ('ESTADO_GENERAL', 'A', 'Anulado', NULL, 3),
    -- Estados propios
    ('ESTADO_JUGADOR', 'S', 'Suspendido', 'Sanción por conducta', 1),
    ('ESTADO_INVITACION', 'P', 'Pendiente', NULL, 1),
    ('ESTADO_INVITACION', 'C', 'Aceptada', NULL, 2),
    ('ESTADO_INVITACION', 'R', 'Rechazada', NULL, 3),
    ('ESTADO_TORNEO', 'P', 'Próximo', NULL, 1),
    ('ESTADO_TORNEO', 'B', 'Inscripción abierta', NULL, 2),
    ('ESTADO_TORNEO', 'C', 'Inscripción cerrada', NULL, 3),
    ('ESTADO_TORNEO', 'J', 'En juego', NULL, 4),
    ('ESTADO_TORNEO', 'T', 'Terminado', NULL, 5),
    ('ESTADO_INSCRIPCION', 'P', 'Pendiente de pago', NULL, 1),
    ('ESTADO_INSCRIPCION', 'C', 'Confirmada', NULL, 2),
    ('ESTADO_INSCRIPCION', 'D', 'Descalificada', NULL, 3),
    ('ESTADO_PARTIDA', 'P', 'Por jugar', NULL, 1),
    ('ESTADO_PARTIDA', 'J', 'En juego', NULL, 2),
    ('ESTADO_PARTIDA', 'T', 'Terminada', NULL, 3),
    ('ESTADO_PARTIDA', 'W', 'Walkover', 'El rival no se presentó', 4),
    ('ESTADO_PAGO', 'P', 'Pendiente', NULL, 1),
    ('ESTADO_PAGO', 'E', 'Exitoso', NULL, 2),
    ('ESTADO_PAGO', 'F', 'Fallido', NULL, 3),
    ('ESTADO_PAGO', 'R', 'Reembolsado', NULL, 4),
    ('ESTADO_COBRO', 'P', 'Pendiente', NULL, 1),
    ('ESTADO_COBRO', 'B', 'Aprobado', NULL, 2),
    ('ESTADO_COBRO', 'G', 'Pagado', 'Pagos los lunes y viernes', 3),
    ('ESTADO_COBRO', 'R', 'Rechazado', NULL, 4)
) AS origen (CTPV_GRUPO, CTPV_COD_VALOR, STPV_NOMBRE, STPV_DESCRIPCION, NTPV_ORDEN)
ON destino.CTPV_GRUPO = origen.CTPV_GRUPO AND destino.CTPV_COD_VALOR = origen.CTPV_COD_VALOR
WHEN MATCHED AND (destino.STPV_NOMBRE <> origen.STPV_NOMBRE
               OR ISNULL(destino.STPV_DESCRIPCION, '') <> ISNULL(origen.STPV_DESCRIPCION, '')
               OR destino.NTPV_ORDEN <> origen.NTPV_ORDEN) THEN
    UPDATE SET STPV_NOMBRE = origen.STPV_NOMBRE,
               STPV_DESCRIPCION = origen.STPV_DESCRIPCION,
               NTPV_ORDEN = origen.NTPV_ORDEN,
               AUD_UPD_FEC = CONVERT(DATETIME2(0), SYSDATETIMEOFFSET() AT TIME ZONE 'SA Pacific Standard Time'),
               AUD_UPD_USER = 'SISTEMA'
WHEN NOT MATCHED BY TARGET THEN
    INSERT (CTPV_GRUPO, CTPV_COD_VALOR, STPV_NOMBRE, STPV_DESCRIPCION, NTPV_ORDEN, AUD_INS_USER)
    VALUES (origen.CTPV_GRUPO, origen.CTPV_COD_VALOR, origen.STPV_NOMBRE, origen.STPV_DESCRIPCION, origen.NTPV_ORDEN, 'SISTEMA');
GO

PRINT 'Script 03 (catálogos) terminado.';
GO
