import React from "react";
import ReactDOM from "react-dom/client";
import { GoogleOAuthProvider } from '@react-oauth/google';
import App from "./App";
import './i18n/config'; // Inicializar i18n

// Configuración de Google Cloud
const GOOGLE_CLIENT_ID = "995630653930-6nafcdrfnl0kh3lbtfltqckfln3ha2fl.apps.googleusercontent.com";

ReactDOM.createRoot(document.getElementById("root") as HTMLElement).render(
  <React.StrictMode>
    <GoogleOAuthProvider clientId={GOOGLE_CLIENT_ID}>
      <App />
    </GoogleOAuthProvider>
  </React.StrictMode>,
);
