import { useState, useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import { useNavigate, useLocation } from 'react-router-dom';
import { GoogleOAuthProvider, useGoogleLogin } from '@react-oauth/google';
import { Wrench, AlertCircle, X, CheckCircle2 } from 'lucide-react';

const GOOGLE_CLIENT_ID = '995630653930-6nafcdrfnl0kh3lbtfltqckfln3ha2fl.apps.googleusercontent.com';

type AlertType = 'error' | 'success' | null;

export default function Login() {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const location = useLocation();

  const [alertInfo, setAlertInfo] = useState<{ type: AlertType; message: string }>({ type: null, message: '' });

  useEffect(() => {
    if (location.state?.alertMessage) {
      setAlertInfo({ type: 'error', message: location.state.alertMessage });
      window.history.replaceState({}, document.title);
    }
  }, [location.state]);

  useEffect(() => {
    if (alertInfo.type) {
      const timer = setTimeout(() => setAlertInfo({ type: null, message: '' }), 5000);
      return () => clearTimeout(timer);
    }
  }, [alertInfo]);

  useEffect(() => {
    const token = localStorage.getItem('token');
    const role = localStorage.getItem('userRole');
    if (token) {
      if (role === 'Administrador' || role === 'Admin') {
        navigate('/admin/dashboard');
      } else if (role === 'Recepcionista') {
        navigate('/recepcion/dashboard');
      } else if (role === 'Mecanico' || role === 'Mecánico') {
        navigate('/mecanico/dashboard');
      } else {
        navigate('/cliente/dashboard');
      }
    }
  }, [navigate]);

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    const formData = new FormData(e.target as HTMLFormElement);
    const data = Object.fromEntries(formData);

    try {
      const res = await fetch('http://localhost:5219/api/auth/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data)
      });

      if (res.ok) {
        const resData = await res.json();
        localStorage.setItem('token', resData.token);
        localStorage.setItem('userEmail', resData.email);
        localStorage.setItem('userRole', resData.roleNombre);
        
        if (resData.roleNombre === 'Administrador' || resData.roleNombre === 'Admin') {
          navigate('/admin/dashboard');
        } else if (resData.roleNombre === 'Recepcionista') {
          navigate('/recepcion/dashboard');
        } else if (resData.roleNombre === 'Mecanico' || resData.roleNombre === 'Mecánico') {
          navigate('/mecanico/dashboard');
        } else {
          navigate('/cliente/dashboard');
        }
      } else {
        const errorData = await res.json();
        setAlertInfo({ type: 'error', message: errorData.message || 'Error de validación.' });
      }
    } catch (error) {
      console.error(error);
      setAlertInfo({ type: 'error', message: 'Error de conexión con el servidor.' });
    }
  };

  const handleGoogleLogin = useGoogleLogin({
    onSuccess: async (tokenResponse) => {
      try {
        const res = await fetch('http://localhost:5219/api/auth/google', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({ idToken: tokenResponse.access_token })
        });
        
        if (res.ok) {
          const data = await res.json();
          localStorage.setItem('token', data.token);
          localStorage.setItem('userEmail', data.email);
          localStorage.setItem('userRole', data.roleNombre);
          
          if (data.roleNombre === 'Administrador' || data.roleNombre === 'Admin') {
            navigate('/admin/dashboard');
          } else if (data.roleNombre === 'Recepcionista') {
            navigate('/recepcion/dashboard');
          } else if (data.roleNombre === 'Mecanico' || data.roleNombre === 'Mecánico') {
            navigate('/mecanico/dashboard');
          } else {
            navigate('/cliente/dashboard');
          }
        } else {
          const errorData = await res.json();
          const errorMessage = errorData.message || '';

          if (errorMessage.includes("Correo no registrado|")) {
             const parts = errorMessage.split('|');
             const email = parts[1] || '';
             const firstName = parts[2] || '';
             const lastName = parts[3] || '';
             
             navigate('/register/google', { 
               state: { 
                 alertMessage: 'Completa tu registro para continuar.',
                 prefillEmail: email,
                 prefillFirstName: firstName,
                 prefillLastName: lastName
               } 
             });
          } else {
             setAlertInfo({ type: 'error', message: errorMessage });
          }
        }
      } catch (error) {
        console.error("Fetch Error:", error);
        setAlertInfo({ type: 'error', message: "Error conectando con el servidor." });
      }
    },
    onError: () => {
      console.error("Google Login Failed");
      setAlertInfo({ type: 'error', message: "Error al iniciar sesión con Google" });
    }
  });

  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-50 to-gray-200 flex items-center justify-center p-4 relative">
      
      {/* ALERTA FLOTANTE (Toast) */}
      <div 
        className={`fixed top-6 left-1/2 -translate-x-1/2 z-50 transition-all duration-500 ease-out flex items-center gap-3 px-5 py-3 rounded-2xl shadow-2xl backdrop-blur-md border ${
          alertInfo.type === 'error' 
            ? 'bg-red-500/10 border-red-500/20 text-red-700 translate-y-0 opacity-100' 
            : alertInfo.type === 'success'
            ? 'bg-green-500/10 border-green-500/20 text-green-700 translate-y-0 opacity-100'
            : '-translate-y-20 opacity-0 pointer-events-none'
        }`}
      >
        {alertInfo.type === 'error' && <AlertCircle className="w-5 h-5 text-red-500/80" />}
        {alertInfo.type === 'success' && <CheckCircle2 className="w-5 h-5 text-green-500/80" />}
        <span className="font-medium text-sm drop-shadow-sm">{alertInfo.message}</span>
        <button onClick={() => setAlertInfo({ type: null, message: '' })} className="ml-2 opacity-50 hover:opacity-100 transition-opacity">
          <X className="w-4 h-4" />
        </button>
      </div>

      {/* Background decoration to simulate a high-end garage vibe */}
      <div className="absolute inset-0 z-0 overflow-hidden">
        <div className="absolute -top-[10%] -right-[10%] w-[50%] h-[50%] rounded-full bg-blue-400/20 blur-[120px]"></div>
        <div className="absolute -bottom-[10%] -left-[10%] w-[50%] h-[50%] rounded-full bg-indigo-500/20 blur-[120px]"></div>
      </div>

      <div className="relative z-10 bg-white/70 backdrop-blur-xl border border-white/40 p-10 rounded-3xl shadow-2xl max-w-md w-full">
        
        {/* Logo / Header */}
        <div className="flex flex-col items-center mb-10">
          <div className="w-16 h-16 bg-gradient-to-tr from-blue-600 to-indigo-500 rounded-2xl flex items-center justify-center shadow-lg shadow-blue-500/30 mb-5 transform rotate-3">
            <Wrench className="w-8 h-8 text-white -rotate-3" />
          </div>
          <h1 className="text-3xl font-extrabold text-gray-800 tracking-tight text-center">
            AutoTaller<span className="text-blue-600">Manager</span>
          </h1>
          <p className="text-gray-500 mt-2 text-center font-medium">
            {t('login.title')}
          </p>
        </div>

        {/* Traditional Form */}
        <form className="space-y-5" onSubmit={handleLogin}>
          <div>
            <label className="block text-sm font-semibold text-gray-700 mb-1.5">
              {t('login.emailLabel')}
            </label>
            <input 
              type="email" 
              name="email"
              placeholder={t('login.emailPlaceholder')}
              className="w-full px-4 py-3 rounded-xl border border-gray-200 bg-white/50 focus:bg-white focus:ring-2 focus:ring-blue-500/50 focus:border-blue-500 transition-all outline-none text-gray-700"
            />
          </div>

          <div>
            <label className="block text-sm font-semibold text-gray-700 mb-1.5">
              {t('login.passwordLabel')}
            </label>
            <input 
              type="password" 
              name="password"
              placeholder={t('login.passwordPlaceholder')}
              className="w-full px-4 py-3 rounded-xl border border-gray-200 bg-white/50 focus:bg-white focus:ring-2 focus:ring-blue-500/50 focus:border-blue-500 transition-all outline-none text-gray-700"
            />
          </div>

          <button className="w-full bg-gray-900 hover:bg-black text-white font-semibold py-3.5 px-6 rounded-xl transition-all duration-200 shadow-md hover:shadow-lg active:scale-[0.98]">
            {t('login.submitBtn')}
          </button>
        </form>

        {/* Divider */}
        <div className="mt-8 mb-6 flex items-center">
          <div className="flex-1 border-t border-gray-200"></div>
          <span className="px-4 text-sm text-gray-400 font-medium">
            {t('login.orContinueWith')}
          </span>
          <div className="flex-1 border-t border-gray-200"></div>
        </div>

        {/* Google Auth Button */}
        <button 
          onClick={() => handleGoogleLogin()}
          className="w-full bg-white hover:bg-gray-50 text-gray-700 font-semibold py-3 px-6 rounded-xl transition-all duration-200 shadow-sm border border-gray-200 flex items-center justify-center gap-3 active:scale-[0.98]"
        >
          <svg className="w-5 h-5" viewBox="0 0 24 24">
            <path d="M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92c-.26 1.37-1.04 2.53-2.21 3.31v2.77h3.57c2.08-1.92 3.28-4.74 3.28-8.09z" fill="#4285F4" />
            <path d="M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99 20.53 7.7 23 12 23z" fill="#34A853" />
            <path d="M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.07H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.93l2.85-2.22.81-.62z" fill="#FBBC05" />
            <path d="M12 5.38c1.62 0 3.06.56 4.21 1.64l3.15-3.15C17.45 2.09 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.07l3.66 2.84c.87-2.6 3.3-4.53 6.16-4.53z" fill="#EA4335" />
          </svg>
          {t('login.googleBtn')}
        </button>

      </div>
    </div>
  );
}
