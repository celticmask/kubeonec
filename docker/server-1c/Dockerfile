FROM ubuntu:22.04 AS add-apt-repositories

ENV DEBIAN_FRONTEND=noninteractive

RUN apt-get update; \
 apt-get install -y software-properties-common gnupg; \
 add-apt-repository -n -y "deb http://archive.ubuntu.com/ubuntu/ focal universe"

COPY server64_*.tar.gz ./
RUN tar -xzvf server64_*.tar.gz

# Second stage
FROM ubuntu:22.04

ARG ONEC_VERSION=8.3.27.1989
ENV ONEC_DATA=/home/usr1cv8/srvinfo
ENV LANG=ru_RU.UTF-8
ENV DEBIAN_FRONTEND=noninteractive
ENV TZ=Etc/UTC

COPY --from=add-apt-repositories /etc/apt/sources.list /etc/apt/sources.list
COPY --from=add-apt-repositories /etc/apt/sources.list.d /etc/apt/sources.list.d
COPY --from=add-apt-repositories ./$ONEC_VERSION /opt/1cv8/x86_64/$ONEC_VERSION

RUN set -xe; \
  apt-get update; \
  apt-get install -y --no-install-recommends locales; \
  locale-gen ${LANG}; \
  dpkg-reconfigure locales

RUN set -xe; \
  echo "ttf-mscorefonts-installer msttcorefonts/accepted-mscorefonts-eula select true" | debconf-set-selections; \
  apt-get update; \
  apt-get install -y --no-install-recommends \
    libenchant1c2a \
    imagemagick \
    unixodbc \
    libgsf-1-114 \
    ttf-mscorefonts-installer \
    fontconfig \
    libgtk-3-0 \
    libharfbuzz-icu0 \
    libgstreamer1.0-0 \
    libgstreamer-plugins-base1.0-0 \
    gstreamer1.0-plugins-good \
    gstreamer1.0-plugins-bad \
    libsecret-1-0 \
    libsoup2.4-1 \
    libgl1 \
    libegl1 \
    libxrender1 \
    libxfixes3 \
    libxslt1.1 \
    geoclue-2.0 \
    tzdata \
    libpng*; \
  rm -rf /var/lib/apt/lists/*

COPY ./configs /opt/1cv8/conf
COPY entrypoint.sh ./entrypoint.sh

RUN set -xe; \
    ln -s /opt/1cv8/x86_64/$ONEC_VERSION/ragent /usr/local/bin/ragent; \
    ln -s /opt/1cv8/x86_64/$ONEC_VERSION/ras /usr/local/bin/ras; \
    chmod +x ./entrypoint.sh
    
RUN groupadd -r usr1cv8 && useradd -r -g usr1cv8 -m -s /bin/bash usr1cv8
USER usr1cv8
#RUN mkdir /home/usr1cv8/srvinfo /var/log/kubeonec

EXPOSE 1540-1541 1545 1550 1560-1591

ENTRYPOINT [ "./entrypoint.sh" ]

# (optional)
#CMD [ "-debug", "-http" ]
