FROM ubuntu:22.04 AS add-apt-repositories

ENV DEBIAN_FRONTEND=noninteractive

RUN apt-get update; \
 apt-get install -y software-properties-common gnupg; \
 add-apt-repository -n -y "deb http://archive.ubuntu.com/ubuntu/ focal universe"

COPY server64_*.tar.gz ./
RUN tar -xzvf server64_*.tar.gz

FROM fredblgr/ubuntu-novnc:22.04

ARG ONEC_VERSION=8.3.22.2411
ENV ONEC_DATA=/home/usr1cv8/srvinfo
ENV LANG=ru_RU.UTF-8
ENV DEBIAN_FRONTEND=noninteractive

COPY --from=add-apt-repositories /etc/apt/sources.list /etc/apt/sources.list
COPY --from=add-apt-repositories /etc/apt/sources.list.d /etc/apt/sources.list.d
COPY --from=add-apt-repositories ./setup-full-* ./

RUN set -xe; \
  apt-get update; \
  apt-get install -y --no-install-recommends locales; \
  locale-gen ${LANG}; \
  dpkg-reconfigure locales


RUN set -xe; \
  ./setup-full-$ONEC_VERSION-x86_64.run --mode unattended  --enable-components client_thin_fib,server_admin  --installer-language en; \
  rm -rf ./setup-full-*

COPY config/conf.cfg /opt/1cv8/x86_64/conf/
COPY config/nethasp.ini /opt/1cv8/x86_64/conf/
 
