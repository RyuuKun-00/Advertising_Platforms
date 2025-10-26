@echo off

echo Assembling the project...

cd ./AdvertisingPlatforms

dotnet build -c Release

cd ./AdvertisingPlatforms/bin/Release/net8.0/

start AdvertisingPlatforms.exe

timeout 5

@start http://localhost:5000

cls

echo The project is assembled and launched!

@pause