FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj trực tiếp (vì nó nằm ngay trong context)
COPY ["StudentAttendanceService.csproj", "."]
RUN dotnet restore "StudentAttendanceService.csproj"

# Copy toàn bộ source
COPY . .

# Build
RUN dotnet build "StudentAttendanceService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "StudentAttendanceService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StudentAttendanceService.dll"]