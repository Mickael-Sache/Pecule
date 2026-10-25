FROM node:24-alpine AS front
WORKDIR /front
COPY front/package.json front/package-lock.json ./
RUN npm ci
COPY front/ ./
RUN npx ng build --configuration production

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY global.json Directory.Build.props Directory.Packages.props .editorconfig ./
COPY src/ src/
RUN dotnet restore src/Pecule.Host/Pecule.Host.csproj
RUN dotnet publish src/Pecule.Host/Pecule.Host.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app .
COPY --from=front /front/dist/front/browser ./wwwroot
USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "Pecule.Host.dll"]