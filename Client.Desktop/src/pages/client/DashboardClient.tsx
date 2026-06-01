import { useEffect, useState } from 'react';
import { Wrench, CheckCircle, Clock, FileText, AlertCircle } from 'lucide-react';
import { useNavigate } from 'react-router-dom';

interface DashboardSummary {
  activeRepair: {
    vehicleInfo: string;
    licensePlate: string;
    progressPercentage: number;
    status: string;
    currentWork: string;
    estimatedDelivery: string;
  } | null;
  upcomingAppointments: {
    id: number;
    title: string;
    date: string;
  }[];
  recentHistory: {
    id: number;
    title: string;
    description: string;
    date: string;
  }[];
}

export default function DashboardClient() {
  const [data, setData] = useState<DashboardSummary | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const navigate = useNavigate();

  useEffect(() => {
    const fetchSummary = async () => {
      try {
        const token = localStorage.getItem('token');
        if (!token) {
          navigate('/login');
          return;
        }

        const res = await fetch('http://localhost:5219/api/dashboard/client', {
          headers: { 'Authorization': `Bearer ${token}` }
        });

        if (res.ok) {
          const summary = await res.json();
          setData(summary);
        } else {
          setError('No se pudo cargar la información del servidor.');
        }
      } catch (e) {
        setError('Error de conexión.');
      } finally {
        setLoading(false);
      }
    };

    fetchSummary();
  }, [navigate]);

  if (loading) {
    return (
      <div className="flex items-center justify-center min-h-[50vh]">
        <div className="animate-spin w-10 h-10 border-4 border-blue-600 border-t-transparent rounded-full"></div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="flex flex-col items-center justify-center min-h-[50vh] text-center">
        <AlertCircle className="w-12 h-12 text-red-500 mb-4" />
        <h2 className="text-xl font-bold text-gray-800">Ups, algo salió mal</h2>
        <p className="text-gray-500">{error}</p>
      </div>
    );
  }

  return (
    <div className="max-w-6xl mx-auto space-y-8 pb-10">
      
      {/* Saludo y Tarjeta Principal del Vehículo */}
      <section>
        <h3 className="text-gray-500 font-medium mb-4 uppercase tracking-wider text-sm">Estado Actual</h3>
        {data?.activeRepair ? (
          <div className="bg-white rounded-3xl p-8 shadow-sm border border-gray-100 flex flex-col md:flex-row items-center gap-8 relative overflow-hidden">
            <div className="absolute -right-20 -top-20 w-64 h-64 bg-blue-50 rounded-full blur-3xl opacity-60"></div>
            
            <div className="flex-1 relative z-10">
              <div className="inline-flex items-center gap-2 px-3 py-1 bg-blue-50 text-blue-700 text-xs font-bold uppercase rounded-full mb-4 tracking-wide">
                <span className="w-2 h-2 rounded-full bg-blue-600 animate-pulse"></span>
                {data.activeRepair.status}
              </div>
              <h2 className="text-3xl font-extrabold text-gray-800 tracking-tight">{data.activeRepair.vehicleInfo}</h2>
              <p className="text-gray-500 font-medium mt-1">Placa: {data.activeRepair.licensePlate}</p>
              
              <div className="mt-8 space-y-3">
                <div className="flex justify-between text-sm font-semibold text-gray-600">
                  <span>Progreso Estimado</span>
                  <span className="text-blue-600">{data.activeRepair.progressPercentage}%</span>
                </div>
                <div className="w-full bg-gray-100 rounded-full h-3 overflow-hidden">
                  <div className={`bg-gradient-to-r from-blue-500 to-indigo-500 h-3 rounded-full transition-all duration-1000`} style={{ width: `${data.activeRepair.progressPercentage}%` }}></div>
                </div>
              </div>
            </div>
            
            <div className="w-full md:w-1/3 bg-gray-50 rounded-2xl p-6 border border-gray-100 relative z-10">
              <h4 className="text-sm font-bold text-gray-700 mb-4 flex items-center gap-2">
                <Wrench className="w-4 h-4 text-gray-400" />
                Trabajo Actual
              </h4>
              <p className="text-gray-600 text-sm leading-relaxed font-medium">
                {data.activeRepair.currentWork}
              </p>
              <div className="mt-6 pt-4 border-t border-gray-200 flex justify-between items-center">
                <span className="text-xs font-semibold text-gray-400 uppercase">Entrega Estimada</span>
                <span className="text-sm font-bold text-gray-800">{data.activeRepair.estimatedDelivery}</span>
              </div>
            </div>
          </div>
        ) : (
          <div className="bg-white rounded-3xl p-8 shadow-sm border border-gray-100 flex flex-col items-center text-center">
            <CheckCircle className="w-12 h-12 text-green-500 mb-4" />
            <h2 className="text-xl font-bold text-gray-800 mb-2">¡Todo al día!</h2>
            <p className="text-gray-500">No tienes vehículos en reparación en este momento.</p>
          </div>
        )}
      </section>

      {/* Grid de Información Secundaria */}
      <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
        
        {/* Próximas Revisiones */}
        <section>
          <div className="flex items-center justify-between mb-4">
            <h3 className="text-gray-500 font-medium uppercase tracking-wider text-sm">Próximas Revisiones</h3>
            <button onClick={() => navigate('/cliente/historial?tab=Revisiones')} className="text-blue-600 text-sm font-semibold hover:underline">Ver calendario</button>
          </div>
          <div className="bg-white rounded-3xl p-6 shadow-sm border border-gray-100 h-full flex flex-col justify-center items-center text-center">
            {data?.upcomingAppointments && data.upcomingAppointments.length > 0 ? (
              <ul className="space-y-4 w-full text-left">
                {data.upcomingAppointments.map((appt) => (
                  <li key={appt.id} className="bg-gray-50 rounded-xl p-4 flex items-center gap-4 border border-gray-100">
                    <div className="w-12 h-12 bg-blue-100 rounded-full flex items-center justify-center text-blue-600 shrink-0">
                      <Clock className="w-6 h-6" />
                    </div>
                    <div>
                      <h4 className="text-gray-800 font-bold text-sm">{appt.title}</h4>
                      <p className="text-blue-600 text-xs mt-1 font-semibold">{appt.date}</p>
                    </div>
                  </li>
                ))}
              </ul>
            ) : (
              <>
                <div className="w-16 h-16 bg-blue-50 rounded-full flex items-center justify-center text-blue-500 mb-4">
                  <Clock className="w-8 h-8" />
                </div>
                <h4 className="text-gray-800 font-bold mb-1">No tienes Revisiones programadas</h4>
                <p className="text-gray-500 text-sm mb-6 max-w-xs mx-auto">
                  Agenda tu próxima revisión preventiva para mantener tu vehículo en óptimas condiciones.
                </p>
                <button onClick={() => navigate('/cliente/historial?tab=Revisiones')} className="px-6 py-2.5 bg-gray-800 hover:bg-gray-900 text-white text-sm font-semibold rounded-xl transition-colors">
                  Agendar Revisión
                </button>
              </>
            )}
          </div>
        </section>

        {/* Historial Reciente */}
        <section>
          <div className="flex items-center justify-between mb-4">
            <h3 className="text-gray-500 font-medium uppercase tracking-wider text-sm">Historial Reciente</h3>
            <button onClick={() => navigate('/cliente/historial?tab=historial')} className="text-blue-600 text-sm font-semibold hover:underline">Ver todo</button>
          </div>
          <div className="bg-white rounded-3xl p-6 shadow-sm border border-gray-100 h-full">
            {data?.recentHistory && data.recentHistory.length > 0 ? (
              <ul className="space-y-5">
                {data.recentHistory.map((hist, index) => (
                  <li key={hist.id} className={`flex items-start gap-4 ${index > 0 ? 'pt-5 border-t border-gray-50' : ''}`}>
                    <div className="w-10 h-10 rounded-full bg-green-50 flex items-center justify-center text-green-600 shrink-0 mt-1">
                      <CheckCircle className="w-5 h-5" />
                    </div>
                    <div>
                      <h4 className="text-gray-800 font-bold text-sm">{hist.title}</h4>
                      <p className="text-gray-500 text-xs mt-1">{hist.description}</p>
                      <p className="text-gray-400 text-xs mt-1 font-medium">{hist.date}</p>
                    </div>
                    <button className="ml-auto p-2 text-gray-400 hover:text-blue-600 transition-colors">
                      <FileText className="w-4 h-4" />
                    </button>
                  </li>
                ))}
              </ul>
            ) : (
              <div className="flex flex-col items-center text-center justify-center h-full text-gray-500">
                <FileText className="w-12 h-12 text-gray-200 mb-2" />
                <p className="text-sm">No hay historial reciente de servicios.</p>
              </div>
            )}
          </div>
        </section>

      </div>
    </div>
  );
}


