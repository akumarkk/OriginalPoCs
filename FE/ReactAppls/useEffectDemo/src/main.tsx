import { StrictMode } from 'react'
import { BrowserRouter, Routes, Route } from 'react-router-dom'
import { createRoot } from 'react-dom/client'
import './index.css'
import App from './App.tsx'
import { UseEffectDemo } from './Pages/useEffectDemo'
import { ReRenderDemo } from './Pages/reRenderDemo.tsx'


createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <BrowserRouter>
    <Routes>
      <Route path="/use-effect-demo" element={<UseEffectDemo />} />
      <Route path="/rerender-demo" element={<ReRenderDemo />} />
    </Routes>
    </BrowserRouter>
    <App />
  </StrictMode>,
)
