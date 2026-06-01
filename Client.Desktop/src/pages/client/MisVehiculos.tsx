import { useEffect, useState, useRef } from 'react';
import { Car, AlertCircle, CheckCircle, Plus, X, ChevronRight, ChevronLeft, Upload, Fuel, Settings, FileText, Camera, Loader2 } from 'lucide-react';

// ─── Types ───────────────────────────────────────────────────────────────────

interface ClientVehicle {
  id: number; brand: string; model: string; year: number;
  color: string; licensePlate: string; mileage: number;
  activeStatus: string | null; activeOrderId: number | null;
}

interface Catalogs {
  brands: { id: number; name: string }[];
  models: { id: number; brandId: number; name: string }[];
  colors: { id: number; name: string }[];
}

interface FormData {
  brandId: number | ''; modelId: number | ''; year: number | '';
  colorId: number | ''; fuelType: string; bodyType: string;
  mileage: number | ''; licensePlate: string; vin: string;
  engineNumber: string; notes: string; photos: string[];
}

// ─── Constants ────────────────────────────────────────────────────────────────

const FUEL_TYPES  = ['Gasolina', 'Diésel', 'Eléctrico', 'Híbrido', 'Gas Natural', 'Otro'];
const BODY_TYPES  = ['Sedán', 'SUV', 'Pick-up', 'Hatchback', 'Coupé', 'Convertible', 'Minivan', 'Furgoneta', 'Camión', 'Otro'];
const STEPS = ['Identificación', 'Detalles', 'Matrícula', 'Fotos', 'Confirmar'];

const statusConfig: Record<string, { label: string; color: string }> = {
  'Pendiente':           { label: 'Pendiente',          color: 'bg-amber-100 text-amber-700' },
  'En Reparación':       { label: 'En Reparación',       color: 'bg-blue-100 text-blue-700' },
  'En Pruebas':          { label: 'En Pruebas',          color: 'bg-purple-100 text-purple-700' },
  'Listo para Entregar': { label: 'Listo para Entregar', color: 'bg-green-100 text-green-700' },
};

const API = 'http://localhost:5219/api/dashboard/client';

// ─── Step indicator ───────────────────────────────────────────────────────────

function StepIndicator({ step, total }: { step: number; total: number }) {
  return (
    <div className="flex items-center justify-center gap-1.5 mb-8">
      {Array.from({ length: total }).map((_, i) => (
        <div key={i} className={`h-1.5 rounded-full transition-all duration-300 ${
          i < step ? 'bg-blue-600 w-8' : i === step ? 'bg-blue-600 w-12' : 'bg-gray-200 w-4'
        }`} />
      ))}
    </div>
  );
}

// ─── Vehicle Card ─────────────────────────────────────────────────────────────

function VehicleCard({ v }: { v: ClientVehicle }) {
  const status = v.activeStatus ? statusConfig[v.activeStatus] : null;
  return (
    <div className="bg-white rounded-3xl p-6 shadow-sm border border-gray-100 hover:shadow-md transition-all group relative overflow-hidden">
      <div className="absolute -top-8 -right-8 w-32 h-32 bg-blue-50 rounded-full opacity-0 group-hover:opacity-60 transition-opacity duration-500" />
      {status ? (
        <div className={`inline-flex items-center gap-1.5 px-3 py-1 rounded-full text-xs font-semibold mb-4 ${status.color}`}>
          {status.label}
        </div>
      ) : (
        <div className="inline-flex items-center gap-1.5 px-3 py-1 rounded-full text-xs font-semibold mb-4 bg-gray-100 text-gray-500">
          <CheckCircle className="w-3.5 h-3.5" /> Sin servicio activo
        </div>
      )}
      <div className="w-14 h-14 bg-gradient-to-br from-blue-500 to-indigo-600 rounded-2xl flex items-center justify-center mb-4 shadow-md">
        <Car className="w-7 h-7 text-white" />
      </div>
      <h3 className="text-lg font-extrabold text-gray-800">{v.brand} {v.model}</h3>
      <p className="text-gray-400 text-sm font-medium">{v.year}</p>
      <div className="mt-5 pt-5 border-t border-gray-50 grid grid-cols-2 gap-3">
        <div><p className="text-xs text-gray-400 uppercase tracking-wide font-semibold">Placa</p>
          <p className="text-sm font-bold text-gray-700 mt-0.5">{v.licensePlate}</p></div>
        <div><p className="text-xs text-gray-400 uppercase tracking-wide font-semibold">Color</p>
          <p className="text-sm font-bold text-gray-700 mt-0.5">{v.color}</p></div>
        <div className="col-span-2"><p className="text-xs text-gray-400 uppercase tracking-wide font-semibold">Kilometraje</p>
          <p className="text-sm font-bold text-gray-700 mt-0.5">{v.mileage.toLocaleString()} km</p></div>
      </div>
    </div>
  );
}

// ─── Main Component ───────────────────────────────────────────────────────────

export default function MisVehiculos() {
  const [vehicles, setVehicles] = useState<ClientVehicle[]>([]);
  const [catalogs, setCatalogs] = useState<Catalogs | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [showModal, setShowModal] = useState(false);
  const [step, setStep] = useState(0);
  const [saving, setSaving] = useState(false);
  const [saveError, setSaveError] = useState('');
  const [form, setForm] = useState<FormData>({
    brandId: '', modelId: '', year: '', colorId: '',
    fuelType: '', bodyType: '', mileage: '',
    licensePlate: '', vin: '', engineNumber: '', notes: '', photos: []
  });
  const fileInputRef = useRef<HTMLInputElement>(null);

  // Fetch vehicles and catalogs
  useEffect(() => {
    const token = localStorage.getItem('token');
    const headers = { 'Authorization': `Bearer ${token}` };
    Promise.all([
      fetch(`${API}/vehiculos`, { headers }).then(r => r.ok ? r.json() : []),
      fetch(`${API}/catalogos`).then(r => r.ok ? r.json() : null),
    ]).then(([v, c]) => {
      setVehicles(v);
      setCatalogs(c);
    }).catch(() => setError('Error cargando datos.')).finally(() => setLoading(false));
  }, []);

  const filteredModels = catalogs?.models.filter(m => m.brandId === Number(form.brandId)) ?? [];

  const setField = (key: keyof FormData, val: unknown) =>
    setForm(f => ({ ...f, [key]: val }));

  const handlePhotos = (e: React.ChangeEvent<HTMLInputElement>) => {
    const files = Array.from(e.target.files ?? []);
    const remaining = 5 - form.photos.length;
    files.slice(0, remaining).forEach(file => {
      const reader = new FileReader();
      reader.onload = ev => setForm(f => ({ ...f, photos: [...f.photos, ev.target!.result as string] }));
      reader.readAsDataURL(file);
    });
  };

  const removePhoto = (i: number) =>
    setForm(f => ({ ...f, photos: f.photos.filter((_, idx) => idx !== i) }));

  const canNext = () => {
    if (step === 0) return form.brandId !== '' && form.modelId !== '' && form.year !== '' && form.colorId !== '';
    if (step === 1) return form.fuelType !== '' && form.bodyType !== '' && form.mileage !== '';
    if (step === 2) return form.licensePlate.trim().length >= 3;
    return true;
  };

  const openModal = () => {
    setForm({ brandId: '', modelId: '', year: '', colorId: '', fuelType: '', bodyType: '', mileage: '', licensePlate: '', vin: '', engineNumber: '', notes: '', photos: [] });
    setStep(0); setSaveError(''); setShowModal(true);
  };

  const handleSubmit = async () => {
    setSaving(true); setSaveError('');
    try {
      const token = localStorage.getItem('token');
      const res = await fetch(`${API}/vehiculos`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json', 'Authorization': `Bearer ${token}` },
        body: JSON.stringify({
          brandId: Number(form.brandId), modelId: Number(form.modelId),
          year: Number(form.year), colorId: Number(form.colorId),
          fuelType: form.fuelType, bodyType: form.bodyType,
          mileage: Number(form.mileage), licensePlate: form.licensePlate.trim().toUpperCase(),
          vin: form.vin?.trim() || null, engineNumber: form.engineNumber?.trim() || null,
          notes: form.notes?.trim() || null, photos: form.photos
        })
      });
      if (res.ok) {
        setShowModal(false);
        // Refresh vehicles list
        const vRes = await fetch(`${API}/vehiculos`, { headers: { 'Authorization': `Bearer ${token}` } });
        if (vRes.ok) setVehicles(await vRes.json());
      } else {
        const err = await res.json();
        setSaveError(err.message || 'Error al registrar el vehículo.');
      }
    } catch {
      setSaveError('Error de conexión.');
    } finally {
      setSaving(false);
    }
  };

  // ─── Render ─────────────────────────────────────────────────────────────────

  if (loading) return (
    <div className="flex items-center justify-center min-h-[50vh]">
      <div className="animate-spin w-10 h-10 border-4 border-blue-600 border-t-transparent rounded-full" />
    </div>
  );

  if (error) return (
    <div className="flex flex-col items-center justify-center min-h-[50vh] text-center gap-4">
      <AlertCircle className="w-12 h-12 text-red-400" />
      <p className="text-gray-500">{error}</p>
    </div>
  );

  const selectedBrand  = catalogs?.brands.find(b => b.id === Number(form.brandId));
  const selectedModel  = catalogs?.models.find(m => m.id === Number(form.modelId));
  const selectedColor  = catalogs?.colors.find(c => c.id === Number(form.colorId));

  return (
    <div className="max-w-6xl mx-auto space-y-8 pb-10">

      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-2xl font-bold text-gray-800">Mis Vehículos</h2>
          <p className="text-sm text-gray-500 mt-1">
            {vehicles.length === 0 ? 'No tienes vehículos registrados'
              : `${vehicles.length} vehículo${vehicles.length > 1 ? 's' : ''} registrado${vehicles.length > 1 ? 's' : ''}`}
          </p>
        </div>
        <button
          onClick={openModal}
          className="flex items-center gap-2 px-5 py-2.5 bg-blue-600 hover:bg-blue-700 text-white text-sm font-semibold rounded-xl shadow-md shadow-blue-200 transition-all active:scale-95"
        >
          <Plus className="w-4 h-4" /> Agregar Vehículo
        </button>
      </div>

      {/* Vehicle Grid */}
      {vehicles.length === 0 ? (
        <div className="bg-white rounded-3xl p-16 shadow-sm border border-gray-100 flex flex-col items-center text-center">
          <div className="w-20 h-20 bg-gray-50 rounded-full flex items-center justify-center mb-6">
            <Car className="w-10 h-10 text-gray-300" />
          </div>
          <h3 className="text-xl font-bold text-gray-700 mb-2">Sin vehículos registrados</h3>
          <p className="text-gray-400 text-sm max-w-xs mb-6">Registra tu primer vehículo para llevar un seguimiento completo de sus servicios.</p>
          <button onClick={openModal} className="flex items-center gap-2 px-5 py-2.5 bg-blue-600 hover:bg-blue-700 text-white text-sm font-semibold rounded-xl transition-all">
            <Plus className="w-4 h-4" /> Agregar mi primer vehículo
          </button>
        </div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {vehicles.map(v => <VehicleCard key={v.id} v={v} />)}
        </div>
      )}

      {/* ── Modal ── */}
      {showModal && (
        <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/40 backdrop-blur-sm">
          <div className="bg-white rounded-3xl shadow-2xl w-full max-w-lg max-h-[90vh] overflow-y-auto">

            {/* Modal Header */}
            <div className="sticky top-0 bg-white border-b border-gray-100 px-8 pt-8 pb-5 rounded-t-3xl z-10">
              <div className="flex items-center justify-between mb-1">
                <h3 className="text-xl font-extrabold text-gray-800">Agregar Vehículo</h3>
                <button onClick={() => setShowModal(false)} className="w-8 h-8 flex items-center justify-center text-gray-400 hover:text-gray-700 hover:bg-gray-100 rounded-lg transition-colors">
                  <X className="w-5 h-5" />
                </button>
              </div>
              <p className="text-sm text-gray-500">{STEPS[step]} · Paso {step + 1} de {STEPS.length}</p>
              <StepIndicator step={step} total={STEPS.length} />
            </div>

            <div className="px-8 pb-8">

              {/* ─ Step 0: Identification ─ */}
              {step === 0 && (
                <div className="space-y-5">
                  <div className="flex items-center gap-3 mb-2">
                    <div className="w-10 h-10 bg-blue-50 rounded-xl flex items-center justify-center">
                      <Car className="w-5 h-5 text-blue-600" />
                    </div>
                    <div>
                      <p className="font-bold text-gray-800">Identifica tu vehículo</p>
                      <p className="text-xs text-gray-500">Marca, modelo y año</p>
                    </div>
                  </div>

                  <div>
                    <label className="block text-sm font-semibold text-gray-700 mb-1.5">Marca *</label>
                    <select value={form.brandId} onChange={e => { setField('brandId', Number(e.target.value)); setField('modelId', ''); }}
                      className="w-full px-4 py-3 rounded-xl border border-gray-200 bg-white focus:ring-2 focus:ring-blue-500/50 focus:border-blue-500 outline-none text-gray-700">
                      <option value="">Seleccionar marca...</option>
                      {catalogs?.brands.map(b => <option key={b.id} value={b.id}>{b.name}</option>)}
                    </select>
                  </div>

                  <div>
                    <label className="block text-sm font-semibold text-gray-700 mb-1.5">Modelo *</label>
                    <select value={form.modelId} onChange={e => setField('modelId', Number(e.target.value))}
                      disabled={!form.brandId}
                      className="w-full px-4 py-3 rounded-xl border border-gray-200 bg-white focus:ring-2 focus:ring-blue-500/50 focus:border-blue-500 outline-none text-gray-700 disabled:opacity-50">
                      <option value="">Seleccionar modelo...</option>
                      {filteredModels.map(m => <option key={m.id} value={m.id}>{m.name}</option>)}
                    </select>
                  </div>

                  <div className="grid grid-cols-2 gap-4">
                    <div>
                      <label className="block text-sm font-semibold text-gray-700 mb-1.5">Año *</label>
                      <input type="number" min={1960} max={new Date().getFullYear() + 1}
                        placeholder={`${new Date().getFullYear()}`}
                        value={form.year} onChange={e => setField('year', e.target.value ? Number(e.target.value) : '')}
                        className="w-full px-4 py-3 rounded-xl border border-gray-200 focus:ring-2 focus:ring-blue-500/50 focus:border-blue-500 outline-none text-gray-700" />
                    </div>
                    <div>
                      <label className="block text-sm font-semibold text-gray-700 mb-1.5">Color *</label>
                      <select value={form.colorId} onChange={e => setField('colorId', Number(e.target.value))}
                        className="w-full px-4 py-3 rounded-xl border border-gray-200 bg-white focus:ring-2 focus:ring-blue-500/50 focus:border-blue-500 outline-none text-gray-700">
                        <option value="">Color...</option>
                        {catalogs?.colors.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}
                      </select>
                    </div>
                  </div>
                </div>
              )}

              {/* ─ Step 1: Details ─ */}
              {step === 1 && (
                <div className="space-y-5">
                  <div className="flex items-center gap-3 mb-2">
                    <div className="w-10 h-10 bg-green-50 rounded-xl flex items-center justify-center">
                      <Settings className="w-5 h-5 text-green-600" />
                    </div>
                    <div>
                      <p className="font-bold text-gray-800">Características</p>
                      <p className="text-xs text-gray-500">Combustible, carrocería y kilometraje</p>
                    </div>
                  </div>

                  <div>
                    <label className="block text-sm font-semibold text-gray-700 mb-1.5">Tipo de combustible *</label>
                    <div className="grid grid-cols-3 gap-2">
                      {FUEL_TYPES.map(ft => (
                        <button key={ft} type="button" onClick={() => setField('fuelType', ft)}
                          className={`py-2.5 px-3 rounded-xl text-sm font-semibold border-2 transition-all ${
                            form.fuelType === ft ? 'border-blue-600 bg-blue-50 text-blue-700' : 'border-gray-100 bg-gray-50 text-gray-600 hover:border-gray-200'
                          }`}>{ft}</button>
                      ))}
                    </div>
                  </div>

                  <div>
                    <label className="block text-sm font-semibold text-gray-700 mb-1.5">Tipo de carrocería *</label>
                    <div className="grid grid-cols-3 gap-2">
                      {BODY_TYPES.map(bt => (
                        <button key={bt} type="button" onClick={() => setField('bodyType', bt)}
                          className={`py-2.5 px-3 rounded-xl text-sm font-semibold border-2 transition-all ${
                            form.bodyType === bt ? 'border-blue-600 bg-blue-50 text-blue-700' : 'border-gray-100 bg-gray-50 text-gray-600 hover:border-gray-200'
                          }`}>{bt}</button>
                      ))}
                    </div>
                  </div>

                  <div>
                    <label className="block text-sm font-semibold text-gray-700 mb-1.5">Kilometraje actual *</label>
                    <div className="relative">
                      <input type="number" min={0} placeholder="Ej: 45000"
                        value={form.mileage} onChange={e => setField('mileage', e.target.value ? Number(e.target.value) : '')}
                        className="w-full px-4 py-3 rounded-xl border border-gray-200 focus:ring-2 focus:ring-blue-500/50 focus:border-blue-500 outline-none text-gray-700 pr-14" />
                      <span className="absolute right-4 top-1/2 -translate-y-1/2 text-sm text-gray-400 font-semibold">km</span>
                    </div>
                  </div>
                </div>
              )}

              {/* ─ Step 2: Registration ─ */}
              {step === 2 && (
                <div className="space-y-5">
                  <div className="flex items-center gap-3 mb-2">
                    <div className="w-10 h-10 bg-purple-50 rounded-xl flex items-center justify-center">
                      <FileText className="w-5 h-5 text-purple-600" />
                    </div>
                    <div>
                      <p className="font-bold text-gray-800">Datos de registro</p>
                      <p className="text-xs text-gray-500">Matrícula, VIN y número de motor</p>
                    </div>
                  </div>

                  <div>
                    <label className="block text-sm font-semibold text-gray-700 mb-1.5">Matrícula / Placa *</label>
                    <input type="text" placeholder="Ej: ABC-123"
                      value={form.licensePlate}
                      onChange={e => setField('licensePlate', e.target.value.toUpperCase())}
                      className="w-full px-4 py-3 rounded-xl border border-gray-200 focus:ring-2 focus:ring-blue-500/50 focus:border-blue-500 outline-none text-gray-700 font-mono tracking-widest uppercase" />
                  </div>

                  <div>
                    <label className="block text-sm font-semibold text-gray-700 mb-1.5">
                      Número VIN <span className="text-gray-400 font-normal">(opcional)</span>
                    </label>
                    <input type="text" maxLength={17} placeholder="17 caracteres alfanuméricos"
                      value={form.vin} onChange={e => setField('vin', e.target.value.toUpperCase())}
                      className="w-full px-4 py-3 rounded-xl border border-gray-200 focus:ring-2 focus:ring-blue-500/50 focus:border-blue-500 outline-none text-gray-700 font-mono tracking-wider uppercase" />
                    {form.vin && form.vin.length > 0 && form.vin.length !== 17 && (
                      <p className="text-xs text-amber-500 mt-1">El VIN tiene {form.vin.length}/17 caracteres</p>
                    )}
                  </div>

                  <div>
                    <label className="block text-sm font-semibold text-gray-700 mb-1.5">
                      Número de Motor <span className="text-gray-400 font-normal">(opcional)</span>
                    </label>
                    <input type="text" placeholder="Ej: 1NZ-FE12345"
                      value={form.engineNumber} onChange={e => setField('engineNumber', e.target.value)}
                      className="w-full px-4 py-3 rounded-xl border border-gray-200 focus:ring-2 focus:ring-blue-500/50 focus:border-blue-500 outline-none text-gray-700 font-mono" />
                  </div>

                  <div>
                    <label className="block text-sm font-semibold text-gray-700 mb-1.5">
                      Notas adicionales <span className="text-gray-400 font-normal">(opcional)</span>
                    </label>
                    <textarea rows={3} placeholder="Estado del vehículo, accesorios, observaciones..."
                      value={form.notes} onChange={e => setField('notes', e.target.value)}
                      className="w-full px-4 py-3 rounded-xl border border-gray-200 focus:ring-2 focus:ring-blue-500/50 focus:border-blue-500 outline-none text-gray-700 resize-none" />
                  </div>
                </div>
              )}

              {/* ─ Step 3: Photos ─ */}
              {step === 3 && (
                <div className="space-y-5">
                  <div className="flex items-center gap-3 mb-2">
                    <div className="w-10 h-10 bg-orange-50 rounded-xl flex items-center justify-center">
                      <Camera className="w-5 h-5 text-orange-600" />
                    </div>
                    <div>
                      <p className="font-bold text-gray-800">Fotos del vehículo</p>
                      <p className="text-xs text-gray-500">Hasta 5 fotos · Opcional</p>
                    </div>
                  </div>

                  {/* Upload area */}
                  {form.photos.length < 5 && (
                    <button type="button" onClick={() => fileInputRef.current?.click()}
                      className="w-full h-36 border-2 border-dashed border-gray-200 rounded-2xl flex flex-col items-center justify-center gap-2 hover:border-blue-400 hover:bg-blue-50/50 transition-all group">
                      <Upload className="w-7 h-7 text-gray-300 group-hover:text-blue-400 transition-colors" />
                      <p className="text-sm text-gray-400 group-hover:text-blue-500 font-medium">Haz clic o arrastra fotos aquí</p>
                      <p className="text-xs text-gray-300">{5 - form.photos.length} foto{5 - form.photos.length > 1 ? 's' : ''} restante{5 - form.photos.length > 1 ? 's' : ''}</p>
                    </button>
                  )}
                  <input ref={fileInputRef} type="file" accept="image/*" multiple className="hidden" onChange={handlePhotos} />

                  {/* Photo previews */}
                  {form.photos.length > 0 && (
                    <div className="grid grid-cols-3 gap-3">
                      {form.photos.map((photo, i) => (
                        <div key={i} className="relative aspect-square rounded-2xl overflow-hidden group shadow-sm">
                          <img src={photo} alt={`Foto ${i + 1}`} className="w-full h-full object-cover" />
                          {i === 0 && (
                            <div className="absolute bottom-1.5 left-1.5 px-2 py-0.5 bg-blue-600 text-white text-[10px] font-bold rounded-full">Principal</div>
                          )}
                          <button type="button" onClick={() => removePhoto(i)}
                            className="absolute top-1.5 right-1.5 w-6 h-6 bg-black/60 text-white rounded-full flex items-center justify-center opacity-0 group-hover:opacity-100 transition-opacity hover:bg-red-500">
                            <X className="w-3.5 h-3.5" />
                          </button>
                        </div>
                      ))}
                    </div>
                  )}
                </div>
              )}

              {/* ─ Step 4: Confirm ─ */}
              {step === 4 && (
                <div className="space-y-5">
                  <div className="flex items-center gap-3 mb-2">
                    <div className="w-10 h-10 bg-emerald-50 rounded-xl flex items-center justify-center">
                      <CheckCircle className="w-5 h-5 text-emerald-600" />
                    </div>
                    <div>
                      <p className="font-bold text-gray-800">Confirma tu registro</p>
                      <p className="text-xs text-gray-500">Revisa los datos antes de guardar</p>
                    </div>
                  </div>

                  <div className="bg-gray-50 rounded-2xl p-5 space-y-3">
                    {[
                      ['Marca', selectedBrand?.name],
                      ['Modelo', selectedModel?.name],
                      ['Año', String(form.year)],
                      ['Color', selectedColor?.name],
                      ['Combustible', form.fuelType],
                      ['Carrocería', form.bodyType],
                      ['Kilometraje', `${Number(form.mileage).toLocaleString()} km`],
                      ['Matrícula', form.licensePlate],
                      form.vin ? ['VIN', form.vin] : null,
                      form.engineNumber ? ['N° Motor', form.engineNumber] : null,
                      form.notes ? ['Notas', form.notes] : null,
                    ].filter(Boolean).map(([label, val]) => (
                      <div key={label} className="flex items-start gap-3">
                        <span className="text-xs font-bold text-gray-400 uppercase tracking-wide w-28 shrink-0 pt-0.5">{label}</span>
                        <span className="text-sm font-semibold text-gray-800 flex-1">{val}</span>
                      </div>
                    ))}
                    <div className="flex items-start gap-3 pt-1 border-t border-gray-200">
                      <span className="text-xs font-bold text-gray-400 uppercase tracking-wide w-28 shrink-0 pt-0.5">Fotos</span>
                      <span className="text-sm font-semibold text-gray-800">{form.photos.length} foto{form.photos.length !== 1 ? 's' : ''}</span>
                    </div>
                  </div>

                  {saveError && (
                    <div className="flex items-start gap-3 bg-red-50 border border-red-200 rounded-xl p-4">
                      <AlertCircle className="w-5 h-5 text-red-500 shrink-0 mt-0.5" />
                      <p className="text-sm text-red-700">{saveError}</p>
                    </div>
                  )}
                </div>
              )}

              {/* ── Navigation Buttons ─────────────────────────────────────── */}
              <div className="flex items-center justify-between mt-8 pt-6 border-t border-gray-100">
                <button type="button" onClick={() => step === 0 ? setShowModal(false) : setStep(s => s - 1)}
                  className="flex items-center gap-2 px-4 py-2.5 text-gray-500 hover:text-gray-800 hover:bg-gray-100 rounded-xl font-semibold text-sm transition-colors">
                  <ChevronLeft className="w-4 h-4" />
                  {step === 0 ? 'Cancelar' : 'Anterior'}
                </button>

                {step < STEPS.length - 1 ? (
                  <button type="button" onClick={() => setStep(s => s + 1)} disabled={!canNext()}
                    className="flex items-center gap-2 px-6 py-2.5 bg-blue-600 hover:bg-blue-700 disabled:opacity-40 disabled:cursor-not-allowed text-white rounded-xl font-semibold text-sm transition-all">
                    Siguiente <ChevronRight className="w-4 h-4" />
                  </button>
                ) : (
                  <button type="button" onClick={handleSubmit} disabled={saving}
                    className="flex items-center gap-2 px-6 py-2.5 bg-emerald-600 hover:bg-emerald-700 disabled:opacity-50 text-white rounded-xl font-semibold text-sm transition-all shadow-md shadow-emerald-200">
                    {saving ? <><Loader2 className="w-4 h-4 animate-spin" /> Guardando...</> : <><CheckCircle className="w-4 h-4" /> Registrar Vehículo</>}
                  </button>
                )}
              </div>

            </div>
          </div>
        </div>
      )}
    </div>
  );
}
