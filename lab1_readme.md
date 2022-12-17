## Запуск тестов

Запустите скрипт run_unit_tests.bat (Windows) или run_unit_tests.sh (Linux, wsl и тд. Если просто тыкнуть на run_unit_tests.sh, то он выполняется из bash-оболочки, а там (у меня) ничего не настроено, падает), который лежит в корне склонированного репозитория. 

Скрипт прогонит тесты как для BL, так и для AccessToDB, сгенерирует единый allure-отчет и откроет его в браузере.


Что требуется: dotnet (6.0), allure, docker (Так как в проекте в качестве БД используется Tarantool, скрипт в процессе работы поднимает докер-контейнер с тарантулом)


Также все тесты прогоняются при push/pull в репозиторий (см. Actions).


## Дисклеймер

1. Лучше запускать не с винды (не очень дружит с поднятым докером, тесты для AccessToDB могут упасть по непрредвиденным причинам.
 
2. (у меня на wsl) Когда запускаем allure open из bash, он пишет Server started at <http://127.0.1.1:41205/>. По такому адресу из винды отчет не откроется. Нужно в bash выполнить "ip a", посмотреть "...4: eth0: ...... inet 172.25.134.163". Берем последний ip-шник и вставляем его вместо 127.0.1.1 (порт не меняем). Вуаля

3. Если На Windows что-то идет не так, то скрипт лучше запустить вот так: cmd /k cmd /c run_unit_tests.bat (хоть сам скрипт не вылетит, можно почитать)


## Дисклемер перед описанием тестов
Во всех тестах явно прописана структура Arrange-Act-Assert (для себя и для наглядности). 

По заданию нужно было попробовать много чего, поэтому далее пропишу, что где используется.


## 1. Unit-Tests for BuisinessLogic (BL.Tests)

LiftsSlopesServiceTests выполнен в классическом стиле (изоляция тестов), все остальные -- в Лондонском (изоляция кода от зависимостей)

1. LiftsServiceTest
* fixture с помощью AutoMoqData (собственный атрибут AutoMoqDataAttribute, лежит в папке ArrangeHelpers) (по сути Dummy)-- для генерации объектов для тестов 
* mock для: ICheckPermissionsService (вызовы и взаимодействия, которые исполняются SUT к зависимым объектам) 
* stub для: ILiftsRepository, ILiftsSlopesRepository (вызовы и взаимодействия,  которые исполняются SUT к зависимым объектам, чтобы запросить и получить  данные) 

2. SlopesServiceTest
* fixture с помощью AutoMoqData (собственный атрибут AutoMoqDataAttribute, лежит в папке ArrangeHelpers) (по сути Dummy)-- для генерации объектов для тестов 
* mock для: ICheckPermissionsService (вызовы и взаимодействия, которые исполняются SUT к зависимым объектам) 
* stub для: ISlopesRepository, ILiftsSlopesRepository (вызовы и взаимодействия,  которые исполняются SUT к зависимым объектам, чтобы запросить и получить  данные) 

3. TurnstilesServiceTest
* fixture с помощью AutoMoqData (собственный атрибут AutoMoqDataAttribute, лежит в папке ArrangeHelpers) (по сути Dummy)-- для генерации объектов для тестов 
* mock для: ICheckPermissionsService (вызовы и взаимодействия, которые исполняются SUT к зависимым объектам) 
* stub для: ITurnstilesRepository (вызовы и взаимодействия,  которые исполняются SUT к зависимым объектам, чтобы запросить и получить  данные) 

4. UsersServiceTests
* паттерн DataBuilder (лежит в папке ArrangeHelpers) -- для генерации объектов для тестов 
* mock для: ICheckPermissionsService (вызовы и взаимодействия, которые исполняются SUT к зависимым объектам) 
* stub для: IUsersRepository (вызовы и взаимодействия,  которые исполняются SUT к зависимым объектам, чтобы запросить и получить  данные) 
* при этом не используется атрибут AutoMoqData, вместо него прямое создание Mock 



5. CheckPermissionsServiceTests:
* паттерн Fabric (Object Mother) (лежит в папке ArrangeHelpers) и атрибут InlineData -- для генерации объектов для тестов 
* stub для IUsersRepository (вызовы и взаимодействия,  которые исполняются SUT к зависимым объектам, чтобы запросить и получить данные) 
* при этом не используется атрибут AutoMoqData, вместо него прямое создание Mock 

6. LiftsSlopesServiceTests
* паттерн Fabric (Object Mother) (лежит в папке ArrangeHelpers) -- для генерации объектов для тестов 
* fixture с помощью AutoMoqData (собственный атрибут AutoMoqDataAttribute, лежит в папке ArrangeHelpers) (по сути Dummy)-- для генерации объектов для тестов 
* код не изолируется от зависимостей внутри unit-а (то есть В отличие от других тестов, НЕ использутеся mock: ICheckPermissionService) 
* код изолируется от shared-зависимостей. Для этого используются stub-ы: ILiftsRepository, ISlopesRepository, ILiftsSlopesRepository, IUsersRepository (вызовы и взаимодействия,  которые исполняются SUT к зависимым объектам, чтобы запросить и получить  данные) 

## 2. Unit-Tests for AccessToDB (AccessToDB.Tests)

LiftsSlopesRepositoryTests выполнен в Лондонский стиле -- изоляция от кода репозиториев ILiftsRepository и ISlopesRepository путем создания их Fake-ов, все остальные -- в классическом.

Во всех тестовых классах применяется Fixture и TearDown: [Share setup and cleanup code: Constructor and Dispose](//https://xunit.net/docs/shared-context). When to use: when you want a clean test context for every test (sharing the setup and cleanup code, without sharing the object instance).

Также используется упомянутый выше AutoMoqData.