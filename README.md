# BMSTU_7sem_testing_and_debugging
7th sem BMSTU, Testing and debugging

# Лабораторная 1

Во всех тестах явно прописана структура Arrange-Act-Assert (для себя и для наглядности).


## Unit-Tests for BuisinessLogic (BL)

LiftsSlopesServiceTests выполнен в классическом стиле (изоляция тестов), все остальные -- в Лондонском (изоляция кода от зависимостей)

Ососбенности Test-Suite-ов:
1. CheckPermissionsServiceTests:
1.1. stub для IUsersRepository (вызовы и взаимодействия,  которые исполняются SUT к зависимым объектам, чтобы запросить и получить данные)
1.2. не используется AutoMoqData, вместо него прямое создание Mock и использование атрибута InlineData
1.3. Fabric (Object Mother) -- для генерации объектов для тестов