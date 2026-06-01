import { useEffect, useState } from 'react';
import { AlertCircle, User, Mail, Phone, Calendar, Shield, BadgeCheck } from 'lucide-react';

interface ClientProfileDto {
  firstName: string;
  lastName: string;
  email: string;
  phone: string | null;
  dateOfBirth: string | null;
  role: string | null;
  memberSince: string | null;
}

export default function Configuracion() {
  const [profile, setProfile] = useState<ClientProfileDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    const fetchProfile = async () => {
      try {
        const token = localStorage.getItem('token');
        const res = await fetch('http://localhost:5219/api/dashboard/client/perfil', {
          headers: { 'Authorization': `Bearer ${token}` }
        });
        if (res.ok) setProfile(await res.json());
        else setError('No se pudo cargar el perfil.');
      } catch {
        setError('Error de conexión.');
      } finally {
        setLoading(false);
      }
    };
    fetchProfile();
  }, []);

  if (loading) return (
    <div className="flex items-center justify-center min-h-[50vh]">
      <div className="animate-spin w-10 h-10 border-4 border-blue-600 border-t-transparent rounded-full" />
    </div>
  );

  if (error || !profile) return (
    <div className="flex flex-col items-center justify-center min-h-[50vh] gap-4 text-center">
      <AlertCircle className="w-12 h-12 text-red-400" />
      <p className="text-gray-500">{error || 'No se pudo cargar tu perfil.'}</p>
    </div>
  );

  const initials = `${profile.firstName.charAt(0)}${profile.lastName.charAt(0)}`.toUpperCase();

  const fields = [
    { icon: <User className="w-5 h-5 text-gray-400" />, label: 'Nombre Completo', value: `${profile.firstName} ${profile.lastName}` },
    { icon: <Mail className="w-5 h-5 text-gray-400" />, label: 'Correo Electrónico', value: profile.email },
    { icon: <Phone className="w-5 h-5 text-gray-400" />, label: 'Teléfono', value: profile.phone ?? 'No registrado' },
    { icon: <Calendar className="w-5 h-5 text-gray-400" />, label: 'Fecha de Nacimiento', value: profile.dateOfBirth ?? 'No registrada' },
  ];

  return (
    <div className="max-w-3xl mx-auto space-y-6 pb-10">
      <div>
        <h2 className="text-2xl font-bold text-gray-800">Configuración</h2>
        <p className="text-sm text-gray-500 mt-1">Información de tu cuenta y perfil</p>
      </div>

      {/* Profile Card */}
      <div className="bg-white rounded-3xl shadow-sm border border-gray-100 overflow-hidden">
        {/* Header gradient */}
        <div className="h-24 bg-gradient-to-r from-blue-500 via-indigo-500 to-purple-500 relative">
          <div className="absolute -bottom-8 left-8">
            <div className="w-20 h-20 rounded-2xl bg-white shadow-lg flex items-center justify-center border-4 border-white">
              <span className="text-2xl font-extrabold bg-gradient-to-br from-blue-500 to-indigo-600 bg-clip-text text-transparent">
                {initials}
              </span>
            </div>
          </div>
        </div>

        {/* Name & role */}
        <div className="pt-12 px-8 pb-6 border-b border-gray-50">
          <div className="flex items-start justify-between">
            <div>
              <h3 className="text-xl font-extrabold text-gray-800">{profile.firstName} {profile.lastName}</h3>
              <div className="flex items-center gap-2 mt-1">
                <span className="inline-flex items-center gap-1.5 px-3 py-1 bg-blue-50 text-blue-700 text-xs font-bold rounded-full">
                  <Shield className="w-3 h-3" />
                  {profile.role ?? 'Cliente'}
                </span>
                {profile.memberSince && (
                  <span className="inline-flex items-center gap-1.5 px-3 py-1 bg-gray-50 text-gray-500 text-xs font-semibold rounded-full">
                    <BadgeCheck className="w-3 h-3" />
                    Miembro desde {profile.memberSince}
                  </span>
                )}
              </div>
            </div>
          </div>
        </div>

        {/* Fields */}
        <div className="px-8 py-6 space-y-5">
          <h4 className="text-xs font-bold uppercase tracking-wider text-gray-400">Información Personal</h4>
          {fields.map(({ icon, label, value }) => (
            <div key={label} className="flex items-center gap-4 py-3 border-b border-gray-50 last:border-0">
              <div className="w-10 h-10 bg-gray-50 rounded-xl flex items-center justify-center shrink-0">
                {icon}
              </div>
              <div className="flex-1 min-w-0">
                <p className="text-xs font-semibold text-gray-400 uppercase tracking-wide">{label}</p>
                <p className="text-sm font-semibold text-gray-800 mt-0.5 truncate">{value}</p>
              </div>
            </div>
          ))}
        </div>
      </div>

      {/* Info note */}
      <div className="bg-blue-50 rounded-2xl p-5 flex items-start gap-4 border border-blue-100">
        <div className="w-10 h-10 bg-blue-100 rounded-xl flex items-center justify-center shrink-0">
          <BadgeCheck className="w-5 h-5 text-blue-600" />
        </div>
        <div>
          <p className="text-sm font-bold text-blue-800">¿Necesitas actualizar tus datos?</p>
          <p className="text-sm text-blue-600 mt-0.5">
            Comunícate con el personal del taller y ellos podrán actualizar tu información de contacto.
          </p>
        </div>
      </div>
    </div>
  );
}
