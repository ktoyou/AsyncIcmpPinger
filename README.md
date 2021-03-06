Развертка базы
==============

Для того, чтобы запустить пинговалку, нам нужно развернуть базу данных.
```mysql
CREATE DATABASE pinger;
USE pinger;
```

После того, как база развернута, нужно создать пользователя, с помощью
которого пинговалка будет работать с базой данных. И сразу дадим пользователю
привелегии для использования таблиц в базе.

```mysql
CREATE USER 'ktoyou'@'localhost' IDENTIFIED BY 'testpassword';
GRANT ALL PRIVILEGES ON *.* TO 'ktoyou'@'localhost'
```

После проделанных выше манипуляций, создадим табличку с такой структурой, с
которой работает пинговалка.

```mysql
CREATE TABLE ping_objects (
  id int NOT NULL AUTO_INCREMENT,
  address varchar(64) NOT NULL,
  title varchar(64) NOT NULL,
  description text,
  online tinyint(1) DEFAULT '0',
  enabled tinyint(1) DEFAULT '0',
  PRIMARY KEY (id)
) ENGINE=InnoDB;
```

```mysql
CREATE TABLE events (
    id INT NOT NULL AUTO_INCREMENT,
    level INT NOT NULL,
    event_id VARCHAR(64) NOT NULL,
    message TEXT NOT NULL,
    begin INT NOT NULL,
    end INT NOT NULL,
    PRIMARY KEY(id)
);
```

id - уникальный идентификатор объекта

address - ip адресс объекта

title - заголовок объекта

description - текстовое описание объекта

online - онлайн объект или нет

enabled - будет ли пинговаться объект

Развертка пинговалки
====================

Linux
-----
Заходим в склонированный репозиторий пинговалки, и пишем следующее:
dotnet build

Данная команда подтянет все необходимые зависимости с nuget, и соберет проект

Windows
-------
Открываем Visual Studio, желательно 2017, 2019, 2022. Открываем sln файл проекта,
и нажимаем кнопку "build", после чего проект соберется.

Запуск пинговалки
=================

Если пинговалка берет данные с базы данных, то нам нужно создать конфигурационный файл.
```json
{
    "server":"xxx",
    "db":"xxx",
    "uid":"xxx",
    "password":"xxx"
}
```
server - сервер к которому будет подключатся пинговалка.

db - имя базы данных где мы будем работать.

uid - имя пользователя.

password - пароль пользователя.

После чего, если все верно сделано - пинговалка запустится, и начнет пинговать объекты из базы.
