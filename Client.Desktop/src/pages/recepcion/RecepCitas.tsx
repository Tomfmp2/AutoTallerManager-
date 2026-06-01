import React, { useState, useEffect } from 'react';
import { Tag, Clock, Settings, Search, Plus } from 'lucide-react';
import AgendarCitaRecepModal from '../../components/AgendarCitaRecepModal';

export default function RecepCitas() {
  const [appointments, setAppointments] = useState<any[]>([]);
  const [loading, setLoading] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');
  const [isModalOpen, setIsModalOpen] = useState(false);

  const fetchAppointments = async () => {
    setLoading(true);
    try {
      const token = localStorage.getItem('token');
      const res = await fetch(`http://localhost:5219/api/ordenesServicio?PageSize=1000`, {
        headers: { 'Authorization': `Bearer ${token}` }
      });
      if (res.ok) {
        let data = await res.json();
        if (data.value && data.value.items) data = data.value.items;
        else if (data.value) data = data.value;
        else if (data.items) data = data.items;
        
        if (searchTerm) {
          const term = searchTerm.toLowerCase();
          data = data.filter((a: any) => 
            a.vehiclePlaca?.toLowerCase().includes(term) ||
            a.clienteNombre?.toLowerCase().includes(term) ||
            a.numeroOrden?.toLowerCase().includes(term)
          );
        }
        
        setAppointments(Array.isArray(data) ? data : []);
      }
    } catch (e) {
      console.error(e);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchAppointments();
  }, []);

  const getStatusStyle = (statusId: number | string) => {
    const status = String(statusId);
    if (status === '1' || status === 'Programada') return 'bg-yellow-100 text-yellow-700';
    if (status === 'EnProceso' || status === '3') return 'bg-green-100 text-green-700';
    if (status === 'Completada' || status === '4' || status === 'Finalizada') return 'bg-emerald-100 text-emerald-700';
    if (status === 'Cancelada' || status === '5') return 'bg-red-100 text-red-700';
    return 'bg-blue-100 text-blue-700';
  };
  
  const getStatusLabel = (statusId: number | string) => {
    const status = String(statusId);
    if (status === '1') return 'Programada';
    if (status === '2') return 'Revisión (Diagnóstico)';
    if (status === '3') return 'En Reparación';
    if (status === '4') return 'Completada';
    if (status === '5') return 'Esperando Pago';
    if (status === '6') return 'Finalizada';
    return status;
  };

  return (
    <div className="space-y-6 animate-fade-in p-6 max-w-7xl mx-auto">
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4 bg-white p-6 rounded-2xl shadow-sm border border-gray-100">
        <div>
          <h2 className="text-3xl font-black tracking-tight text-gray-900">Historial de Revisiones</h2>
          <p className="text-sm text-gray-500 font-medium mt-1">Visualiza todas las Revisiones y órdenes de servicio.</p>
        </div>
        
        <div className="flex items-center gap-4 w-full md:w-auto">
          <div className="relative flex-1 md:w-[350px]">
            <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
              <Search className="h-5 w-5 text-gray-400" />
            </div>
            <input
              type="text"
              placeholder="Buscar por placa, cliente..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              onKeyDown={(e) => e.key === 'Enter' && fetchAppointments()}
              className="block w-full pl-10 pr-4 py-3 border border-gray-200 rounded-xl leading-5 bg-gray-50 placeholder-gray-400 focus:outline-none focus:bg-white focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-all sm:text-sm"
            />
          </div>
          <button 
            onClick={() => setIsModalOpen(true)}
            className="flex items-center gap-2 px-5 py-3 bg-blue-600 text-white text-sm font-bold rounded-xl hover:bg-blue-700 transition-colors shadow-md shadow-blue-500/20 whitespace-nowrap"
          >
            <Plus className="w-5 h-5" /> Nueva Revisión
          </button>
        </div>
      </div>

      <div className="bg-white rounded-2xl shadow-sm border border-gray-100 overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full text-left text-sm text-gray-500">
            <thead className="bg-gray-50 text-gray-700 text-xs uppercase font-bold border-b border-gray-100">
              <tr>
                <th className="px-6 py-5">Orden</th>
                <th className="px-6 py-5">Cliente y Vehículo</th>
                <th className="px-6 py-5">Fecha</th>
                <th className="px-6 py-5">Mecánico Asignado</th>
                <th className="px-6 py-5">Estado</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {loading ? (
                <tr>
                  <td colSpan={5} className="p-12 text-center">
                    <div className="inline-block animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
                    <p className="mt-2 text-gray-500 font-medium">Cargando Revisiones...</p>
                  </td>
                </tr>
              ) : appointments.length === 0 ? (
                <tr><td colSpan={5} className="p-8 text-center">No hay Revisiones registradas.</td></tr>
              ) : (
                appointments.map(a => (
                  <tr key={a.id} className="hover:bg-gray-50 transition-colors">
                    <td className="px-6 py-4">
                      <div className="flex items-center gap-3">
                        <div className="w-10 h-10 rounded-xl bg-blue-50 flex items-center justify-center text-blue-700 border border-blue-100">
                          <Tag className="w-5 h-5" />
                        </div>
                        <div>
                          <p className="font-bold text-gray-900 text-base">{a.numeroOrden}</p>
                          <p className="text-xs text-gray-500">{a.tipoServicio || 'Mantenimiento'}</p>
                        </div>
                      </div>
                    </td>
                    <td className="px-6 py-4">
                      <p className="font-bold text-gray-800">{a.clienteNombre}</p>
                      <p className="text-xs font-mono bg-gray-100 px-1.5 py-0.5 rounded inline-block mt-1 tracking-wider text-gray-600">
                        {a.vehiclePlaca}
                      </p>
                    </td>
                    <td className="px-6 py-4">
                      <div className="flex items-center gap-2 text-gray-600">
                        <Clock className="w-4 h-4 text-gray-400" />
                        <span>
                          {a.fechaIngreso ? new Date(a.fechaIngreso).toLocaleString() : 'No agendada'}
                        </span>
                      </div>
                    </td>
                    <td className="px-6 py-4">
                      <div className="flex items-center gap-2 text-gray-600">
                        <Settings className="w-4 h-4 text-gray-400" />
                        <span className="font-medium text-gray-700">
                          {a.mecanicoNombre || 'Sin asignar'}
                        </span>
                      </div>
                    </td>
                    <td className="px-6 py-4">
                      <span className={`px-3 py-1 rounded-full text-xs font-bold inline-flex ${getStatusStyle(a.estado)}`}>
                        {getStatusLabel(a.estado)}
                      </span>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      </div>
      
      <AgendarCitaRecepModal
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        onSuccess={() => {
          fetchAppointments();
          setIsModalOpen(false);
        }}
      />
    </div>
  );
}


