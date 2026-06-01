import React, { useState, useEffect } from 'react';
import { Search, Car, Calendar, Activity, Plus, X, AlertCircle, CheckCircle2 } from 'lucide-react';

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
}

interface ClienteDto {
  id: number;
  nombre: string;
  correo: string;
  telefono: string;
}

interface Catalogs {
  brands: { id: number; name: string }[];
  models: { id: number; brandId: number; name: string }[];
  colors: { id: number; name: string }[];
}

export default function RecepVehiculos() {
  const [vehicles, setVehicles] = useState<VehiculoDto[]>([]);
  const [clientes, setClientes] = useState<ClienteDto[]>([]);
  const [catalogs, setCatalogs] = useState<Catalogs | null>(null);

  const [loading, setLoading] = useState(false);
  const [loadingSubmit, setLoadingSubmit] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');
  
  const [showAddModal, setShowAddModal] = useState(false);
  const [alertInfo, setAlertInfo] = useState<{type: 'success' | 'error' | null, message: string}>({ type: null, message: '' });

  const [newVehicle, setNewVehicle] = useState({
    placa: '',
    vin: '',
    anio: new Date().getFullYear().toString(),
    kilometraje: '0',
    customerId: '',
    brandId: '',
    modelId: '',
    colorId: '',
    notas: ''
  });

  const fetchData = async () => {
    setLoading(true);
    try {
      const token = localStorage.getItem('token');
      const headers = { 'Authorization': `Bearer ${token}` };

      const [vehRes, cliRes, catRes] = await Promise.all([
        fetch(`http://localhost:5219/api/vehiculos?PageSize=1000`, { headers }),
        fetch(`http://localhost:5219/api/clientes?PageSize=1000`, { headers }),
        fetch(`http://localhost:5219/api/dashboard/client/catalogos`, { headers })
      ]);

      if (vehRes.ok) {
        let data = await vehRes.json();
        if (data.value && data.value.items) data = data.value.items;
        else if (data.value) data = data.value;
        else if (data.items) data = data.items;
        
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
        setVehicles(Array.isArray(data) ? data : []);
      }

      if (cliRes.ok) {
        let data = await cliRes.json();
        if (data.value && data.value.items) data = data.value.items;
        else if (data.value) data = data.value;
        else if (data.items) data = data.items;
        setClientes(Array.isArray(data) ? data : []);
      }

      if (catRes.ok) {
        setCatalogs(await catRes.json());
      }

    } catch (e) {
      console.error(e);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, [searchTerm]);

  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoadingSubmit(true);
    try {
      const token = localStorage.getItem('token');
      const payload = {
        workshopId: 1,
        customerId: Number(newVehicle.customerId),
        modelId: Number(newVehicle.modelId),
        colorId: Number(newVehicle.colorId),
        placa: newVehicle.placa,
        vin: newVehicle.vin,
        anio: Number(newVehicle.anio),
        kilometraje: Number(newVehicle.kilometraje),
        notas: newVehicle.notas
      };

      const res = await fetch('http://localhost:5219/api/vehiculos', {
        method: 'POST',
        headers: { 
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(payload)
      });
      
      if (res.ok) {
        setAlertInfo({ type: 'success', message: 'Vehículo registrado y asignado exitosamente' });
        setShowAddModal(false);
        setNewVehicle({ placa: '', vin: '', anio: new Date().getFullYear().toString(), kilometraje: '0', customerId: '', brandId: '', modelId: '', colorId: '', notas: '' });
        fetchData();
        setTimeout(() => setAlertInfo({ type: null, message: '' }), 4000);
      } else {
        const errorData = await res.json();
        setAlertInfo({ type: 'error', message: errorData.message || 'Error al crear el vehículo. Verifica los datos.' });
        setTimeout(() => setAlertInfo({ type: null, message: '' }), 4000);
      }
    } catch (error) {
      setAlertInfo({ type: 'error', message: 'Error de conexión' });
      setTimeout(() => setAlertInfo({ type: null, message: '' }), 4000);
    } finally {
      setLoadingSubmit(false);
    }
  };

  const filteredModels = catalogs?.models.filter(m => m.brandId === Number(newVehicle.brandId)) ?? [];

  return (
    <div className="space-y-6 relative animate-fade-in p-6 max-w-7xl mx-auto">
      {/* Toast Alert */}
      <div 
        className={`fixed top-6 left-1/2 -translate-x-1/2 z-[100] transition-all duration-500 ease-out flex items-center gap-3 px-5 py-3 rounded-2xl shadow-2xl backdrop-blur-md border ${
          alertInfo.type === 'error' 
            ? 'bg-red-500/10 border-red-500/20 text-red-700 translate-y-0 opacity-100' 
            : alertInfo.type === 'success'
            ? 'bg-green-500/10 border-green-500/20 text-green-700 translate-y-0 opacity-100'
            : '-translate-y-20 opacity-0 pointer-events-none'
        }`}
      >
        {alertInfo.type === 'success' ? <CheckCircle2 className="w-5 h-5" /> : <AlertCircle className="w-5 h-5" />}
        <span className="font-medium">{alertInfo.message}</span>
        <button onClick={() => setAlertInfo({ type: null, message: '' })} className="ml-2 hover:bg-black/5 p-1 rounded-full transition-colors">
          <X className="w-4 h-4" />
        </button>
      </div>

      <div className="flex flex-col md:flex-row md:items-end justify-between gap-4 bg-white p-6 rounded-2xl shadow-sm border border-gray-100">
        <div>
          <h2 className="text-3xl font-black tracking-tight text-gray-900">Directorio Automotor</h2>
          <p className="text-sm text-gray-500 font-medium mt-1">Consulta y registra los vehículos del taller.</p>
        </div>
        
        <div className="flex flex-col sm:flex-row items-center gap-3">
          <div className="relative w-full sm:w-[300px]">
            <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
              <Search className="h-5 w-5 text-gray-400" />
            </div>
            <input
              type="text"
              placeholder="Placa, VIN, Marca..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="block w-full pl-10 pr-4 py-3 border border-gray-200 rounded-xl leading-5 bg-gray-50 placeholder-gray-400 focus:outline-none focus:bg-white focus:ring-2 focus:ring-blue-500 transition-all sm:text-sm"
            />
          </div>
          <button 
            onClick={() => setShowAddModal(true)}
            className="w-full sm:w-auto px-5 py-3 bg-blue-600 hover:bg-blue-700 text-white font-bold rounded-xl flex items-center justify-center gap-2 transition-all shadow-sm shadow-blue-500/30"
          >
            <Plus className="w-5 h-5" />
            Añadir Vehículo
          </button>
        </div>
      </div>

      <div className="bg-white rounded-2xl shadow-sm border border-gray-100 overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full text-left text-sm text-gray-600">
            <thead className="bg-gray-50/80 text-gray-500 text-xs uppercase font-extrabold tracking-wider border-b border-gray-100">
              <tr>
                <th className="px-6 py-5">Identificación del Vehículo</th>
                <th className="px-6 py-5">Especificaciones</th>
                <th className="px-6 py-5">Propietario</th>
                <th className="px-6 py-5 text-center">Órdenes</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {loading ? (
                <tr>
                  <td colSpan={4} className="p-12 text-center">
                    <div className="inline-block animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
                    <p className="mt-2 text-gray-500 font-medium">Cargando vehículos...</p>
                  </td>
                </tr>
              ) : vehicles.length === 0 ? (
                <tr>
                  <td colSpan={4} className="p-12 text-center text-gray-500">
                    <Car className="w-12 h-12 mx-auto text-gray-300 mb-3" />
                    <p className="font-semibold text-lg text-gray-600">No se encontraron vehículos.</p>
                  </td>
                </tr>
              ) : (
                vehicles.map(v => (
                  <tr key={v.id} className="hover:bg-blue-50/30 transition-colors group">
                    <td className="px-6 py-4">
                      <div className="flex items-center gap-4">
                        <div className="w-12 h-12 rounded-xl bg-gradient-to-br from-blue-100 to-indigo-100 flex items-center justify-center text-blue-700 shadow-sm border border-blue-200/50">
                          <Car className="w-6 h-6" />
                        </div>
                        <div>
                          <div className="flex items-center gap-2">
                            <p className="font-black text-gray-900 text-lg uppercase tracking-wider">{v.placa}</p>
                          </div>
                          <p className="text-xs font-mono text-gray-500 bg-gray-50 px-1.5 py-0.5 rounded inline-block mt-1">VIN: {v.vin}</p>
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
                    <td className="px-6 py-4">
                      <span className="font-medium text-gray-700">{v.propietarioActual || 'Sin asignar'}</span>
                    </td>
                    <td className="px-6 py-4 text-center">
                      <span className={`inline-flex items-center justify-center w-8 h-8 rounded-full font-bold text-sm
                        ${v.totalOrdenesServicio > 0 ? 'bg-blue-100 text-blue-700' : 'bg-gray-100 text-gray-500'}`}>
                        {v.totalOrdenesServicio}
                      </span>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      </div>

      {/* Add Vehicle Modal */}
      {showAddModal && (
        <div className="fixed inset-0 bg-gray-900/50 backdrop-blur-sm flex items-center justify-center p-4 z-[200]">
          <div className="bg-white rounded-3xl shadow-2xl w-full max-w-2xl overflow-hidden flex flex-col max-h-[90vh]">
            <div className="p-6 border-b border-gray-100 flex justify-between items-center bg-gray-50">
              <h3 className="text-2xl font-bold text-gray-800 flex items-center gap-2">
                <Car className="w-6 h-6 text-blue-600" /> Registrar Vehículo
              </h3>
              <button onClick={() => setShowAddModal(false)} className="p-2 hover:bg-gray-200 rounded-full transition-colors text-gray-500">
                <X className="w-5 h-5" />
              </button>
            </div>
            
            <form onSubmit={handleCreate} className="p-6 overflow-y-auto space-y-6">
              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                <div className="space-y-2 md:col-span-2">
                  <label className="text-sm font-bold text-gray-700">Propietario (Cliente) *</label>
                  <select required value={newVehicle.customerId} onChange={e => setNewVehicle({...newVehicle, customerId: e.target.value})} className="w-full px-4 py-3 rounded-xl border border-gray-200 focus:ring-2 focus:ring-blue-500 outline-none bg-gray-50 focus:bg-white transition-all">
                    <option value="" disabled>Seleccione un cliente</option>
                    {clientes.map(c => (
                      <option key={c.id} value={c.id}>{c.nombre} ({c.correo})</option>
                    ))}
                  </select>
                </div>

                <div className="space-y-2">
                  <label className="text-sm font-bold text-gray-700">Marca *</label>
                  <select required value={newVehicle.brandId} onChange={e => setNewVehicle({...newVehicle, brandId: e.target.value, modelId: ''})} className="w-full px-4 py-3 rounded-xl border border-gray-200 focus:ring-2 focus:ring-blue-500 outline-none bg-gray-50 focus:bg-white transition-all">
                    <option value="" disabled>Seleccione una marca</option>
                    {catalogs?.brands.map(b => (
                      <option key={b.id} value={b.id}>{b.name}</option>
                    ))}
                  </select>
                </div>
                <div className="space-y-2">
                  <label className="text-sm font-bold text-gray-700">Modelo *</label>
                  <select required disabled={!newVehicle.brandId} value={newVehicle.modelId} onChange={e => setNewVehicle({...newVehicle, modelId: e.target.value})} className="w-full px-4 py-3 rounded-xl border border-gray-200 focus:ring-2 focus:ring-blue-500 outline-none bg-gray-50 focus:bg-white transition-all disabled:opacity-50">
                    <option value="" disabled>Seleccione un modelo</option>
                    {filteredModels.map(m => (
                      <option key={m.id} value={m.id}>{m.name}</option>
                    ))}
                  </select>
                </div>
                <div className="space-y-2">
                  <label className="text-sm font-bold text-gray-700">Color *</label>
                  <select required value={newVehicle.colorId} onChange={e => setNewVehicle({...newVehicle, colorId: e.target.value})} className="w-full px-4 py-3 rounded-xl border border-gray-200 focus:ring-2 focus:ring-blue-500 outline-none bg-gray-50 focus:bg-white transition-all">
                    <option value="" disabled>Seleccione un color</option>
                    {catalogs?.colors.map(c => (
                      <option key={c.id} value={c.id}>{c.name}</option>
                    ))}
                  </select>
                </div>

                <div className="space-y-2">
                  <label className="text-sm font-bold text-gray-700">Placa *</label>
                  <input required type="text" maxLength={6} value={newVehicle.placa} onChange={e => setNewVehicle({...newVehicle, placa: e.target.value.toUpperCase()})} className="w-full px-4 py-3 rounded-xl border border-gray-200 focus:ring-2 focus:ring-blue-500 outline-none bg-gray-50 focus:bg-white uppercase" placeholder="Ej: ABC123" />
                </div>
                <div className="space-y-2 md:col-span-2">
                  <label className="text-sm font-bold text-gray-700">VIN (Opcional)</label>
                  <input type="text" maxLength={17} value={newVehicle.vin} onChange={e => setNewVehicle({...newVehicle, vin: e.target.value.toUpperCase()})} className="w-full px-4 py-3 rounded-xl border border-gray-200 focus:ring-2 focus:ring-blue-500 outline-none bg-gray-50 focus:bg-white uppercase" placeholder="17 caracteres alfanuméricos" />
                </div>
                <div className="space-y-2">
                  <label className="text-sm font-bold text-gray-700">Año *</label>
                  <input required type="number" min="1900" max={new Date().getFullYear() + 1} value={newVehicle.anio} onChange={e => setNewVehicle({...newVehicle, anio: e.target.value})} className="w-full px-4 py-3 rounded-xl border border-gray-200 focus:ring-2 focus:ring-blue-500 outline-none bg-gray-50 focus:bg-white" placeholder="2020" />
                </div>
                <div className="space-y-2">
                  <label className="text-sm font-bold text-gray-700">Kilometraje *</label>
                  <input required type="number" min="0" value={newVehicle.kilometraje} onChange={e => setNewVehicle({...newVehicle, kilometraje: e.target.value})} className="w-full px-4 py-3 rounded-xl border border-gray-200 focus:ring-2 focus:ring-blue-500 outline-none bg-gray-50 focus:bg-white" placeholder="0" />
                </div>
                <div className="space-y-2 md:col-span-2">
                  <label className="text-sm font-bold text-gray-700">Notas Adicionales</label>
                  <textarea value={newVehicle.notas} onChange={e => setNewVehicle({...newVehicle, notas: e.target.value})} className="w-full px-4 py-3 rounded-xl border border-gray-200 focus:ring-2 focus:ring-blue-500 outline-none bg-gray-50 focus:bg-white resize-none" placeholder="Observaciones sobre el estado del vehículo..." rows={3} />
                </div>
              </div>
              <div className="pt-6 border-t border-gray-100 flex justify-end gap-3">
                <button type="button" onClick={() => setShowAddModal(false)} className="px-6 py-3 bg-gray-100 hover:bg-gray-200 text-gray-800 font-bold rounded-xl transition-colors">
                  Cancelar
                </button>
                <button type="submit" disabled={loadingSubmit || !newVehicle.customerId} className="px-6 py-3 bg-blue-600 hover:bg-blue-700 disabled:opacity-50 text-white font-bold rounded-xl transition-colors shadow-lg shadow-blue-500/30 flex items-center gap-2">
                  {loadingSubmit ? <div className="w-5 h-5 border-2 border-white/30 border-t-white rounded-full animate-spin" /> : <Plus className="w-5 h-5" />}
                  {loadingSubmit ? 'Guardando...' : 'Guardar y Asignar'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
