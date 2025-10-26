FROM ubuntu:jammy-20230605 AS add-apt-repositories

ENV PG_VERSION=16

RUN apt-get update \
 && DEBIAN_FRONTEND=noninteractive apt-get install -y wget gnupg \
 && wget --quiet -O - http://repo.postgrespro.ru/keys/GPG-KEY-POSTGRESPRO | apt-key add - \
 && echo deb http://repo.postgrespro.ru/1c-${PG_VERSION}/ubuntu jammy main >> /etc/apt/sources.list

FROM ubuntu:jammy-20230605

ENV PG_VERSION=16 \
    LANG=ru_RU.UTF-8 \
    LANGUAGE=ru_RU.UTF-8 \
    PGDATA=/var/lib/postgresql/data \
    DEBIAN_FRONTEND=noninteractive

ENV PG_BINDIR=/opt/pgpro/1c-${PG_VERSION}/bin

COPY --from=add-apt-repositories /etc/apt/trusted.gpg /etc/apt/trusted.gpg
COPY --from=add-apt-repositories /etc/apt/sources.list /etc/apt/sources.list

RUN set -eux; \
        groupadd -r postgres --gid=999; \
        useradd -r -g postgres --uid=999 --home-dir=/var/lib/postgresql --shell=/bin/bash postgres; \
        mkdir -p /var/lib/postgresql; \
        chown -R postgres:postgres /var/lib/postgresql

RUN apt-get update \
 && apt-get install -y acl sudo locales gosu \
 && locale-gen ${LANG} \
 && locale-gen en_US.UTF-8 \
 && update-locale ${LANG} \
 && dpkg-reconfigure --frontend=noninteractive locales \
 && apt-get install -y postgrespro-1c-${PG_VERSION}-server \
 && ${PG_BINDIR}/pg-wrapper links update \
 && postgres --version \
 && rm -rf /var/lib/apt/lists/*

RUN mkdir /docker-entrypoint-initdb.d \
 && mkdir -p /var/run/postgresql \
 && chown -R postgres:postgres /var/run/postgresql \
 && chmod 3777 /var/run/postgresql \
 && mkdir -p "$PGDATA" && chown -R postgres:postgres "$PGDATA" && chmod 1777 "$PGDATA"

COPY docker-entrypoint.sh docker-ensure-initdb.sh /usr/local/bin/

RUN ln -sT docker-ensure-initdb.sh /usr/local/bin/docker-enforce-initdb.sh

ENTRYPOINT ["docker-entrypoint.sh"]

STOPSIGNAL SIGINT

EXPOSE 5432
CMD ["postgres"]
