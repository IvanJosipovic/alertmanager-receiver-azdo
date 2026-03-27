ARG BUILDPLATFORM=linux/amd64

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0-alpine@sha256:a65df8d9ad0661a1a785a4f26188b3a2826540d448df317ac69cfb6e801e1592 AS build
RUN apk add build-base zlib-dev
WORKDIR /src
COPY Alertmanager.Receiver.AzureDevOps.csproj .
RUN dotnet restore "Alertmanager.Receiver.AzureDevOps.csproj"
COPY . .
RUN dotnet build "Alertmanager.Receiver.AzureDevOps.csproj" -c Release -o /app/build

FROM build AS publish
ARG VERSION=0.0.1
RUN dotnet publish "Alertmanager.Receiver.AzureDevOps.csproj" -c Release --use-current-runtime -o /app/publish /p:Version=${VERSION}

FROM mcr.microsoft.com/dotnet/runtime-deps:10.0-alpine@sha256:9ec585b84e539eff25a0c925137263ecb900db78d47e02a3b9ceb5d5009551fb AS base
WORKDIR /app
EXPOSE 8080

LABEL org.opencontainers.image.description="Prometheus Alertmanager Receiver for Azure DevOps"

# renovate: datasource=repology depName=alpine_3_23/ca-certificates-bundle versioning=loose
ENV CA_CERTIFICATES_BUNDLES_VERSION="20251003-r0"

# renovate: datasource=repology depName=alpine_3_23/busybox versioning=loose
ENV BUSYBOX_VERSION="1.37.0-r30"

# renovate: datasource=repology depName=alpine_3_23/busybox-binsh versioning=loose
ENV BUSYBOX_BINSH_VERSION="1.37.0-r30"

# renovate: datasource=repology depName=alpine_3_23/libcrypto3 versioning=loose
ENV LIBCRYPTO3_VERSION="3.5.5-r0"

# renovate: datasource=repology depName=alpine_3_23/ssl_client versioning=loose
ENV SSL_VERSION="1.37.0-r30"

# renovate: datasource=repology depName=alpine_3_23/libgcc versioning=loose
ENV LIBGCC_VERSION="15.2.0-r2"

# renovate: datasource=repology depName=alpine_3_23/libssl3 versioning=loose
ENV LIBSSL3_VERSION="3.5.5-r0"

# renovate: datasource=repology depName=alpine_3_23/libstdc++ versioning=loose
ENV LIBSTDC_VERSION="15.2.0-r2"

# renovate: datasource=repology depName=alpine_3_23/zlib versioning=loose
ENV LIBZLIB_VERSION="1.3.2-r0"

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
        libstdc++>$LIBSTDC_VERSION\
        zlib>$LIBZLIB_VERSION

USER app

COPY --from=publish /app/publish .
ENTRYPOINT ["./Alertmanager.Receiver.AzureDevOps"]
