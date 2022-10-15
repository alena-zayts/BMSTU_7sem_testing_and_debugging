# BMSTU_7sem_testing_and_debugging
7th sem BMSTU, Testing and debugging

# Лабораторная 1

## Запуск тестов

На WIndows: запустите файл run_unit_tests.bat, который лежит в корне склонированного репозитория. Скрипт прогонит тесты как для BL, так и для AccessToDB, сгенерирует единый allure-отчет и откроет его в браузере.

Так как в проекте в качестве БД используется Tarantool, скрипт в процессе работы поднимает докер-контейнер с тарантулом. Также требуется, чтобы был установлен allure (можно установить, например, так: npm install -g allure-commandline --save-dev)

Также все тесты прогоняются при push/pull в репозиторий (см. Actions).

Если что-то идет не так, то скрипт лучше запустить вот так: cmd /k cmd /c run_unit_tests.bat (так хоть скрипт не закроется)

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