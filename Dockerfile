FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /KafkaBro

COPY . .

RUN dotnet restore "/KafkaBro/KafkaBro.csproj"

RUN dotnet publish -c release -o /out


FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /out

COPY --from=build /out .

ENTRYPOINT ["dotnet", "KafkaBro.dll"]
