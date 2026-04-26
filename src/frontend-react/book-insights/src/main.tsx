import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import sharedFaviconUrl from '../../../shared/icons/favicon.ico'
import './index.css'
import App from './App.tsx'

const faviconLink =
  document.querySelector<HTMLLinkElement>("link[rel='icon']") ?? document.createElement('link')

faviconLink.setAttribute('rel', 'icon')
faviconLink.setAttribute('type', 'image/x-icon')
faviconLink.setAttribute('href', sharedFaviconUrl)

if (!faviconLink.parentNode) {
  document.head.appendChild(faviconLink)
}

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <App />
  </StrictMode>,
)
