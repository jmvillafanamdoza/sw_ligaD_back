# Base de datos LigaD — diseño (BORRADOR v2 para revisión)

- **Motor:** SQL Server 2022 · **Desarrollo:** `.\SQLEXPRESS`, base `BD_LIGAD` (autenticación de Windows).
- **Estilo:** el mismo del CRM MKK. Prefijo de tablas `TLGD_`, de SPs `PRLGD_`.
- **Fechas:** `DATETIME2(0)` en hora de Perú (America/Lima, UTC-5; Perú no usa horario de verano).
- **Dinero:** `DECIMAL(10,2)` en soles (PEN).
- **Ninguna acción sobre la base (crear, ejecutar scripts, cargar datos) se hace sin permiso del usuario.**

## Reglas de nombres

| Elemento | Regla | Ejemplo |
|---|---|---|
| Tabla | `TLGD_<NOMBRE>` + abreviatura de 3 letras | `TLGD_JUGADOR` (JUG) |
| Columna | `<PREFIJO><ABR>_<NOMBRE>` | `SJUG_NICKNAME` |
| Prefijo `C` | id o código (int o varchar) | `CJUG_ID_JUGADOR`, `CTOR_COD_TIPO` |
| Prefijo `S` | texto descriptivo | `SEQU_NOMBRE` |
| Prefijo `N` | número o monto | `NTOR_COSTO_INSCRIPCION` |
| Prefijo `F` | estado `CHAR(1)` | `FTOR_ESTADO` |
| Prefijo `B` | booleano `BIT` | `BJUG_OPENDOTA_VERIFICADO` |
| Prefijo `D` / `H` | fecha / hora | `DTOR_FEC_INICIO` |
| FK | mismo nombre que la PK de origen | `CJUG_ID_JUGADOR` |
| SP | `PRLGD_<OP>_<ENTIDAD>` · OP = `CON`, `INS`, `UPD`, `DEL`, `PRC`; cambio de estado `PRLGD_UPD_EST_<ENTIDAD>` | `PRLGD_INS_INSCRIPCION` |
| Parámetros | `@i_<COLUMNA>`; salida `@o_resultMessage VARCHAR(500)` = `'1|mensaje'` / `'0|error'` | |
| IDs | `INT IDENTITY(1,1)` | |

### Auditoría: obligatoria en TODAS las tablas (últimas 4 columnas)
| Columna | Tipo | Nota |
|---|---|---|
| AUD_INS_FEC | DATETIME2(0) NOT NULL DEFAULT (hora Perú) | fecha de creación |
| AUD_INS_USER | VARCHAR(50) NOT NULL | quién creó (nickname, `SISTEMA` para procesos) |
| AUD_UPD_FEC | DATETIME2(0) NULL | última modificación |
| AUD_UPD_USER | VARCHAR(50) NULL | quién modificó |

En las tablas de abajo se indica **+ AUD** para no repetirlas, pero cada script las crea explícitamente.

### Estados (`F*_ESTADO`)
Todas las tablas comparten **`V` vigente · `I` inactivo · `A` anulado**. Algunas agregan estados propios (se indican en cada tabla). Los nombres y descripciones de todos los estados viven en `TLGD_TIP_VALOR` (grupo `ESTADO_<TABLA>`).

### Códigos (`C*_COD_*`) → TLGD_TIP_VALOR
Igual que MKK: las columnas de tipo guardan **solo el código** y su nombre y descripción están en `TLGD_TIP_VALOR`.

| Grupo | Códigos |
|---|---|
| `TIPO_TORNEO` | `SAT` Satélite Cup · `BAT` Battle Cup · `INT` International Cup |
| `ROL_PLATAFORMA` | `JUGADOR` · `ADMIN` |
| `ROL_EQUIPO` | `CAPITAN` · `TITULAR` · `SUPLENTE` |
| `MEDALLA_DOTA` | `HERALD` · `GUARDIAN` · `CRUSADER` · `ARCHON` · `LEGEND` · `ANCIENT` · `DIVINE` · `IMMORTAL` |
| `METODO_COBRO` | `YAPE` · `PLIN` · `BCP` |
| `METODO_INSCRIPCION` | `PAGO` · `CUPO` |
| `TIER_LOGRO` | `TANGO` · `MEKANS` · `DAGON` · `BATTLE` · `DIVINE` · `INMORTAL` |
| `ESTADO_<TABLA>` | estados de cada tabla |

## Tablas

### TLGD_TIP_VALOR (TPV) — catálogos
| Columna | Tipo | Nota |
|---|---|---|
| CTPV_GRUPO | VARCHAR(30) | PK compuesta |
| CTPV_COD_VALOR | VARCHAR(10) | PK compuesta |
| STPV_NOMBRE | VARCHAR(60) | "Battle Cup" |
| STPV_DESCRIPCION | VARCHAR(250) NULL | |
| NTPV_ORDEN | INT | |
| FTPV_ESTADO | CHAR(1) DEFAULT 'V' | V · I · A |
| + AUD | | |

### TLGD_JUGADOR (JUG)
| Columna | Tipo | Nota |
|---|---|---|
| CJUG_ID_JUGADOR | INT IDENTITY PK | |
| CJUG_STEAM_ID | VARCHAR(32) UNIQUE | SteamID64 |
| SJUG_NICKNAME | VARCHAR(50) | |
| SJUG_EMAIL | VARCHAR(100) NULL | |
| SJUG_AVATAR_URL | VARCHAR(500) NULL | avatar de Steam |
| SJUG_BANNER_URL | VARCHAR(500) NULL | null = banner por defecto |
| SJUG_DISCORD | VARCHAR(50) NULL | |
| CJUG_COD_MEDALLA | VARCHAR(10) NULL | → `MEDALLA_DOTA`; **medalla actual** (ver "Medalla Dota") |
| NJUG_ESTRELLAS_MEDALLA | INT NULL | 1–5 (Immortal: 0) |
| NJUG_RANK_INMORTAL | INT NULL | puesto en la tabla Immortal, si aplica |
| DJUG_FEC_SYNC_MEDALLA | DATETIME2(0) NULL | última sincronización con OpenDota |
| BJUG_OPENDOTA_VERIFICADO | BIT DEFAULT 0 | perfil público en OpenDota |
| NJUG_SALDO_PREMIOS | DECIMAL(10,2) DEFAULT 0 | saldo pendiente de cobro |
| CJUG_COD_METODO_COBRO | VARCHAR(10) NULL | → `METODO_COBRO` |
| SJUG_COBRO_DESTINO | VARCHAR(30) NULL | teléfono o cuenta |
| SJUG_COBRO_TITULAR | VARCHAR(100) NULL | |
| CJUG_COD_ROL | VARCHAR(10) DEFAULT 'JUGADOR' | → `ROL_PLATAFORMA` (JUGADOR / ADMIN) |
| FJUG_ESTADO | CHAR(1) DEFAULT 'V' | V · I · A · **S suspendido** |
| + AUD | | |

### TLGD_JUGADOR_MEDALLA (JME) — historial de medallas
| Columna | Tipo | Nota |
|---|---|---|
| CJME_ID_JUGADOR_MEDALLA | INT IDENTITY PK | |
| CJUG_ID_JUGADOR | INT FK | |
| CJME_COD_MEDALLA | VARCHAR(10) | → `MEDALLA_DOTA` |
| NJME_ESTRELLAS | INT NULL | |
| NJME_RANK_INMORTAL | INT NULL | |
| DJME_FEC_REGISTRO | DATETIME2(0) | cuándo se detectó el cambio |
| FJME_ESTADO | CHAR(1) DEFAULT 'V' | V · I · A |
| + AUD | | |

### TLGD_EQUIPO (EQU)
| Columna | Tipo | Nota |
|---|---|---|
| CEQU_ID_EQUIPO | INT IDENTITY PK | |
| SEQU_NOMBRE | VARCHAR(60) UNIQUE | |
| SEQU_TAG | VARCHAR(10) | |
| SEQU_LOGO_URL | VARCHAR(500) NULL | **obligatorio para inscribirse** (lo valida el SP); mientras sea null el front muestra el escudo generado |
| SEQU_BANNER_URL | VARCHAR(500) NULL | opcional; null = banner por defecto |
| CEQU_COD_INVITACION | VARCHAR(12) UNIQUE | link de invitación |
| FEQU_ESTADO | CHAR(1) DEFAULT 'V' | V · I · A |
| + AUD | | |

> El capitán **no** se guarda aquí: sale de `TLGD_EQUIPO_MIEMBRO` (rol `CAPITAN`), para que haya una sola fuente.

### TLGD_EQUIPO_MIEMBRO (EQM) — roster (mín. 5, máx. 7)
| Columna | Tipo | Nota |
|---|---|---|
| CEQM_ID_MIEMBRO | INT IDENTITY PK | |
| CEQU_ID_EQUIPO | INT FK | |
| CJUG_ID_JUGADOR | INT FK | |
| CEQM_COD_ROL | VARCHAR(10) | → `ROL_EQUIPO` (CAPITAN / TITULAR / SUPLENTE) |
| DEQM_FEC_INGRESO | DATE | |
| FEQM_ESTADO | CHAR(1) DEFAULT 'V' | V · I (salió) · A |
| + AUD | | |
| *Reglas* | índices únicos filtrados (`WHERE FEQM_ESTADO = 'V'`) | un jugador activo en un solo equipo · un solo `CAPITAN` activo por equipo |

### TLGD_INVITACION (INV)
| Columna | Tipo | Nota |
|---|---|---|
| CINV_ID_INVITACION | INT IDENTITY PK | |
| CEQU_ID_EQUIPO | INT FK | |
| CJUG_ID_JUGADOR | INT FK NULL | invitado (si ya existe) |
| SINV_NICKNAME | VARCHAR(50) | |
| FINV_ESTADO | CHAR(1) DEFAULT 'P' | **P pendiente · C aceptada (confirmada) · R rechazada** · A anulada |
| + AUD | | |

### TLGD_TORNEO (TOR)
| Columna | Tipo | Nota |
|---|---|---|
| CTOR_ID_TORNEO | INT IDENTITY PK | |
| CTOR_CODIGO | VARCHAR(20) UNIQUE | ej. `SAT-2026-10-24` |
| STOR_NOMBRE | VARCHAR(100) | "Battle Cup #14" |
| CTOR_COD_TIPO | VARCHAR(3) | → `TIPO_TORNEO` (SAT / BAT / INT) |
| CTOR_ID_TORNEO_DESTINO | INT FK NULL (a sí misma) | International del mes al que clasifica (SAT/BAT) |
| NTOR_MAX_EQUIPOS | INT | 4 / 8 / 32 |
| NTOR_COSTO_INSCRIPCION | DECIMAL(10,2) | 15 / 25 / 50 |
| NTOR_PREMIO_EFECTIVO | DECIMAL(10,2) DEFAULT 0 | Battle Cup: 100 |
| NTOR_APORTE_POZO | DECIMAL(10,2) DEFAULT 0 | Battle Cup: 50 al International |
| NTOR_POZO_GARANTIZADO | DECIMAL(10,2) DEFAULT 0 | International: 1000 |
| NTOR_POZO_ACUMULADO | DECIMAL(10,2) DEFAULT 0 | lo actualiza el SP al confirmar pagos/aportes |
| DTOR_FEC_CIERRE_INSCRIPCION | DATETIME2(0) | countdown del sidebar |
| DTOR_FEC_INICIO | DATETIME2(0) | |
| FTOR_ESTADO | CHAR(1) DEFAULT 'P' | **P próximo · B inscripción abierta · C cerrada · J en juego · T terminado** · A anulado |
| + AUD | | |

### TLGD_INSCRIPCION (INS)
| Columna | Tipo | Nota |
|---|---|---|
| CINS_ID_INSCRIPCION | INT IDENTITY PK | |
| CTOR_ID_TORNEO | INT FK | |
| CEQU_ID_EQUIPO | INT FK | UNIQUE (torneo, equipo) |
| CINS_COD_METODO | VARCHAR(10) | → `METODO_INSCRIPCION` (PAGO / CUPO) |
| CTOR_ID_TORNEO_ORIGEN | INT FK NULL | si es CUPO: Satélite o Battle Cup donde clasificó |
| NINS_SEMILLA | INT NULL | posición en el sorteo |
| FINS_ESTADO | CHAR(1) DEFAULT 'P' | **P pendiente de pago · C confirmada · D descalificada** · A anulada |
| + AUD | | |

> **Ya no hay tabla de cupos.** Los equipos no gestionan cupos: al terminar un Satélite o una Battle Cup, el SP `PRLGD_PRC_CLASIFICAR_INTERNATIONAL` **inscribe automáticamente** al ganador (y al subcampeón de Battle Cup) en el International de ese mes, con método `CUPO` y estado `C`. Si el equipo ya estaba inscrito en ese International (por pago o por otro cupo), no se duplica: un equipo juega un solo International por mes.

> **Equipo con cupo = fuera del resto del mes.** Un equipo con inscripción `CUPO` confirmada en el International de un mes **no puede inscribirse en ningún otro torneo de ese mes** (Satélite ni Battle Cup). "Del mes" se define por el vínculo, no por el calendario: son los torneos cuyo `CTOR_ID_TORNEO_DESTINO` es ese International.
> - `PRLGD_INS_INSCRIPCION` lo bloquea con `'0|Tu equipo ya clasificó al International de este mes'`.
> - Al clasificar, `PRLGD_PRC_CLASIFICAR_INTERNATIONAL` **anula (`A`) las inscripciones que el equipo ya tenía** en otros Satélites o Battle Cups pendientes de ese mes, en la misma transacción. Si alguna estaba pagada, el pago queda marcado para **reembolso** (`FPAG_ESTADO = 'R'` tras devolverlo por Culqi).

### TLGD_PARTIDA (PAR) — fixture
| Columna | Tipo | Nota |
|---|---|---|
| CPAR_ID_PARTIDA | INT IDENTITY PK | |
| CTOR_ID_TORNEO | INT FK | |
| NPAR_RONDA | INT | 1 = primera ronda |
| NPAR_POSICION | INT | orden dentro de la ronda (1, 2, 3…) |
| CEQU_ID_EQUIPO_A / CEQU_ID_EQUIPO_B | INT FK NULL | null = por definir |
| NPAR_PUNTOS_A / NPAR_PUNTOS_B | INT NULL | |
| CEQU_ID_GANADOR | INT FK NULL | |
| DPAR_FEC_PROGRAMADA | DATETIME2(0) NULL | |
| CPAR_DOTA_MATCH_ID | VARCHAR(20) NULL | para validar con OpenDota |
| FPAR_ESTADO | CHAR(1) DEFAULT 'P' | **P por jugar · J en juego · T terminada · W walkover** · A anulada |
| + AUD | | UNIQUE (torneo, ronda, posición) |

> **Sin columna de "partida siguiente".** En eliminación directa el destino se calcula: el ganador de (ronda `r`, posición `p`) pasa a (ronda `r+1`, posición `CEILING(p/2)`), como equipo A si `p` es impar y como B si es par. El SP `PRLGD_PRC_REGISTRAR_RESULTADO` guarda el ganador y llena ese casillero en la misma transacción. La estructura de la llave se crea una sola vez al sortear (`PRLGD_PRC_GENERAR_FIXTURE`) y no se reescribe.

### TLGD_PAGO (PAG) — cobros de inscripción con Culqi
| Columna | Tipo | Nota |
|---|---|---|
| CPAG_ID_PAGO | INT IDENTITY PK | |
| CINS_ID_INSCRIPCION | INT FK | inscripción que se paga |
| CPAG_CULQI_CARGO_ID | VARCHAR(100) UNIQUE | evita doble cobro (webhook idempotente) |
| NPAG_MONTO | DECIMAL(10,2) | total del cargo |
| CPAG_COD_MONEDA | CHAR(3) DEFAULT 'PEN' | |
| SPAG_CONCEPTO | VARCHAR(150) | |
| SPAG_RECIBO_URL | VARCHAR(500) NULL | |
| FPAG_ESTADO | CHAR(1) DEFAULT 'P' | **P pendiente · E exitoso · F fallido · R reembolsado** · A anulado |
| + AUD | | |

### TLGD_JUGADOR_PAGO (JPA) — intermedia jugador ↔ pago
| Columna | Tipo | Nota |
|---|---|---|
| CJPA_ID_JUGADOR_PAGO | INT IDENTITY PK | |
| CJUG_ID_JUGADOR | INT FK | jugador que aportó |
| CPAG_ID_PAGO | INT FK | |
| NJPA_MONTO | DECIMAL(10,2) | lo que aportó ese jugador (permite que el equipo divida la inscripción) |
| FJPA_ESTADO | CHAR(1) DEFAULT 'V' | V · I · A |
| + AUD | | UNIQUE (jugador, pago) |

### TLGD_COBRO (COB) — retiro de premios
| Columna | Tipo | Nota |
|---|---|---|
| CCOB_ID_COBRO | INT IDENTITY PK | |
| CJUG_ID_JUGADOR | INT FK | quien cobra |
| NCOB_MONTO | DECIMAL(10,2) | |
| CCOB_COD_METODO | VARCHAR(10) | → `METODO_COBRO` |
| SCOB_DESTINO | VARCHAR(30) | copia del teléfono/cuenta al momento de pedir |
| SCOB_TITULAR | VARCHAR(100) | |
| SCOB_OPERACION | VARCHAR(50) NULL | n.º de operación Yape/Plin/BCP al pagar |
| DCOB_FEC_PAGO | DATETIME2(0) NULL | pagos lunes y viernes |
| FCOB_ESTADO | CHAR(1) DEFAULT 'P' | **P pendiente · B aprobado · G pagado · R rechazado** · A anulado |
| + AUD | | |

### TLGD_LOGRO (LOG) y TLGD_JUGADOR_LOGRO (JLO)
- `LOG`: `CLOG_ID_LOGRO`, `CLOG_CODIGO` UNIQUE, `SLOG_TITULO`, `SLOG_DESCRIPCION`, `CLOG_COD_TIER` (→ `TIER_LOGRO`), `SLOG_IMAGEN_URL`, `FLOG_ESTADO` (V · I · A) + AUD.
- `JLO`: `CJLO_ID_JUGADOR_LOGRO`, `CJUG_ID_JUGADOR`, `CLOG_ID_LOGRO`, `NJLO_CANTIDAD`, `DJLO_FEC_OBTENIDO`, `FJLO_ESTADO` (V · I · A) + AUD. UNIQUE (jugador, logro).

### TLGD_PREMIO_MES (PRM) — figura 3D del International (sidebar)
| Columna | Tipo | Nota |
|---|---|---|
| CPRM_ID_PREMIO | INT IDENTITY PK | |
| NPRM_ANIO / NPRM_MES | INT | UNIQUE (año, mes) |
| SPRM_NOMBRE | VARCHAR(100) | "Roshan de Halloween" |
| SPRM_IMAGEN_URL / SPRM_SILUETA_URL | VARCHAR(500) | |
| CTOR_ID_TORNEO | INT FK NULL | International del mes |
| CEQU_ID_EQUIPO | INT FK NULL | ganador (null = aún sin ganador) |
| SPRM_FOTO_GANADOR_URL | VARCHAR(500) NULL | **si es null se muestra el logo del equipo ganador** (`COALESCE(SPRM_FOTO_GANADOR_URL, SEQU_LOGO_URL)` en el SP de consulta) |
| FPRM_ESTADO | CHAR(1) DEFAULT 'V' | V · I · A |
| + AUD | | |

## Medalla Dota (cambia con el tiempo)
- En `TLGD_JUGADOR` se guarda solo la **medalla actual** (código + estrellas + puesto Immortal) y la fecha de la última sincronización. Cada cambio queda en `TLGD_JUGADOR_MEDALLA` (historial), así se puede ver la evolución y saber con qué medalla jugó alguien un torneo.
- Fuente: API de OpenDota (`/players/{account_id}` → `rank_tier`: decena = medalla, unidad = estrellas; `leaderboard_rank` para Immortal).
- **Sincronización semanal automática: lunes 3:00 a. m. (hora Perú).** Recomendada sobre el domingo 11 p. m. porque el último domingo del mes se juega la final del International y conviene no cargar el sistema; el lunes de madrugada es el momento con menos actividad. Lo ejecuta un servicio programado del API, en lotes y con pausas (OpenDota gratuito permite ~60 consultas por minuto).
- Además: se sincroniza **al iniciar sesión** si la última sincronización tiene más de 7 días, y **al inscribirse** en un torneo (para validar la medalla real del momento).
- SP: `PRLGD_UPD_MEDALLA_JUGADOR` actualiza la medalla actual e inserta en el historial **solo si cambió**.

## Reglas de negocio que garantiza la base (SPs + restricciones)
- Un jugador activo en un solo equipo; un solo capitán activo por equipo. Las acciones de capitán (editar logo/banner, invitar, expulsar, inscribir) las valida el SP contra `TLGD_EQUIPO_MIEMBRO`.
- **Un equipo no puede inscribirse a un torneo sin logo** (`SEQU_LOGO_URL` no nulo); el banner es opcional.
- Un equipo se inscribe una sola vez por torneo y juega un solo International por mes.
- Los clasificados de Satélite y Battle Cup se inscriben solos en el International del mes.
- **Un equipo que ya tiene cupo al International del mes no puede participar en ningún otro torneo de ese mes**; sus inscripciones pendientes en otros torneos de ese mes se anulan al clasificar (con reembolso si estaban pagadas).
- Pagos idempotentes por `CPAG_CULQI_CARGO_ID`; pagos y cobros nunca se borran, solo cambian de estado.
- Toda escritura de dinero o clasificación va en transacción y responde con `@o_resultMessage`.

## Scripts (a crear tras aprobar este diseño y con permiso para ejecutarlos)
```
DataBase/
  scripts/
    00_ejecutar_todo.sql      (modo SQLCMD, :r de los demás)
    01_crear_base.sql
    02_tablas.sql
    03_catalogos.sql          (TLGD_TIP_VALOR)
    04_sp_jugador_equipo.sql
    05_sp_torneos_inscripciones.sql
    06_sp_partidas.sql
    07_sp_pagos_cobros.sql
  seed/
    datos_prueba.sql          (los mismos equipos y torneos que hoy muestra el front)
```
