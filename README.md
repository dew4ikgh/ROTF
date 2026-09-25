# Remains of the Forest (ROTF)

Клієнт-серверна багатокористувацька гра в жанрі **survival-horror**.
Проєкт складається з ігрового клієнта на **Unity 6** та бекенду
**REST API** на **ASP.NET Core (.NET 8)**, які разом забезпечують
автентифікацію гравців, збереження прогресу, інвентарю та стану світу.

> На основі курсового проєкту «Розробка клієнт-серверної архітектури для
> багатокористувацької гри в жанрі survival-horror "Remains of the Forest"»
> (Шишковський Денис, ІІІ курс, спец. 121 «Інженерія програмного забезпечення»).

## Призначення проєкту

Гравець опиняється у ворожому лісовому середовищі, де потрібно виживати
(здоров'я, голод, спрага, втома, температура тіла), збирати ресурси,
будувати укриття та взаємодіяти з іншими гравцями на спільному сервері.
Уся критична бізнес-логіка (прогрес, інвентар, авторизація) винесена на
бекенд за принципом «тонкий клієнт – товстий сервер», що унеможливлює
читерство через локальну зміну збережень.

**Основні задачі, які вирішує проєкт:**
- проєктування реляційної бази даних для прогресу, інвентарю та стану світу;
- REST API на ASP.NET Core + Entity Framework Core;
- захищена автентифікація (JWT-токени, хешування паролів BCrypt);
- клієнт на Unity 3D (HDRP), що взаємодіє з сервером через HTTP.

## Архітектура

Гібридна клієнт-серверна архітектура:

- **Клієнт (Unity 6, HDRP)** — компонентно-орієнтована архітектура
  (MonoBehaviour / NetworkBehaviour). Відповідає лише за рендеринг,
  фізику, ввід користувача та мережеву синхронізацію (Unity Netcode).
- **Сервер (ASP.NET Core Web API)** — багатошарова архітектура (3-Tier):
  `Controllers` (HTTP) → `Services` (бізнес-логіка) → `Repositories`
  (доступ до даних через EF Core) → **MySQL**.

Патерни проєктування: **Singleton** (менеджери на клієнті),
**Repository** (ізоляція доступу до БД), **DTO** (обмін даними клієнт↔сервер),
**State Pattern** (стани гравця/ІІ ворогів).

## Структура проєкту

Бекенд (`ROTF.Server`):

ROTF.Server/
├── Common/
│   └── ServiceResponse.cs        # уніфікована обгортка відповіді сервісів
├── Controllers/
│   ├── BaseApiController.cs      # базовий контролер, api/v1/[controller]
│   ├── AuthController.cs         # register / login
│   ├── ProfileController.cs      # профіль гравця, аватар
│   ├── ServersController.cs      # список / створення ігрових серверів
│   ├── PlayerProgressController.cs
│   ├── InventoryController.cs
│   ├── ItemsController.cs
│   └── BuildingsController.cs
├── Data/
│   └── AppDbContext.cs           # DbContext (EF Core)
├── DTOs/
│   ├── LoginDto.cs, RegisterDto.cs, UserProfileDto.cs
│   ├── GameServerDTO.cs (CreateServerDto)
│   └── PlayerProgressDtos.cs
├── Interfaces/ (Services/Interfaces, Repositories)
│   └── IAuthService, IProfileService, IServerService,
│       IPlayerProgressService, IInventoryService, IItemService,
│       IBuildingService, IWorldItemService, IGenericRepository<T>,
│       IInventoryRepository
├── Middleware/
│   └── ExceptionMiddleware.cs    # глобальна обробка винятків
├── Models/                       # сутності EF Core (Users, Servers, Items,
│                                  # PlayerProgress, Inventory, WorldItems, Buildings)
├── Repositories/
│   ├── GenericRepository.cs
│   └── InventoryRepository.cs
├── Services/
│   ├── AuthService.cs, ProfileService.cs, ServerService.cs
│   └── PlayerProgressService.cs, InventoryService.cs, ItemService.cs,
│       BuildingService.cs, WorldItemService.cs
├── appsettings.json               # ConnectionStrings:DefaultConnection, Jwt:*
└── Program.cs


Клієнт (Unity, ключові скрипти за темами):

Assets/Scripts/
├── Auth/          AuthManager.cs, RegistrationController.cs
├── Player/         FPSMovement.cs (NetworkBehaviour), PlayerStats.cs,
│                   стани: IdleState, RunningState, CraftingState, DeadState
├── Network/       MultiplayerMenuController.cs (список/створення серверів)
├── Profile/       ProfileService.cs (клієнтський виклик GET /profile/me)
├── SaveSystem/    GameSaveManager.cs, SaveSystem.cs
└── UI/            AuthPanel, HUD, PauseMenu, SurvivalTab та ін. (Canvas)


Модель бази даних (MySQL)

| Таблиця | Призначення |
|---|---|
| `users_r` | облікові записи гравців (username, email, passwordhash, roleid) |
| `servers` | ігрові сервери/сесії, створені гравцями (FK → users_r) |
| `items` | статичний довідник ігрових предметів |
| `playerprogress` | прогрес персонажа: HP, stamina, hunger, thirst, fatigue, temperature, координати, `UNIQUE(userid, serverid)` |
| `inventory` | зв'язок «багато-до-багатьох» playerprogress ↔ items |
| `world_items` | предмети, розкидані/заспавнені у світі конкретного сервера |
| `buildings` | споруди гравців (тип, координати, міцність) |

База нормалізована до 3NF, усі зв'язки — з `ON DELETE CASCADE`.

## Основні API-ендпоінти

| Метод | Маршрут | Опис |
|---|---|---|
| POST | `/api/v1/auth/register` | реєстрація нового гравця |
| POST | `/api/v1/auth/login` | вхід, повертає JWT |
| GET | `/api/v1/profile/me` | дані профілю (потрібен JWT) |
| POST | `/api/v1/profile/avatar` | завантаження аватара |
| GET | `/api/v1/servers` | список ігрових серверів |
| POST | `/api/v1/servers/create` | створення сервера |
| GET | `/api/v1/playerprogress/server/{serverId}` | прогрес гравця на сервері |
| GET | `/api/v1/playerprogress/my-saves` | усі збереження користувача |
| POST | `/api/v1/playerprogress/update-stats` | оновлення показників виживання |
| POST | `/api/v1/playerprogress/save-position` | збереження позиції у світі |
| GET | `/api/v1/inventory/server/{serverId}` | інвентар гравця |
| POST | `/api/v1/inventory/add` | додати предмет до інвентаря |
| DELETE | `/api/v1/inventory/item/{id}` | видалити предмет |
| GET | `/api/v1/items` / `/{id}` | довідник предметів |
| GET | `/api/v1/buildings/server/{serverId}` | споруди на сервері |
| POST | `/api/v1/buildings/place` | розмістити споруду |
| DELETE | `/api/v1/buildings/{id}` | знести споруду |

Усі ендпоінти, окрім `auth/*`, захищені атрибутом `[Authorize]` (JWT Bearer).

## Технології та залежності

**Сервер (.NET 8 / NuGet):**
- ASP.NET Core Web API
- Entity Framework Core 8 (`Pomelo.EntityFrameworkCore.MySql` — провайдер MySQL)
- `Microsoft.AspNetCore.Authentication.JwtBearer` — автентифікація JWT
- `BCrypt.Net-Next` — хешування паролів
- `Serilog.AspNetCore` (+ файловий синк) — логування (`Logs/rotf_server_log-*.txt`)
- `Swashbuckle.AspNetCore` — Swagger/OpenAPI з підтримкою Bearer-токена
- MySQL (СУБД)

**Клієнт (Unity 6):**
- Render Pipeline: **HDRP**
- **Unity Netcode for GameObjects** (`Unity.Netcode`) — мультиплеєрна синхронізація
- Новий **Input System** (`UnityEngine.InputSystem`)
- **TextMeshPro**
- `UnityWebRequest` / `UnityEngine.Networking` — HTTP-запити до REST API
- NavMesh — навігація ІІ ворогів
- Вбудований фізичний рушій PhysX

**Інструменти розробки:** Visual Studio 2022 / JetBrains Rider, Postman
(тестування ендпоінтів), Git/GitHub.

## Налаштування та запуск (сервер)

1. Встановити .NET 8 SDK та MySQL Server.
2. У `appsettings.json` заповнити:
   json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=rotf;User=root;Password=***;"
     },
     "Jwt": {
       "Issuer": "ROTF.Server",
       "Audience": "ROTF.Client",
       "Key": "тут-довгий-секретний-ключ"
     }
   }
   
3. Застосувати міграції EF Core:
   bash
   dotnet ef database update
4. Запустити сервер:
   bash
   dotnet run
   Swagger UI доступний у Development-режимі за `/swagger`.
5. У Unity-клієнті вказати базову URL-адресу API (у налаштуваннях
   мережевого/HTTP-сервісу клієнта) та запустити сцену авторизації.
