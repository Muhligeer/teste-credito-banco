FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

COPY . .

RUN dotnet restore "TesteCreditoBanco.sln"

FROM build-env AS publish-customers
WORKDIR /app/src/Customers.Api
RUN dotnet publish -c Release -o /app/out --no-restore

FROM build-env AS publish-creditproposals
WORKDIR /app/src/CreditProposals.Api
RUN dotnet publish -c Release -o /app/out --no-restore

FROM build-env AS publish-creditcards
WORKDIR /app/src/CreditCards.Api
RUN dotnet publish -c Release -o /app/out --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final-customers
WORKDIR /app
COPY --from=publish-customers /app/out .
ENTRYPOINT ["dotnet", "Customers.Api.dll"]

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final-creditproposals
WORKDIR /app
COPY --from=publish-creditproposals /app/out .
ENTRYPOINT ["dotnet", "CreditProposals.Api.dll"]

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final-creditcards
WORKDIR /app
COPY --from=publish-creditcards /app/out .
ENTRYPOINT ["dotnet", "CreditCards.Api.dll"]