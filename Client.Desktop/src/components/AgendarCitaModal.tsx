import React, { useState, useEffect } from 'react';
import { X, Calendar, Clock, Wrench } from 'lucide-react';

interface VehicleDto {
  id: number;
  brand: string;
  model: string;
  licensePlate: string;
}

interface ServiceTypeDto {
  id: number;
  name: string;
  estimatedDurationHours: number;
}

interface AgendarCitaModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSuccess: () => void;
}

export default function AgendarCitaModal({ isOpen, onClose, onSuccess }: AgendarCitaModalProps) {
  const [vehicles, setVehicles] = useState<VehicleDto[]>([]);
  const [serviceTypes, setServiceTypes] = useState<ServiceTypeDto[]>([]);
  const [loadingData, setLoadingData] = useState(true);
  
  const [vehicleId, setVehicleId] = useState('');
  const [serviceTypeId, setServiceTypeId] = useState('');
  const [date, setDate] = useState('');
  const [time, setTime] = useState('');
  const [observations, setObservations] = useState('');
  
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState('');

  useEffect(() => {
    if (!isOpen) return;
    
    const fetchData = async () => {
      setLoadingData(true);
      setError('');
      try {
        const token = localStorage.getItem('token');
        const [vehRes, srvRes] = await Promise.all([
          fetch('http://localhost:5219/api/dashboard/client/vehiculos', {
            headers: { 'Authorization': `Bearer ${token}` }
          }),
          fetch('http://localhost:5219/api/dashboard/client/service-types', {
            headers: { 'Authorization': `Bearer ${token}` }
          })
        ]);
        
        if (vehRes.ok) setVehicles(await vehRes.json());
        if (srvRes.ok) setServiceTypes(await srvRes.json());
      } catch (err) {
        setError('Error al cargar datos. Intente de nuevo.');
      } finally {
        setLoadingData(false);
      }
    };
    
    fetchData();
  }, [isOpen]);

  if (!isOpen) return null;

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!vehicleId || !serviceTypeId || !date || !time) {
      setError('Por favor complete todos los campos obligatorios.');
      return;
    }

    setSubmitting(true);
    setError('');

    try {
      const scheduledDate = new Date(`${date}T${time}`).toISOString();
      const token = localStorage.getItem('token');
      
      const res = await fetch('http://localhost:5219/api/dashboard/client/appointments', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`
        },
        body: JSON.stringify({
          vehicleId: Number(vehicleId),
          serviceTypeId: Number(serviceTypeId),
          scheduledDate,
          observations
        })
      });

      if (res.ok) {
        onSuccess();
        onClose();
        // Reset form
        setVehicleId('');
        setServiceTypeId('');
        setDate('');
        setTime('');
        setObservations('');
      } else {
        const data = await res.json();
        setError(data.message || 'Error al agendar la Revisión. Es posible que no haya mecánicos disponibles en ese horario.');
      }
    } catch (err) {
      setError('Error de conexión.');
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/50 backdrop-blur-sm">
      <div className="bg-white rounded-3xl w-full max-w-lg shadow-2xl overflow-hidden">
        <div className="px-6 py-4 border-b border-gray-100 flex justify-between items-center bg-gray-50/50">
          <h2 className="text-xl font-bold text-gray-800 flex items-center gap-2">
            <Calendar className="w-5 h-5 text-blue-600" />
            Agendar Nueva Revisión
          </h2>
          <button onClick={onClose} className="p-2 text-gray-400 hover:text-gray-600 hover:bg-gray-100 rounded-full transition-colors">
            <X className="w-5 h-5" />
          </button>
        </div>

        <div className="p-6">
          {error && (
            <div className="mb-6 p-4 bg-red-50 text-red-700 text-sm font-medium rounded-xl border border-red-100">
              {error}
            </div>
          )}

          {loadingData ? (
            <div className="flex justify-center py-10">
              <div className="animate-spin w-8 h-8 border-4 border-blue-600 border-t-transparent rounded-full" />
            </div>
          ) : (
            <form onSubmit={handleSubmit} className="space-y-5">
              <div>
                <label className="block text-sm font-semibold text-gray-700 mb-1">Vehículo *</label>
                <div className="relative">
                  <Wrench className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-gray-400" />
                  <select
                    value={vehicleId}
                    onChange={e => setVehicleId(e.target.value)}
                    className="w-full pl-10 pr-4 py-2.5 bg-gray-50 border border-gray-200 rounded-xl focus:ring-2 focus:ring-blue-500/20 focus:border-blue-500 outline-none transition-all"
                    required
                  >
                    <option value="">Seleccione un vehículo...</option>
                    {vehicles.map(v => (
                      <option key={v.id} value={v.id}>{v.brand} {v.model} - {v.licensePlate}</option>
                    ))}
                  </select>
                </div>
              </div>

              <div>
                <label className="block text-sm font-semibold text-gray-700 mb-1">Tipo de Servicio *</label>
                <select
                  value={serviceTypeId}
                  onChange={e => setServiceTypeId(e.target.value)}
                  className="w-full px-4 py-2.5 bg-gray-50 border border-gray-200 rounded-xl focus:ring-2 focus:ring-blue-500/20 focus:border-blue-500 outline-none transition-all"
                  required
                >
                  <option value="">Seleccione el servicio...</option>
                  {serviceTypes.map(s => (
                    <option key={s.id} value={s.id}>
                      {s.name} {s.estimatedDurationHours > 0 ? `(~${s.estimatedDurationHours}h)` : ''}
                    </option>
                  ))}
                </select>
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-semibold text-gray-700 mb-1">Día *</label>
                  <div className="relative">
                    <Calendar className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-gray-400" />
                    <input
                      type="date"
                      value={date}
                      onChange={e => setDate(e.target.value)}
                      min={new Date().toISOString().split('T')[0]}
                      className="w-full pl-10 pr-4 py-2.5 bg-gray-50 border border-gray-200 rounded-xl focus:ring-2 focus:ring-blue-500/20 focus:border-blue-500 outline-none transition-all"
                      required
                    />
                  </div>
                </div>
                <div>
                  <label className="block text-sm font-semibold text-gray-700 mb-1">Hora *</label>
                  <div className="relative">
                    <Clock className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-gray-400" />
                    <input
                      type="time"
                      value={time}
                      onChange={e => setTime(e.target.value)}
                      className="w-full pl-10 pr-4 py-2.5 bg-gray-50 border border-gray-200 rounded-xl focus:ring-2 focus:ring-blue-500/20 focus:border-blue-500 outline-none transition-all"
                      required
                    />
                  </div>
                </div>
              </div>

              <div>
                <label className="block text-sm font-semibold text-gray-700 mb-1">Observaciones / Motivo Adicional</label>
                <textarea
                  value={observations}
                  onChange={e => setObservations(e.target.value)}
                  placeholder="Describe qué le pasa al vehículo o si tienes alguna petición especial..."
                  rows={3}
                  className="w-full px-4 py-3 bg-gray-50 border border-gray-200 rounded-xl focus:ring-2 focus:ring-blue-500/20 focus:border-blue-500 outline-none transition-all resize-none"
                />
              </div>

              <div className="pt-4">
                <button
                  type="submit"
                  disabled={submitting || vehicles.length === 0}
                  className="w-full py-3 px-4 bg-blue-600 hover:bg-blue-700 disabled:bg-blue-300 text-white font-bold rounded-xl shadow-md shadow-blue-500/20 transition-all flex justify-center items-center gap-2"
                >
                  {submitting ? (
                    <><div className="w-5 h-5 border-2 border-white/30 border-t-white rounded-full animate-spin" /> Procesando...</>
                  ) : (
                    'Confirmar Revisión'
                  )}
                </button>
              </div>
            </form>
          )}
        </div>
      </div>
    </div>
  );
}


