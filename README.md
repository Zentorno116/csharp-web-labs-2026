# Веб-программирование — 7 семестр (Замилов Тимур Рамилевич)

Все 4 задания из ТЗ (`Семестр 7.md`) реализованы на **.NET 10**, **PostgreSQL 18**, **SignalR** и **React + Vite + TypeScript**.

## 1. LINQ (`01_LinqApp`, 8 баллов)
Вариант: **7, 17, 27, 37, 47, 57, 67** (одна последовательность) и **77, 87, 97** (несколько взаимосвязанных последовательностей) из `NET_LINQ.pdf`.
- Поддержка кодировки `windows-1251` (`EncodingHelper.cs`) + автоматическая генерация тестовых входных/выходных файлов.
- Запуск всех задач: `dotnet run --project 01_LinqApp/LinqApp -- --all` (или без `--all` для интерактивного меню).
- Тесты (11/11): `dotnet test 01_LinqApp/LinqApp.slnx`

## 2. SignalR Чат с комнатами и модераторами (`02_SignalRChat`, 8 баллов)
Реалтайм-чат **PulseChat** (`http://localhost:5020`):
- Создание/удаление комнат, счётчики онлайна, история сообщений, индикатор набора текста, тёмная/светлая тема.
- Роли: **Владелец** (`admin` в общих комнатах или создатель новой комнаты), **Модератор** (`moderator` или назначенный владельцем), **Участник**.
- Модерация: назначение/снятие модератора, мут на 5 минут / размут, кик, бан / разбан (модальное окно списка банов), удаление сообщений.
- Запуск: `dotnet run --project 02_SignalRChat/SignalRChat`
- Тесты: `dotnet test 02_SignalRChat/SignalRChat.slnx`

## 3. ASP.NET Core 10 Web API — TaskFlow (`03_TaskFlowApi`, 20 баллов)
REST API трекера проектов и задач (`http://localhost:5015`):
- **PostgreSQL 18** (`TaskFlowDB`, EF Core миграции в `Migrations/`, автомиграция и seed при старте).
- **5 сущностей с CRUD**: `Users`, `Projects`, `TaskItems`, `Comments`, `Tags` (связи 1:N и M:N `TaskItemTags`).
- **DTO-контроллеры**: `AuthController`, `UsersController`, `ProjectsController`, `TasksController`, `CommentsController`, `TagsController`.
- **JWT авторизация** (`POST /api/auth/login`, `/token`, `/api/auth/register`, роли `admin` и `user`) + **Swagger с JWT Bearer** (`/swagger/index.html`).
- **Repository + UnitOfWork** (`Repositories/`) и **Service Layer** (`Services/`).
- **Unit-тесты с Moq** (17/17): `dotnet test 03_TaskFlowApi/TaskFlowApi.slnx`.
- Демо-аккаунты: `admin@taskflow.local` / `Admin123!` и `timur@taskflow.local` / `User123!`.

## 4. Клиентское приложение + хостинг ngrok (`04_TaskFlowClient`, 15 баллов)
SPA на **React + Vite + TypeScript**:
- Вход/регистрация по JWT, Kanban-доска задач, фильтры, полный CRUD по проектам, задачам, комментариям, тегам и пользователям (управление ролями для `admin`).
- Режим разработки: `cd 04_TaskFlowClient && npm run dev` (`http://localhost:5173`, проксирует `/api` и `/swagger` на `:5015`).
- Продакшен-сборка в API + публичный HTTPS через **ngrok**:
  1. Один раз добавьте токен с https://dashboard.ngrok.com:
     `.\04_TaskFlowClient\tools\ngrok.exe config add-authtoken <ВАШ_ТОКЕН>`
  2. Запустите одной командой:
     `.\start-with-ngrok.ps1` (для TaskFlow API + React + Swagger) или `.\start-with-ngrok.ps1 -App chat` (для SignalR чата).
