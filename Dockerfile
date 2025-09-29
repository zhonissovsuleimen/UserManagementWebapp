FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["UserManagementWebapp/UserManagementWebapp.csproj", "UserManagementWebapp/"]
RUN dotnet restore "UserManagementWebapp/UserManagementWebapp.csproj"

COPY . .
WORKDIR "/src/UserManagementWebapp"

RUN dotnet publish "UserManagementWebapp.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "UserManagementWebapp.dll"]