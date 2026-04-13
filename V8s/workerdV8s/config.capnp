using Workerd = import "/workerd/workerd.capnp";

const config :Workerd.Config = (
  services = [
    (
      name = "main",
      worker = (
        modules = [
          (name = "main", esModule = embed "worker.js")
        ],
        compatibilityDate = "2026-01-01",
      )
    ),
    (name = "auth-service", worker = (modules = [(name = "main", esModule = embed "auth.js")], compatibilityDate = "2026-01-01")),
    (name = "api-service", worker = (modules = [(name = "main", esModule = embed "api.js")], compatibilityDate = "2026-01-01"))
  ],
  sockets = [
    (
      name = "http",
      address = "*:8080",
      http = (),
      service = "main"
    ),
    (name = "auth-socket", address = "*:9080", http = (), service = "auth-service"),
    (name = "api-socket", address = "*:9090", http = (), service = "api-service")
  ]
);