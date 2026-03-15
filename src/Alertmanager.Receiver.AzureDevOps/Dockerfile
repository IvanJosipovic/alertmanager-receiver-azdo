ARG BUILDPLATFORM=linux/amd64

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
RUN apk add build-base zlib-dev
WORKDIR /src
COPY Alertmanager.Receiver.AzureDevOps.csproj .
RUN dotnet restore "Alertmanager.Receiver.AzureDevOps.csproj"
COPY . .
RUN dotnet build "Alertmanager.Receiver.AzureDevOps.csproj" -c Release -o /app/build

FROM build AS publish
ARG VERSION=0.0.1
RUN dotnet publish "Alertmanager.Receiver.AzureDevOps.csproj" -c Release --use-current-runtime -o /app/publish /p:Version=${VERSION}

FROM mcr.microsoft.com/dotnet/runtime-deps:10.0-alpine AS base
WORKDIR /app
EXPOSE 8080

LABEL org.opencontainers.image.description="Prometheus Alertmanager Receiver for Azure DevOps"

# renovate: datasource=repology depName=alpine_3_23/ca-certificates-bundle versioning=loose
ENV CA_CERTIFICATES_BUNDLES_VERSION="20250911-r0"

# renovate: datasource=repology depName=alpine_3_23/busybox versioning=loose
ENV BUSYBOX_VERSION="1.37.0-r14"

# renovate: datasource=repology depName=alpine_3_23/busybox-binsh versioning=loose
ENV BUSYBOX_BINSH_VERSION="1.37.0-r14"

# renovate: datasource=repology depName=alpine_3_23/libcrypto3 versioning=loose
ENV LIBCRYPTO3_VERSION="3.3.6-r0"

# renovate: datasource=repology depName=alpine_3_23/ssl_client versioning=loose
ENV SSL_VERSION="1.37.0-r14"

# renovate: datasource=repology depName=alpine_3_23/libgcc versioning=loose
ENV LIBGCC_VERSION="14.2.0-r4"

# renovate: datasource=repology depName=alpine_3_23/libssl3 versioning=loose
ENV LIBSSL3_VERSION="3.3.6-r0"

# renovate: datasource=repology depName=alpine_3_23/libstdc++ versioning=loose
ENV LIBSTDC_VERSION="14.2.0-r4"

RUN apk add --upgrade --no-cache \
        ca-certificates-bundle>$CA_CERTIFICATES_BUNDLES_VERSION \
        busybox>$BUSYBOX_BINSH_VERSION \
        busybox-binsh>$BUSYBOX_BINSH_VERSION \
        libcrypto3>$LIBCRYPTO3_VERSION \
        ssl_client>$SSL_VERSION \
        \
        # .NET dependencies
        libgcc>$LIBGCC_VERSION \
        libssl3>$LIBSSL3_VERSION \
        libstdc++>$LIBSTDC_VERSION

USER app

COPY --from=publish /app/publish .
ENTRYPOINT ["./Alertmanager.Receiver.AzureDevOps"]
