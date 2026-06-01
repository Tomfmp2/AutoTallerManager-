import { useEffect, useState } from 'react';
import {
  CheckCircle, XCircle, AlertCircle, Calendar, Wrench, FileText,
  DollarSign, ChevronDown, ChevronUp, Package, Clock, User, Search, ArrowDownUp
} from 'lucide-react';
import { useNavigate } from 'react-router-dom';

const API = 'http://localhost:5219';

interface PendingOrder {
  id: number;
  serviceType?: string;
  tipoServicio?: string;
  vehicle?: string;
  licensePlate?: string;
  vehiclePlaca?: string;
  entryDate?: string;
  fechaIngreso?: string;
  scheduledDate?: string | null;
  workPerformed?: string | null;
  descripcion?: string | null;
  status?: string;
  clientName?: string;
  clienteNombre?: string;
}

interface Mechanic {
  id: number;
  name: string;
  email: string;
}

interface DiagnosticReport {
  id: number;
  reportText: string;
  mechanicName: string;
  estimatedHours: number;
  reportParts?: { partName: string; quantity: number; unitPrice: number }[];
}

interface WaitingPaymentOrder extends PendingOrder {
  invoiceId?: number;
  total?: number;
  paymentStatus?: string;
}

export default function RecepAprobaciones() {
  const [orders, setOrders] = useState<PendingOrder[]>([]);
  const [diagnosticOrders, setDiagnosticOrders] = useState<PendingOrder[]>([]);
  const [waitingPaymentOrders, setWaitingPaymentOrders] = useState<WaitingPaymentOrder[]>([]);
  const [mechanics, setMechanics] = useState<Mechanic[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  
  const [selectedMechanics, setSelectedMechanics] = useState<Record<number, string>>({});
  const [rejectReasons, setRejectReasons] = useState<Record<number, string>>({});
  const [diagnosticReports, setDiagnosticReports] = useState<Record<number, DiagnosticReport | null>>({});
  const [expandedOrders, setExpandedOrders] = useState<Record<number, boolean>>({});
  const [submitting, setSubmitting] = useState<Record<number, boolean>>({});
  const [msg, setMsg] = useState<{ type: 'ok' | 'err'; text: string } | null>(null);

  // Accordion & Filter states
  const [sectionsOpen, setSectionsOpen] = useState({ new: true, diag: false, pay: false });
  const [filters, setFilters] = useState({
    new: { search: '', sort: 'recent' },
    diag: { search: '', sort: 'recent' },
    pay: { search: '', sort: 'recent' }
  });

  const navigate = useNavigate();
  const token = localStorage.getItem('token');
  const headers = { 'Content-Type': 'application/json', Authorization: `Bearer ${token}` };

  const showMsg = (type: 'ok' | 'err', text: string) => {
    setMsg({ type, text });
    setTimeout(() => setMsg(null), 4500);
  };

  const fetchData = async () => {
    setLoading(true);
    try {
      if (!token) { navigate('/login'); return; }

      const [ordRes, diagRes, mechRes] = await Promise.all([
        fetch(`${API}/api/recepcion/ordenes/pendientes`, { headers }),
        fetch(`${API}/api/recepcion/ordenes/diagnostico-completado`, { headers }),
        fetch(`${API}/api/recepcion/mecanicos`, { headers })
      ]);

      if (ordRes.ok) setOrders(await ordRes.json());
      else setError('Error al cargar órdenes pendientes.');

      if (diagRes.ok) {
        const dOrders: PendingOrder[] = await diagRes.json();
        setDiagnosticOrders(dOrders);

        // Fetch diagnostic reports
        const reportFetches = dOrders.map(async o => {
          try {
            const rRes = await fetch(`${API}/api/mechanic/${o.id}/reports`, { headers });
            if (rRes.ok) {
              const data = await rRes.json();
              const reports: any[] = data.value || [];
              const diag = reports.find((r: any) => r.isDiagnostic) || null;
              return { id: o.id, report: diag };
            }
          } catch {}
          return { id: o.id, report: null };
        });
        const results = await Promise.all(reportFetches);
        const reportsMap: Record<number, DiagnosticReport | null> = {};
        results.forEach(r => { reportsMap[r.id] = r.report; });
        setDiagnosticReports(reportsMap);
      }

      if (mechRes.ok) setMechanics(await mechRes.json());

      // Fetch orders waiting for payment approval
      try {
        const wpRes = await fetch(`${API}/api/recepcion/ordenes/esperando-pago`, { headers });
        if (wpRes.ok) {
          const wpData = await wpRes.json();
          setWaitingPaymentOrders(wpData.value || wpData || []);
        }
      } catch {}

    } catch { setError('Error de conexión.'); }
    finally { setLoading(false); }
  };

  useEffect(() => { fetchData(); }, [navigate]);

  const setSubmit = (id: number, val: boolean) => setSubmitting(prev => ({ ...prev, [id]: val }));

  const handleApprove = async (orderId: number) => {
    const mechanicId = selectedMechanics[orderId];
    if (!mechanicId) { showMsg('err', 'Selecciona un mecánico para aprobar la orden.'); return; }
    setSubmit(orderId, true);
    try {
      const res = await fetch(`${API}/api/recepcion/ordenes/${orderId}/aprobar`, {
        method: 'POST', headers,
        body: JSON.stringify({ mechanicId: Number(mechanicId) })
      });
      if (res.ok) { showMsg('ok', 'âœ… Revisión aprobada. Mecánico asignado y notificado.'); fetchData(); }
      else { const e = await res.json(); showMsg('err', e.message || 'Error al aprobar.'); }
    } catch { showMsg('err', 'Error de conexión.'); }
    finally { setSubmit(orderId, false); }
  };

  const handleReject = async (orderId: number) => {
    const reason = rejectReasons[orderId];
    if (!reason) { showMsg('err', 'Ingresa un motivo de rechazo.'); return; }
    setSubmit(orderId, true);
    try {
      const res = await fetch(`${API}/api/recepcion/ordenes/${orderId}/rechazar`, {
        method: 'POST', headers,
        body: JSON.stringify({ reason })
      });
      if (res.ok) { showMsg('ok', 'âŒ Revisión rechazada. Cliente notificado.'); fetchData(); }
      else { const e = await res.json(); showMsg('err', e.message || 'Error al rechazar.'); }
    } catch { showMsg('err', 'Error de conexión.'); }
    finally { setSubmit(orderId, false); }
  };

  const handleGenerateInvoice = async (orderId: number) => {
    setSubmit(orderId, true);
    try {
      const res = await fetch(`${API}/api/recepcion/ordenes/${orderId}/generar-factura`, {
        method: 'POST', headers
      });
      if (res.ok) { showMsg('ok', 'ðŸ§¾ Factura generada y enviada al cliente para aprobación.'); fetchData(); }
      else { const e = await res.json(); showMsg('err', e.message || 'Error al generar factura.'); }
    } catch { showMsg('err', 'Error de conexión.'); }
    finally { setSubmit(orderId, false); }
  };

  const handleConfirmPayment = async (invoiceId: number, orderId: number) => {
    setSubmit(orderId, true);
    try {
      const res = await fetch(`${API}/api/recepcion/facturas/${invoiceId}/confirmar-pago`, {
        method: 'POST', headers
      });
      if (res.ok) { showMsg('ok', 'ðŸ’° Pago confirmado. El mantenimiento ha iniciado.'); fetchData(); }
      else { const e = await res.json(); showMsg('err', e.message || 'Error al confirmar pago.'); }
    } catch { showMsg('err', 'Error de conexión.'); }
    finally { setSubmit(orderId, false); }
  };

  const toggleExpand = (id: number) => setExpandedOrders(prev => ({ ...prev, [id]: !prev[id] }));

  const toggleSection = (section: 'new' | 'diag' | 'pay') => {
    setSectionsOpen(prev => ({ ...prev, [section]: !prev[section] }));
  };

  const setFilter = (section: 'new' | 'diag' | 'pay', key: 'search' | 'sort', value: string) => {
    setFilters(prev => ({
      ...prev,
      [section]: { ...prev[section], [key]: value }
    }));
  };

  const getFilteredOrders = <T extends PendingOrder>(list: T[], section: 'new' | 'diag' | 'pay'): T[] => {
    const { search, sort } = filters[section];
    const s = search.toLowerCase();
    
    let result = list.filter(o => 
      (o.clientName || o.clienteNombre || '').toLowerCase().includes(s) ||
      (o.id.toString().includes(s)) ||
      (o.vehicle || '').toLowerCase().includes(s) ||
      (o.licensePlate || o.vehiclePlaca || '').toLowerCase().includes(s)
    );

    result = result.sort((a, b) => {
      const dateA = new Date(a.entryDate || a.fechaIngreso || a.scheduledDate || 0).getTime() || a.id;
      const dateB = new Date(b.entryDate || b.fechaIngreso || b.scheduledDate || 0).getTime() || b.id;
      if (sort === 'oldest') return dateA - dateB;
      return dateB - dateA;
    });

    return result;
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center min-h-[50vh]">
        <div className="animate-spin w-10 h-10 border-4 border-blue-600 border-t-transparent rounded-full" />
      </div>
    );
  }

  if (error) {
    return (
      <div className="flex flex-col items-center justify-center min-h-[50vh] text-center">
        <AlertCircle className="w-12 h-12 text-red-500 mb-4" />
        <p className="text-gray-500">{error}</p>
      </div>
    );
  }

  const filteredOrders = getFilteredOrders(orders, 'new');
  const filteredDiagnostic = getFilteredOrders(diagnosticOrders, 'diag');
  const filteredPayment = getFilteredOrders(waitingPaymentOrders, 'pay');

  const totalPending = orders.length + diagnosticOrders.length + waitingPaymentOrders.length;

  return (
    <div className="max-w-5xl mx-auto space-y-6 pb-12">

      {/* Toast */}
      {msg && (
        <div className={`fixed top-6 right-6 z-50 px-5 py-3 rounded-xl font-semibold shadow-xl animate-fade-in ${
          msg.type === 'ok' ? 'bg-emerald-500 text-white' : 'bg-red-500 text-white'
        }`}>
          {msg.text}
        </div>
      )}

      {/* Header */}
      <div className="flex items-center justify-between mb-8">
        <div>
          <h2 className="text-3xl font-black text-gray-900">Aprobaciones y Flujo</h2>
          <p className="text-gray-500 mt-1 font-medium">Gestiona Revisiones, diagnósticos, facturas y pagos</p>
        </div>
        <span className="bg-amber-100 text-amber-700 text-sm font-bold px-4 py-2 rounded-full border border-amber-200 shadow-sm">
          {totalPending} pendiente{totalPending !== 1 ? 's' : ''}
        </span>
      </div>

      {/* â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â• SECCIÃ“N 1: Revisiones NUEVAS â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â• */}
      <section className="bg-white rounded-3xl shadow-sm border border-gray-100 overflow-hidden transition-all">
        <button 
          onClick={() => toggleSection('new')}
          className={`w-full flex items-center justify-between p-6 bg-amber-50/50 hover:bg-amber-50 transition-colors ${sectionsOpen.new ? 'border-b border-amber-100' : ''}`}
        >
          <div className="flex items-center gap-3">
            <div className="w-10 h-10 rounded-2xl bg-amber-100 text-amber-600 flex items-center justify-center">
              <Calendar className="w-5 h-5" />
            </div>
            <div className="text-left">
              <h3 className="text-xl font-bold text-gray-800 flex items-center gap-2">
                Revisiones Nuevas 
                <span className="bg-amber-500 text-white text-xs font-bold px-2 py-0.5 rounded-full">{orders.length}</span>
              </h3>
              <p className="text-sm text-gray-500 font-medium">Revisiones recién creadas pendientes de revisión y asignación</p>
            </div>
          </div>
          <div className="p-2 bg-white rounded-full shadow-sm text-gray-400">
            {sectionsOpen.new ? <ChevronUp className="w-5 h-5" /> : <ChevronDown className="w-5 h-5" />}
          </div>
        </button>

        {sectionsOpen.new && (
          <div className="p-6 bg-gray-50/30">
            <div className="flex flex-col sm:flex-row gap-3 mb-6">
              <div className="relative flex-1">
                <Search className="w-4 h-4 absolute left-4 top-1/2 -translate-y-1/2 text-gray-400" />
                <input 
                  type="text" 
                  placeholder="Buscar por ID, cliente, vehículo o placa..." 
                  value={filters.new.search}
                  onChange={e => setFilter('new', 'search', e.target.value)}
                  className="w-full pl-10 pr-4 py-2.5 bg-white border border-gray-200 rounded-xl focus:ring-2 focus:ring-amber-500 outline-none text-sm font-medium"
                />
              </div>
              <div className="relative w-full sm:w-48">
                <ArrowDownUp className="w-4 h-4 absolute left-4 top-1/2 -translate-y-1/2 text-gray-400" />
                <select 
                  value={filters.new.sort}
                  onChange={e => setFilter('new', 'sort', e.target.value)}
                  className="w-full pl-10 pr-4 py-2.5 bg-white border border-gray-200 rounded-xl focus:ring-2 focus:ring-amber-500 outline-none text-sm font-medium appearance-none cursor-pointer"
                >
                  <option value="recent">Más recientes</option>
                  <option value="oldest">Más antiguas</option>
                </select>
              </div>
            </div>

            {filteredOrders.length === 0 ? (
              <div className="bg-white rounded-2xl p-10 border border-dashed border-gray-200 flex flex-col items-center text-center">
                <Calendar className="w-10 h-10 text-gray-300 mb-3" />
                <p className="text-gray-500 font-medium">No hay Revisiones en esta sección</p>
              </div>
            ) : (
              <div className="space-y-4">
                {filteredOrders.map(order => (
                  <div key={order.id} className="bg-white rounded-2xl shadow-sm border border-gray-100 overflow-hidden hover:border-amber-200 transition-colors">
                    <div className="flex items-center justify-between px-5 py-3 bg-amber-50/50 border-b border-amber-50">
                      <div className="flex items-center gap-2">
                        <span className="w-2 h-2 rounded-full bg-amber-400"></span>
                        <span className="text-xs font-bold text-amber-700 uppercase tracking-wide">ID: #{order.id} Â· Nueva Revisión</span>
                      </div>
                      <button onClick={() => toggleExpand(order.id)} className="text-gray-400 hover:text-gray-600 transition-colors">
                        {expandedOrders[order.id] ? <ChevronUp className="w-4 h-4" /> : <ChevronDown className="w-4 h-4" />}
                      </button>
                    </div>

                    <div className="p-5">
                      <div className="flex items-start justify-between gap-4 mb-3">
                        <div>
                          <h4 className="font-bold text-gray-900 text-lg mb-1">{order.serviceType || order.tipoServicio}</h4>
                          <p className="text-sm text-gray-600 flex items-center gap-2 mt-1">
                            <User className="w-4 h-4 text-gray-400" /> <span className="font-semibold">{order.clientName || order.clienteNombre || 'Sin cliente'}</span>
                          </p>
                          <p className="text-sm text-gray-600 flex items-center gap-2 mt-1">
                            <Wrench className="w-4 h-4 text-gray-400" />{order.vehicle || 'Vehículo'} Â· <span className="font-semibold">{order.licensePlate || order.vehiclePlaca}</span>
                          </p>
                          {order.scheduledDate && (
                            <p className="text-sm text-amber-600 flex items-center gap-2 mt-1 font-medium">
                              <Calendar className="w-4 h-4" />
                              {new Date(order.scheduledDate).toLocaleString()}
                            </p>
                          )}
                        </div>
                      </div>

                      {expandedOrders[order.id] && (order.workPerformed || order.descripcion) && (
                        <div className="mb-4 p-4 bg-gray-50 rounded-xl border border-gray-100 text-sm text-gray-600 italic">
                          "{order.workPerformed || order.descripcion}"
                        </div>
                      )}

                      <div className="flex flex-col sm:flex-row gap-3 mt-5 pt-5 border-t border-gray-100">
                        <div className="flex-1 flex gap-2">
                          <select
                            value={selectedMechanics[order.id] || ''}
                            onChange={e => setSelectedMechanics({ ...selectedMechanics, [order.id]: e.target.value })}
                            className="flex-1 px-4 py-2.5 border border-gray-200 bg-gray-50 rounded-xl text-sm font-medium focus:ring-2 focus:ring-green-500/30 focus:border-green-300 outline-none transition-all"
                          >
                            <option value="">Asignar mecánico...</option>
                            {mechanics.map(m => <option key={m.id} value={m.id}>{m.name}</option>)}
                          </select>
                          <button
                            onClick={() => handleApprove(order.id)}
                            disabled={!selectedMechanics[order.id] || submitting[order.id]}
                            className="flex items-center gap-2 px-5 py-2.5 bg-green-600 hover:bg-green-700 disabled:bg-green-300 text-white rounded-xl text-sm font-bold transition-colors shrink-0 shadow-sm shadow-green-600/20"
                          >
                            <CheckCircle className="w-4 h-4" /> Aprobar
                          </button>
                        </div>
                        <div className="flex gap-2">
                          <input
                            type="text"
                            placeholder="Motivo rechazo..."
                            value={rejectReasons[order.id] || ''}
                            onChange={e => setRejectReasons({ ...rejectReasons, [order.id]: e.target.value })}
                            className="flex-1 min-w-0 px-4 py-2.5 border border-gray-200 bg-gray-50 rounded-xl text-sm font-medium focus:ring-2 focus:ring-red-500/30 focus:border-red-300 outline-none transition-all"
                          />
                          <button
                            onClick={() => handleReject(order.id)}
                            disabled={!rejectReasons[order.id] || submitting[order.id]}
                            className="flex items-center gap-2 px-5 py-2.5 bg-red-50 text-red-600 hover:bg-red-100 disabled:opacity-40 rounded-xl text-sm font-bold transition-colors shrink-0 border border-red-100"
                          >
                            <XCircle className="w-4 h-4" /> Rechazar
                          </button>
                        </div>
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>
        )}
      </section>

      {/* â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â• SECCIÃ“N 2: DIAGNÃ“STICO COMPLETADO â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â• */}
      <section className="bg-white rounded-3xl shadow-sm border border-gray-100 overflow-hidden transition-all">
        <button 
          onClick={() => toggleSection('diag')}
          className={`w-full flex items-center justify-between p-6 bg-blue-50/50 hover:bg-blue-50 transition-colors ${sectionsOpen.diag ? 'border-b border-blue-100' : ''}`}
        >
          <div className="flex items-center gap-3">
            <div className="w-10 h-10 rounded-2xl bg-blue-100 text-blue-600 flex items-center justify-center">
              <FileText className="w-5 h-5" />
            </div>
            <div className="text-left">
              <h3 className="text-xl font-bold text-gray-800 flex items-center gap-2">
                Diagnósticos Listos â†’ Generar Factura 
                <span className="bg-blue-500 text-white text-xs font-bold px-2 py-0.5 rounded-full">{diagnosticOrders.length}</span>
              </h3>
              <p className="text-sm text-gray-500 font-medium">Reportes de mecánicos listos para ser cotizados y enviados</p>
            </div>
          </div>
          <div className="p-2 bg-white rounded-full shadow-sm text-gray-400">
            {sectionsOpen.diag ? <ChevronUp className="w-5 h-5" /> : <ChevronDown className="w-5 h-5" />}
          </div>
        </button>

        {sectionsOpen.diag && (
          <div className="p-6 bg-gray-50/30">
            <div className="flex flex-col sm:flex-row gap-3 mb-6">
              <div className="relative flex-1">
                <Search className="w-4 h-4 absolute left-4 top-1/2 -translate-y-1/2 text-gray-400" />
                <input 
                  type="text" 
                  placeholder="Buscar por ID, cliente, vehículo o placa..." 
                  value={filters.diag.search}
                  onChange={e => setFilter('diag', 'search', e.target.value)}
                  className="w-full pl-10 pr-4 py-2.5 bg-white border border-gray-200 rounded-xl focus:ring-2 focus:ring-blue-500 outline-none text-sm font-medium"
                />
              </div>
              <div className="relative w-full sm:w-48">
                <ArrowDownUp className="w-4 h-4 absolute left-4 top-1/2 -translate-y-1/2 text-gray-400" />
                <select 
                  value={filters.diag.sort}
                  onChange={e => setFilter('diag', 'sort', e.target.value)}
                  className="w-full pl-10 pr-4 py-2.5 bg-white border border-gray-200 rounded-xl focus:ring-2 focus:ring-blue-500 outline-none text-sm font-medium appearance-none cursor-pointer"
                >
                  <option value="recent">Más recientes</option>
                  <option value="oldest">Más antiguas</option>
                </select>
              </div>
            </div>

            {filteredDiagnostic.length === 0 ? (
              <div className="bg-white rounded-2xl p-10 border border-dashed border-gray-200 flex flex-col items-center text-center">
                <FileText className="w-10 h-10 text-gray-300 mb-3" />
                <p className="text-gray-500 font-medium">No hay diagnósticos en esta sección</p>
              </div>
            ) : (
              <div className="space-y-4">
                {filteredDiagnostic.map(order => {
                  const report = diagnosticReports[order.id];
                  return (
                    <div key={order.id} className="bg-white rounded-2xl shadow-sm border border-blue-100 overflow-hidden hover:border-blue-300 transition-colors">
                      <div className="flex items-center justify-between px-5 py-3 bg-blue-50/50 border-b border-blue-50">
                        <div className="flex items-center gap-2">
                          <span className="w-2 h-2 rounded-full bg-blue-500"></span>
                          <span className="text-xs font-bold text-blue-700 uppercase tracking-wide">ID: #{order.id} Â· Diagnóstico Completado</span>
                        </div>
                      </div>

                      <div className="p-5">
                        <div className="mb-5">
                          <h4 className="font-bold text-gray-900 text-lg mb-1">{order.serviceType || order.tipoServicio}</h4>
                          <p className="text-sm text-gray-600 flex items-center gap-2 mt-1">
                            <User className="w-4 h-4 text-gray-400" /> <span className="font-semibold">{order.clientName || order.clienteNombre || 'Sin cliente'}</span>
                          </p>
                          <p className="text-sm text-gray-600 flex items-center gap-2 mt-1">
                            <Wrench className="w-4 h-4 text-gray-400" />{order.vehicle || 'Vehículo'} Â· <span className="font-semibold">{order.licensePlate || order.vehiclePlaca}</span>
                          </p>
                        </div>

                        {report && (
                          <div className="bg-blue-50/50 border border-blue-100 rounded-2xl p-5 mb-5 space-y-4">
                            <h5 className="font-bold text-blue-800 flex items-center gap-2">
                              <FileText className="w-5 h-5 text-blue-600" /> Reporte del Mecánico
                            </h5>
                            <p className="text-sm text-gray-700 leading-relaxed bg-white p-3 rounded-xl border border-blue-50">{report.reportText}</p>
                            <div className="flex gap-4 text-sm text-blue-700 font-medium bg-blue-100/50 p-3 rounded-xl inline-flex flex-wrap">
                              <span className="flex items-center gap-1.5"><User className="w-4 h-4" />{report.mechanicName}</span>
                              <span className="flex items-center gap-1.5"><Clock className="w-4 h-4" />{report.estimatedHours} h estimadas</span>
                            </div>
                            {report.reportParts && report.reportParts.length > 0 && (
                              <div className="pt-2">
                                <p className="text-sm font-bold text-blue-800 mb-3 flex items-center gap-2">
                                  <Package className="w-4 h-4" /> Repuestos requeridos:
                                </p>
                                <div className="space-y-2">
                                  {report.reportParts.map((rp, i) => (
                                    <div key={i} className="flex justify-between items-center text-sm text-gray-700 bg-white rounded-xl px-4 py-3 border border-blue-100 shadow-sm">
                                      <span className="font-medium">{rp.partName} <span className="text-gray-400 font-normal ml-2">Ã— {rp.quantity}</span></span>
                                      <span className="font-bold text-gray-900">${(rp.unitPrice * rp.quantity).toFixed(2)}</span>
                                    </div>
                                  ))}
                                </div>
                              </div>
                            )}
                          </div>
                        )}

                        {!report && (
                          <div className="bg-amber-50 border border-amber-100 rounded-xl p-4 mb-5 text-sm font-medium text-amber-700 flex items-center gap-3">
                            <AlertCircle className="w-5 h-5 shrink-0" /> 
                            <span>Esperando a que el mecánico envíe el reporte de diagnóstico detallado...</span>
                          </div>
                        )}

                        <button
                          onClick={() => handleGenerateInvoice(order.id)}
                          disabled={!report || submitting[order.id]}
                          className="w-full flex items-center justify-center gap-2 py-3.5 bg-blue-600 hover:bg-blue-700 disabled:bg-blue-300 text-white rounded-xl font-bold transition-colors shadow-lg shadow-blue-600/20"
                        >
                          <FileText className="w-5 h-5" />
                          {submitting[order.id] ? 'Generando Factura...' : 'Generar Factura y Enviar al Cliente'}
                        </button>
                      </div>
                    </div>
                  );
                })}
              </div>
            )}
          </div>
        )}
      </section>

      {/* â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â• SECCIÃ“N 3: ESPERANDO PAGO â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â• */}
      <section className="bg-white rounded-3xl shadow-sm border border-gray-100 overflow-hidden transition-all">
        <button 
          onClick={() => toggleSection('pay')}
          className={`w-full flex items-center justify-between p-6 bg-emerald-50/50 hover:bg-emerald-50 transition-colors ${sectionsOpen.pay ? 'border-b border-emerald-100' : ''}`}
        >
          <div className="flex items-center gap-3">
            <div className="w-10 h-10 rounded-2xl bg-emerald-100 text-emerald-600 flex items-center justify-center">
              <DollarSign className="w-5 h-5" />
            </div>
            <div className="text-left">
              <h3 className="text-xl font-bold text-gray-800 flex items-center gap-2">
                Factura Aprobada â†’ Confirmar Pago 
                <span className="bg-emerald-500 text-white text-xs font-bold px-2 py-0.5 rounded-full">{waitingPaymentOrders.length}</span>
              </h3>
              <p className="text-sm text-gray-500 font-medium">Facturas aprobadas por clientes en espera de verificación de pago</p>
            </div>
          </div>
          <div className="p-2 bg-white rounded-full shadow-sm text-gray-400">
            {sectionsOpen.pay ? <ChevronUp className="w-5 h-5" /> : <ChevronDown className="w-5 h-5" />}
          </div>
        </button>

        {sectionsOpen.pay && (
          <div className="p-6 bg-gray-50/30">
            <div className="flex flex-col sm:flex-row gap-3 mb-6">
              <div className="relative flex-1">
                <Search className="w-4 h-4 absolute left-4 top-1/2 -translate-y-1/2 text-gray-400" />
                <input 
                  type="text" 
                  placeholder="Buscar por ID, cliente, vehículo o placa..." 
                  value={filters.pay.search}
                  onChange={e => setFilter('pay', 'search', e.target.value)}
                  className="w-full pl-10 pr-4 py-2.5 bg-white border border-gray-200 rounded-xl focus:ring-2 focus:ring-emerald-500 outline-none text-sm font-medium"
                />
              </div>
              <div className="relative w-full sm:w-48">
                <ArrowDownUp className="w-4 h-4 absolute left-4 top-1/2 -translate-y-1/2 text-gray-400" />
                <select 
                  value={filters.pay.sort}
                  onChange={e => setFilter('pay', 'sort', e.target.value)}
                  className="w-full pl-10 pr-4 py-2.5 bg-white border border-gray-200 rounded-xl focus:ring-2 focus:ring-emerald-500 outline-none text-sm font-medium appearance-none cursor-pointer"
                >
                  <option value="recent">Más recientes</option>
                  <option value="oldest">Más antiguas</option>
                </select>
              </div>
            </div>

            {filteredPayment.length === 0 ? (
              <div className="bg-white rounded-2xl p-10 border border-dashed border-gray-200 flex flex-col items-center text-center">
                <DollarSign className="w-10 h-10 text-gray-300 mb-3" />
                <p className="text-gray-500 font-medium">No hay pagos pendientes en esta sección</p>
              </div>
            ) : (
              <div className="space-y-4">
                {filteredPayment.map(order => (
                  <div key={order.id} className="bg-white rounded-2xl shadow-sm border border-emerald-100 overflow-hidden hover:border-emerald-300 transition-colors">
                    <div className="flex items-center justify-between px-5 py-3 bg-emerald-50/50 border-b border-emerald-50">
                      <div className="flex items-center gap-2">
                        <span className="w-2 h-2 rounded-full bg-emerald-500"></span>
                        <span className="text-xs font-bold text-emerald-700 uppercase tracking-wide">
                          ID: #{order.id} Â· {order.paymentStatus === 'AprobadaPorCliente' ? 'Cliente Aprobó' : 'Esperando Aprobación Cliente'}
                        </span>
                      </div>
                      {order.total && (
                        <span className="text-lg font-black text-emerald-700 bg-emerald-100 px-3 py-1 rounded-xl border border-emerald-200 shadow-sm">${order.total.toFixed(2)}</span>
                      )}
                    </div>
                    <div className="p-5">
                      <h4 className="font-bold text-gray-900 text-lg mb-1">{order.serviceType || order.tipoServicio}</h4>
                      <p className="text-sm text-gray-600 flex items-center gap-2 mt-1">
                        <User className="w-4 h-4 text-gray-400" /> <span className="font-semibold">{order.clientName || order.clienteNombre || 'Sin cliente'}</span>
                      </p>
                      <p className="text-sm text-gray-600 flex items-center gap-2 mt-1">
                        <Wrench className="w-4 h-4 text-gray-400" />{order.vehicle || 'Vehículo'} Â· <span className="font-semibold">{order.licensePlate || order.vehiclePlaca}</span>
                      </p>

                      {order.paymentStatus === 'AprobadaPorCliente' && order.invoiceId ? (
                        <button
                          onClick={() => handleConfirmPayment(order.invoiceId!, order.id)}
                          disabled={submitting[order.id]}
                          className="mt-6 w-full flex items-center justify-center gap-2 py-3.5 bg-emerald-600 hover:bg-emerald-700 disabled:bg-emerald-300 text-white rounded-xl font-bold text-sm transition-colors shadow-lg shadow-emerald-600/20"
                        >
                          <DollarSign className="w-5 h-5" />
                          {submitting[order.id] ? 'Confirmando...' : 'Confirmar Recepción de Pago e Iniciar Mantenimiento'}
                        </button>
                      ) : (
                        <div className="mt-5 flex items-center gap-3 text-sm font-medium text-amber-700 bg-amber-50 rounded-xl p-4 border border-amber-100">
                          <Clock className="w-5 h-5 shrink-0" />
                          Esperando que el cliente apruebe la factura generada desde su plataforma...
                        </div>
                      )}
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>
        )}
      </section>

    </div>
  );
}



