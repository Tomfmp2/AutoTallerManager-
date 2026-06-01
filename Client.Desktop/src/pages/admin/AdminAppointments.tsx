import React, { useState, useEffect } from 'react';
import { Search, XCircle, CalendarClock, Clock, Tag } from 'lucide-react';

export default function AdminAppointments() {
  const [appointments, setAppointments] = useState<any[]>([]);
  const [loading, setLoading] = useState(false);

  const fetchAppointments = async () => {
    setLoading(true);
    try {
      const token = localStorage.getItem('token');
      // Fetching all orders, assuming backend returns all if admin
      const res = await fetch(`http://localhost:5219/api/ordenesServicio`, {
        headers: { 'Authorization': `Bearer ${token}` }
      });
      if (res.ok) {
        let data = await res.json();
        if (data.items) data = data.items;
        setAppointments(data);
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

  const handleCancel = async (id: number) => {
    if (!window.confirm(`¿Estás seguro que deseas CANCELAR la Revisión/orden #${id}?`)) return;
    
    try {
      const token = localStorage.getItem('token');
      const res = await fetch(`http://localhost:5219/api/ordenesServicio/${id}/cancel`, {
        method: 'DELETE',
        headers: { 'Authorization': `Bearer ${token}` }
      });
      
      if (res.ok) {
        // Optimistic UI update
        setAppointments(prev => prev.map(a => a.id === id ? {...a, statusName: 'Cancelada'} : a));
      } else {
        alert('Error al cancelar la Revisión.');
      }
    } catch (error) {
      console.error(error);
    }
  };

  return (
    <div className="space-y-6">
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
        <div>
          <h2 className="text-2xl font-black tracking-tight text-gray-900">Revisiones y Ã“rdenes</h2>
          <p className="text-sm text-gray-500 font-medium">Visualiza y cancela las Revisiones agendadas.</p>
        </div>
        
        <button 
          onClick={fetchAppointments}
          className="px-4 py-2 bg-orange-50 text-orange-700 font-bold rounded-xl hover:bg-orange-100 transition-colors"
        >
          Recargar Lista
        </button>
      </div>

      <div className="bg-white rounded-2xl shadow-sm border border-gray-100 overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full text-left text-sm text-gray-500">
            <thead className="bg-gray-50 text-gray-700 text-xs uppercase font-bold">
              <tr>
                <th className="px-6 py-4">Orden</th>
                <th className="px-6 py-4">Vehículo</th>
                <th className="px-6 py-4">Fecha Agendada</th>
                <th className="px-6 py-4">Estado</th>
                <th className="px-6 py-4 text-right">Acciones</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {loading ? (
                <tr><td colSpan={5} className="p-8 text-center">Cargando...</td></tr>
              ) : appointments.length === 0 ? (
                <tr><td colSpan={5} className="p-8 text-center">No hay Revisiones registradas.</td></tr>
              ) : (
                appointments.map(a => (
                  <tr key={a.id} className="hover:bg-gray-50 transition-colors">
                    <td className="px-6 py-4">
                      <div className="flex items-center gap-3">
                        <div className="w-10 h-10 rounded-xl bg-orange-100 flex items-center justify-center text-orange-700">
                          <Tag className="w-5 h-5" />
                        </div>
                        <div>
                          <p className="font-bold text-gray-900 text-base">#{a.id}</p>
                          <p className="text-xs text-gray-400">Servicio {a.serviceTypeName}</p>
                        </div>
                      </div>
                    </td>
                    <td className="px-6 py-4 font-medium text-gray-800">{a.vehicleLicensePlate}</td>
                    <td className="px-6 py-4">
                      <div className="flex items-center gap-2 text-gray-600">
                        <Clock className="w-4 h-4 text-gray-400" />
                        <span>
                          {a.scheduledDate ? new Date(a.scheduledDate).toLocaleString() : 'No agendada'}
                        </span>
                      </div>
                    </td>
                    <td className="px-6 py-4">
                      <span className={`px-2.5 py-1 rounded-full text-xs font-bold inline-flex
                        ${a.statusName === 'Cancelada' ? 'bg-red-100 text-red-700' : 
                          a.statusName === 'Completada' ? 'bg-emerald-100 text-emerald-700' :
                          'bg-blue-100 text-blue-700'}`}>
                        {a.statusName}
                      </span>
                    </td>
                    <td className="px-6 py-4 text-right">
                      {a.statusName !== 'Cancelada' && a.statusName !== 'Completada' && (
                        <button 
                          onClick={() => handleCancel(a.id)}
                          className="p-2 text-red-500 hover:bg-red-50 rounded-lg transition-colors inline-flex"
                          title="Cancelar Revisión"
                        >
                          <XCircle className="w-5 h-5" />
                        </button>
                      )}
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}


