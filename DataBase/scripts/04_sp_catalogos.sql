-- ============================================================================
-- BASE DE DATOS: BD_LIGAD (LigaD)
-- Script 04 · SPs de catálogos
-- Tablas: TLGD_TIP_VALOR
-- SPs:    PRLGD_CON_TIP_VALOR (consulta de códigos por grupo)
-- Estados (FTPV_ESTADO): V vigente · I inactivo · A anulado
-- Idempotente (CREATE OR ALTER).
-- ============================================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
USE BD_LIGAD;
GO

-- ----------------------------------------------------------------------------
-- PRLGD_CON_TIP_VALOR
-- Devuelve los códigos de un grupo (o de todos si @i_CTPV_GRUPO es NULL),
-- filtrados por estado (NULL = todos), ordenados por grupo y orden.
-- Uso en la API: GET /api/catalogos?grupo=TIPO_TORNEO
-- ----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.PRLGD_CON_TIP_VALOR
    @i_CTPV_GRUPO  VARCHAR(30) = NULL,
    @i_FTPV_ESTADO CHAR(1)     = 'V'
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        TPV.CTPV_GRUPO,
        TPV.CTPV_COD_VALOR,
        TPV.STPV_NOMBRE,
        TPV.STPV_DESCRIPCION,
        TPV.NTPV_ORDEN,
        TPV.FTPV_ESTADO
    FROM dbo.TLGD_TIP_VALOR AS TPV
    WHERE (@i_CTPV_GRUPO IS NULL OR TPV.CTPV_GRUPO = @i_CTPV_GRUPO)
      AND (@i_FTPV_ESTADO IS NULL OR TPV.FTPV_ESTADO = @i_FTPV_ESTADO)
    ORDER BY TPV.CTPV_GRUPO, TPV.NTPV_ORDEN, TPV.CTPV_COD_VALOR;
END
GO
