# Liga D

> Copia del `CLAUDE.md` del espacio de trabajo `D:\AIwara\Dota\` (carpeta con `ws_ligaD_front/`, `ws_ligaD_back/` y `General Liga D/`). Para trabajar con todo el contexto, clona ambos repos dentro de una misma carpeta. La carpeta `General Liga D/` (manual, imágenes originales, prompts, mecánica del negocio) se comparte aparte, no está en git.

Plataforma de torneos amateur de Dota 2 (Perú) con modelo de buy-ins y satélites tipo póker: Satélites Exprés → Battle Cups semanales → International Cup mensual. Pagos con Culqi, retiros de premios por Yape/Plin/BCP.

## Carpetas
- `ws_ligaD_front/`: front. Vue 3 + Vite + Vuetify + vue-router. Repo git `github.com/jmvillafanamdoza/ws_ligaD_front`. `npm run dev` → http://localhost:5173. Todo funciona con datos mock (`src/data/*.mock.js`). Componentes por niveles: `components/atoms|molecules|organisms`, más `components/profile/`.
- `ws_ligaD_back/`: back. **API .NET 9 (Minimal API + Dapper + SPs) + SQL Server**, misma arquitectura que el CRM MKK. Repo git local (rama `main`, sin remoto aún). Desarrollo: base `BD_LIGAD` en `.\SQLEXPRESS` (SQL Server 2022, autenticación de Windows). Diseño de la base: `ws_ligaD_back/DataBase/README.md` (estilo MKK: tablas `TLGD_`, SPs `PRLGD_`, IDs INT IDENTITY). El esqueleto NestJS/Prisma anterior se borró (2026-10-18).
- `General Liga D/`: material general del proyecto. `Liga_D_Manual.pdf` (manual de identidad visual, 112 MB: no se puede leer directo, hay que extraer el texto con `pypdf`) y `imagenes/` (`fondo_azul_esencia.png`, `fondo_ancients.jpeg`, `imagen logo.jpg`; las del inicio público van en `imagenes/Home/`: fondo de fuego, torres del hero y portadas de torneo). Aquí va la documentación y los datos de negocio nuevos. Las imágenes que usa el front se convierten a WebP en `ws_ligaD_front/public/img/` (no se sirven los originales pesados).
- Hay dos "inicios": `/home` es la cara comercial pública (hero con `fondo_rojo_fuego`); `/panel` es el inicio privado del jugador.
- Logo oficial: archivos del diseñador en `General Liga D/imagenes/wetransfer_liga-d_2026-10-18_0158/` (AI, EPS, PDF, PNG; la carpeta "JPG" trae PNG 1080×1080). En el front se usa `v1_borde` (principal) y `blanco` (footer), exportados a `public/img/marca/` y referenciados solo vía `BRAND_LOGO` en `src/data/images.js`.
- Identidad de equipo: si el equipo no subió logo/banner se usan los de por defecto. Logo = `components/atoms/TeamLogo.vue` (escudo SVG con iniciales y color derivado del nombre); banner = `public/img/equipo/banner_default.webp`. Solo el capitán edita (`TeamBrandingModal.vue`, hoy solo vista previa local). Almacenamiento pendiente de definir (propuesta: Cloudflare R2); en la base: `SEQU_LOGO_URL` / `SEQU_BANNER_URL` (null = por defecto).
- Sidebar del panel (`layouts/AppShell.vue`): `components/sidebar/` → `PremioDelMes` (datos en `src/data/premioDelMes.js`, imágenes en `public/img/premios/`), `ProximoTorneo` (primer elemento de `upcomingTournaments`, misma fuente que las cards del home) y `CreditoEmpresa` (`src/data/empresa.js`). En pantallas < lg o con alto < 820px se oculta el premio y el próximo torneo va compacto.
- Imágenes del `/home`: todos los espacios (IMG 01–10, con medida) se configuran en `ws_ligaD_front/src/data/images.js`; con `src: null` se ve un recuadro punteado (`components/atoms/ImageSlot.vue`). Las imágenes son propias del proyecto (no usar arte de Valve como imagen principal). Las tarjetas compartidas con el panel solo muestran imagen con `show-cover` / `show-logo`.

Diseño de la base y reglas que garantiza: @DataBase/README.md. (`ws_ligaD_front/README_ARQUITECTURA_LIGAD.md` y `ws_ligaD_front/docs/` describen el plan antiguo NestJS/Prisma: solo como referencia histórica.)

## Agentes del proyecto (`.claude/agents/`)
- `db-dev`: SQL Server — tablas, SPs `PRLGD_*`, catálogos y datos de prueba en `ws_ligaD_back/DataBase/`.
- `backend-dev`: API .NET 9 — endpoints, repositorios Dapper, auth Steam/JWT, Culqi, wallet.
- `frontend-dev`: vistas Vue/Vuetify, tema, conexión con la API.
Delegar en ellos según la capa. Para una funcionalidad completa, el orden es: db-dev → backend-dev → frontend-dev. Ningún agente hace commits.

## Estado real (2026-10-18)
- Diseño visual del front cerrado (pendiente solo la imagen P04 del slide Discord). Todo funciona con datos de prueba.
- Back: carpeta limpia con repo git (sin commits). **Diseño de la base v2 aprobado** (`DataBase/README.md`). Aún no existe `BD_LIGAD` ni el proyecto .NET. **Siguiente: fase 1 (scripts de la base); pedir permiso antes de ejecutar cualquier cosa sobre la base.**
- Mecánica del negocio y decisiones pendientes: `General Liga D/MECANICA_DEL_NEGOCIO.md` (también publicada como artifact privado: https://claude.ai/artifact/UXTsrahvTobTTw96uwnoti).
- Fases: 1) base (tablas, catálogos, semillas) → 2) API base → 3) login Steam + JWT → 4) torneos, equipos, inscripciones → 5) pagos Culqi, wallet, cobros → 6) conectar el front.

## Reglas de negocio clave
- International Cup (mensual): S/ 50 por equipo o gratis canjeando un cupo; pozo garantizado S/ 1,000.
- Battle Cup (semanal, 3 al mes): 8 equipos, S/ 25 por equipo; aporta S/ 50 al pozo del International; campeón gana S/ 100 + cupo.
- Satélites (4 por semana: vie 6 y 9 pm, sáb 4 y 7 pm): 4 equipos, S/ 15 por equipo; premio: cupo al International.
- Cupos: los equipos no los gestionan; al ganar se inscriben solos en el International del mes. **Un equipo con cupo al International del mes ya no puede participar en ningún otro torneo de ese mes** (sus inscripciones pendientes se anulan, con reembolso si estaban pagadas).
- Login con Steam OpenID + JWT; medalla Dota verificada con OpenDota.
- Dinero: operaciones de pagos, wallet y retiros siempre en transacciones; nunca guardar llaves de Culqi en el repo.

## Marca (manual de identidad)
- Paleta: Deep Space Blue #213C56, Hot Fuchsia #FF4654, Jet Black #1A2632, Turquoise #7AD8BD, White #FFFFFF.
- Tipografía del logo: Countach (Adobe Fonts). Logo = isologo (espada de Dragon Knight con reflejos neón); no separar sus elementos. Tamaño mínimo digital: 30×30 px.
- Aplicada en el tema de Vuetify (`src/plugins/vuetify.js`); los componentes usan `rgb(var(--v-theme-*))`.
