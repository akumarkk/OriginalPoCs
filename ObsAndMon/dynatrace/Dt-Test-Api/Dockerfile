# ==========================================
# Stage 1: Download & Prepare Agent (Matching glibc runtime)
# ==========================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-agent
ARG DT_TOKEN

# Install curl and unzip via apt
RUN apt-get update && apt-get install -y --no-install-recommends \
    curl \
    unzip \
    wget \
    && rm -rf /var/lib/apt/lists/*

# Gaurav
# RUN wget -O Dynatrace-OneAgent-Linux-x86-1.337.51.20260520-164208.sh "https://kvg21893.live.dynatrace.com/api/v1/deployment/installer/agent/unix/default/latest?arch=x86" --header="Authorization: Api-Token ${DT_TOKEN}"
# RUN wget https://ca.dynatrace.com/dt-root.cert.pem ; ( echo 'Content-Type: multipart/signed; protocol="application/x-pkcs7-signature"; micalg="sha-256"; boundary="--SIGNED-INSTALLER"'; echo ; echo ; echo '----SIGNED-INSTALLER' ; cat Dynatrace-OneAgent-Linux-x86-1.337.51.20260520-164208.sh ) | openssl cms -verify -CAfile dt-root.cert.pem > /dev/null
# RUN /bin/sh Dynatrace-OneAgent-Linux-x86-1.337.51.20260520-164208.sh --set-monitoring-mode=fullstack --set-app-log-content-access=true


RUN wget -O /tmp/installer.sh -q --no-check-certificate "https://kvg21893.live.dynatrace.com/api/v1/deployment/installer/agent/unix/paas-sh/latest?Api-Token=${DT_TOKEN}&flavor=musl&include=dotnet" && sh /tmp/installer.sh && rm -f /tmp/installer.sh

# Download default glibc binary modules directly from your tenant
# RUN curl -sSLo /tmp/agent.zip "https://kvg21893.live.dynatrace.com/api/v1/deployment/installer/agent/unix/paas/latest?flavor=default&include=dotnet" \
#     -H "Authorization: Api-Token ${DT_TOKEN}" \
#     && mkdir -p /opt/dynatrace/oneagent \
#     && unzip -q /tmp/agent.zip -d /opt/dynatrace/oneagent \
#     && rm /tmp/agent.zip


# ==========================================
# Stage 2: Standard .NET SDK compilation
# ==========================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-app
WORKDIR /src
COPY ["Dt-Test-Api.csproj", "./"]
RUN dotnet restore "Dt-Test-Api.csproj"
COPY . .
RUN dotnet publish "Dt-Test-Api.csproj" -c Release -o /app/publish


# ==========================================
# Stage 3: Clean runtime image (Debian/glibc based)
# ==========================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS runtime
WORKDIR /app

# Copy compiled application binaries from Stage 2 (build-app)
COPY --from=build-app /app/publish .

# Copy the agent modules from Stage 1 and hand ownership directly to the .NET app user
# COPY --from=build-agent --chown=app:app /opt/dynatrace /opt/dynatrace

# --- Dynatrace Core Configuration ---
ENV DT_HOME=/opt/dynatrace/oneagent
ENV LD_PRELOAD=/opt/dynatrace/oneagent/agent/lib64/liboneagentproc.so

# --- .NET CLR Profiler Configuration ---
ENV CORECLR_ENABLE_PROFILING=1
ENV CORECLR_PROFILER={844887C7-95B7-4D2F-9624-C20D2756354D}
ENV CORECLR_PROFILER_PATH=/opt/dynatrace/oneagent/agent/lib64/liboneagentproc.so

# --- Diagnostics Framework ---
ENV DOTNET_CLR_EnableLog=1
ENV DOTNET_CLR_LogDestination=stdout
ENV DT_LOGLEVELCON=debug
ENV DT_LOGCON_PROC=stdout

# ENTRYPOINT ["/bin/sh", "-c", "source /opt/dynatrace/oneagent/dynatrace-env.sh && dotnet Dt-Test-Api.dll"]

ENTRYPOINT ["dotnet", "Dt-Test-Api.dll"]