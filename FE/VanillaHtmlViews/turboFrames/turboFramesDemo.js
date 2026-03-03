const express = require('express');
const app = express();

// Layout helper to wrap our frames in a full HTML document
const layout = (content) => `
  <!DOCTYPE html>
  <html>
  <head>
    <title>Turbo Frame Demo</title>
    <script src="https://unpkg.com/@hotwired/turbo@7.3.0/dist/turbo.es2017-umd.js"></script>
    <style>
      body { font-family: sans-serif; padding: 20px; line-height: 1.6; }
      .sidebar { background: #eee; padding: 10px; margin-bottom: 20px; }
      turbo-frame { display: block; border: 2px dashed #ccc; padding: 15px; margin: 10px 0; }
      .btn { background: #007bff; color: white; padding: 5px 10px; text-decoration: none; border-radius: 4px; }
    </style>
  </head>
  <body>
    <div class="sidebar">Global Navigation (Never Reloads)</div>
    ${content}
  </body>
  </html>
`;

// 1. The Main Page
app.get('/', (req, res) => {
  res.send(layout(`
    <h1>User Dashboard</h1>
    <turbo-frame id="user_profile">
      <p>Current User: <strong>John Doe</strong></p>
      <a href="/edit" class="btn">Edit Profile Name</a>
    </turbo-frame>
    <p>This text is outside the frame and stays put.</p>
  `));
});

// 2. The Edit Page (Requested when "Edit" is clicked)
app.get('/edit', (req, res) => {
  res.send(layout(`
    <h1>Edit Screen</h1>
    <turbo-frame id="user_profile">
      <form action="/" method="get">
        <input type="text" value="John Doe">
        <button type="submit">Save</button>
        <a href="/">Cancel</a>
      </form>
    </turbo-frame>
    <div style="color: red;">This warning is ignored by Turbo!</div>
  `));
});

app.listen(3000, () => console.log('Demo running at http://localhost:3000'));