# ===== STAGE 1: Build =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution và các .csproj (để cache restore)
COPY *.sln .
COPY CourseScheduleService.Domain/*.csproj ./CourseScheduleService.Domain/
COPY CourseScheduleService.Application/*.csproj ./CourseScheduleService.Application/
COPY CourseScheduleService.Infrastructure/*.csproj ./CourseScheduleService.Infrastructure/
COPY CourseScheduleService.API/*.csproj ./CourseScheduleService.API/

# Restore packages
RUN dotnet restore

# Copy toàn bộ source và publish API project (tự động lấy các project khác)
COPY . .
WORKDIR /src/CourseScheduleService.API
RUN dotnet publish -c Release -o /app/publish

# ===== STAGE 2: Runtime =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 5000
ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "CourseScheduleService.API.dll"]