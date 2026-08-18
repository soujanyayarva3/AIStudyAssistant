FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY AIStudyAssistant.API8/AIStudyAssistant.API8.csproj AIStudyAssistant.API8/
COPY AIStudyAssistant.Application/AIStudyAssistant.Application.csproj AIStudyAssistant.Application/
COPY AIStudyAssistant.Domain/AIStudyAssistant.Domain.csproj AIStudyAssistant.Domain/
COPY AIStudyAssistant.Infrastructure/AIStudyAssistant.Infrastructure.csproj AIStudyAssistant.Infrastructure/

RUN dotnet restore AIStudyAssistant.API8/AIStudyAssistant.API8.csproj

COPY AIStudyAssistant.API8/ AIStudyAssistant.API8/
COPY AIStudyAssistant.Application/ AIStudyAssistant.Application/
COPY AIStudyAssistant.Domain/ AIStudyAssistant.Domain/
COPY AIStudyAssistant.Infrastructure/ AIStudyAssistant.Infrastructure/

RUN dotnet publish AIStudyAssistant.API8/AIStudyAssistant.API8.csproj \
    -c Release \
    -o /app/publish \
    --no-restore


FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "AIStudyAssistant.API8.dll"]