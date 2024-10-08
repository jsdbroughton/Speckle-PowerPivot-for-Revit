FROM mcr.microsoft.com/dotnet/sdk:8.0 as build-env

WORKDIR /src
COPY SpecklePowerPivotForRevit/ .
RUN dotnet restore --use-current-runtime
RUN dotnet publish --use-current-runtime --self-contained false --no-restore -o /publish

FROM mcr.microsoft.com/dotnet/runtime:8.0 as runtime
WORKDIR /publish
COPY --from=build-env /publish .
