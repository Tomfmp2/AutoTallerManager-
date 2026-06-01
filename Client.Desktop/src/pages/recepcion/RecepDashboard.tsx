import { useEffect, useState } from 'react';
import { Calendar, Users, CheckCircle, FileText, AlertCircle } from 'lucide-react';
import { useNavigate } from 'react-router-dom';

export default function RecepDashboard() {
  const [stats, setStats] = useState({
    pendingAppointments: 0,
    activeRepairs: 0,
    totalClients: 0
  });
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  useEffect(() => {
    const fetchStats = async () => {
      try {
        const token = localStorage.getItem('token');
        const res = await fetch('http://localhost:5219/api/recepcion/dashboard-stats', {
          headers: {
            'Authorization': `Bearer ${token}`
          }
        });
        
        if (res.ok) {
          const data = await res.json();
          setStats({
            pendingAppointments: data.pendingAppointments || 0,
            activeRepairs: data.activeRepairs || 0,
            totalClients: data.totalClients || 0
          });
        } else {
          console.error('Error fetching dashboard stats', await res.text());
        }
      } catch (err) {
        console.error('Network error fetching stats', err);
      } finally {
        setLoading(false);
      }
    };

    fetchStats();
  }, []);

  if (loading) {
    return (
      <div className="flex items-center justify-center min-h-[50vh]">
        <div className="animate-spin w-10 h-10 border-4 border-blue-600 border-t-transparent rounded-full" />
      </div>
    );
  }

  return (
    <div className="max-w-6xl mx-auto space-y-8 pb-10">
      <div className="flex justify-between items-end">
        <div>
          <h2 className="text-2xl font-bold text-gray-800">Hola, Recepcionista</h2>
          <p className="text-sm text-gray-500 mt-1">Aquí tienes un resumen de la actividad del taller.</p>
        </div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        <div 
          onClick={() => navigate('/recepcion/aprobaciones')}
          className="bg-amber-50 rounded-2xl p-6 border border-amber-100 flex items-center justify-between cursor-pointer hover:shadow-md transition-shadow"
        >
          <div>
            <p className="text-sm font-semibold text-amber-700 uppercase tracking-wide">Revisiones Pendientes</p>
            <h3 className="text-4xl font-black text-amber-900 mt-2">{stats.pendingAppointments}</h3>
          </div>
          <div className="w-16 h-16 bg-amber-200/50 rounded-full flex items-center justify-center text-amber-600">
            <Calendar className="w-8 h-8" />
          </div>
        </div>

        <div className="bg-blue-50 rounded-2xl p-6 border border-blue-100 flex items-center justify-between">
          <div>
            <p className="text-sm font-semibold text-blue-700 uppercase tracking-wide">Reparaciones Activas</p>
            <h3 className="text-4xl font-black text-blue-900 mt-2">{stats.activeRepairs}</h3>
          </div>
          <div className="w-16 h-16 bg-blue-200/50 rounded-full flex items-center justify-center text-blue-600">
            <Wrench className="w-8 h-8" />
          </div>
        </div>

        <div 
          onClick={() => navigate('/recepcion/clientes')}
          className="bg-green-50 rounded-2xl p-6 border border-green-100 flex items-center justify-between cursor-pointer hover:shadow-md transition-shadow"
        >
          <div>
            <p className="text-sm font-semibold text-green-700 uppercase tracking-wide">Clientes Registrados</p>
            <h3 className="text-4xl font-black text-green-900 mt-2">{stats.totalClients}</h3>
          </div>
          <div className="w-16 h-16 bg-green-200/50 rounded-full flex items-center justify-center text-green-600">
            <Users className="w-8 h-8" />
          </div>
        </div>
      </div>

      <div className="bg-white rounded-2xl border p-8 text-center border-gray-100">
        <div className="flex justify-center mb-4">
          <FileText className="w-16 h-16 text-gray-200" />
        </div>
        <h3 className="text-lg font-bold text-gray-700">Bienvenido al nuevo panel</h3>
        <p className="text-gray-500 max-w-md mx-auto mt-2">
          Desde aquí podrás gestionar las Revisiones pendientes, aceptar clientes, asignar mecánicos y enviar notificaciones.
        </p>
      </div>
    </div>
  );
}

// Added Wrench import since it was missing
import { Wrench } from 'lucide-react';


