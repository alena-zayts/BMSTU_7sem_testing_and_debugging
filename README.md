# BMSTU_7sem_testing_and_debugging
7th sem BMSTU, Testing and debugging

# Лабораторная 1

Во всех тестах явно прописана структура Arrange-Act-Assert (для себя и для наглядности). По заданию нужно было попробовать много чего, поэтому далее пропишу, что где используется.


## Unit-Tests for BuisinessLogic (BL.Tests)

LiftsSlopesServiceTests выполнен в классическом стиле (изоляция тестов), все остальные -- в Лондонском (изоляция кода от зависимостей)

1. LiftsServiceTest
* fixture с помощью AutoMoqData (собственный атрибут AutoMoqDataAttribute, лежит в папке ArrangeHelpers) (по сути Dummy)-- для генерации объектов для тестов
* mock для: ICheckPermissionsService (вызовы и взаимодействия, которые исполняются SUT к зависимым объектам)
* stub для: ILiftsRepository, ILiftsSlopesRepository (вызовы и взаимодействия,  которые исполняются SUT к зависимым объектам, чтобы запросить и получить  данные)

1. SlopesServiceTest
* fixture с помощью AutoMoqData (собственный атрибут AutoMoqDataAttribute, лежит в папке ArrangeHelpers) (по сути Dummy)-- для генерации объектов для тестов
* mock для: ICheckPermissionsService (вызовы и взаимодействия, которые исполняются SUT к зависимым объектам)
* stub для: ISlopesRepository, ILiftsSlopesRepository (вызовы и взаимодействия,  которые исполняются SUT к зависимым объектам, чтобы запросить и получить  данные)

1. TurnstilesServiceTest
* fixture с помощью AutoMoqData (собственный атрибут AutoMoqDataAttribute, лежит в папке ArrangeHelpers) (по сути Dummy)-- для генерации объектов для тестов
* mock для: ICheckPermissionsService (вызовы и взаимодействия, которые исполняются SUT к зависимым объектам)
* stub для: ITurnstilesRepository (вызовы и взаимодействия,  которые исполняются SUT к зависимым объектам, чтобы запросить и получить  данные)

1. UsersServiceTests
* паттерн DataBuilder (лежит в папке ArrangeHelpers) -- для генерации объектов для тестов
* mock для: ICheckPermissionsService (вызовы и взаимодействия, которые исполняются SUT к зависимым объектам)
* stub для: IUsersRepository (вызовы и взаимодействия,  которые исполняются SUT к зависимым объектам, чтобы запросить и получить  данные)
* при этом не используется атрибут AutoMoqData, вместо него прямое создание Mock



1. CheckPermissionsServiceTests:
* паттерн Fabric (Object Mother) (лежит в папке ArrangeHelpers) и атрибут InlineData -- для генерации объектов для тестов
* stub для IUsersRepository (вызовы и взаимодействия,  которые исполняются SUT к зависимым объектам, чтобы запросить и получить данные)
* при этом не используется атрибут AutoMoqData, вместо него прямое создание Mock

1. LiftsSlopesServiceTests
* паттерн Fabric (Object Mother) (лежит в папке ArrangeHelpers) -- для генерации объектов для тестов
* fixture с помощью AutoMoqData (собственный атрибут AutoMoqDataAttribute, лежит в папке ArrangeHelpers) (по сути Dummy)-- для генерации объектов для тестов
* код не изолируется от зависимостей внутри unit-а (то есть В отличие от других тестов, НЕ использутеся mock: ICheckPermissionService)
* код изолируется от shared-зависимостей. Для этого используются stub-ы: ILiftsRepository, ISlopesRepository, ILiftsSlopesRepository, IUsersRepository (вызовы и взаимодействия,  которые исполняются SUT к зависимым объектам, чтобы запросить и получить  данные)

