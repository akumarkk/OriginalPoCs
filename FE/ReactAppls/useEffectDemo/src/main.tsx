import { StrictMode } from 'react'
import { BrowserRouter, Routes, Route } from 'react-router-dom'
import { createRoot } from 'react-dom/client'
import './index.css'
import App from './App.tsx'
import { UseEffectDemo } from './Pages/useEffectDemo'
import { ReRenderWithMemoDemo } from './Pages/reRenderWithMemoDemo.tsx'
import { ReRenderNoMemoDemo } from './Pages/reRenderNoMemoDemo.tsx'
import { ReRenderWithCallbackDemo } from './Pages/reRenderWithCallbackDemo.tsx'


createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <BrowserRouter>
    <Routes>
      <Route path="/use-effect-demo" element={<UseEffectDemo />} />
      <Route path="/rerender-callback-demo" element={<ReRenderWithCallbackDemo />} />
      <Route path="/rerender-memo-demo" element={<ReRenderWithMemoDemo />} />
      <Route path="/rerender-no-memo-demo" element={<ReRenderNoMemoDemo />} />
    </Routes>
    </BrowserRouter>
    <App />
  </StrictMode>,
)
