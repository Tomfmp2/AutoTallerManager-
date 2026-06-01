import React, { useEffect, useState } from 'react';
import { Search, Package, Plus, Edit2, Trash2, Box, Tag, DollarSign, AlertCircle, X, CheckCircle2, Filter, ArrowDownUp } from 'lucide-react';

interface PartDto {
  id: number;
  codigo: string;
  descripcion: string;
  precioUnitario: number;
  partCategoryId: number;
  categoria: string;
  stockActual: number;
  stockDisponible: boolean;
  totalUtilizaciones: number;
  createdAt: string;
}

type AlertType = 'error' | 'success' | null;

export const AdminInventory: React.FC = () => {
  const [parts, setParts] = useState<PartDto[]>([]);
  const [searchTerm, setSearchTerm] = useState('');
  const [loading, setLoading] = useState(false);
  const [alertInfo, setAlertInfo] = useState<{ type: AlertType; message: string }>({ type: null, message: '' });
  const [partToDelete, setPartToDelete] = useState<number | null>(null);
  const [selectedCategory, setSelectedCategory] = useState<string>('Todas');
  const [sortBy, setSortBy] = useState<string>('Recientes');
  
  const uniqueCategories = ['Todas', ...Array.from(new Set(parts.map(p => p.categoria || 'Variedad')))];

  useEffect(() => {
    fetchParts();
  }, []);

  useEffect(() => {
    if (alertInfo.type) {
      const timer = setTimeout(() => {
        setAlertInfo({ type: null, message: '' });
      }, 5000);
      return () => clearTimeout(timer);
    }
  }, [alertInfo]);

  const fetchParts = async () => {
    setLoading(true);
    try {
      const token = localStorage.getItem('token');
      const res = await fetch('http://localhost:5219/api/repuestos?pageSize=1000', {
        headers: { 'Authorization': `Bearer ${token}` }
      });
      if (res.ok) {
        const data = await res.json();
        if (data.items && Array.isArray(data.items)) {
          setParts(data.items);
        } else if (Array.isArray(data)) {
          setParts(data);
        } else {
          setParts([]);
        }
      } else {
        setAlertInfo({ type: 'error', message: 'Error al cargar el inventario' });
      }
    } catch (error) {
      setAlertInfo({ type: 'error', message: 'Error de conexión con el servidor' });
    } finally {
      setLoading(false);
    }
  };

  const executeDelete = async (id: number) => {
    setPartToDelete(null);
    try {
      const token = localStorage.getItem('token');
      const res = await fetch(`http://localhost:5219/api/repuestos/${id}`, {
        method: 'DELETE',
        headers: { 'Authorization': `Bearer ${token}` }
      });
      
      if (res.ok) {
        setAlertInfo({ type: 'success', message: 'Repuesto eliminado con éxito' });
        fetchParts();
      } else {
        setAlertInfo({ type: 'error', message: 'No se pudo eliminar el repuesto (¿está en uso?)' });
      }
    } catch (error) {
      setAlertInfo({ type: 'error', message: 'Error de conexión al eliminar' });
    }
  };

  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat('es-CO', { style: 'currency', currency: 'COP', maximumFractionDigits: 0 }).format(amount);
  };

  const [showAddModal, setShowAddModal] = useState(false);
  const [newPart, setNewPart] = useState({
    codigo: '',
    descripcion: '',
    precioUnitario: '',
    stockInicial: '',
    stockMinimo: '5',
    marca: '',
    ubicacion: '',
    categoria: ''
  });

  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    try {
      const token = localStorage.getItem('token');
      
      const categoryMap = new Map<string, number>();
      parts.forEach(p => {
        if (p.categoria && p.partCategoryId) {
          categoryMap.set(p.categoria, p.partCategoryId);
        }
      });
      
      const partCategoryId = categoryMap.get(newPart.categoria) || 1;

      const payload = {
        workshopId: 1,
        partCategoryId,
        codigo: newPart.codigo,
        descripcion: newPart.descripcion,
        precioUnitario: Number(newPart.precioUnitario),
        stockInicial: Number(newPart.stockInicial),
        stockMinimo: Number(newPart.stockMinimo),
        marca: newPart.marca,
        ubicacion: newPart.ubicacion
      };

      const res = await fetch('http://localhost:5219/api/repuestos', {
        method: 'POST',
        headers: { 
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(payload)
      });
      
      if (res.ok) {
        setAlertInfo({ type: 'success', message: 'Producto creado exitosamente' });
        setShowAddModal(false);
        setNewPart({ codigo: '', descripcion: '', precioUnitario: '', stockInicial: '', stockMinimo: '5', marca: '', ubicacion: '', categoria: '' });
        fetchParts();
      } else {
        setAlertInfo({ type: 'error', message: 'No se pudo crear el producto. Revisa los datos.' });
      }
    } catch (error) {
      setAlertInfo({ type: 'error', message: 'Error de conexión al crear' });
    } finally {
      setLoading(false);
    }
  };

  const filteredParts = parts.filter(p => {
    const matchesSearch = (p.descripcion || '').toLowerCase().includes(searchTerm.toLowerCase()) ||
                          (p.codigo || '').toLowerCase().includes(searchTerm.toLowerCase()) ||
                          (p.categoria || '').toLowerCase().includes(searchTerm.toLowerCase());
    const matchesCategory = selectedCategory === 'Todas' || (p.categoria || 'Variedad') === selectedCategory;
    return matchesSearch && matchesCategory;
  }).sort((a, b) => {
    if (sortBy === 'Precio Alto') return b.precioUnitario - a.precioUnitario;
    if (sortBy === 'Precio Bajo') return a.precioUnitario - b.precioUnitario;
    if (sortBy === 'Recientes') return new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime();
    return 0;
  });

  return (
    <div className="space-y-6 relative">
      {/* ALERTA FLOTANTE (Toast) */}
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

      <div className="flex justify-between items-end">
        <div>
          <h1 className="text-3xl font-black text-gray-900 tracking-tight flex items-center gap-3">
            <div className="p-3 bg-blue-600 rounded-2xl text-white shadow-lg shadow-blue-500/30">
              <Package className="w-8 h-8" />
            </div>
            Inventario y Repuestos
          </h1>
          <p className="text-gray-500 mt-2 ml-1 font-medium">Gestiona el catálogo de productos, precios y stock del taller</p>
        </div>
        
        <button 
          onClick={() => setShowAddModal(true)}
          className="bg-blue-600 hover:bg-blue-700 text-white px-6 py-3 rounded-xl font-bold flex items-center gap-2 transition-all hover:scale-105 shadow-lg shadow-blue-500/30"
        >
          <Plus className="w-5 h-5" />
          Añadir Producto
        </button>
      </div>

      {/* Stats Cards */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-6">
        <div className="bg-white rounded-2xl p-6 shadow-sm border border-gray-100 flex items-center gap-4">
          <div className="p-4 bg-blue-50 text-blue-600 rounded-2xl"><Box className="w-6 h-6" /></div>
          <div>
            <p className="text-sm text-gray-500 font-medium">Total Productos</p>
            <p className="text-2xl font-bold text-gray-900">{parts.length}</p>
          </div>
        </div>
        <div className="bg-white rounded-2xl p-6 shadow-sm border border-gray-100 flex items-center gap-4">
          <div className="p-4 bg-emerald-50 text-emerald-600 rounded-2xl"><Tag className="w-6 h-6" /></div>
          <div>
            <p className="text-sm text-gray-500 font-medium">Categorías</p>
            <p className="text-2xl font-bold text-gray-900">{new Set(parts.map(p => p.categoria || 'Variedad')).size}</p>
          </div>
        </div>
        <div className="bg-white rounded-2xl p-6 shadow-sm border border-gray-100 flex items-center gap-4">
          <div className="p-4 bg-amber-50 text-amber-600 rounded-2xl"><AlertCircle className="w-6 h-6" /></div>
          <div>
            <p className="text-sm text-gray-500 font-medium">Bajo Stock</p>
            <p className="text-2xl font-bold text-gray-900">{parts.filter(p => p.stockActual < 10).length}</p>
          </div>
        </div>
        <div className="bg-white rounded-2xl p-6 shadow-sm border border-gray-100 flex items-center gap-4">
          <div className="p-4 bg-purple-50 text-purple-600 rounded-2xl"><DollarSign className="w-6 h-6" /></div>
          <div>
            <p className="text-sm text-gray-500 font-medium">Valor Inventario</p>
            <p className="text-2xl font-bold text-gray-900">{formatCurrency(parts.reduce((sum, p) => sum + (p.precioUnitario * p.stockActual), 0))}</p>
          </div>
        </div>
      </div>

      {/* Table Section */}
      <div className="bg-white rounded-3xl shadow-sm border border-gray-100 overflow-hidden">
        <div className="p-6 border-b border-gray-100 bg-gray-50/50 flex flex-col md:flex-row gap-4 justify-between items-center">
          <div className="relative w-full md:w-96">
            <Search className="w-5 h-5 absolute left-4 top-1/2 -translate-y-1/2 text-gray-400" />
            <input 
              type="text" 
              placeholder="Buscar por código o descripción..." 
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="w-full pl-12 pr-4 py-3 bg-white border border-gray-200 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-blue-500 outline-none transition-all shadow-sm font-medium"
            />
          </div>
          <div className="flex gap-3 w-full md:w-auto">
            <div className="relative">
              <Filter className="w-4 h-4 absolute left-4 top-1/2 -translate-y-1/2 text-gray-400" />
              <select
                value={selectedCategory}
                onChange={(e) => setSelectedCategory(e.target.value)}
                className="pl-10 pr-8 py-3 bg-white border border-gray-200 rounded-xl focus:ring-2 focus:ring-blue-500 outline-none appearance-none font-medium text-gray-700 w-full cursor-pointer shadow-sm"
              >
                {uniqueCategories.map(cat => (
                  <option key={cat} value={cat}>{cat}</option>
                ))}
              </select>
            </div>
            <div className="relative">
              <ArrowDownUp className="w-4 h-4 absolute left-4 top-1/2 -translate-y-1/2 text-gray-400" />
              <select
                value={sortBy}
                onChange={(e) => setSortBy(e.target.value)}
                className="pl-10 pr-8 py-3 bg-white border border-gray-200 rounded-xl focus:ring-2 focus:ring-blue-500 outline-none appearance-none font-medium text-gray-700 w-full cursor-pointer shadow-sm"
              >
                <option value="Recientes">Más Recientes</option>
                <option value="Precio Alto">Mayor Precio</option>
                <option value="Precio Bajo">Menor Precio</option>
              </select>
            </div>
          </div>
        </div>

        <div className="overflow-x-auto">
          {loading ? (
            <div className="flex justify-center items-center p-12">
              <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
            </div>
          ) : (
            <table className="w-full text-left text-sm text-gray-500">
              <thead className="text-xs text-gray-400 uppercase bg-gray-50/80 font-bold tracking-wider">
                <tr>
                  <th className="px-6 py-4">Código</th>
                  <th className="px-6 py-4">Descripción</th>
                  <th className="px-6 py-4">Categoría</th>
                  <th className="px-6 py-4">Precio Und.</th>
                  <th className="px-6 py-4 text-center">Stock</th>
                  <th className="px-6 py-4 text-right">Acciones</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-100">
                {filteredParts.map((p) => (
                  <tr key={p.id} className="hover:bg-blue-50/30 transition-colors group">
                    <td className="px-6 py-4 font-mono text-gray-500 font-medium">
                      {p.codigo}
                    </td>
                    <td className="px-6 py-4 font-bold text-gray-900">
                      {p.descripcion}
                    </td>
                    <td className="px-6 py-4">
                      <span className="px-3 py-1 bg-gray-100 text-gray-600 rounded-full text-xs font-bold border border-gray-200">
                        {p.categoria}
                      </span>
                    </td>
                    <td className="px-6 py-4 font-semibold text-gray-700">
                      {formatCurrency(p.precioUnitario)}
                    </td>
                    <td className="px-6 py-4 text-center">
                      <span className={`px-3 py-1 rounded-full text-xs font-bold inline-flex items-center gap-1 ${
                        p.stockActual > 15 ? 'bg-green-100 text-green-700' : 
                        p.stockActual > 5 ? 'bg-amber-100 text-amber-700' : 'bg-red-100 text-red-700 animate-pulse'
                      }`}>
                        {p.stockActual} und
                      </span>
                    </td>
                    <td className="px-6 py-4 text-right">
                      <div className="flex justify-end gap-2 opacity-0 group-hover:opacity-100 transition-opacity">
                        <button 
                          className="p-2 text-blue-500 hover:bg-blue-50 rounded-lg transition-colors"
                          title="Editar producto"
                        >
                          <Edit2 className="w-4 h-4" />
                        </button>
                        <button 
                          onClick={() => setPartToDelete(p.id)}
                          className="p-2 text-red-500 hover:bg-red-50 rounded-lg transition-colors"
                          title="Eliminar producto"
                        >
                          <Trash2 className="w-4 h-4" />
                        </button>
                      </div>
                    </td>
                  </tr>
                ))}
                {filteredParts.length === 0 && (
                  <tr>
                    <td colSpan={6} className="px-6 py-12 text-center text-gray-500">
                      <Package className="w-12 h-12 mx-auto mb-4 text-gray-300" />
                      <p className="font-medium text-lg">No se encontraron repuestos</p>
                      <p className="text-sm">Prueba con otros términos de búsqueda.</p>
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          )}
        </div>
      </div>

      {/* Delete Confirmation Modal */}
      {partToDelete !== null && (
        <div className="fixed inset-0 bg-gray-900/50 backdrop-blur-sm flex items-center justify-center p-4 z-[200]">
          <div className="bg-white rounded-3xl shadow-2xl w-full max-w-sm overflow-hidden p-6 text-center space-y-4">
            <div className="w-16 h-16 bg-red-100 text-red-500 rounded-full flex items-center justify-center mx-auto mb-4">
              <Trash2 className="w-8 h-8" />
            </div>
            <h3 className="text-xl font-bold text-gray-800">Eliminar Producto</h3>
            <p className="text-gray-500 font-medium">
              ¿Estás seguro de que deseas eliminar este producto de forma permanente?
            </p>
            <div className="flex gap-3 pt-4">
              <button 
                onClick={() => setPartToDelete(null)}
                className="flex-1 py-3 px-4 bg-gray-100 hover:bg-gray-200 text-gray-800 font-bold rounded-xl transition-colors"
              >
                Cancelar
              </button>
              <button 
                onClick={() => executeDelete(partToDelete)}
                className="flex-1 py-3 px-4 bg-red-500 hover:bg-red-600 text-white font-bold rounded-xl transition-colors shadow-lg shadow-red-500/30"
              >
                Eliminar
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Add Product Modal */}
      {showAddModal && (
        <div className="fixed inset-0 bg-gray-900/50 backdrop-blur-sm flex items-center justify-center p-4 z-[200]">
          <div className="bg-white rounded-3xl shadow-2xl w-full max-w-2xl overflow-hidden flex flex-col max-h-[90vh]">
            <div className="p-6 border-b border-gray-100 flex justify-between items-center bg-gray-50">
              <h3 className="text-2xl font-bold text-gray-800 flex items-center gap-2">
                <Package className="w-6 h-6 text-blue-600" /> Nuevo Producto
              </h3>
              <button onClick={() => setShowAddModal(false)} className="p-2 hover:bg-gray-200 rounded-full transition-colors text-gray-500">
                <X className="w-5 h-5" />
              </button>
            </div>
            
            <form onSubmit={handleCreate} className="p-6 overflow-y-auto space-y-6">
              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                <div className="space-y-2">
                  <label className="text-sm font-bold text-gray-700">Código / SKU *</label>
                  <input required type="text" value={newPart.codigo} onChange={e => setNewPart({...newPart, codigo: e.target.value})} className="w-full px-4 py-3 rounded-xl border border-gray-200 focus:ring-2 focus:ring-blue-500 focus:border-blue-500 outline-none transition-all bg-gray-50 focus:bg-white" placeholder="Ej: BRK-001" />
                </div>
                <div className="space-y-2">
                  <label className="text-sm font-bold text-gray-700">Categoría *</label>
                  <select required value={newPart.categoria} onChange={e => setNewPart({...newPart, categoria: e.target.value})} className="w-full px-4 py-3 rounded-xl border border-gray-200 focus:ring-2 focus:ring-blue-500 outline-none bg-gray-50 focus:bg-white">
                    <option value="" disabled>Seleccione una categoría</option>
                    {uniqueCategories.filter(c => c !== 'Todas').map(cat => (
                      <option key={cat} value={cat}>{cat}</option>
                    ))}
                  </select>
                </div>
                <div className="space-y-2 md:col-span-2">
                  <label className="text-sm font-bold text-gray-700">Descripción *</label>
                  <input required type="text" value={newPart.descripcion} onChange={e => setNewPart({...newPart, descripcion: e.target.value})} className="w-full px-4 py-3 rounded-xl border border-gray-200 focus:ring-2 focus:ring-blue-500 outline-none bg-gray-50 focus:bg-white" placeholder="Nombre y detalles del repuesto" />
                </div>
                <div className="space-y-2">
                  <label className="text-sm font-bold text-gray-700">Precio Unitario (COP) *</label>
                  <input required type="number" min="0" step="100" value={newPart.precioUnitario} onChange={e => setNewPart({...newPart, precioUnitario: e.target.value})} className="w-full px-4 py-3 rounded-xl border border-gray-200 focus:ring-2 focus:ring-blue-500 outline-none bg-gray-50 focus:bg-white" placeholder="0" />
                </div>
                <div className="space-y-2">
                  <label className="text-sm font-bold text-gray-700">Marca</label>
                  <input type="text" value={newPart.marca} onChange={e => setNewPart({...newPart, marca: e.target.value})} className="w-full px-4 py-3 rounded-xl border border-gray-200 focus:ring-2 focus:ring-blue-500 outline-none bg-gray-50 focus:bg-white" placeholder="Opcional" />
                </div>
                <div className="space-y-2">
                  <label className="text-sm font-bold text-gray-700">Stock Inicial *</label>
                  <input required type="number" min="0" value={newPart.stockInicial} onChange={e => setNewPart({...newPart, stockInicial: e.target.value})} className="w-full px-4 py-3 rounded-xl border border-gray-200 focus:ring-2 focus:ring-blue-500 outline-none bg-gray-50 focus:bg-white" placeholder="0" />
                </div>
                <div className="space-y-2">
                  <label className="text-sm font-bold text-gray-700">Stock Mínimo</label>
                  <input required type="number" min="0" value={newPart.stockMinimo} onChange={e => setNewPart({...newPart, stockMinimo: e.target.value})} className="w-full px-4 py-3 rounded-xl border border-gray-200 focus:ring-2 focus:ring-blue-500 outline-none bg-gray-50 focus:bg-white" placeholder="5" />
                </div>
              </div>
              <div className="pt-6 border-t border-gray-100 flex justify-end gap-3">
                <button type="button" onClick={() => setShowAddModal(false)} className="px-6 py-3 bg-gray-100 hover:bg-gray-200 text-gray-800 font-bold rounded-xl transition-colors">
                  Cancelar
                </button>
                <button type="submit" disabled={loading} className="px-6 py-3 bg-blue-600 hover:bg-blue-700 text-white font-bold rounded-xl transition-colors shadow-lg shadow-blue-500/30 flex items-center gap-2">
                  {loading ? <div className="w-5 h-5 border-2 border-white/30 border-t-white rounded-full animate-spin" /> : <Plus className="w-5 h-5" />}
                  {loading ? 'Guardando...' : 'Guardar Producto'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};
