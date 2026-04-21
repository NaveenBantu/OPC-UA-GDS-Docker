# GDS Docker Stack – Setup Guide

Full OPC UA Global Discovery Server setup based on:
- **GDSwithREST** – OPC UA GDS backend + Swagger REST API
- **GDSwithUI** – Blazor web UI frontend
- **SQL Server 2022** – persistence

---

## Directory layout

```
gds-docker/
├── docker-compose.yml          ← orchestrates all three services
├── GDSwithREST/                ← clone of romanett/GDSwithREST here
│   └── Dockerfile              ← build-from-source Dockerfile
└── GDSwithUI/                  ← clone of romanett/GDSwithUI here
    └── Dockerfile              ← build-from-source Dockerfile
```

---

## Step 1 – Clone the source repos

```bash
# from inside gds-docker/
git clone https://github.com/romanett/GDSwithREST.git  GDSwithREST
git clone https://github.com/romanett/GDSwithUI.git     GDSwithUI

# Copy the Dockerfiles we generated into each repo folder
# (they are already placed correctly if you kept the layout above)
```

---

## Step 2 – Change the SQL password (important!)

In `docker-compose.yml`, replace **all three occurrences** of `Str0ng!GDS_Passw0rd`
with your own strong password. The same string must appear in:

- `db` → `MSSQL_SA_PASSWORD`
- `db` → `healthcheck` test command
- `gds-rest` → `ConnectionStrings__DefaultConnection`

---

## Step 3 – Build and start

```bash
cd gds-docker
docker compose up --build
```

First build downloads the .NET SDK image and restores NuGet packages —
allow ~5 minutes on a fresh machine.

---

## Step 4 – Access the services

| Service | URL | Notes |
|---------|-----|-------|
| REST API (HTTP) | http://localhost:8080 | Redirects to HTTPS |
| REST API Swagger | https://localhost:8081/swagger | Interactive API docs |
| Blazor Web UI | http://localhost:8082 | GDS management UI |
| OPC UA endpoint | opc.tcp://localhost:58810/GlobalDiscoveryServer | UA clients connect here |

---

## Default GDS credentials

| Role | Username | Password |
|------|----------|----------|
| CA Admin | `CertificateAuthorityAdmin` | `demo` |
| Discovery Admin | `DiscoveryAdmin` | `demo` |
| Sys Admin (push) | `sysadmin` | `demo` |

Change these in `GDSwithREST/GDSwithREST/appsettings.json` before exposing to a network.

---

## Connecting a PCD3.M6893 / Niagara client (Pull model)

1. In your OPC UA client config, set the GDS endpoint to:
   ```
   opc.tcp://<host-ip>:58810/GlobalDiscoveryServer
   ```
2. Authenticate with one of the roles above.
3. Request a signed certificate via the GDS Pull workflow; the GDS will
   sign it with its own CA and return the trust list.

For Push model, use the OPC Foundation GDS client tool to push certificates
to the GDS on behalf of your controllers.

---

## Stopping / removing

```bash
docker compose down          # stop containers, keep volumes
docker compose down -v       # stop containers AND delete DB + OPC cert volumes
```

---

## Troubleshooting

**`sqlcmd` not found in healthcheck** – The mssql-tools path changed between
SQL Server versions. If the healthcheck keeps failing, update the path in
`docker-compose.yml`:
- Older images: `/opt/mssql-tools/bin/sqlcmd`
- Newer images: `/opt/mssql-tools18/bin/sqlcmd`

**SSL errors between gds-ui and gds-rest** – The REST API uses a self-signed
certificate. `GdsApi__DisableSslValidation: "true"` suppresses the error
inside the Docker network. Do not expose this externally without a proper cert.

**OPC UA certificate untrusted** – On first connect, the GDS server cert may
be rejected by your client. Accept/trust it once via your OPC UA client's
certificate manager, or add it to the application trust list.
