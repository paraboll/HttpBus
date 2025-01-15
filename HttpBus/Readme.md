# Развертывание через службу:

## 1. Создание службы: 
    sc create "MyTestBus" binpath="путь до MyTestBus.exe"
    sc start MyTestBus
    sc stop MyTestBus
    sc stop MyTestBus
    sc delete MyTestBus

## 2. Swagger: 
	http://localhost:5000/swagger/index.html

## 3. Пример appsettings.json
```
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },

  "DbConfig": {
    //"Provider": "SQLServer",
    //"ConnectionString": "Server=localhost;Database=helloappdb1;User Id=sa;Password=sa;TrustServerCertificate=false;"
    "Provider": "InMemory"
  },

  "Kestrel": {
    "Endpoints": {
      "Http": {
        "Url": "http://localhost:5000"
        //},
        //"Https": {
        //  "Url": "https://localhost:5001"
      }
    }
  },

  "AllowedHosts": "*"
}
```
---

# Развертывание через докер:

## 1. appsettings.json:
```
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },

  "DbConfig": {
    "Provider": "InMemory"
  },

  "Kestrel": {
    "Endpoints": {
      "Http": {
        "Url": "http://0.0.0.0:5554"
      }
    }
  },

  "AllowedHosts": "*"
}
```

## 2. NLog.config:
```
<?xml version="1.0" encoding="utf-8" ?>
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
      xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
      xsi:schemaLocation="http://www.nlog-project.org/schemas/NLog.xsd NLog.xsd"
      autoReload="true"
      throwExceptions="false"
      internalLogLevel="Off" internalLogFile="c:\temp\nlog-internal.log">

  <variable name="myvar" value="myvalue"/>

  <targets>
    <target xsi:type="File" name="f" fileName="${basedir}/logs/${shortdate}.log"
            layout="${longdate} ${uppercase:${level}} ${message}" />
  </targets>

  <rules>
    <logger name="*" minlevel="Warn" writeTo="f" />
  </rules>
</nlog>
```

## 3. Создайте образ Docker. Откройте терминал в директории с вашим Dockerfile и выполните команду:
	docker build -t my_test_bus .

## 4. Запустите контейнер с ограничением памяти. Используйте следующую команду для запуска контейнера с ограничением на 4 ГБ оперативной памяти:
	docker run -d --name MyTestBus -p 5554:5554 --memory="4g" my_test_bus