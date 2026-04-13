export default {
  async fetch(request, env, ctx) {
    const url = new URL(request.url);

    // Endpoint 1: Hello
    if (url.pathname === "/hello") {
      return new Response("Welcome to the Hello endpoint!");
    }

    // Endpoint 2: JSON Data
    if (url.pathname === "/api/data") {
      const data = { status: "online", isolate: "v8-primary" };
      return new Response(JSON.stringify(data), {
        headers: { "Content-Type": "application/json" }
      });
    }

    // Endpoint 3: User Agent Check
    if (url.pathname === "/check") {
      const ua = request.headers.get("user-agent");
      return new Response(`You are visiting from: ${ua}`);
    }

    return new Response("404 - Not Found", { status: 404 });
  }
};
