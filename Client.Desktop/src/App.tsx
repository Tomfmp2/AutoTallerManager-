import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import Login from './pages/Login';
import Register from './pages/Register';
import RegisterGoogle from './pages/RegisterGoogle';
import ProtectedRoute from './components/ProtectedRoute';
import ClientLayout from './layouts/ClientLayout';
import DashboardClient from './pages/client/DashboardClient';
import MisVehiculos from './pages/client/MisVehiculos';
import HistorialCitas from './pages/client/HistorialCitas';
import Configuracion from './pages/client/Configuracion';
import AdminLayout from './layouts/AdminLayout';
import AdminDashboard from './pages/admin/AdminDashboard';
import AdminUsers from './pages/admin/AdminUsers';
import AdminVehicles from './pages/admin/AdminVehicles';
import AdminAppointments from './pages/admin/AdminAppointments';
import { AdminInventory } from './pages/admin/AdminInventory';
import RecepcionLayout from './layouts/RecepcionLayout';
import RecepDashboard from './pages/recepcion/RecepDashboard';
import RecepAprobaciones from './pages/recepcion/RecepAprobaciones';
import MechanicLayout from './layouts/MechanicLayout';
import MechanicDashboard from './pages/mechanic/MechanicDashboard';
import RecepClientes from './pages/recepcion/RecepClientes';
import RecepVehiculos from './pages/recepcion/RecepVehiculos';
import RecepCitas from './pages/recepcion/RecepCitas';
import "./App.css";

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Navigate to="/login" replace />} />
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />
        <Route path="/register/google" element={<RegisterGoogle />} />

        {/* Rutas Privadas del Cliente */}
        <Route element={<ProtectedRoute allowedRoles={['Cliente', 'User']} />}>
          <Route path="/cliente" element={<ClientLayout />}>
            <Route index element={<Navigate to="dashboard" replace />} />
            <Route path="dashboard"     element={<DashboardClient />} />
            <Route path="vehiculos"     element={<MisVehiculos />} />
            <Route path="historial"     element={<HistorialCitas />} />
            <Route path="configuracion" element={<Configuracion />} />
          </Route>
        </Route>

        {/* Rutas Privadas del Mecánico */}
        <Route element={<ProtectedRoute allowedRoles={['Mecanico', 'Mechanic', 'Mecánico']} />}>
          <Route path="/mecanico" element={<MechanicLayout />}>
            <Route index element={<Navigate to="dashboard" replace />} />
            <Route path="dashboard" element={<MechanicDashboard />} />
            <Route path="config" element={<div className="p-8 animate-fade-in"><div className="bg-white p-8 rounded-2xl shadow-sm border border-gray-100 text-center"><h2 className="text-2xl font-bold text-gray-800 mb-2">Configuración del Mecánico</h2><p className="text-gray-500">Este módulo se encuentra actualmente en desarrollo. Aquí podrás cambiar tu contraseña y preferencias de notificaciones.</p></div></div>} />
          </Route>
        </Route>

        {/* Rutas Privadas del Administrador */}
        <Route element={<ProtectedRoute allowedRoles={['Administrador', 'Admin']} />}>
          <Route path="/admin" element={<AdminLayout />}>
            <Route index element={<Navigate to="dashboard" replace />} />
            <Route path="dashboard"     element={<AdminDashboard />} />
            <Route path="usuarios"      element={<AdminUsers />} />
            <Route path="vehiculos"     element={<AdminVehicles />} />
            <Route path="Revisiones"         element={<AdminAppointments />} />
            <Route path="inventario"    element={<AdminInventory />} />
            <Route path="configuracion" element={<div className="p-8">Configuración (En Desarrollo)</div>} />
          </Route>
        </Route>

        {/* Rutas Privadas de Recepción */}
        <Route element={<ProtectedRoute allowedRoles={['Recepcionista', 'Administrador', 'Admin']} />}>
          <Route path="/recepcion" element={<RecepcionLayout />}>
            <Route index element={<Navigate to="dashboard" replace />} />
            <Route path="dashboard"     element={<RecepDashboard />} />
            <Route path="aprobaciones"  element={<RecepAprobaciones />} />
            <Route path="clientes"      element={<RecepClientes />} />
            <Route path="vehiculos"     element={<RecepVehiculos />} />
            <Route path="Revisiones"         element={<RecepCitas />} />
          </Route>
        </Route>

      </Routes>
    </BrowserRouter>
  );
}

export default App;


