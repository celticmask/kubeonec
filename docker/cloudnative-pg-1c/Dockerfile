# Modification from https://github.com/cloudnative-pg/postgres-containers/blob/main/Dockerfile
ARG BASE=debian:trixie-slim@sha256:9b6ccd28f4913155f35e10ecd4437347d86ebce4ecf5853b3568141468faec56
FROM $BASE AS minimal

ARG PG_MAJOR=18

COPY ./*.deb .

ENV PATH=$PATH:/usr/lib/postgresql/$PG_MAJOR/bin \
    LANG=ru_RU.UTF-8 \
    LANGUAGE=ru_RU.UTF-8

RUN apt-get update && \
    apt-get upgrade -y && \
    apt-get install -y acl locales && \
    locale-gen ${LANG} && \
    locale-gen en_US.UTF-8 && \
    update-locale ${LANG} && \
    apt-get install -y --no-install-recommends ./postgresql-common*.deb ./postgresql-client-common*.deb ca-certificates gnupg && \
    apt-get install -y --no-install-recommends -o Dpkg::::="--force-confdef" -o Dpkg::::="--force-confold" ./postgresql-common*.deb ./postgresql-client-common*.deb && \
    sed -ri 's/#(create_main_cluster) .*$/\1 = false/' /etc/postgresql-common/createcluster.conf && \
    apt-get install -y --no-install-recommends \
      libsasl2-modules libldap-common \
      -o Dpkg::::="--force-confdef" -o Dpkg::::="--force-confold" ./*.deb && \
    apt-get purge -y --auto-remove -o APT::AutoRemove::RecommendsImportant=false && \
    rm -rf /var/lib/apt/lists/* /var/cache/* /var/log/*

RUN usermod -u 26 postgres
USER 26

FROM minimal AS standard
ARG EXTENSIONS
ARG STANDARD_ADDITIONAL_POSTGRES_PACKAGES
USER root
RUN apt-get update && \
    apt-get install -y --no-install-recommends locales-all ${STANDARD_ADDITIONAL_POSTGRES_PACKAGES} ${EXTENSIONS} && \
    apt-get purge -y --auto-remove -o APT::AutoRemove::RecommendsImportant=false && \
    rm -rf /var/lib/apt/lists/* /var/cache/* /var/log/*
USER 26

FROM standard AS system
ARG BARMAN_VERSION=3.20.1

# We need to break the system packages to install barman-cloud in bookworm and later
ENV PIP_BREAK_SYSTEM_PACKAGES=1

USER root
RUN apt-get update && \
	apt-get install -y --no-install-recommends \
		# We require build-essential and python3-dev to build lz4 on arm64 since there isn't a pre-compiled wheel available
		build-essential python3-dev \
		python3-pip \
		python3-psycopg2 \
	&& \
	pip3 install --no-cache-dir --ignore-requires-python barman[cloud,azure,snappy,google,zstandard,lz4]==${BARMAN_VERSION} && \
	python3 -c "import sysconfig, compileall; compileall.compile_dir(sysconfig.get_path('stdlib'), quiet=1); compileall.compile_dir(sysconfig.get_path('purelib'), quiet=1); compileall.compile_dir(sysconfig.get_path('platlib'), quiet=1)" && \
	apt-get remove -y --purge --autoremove build-essential python3-dev && \
	apt-get purge -y --auto-remove -o APT::AutoRemove::RecommendsImportant=false && \
	rm -rf /var/lib/apt/lists/* /var/cache/* /var/log/*
USER 26
