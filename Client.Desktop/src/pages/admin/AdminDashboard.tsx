import React, { useState, useEffect } from 'react';
import { Users, Car, CalendarClock, DollarSign, TrendingUp } from 'lucide-react';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer, AreaChart, Area } from 'recharts';

interface ChartData {
  name: string;
  value: number;
}

interface AdminStatsDto {
  totalUsers: number;
  totalVehicles: number;
  totalAppointments: number;
  totalRevenue: number;
  appointmentsByDay: ChartData[];
  appointmentsByWeek: ChartData[];
  appointmentsByMonth: ChartData[];
  appointmentsByYear: ChartData[];
}

export default function AdminDashboard() {
  const [stats, setStats] = useState<AdminStatsDto | null>(null);
  const [period, setPeriod] = useState<'day' | 'week' | 'month' | 'year'>('month');
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    fetchStats();
  }, []);

  const fetchStats = async () => {
    setLoading(true);
    setError(null);
    try {
      const token = localStorage.getItem('token');
      const res = await fetch('http://localhost:5219/api/admin/dashboard/stats', {
        headers: { 'Authorization': `Bearer ${token}` }
      });
      if (res.ok) {
        setStats(await res.json());
      } else {
        setError(`Error del servidor: ${res.status}`);
      }
    } catch (e: any) {
      console.error(e);
      setError(e.message || 'Error de red al conectar con el servidor.');
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center h-[60vh]">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-indigo-600"></div>
      </div>
    );
  }

  if (error || !stats) {
    return (
      <div className="flex flex-col items-center justify-center h-[60vh] space-y-4">
        <div className="text-red-500 flex flex-col items-center">
          <svg className="w-16 h-16 mb-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
          </svg>
          <h3 className="text-xl font-bold">Ups, algo salió mal</h3>
          <p className="text-sm font-medium opacity-80">{error || 'No se pudo cargar la información del servidor.'}</p>
        </div>
        <button onClick={fetchStats} className="px-4 py-2 bg-indigo-50 text-indigo-700 font-bold rounded-xl hover:bg-indigo-100 transition-colors">
          Reintentar
        </button>
      </div>
    );
  }

  const getChartData = () => {
    switch (period) {
      case 'day': return stats.appointmentsByDay;
      case 'week': return stats.appointmentsByWeek;
      case 'month': return stats.appointmentsByMonth;
      case 'year': return stats.appointmentsByYear;
    }
  };

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-2xl font-black tracking-tight text-gray-900">Resumen Estadístico</h2>
          <p className="text-sm text-gray-500 font-medium">Métricas globales y evolución operativa</p>
        </div>
        <button onClick={fetchStats} className="px-4 py-2 bg-indigo-50 text-indigo-700 font-bold rounded-xl hover:bg-indigo-100 transition-colors">
          Actualizar Datos
        </button>
      </div>

      {/* KPIs */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        <StatCard icon={<Users className="text-blue-500" />} title="Usuarios" value={stats.totalUsers} trend="+12% este mes" />
        <StatCard icon={<Car className="text-emerald-500" />} title="Vehículos" value={stats.totalVehicles} trend="+5% este mes" />
        <StatCard icon={<CalendarClock className="text-orange-500" />} title="Revisiones Históricas" value={stats.totalAppointments} trend="Operativo" />
        <StatCard icon={<DollarSign className="text-indigo-500" />} title="Ingresos Brutos" value={`$${stats.totalRevenue.toLocaleString()}`} trend="Cierre de órdenes" />
      </div>

      {/* Charts */}
      <div className="bg-white p-6 rounded-2xl shadow-sm border border-gray-100">
        <div className="flex items-center justify-between mb-8">
          <h3 className="text-lg font-bold text-gray-800">Evolución de Revisiones Agendadas</h3>
          
          <div className="flex bg-gray-100 p-1 rounded-lg">
            {(['day', 'week', 'month', 'year'] as const).map(p => (
              <button
                key={p}
                onClick={() => setPeriod(p)}
                className={`px-4 py-1.5 rounded-md text-sm font-bold capitalize transition-all ${
                  period === p ? 'bg-white shadow-sm text-indigo-700' : 'text-gray-500 hover:text-gray-700'
                }`}
              >
                {p === 'day' ? 'Días' : p === 'week' ? 'Semanas' : p === 'month' ? 'Meses' : 'Años'}
              </button>
            ))}
          </div>
        </div>

        <div className="h-80 w-full">
          <ResponsiveContainer width="100%" height="100%">
            <AreaChart data={getChartData()} margin={{ top: 10, right: 30, left: 0, bottom: 0 }}>
              <defs>
                <linearGradient id="colorValue" x1="0" y1="0" x2="0" y2="1">
                  <stop offset="5%" stopColor="#4f46e5" stopOpacity={0.3}/>
                  <stop offset="95%" stopColor="#4f46e5" stopOpacity={0}/>
                </linearGradient>
              </defs>
              <CartesianGrid strokeDasharray="3 3" vertical={false} stroke="#f3f4f6" />
              <XAxis dataKey="name" axisLine={false} tickLine={false} tick={{ fill: '#9ca3af', fontSize: 12, fontWeight: 600 }} dy={10} />
              <YAxis axisLine={false} tickLine={false} tick={{ fill: '#9ca3af', fontSize: 12, fontWeight: 600 }} />
              <Tooltip 
                contentStyle={{ borderRadius: '12px', border: 'none', boxShadow: '0 4px 6px -1px rgb(0 0 0 / 0.1)' }}
                cursor={{ stroke: '#e5e7eb', strokeWidth: 2 }}
              />
              <Area type="monotone" dataKey="value" stroke="#4f46e5" strokeWidth={3} fillOpacity={1} fill="url(#colorValue)" />
            </AreaChart>
          </ResponsiveContainer>
        </div>
      </div>
    </div>
  );
}

function StatCard({ icon, title, value, trend }: { icon: React.ReactNode, title: string, value: string | number, trend: string }) {
  return (
    <div className="bg-white p-5 rounded-2xl shadow-sm border border-gray-100 flex items-start gap-4 hover:shadow-md transition-shadow">
      <div className="p-3 bg-gray-50 rounded-xl">
        {icon}
      </div>
      <div>
        <p className="text-sm font-semibold text-gray-500">{title}</p>
        <h4 className="text-2xl font-black text-gray-800 tracking-tight mt-1">{value}</h4>
        <p className="text-xs font-medium text-gray-400 mt-2 flex items-center gap-1">
          <TrendingUp className="w-3 h-3" /> {trend}
        </p>
      </div>
    </div>
  );
}


