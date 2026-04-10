###### podman containers

```


podman machine init
podman machine start
podman run -d --name my-vscode-server `
  -p 8080:8080 `
  -v "${PWD}:/home/coder/project:Z" `
  -e PASSWORD=yourpassword `
  codercom/code-server:latest
```