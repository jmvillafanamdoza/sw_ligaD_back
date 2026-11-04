-- ============================================================================
-- BASE DE DATOS: BD_LIGAD (LigaD)
-- Script 01 · Creación de la base
-- Crea la base BD_LIGAD si no existe. Idempotente: si ya existe, no hace nada.
-- Intercalación en español, sin distinguir mayúsculas (CI) y con tildes (AS).
-- ============================================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF DB_ID(N'BD_LIGAD') IS NULL
BEGIN
    CREATE DATABASE BD_LIGAD COLLATE Modern_Spanish_CI_AS;
    PRINT 'Base BD_LIGAD creada.';
END
ELSE
    PRINT 'La base BD_LIGAD ya existe; no se modifica.';
GO
