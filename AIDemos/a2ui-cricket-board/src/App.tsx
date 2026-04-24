import { A2UIRenderer } from '@copilotkit/a2ui-renderer';
import { A2UIProvider } from '@a2ui/react';
import './App.css'
import { Catalog } from './Catalog'

function App() {
  return (
    <A2UIProvider>
      <div className="App">
        <h1>Cricket Legends Dashboard</h1>
        <A2UIRenderer 
          catalog={Catalog} 
        />
      </div>
    </A2UIProvider>
  );
}

export default App
