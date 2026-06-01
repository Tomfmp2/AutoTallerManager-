import { useState, useEffect } from 'react';
import { Wrench, CheckCircle, FileText, ChevronRight, Car, User, Clock, Package, Plus, Minus, X, Search, ChevronDown, ChevronUp, Calendar } from 'lucide-react';

interface Order {
  id: number;
  tipoServicio: string;
  vehiclePlaca: string;
  fechaIngreso: string;
  estado: string;
  clienteNombre: string;
}

interface Part {
  id: number;
  codigo: string;
  descripcion: string;
  precioUnitario: number;
  stockActual: number;
}

interface SelectedPart {
  part: Part;
  quantity: number;
}

interface ServiceOrderReport {
  id: number;
  reportText: string;
  isDiagnostic: boolean;
  estimatedHours?: number;
  createdAt: string;
  mechanicName: string;
}

const API = 'http://localhost:5219';

export default function MechanicDashboard() {
  const [orders, setOrders] = useState<Order[]>([]);
  const [loading, setLoading] = useState(true);
  const [selectedOrder, setSelectedOrder] = useState<Order | null>(null);
  const [parts, setParts] = useState<Part[]>([]);
  const [partsSearch, setPartsSearch] = useState('');
  const [selectedParts, setSelectedParts] = useState<SelectedPart[]>([]);
  const [diagnosticText, setDiagnosticText] = useState('');
  const [estimatedHours, setEstimatedHours] = useState(1);
  const [progressText, setProgressText] = useState('');
  const [finalReportText, setFinalReportText] = useState('');
  const [submitting, setSubmitting] = useState(false);
  const [msg, setMsg] = useState<{ type: 'ok' | 'err'; text: string } | null>(null);
  const [existingDiagnostic, setExistingDiagnostic] = useState<ServiceOrderReport | null>(null);
  const [isDiagnosticExpanded, setIsDiagnosticExpanded] = useState(false);

  const token = localStorage.getItem('token');
  const headers = { 'Content-Type': 'application/json', Authorization: `Bearer ${token}` };

  const showMsg = (type: 'ok' | 'err', text: string) => {
    setMsg({ type, text });
    setTimeout(() => setMsg(null), 4000);
  };

  const fetchOrders = async () => {
    try {
      const res = await fetch(`${API}/api/mechanic/assigned`, { headers });
      if (res.ok) {
        const data = await res.json();
        const arr = data?.value?.items || data?.value || data;
        setOrders(Array.isArray(arr) ? arr : []);
      }
    } catch (e) {
      console.error(e);
    } finally {
      setLoading(false);
    }
  };

  const fetchParts = async () => {
    try {
      const res = await fetch(`${API}/api/repuestos?PageSize=1000`, { headers });
      if (res.ok) {
        const data = await res.json();
        const arr = data?.value?.items || data?.value || data;
        setParts(Array.isArray(arr) ? arr : []);
      }
    } catch (e) {
      console.error(e);
    }
  };

  useEffect(() => {
    fetchOrders();
    fetchParts();
  }, []);

  const handleSelectOrder = async (o: Order) => {
    setSelectedOrder(o);
    setSelectedParts([]);
    setDiagnosticText('');
    setEstimatedHours(1);
    setExistingDiagnostic(null);
    setIsDiagnosticExpanded(false);

    try {
      const res = await fetch(`${API}/api/mechanic/${o.id}/reports`, { headers });
      if (res.ok) {
        const data = await res.json();
        const arr: ServiceOrderReport[] = data?.value || data || [];
        const diag = arr.find(r => r.isDiagnostic);
        if (diag) {
          setExistingDiagnostic(diag);
        }
      }
    } catch (e) {
      console.error(e);
    }
  };

  const addPart = (part: Part) => {
    setSelectedParts(prev => {
      const existing = prev.find(p => p.part.id === part.id);
      if (existing) {
        return prev.map(p => p.part.id === part.id ? { ...p, quantity: p.quantity + 1 } : p);
      }
      return [...prev, { part, quantity: 1 }];
    });
  };

  const removePart = (partId: number) => {
    setSelectedParts(prev => prev.filter(p => p.part.id !== partId));
  };

  const changeQty = (partId: number, delta: number) => {
    setSelectedParts(prev => prev.map(p => {
      if (p.part.id !== partId) return p;
      const newQty = p.quantity + delta;
      return newQty <= 0 ? p : { ...p, quantity: newQty };
    }));
  };

  const totalPartsCost = selectedParts.reduce((sum, sp) => sum + sp.part.precioUnitario * sp.quantity, 0);

  const filteredParts = parts.filter(p =>
    (p.descripcion || '').toLowerCase().includes((partsSearch || '').toLowerCase()) ||
    (p.codigo || '').toLowerCase().includes((partsSearch || '').toLowerCase())
  );

  const handleSubmitDiagnostic = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!selectedOrder) return;
    setSubmitting(true);
    try {
      const res = await fetch(`${API}/api/mechanic/inspection`, {
        method: 'POST',
        headers,
        body: JSON.stringify({
          serviceOrderId: selectedOrder.id,
          diagnosticText,
          estimatedHours,
          parts: selectedParts.map(sp => ({ partId: sp.part.id, quantity: sp.quantity }))
        })
      });
      if (res.ok) {
        showMsg('ok', '✅ Diagnóstico enviado a recepción. Esperando generación de factura.');
        setDiagnosticText('');
        setEstimatedHours(1);
        setSelectedParts([]);
        fetchOrders();
        handleSelectOrder(selectedOrder); // Refresh diagnostic state
      } else {
        const err = await res.json();
        showMsg('err', err.message || 'Error al enviar diagnóstico.');
      }
    } catch (e) {
      showMsg('err', 'Error de conexión.');
    } finally {
      setSubmitting(false);
    }
  };

  const handleSubmitProgress = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!selectedOrder) return;
    setSubmitting(true);
    try {
      const res = await fetch(`${API}/api/mechanic/progress`, {
        method: 'POST',
        headers,
        body: JSON.stringify({ serviceOrderId: selectedOrder.id, reportText: progressText })
      });
      if (res.ok) {
        showMsg('ok', '📝 Avance registrado correctamente.');
        setProgressText('');
      } else {
        const err = await res.json();
        showMsg('err', err.message || 'Error al registrar avance.');
      }
    } catch (e) {
      showMsg('err', 'Error de conexión.');
    } finally {
      setSubmitting(false);
    }
  };

  const handleFinish = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!selectedOrder) return;
    setSubmitting(true);
    try {
      const res = await fetch(`${API}/api/mechanic/complete`, {
        method: 'POST',
        headers,
        body: JSON.stringify({ serviceOrderId: selectedOrder.id, finalReport: finalReportText })
      });
      if (res.ok) {
        showMsg('ok', '🏁 Mantenimiento finalizado. Notificación enviada a recepción.');
        setSelectedOrder(null);
        fetchOrders();
      } else {
        const err = await res.json();
        showMsg('err', err.message || 'Error al finalizar.');
      }
    } catch (e) {
      showMsg('err', 'Error de conexión.');
    } finally {
      setSubmitting(false);
    }
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center h-[60vh]">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
      </div>
    );
  }

  return (
    <div className="p-6 max-w-7xl mx-auto space-y-6 animate-fade-in">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-black text-gray-900 tracking-tight">Mi Plan de Trabajo</h1>
          <p className="text-gray-500 font-medium mt-1">Gestiona los vehículos asignados y reporta avances</p>
        </div>
        <div className="flex items-center gap-2 bg-blue-50 border border-blue-100 rounded-xl px-4 py-2">
          <Car className="w-4 h-4 text-blue-500" />
          <span className="text-blue-700 font-bold text-sm">{orders.length} asignación{orders.length !== 1 ? 'es' : ''}</span>
        </div>
      </div>

      {/* Toast */}
      {msg && (
        <div className={`fixed top-6 right-6 z-50 px-5 py-3 rounded-xl font-semibold shadow-xl transition-all animate-fade-in ${
          msg.type === 'ok' ? 'bg-emerald-500 text-white' : 'bg-red-500 text-white'
        }`}>
          {msg.text}
        </div>
      )}

      <div className="grid grid-cols-1 lg:grid-cols-12 gap-6">
        {/* Lista de asignaciones */}
        <div className="lg:col-span-4 bg-white rounded-2xl shadow-sm border border-gray-100 overflow-hidden flex flex-col" style={{ height: 'calc(100vh - 220px)' }}>
          <div className="p-5 border-b border-gray-100 bg-gradient-to-r from-gray-50 to-white flex items-center justify-between shrink-0">
            <h2 className="font-bold text-gray-800 text-lg flex items-center gap-2">
              <Car className="w-5 h-5 text-blue-500" /> Mis Asignaciones
            </h2>
            <span className="bg-blue-100 text-blue-700 text-xs font-bold px-2 py-1 rounded-full">{orders.length}</span>
          </div>
          <div className="overflow-y-auto flex-1 p-3 space-y-2">
            {orders.length === 0 && (
              <div className="flex flex-col items-center justify-center h-40 text-gray-400 space-y-2">
                <CheckCircle className="w-8 h-8 opacity-50" />
                <p className="text-sm font-medium text-center">¡Al día!<br />No tienes tareas pendientes.</p>
              </div>
            )}
            {orders.map(o => (
              <button
                key={o.id}
                onClick={() => handleSelectOrder(o)}
                className={`w-full text-left p-4 rounded-xl border-2 transition-all duration-200 group ${
                  selectedOrder?.id === o.id
                    ? 'border-blue-500 bg-blue-50/50 shadow-md'
                    : 'border-transparent bg-gray-50 hover:bg-gray-100 hover:border-gray-200'
                }`}
              >
                <div className="flex justify-between items-start mb-2">
                  <span className="bg-gray-800 text-white text-xs font-bold px-2 py-0.5 rounded-md uppercase tracking-wider">{o.vehiclePlaca}</span>
                  <ChevronRight className={`w-4 h-4 transition-transform ${selectedOrder?.id === o.id ? 'text-blue-500 translate-x-1' : 'text-gray-400'}`} />
                </div>
                <p className="font-bold text-gray-900 text-sm line-clamp-1">{o.tipoServicio}</p>
                <p className="text-xs text-gray-500 mt-1 flex items-center gap-1"><User className="w-3 h-3" />{o.clienteNombre}</p>
                <p className="text-xs text-gray-400 mt-1 flex items-center gap-1"><Clock className="w-3 h-3" />{new Date(o.fechaIngreso).toLocaleDateString()}</p>
                {o.estado && (
                  <span className={`mt-2 inline-block text-xs font-semibold px-2 py-0.5 rounded-full ${
                    o.estado === 'Programada' ? 'bg-yellow-100 text-yellow-700' :
                    o.estado === 'EnProceso' ? 'bg-green-100 text-green-700' :
                    'bg-gray-100 text-gray-600'
                  }`}>{o.estado}</span>
                )}
              </button>
            ))}
          </div>
        </div>

        {/* Panel de trabajo */}
        <div className="lg:col-span-8 bg-white rounded-2xl shadow-sm border border-gray-100 flex flex-col relative overflow-hidden" style={{ height: 'calc(100vh - 220px)' }}>
          {!selectedOrder ? (
            <div className="flex-1 flex flex-col items-center justify-center text-gray-400 bg-gray-50/50 p-8">
              <div className="w-24 h-24 mb-6 rounded-full bg-gray-100 flex items-center justify-center">
                <Wrench className="w-10 h-10 text-gray-300" />
              </div>
              <h3 className="text-lg font-semibold text-gray-600">Ningún vehículo seleccionado</h3>
              <p className="text-sm mt-1 max-w-sm text-center text-gray-400">Selecciona un vehículo de la lista lateral para registrar el diagnóstico, seleccionar repuestos y reportar avances.</p>
            </div>
          ) : (
            <div className="flex flex-col h-full">
              {/* Header */}
              <div className="p-5 border-b border-gray-100 bg-white shrink-0">
                <div className="flex items-center gap-3 mb-1">
                  <span className="px-3 py-1 bg-gray-900 text-white rounded-lg font-bold tracking-widest text-sm">{selectedOrder.vehiclePlaca}</span>
                  <span className="px-2 py-1 bg-blue-100 text-blue-700 rounded-md font-semibold text-xs border border-blue-200">{selectedOrder.tipoServicio}</span>
                </div>
                <h2 className="text-xl font-black text-gray-800">{selectedOrder.clienteNombre}</h2>
                <p className="text-gray-400 text-xs flex items-center gap-1 mt-1"><Clock className="w-3 h-3" />Ingreso: {new Date(selectedOrder.fechaIngreso).toLocaleDateString()}</p>
              </div>

              {/* Formularios */}
              <div className="flex-1 overflow-y-auto p-5 bg-gray-50 space-y-5">

                {/* ═══ ETAPA 1: DIAGNÓSTICO ═══ */}
                {existingDiagnostic ? (
                  <div className="bg-white p-5 rounded-2xl border border-blue-100 shadow-sm overflow-hidden relative">
                    <div className="absolute top-0 left-0 w-1 h-full bg-blue-500"></div>
                    <button 
                      onClick={() => setIsDiagnosticExpanded(!isDiagnosticExpanded)}
                      className="w-full flex items-center justify-between text-left pl-2 focus:outline-none"
                    >
                      <div className="flex items-center gap-3">
                        <div className="p-2 bg-blue-50 text-blue-600 rounded-lg"><CheckCircle className="w-5 h-5" /></div>
                        <div>
                          <h3 className="font-bold text-gray-800">Diagnóstico Enviado</h3>
                          <p className="text-xs text-gray-500">Toca para ver los detalles</p>
                        </div>
                      </div>
                      {isDiagnosticExpanded ? <ChevronUp className="w-5 h-5 text-gray-400" /> : <ChevronDown className="w-5 h-5 text-gray-400" />}
                    </button>
                    
                    {isDiagnosticExpanded && (
                      <div className="mt-4 bg-gray-50 border border-gray-100 rounded-xl p-4 space-y-3 animate-fade-in ml-2">
                        <div>
                          <h4 className="text-sm font-semibold text-gray-700">Descripción del diagnóstico</h4>
                          <p className="text-sm text-gray-600 mt-1 whitespace-pre-wrap">{existingDiagnostic.reportText}</p>
                        </div>
                        <div className="flex items-center gap-4 text-sm text-gray-600 pt-3 border-t border-gray-200">
                          <span className="flex items-center gap-1 font-medium"><Clock className="w-4 h-4 text-blue-500" /> {existingDiagnostic.estimatedHours} h estimadas</span>
                          <span className="flex items-center gap-1 font-medium"><Calendar className="w-4 h-4 text-blue-500" /> {new Date(existingDiagnostic.createdAt).toLocaleDateString()}</span>
                        </div>
                      </div>
                    )}
                  </div>
                ) : (
                  <div className="bg-white p-5 rounded-2xl border border-gray-100 shadow-sm relative overflow-hidden">
                    <div className="absolute top-0 left-0 w-1 h-full bg-blue-400"></div>
                    <div className="flex items-center gap-3 mb-4 pl-2">
                      <div className="p-2 bg-blue-50 text-blue-600 rounded-lg"><FileText className="w-5 h-5" /></div>
                      <div>
                        <h3 className="font-bold text-gray-800">Revisión y Diagnóstico</h3>
                        <p className="text-xs text-gray-500">Diagnóstico + repuestos → se genera factura para el cliente</p>
                      </div>
                    </div>
                    <form onSubmit={handleSubmitDiagnostic} className="space-y-4 pl-2">
                      {/* Texto diagnóstico */}
                      <div>
                        <label className="block text-sm font-semibold text-gray-700 mb-1.5">Descripción del diagnóstico</label>
                        <textarea
                          required
                          value={diagnosticText}
                          onChange={e => setDiagnosticText(e.target.value)}
                          rows={3}
                          className="w-full border border-gray-200 bg-gray-50 rounded-xl p-3 text-sm focus:ring-2 focus:ring-blue-500 focus:border-transparent focus:bg-white transition-all resize-none"
                          placeholder="Describe el estado del vehículo y las reparaciones necesarias..."
                        />
                      </div>

                      {/* Horas estimadas */}
                      <div>
                        <label className="block text-sm font-semibold text-gray-700 mb-1.5">Tiempo estimado (horas)</label>
                        <input
                          type="number"
                          min="1"
                          required
                          value={estimatedHours}
                          onChange={e => setEstimatedHours(parseInt(e.target.value) || 1)}
                          className="w-full md:w-32 border border-gray-200 bg-gray-50 rounded-xl p-3 text-sm focus:ring-2 focus:ring-blue-500 focus:border-transparent focus:bg-white transition-all"
                        />
                      </div>

                      {/* Selector de repuestos */}
                      <div>
                        <label className="block text-sm font-semibold text-gray-700 mb-2 flex items-center gap-2">
                          <Package className="w-4 h-4 text-purple-500" />
                          Repuestos a utilizar
                        </label>

                        {/* Repuestos seleccionados */}
                        {selectedParts.length > 0 && (
                          <div className="mb-3 bg-purple-50 border border-purple-100 rounded-xl p-3 space-y-2">
                            <p className="text-xs font-semibold text-purple-700 mb-2">Seleccionados ({selectedParts.length})</p>
                            {selectedParts.map(sp => (
                              <div key={sp.part.id} className="flex items-center justify-between gap-2 bg-white rounded-lg p-2 border border-purple-100">
                                <div className="flex-1 min-w-0">
                                  <p className="text-sm font-semibold text-gray-800 truncate">{sp.part.descripcion}</p>
                                  <p className="text-xs text-gray-400">${sp.part.precioUnitario.toFixed(2)} c/u</p>
                                </div>
                                <div className="flex items-center gap-2 shrink-0">
                                  <button type="button" onClick={() => changeQty(sp.part.id, -1)} className="w-6 h-6 rounded-full bg-gray-100 hover:bg-gray-200 flex items-center justify-center transition-colors">
                                    <Minus className="w-3 h-3 text-gray-600" />
                                  </button>
                                  <span className="w-6 text-center text-sm font-bold text-gray-800">{sp.quantity}</span>
                                  <button type="button" onClick={() => changeQty(sp.part.id, 1)} className="w-6 h-6 rounded-full bg-gray-100 hover:bg-gray-200 flex items-center justify-center transition-colors">
                                    <Plus className="w-3 h-3 text-gray-600" />
                                  </button>
                                  <button type="button" onClick={() => removePart(sp.part.id)} className="w-6 h-6 rounded-full bg-red-100 hover:bg-red-200 flex items-center justify-center transition-colors ml-1">
                                    <X className="w-3 h-3 text-red-600" />
                                  </button>
                                </div>
                                <span className="text-sm font-bold text-purple-700 w-16 text-right shrink-0">${(sp.part.precioUnitario * sp.quantity).toFixed(2)}</span>
                              </div>
                            ))}
                            <div className="flex justify-between items-center pt-1 border-t border-purple-100 mt-2">
                              <span className="text-xs font-semibold text-purple-600">Total repuestos:</span>
                              <span className="text-sm font-black text-purple-700">${totalPartsCost.toFixed(2)}</span>
                            </div>
                          </div>
                        )}

                        {/* Buscador de repuestos */}
                        <div className="border border-gray-200 rounded-xl overflow-hidden">
                          <div className="flex items-center gap-2 px-3 py-2 bg-gray-50 border-b border-gray-100">
                            <Search className="w-4 h-4 text-gray-400" />
                            <input
                              type="text"
                              value={partsSearch}
                              onChange={e => setPartsSearch(e.target.value)}
                              placeholder="Buscar repuesto por nombre o código..."
                              className="flex-1 bg-transparent text-sm outline-none text-gray-700 placeholder-gray-400"
                            />
                          </div>
                          <div className="max-h-40 overflow-y-auto divide-y divide-gray-50">
                            {filteredParts.length === 0 && (
                              <p className="text-center text-xs text-gray-400 py-4">No se encontraron repuestos</p>
                            )}
                            {filteredParts.slice(0, 20).map(p => (
                              <button
                                key={p.id}
                                type="button"
                                onClick={() => addPart(p)}
                                disabled={p.stockActual <= 0}
                                className="w-full text-left px-3 py-2 hover:bg-blue-50 flex items-center justify-between gap-2 transition-colors disabled:opacity-40 disabled:cursor-not-allowed"
                              >
                                <div>
                                  <p className="text-sm font-medium text-gray-800">{p.descripcion}</p>
                                  <p className="text-xs text-gray-400">{p.codigo} · Stock: {p.stockActual}</p>
                                </div>
                                <div className="text-right shrink-0">
                                  <p className="text-sm font-bold text-gray-700">${p.precioUnitario.toFixed(2)}</p>
                                  <div className="flex items-center gap-1 justify-end">
                                    <Plus className="w-3 h-3 text-blue-500" />
                                    <span className="text-xs text-blue-500 font-semibold">Agregar</span>
                                  </div>
                                </div>
                              </button>
                            ))}
                          </div>
                        </div>
                      </div>

                      <button
                        type="submit"
                        disabled={submitting}
                        className="w-full bg-blue-600 text-white py-3 rounded-xl font-bold text-sm shadow-md shadow-blue-200 hover:bg-blue-700 hover:shadow-lg hover:-translate-y-0.5 transition-all disabled:opacity-60 flex items-center justify-center gap-2"
                      >
                        <FileText className="w-4 h-4" />
                        {submitting ? 'Enviando...' : 'Enviar Diagnóstico a Recepción'}
                      </button>
                    </form>
                  </div>
                )}

                {/* ═══ ETAPA 2: AVANCES ═══ */}
                <div className="bg-white p-5 rounded-2xl border border-gray-100 shadow-sm relative overflow-hidden">
                  <div className="absolute top-0 left-0 w-1 h-full bg-orange-400"></div>
                  <div className="flex items-center gap-3 mb-4 pl-2">
                    <div className="p-2 bg-orange-50 text-orange-500 rounded-lg"><Wrench className="w-5 h-5" /></div>
                    <div>
                      <h3 className="font-bold text-gray-800">Bitácora de Avances</h3>
                      <p className="text-xs text-gray-500">Registra el progreso durante el mantenimiento</p>
                    </div>
                  </div>
                  <form onSubmit={handleSubmitProgress} className="space-y-3 pl-2">
                    <textarea
                      required
                      value={progressText}
                      onChange={e => setProgressText(e.target.value)}
                      rows={2}
                      className="w-full border border-gray-200 bg-gray-50 rounded-xl p-3 text-sm focus:ring-2 focus:ring-orange-500 focus:border-transparent focus:bg-white transition-all resize-none"
                      placeholder="¿Qué hiciste hasta ahora? (Ej: Aceite drenado y filtro reemplazado)"
                    />
                    <button
                      type="submit"
                      disabled={submitting}
                      className="bg-orange-100 text-orange-700 px-5 py-2.5 rounded-xl text-sm font-bold hover:bg-orange-200 transition-colors disabled:opacity-60"
                    >
                      Guardar Avance
                    </button>
                  </form>
                </div>

                {/* ═══ ETAPA 3: FINALIZAR ═══ */}
                <div className="bg-white p-5 rounded-2xl border border-emerald-100 shadow-sm relative overflow-hidden">
                  <div className="absolute top-0 left-0 w-1 h-full bg-emerald-500"></div>
                  <div className="flex items-center gap-3 mb-4 pl-2">
                    <div className="p-2 bg-emerald-100 text-emerald-600 rounded-lg"><CheckCircle className="w-5 h-5" /></div>
                    <div>
                      <h3 className="font-bold text-gray-800">Finalizar Mantenimiento</h3>
                      <p className="text-xs text-gray-500">Emite el reporte final al completar el trabajo</p>
                    </div>
                  </div>
                  <form onSubmit={handleFinish} className="space-y-3 pl-2">
                    <textarea
                      required
                      value={finalReportText}
                      onChange={e => setFinalReportText(e.target.value)}
                      rows={3}
                      className="w-full border border-gray-200 bg-white rounded-xl p-3 text-sm focus:ring-2 focus:ring-emerald-500 focus:border-transparent transition-all resize-none"
                      placeholder="Conclusión final, observaciones o recomendaciones para el cliente..."
                    />
                    <button
                      type="submit"
                      disabled={submitting}
                      className="w-full bg-gradient-to-r from-emerald-500 to-emerald-600 text-white py-4 rounded-xl font-bold shadow-lg shadow-emerald-200 hover:shadow-xl hover:-translate-y-0.5 transition-all flex items-center justify-center gap-2 text-base disabled:opacity-60"
                    >
                      <CheckCircle className="w-5 h-5" />
                      {submitting ? 'Enviando...' : 'Marcar Vehículo como Listo'}
                    </button>
                  </form>
                </div>

              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
