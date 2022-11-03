#echo ${PWD}
#echo %~dp0
#cmd /k cmd /c run_unit_tests.bat


function jumpto
{
    label=$1
    cmd=$(sed -n "/$label:/{:a;n;p;ba};" $0 | grep -v ':$')
    eval "$cmd"
    exit
}
start=${1:-"start"}
jumpto $start



start:
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
BLUnitTests:
echo "[101;93m Run Tests for BL [0m"
cd SkiResort/BL.Tests && /home/alena/.dotnet/dotnet test --logger:trx
if ! [ $? -eq 0 ]
then
  echo "[101;93m FAILED. Go to end_tests[0m"
  cd ../..
  jumpto end_tests
fi
cd ../..
echo "[101;93m Tests for BL finished [0m"



# AccessToDB Unit Tests
AccessToDBUnitTests:
echo "[101;93m Raising DB or loading test data [0m"
docker-compose --env-file .docker-env -f ${PWD}/SkiResort/tarantool/docker-compose-console.yml up -d
echo "[101;93m Docker is running [0m"


echo "[101;93m Run Tests for AccessToDB [0m"
cd SkiResort/AccessToDB.Tests && /home/alena/.dotnet/dotnet test --logger:trx
if ! [ $? -eq 0 ]
then
  echo "[101;93m FAILED. Go to end_tests[0m"
  cd ../..
  jumpto end_tests
fi
cd ../..
echo "[101;93m Tests for AccessToDB finished [0m"


echo "[101;93m Stopping DB or rolling bask test data [0m"
docker-compose --env-file .docker-env -f ${PWD}/SkiResort/tarantool/docker-compose-console.yml down
echo "[101;93m Docker stopped [0m"




# Integration Tests
IntegrationTests:
echo "[101;93m Raising DB or loading test data [0m"
docker-compose --env-file .docker-env -f ${PWD}/SkiResort/tarantool/docker-compose-console.yml up -d
docker exec tarantool tarantool /usr/local/share/tarantool/app.init.lua
echo "[101;93m Docker is running [0m"


echo "[101;93m Run Integration Tests[0m"
cd SkiResort/IntegrationTests && /home/alena/.dotnet/dotnet test --logger:trx
if ! [ $? -eq 0 ]
then
  echo "[101;93m FAILED. Go to end_tests[0m"
  cd ../..
  jumpto end_tests
fi
cd ../..
echo "[101;93m Integration Tests finished [0m"


echo "[101;93m Stopping DB or rolling bask test data [0m"
docker-compose --env-file .docker-env -f ${PWD}/SkiResort/tarantool/docker-compose-console.yml down
echo "[101;93m Docker stopped [0m"




# E2E Tests
E2ETests:
echo "[101;93m Raising DB or loading test data [0m"
docker-compose --env-file .docker-env -f ${PWD}/SkiResort/tarantool/docker-compose-console.yml up -d
echo "[101;93m Docker is running [0m"


echo "[101;93m Run E2ET Tests[0m"
cd SkiResort/E2ETests && /home/alena/.dotnet/dotnet test --logger:trx
if ! [ $? -eq 0 ]
then
  echo "[101;93m FAILED. Go to end_tests[0m"
  cd ../..
  jumpto end_tests
fi
cd ../..
echo "[101;93m E2E Tests finished [0m"


echo "[101;93m Stopping DB or rolling bask test data [0m"
docker-compose --env-file .docker-env -f ${PWD}/SkiResort/tarantool/docker-compose-console.yml down
echo "[101;93m Docker stopped [0m"





# Report
end_tests:
echo "[101;93m Stopping DB or rolling bask test data [0m"
docker-compose --env-file .docker-env -f ${PWD}/SkiResort/tarantool/docker-compose-console.yml down
echo "[101;93m Docker stopped [0m"
Report:
echo "[101;93m Generating report [0m"
allure generate ${PWD}/SkiResort/BL.Tests/bin/Debug/net6.0/allure-results ${PWD}/SkiResort/AccessToDB.Tests/bin/Debug/net6.0/allure-results ${PWD}/SkiResort/IntegrationTests/bin/Debug/net6.0/allure-results ${PWD}/SkiResort/E2ETests/bin/Debug/net6.0/allure-results --clean -o ${PWD}/allure-report-for-all-tests


echo "[101;93m Showing report [0m"
allure open ${PWD}/allure-report-for-all-tests
sleep 3












