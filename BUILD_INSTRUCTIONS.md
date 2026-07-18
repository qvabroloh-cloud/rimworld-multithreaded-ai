# 📋 Инструкция по сборке RimWorld Pawn AI Multithread Mod

## ✅ Способ 1: Автоматическая сборка через GitHub Actions (Рекомендуется)

### Что нужно:
- GitHub аккаунт
- Репозиторий с кодом мода

### Как сделать:

1. **Выгрузите файлы в GitHub**
   ```bash
   git init
   git add .
   git commit -m "Initial commit"
   git push -u origin main
   ```

2. **Создайте тег для выпуска**
   ```bash
   git tag v1.1.0
   git push origin v1.1.0
   ```

3. **GitHub Actions автоматически:**
   - ✅ Скомпилирует код
   - ✅ Создаст структуру мода
   - ✅ Загрузит артефакт в Release
   - ✅ Создаст готовый файл для скачивания

---

## 🏠 Способ 2: Локальная сборка на Windows

### Требования:
- **Visual Studio 2019+** (Community версия бесплатна)
- **.NET Framework 4.7.2**
- **RimWorld 1.4+** (для библиотек)

### Шаг 1: Установите Visual Studio
```
Скачайте: https://visualstudio.microsoft.com/downloads/
Выберите: "Desktop development with C++" + ".NET Framework 4.7.2"
```

### Шаг 2: Найдите RimWorld в Steam
```
C:\Program Files\Steam\steamapps\common\RimWorld\
```

### Шаг 3: Откройте проект
```
1. Откройте Visual Studio
2. File → Open → Project/Solution
3. Выберите: Source/PawnAIMultithread.csproj
```

### Шаг 4: Компилируйте
```
1. Build → Build Solution (Ctrl+Shift+B)
2. Выберите: Release configuration
3. Подождите завершения
```

### Шаг 5: Найдите DLL
```
Source/bin/Release/PawnAIMultithread.dll
```

---

## 📦 Способ 3: Сборка через Command Line (MSBuild)

### На Windows:

```batch
# Откройте Command Prompt как администратор

# Перейдите в папку проекта
cd C:\Users\YourName\Documents\rimworld-multithreaded-ai

# Выполните сборку
msbuild Source\PawnAIMultithread.csproj /p:Configuration=Release /p:Platform="Any CPU"

# Результат будет в:
# Source\bin\Release\PawnAIMultithread.dll
```

---

## ✨ Структура готового мода

После компиляции создайте такую структуру:

```
PawnAIMultithread/
├── Assemblies/
│   └── PawnAIMultithread.dll        ← Скомпилированный файл
├── About/
│   ├── About.xml
│   └── Manifest.xml
└── Source/                          ← Опционально, для исходников
    └── *.cs файлы
```

---

## 📥 Установка скомпилированного мода

### На Windows:

```
1. Найдите папку Mods в RimWorld:
   C:\Program Files\Steam\steamapps\common\RimWorld\Mods\

2. Создайте новую папку:
   Mods/PawnAIMultithread/

3. Скопируйте туда:
   ├── Assemblies/
   │   └── PawnAIMultithread.dll
   ├── About/
   │   ├── About.xml
   │   └── Manifest.xml
   └── Source/ (опционально)

4. Запустите RimWorld

5. Mods → Manage Mods → Включите "Pawn AI Multithread"

6. Перезагрузите игру
```

---

## 🐛 Решение проблем

### Ошибка: "Assembly-CSharp.dll not found"
```
Решение: Проверьте путь к RimWorld в .csproj файле
Должно быть: C:\Program Files\Steam\steamapps\common\RimWorld
```

### Ошибка: ".NET Framework 4.7.2 not found"
```
Решение: Установите .NET Framework 4.7.2
https://www.microsoft.com/net/download/dotnet-framework-runtime
```

### Ошибка: "0Harmony.dll not found"
```
Решение: HugsLib должен быть установлен как мод в RimWorld
Или скачайте сам файл 0Harmony.dll
```

### Ошибка: Compilation errors (ошибки синтаксиса)
```
Решение:
1. Убедитесь, что все .cs файлы в правильных папках
2. Проверьте версию C# (должна быть latest)
3. Очистите и пересоберите: Clean → Rebuild
```

---

## 🎯 Быстрый чек-лист

- [ ] Установлена Visual Studio 2019+
- [ ] Установлен .NET Framework 4.7.2
- [ ] Найдена папка RimWorld
- [ ] HugsLib установлен как мод
- [ ] Все .cs файлы скопированы
- [ ] .csproj файл обновлён с путями
- [ ] Выполнена сборка (Build → Build Solution)
- [ ] DLL файл создан в bin/Release/
- [ ] Мод скопирован в RimWorld/Mods/
- [ ] Мод активирован в игре

---

## 📞 Поддержка

Если что-то не работает:

1. Проверьте логи: `%AppData%\LocalLow\Ludeon Studios\RimWorld by Ludeon Studios\Player.log`
2. Посмотрите версию RimWorld: `About → Version`
3. Убедитесь, что HugsLib загружается перед модом
4. Очистите и пересоберите проект