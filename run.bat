::echo %cd%
::echo %~dp0
::cmd /k cmd /c run.bat

@echo off

:: delete old results
rd /s /q %cd%\SkiResort\BL.Tests\bin\Debug\net6.0\allure-results
rd /s /q %cd%\SkiResort\AccessToDB.Tests\bin\Debug\net6.0\allure-results
rd /s /q %cd%\allure-report-for-unit-tests

echo [101;93m Messages from me will be coloured like this [0m


:: BL Tests
echo [101;93m Run Tests for BL [0m 
cd SkiResort/BL.Tests && call dotnet test --logger:trx
cd ../..
echo [101;93m Tests for BL finished [0m



:: AccessToDB Tests
echo [101;93m Raising docker (it's used to run Tarantool, and Tarantool is used in AccessToDB) [0m
bash raise_docker.sh

echo [101;93m Run Tests for AccessToDB [0m
cd SkiResort/AccessToDB.Tests && call dotnet test --logger:trx
cd ../..
echo [101;93m Tests for AccessToDB finished [0m


echo [101;93m Stopping docker [0m
bash stop_docker.sh



:: Report
echo [101;93m Generating report [0m
call allure generate %cd%\SkiResort\BL.Tests\bin\Debug\net6.0\allure-results %cd%\SkiResort\AccessToDB.Tests\bin\Debug\net6.0\allure-results --clean -o %cd%\allure-report-for-unit-tests


echo [101;93m Showing report [0m
call allure open %cd%\allure-report-for-unit-tests
pause




