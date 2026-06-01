import React, { useState, useEffect } from 'react';
import { Search, Trash2, Car, Calendar, Hash, Info, User, Activity, PenTool } from 'lucide-react';

interface VehiculoDto {
  id: number;
  placa: string;
  vin: string;
  anio: number;
  kilometraje: number;
  marca: string;
  modelo: string;
  color: string;
  propietarioActual: string | null;
  totalOrdenesServicio: number;
  createdAt: string;
}

export default function AdminVehicles() {
  const [vehicles, setVehicles] = useState<VehiculoDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');

  const fetchVehicles = async () => {
    setLoading(true);
    try {
      const token = localStorage.getItem('token');
      const res = await fetch(`http://localhost:5219/api/vehiculos`, {
        headers: { 'Authorization': `Bearer ${token}` }
      });
      if (res.ok) {
        let data = await res.json();
        if (data.items) data = data.items;
        
        if (searchTerm) {
          const term = searchTerm.toLowerCase();
          data = data.filter((v: VehiculoDto) => 
            v.placa?.toLowerCase().includes(term) ||
            v.vin?.toLowerCase().includes(term) ||
            v.marca?.toLowerCase().includes(term) ||
            v.modelo?.toLowerCase().includes(term) ||
            v.propietarioActual?.toLowerCase().includes(term)
          );
        }
        setVehicles(data);
      }
    } catch (e) {
      console.error(e);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchVehicles();
  }, []);

  const handleDelete = async (id: number, placa: string) => {
    if (!window.confirm(`¿Estás seguro que deseas ELIMINAR el vehículo con placa ${placa}? Esta acción lo ocultará del sistema.`)) return;
    
    try {
      const token = localStorage.getItem('token');
      const res = await fetch(`http://localhost:5219/api/vehiculos/${id}`, {
        method: 'DELETE',
        headers: { 'Authorization': `Bearer ${token}` }
      });
      
      if (res.ok) {
        setVehicles(prev => prev.filter(v => v.id !== id));
      } else {
        alert('Error al eliminar el vehículo.');
      }
    } catch (error) {
      console.error(error);
    }
  };

  return (
    <div className="space-y-6">
      <div className="flex flex-col md:flex-row md:items-end justify-between gap-4 bg-white p-6 rounded-2xl shadow-sm border border-gray-100">
        <div>
          <h2 className="text-3xl font-black tracking-tight text-gray-900">Directorio Automotor</h2>
          <p className="text-sm text-gray-500 font-medium mt-1">Supervisa todos los vehículos registrados, su historial y dueños.</p>
        </div>
        
        <div className="relative w-full md:w-[400px]">
          <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
            <Search className="h-5 w-5 text-gray-400" />
          </div>
          <input
            type="text"
            placeholder="Placa, VIN, Marca, Dueño..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            onKeyDown={(e) => e.key === 'Enter' && fetchVehicles()}
            className="block w-full pl-10 pr-24 py-3 border border-gray-200 rounded-xl leading-5 bg-gray-50 placeholder-gray-400 focus:outline-none focus:bg-white focus:ring-2 focus:ring-emerald-500 focus:border-emerald-500 transition-all sm:text-sm"
          />
          <button 
            onClick={fetchVehicles}
            className="absolute inset-y-1.5 right-1.5 px-4 bg-emerald-600 text-white text-sm font-bold rounded-lg hover:bg-emerald-700 transition-colors shadow-sm"
          >
            Buscar
          </button>
        </div>
      </div>

      <div className="bg-white rounded-2xl shadow-sm border border-gray-100 overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full text-left text-sm text-gray-600">
            <thead className="bg-gray-50/80 text-gray-500 text-xs uppercase font-extrabold tracking-wider border-b border-gray-100">
              <tr>
                <th className="px-6 py-5">Identificación del Vehículo</th>
                <th className="px-6 py-5">Propietario / Cliente</th>
                <th className="px-6 py-5">Especificaciones</th>
                <th className="px-6 py-5 text-center">Órdenes</th>
                <th className="px-6 py-5 text-right">Acciones</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {loading ? (
                <tr>
                  <td colSpan={5} className="p-12 text-center">
                    <div className="inline-block animate-spin rounded-full h-8 w-8 border-b-2 border-emerald-600"></div>
                    <p className="mt-2 text-gray-500 font-medium">Cargando vehículos...</p>
                  </td>
                </tr>
              ) : vehicles.length === 0 ? (
                <tr>
                  <td colSpan={5} className="p-12 text-center text-gray-500">
                    <Car className="w-12 h-12 mx-auto text-gray-300 mb-3" />
                    <p className="font-semibold text-lg text-gray-600">No se encontraron vehículos.</p>
                    <p className="text-sm">Intenta con otros términos de búsqueda.</p>
                  </td>
                </tr>
              ) : (
                vehicles.map(v => (
                  <tr key={v.id} className="hover:bg-emerald-50/30 transition-colors group">
                    <td className="px-6 py-4">
                      <div className="flex items-center gap-4">
                        <div className="w-12 h-12 rounded-xl bg-gradient-to-br from-emerald-100 to-teal-100 flex items-center justify-center text-emerald-700 shadow-sm border border-emerald-200/50">
                          <Car className="w-6 h-6" />
                        </div>
                        <div>
                          <div className="flex items-center gap-2">
                            <p className="font-black text-gray-900 text-lg uppercase tracking-wider">{v.placa}</p>
                            <span className="px-2 py-0.5 rounded text-[10px] font-bold bg-gray-100 text-gray-500 border border-gray-200">
                              ID: {v.id}
                            </span>
                          </div>
                          <p className="text-xs font-mono text-gray-500 bg-gray-50 px-1.5 py-0.5 rounded inline-block mt-1">VIN: {v.vin}</p>
                        </div>
                      </div>
                    </td>
                    <td className="px-6 py-4">
                      <div className="flex items-center gap-2">
                        <div className="w-8 h-8 rounded-full bg-indigo-50 flex items-center justify-center text-indigo-600">
                          <User className="w-4 h-4" />
                        </div>
                        <div>
                          <p className="font-bold text-gray-800">{v.propietarioActual || 'Sin Asignar'}</p>
                          <p className="text-xs text-gray-400">Dueño Registrado</p>
                        </div>
                      </div>
                    </td>
                    <td className="px-6 py-4">
                      <p className="font-bold text-gray-800">{v.marca} {v.modelo}</p>
                      <p className="text-xs text-gray-500 flex items-center gap-1.5 mt-0.5">
                        <Calendar className="w-3.5 h-3.5" /> {v.anio} &bull; 
                        <Activity className="w-3.5 h-3.5 ml-1" /> {v.kilometraje.toLocaleString()} km
                      </p>
                    </td>
                    <td className="px-6 py-4 text-center">
                      <span className={`inline-flex items-center justify-center w-8 h-8 rounded-full font-bold text-sm
                        ${v.totalOrdenesServicio > 0 ? 'bg-emerald-100 text-emerald-700' : 'bg-gray-100 text-gray-500'}`}>
                        {v.totalOrdenesServicio}
                      </span>
                    </td>
                    <td className="px-6 py-4 text-right">
                      <div className="flex items-center justify-end gap-2 opacity-0 group-hover:opacity-100 transition-opacity">
                        <button 
                          className="p-2 text-indigo-600 hover:bg-indigo-50 rounded-lg transition-colors tooltip"
                          title="Ver Detalles (Próximamente)"
                        >
                          <Info className="w-5 h-5" />
                        </button>
                        <button 
                          onClick={() => handleDelete(v.id, v.placa)}
                          className="p-2 text-red-500 hover:bg-red-50 hover:text-red-600 rounded-lg transition-colors tooltip"
                          title="Dar de Baja"
                        >
                          <Trash2 className="w-5 h-5" />
                        </button>
                      </div>
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
