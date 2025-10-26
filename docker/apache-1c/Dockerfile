FROM ubuntu:22.04 AS add-apt-repositories

ENV DEBIAN_FRONTEND=noninteractive

RUN apt-get update; \
 apt-get install -y software-properties-common gnupg curl; \
 add-apt-repository -n -y "deb http://archive.ubuntu.com/ubuntu/ focal universe"

ARG ONEC_VERSION=8.3.22.2411
ENV ONEC_DATA=/home/usr1cv8/srvinfo
ENV LANG=ru_RU.UTF-8
ENV DEBIAN_FRONTEND=noninteractive

ADD http://localhost:43419/server64_$ONEC_VERSION.tar.gz .
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

ARG ONEC_VERSION=8.3.22.2411

RUN echo "LoadModule _1cws_module modules/1c/wsap24.so\n" >> conf/httpd.conf && echo "Include conf/extra/httpd-default.conf\n" >> conf/httpd.conf

COPY --from=add-apt-repositories /opt/1cv8/x86_64/$ONEC_VERSION/*.so modules/1c/
COPY --from=add-apt-repositories /opt/1cv8/x86_64/$ONEC_VERSION/*.so.* modules/1c/
RUN chmod a+x modules/1c/*.so

