import { useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { Wrench, AlertCircle, CheckCircle2, X } from 'lucide-react';

type AlertType = 'error' | 'success' | null;

export default function RegisterGoogle() {
  const navigate = useNavigate();
  const location = useLocation();

  const [alertInfo, setAlertInfo] = useState<{ type: AlertType; message: string }>({ type: null, message: '' });

  useEffect(() => {
    if (location.state?.alertMessage) {
      setAlertInfo({ type: 'error', message: location.state.alertMessage });
      window.history.replaceState({}, document.title);
    }
  }, [location.state]);

  // Auto-hide alert after 4 seconds
  useEffect(() => {
    if (alertInfo.type) {
      const timer = setTimeout(() => setAlertInfo({ type: null, message: '' }), 4000);
      return () => clearTimeout(timer);
    }
  }, [alertInfo]);

  const handleRegister = async (e: React.FormEvent) => {
    e.preventDefault();
    const formData = new FormData(e.target as HTMLFormElement);
    const data = Object.fromEntries(formData);
    
    // 1. Validar campos vacíos manualmente
    const requiredFields = [
      { key: 'firstName', name: 'Nombre' },
      { key: 'lastName', name: 'Apellido' },
      { key: 'email', name: 'Correo Electrónico' },
      { key: 'phone', name: 'Teléfono' },
      { key: 'dateOfBirth', name: 'Fecha de Nacimiento' }
    ];

    const missingFields = requiredFields.filter(f => !data[f.key]?.toString().trim());

    if (missingFields.length > 0) {
      const names = missingFields.map(f => f.name).join(', ');
      setAlertInfo({ 
        type: 'error', 
        message: `Por favor llena los siguientes campos: ${names}`
      });
      return;
    }

    try {
      const res = await fetch('http://localhost:5219/api/auth/register-google', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          firstName: data.firstName,
          lastName: data.lastName,
          email: data.email,
          phone: data.phone,
          dateOfBirth: data.dateOfBirth
        })
      });

      if (res.ok) {
        setAlertInfo({ type: 'success', message: '¡Cuenta creada! Redirigiendo para que inicies sesión con Google...' });
        setTimeout(() => navigate('/login'), 2000);
      } else {
        const errorData = await res.json();
        
        if (errorData.message?.toLowerCase().includes("existe un usuario") || errorData.detail?.toLowerCase().includes("existe")) {
          setAlertInfo({ type: 'error', message: 'Este correo ya está registrado, por favor inicie sesión.' });
        } else {
          setAlertInfo({ type: 'error', message: errorData.message || 'Error desconocido al registrar.' });
        }
      }
    } catch (error) {
      console.error(error);
      setAlertInfo({ type: 'error', message: 'Error de conexión con el servidor.' });
    }
  };

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

      <div className="absolute inset-0 z-0 overflow-hidden">
        <div className="absolute -top-[10%] -left-[10%] w-[50%] h-[50%] rounded-full bg-blue-400/20 blur-[120px]"></div>
        <div className="absolute -bottom-[10%] -right-[10%] w-[50%] h-[50%] rounded-full bg-indigo-500/20 blur-[120px]"></div>
      </div>

      <div className="relative z-10 bg-white/70 backdrop-blur-xl border border-white/40 p-10 rounded-3xl shadow-2xl max-w-lg w-full">
        
        {/* Header */}
        <div className="flex flex-col items-center mb-8">
          <div className="w-16 h-16 bg-gradient-to-tr from-blue-600 to-indigo-500 rounded-2xl flex items-center justify-center shadow-lg shadow-blue-500/30 mb-5 transform rotate-3">
            <Wrench className="w-8 h-8 text-white -rotate-3" />
          </div>
          <h1 className="text-3xl font-extrabold text-gray-800 tracking-tight text-center">
            AutoTaller<span className="text-blue-600">Manager</span>
          </h1>
          <p className="text-gray-500 mt-2 text-center font-medium">
            Completa tu cuenta con Google
          </p>
        </div>

        {/* Formulario (noValidate quita los mensajes por defecto del navegador) */}
        <form className="space-y-4" onSubmit={handleRegister} noValidate>
          
          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-semibold text-gray-700 mb-1">Nombre</label>
              <input 
                type="text" name="firstName"
                defaultValue={location.state?.prefillFirstName || ''}
                className="w-full px-4 py-3 rounded-xl border border-gray-200 bg-white/50 focus:bg-white focus:ring-2 focus:ring-blue-500/50 outline-none transition-all"
              />
            </div>
            <div>
              <label className="block text-sm font-semibold text-gray-700 mb-1">Apellido</label>
              <input 
                type="text" name="lastName"
                defaultValue={location.state?.prefillLastName || ''}
                className="w-full px-4 py-3 rounded-xl border border-gray-200 bg-white/50 focus:bg-white focus:ring-2 focus:ring-blue-500/50 outline-none transition-all"
              />
            </div>
          </div>

          <div>
            <label className="block text-sm font-semibold text-gray-700 mb-1">Correo Electrónico</label>
            <input 
              type="email" name="email"
              readOnly
              defaultValue={location.state?.prefillEmail || ''}
              className="w-full px-4 py-3 rounded-xl border border-gray-200 bg-gray-100 text-gray-500 cursor-not-allowed outline-none transition-all"
            />
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-semibold text-gray-700 mb-1">Teléfono (Celular)</label>
              <input 
                type="tel" name="phone"
                className="w-full px-4 py-3 rounded-xl border border-gray-200 bg-white/50 focus:bg-white focus:ring-2 focus:ring-blue-500/50 outline-none transition-all"
              />
            </div>
            <div>
              <label className="block text-sm font-semibold text-gray-700 mb-1">Fecha de Nacimiento</label>
              <input 
                type="date" name="dateOfBirth"
                className="w-full px-4 py-3 rounded-xl border border-gray-200 bg-white/50 focus:bg-white focus:ring-2 focus:ring-blue-500/50 outline-none transition-all text-gray-700"
              />
            </div>
          </div>

          {/* Omitimos contraseña porque inicia sesión con Google */}

          <div className="pt-4">
            <button type="submit" className="w-full bg-blue-600 hover:bg-blue-700 text-white font-semibold py-3.5 px-6 rounded-xl transition-all duration-200 shadow-md hover:shadow-lg active:scale-[0.98]">
              Completar Registro
            </button>
          </div>
        </form>

        <div className="mt-6 text-center">
          <button onClick={() => navigate('/login')} className="text-gray-500 font-medium hover:text-blue-600 transition-colors">
            Cancelar y volver al Login
          </button>
        </div>

      </div>
    </div>
  );
}
