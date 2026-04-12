const ivm = require('isolated-vm');

// 1. Create a pool of 8 Isolates
const isolates = Array.from({ length: 8 }, () => new ivm.Isolate({ memoryLimit: 128 }));

async function runInIsolate(isolate, id) {
  const context = await isolate.createContext();
  const jail = context.global;

  // 3. Setup a more direct "log" bridge
  // Using .setSync or a simple reference for basic strings is often more stable
  await jail.set('log', new ivm.Reference(function(...args) {
    console.log(`[Isolate ${id}]:`, ...args);
  }));

  // 4. The Logic
  // Notice we use $0.applySync to call the reference we passed in
  const script = await isolate.compileScript(`
    const message = "Hello from the sandbox";
    // Using the reference requires calling it via the bridge
    log.applySync(undefined, [message + " logic!"]);
    "Result from " + ${id};
  `);

  const result = await script.run(context);
  console.log(`✅ Main Process received: ${result}`);
  return result;
}

// Wrap in a top-level catch to see hidden errors
(async () => {
    console.log("🚀 Dispatching isolates...");
    try {
        const results = await Promise.all(isolates.map((iso, i) => runInIsolate(iso, i)));
        console.log('🏁 All results gathered:', results);
    } catch (e) {
        console.error("❌ Fatal Error:", e);
    }
})();