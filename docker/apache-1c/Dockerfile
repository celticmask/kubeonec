FROM ubuntu:22.04 AS add-apt-repositories

ENV DEBIAN_FRONTEND=noninteractive

RUN apt-get update; \
 apt-get install -y software-properties-common gnupg curl; \
 add-apt-repository -n -y "deb http://archive.ubuntu.com/ubuntu/ focal universe"

ARG ONEC_VERSION=8.3.20.1710
ENV ONEC_DATA=/home/usr1cv8/srvinfo
ENV LANG=ru_RU.UTF-8
ENV DEBIAN_FRONTEND=noninteractive

COPY server64_$ONEC_VERSION.tar.gz .
RUN tar -xzvf ./server64_$ONEC_VERSION.tar.gz

RUN set -xe; \
  apt-get update; \
  apt-get install -y --no-install-recommends locales; \
  locale-gen ${LANG}; \
  dpkg-reconfigure locales

RUN set -xe; \
  ./setup-full-$ONEC_VERSION-x86_64.run --mode unattended  --enable-components ws  --installer-language en; \
  rm -rf ./setup-full-*

# Second stage 
FROM httpd:2.4

ARG ONEC_VERSION=8.3.20.1710

RUN echo "LoadModule _1cws_module modules/1c/wsap24.so\n" >> conf/httpd.conf && echo "Include conf/extra/httpd-default.conf\n" >> conf/httpd.conf

COPY --chmod=0755 --from=add-apt-repositories /opt/1cv8/x86_64/$ONEC_VERSION/* modules/1c/
