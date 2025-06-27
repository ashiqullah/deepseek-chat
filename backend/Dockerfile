# Use the official ASP.NET Core runtime as a parent image
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the SDK image to build the application
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG TARGETARCH
WORKDIR /src
COPY ["DeepSeekChatApi.csproj", "./"]
RUN dotnet restore "DeepSeekChatApi.csproj" -a $TARGETARCH
COPY . .
RUN dotnet build "DeepSeekChatApi.csproj" -c Release -o /app/build -a $TARGETARCH

FROM build AS publish
RUN dotnet publish "DeepSeekChatApi.csproj" -c Release -o /app/publish /p:UseAppHost=false -a $TARGETARCH

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DeepSeekChatApi.dll"] 