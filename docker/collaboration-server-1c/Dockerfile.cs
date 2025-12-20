FROM bellsoft/liberica-openjdk-debian:11

RUN apt-get update && apt-get -y install sudo curl nano gawk mc; \
	rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY 1c_cs ./

# Install specified components only
RUN ./1ce-installer-cli install 1c-cs --components 1c-cs-server-small --ignore-signature-warnings; \
    rm -rf /app    

# Make ring utility available in PATH
ENV PATH /opt/1C/1CE/components/1c-enterprise-ring-0.19.5+12-x86_64:/usr/local/sbin:/usr/local/bin:/usr/sbin:/usr/bin:/sbin:/bin

RUN groupadd -r cs && useradd -r -s /bin/false -g cs cs; \
	mkdir -p /var/cs/cs_instance && chown -R cs:cs /opt/1C /var/cs

RUN ring cs instance create --dir /var/cs/cs_instance --owner cs 

USER cs

# CS settings
# hazelcast
RUN ring cs --instance cs_instance hazelcast set-params --group-name 1ce-cs --group-password cs-pass --addresses hazelcast\
    # elasticsearch
    && ring cs --instance cs_instance elasticsearch set-params --addresses elasticsearch:9300\
    # postgres
    && ring cs --instance cs_instance jdbc pools --name common set-params --url jdbc:postgresql://db:5432/cs_db?currentSchema=public\
    && ring cs --instance cs_instance jdbc pools --name common set-params --username postgres\
    && ring cs --instance cs_instance jdbc pools --name common set-params --password postgres\
    && ring cs --instance cs_instance jdbc pools --name privileged set-params --url jdbc:postgresql://db:5432/cs_db?currentSchema=public\
    && ring cs --instance cs_instance jdbc pools --name privileged set-params --username postgres\
    && ring cs --instance cs_instance jdbc pools --name privileged set-params --password postgres\
    # WebSocket
    && ring cs --instance cs_instance websocket set-params --hostname 0.0.0.0 \
    && ring cs --instance cs_instance websocket set-params --port 9094

EXPOSE 9094
EXPOSE 8087

ENTRYPOINT ["/opt/1C/1CE/components/1c-cs-server-small-27.0.37-x86_64/bin/launcher", "start", "--instance", "/var/cs/cs_instance"]
