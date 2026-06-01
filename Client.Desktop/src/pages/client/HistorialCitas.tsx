import { useEffect, useState } from 'react';
import { useSearchParams } from 'react-router-dom';
import {
  AlertCircle, CheckCircle, Clock, Wrench, Calendar,
  FileText, ChevronRight, Plus, DollarSign, XCircle, Package
} from 'lucide-react';
import AgendarCitaModal from '../../components/AgendarCitaModal';

const API = 'http://localhost:5219';

interface ServiceOrderDto {
  id: number;
  serviceType: string;
  vehicle: string;
  licensePlate: string;
  status: string;
  workPerformed: string | null;
  mechanic: string | null;
  entryDate: string;
  deliveryDate: string | null;
  scheduledDate: string | null;
}

interface ClientHistoryDto {
  history: ServiceOrderDto[];
  upcomingAppointments: ServiceOrderDto[];
}

interface InvoiceDto {
  id: number;
  invoiceNumber: string;
  invoiceDate: string;
  total: number;
  subtotal: number;
  taxes: number;
  paymentStatus: string;
  serviceOrderId: number;
  details: { concept: string; quantity: number; unitPrice: number; subtotal: number }[];
}

type Tab = 'historial' | 'Revisiones' | 'facturas';

const statusColors: Record<string, string> = {
  'Finalizado': 'bg-green-100 text-green-700',
  'Entregado': 'bg-emerald-100 text-emerald-700',
  'Pendiente': 'bg-amber-100 text-amber-700',
  'EnProceso': 'bg-blue-100 text-blue-700',
  'Programada': 'bg-indigo-100 text-indigo-700',
  'EsperandoAprobacionCliente': 'bg-purple-100 text-purple-700',
  'EsperandoPago': 'bg-orange-100 text-orange-700',
  'DiagnosticoCompletado': 'bg-cyan-100 text-cyan-700',
  'Cancelada': 'bg-red-100 text-red-700',
};

export default function HistorialCitas() {
  const [data, setData] = useState<ClientHistoryDto | null>(null);
  const [invoices, setInvoices] = useState<InvoiceDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [searchParams, setSearchParams] = useSearchParams();
  const activeTab: Tab = (searchParams.get('tab') as Tab) ?? 'historial';
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [expandedInvoice, setExpandedInvoice] = useState<number | null>(null);
  const [rejectReason, setRejectReason] = useState('');
  const [submitting, setSubmitting] = useState(false);
  const [msg, setMsg] = useState<{ type: 'ok' | 'err'; text: string } | null>(null);

  const setTab = (tab: Tab) => setSearchParams({ tab });
  const token = localStorage.getItem('token');
  const headers = { 'Content-Type': 'application/json', Authorization: `Bearer ${token}` };

  const showMsg = (type: 'ok' | 'err', text: string) => {
    setMsg({ type, text });
    setTimeout(() => setMsg(null), 4000);
  };

  const loadData = async () => {
    setLoading(true);
    try {
      const [histRes, invRes] = await Promise.all([
        fetch(`${API}/api/dashboard/client/historial`, { headers }),
        fetch(`${API}/api/dashboard/client/facturas`, { headers })
      ]);
      if (histRes.ok) setData(await histRes.json());
      else setError('No se pudo cargar el historial.');
      if (invRes.ok) {
        const invData = await invRes.json();
        setInvoices(invData.value || invData || []);
      }
    } catch {
      setError('Error de conexión.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { loadData(); }, []);

  const handleApproveInvoice = async (invoiceId: number) => {
    setSubmitting(true);
    try {
      const res = await fetch(`${API}/api/dashboard/client/facturas/${invoiceId}/aprobar`, {
        method: 'POST', headers
      });
      if (res.ok) {
        showMsg('ok', 'âœ… Factura aprobada. La recepcionista procederá con el pago.');
        loadData();
      } else {
        const e = await res.json();
        showMsg('err', e.message || 'Error al aprobar factura.');
      }
    } catch { showMsg('err', 'Error de conexión.'); }
    finally { setSubmitting(false); }
  };

  const handleRejectInvoice = async (invoiceId: number) => {
    if (!rejectReason) { showMsg('err', 'Ingresa un motivo para rechazar.'); return; }
    setSubmitting(true);
    try {
      const res = await fetch(`${API}/api/dashboard/client/facturas/${invoiceId}/rechazar`, {
        method: 'POST', headers,
        body: JSON.stringify({ reason: rejectReason })
      });
      if (res.ok) {
        showMsg('ok', 'âŒ Factura rechazada. La recepcionista será notificada.');
        setRejectReason('');
        loadData();
      } else {
        const e = await res.json();
        showMsg('err', e.message || 'Error al rechazar factura.');
      }
    } catch { showMsg('err', 'Error de conexión.'); }
    finally { setSubmitting(false); }
  };

  const handleCitaSuccess = () => {
    loadData();
    setTab('Revisiones');
  };

  const pendingInvoices = invoices.filter(i => i.paymentStatus === 'PendienteAprobacion');

  if (loading) return (
    <div className="flex items-center justify-center min-h-[50vh]">
      <div className="animate-spin w-10 h-10 border-4 border-blue-600 border-t-transparent rounded-full" />
    </div>
  );

  if (error) return (
    <div className="flex flex-col items-center justify-center min-h-[50vh] gap-4 text-center">
      <AlertCircle className="w-12 h-12 text-red-400" />
      <p className="text-gray-500">{error}</p>
    </div>
  );

  const currentList = activeTab === 'historial'
    ? (data?.history ?? [])
    : (data?.upcomingAppointments ?? []);

  return (
    <div className="max-w-5xl mx-auto space-y-6 pb-10">

      {/* Toast */}
      {msg && (
        <div className={`fixed top-6 right-6 z-50 px-5 py-3 rounded-xl font-semibold shadow-xl animate-fade-in ${
          msg.type === 'ok' ? 'bg-emerald-500 text-white' : 'bg-red-500 text-white'
        }`}>
          {msg.text}
        </div>
      )}

      {/* Header */}
      <div className="flex justify-between items-end">
        <div>
          <h2 className="text-2xl font-black text-gray-900">Historial y Revisiones</h2>
          <p className="text-sm text-gray-500 mt-1">Revisa tus servicios, Revisiones y facturas pendientes</p>
        </div>
        <button
          onClick={() => setIsModalOpen(true)}
          className="flex items-center gap-2 px-5 py-2.5 bg-blue-600 hover:bg-blue-700 text-white text-sm font-bold rounded-xl shadow-md shadow-blue-500/20 transition-all"
        >
          <Plus className="w-5 h-5" /> Agendar Revisión
        </button>
      </div>

      {/* Alert facturas pendientes */}
      {pendingInvoices.length > 0 && (
        <div className="bg-purple-50 border border-purple-200 rounded-2xl p-4 flex items-center gap-3">
          <DollarSign className="w-6 h-6 text-purple-600 shrink-0" />
          <div className="flex-1">
            <p className="font-bold text-purple-800">Tienes {pendingInvoices.length} factura{pendingInvoices.length > 1 ? 's' : ''} pendiente{pendingInvoices.length > 1 ? 's' : ''} de aprobación</p>
            <p className="text-sm text-purple-600">Revisa y aprueba la factura en la pestaña Facturas para continuar con tu mantenimiento.</p>
          </div>
          <button
            onClick={() => setTab('facturas')}
            className="shrink-0 px-4 py-2 bg-purple-600 text-white text-sm font-bold rounded-xl hover:bg-purple-700 transition-colors"
          >
            Ver Facturas
          </button>
        </div>
      )}

      {/* Tabs */}
      <div className="bg-white rounded-2xl p-1.5 shadow-sm border border-gray-100 inline-flex gap-1">
        {[
          { id: 'historial', label: 'Historial', Icon: FileText, count: data?.history.length ?? 0 },
          { id: 'Revisiones', label: 'Próximas Revisiones', Icon: Calendar, count: data?.upcomingAppointments.length ?? 0 },
          { id: 'facturas', label: 'Facturas', Icon: DollarSign, count: invoices.length, badge: pendingInvoices.length }
        ].map(({ id, label, Icon, count, badge }) => (
          <button
            key={id}
            onClick={() => setTab(id as Tab)}
            className={`relative flex items-center gap-2 px-5 py-2.5 rounded-xl text-sm font-semibold transition-all duration-200 ${
              activeTab === id ? 'bg-gray-900 text-white shadow-md' : 'text-gray-500 hover:text-gray-800'
            }`}
          >
            <Icon className="w-4 h-4" />
            {label}
            {count > 0 && (
              <span className={`text-xs px-2 py-0.5 rounded-full font-bold ${
                activeTab === id ? 'bg-white/20 text-white' : 'bg-gray-100 text-gray-600'
              }`}>{count}</span>
            )}
            {badge && badge > 0 && (
              <span className="absolute -top-1 -right-1 w-4 h-4 bg-red-500 text-white text-xs font-black rounded-full flex items-center justify-center">
                {badge}
              </span>
            )}
          </button>
        ))}
      </div>

      {/* â•â•â• FACTURAS TAB â•â•â• */}
      {activeTab === 'facturas' && (
        <div className="space-y-4">
          {invoices.length === 0 ? (
            <div className="bg-white rounded-2xl p-12 border border-gray-100 shadow-sm flex flex-col items-center text-center">
              <DollarSign className="w-12 h-12 text-gray-300 mb-3" />
              <p className="text-gray-500 font-medium">No tienes facturas aún</p>
            </div>
          ) : (
            invoices.map(inv => {
              const isPending = inv.paymentStatus === 'PendienteAprobacion';
              const isApproved = inv.paymentStatus === 'AprobadaPorCliente';
              const isPaid = inv.paymentStatus === 'Pagada';
              const isRejected = inv.paymentStatus === 'Rechazada';
              const isExpanded = expandedInvoice === inv.id;

              return (
                <div key={inv.id} className={`bg-white rounded-2xl shadow-sm overflow-hidden border ${
                  isPending ? 'border-purple-200' : isApproved ? 'border-orange-200' : isPaid ? 'border-green-200' : 'border-gray-100'
                }`}>
                  {/* Status bar */}
                  <div className={`px-5 py-3 flex items-center justify-between ${
                    isPending ? 'bg-purple-50' : isApproved ? 'bg-orange-50' : isPaid ? 'bg-green-50' : 'bg-gray-50'
                  }`}>
                    <div className="flex items-center gap-2">
                      <span className={`w-2 h-2 rounded-full ${
                        isPending ? 'bg-purple-500' : isApproved ? 'bg-orange-400' : isPaid ? 'bg-green-500' : 'bg-gray-400'
                      }`}></span>
                      <span className={`text-xs font-bold uppercase tracking-wide ${
                        isPending ? 'text-purple-700' : isApproved ? 'text-orange-700' : isPaid ? 'text-green-700' : 'text-gray-600'
                      }`}>
                        {isPending ? 'âš¡ Pendiente de tu aprobación' :
                         isApproved ? 'â³ Esperando confirmación de pago' :
                         isPaid ? 'âœ… Pagada' : isRejected ? 'âŒ Rechazada' : inv.paymentStatus}
                      </span>
                    </div>
                    <span className="text-sm font-black text-gray-800">${inv.total.toFixed(2)}</span>
                  </div>

                  <div className="p-5">
                    <div className="flex items-start justify-between gap-4">
                      <div>
                        <p className="font-bold text-gray-900">{inv.invoiceNumber}</p>
                        <p className="text-sm text-gray-500 mt-0.5">{new Date(inv.invoiceDate).toLocaleDateString()}</p>
                      </div>
                      <button
                        onClick={() => setExpandedInvoice(isExpanded ? null : inv.id)}
                        className="text-sm text-blue-600 hover:text-blue-700 font-semibold flex items-center gap-1"
                      >
                        <Package className="w-4 h-4" />
                        {isExpanded ? 'Ocultar' : 'Ver detalle'}
                      </button>
                    </div>

                    {isExpanded && (
                      <div className="mt-4 bg-gray-50 rounded-xl border border-gray-100 overflow-hidden">
                        <table className="w-full text-sm">
                          <thead>
                            <tr className="bg-gray-100 text-gray-600 text-xs font-bold">
                              <th className="px-4 py-2 text-left">Concepto</th>
                              <th className="px-4 py-2 text-center">Cant.</th>
                              <th className="px-4 py-2 text-right">Precio</th>
                              <th className="px-4 py-2 text-right">Total</th>
                            </tr>
                          </thead>
                          <tbody className="divide-y divide-gray-100">
                            {inv.details.map((d, i) => (
                              <tr key={i} className="hover:bg-gray-50">
                                <td className="px-4 py-2.5 text-gray-700">{d.concept}</td>
                                <td className="px-4 py-2.5 text-center text-gray-600">{d.quantity}</td>
                                <td className="px-4 py-2.5 text-right text-gray-600">${d.unitPrice.toFixed(2)}</td>
                                <td className="px-4 py-2.5 text-right font-semibold text-gray-800">${d.subtotal.toFixed(2)}</td>
                              </tr>
                            ))}
                          </tbody>
                          <tfoot className="bg-gray-50 border-t border-gray-200 text-sm">
                            <tr>
                              <td colSpan={3} className="px-4 py-2 text-right font-semibold text-gray-600">Subtotal</td>
                              <td className="px-4 py-2 text-right font-semibold text-gray-800">${inv.subtotal.toFixed(2)}</td>
                            </tr>
                            <tr>
                              <td colSpan={3} className="px-4 py-2 text-right text-gray-500">IVA (19%)</td>
                              <td className="px-4 py-2 text-right text-gray-600">${inv.taxes.toFixed(2)}</td>
                            </tr>
                            <tr className="font-black">
                              <td colSpan={3} className="px-4 py-3 text-right text-gray-900 text-base">TOTAL</td>
                              <td className="px-4 py-3 text-right text-gray-900 text-base">${inv.total.toFixed(2)}</td>
                            </tr>
                          </tfoot>
                        </table>
                      </div>
                    )}

                    {/* Acciones solo si pendiente */}
                    {isPending && (
                      <div className="mt-4 pt-4 border-t border-gray-100 space-y-3">
                        <p className="text-sm text-gray-600 font-medium">¿Apruebas esta factura para iniciar el mantenimiento?</p>
                        <div className="flex gap-3 flex-wrap">
                          <button
                            onClick={() => handleApproveInvoice(inv.id)}
                            disabled={submitting}
                            className="flex items-center gap-2 px-5 py-2.5 bg-green-600 hover:bg-green-700 text-white font-bold text-sm rounded-xl transition-colors disabled:opacity-60 shadow-sm"
                          >
                            <CheckCircle className="w-4 h-4" /> Aprobar Factura
                          </button>
                          <div className="flex gap-2 flex-1 min-w-0">
                            <input
                              type="text"
                              value={rejectReason}
                              onChange={e => setRejectReason(e.target.value)}
                              placeholder="Motivo de rechazo..."
                              className="flex-1 min-w-0 border border-gray-200 bg-gray-50 rounded-xl px-3 py-2 text-sm outline-none focus:ring-2 focus:ring-red-500/30"
                            />
                            <button
                              onClick={() => handleRejectInvoice(inv.id)}
                              disabled={submitting}
                              className="flex items-center gap-2 px-4 py-2 bg-red-50 text-red-600 font-bold text-sm rounded-xl border border-red-100 hover:bg-red-100 transition-colors disabled:opacity-60 shrink-0"
                            >
                              <XCircle className="w-4 h-4" /> Rechazar
                            </button>
                          </div>
                        </div>
                      </div>
                    )}
                  </div>
                </div>
              );
            })
          )}
        </div>
      )}

      {/* â•â•â• HISTORIAL / Revisiones TABS â•â•â• */}
      {(activeTab === 'historial' || activeTab === 'Revisiones') && (
        currentList.length === 0 ? (
          <div className="bg-white rounded-2xl p-12 shadow-sm border border-gray-100 flex flex-col items-center text-center">
            <div className="w-16 h-16 bg-gray-50 rounded-full flex items-center justify-center mb-5">
              {activeTab === 'historial' ? <FileText className="w-8 h-8 text-gray-300" /> : <Calendar className="w-8 h-8 text-gray-300" />}
            </div>
            <h3 className="text-lg font-bold text-gray-700 mb-2">
              {activeTab === 'historial' ? 'Sin historial de servicios' : 'Sin Revisiones programadas'}
            </h3>
            <p className="text-gray-400 text-sm max-w-xs">
              {activeTab === 'historial' ? 'Los servicios completados aparecerán aquí.' : 'Cuando el taller registre una Revisión para ti, aparecerá aquí.'}
            </p>
          </div>
        ) : (
          <div className="space-y-3">
            {currentList.map(order => {
              const statusCls = statusColors[order.status] ?? 'bg-gray-100 text-gray-600';
              return (
                <div key={order.id} className="bg-white rounded-2xl p-5 shadow-sm border border-gray-100 hover:shadow-md transition-shadow">
                  <div className="flex items-start gap-4">
                    <div className={`w-11 h-11 rounded-xl flex items-center justify-center shrink-0 ${
                      activeTab === 'historial' ? 'bg-green-50' : 'bg-blue-50'
                    }`}>
                      {activeTab === 'historial'
                        ? <CheckCircle className="w-6 h-6 text-green-500" />
                        : <Clock className="w-6 h-6 text-blue-500" />}
                    </div>
                    <div className="flex-1 min-w-0">
                      <div className="flex items-center gap-3 flex-wrap">
                        <h4 className="font-bold text-gray-800">{order.serviceType}</h4>
                        <span className={`text-xs font-semibold px-2.5 py-0.5 rounded-full ${statusCls}`}>
                          {order.status?.replace(/([A-Z])/g, ' $1').trim()}
                        </span>
                      </div>
                      <p className="text-sm text-gray-500 mt-1 flex items-center gap-1.5">
                        <Wrench className="w-3.5 h-3.5 shrink-0" />
                        {order.vehicle} Â· <span className="font-semibold">{order.licensePlate}</span>
                      </p>
                      {order.workPerformed && (
                        <p className="text-sm text-gray-400 mt-2 line-clamp-2">{order.workPerformed}</p>
                      )}
                      <div className="flex flex-wrap gap-4 mt-2 text-xs text-gray-400 font-medium">
                        <span>ðŸ“… {order.entryDate}</span>
                        {order.scheduledDate && <span>ðŸ—“ <span className="text-blue-600">{order.scheduledDate}</span></span>}
                        {order.deliveryDate && <span>âœ… {order.deliveryDate}</span>}
                        {order.mechanic && <span>ðŸ”§ {order.mechanic}</span>}
                      </div>
                    </div>
                    <ChevronRight className="w-4 h-4 text-gray-300 shrink-0 mt-1" />
                  </div>
                </div>
              );
            })}
          </div>
        )
      )}

      <AgendarCitaModal
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        onSuccess={handleCitaSuccess}
      />
    </div>
  );
}


