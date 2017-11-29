FROM microsoft/dotnet

WORKDIR /simplehttpserver

# copy project.json and restore as distinct layers
COPY SimpleHttpServer.csproj .
RUN dotnet restore SimpleHttpServer.csproj

# Make port 80 available to the world outside this container
EXPOSE 80

# copy and build everything else
COPY . .
RUN dotnet publish -c Release -o out
ENTRYPOINT ["dotnet", "out/SimpleHttpServer.dll"]





#docker build -t simplehttpserver .
#docker run --rm simplehttpserver
#https://docs.docker.com/get-started/part2/#dockerfile
#https://hub.docker.com/r/microsoft/dotnet/