-- ============================================================================
-- BASE DE DATOS: BD_LIGAD (LigaD)
-- Script 00 · Ejecuta todos los scripts en orden (modo SQLCMD).
--
-- Desde la carpeta DataBase/scripts:
--   sqlcmd -S ".\SQLEXPRESS" -E -C -i 00_ejecutar_todo.sql
-- En SSMS: menú Consulta → Modo SQLCMD, abrir este archivo desde esta carpeta y ejecutar.
--
-- Todos los scripts son idempotentes: se puede ejecutar varias veces sin error ni duplicados.
-- ============================================================================
:on error exit

:r .\01_crear_base.sql
:r .\02_tablas.sql
:r .\03_catalogos.sql

PRINT 'BD_LIGAD lista: base, tablas y catálogos.';
GO
