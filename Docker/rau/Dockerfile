FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Possible options:
# arm64
# x64
ARG CPU_ARCH=x64

# Possible options:
# linux
# win
ARG OS=linux

RUN mkdir -p /app/ && \
    mkdir -p /config/ && \
    mkdir -p /data/ && \
    apt update -y && \
    apt install unzip -y

ADD "http://jenkins.lan.thenaterhood.com/job/Rau/job/Rau_Build/lastSuccessfulBuild/artifact/checkout/dist/zip/Rau_${OS}-${CPU_ARCH}.zip" /tmp/Rau.zip

RUN unzip /tmp/Rau.zip -d /app/ && \
    rm /tmp/Rau.zip && \
    apt remove wget unzip -y

ADD RauConfig.cs /config/RauConfig.cs

ENTRYPOINT [ "dotnet", "/app/bin/Rau.dll", "--config_file=/config/RauConfig.cs" ]
