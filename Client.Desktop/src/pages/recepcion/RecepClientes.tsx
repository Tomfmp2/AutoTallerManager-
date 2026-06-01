import React, { useState, useEffect } from 'react';
import { Search, User, Phone, Mail, Calendar, Plus, X, CheckCircle2, AlertCircle, MapPin } from 'lucide-react';

interface ClienteDto {
  id: number;
  nombre: string;
  correo: string;
  telefono: string;
  createdAt: string;
  totalVehiculos: number;
}

export default function RecepClientes() {
  const [clientes, setClientes] = useState<ClienteDto[]>([]);
  const [searchTerm, setSearchTerm] = useState('');
  const [loading, setLoading] = useState(false);

  // Estados para crear cliente
  const [showAddModal, setShowAddModal] = useState(false);
  const [loadingSubmit, setLoadingSubmit] = useState(false);
  const [alertInfo, setAlertInfo] = useState<{type: 'success' | 'error' | null, message: string}>({ type: null, message: '' });
  
  const [newClient, setNewClient] = useState({
    nombre: '',
    correo: '',
    telefono: '',
    direccion: ''
  });

  const fetchClientes = async () => {
    setLoading(true);
    try {
      const token = localStorage.getItem('token');
      const res = await fetch(`http://localhost:5219/api/clientes?PageSize=1000`, {
        headers: { 'Authorization': `Bearer ${token}` }
      });
      if (res.ok) {
        let data = await res.json();
        if (data.value && data.value.items) data = data.value.items;
        else if (data.value) data = data.value;
        else if (data.items) data = data.items;
        
        if (searchTerm) {
          const term = searchTerm.toLowerCase();
          data = data.filter((c: ClienteDto) => 
            c.nombre?.toLowerCase().includes(term) ||
            c.correo?.toLowerCase().includes(term)
          );
        }
        setClientes(Array.isArray(data) ? data : []);
      }
    } catch (e) {
      console.error(e);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchClientes();
  }, [searchTerm]);

  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoadingSubmit(true);
    try {
      const token = localStorage.getItem('token');
      const payload = {
        workshopId: 1, // Default workshop
        nombre: newClient.nombre,
        correo: newClient.correo,
        telefono: newClient.telefono,
        direccion: newClient.direccion || null
      };

      const res = await fetch('http://localhost:5219/api/clientes', {
        method: 'POST',
        headers: { 
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(payload)
      });
      
      if (res.ok) {
        setAlertInfo({ type: 'success', message: 'Cliente registrado exitosamente' });
        setShowAddModal(false);
        setNewClient({ nombre: '', correo: '', telefono: '', direccion: '' });
        fetchClientes();
        setTimeout(() => setAlertInfo({ type: null, message: '' }), 4000);
      } else {
        const contentType = res.headers.get("content-type");
        let errorMsg = 'Error al crear el cliente.';
        if (contentType && contentType.indexOf("application/json") !== -1) {
          try {
            const errorData = await res.json();
            errorMsg = errorData.message || errorData.title || JSON.stringify(errorData) || errorMsg;
          } catch(e) {}
        } else {
          try {
            const text = await res.text();
            if (text && !text.trim().startsWith('<')) {
              errorMsg = text.length > 150 ? text.substring(0, 150) + '...' : text;
            } else if (res.status === 500) {
              errorMsg = "Error interno del servidor (500). Revisa la consola del backend.";
            }
          } catch(e) {}
        }
        setAlertInfo({ type: 'error', message: errorMsg });
        setTimeout(() => setAlertInfo({ type: null, message: '' }), 6000);
      }
    } catch (error: any) {
      setAlertInfo({ type: 'error', message: error?.message || 'Error de conexión' });
      setTimeout(() => setAlertInfo({ type: null, message: '' }), 4000);
    } finally {
      setLoadingSubmit(false);
    }
  };

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
          <h2 className="text-3xl font-black tracking-tight text-gray-900">Directorio de Clientes</h2>
          <p className="text-sm text-gray-500 font-medium mt-1">Consulta y registra clientes en el sistema.</p>
        </div>
        
        <div className="flex flex-col sm:flex-row items-center gap-3">
          <div className="relative w-full sm:w-[350px]">
            <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
              <Search className="h-5 w-5 text-gray-400" />
            </div>
            <input
              type="text"
              placeholder="Buscar por nombre o correo..."
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
            Añadir Cliente
          </button>
        </div>
      </div>

      <div className="bg-white rounded-2xl shadow-sm border border-gray-100 overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full text-left text-sm text-gray-600">
            <thead className="bg-gray-50/80 text-gray-500 text-xs uppercase font-extrabold tracking-wider border-b border-gray-100">
              <tr>
                <th className="px-6 py-5">Cliente</th>
                <th className="px-6 py-5">Contacto</th>
                <th className="px-6 py-5">Registro</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {loading ? (
                <tr>
                  <td colSpan={3} className="p-12 text-center">
                    <div className="inline-block animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
                    <p className="mt-2 text-gray-500 font-medium">Cargando clientes...</p>
                  </td>
                </tr>
              ) : clientes.length === 0 ? (
                <tr>
                  <td colSpan={3} className="p-12 text-center text-gray-500">
                    <User className="w-12 h-12 mx-auto text-gray-300 mb-3" />
                    <p className="font-semibold text-lg text-gray-600">No se encontraron clientes.</p>
                  </td>
                </tr>
              ) : (
                clientes.map(c => (
                  <tr key={c.id} className="hover:bg-blue-50/30 transition-colors group">
                    <td className="px-6 py-4">
                      <div className="flex items-center gap-4">
                        <div className="w-10 h-10 rounded-full bg-blue-100 flex items-center justify-center text-blue-700 font-bold">
                          {c.nombre ? c.nombre[0].toUpperCase() : 'C'}
                        </div>
                        <div>
                          <p className="font-bold text-gray-900">{c.nombre}</p>
                          <p className="text-xs text-gray-400 mt-0.5">ID: {c.id}</p>
                        </div>
                      </div>
                    </td>
                    <td className="px-6 py-4">
                      <div className="space-y-1">
                        <p className="flex items-center gap-2 text-gray-600"><Mail className="w-3.5 h-3.5 text-gray-400" /> {c.correo}</p>
                        <p className="flex items-center gap-2 text-gray-600"><Phone className="w-3.5 h-3.5 text-gray-400" /> {c.telefono || 'Sin teléfono'}</p>
                      </div>
                    </td>
                    <td className="px-6 py-4">
                      <p className="flex items-center gap-2 text-gray-500">
                        <Calendar className="w-4 h-4 text-gray-400" />
                        {new Date(c.createdAt).toLocaleDateString()}
                      </p>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      </div>

      {/* Modal Añadir Cliente */}
      {showAddModal && (
        <div className="fixed inset-0 bg-gray-900/50 backdrop-blur-sm flex items-center justify-center p-4 z-[200]">
          <div className="bg-white rounded-3xl shadow-2xl w-full max-w-lg overflow-hidden flex flex-col max-h-[90vh]">
            <div className="p-6 border-b border-gray-100 flex justify-between items-center bg-gray-50">
              <h3 className="text-2xl font-bold text-gray-800 flex items-center gap-2">
                <User className="w-6 h-6 text-blue-600" /> Registrar Cliente
              </h3>
              <button onClick={() => setShowAddModal(false)} className="p-2 hover:bg-gray-200 rounded-full transition-colors text-gray-500">
                <X className="w-5 h-5" />
              </button>
            </div>
            
            <form onSubmit={handleCreate} className="p-6 overflow-y-auto space-y-5">
              <div className="space-y-2">
                <label className="text-sm font-bold text-gray-700 flex items-center gap-1.5"><User className="w-4 h-4 text-gray-400"/> Nombre Completo *</label>
                <input required type="text" placeholder="Ej: Juan Pérez" value={newClient.nombre} onChange={e => setNewClient({...newClient, nombre: e.target.value})} className="w-full px-4 py-3 rounded-xl border border-gray-200 focus:ring-2 focus:ring-blue-500 outline-none bg-gray-50 focus:bg-white transition-all" />
              </div>
              
              <div className="space-y-2">
                <label className="text-sm font-bold text-gray-700 flex items-center gap-1.5"><Mail className="w-4 h-4 text-gray-400"/> Correo Electrónico *</label>
                <input required type="email" placeholder="ejemplo@correo.com" value={newClient.correo} onChange={e => setNewClient({...newClient, correo: e.target.value})} className="w-full px-4 py-3 rounded-xl border border-gray-200 focus:ring-2 focus:ring-blue-500 outline-none bg-gray-50 focus:bg-white transition-all" />
              </div>

              <div className="space-y-2">
                <label className="text-sm font-bold text-gray-700 flex items-center gap-1.5"><Phone className="w-4 h-4 text-gray-400"/> Teléfono *</label>
                <input required type="tel" placeholder="Ej: +1234567890" value={newClient.telefono} onChange={e => setNewClient({...newClient, telefono: e.target.value})} className="w-full px-4 py-3 rounded-xl border border-gray-200 focus:ring-2 focus:ring-blue-500 outline-none bg-gray-50 focus:bg-white transition-all" />
              </div>

              <div className="space-y-2">
                <label className="text-sm font-bold text-gray-700 flex items-center gap-1.5"><MapPin className="w-4 h-4 text-gray-400"/> Dirección <span className="text-gray-400 font-normal text-xs ml-1">(Opcional)</span></label>
                <textarea rows={2} placeholder="Dirección completa del cliente..." value={newClient.direccion} onChange={e => setNewClient({...newClient, direccion: e.target.value})} className="w-full px-4 py-3 rounded-xl border border-gray-200 focus:ring-2 focus:ring-blue-500 outline-none bg-gray-50 focus:bg-white transition-all resize-none" />
              </div>

              <div className="pt-4 flex justify-end gap-3 border-t border-gray-100">
                <button type="button" onClick={() => setShowAddModal(false)} className="px-5 py-2.5 text-gray-600 font-semibold hover:bg-gray-100 rounded-xl transition-all">
                  Cancelar
                </button>
                <button type="submit" disabled={loadingSubmit} className="px-6 py-2.5 bg-blue-600 hover:bg-blue-700 text-white font-bold rounded-xl shadow-md disabled:opacity-50 transition-all flex items-center gap-2">
                  {loadingSubmit ? (
                    <><div className="w-4 h-4 border-2 border-white/30 border-t-white rounded-full animate-spin"/> Guardando...</>
                  ) : (
                    'Registrar Cliente'
                  )}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
