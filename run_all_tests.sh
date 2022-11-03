#echo ${PWD}
#echo %~dp0
#cmd /k cmd /c run_unit_tests.bat


# delete old results
rm -r -f ${PWD}/SkiResort/BL.Tests/bin/Debug/net6.0/allure-results
rm -r -f ${PWD}/SkiResort/AccessToDB.Tests/bin/Debug/net6.0/allure-results
rm -r -f ${PWD}/allure-report-for-unit-tests
rm -r -f ${PWD}/SkiResort/IntegrationTests/bin/Debug/net6.0/allure-results
rm -r -f ${PWD}/allure-report-for-integration-tests
rm -r -f ${PWD}/SkiResort/E2ETests/bin/Debug/net6.0/allure-results
rm -r -f ${PWD}/allure-report-for-e2e-tests
rm -r -f ${PWD}/allure-report-for-all-tests


echo "[101;93m Messages from me will be coloured like this [0m"






# BL Unit Tests
echo "[101;93m Run Tests for BL [0m"
cd SkiResort/BL.Tests && /home/alena/.dotnet/dotnet test --logger:trx
cd ../..
echo "[101;93m Tests for BL finished [0m"



# AccessToDB Unit Tests
echo "[101;93m Raising docker (it's used to run Tarantool, and Tarantool is used in AccessToDB) [0m"
docker-compose --env-file .docker-env -f ${PWD}/SkiResort/tarantool/docker-compose-console.yml up -d
echo "[101;93m Docker started [0m"


echo "[101;93m Run Tests for AccessToDB [0m"
cd SkiResort/AccessToDB.Tests && /home/alena/.dotnet/dotnet test --logger:trx
cd ../..
echo "[101;93m Tests for AccessToDB finished [0m"


echo "[101;93m Stopping docker [0m"
docker-compose --env-file .docker-env -f ${PWD}/SkiResort/tarantool/docker-compose-console.yml down
echo "[101;93m Docker stopped [0m"




# Integration Tests
echo "[101;93m Raising docker (it's used to run Tarantool, and Tarantool is used in AccessToDB) [0m"
docker-compose --env-file .docker-env -f ${PWD}/SkiResort/tarantool/docker-compose-console.yml up -d
docker exec skiresort tarantool /usr/local/share/tarantool/app.init.lua
echo "[101;93m Docker started [0m"


echo "[101;93m Run Integration Tests[0m"
cd SkiResort/IntegrationTests && /home/alena/.dotnet/dotnet test --logger:trx
cd ../..
echo "[101;93m Integration Tests finished [0m"


echo "[101;93m Stopping docker [0m"
docker-compose --env-file .docker-env -f ${PWD}/SkiResort/tarantool/docker-compose-console.yml down
echo "[101;93m Docker stopped [0m"




# E2E Tests
echo "[101;93m Raising docker (it's used to run Tarantool, and Tarantool is used in AccessToDB) [0m"
docker-compose --env-file .docker-env -f ${PWD}/SkiResort/tarantool/docker-compose-console.yml up -d
echo "[101;93m Docker started [0m"


echo "[101;93m Run E2ET Tests[0m"
cd SkiResort/E2ETests && /home/alena/.dotnet/dotnet test --logger:trx
cd ../..
echo "[101;93m E2E Tests finished [0m"


echo "[101;93m Stopping docker [0m"
docker-compose --env-file .docker-env -f ${PWD}/SkiResort/tarantool/docker-compose-console.yml down
echo "[101;93m Docker stopped [0m"





# Report
echo "[101;93m Generating report [0m"
allure generate ${PWD}/SkiResort/BL.Tests/bin/Debug/net6.0/allure-results ${PWD}/SkiResort/AccessToDB.Tests/bin/Debug/net6.0/allure-results ${PWD}/SkiResort/IntegrationTests/bin/Debug/net6.0/allure-results ${PWD}/SkiResort/E2ETests/bin/Debug/net6.0/allure-results --clean -o ${PWD}/allure-report-for-all-tests


echo "[101;93m Showing report [0m"
allure open ${PWD}/allure-report-for-all-tests
sleep 3












