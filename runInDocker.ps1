dotnet publish
cd bin/Debug/netcoreapp1.1/publish/
docker kill laos1
docker rm laos1
docker run -d -p 4000:80 -v C:\Projects\dotNet\LaoS\LaoS\views:/LaoS/views -v C:\Projects\dotNet\LaoS\LaoS\wwwroot:/LaoS/wwwroot --name laos1 laos

cd ../../../../