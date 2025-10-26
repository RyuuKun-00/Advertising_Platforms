@echo off

echo Assembling and running a project in docker...

docker build -t advertising-platforms ./AdvertisingPlatforms

docker run -d --name AdvertisingPlatforms_Container -p 5000:8080 advertising-platforms

@start http://localhost:5000


echo The project is assembled and launched!

@pause