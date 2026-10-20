# UP_Markov_Matveev WPF-приложение «Логистическая компания»

Группа 4ИСИП-523, тема курсового проекта: «Проектирование и разработка базы данных
для логистической компании» (клиентская часть: Clients, Warehouses, Orders, Cargo).

## Структура репозитория

```
UP_Markov_Matveev.sln   — решение Visual Studio
UP_Markov_Matveev/      — исходный код WPF-проекта
db/schema.sql           — скрипт создания БД и тестовых данных
res/                    — иконка приложения и изображения
README.md               — этот файл
```

## Технологии

- WPF, .NET Framework 4.7.2, C#
- Microsoft SQL Server + Entity Framework 6, Database First (класс `Entities`
  и модели таблиц созданы мастером «ADO.NET Entity Data Model», `.edmx`-файл)
- Microsoft.Office.Interop.Word / Excel — выгрузка отчётов в .docx, .pdf, .xlsx
- System.Windows.Forms.DataVisualization.Charting — диаграммы

## Как запустить

1. Выполните `db/schema.sql` в SQL Server Management Studio — скрипт создаёт
   базу `DB_Markov_Matveev`, все таблицы, связи и тестовые данные.
2. Откройте `UP_Markov_Matveev.sln` в Visual Studio (с компонентом ".NET
   desktop development").
3. Восстановите NuGet-пакеты (`EntityFramework` 6.4.4,
   `Microsoft.Office.Interop.Word` 15.0.4797.1004,
   `Microsoft.Office.Interop.Excel` 15.0.4795.1001).
4. Проверьте строку подключения в `App.config` (`connectionStrings` →
   `Entities`) — при необходимости поправьте имя сервера на своё.
5. Запустите проект (F5). Откроется страница авторизации.

Если модель `Entities.edmx` в проекте отсутствует или не подключена к базе —
её нужно пересоздать через **Добавить → Создать новый элемент → «Модель
ADO.NET EDM»** (EF Designer from database, Entity Framework 6.x, все 9
таблиц, без пluralize/singularize). Таблица соответствия навигационных
свойств, которые при этом нужно выставить в дизайнере `.edmx`:

| Таблица      | Связь с           | Навигационное свойство |
|--------------|--------------------|-------------------------|
| `Orders`     | `Clients`          | `Client`                |
| `Orders`     | `Warehouses`       | `Warehouse`              |
| `Orders`     | `OrderStatuses`    | `OrderStatus`            |
| `Clients`    | `ClientTypes`      | `ClientType`             |
| `Clients`    | `Cities`           | `City`                   |
| `Warehouses` | `Cities`           | `City`                   |
| `Cargo`      | `Orders`           | `Order`                  |
| `Cargo`      | `CargoTypes`       | `CargoType`              |

Имя контейнера сущностей и имя строки подключения в `App.config` — `Entities`.

### Учётные записи для входа

| Логин | Пароль   | Роль  |
|-------|----------|-------|
| admin | admin123 | Admin |
| user  | user123  | User  |

Также можно зарегистрировать нового пользователя через кнопку «Регистрация».

## Что реализовано

- Главное окно с Frame-навигацией, часами и подтверждением выхода
- Два словаря стилей по корпоративному Руководству по стилю, переключение
  темы с сохранением выбора между запусками
- Авторизация с хэшированием пароля (SHA1) и капчей после 3 неверных попыток
- Регистрация с проверкой заполнения полей и выбором роли
- Смена пароля
- Страница пользователя: список заказов с поиском, сортировкой по дате и
  фильтром по статусу
- Страница администратора: CRUD для Clients, Warehouses, Orders, Cargo
  (добавление / редактирование / удаление с подтверждением)
- Каскадное удаление на уровне БД (удаление клиента удаляет его заказы,
  удаление заказа удаляет его грузы; удаление склада не ломает заказы —
  WarehouseId становится NULL)
- Диаграмма стоимости заказов по статусам (в разрезе клиента, с выбором типа
  диаграммы)
- Выгрузка отчёта по заказам в Microsoft Word (.docx и .pdf) и Microsoft
  Excel (.xlsx)
  
